using System;
using System.Globalization;
using System.Web.UI;
using PayrollSystem.Core.Models;
using PayrollSystem.Web.App_Start;

namespace PayrollSystem.Web
{
    public partial class EmployeeEdit : Page
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
            if (IsPostBack) return;

            if (EmployeeId.HasValue)
            {
                var existing = RepositoryFactory.Employees.GetById(EmployeeId.Value);
                if (existing != null)
                {
                    litTitle.Text     = "Edit Employee";
                    txtFirstName.Text = existing.FirstName;
                    txtLastName.Text  = existing.LastName;
                    txtPosition.Text  = existing.Position;
                    txtBaseRate.Text  = existing.BaseHourlyRate.ToString("0.##", CultureInfo.InvariantCulture);
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            decimal rate = decimal.Parse(txtBaseRate.Text, NumberStyles.Number, CultureInfo.InvariantCulture);

            if (EmployeeId.HasValue)
            {
                var existing = RepositoryFactory.Employees.GetById(EmployeeId.Value);
                if (existing == null) { Response.Redirect("Employees.aspx"); return; }

                existing.FirstName      = txtFirstName.Text.Trim();
                existing.LastName       = txtLastName.Text.Trim();
                existing.Position       = txtPosition.Text.Trim();
                existing.BaseHourlyRate = rate;
                RepositoryFactory.Employees.Update(existing);
            }
            else
            {
                var employee = new Employee
                {
                    FirstName      = txtFirstName.Text.Trim(),
                    LastName       = txtLastName.Text.Trim(),
                    Position       = txtPosition.Text.Trim(),
                    BaseHourlyRate = rate,
                    IsActive       = true,
                    CreatedAt      = DateTime.UtcNow
                };
                RepositoryFactory.Employees.Insert(employee);
            }

            Response.Redirect("Employees.aspx");
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Employees.aspx");
        }
    }
}
