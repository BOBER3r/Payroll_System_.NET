using System;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using PayrollSystem.Core.Models;
using PayrollSystem.Core.Services;
using PayrollSystem.Web.App_Start;

namespace PayrollSystem.Web
{
    public partial class Payroll : Page
    {
        private readonly IPayrollCalculator _calculator = new PayrollCalculator();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindEmployees();
                btnSave.Enabled = false;
                pnlSummary.Visible = false;
            }
        }

        private void BindEmployees()
        {
            ddlEmployee.Items.Clear();
            ddlEmployee.DataSource = null;

            var active = new System.Collections.Generic.List<Employee>();
            foreach (var emp in RepositoryFactory.Employees.GetAll())
            {
                if (emp.IsActive) active.Add(emp);
            }

            ddlEmployee.DataTextField = "FullName";
            ddlEmployee.DataValueField = "Id";
            ddlEmployee.DataSource = active;
            ddlEmployee.DataBind();

            ddlEmployee.Items.Insert(0, new ListItem("-- select --", ""));
            lblBaseRate.Text = "";
        }

        protected void ddlEmployee_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id;
            if (!int.TryParse(ddlEmployee.SelectedValue, out id))
            {
                lblBaseRate.Text = "";
                return;
            }

            var employee = RepositoryFactory.Employees.GetById(id);
            lblBaseRate.Text = employee != null
                ? employee.BaseHourlyRate.ToString("N2", CultureInfo.InvariantCulture)
                : "";
        }

        protected void Calculate_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            int employeeId;
            if (!int.TryParse(ddlEmployee.SelectedValue, out employeeId))
            {
                ShowMessage("Please select an employee.", "error");
                return;
            }

            var employee = RepositoryFactory.Employees.GetById(employeeId);
            if (employee == null)
            {
                ShowMessage("Selected employee no longer exists.", "error");
                return;
            }

            decimal regular, overtime, other;
            if (!decimal.TryParse(txtRegularHours.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out regular)
             || !decimal.TryParse(txtOvertimeHours.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out overtime)
             || !decimal.TryParse(txtOtherDeductions.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out other))
            {
                ShowMessage("Hours and deductions must be valid decimal numbers.", "error");
                return;
            }

            var input = new PayrollInput
            {
                EmployeeId      = employeeId,
                PeriodLabel     = txtPeriodLabel.Text.Trim(),
                RegularHours    = regular,
                OvertimeHours   = overtime,
                BaseRate        = employee.BaseHourlyRate,
                OtherDeductions = other
            };

            var brackets = RepositoryFactory.TaxBrackets.GetAll();
            PayrollCalculationResult result = _calculator.Calculate(input, brackets);

            // Round-trip ALL inputs + results through ViewState so Save_Click rebuilds without recalc
            ViewState["EmployeeId"]         = input.EmployeeId;
            ViewState["PeriodLabel"]        = input.PeriodLabel;
            ViewState["RegularHours"]       = input.RegularHours;
            ViewState["OvertimeHours"]      = input.OvertimeHours;
            ViewState["BaseRate"]           = input.BaseRate;
            ViewState["OtherDeductions"]    = input.OtherDeductions;
            ViewState["GrossPay"]           = result.GrossPay;
            ViewState["SocialContribution"] = result.SocialContribution;
            ViewState["TaxableIncome"]      = result.TaxableIncome;
            ViewState["IncomeTax"]          = result.IncomeTax;
            ViewState["NetPay"]             = result.NetPay;

            lblGross.Text   = result.GrossPay.ToString("N2", CultureInfo.InvariantCulture);
            lblSocial.Text  = result.SocialContribution.ToString("N2", CultureInfo.InvariantCulture);
            lblTaxable.Text = result.TaxableIncome.ToString("N2", CultureInfo.InvariantCulture);
            lblTax.Text     = result.IncomeTax.ToString("N2", CultureInfo.InvariantCulture);
            lblOther.Text   = result.OtherDeductions.ToString("N2", CultureInfo.InvariantCulture);
            lblNet.Text     = result.NetPay.ToString("N2", CultureInfo.InvariantCulture);

            pnlSummary.Visible = true;
            btnSave.Enabled    = true;
            lblMessage.Visible = false;
        }

        protected void Save_Click(object sender, EventArgs e)
        {
            if (ViewState["GrossPay"] == null)
            {
                ShowMessage("Please run Calculate before saving.", "error");
                return;
            }

            var record = new PayrollRecord
            {
                EmployeeId         = (int)    ViewState["EmployeeId"],
                PeriodLabel        = (string) ViewState["PeriodLabel"],
                RegularHours       = (decimal)ViewState["RegularHours"],
                OvertimeHours      = (decimal)ViewState["OvertimeHours"],
                BaseRate           = (decimal)ViewState["BaseRate"],
                OtherDeductions    = (decimal)ViewState["OtherDeductions"],
                GrossPay           = (decimal)ViewState["GrossPay"],
                SocialContribution = (decimal)ViewState["SocialContribution"],
                TaxableIncome      = (decimal)ViewState["TaxableIncome"],
                IncomeTax          = (decimal)ViewState["IncomeTax"],
                NetPay             = (decimal)ViewState["NetPay"],
                CalculatedAt       = DateTime.UtcNow
            };

            int newId = RepositoryFactory.Payroll.Insert(record);

            // Reset form so user can enter a new payroll
            txtRegularHours.Text    = "";
            txtOvertimeHours.Text   = "";
            txtOtherDeductions.Text = "";
            txtPeriodLabel.Text     = "";
            pnlSummary.Visible      = false;
            btnSave.Enabled         = false;

            ViewState.Remove("EmployeeId");
            ViewState.Remove("PeriodLabel");
            ViewState.Remove("RegularHours");
            ViewState.Remove("OvertimeHours");
            ViewState.Remove("BaseRate");
            ViewState.Remove("OtherDeductions");
            ViewState.Remove("GrossPay");
            ViewState.Remove("SocialContribution");
            ViewState.Remove("TaxableIncome");
            ViewState.Remove("IncomeTax");
            ViewState.Remove("NetPay");

            BindEmployees();

            ShowMessage("Saved successfully (Id=" + newId + ").", "info");
        }

        private void ShowMessage(string text, string cssClass)
        {
            lblMessage.Text     = Server.HtmlEncode(text);
            lblMessage.CssClass = cssClass;
            lblMessage.Visible  = true;
        }
    }
}
