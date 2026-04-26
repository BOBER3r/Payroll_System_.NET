using System;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using PayrollSystem.Core.Models;
using PayrollSystem.Web.App_Start;

namespace PayrollSystem.Web
{
    public partial class TaxBrackets : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        private void BindGrid()
        {
            gvBrackets.DataSource = RepositoryFactory.TaxBrackets.GetAll();
            gvBrackets.DataBind();
        }

        protected void gvBrackets_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvBrackets.EditIndex = e.NewEditIndex;
            BindGrid();
        }

        protected void gvBrackets_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvBrackets.EditIndex = -1;
            BindGrid();
        }

        protected void gvBrackets_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int id = (int)gvBrackets.DataKeys[e.RowIndex].Value;
            var row = gvBrackets.Rows[e.RowIndex];

            var txtLower = (TextBox)row.FindControl("txtLower");
            var txtUpper = (TextBox)row.FindControl("txtUpper");
            var txtRate  = (TextBox)row.FindControl("txtRate");

            // Parse LowerBound
            decimal lowerBound;
            if (!decimal.TryParse(txtLower.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out lowerBound))
            {
                ShowMessage("Lower bound must be a valid decimal number.", "error");
                return;
            }

            // Parse UpperBound (empty = null = +infinity)
            decimal? upperBound = null;
            if (!string.IsNullOrWhiteSpace(txtUpper.Text))
            {
                decimal upper;
                if (!decimal.TryParse(txtUpper.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out upper))
                {
                    ShowMessage("Upper bound must be a valid decimal number or left blank.", "error");
                    return;
                }
                upperBound = upper;
            }

            // Parse Rate entered as a percent (e.g. 10 means 0.10 stored)
            decimal ratePercent;
            if (!decimal.TryParse(txtRate.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out ratePercent))
            {
                ShowMessage("Rate must be a valid number (e.g. 10 for 10%).", "error");
                return;
            }
            decimal rate = ratePercent / 100m;

            // SC9 Validation — three explicit guards
            if (lowerBound < 0m)
            {
                ShowMessage("Lower bound must be non-negative (>= 0).", "error");
                return;
            }

            if (rate < 0m || rate > 1m)
            {
                ShowMessage("Rate must be between 0% and 100% (stored as 0 to 1).", "error");
                return;
            }

            if (upperBound != null && upperBound <= lowerBound)
            {
                ShowMessage("Upper bound must be greater than lower bound, or left blank for +infinity.", "error");
                return;
            }

            // All validations passed — persist
            RepositoryFactory.TaxBrackets.Update(new TaxBracket
            {
                Id         = id,
                LowerBound = lowerBound,
                UpperBound = upperBound,
                Rate       = rate
            });

            gvBrackets.EditIndex = -1;
            BindGrid();
            ShowMessage("Saved.", "info");
        }

        private void ShowMessage(string text, string cssClass)
        {
            lblMessage.Text     = Server.HtmlEncode(text);
            lblMessage.CssClass = cssClass;
            lblMessage.Visible  = true;
        }
    }
}
