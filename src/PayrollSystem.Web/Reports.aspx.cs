using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PayrollSystem.Core.Models;
using PayrollSystem.Core.Services;
using PayrollSystem.Web.App_Start;

namespace PayrollSystem.Web
{
    public partial class Reports : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindEmployeeFilter();
            }
        }

        private void BindEmployeeFilter()
        {
            ddlEmployeeFilter.Items.Clear();
            ddlEmployeeFilter.DataTextField  = "FullName";
            ddlEmployeeFilter.DataValueField = "Id";
            ddlEmployeeFilter.DataSource     = RepositoryFactory.Employees.GetAll();
            ddlEmployeeFilter.DataBind();
            ddlEmployeeFilter.Items.Insert(0, new ListItem("All Employees", ""));
        }

        protected void btnApply_Click(object sender, EventArgs e)
        {
            int? empId = null;
            int parsedId;
            if (!string.IsNullOrEmpty(ddlEmployeeFilter.SelectedValue)
                && int.TryParse(ddlEmployeeFilter.SelectedValue, out parsedId))
            {
                empId = parsedId;
            }

            DateTime? fromDate = null;
            DateTime parsedFrom;
            if (!string.IsNullOrWhiteSpace(txtFromDate.Text)
                && DateTime.TryParse(txtFromDate.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedFrom))
            {
                fromDate = parsedFrom;
            }

            DateTime? toDate = null;
            DateTime parsedTo;
            if (!string.IsNullOrWhiteSpace(txtToDate.Text)
                && DateTime.TryParse(txtToDate.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedTo))
            {
                toDate = parsedTo;
            }

            // Persist filter in ViewState so btnExport can re-run the same query
            ViewState["ReportFilterEmployeeId"] = empId;
            ViewState["ReportFilterFrom"]        = fromDate;
            ViewState["ReportFilterTo"]          = toDate;

            BindGrid(empId, fromDate, toDate);
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            int?      empId    = ViewState["ReportFilterEmployeeId"] as int?;
            DateTime? fromDate = ViewState["ReportFilterFrom"] as DateTime?;
            DateTime? toDate   = ViewState["ReportFilterTo"]   as DateTime?;

            var records       = RepositoryFactory.Payroll.Search(empId, fromDate, toDate);
            var employeeNames = BuildEmployeeNameDictionary();

            var csv = CsvExporter.Export(records, employeeNames);

            Response.Clear();
            Response.ContentType = "text/csv";
            Response.AddHeader("Content-Disposition",
                "attachment; filename=payroll-report-" + DateTime.UtcNow.ToString("yyyyMMdd") + ".csv");
            Response.Write(csv);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        private void BindGrid(int? empId, DateTime? fromDate, DateTime? toDate)
        {
            var records       = RepositoryFactory.Payroll.Search(empId, fromDate, toDate);
            var employeeNames = BuildEmployeeNameDictionary();

            var rows = records.Select(r => new
            {
                r.Id,
                EmployeeName = employeeNames.ContainsKey(r.EmployeeId) ? employeeNames[r.EmployeeId] : string.Empty,
                r.PeriodLabel,
                r.RegularHours,
                r.OvertimeHours,
                r.GrossPay,
                r.IncomeTax,
                r.NetPay,
                r.CalculatedAt
            }).ToList();

            gvReport.DataSource = rows;
            gvReport.DataBind();
        }

        private static Dictionary<int, string> BuildEmployeeNameDictionary()
        {
            var dict = new Dictionary<int, string>();
            foreach (var emp in RepositoryFactory.Employees.GetAll())
            {
                dict[emp.Id] = emp.FullName;
            }
            return dict;
        }

        private void ShowMessage(string text, string cssClass)
        {
            lblMessage.Text     = Server.HtmlEncode(text);
            lblMessage.CssClass = cssClass;
            lblMessage.Visible  = true;
        }
    }
}
