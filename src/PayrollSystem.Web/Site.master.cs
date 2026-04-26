using System;
using System.Web;
using System.Web.UI;

namespace PayrollSystem.Web
{
    public partial class Site : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string path = Request.AppRelativeCurrentExecutionFilePath ?? string.Empty;
            // Case-insensitive match against the active page; "active" CSS class highlights it.
            ApplyActive(navHome,        path, "~/Default.aspx");
            ApplyActive(navEmployees,   path, "~/Employees.aspx");
            ApplyActive(navEmployees,   path, "~/EmployeeEdit.aspx"); // EmployeeEdit also lights the Employees tab
            ApplyActive(navPayroll,     path, "~/Payroll.aspx");
            ApplyActive(navEmployees,   path, "~/EmployeeHistory.aspx"); // History also lights the Employees tab
            ApplyActive(navReports,     path, "~/Reports.aspx");
            ApplyActive(navTaxBrackets, path, "~/TaxBrackets.aspx");
        }

        private static void ApplyActive(System.Web.UI.HtmlControls.HtmlAnchor anchor, string currentPath, string targetPath)
        {
            if (string.Equals(currentPath, targetPath, StringComparison.OrdinalIgnoreCase))
            {
                string existing = anchor.Attributes["class"];
                anchor.Attributes["class"] = string.IsNullOrEmpty(existing) ? "active" : existing + " active";
            }
        }
    }
}
