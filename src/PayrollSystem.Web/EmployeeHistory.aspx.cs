using System;
using System.Globalization;
using System.Web.UI;
using PayrollSystem.Web.App_Start;

namespace PayrollSystem.Web
{
    public partial class EmployeeHistory : Page
    {
        private int? EmployeeId
        {
            get
            {
                int id;
                string raw = Request.QueryString["id"];
                if (!string.IsNullOrEmpty(raw) && int.TryParse(raw, out id))
                    return id;
                return null;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!EmployeeId.HasValue)
                {
                    Response.Redirect("Employees.aspx");
                    return;
                }

                var employee = RepositoryFactory.Employees.GetById(EmployeeId.Value);
                if (employee == null)
                {
                    Response.Redirect("Employees.aspx");
                    return;
                }

                lblEmployeeHeader.Text = string.Format(
                    "{0} — {1} — Base Rate: {2}",
                    employee.FullName,
                    employee.Position,
                    employee.BaseHourlyRate.ToString("N2", CultureInfo.InvariantCulture));

                var records = RepositoryFactory.Payroll.GetByEmployee(EmployeeId.Value);

                gvHistory.DataSource = records;
                gvHistory.DataBind();

                decimal totalGross = 0m;
                decimal totalTax   = 0m;
                decimal totalNet   = 0m;

                foreach (var r in records)
                {
                    totalGross += r.GrossPay;
                    totalTax   += r.IncomeTax;
                    totalNet   += r.NetPay;
                }

                lblTotalGross.Text = totalGross.ToString("N2", CultureInfo.InvariantCulture);
                lblTotalTax.Text   = totalTax.ToString("N2", CultureInfo.InvariantCulture);
                lblTotalNet.Text   = totalNet.ToString("N2", CultureInfo.InvariantCulture);
            }
        }
    }
}
