<%@ Page Title="Payroll" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
         CodeBehind="Payroll.aspx.cs" Inherits="PayrollSystem.Web.Payroll" %>
<asp:Content ID="MainBody" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Payroll</h2>

    <asp:Label ID="lblMessage" runat="server" Visible="false" />

    <asp:ValidationSummary ID="vsErrors" runat="server" CssClass="error" DisplayMode="BulletList" />

    <div class="form-row">
        <label for="<%= ddlEmployee.ClientID %>">Employee</label>
        <asp:DropDownList ID="ddlEmployee" runat="server"
                          AutoPostBack="true"
                          OnSelectedIndexChanged="ddlEmployee_SelectedIndexChanged" />
    </div>

    <div class="form-row">
        <label>Base Hourly Rate</label>
        <asp:Label ID="lblBaseRate" runat="server" Text="" />
    </div>

    <div class="form-row">
        <label for="<%= txtPeriodStart.ClientID %>">Period Start</label>
        <asp:TextBox ID="txtPeriodStart" runat="server" TextMode="Date" />
        <asp:RequiredFieldValidator ID="rfvPeriodStart" runat="server"
            ControlToValidate="txtPeriodStart" Display="Dynamic"
            ErrorMessage="Period start date is required." Text="*" CssClass="error" />
    </div>

    <div class="form-row">
        <label for="<%= txtPeriodEnd.ClientID %>">Period End</label>
        <asp:TextBox ID="txtPeriodEnd" runat="server" TextMode="Date" />
        <asp:RequiredFieldValidator ID="rfvPeriodEnd" runat="server"
            ControlToValidate="txtPeriodEnd" Display="Dynamic"
            ErrorMessage="Period end date is required." Text="*" CssClass="error" />
        <asp:CompareValidator ID="cmpPeriodOrder" runat="server"
            ControlToValidate="txtPeriodEnd" ControlToCompare="txtPeriodStart"
            Type="Date" Operator="GreaterThanEqual" Display="Dynamic"
            ErrorMessage="Period end date must be on or after the start date." Text="*" CssClass="error" />
    </div>

    <div class="form-row">
        <label for="<%= txtRegularHours.ClientID %>">Regular Hours</label>
        <asp:TextBox ID="txtRegularHours" runat="server"
            TextMode="Number" min="0" max="744" step="0.5" placeholder="0" />
        <asp:RequiredFieldValidator ID="rfvRegularHours" runat="server"
            ControlToValidate="txtRegularHours" Display="Dynamic"
            ErrorMessage="Regular hours are required." Text="*" CssClass="error" />
        <asp:RangeValidator ID="rngRegularHours" runat="server"
            ControlToValidate="txtRegularHours" Type="Double"
            MinimumValue="0" MaximumValue="744" Display="Dynamic"
            ErrorMessage="Regular hours must be between 0 and 744." Text="*" CssClass="error" />
    </div>

    <div class="form-row">
        <label for="<%= txtOvertimeHours.ClientID %>">Overtime Hours</label>
        <asp:TextBox ID="txtOvertimeHours" runat="server"
            TextMode="Number" min="0" max="200" step="0.5" placeholder="0" />
        <asp:RequiredFieldValidator ID="rfvOvertimeHours" runat="server"
            ControlToValidate="txtOvertimeHours" Display="Dynamic"
            ErrorMessage="Overtime hours are required." Text="*" CssClass="error" />
        <asp:RangeValidator ID="rngOvertime" runat="server"
            ControlToValidate="txtOvertimeHours" Type="Double"
            MinimumValue="0" MaximumValue="200" Display="Dynamic"
            ErrorMessage="Overtime hours must be between 0 and 200." Text="*" CssClass="error" />
    </div>

    <div class="form-row">
        <label for="<%= txtOtherDeductions.ClientID %>">Other Deductions</label>
        <asp:TextBox ID="txtOtherDeductions" runat="server"
            TextMode="Number" min="0" step="0.01" placeholder="0" Text="0" />
        <asp:RequiredFieldValidator ID="rfvOtherDeductions" runat="server"
            ControlToValidate="txtOtherDeductions" Display="Dynamic"
            ErrorMessage="Other deductions are required (use 0 if none)." Text="*" CssClass="error" />
        <asp:CompareValidator ID="cmpDeductionsNonNeg" runat="server"
            ControlToValidate="txtOtherDeductions" Type="Double"
            Operator="GreaterThanEqual" ValueToCompare="0" Display="Dynamic"
            ErrorMessage="Other deductions must be a non-negative number." Text="*" CssClass="error" />
    </div>

    <div class="form-actions">
        <asp:Button ID="btnCalculate" runat="server" Text="Calculate"
                    OnClick="Calculate_Click" CssClass="btn primary" />
        <asp:Button ID="btnSave" runat="server" Text="Save"
                    OnClick="Save_Click" Enabled="false"
                    CausesValidation="false" CssClass="btn" />
    </div>

    <asp:Panel ID="pnlSummary" runat="server" Visible="false">
        <h3>Calculation Summary</h3>
        <table class="data-table">
            <tr><th>Gross Pay</th>           <td class="num"><asp:Label ID="lblGross"   runat="server" /></td></tr>
            <tr><th>Social Contribution</th> <td class="num"><asp:Label ID="lblSocial"  runat="server" /></td></tr>
            <tr><th>Taxable Income</th>      <td class="num"><asp:Label ID="lblTaxable" runat="server" /></td></tr>
            <tr><th>Income Tax</th>          <td class="num"><asp:Label ID="lblTax"     runat="server" /></td></tr>
            <tr><th>Other Deductions</th>    <td class="num"><asp:Label ID="lblOther"   runat="server" /></td></tr>
            <tr><th>Net Pay</th>             <td class="num"><asp:Label ID="lblNet"     runat="server" /></td></tr>
        </table>
    </asp:Panel>

</asp:Content>
