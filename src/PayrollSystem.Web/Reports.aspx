<%@ Page Title="Reports" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
         CodeBehind="Reports.aspx.cs" Inherits="PayrollSystem.Web.Reports" %>
<asp:Content ID="MainBody" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Payroll Reports</h2>

    <asp:Label ID="lblMessage" runat="server" Visible="false" />

    <div class="form-row">
        <label for="<%= ddlEmployeeFilter.ClientID %>">Employee</label>
        <asp:DropDownList ID="ddlEmployeeFilter" runat="server" />
    </div>
    <div class="form-row">
        <label for="<%= txtFromDate.ClientID %>">From</label>
        <asp:TextBox ID="txtFromDate" runat="server" TextMode="Date" />
    </div>
    <div class="form-row">
        <label for="<%= txtToDate.ClientID %>">To</label>
        <asp:TextBox ID="txtToDate" runat="server" TextMode="Date" />
    </div>
    <div class="form-actions">
        <asp:Button ID="btnApply"  runat="server" Text="Apply"      OnClick="btnApply_Click"  CssClass="btn primary" />
        <asp:Button ID="btnExport" runat="server" Text="Export CSV" OnClick="btnExport_Click" CssClass="btn" />
    </div>

    <asp:GridView ID="gvReport" runat="server"
                  AutoGenerateColumns="false"
                  CssClass="data-table"
                  EmptyDataText="No payroll records match the filter.">
        <Columns>
            <asp:BoundField DataField="CalculatedAt"  HeaderText="Date"     DataFormatString="{0:yyyy-MM-dd}" />
            <asp:BoundField DataField="EmployeeName"  HeaderText="Employee" />
            <asp:BoundField DataField="PeriodLabel"   HeaderText="Period" />
            <asp:BoundField DataField="RegularHours"  HeaderText="Reg Hrs"  DataFormatString="{0:N2}" ItemStyle-CssClass="num" />
            <asp:BoundField DataField="OvertimeHours" HeaderText="OT Hrs"   DataFormatString="{0:N2}" ItemStyle-CssClass="num" />
            <asp:BoundField DataField="GrossPay"      HeaderText="Gross"    DataFormatString="{0:N2}" ItemStyle-CssClass="num" />
            <asp:BoundField DataField="IncomeTax"     HeaderText="Tax"      DataFormatString="{0:N2}" ItemStyle-CssClass="num" />
            <asp:BoundField DataField="NetPay"        HeaderText="Net"      DataFormatString="{0:N2}" ItemStyle-CssClass="num" />
        </Columns>
    </asp:GridView>

</asp:Content>
