using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using PayrollSystem.Web.App_Start;

namespace PayrollSystem.Web
{
    public partial class Employees : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                BindGrid();
        }

        private void BindGrid()
        {
            gvEmployees.DataSource = RepositoryFactory.Employees.GetAll();
            gvEmployees.DataBind();
        }

        protected void gvEmployees_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "DeleteEmployee") return;

            int id;
            if (!int.TryParse(e.CommandArgument as string, out id)) return;

            try
            {
                RepositoryFactory.Employees.Delete(id);
                ShowMessage("Employee deleted.", "info");
            }
            catch (InvalidOperationException ex)
            {
                // Sprint 2 contract: thrown when employee has payroll records (verbatim message)
                ShowMessage(ex.Message, "error");
            }

            BindGrid();
        }

        private void ShowMessage(string text, string cssClass)
        {
            lblMessage.Text = Server.HtmlEncode(text);
            lblMessage.CssClass = cssClass;
            lblMessage.Visible = true;
        }
    }
}
