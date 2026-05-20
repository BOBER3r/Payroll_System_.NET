<%@ Page Title="Employee" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
         CodeBehind="EmployeeEdit.aspx.cs" Inherits="PayrollSystem.Web.EmployeeEdit" %>
<asp:Content ID="MainBody" ContentPlaceHolderID="MainContent" runat="server">
    <h2><asp:Literal ID="litTitle" runat="server" Text="Add Employee" /></h2>

    <asp:ValidationSummary ID="vsErrors" runat="server" CssClass="error" DisplayMode="BulletList" />

    <div class="form-row">
        <label for="<%= txtFirstName.ClientID %>">First Name</label>
        <asp:TextBox ID="txtFirstName" runat="server" />
        <asp:RequiredFieldValidator ID="rfvFirstName" runat="server"
            ControlToValidate="txtFirstName" Display="Dynamic"
            ErrorMessage="First name is required." Text="*" />
    </div>

    <div class="form-row">
        <label for="<%= txtLastName.ClientID %>">Last Name</label>
        <asp:TextBox ID="txtLastName" runat="server" />
        <asp:RequiredFieldValidator ID="rfvLastName" runat="server"
            ControlToValidate="txtLastName" Display="Dynamic"
            ErrorMessage="Last name is required." Text="*" />
    </div>

    <div class="form-row">
        <label for="<%= txtPosition.ClientID %>">Position</label>
        <asp:TextBox ID="txtPosition" runat="server" />
        <asp:RequiredFieldValidator ID="rfvPosition" runat="server"
            ControlToValidate="txtPosition" Display="Dynamic"
            ErrorMessage="Position is required." Text="*" />
    </div>

    <div class="form-row">
        <label for="<%= txtBaseRate.ClientID %>">Base Hourly Rate</label>
        <asp:TextBox ID="txtBaseRate" runat="server"
            TextMode="Number" min="0.01" max="10000" step="0.01" placeholder="0.00" />
        <asp:RequiredFieldValidator ID="rfvBaseRate" runat="server"
            ControlToValidate="txtBaseRate" Display="Dynamic"
            ErrorMessage="Base rate is required." Text="*" />
        <asp:RangeValidator ID="rngBaseRate" runat="server"
            ControlToValidate="txtBaseRate" Type="Double"
            MinimumValue="0.01" MaximumValue="10000" Display="Dynamic"
            ErrorMessage="Base rate must be between 0.01 and 10000." Text="*" />
    </div>

    <div class="form-actions">
        <asp:Button ID="btnSave"   runat="server" Text="Save"   OnClick="btnSave_Click" CssClass="btn primary" />
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click"
                    CausesValidation="false" CssClass="btn" />
    </div>
</asp:Content>
