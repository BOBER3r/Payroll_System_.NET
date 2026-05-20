namespace PayrollSystem.Web
{
    public partial class Payroll
    {
        protected global::System.Web.UI.WebControls.Label                  lblMessage;
        protected global::System.Web.UI.WebControls.ValidationSummary      vsErrors;
        protected global::System.Web.UI.WebControls.DropDownList           ddlEmployee;
        protected global::System.Web.UI.WebControls.Label                  lblBaseRate;
        protected global::System.Web.UI.WebControls.TextBox                txtPeriodStart;
        protected global::System.Web.UI.WebControls.RequiredFieldValidator rfvPeriodStart;
        protected global::System.Web.UI.WebControls.TextBox                txtPeriodEnd;
        protected global::System.Web.UI.WebControls.RequiredFieldValidator rfvPeriodEnd;
        protected global::System.Web.UI.WebControls.CompareValidator       cmpPeriodOrder;
        protected global::System.Web.UI.WebControls.TextBox                txtRegularHours;
        protected global::System.Web.UI.WebControls.RequiredFieldValidator rfvRegularHours;
        protected global::System.Web.UI.WebControls.RangeValidator         rngRegularHours;
        protected global::System.Web.UI.WebControls.TextBox                txtOvertimeHours;
        protected global::System.Web.UI.WebControls.RequiredFieldValidator rfvOvertimeHours;
        protected global::System.Web.UI.WebControls.RangeValidator         rngOvertime;
        protected global::System.Web.UI.WebControls.TextBox                txtOtherDeductions;
        protected global::System.Web.UI.WebControls.RequiredFieldValidator rfvOtherDeductions;
        protected global::System.Web.UI.WebControls.CompareValidator       cmpDeductionsNonNeg;
        protected global::System.Web.UI.WebControls.Button                 btnCalculate;
        protected global::System.Web.UI.WebControls.Button                 btnSave;
        protected global::System.Web.UI.WebControls.Panel                  pnlSummary;
        protected global::System.Web.UI.WebControls.Label                  lblGross;
        protected global::System.Web.UI.WebControls.Label                  lblSocial;
        protected global::System.Web.UI.WebControls.Label                  lblTaxable;
        protected global::System.Web.UI.WebControls.Label                  lblTax;
        protected global::System.Web.UI.WebControls.Label                  lblOther;
        protected global::System.Web.UI.WebControls.Label                  lblNet;
    }
}
