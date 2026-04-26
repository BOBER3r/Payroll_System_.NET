<%@ Page Title="Employee History" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
         CodeBehind="EmployeeHistory.aspx.cs" Inherits="PayrollSystem.Web.EmployeeHistory" %>
<asp:Content ID="MainBody" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Employee History</h2>

    <p><asp:Label ID="lblEmployeeHeader" runat="server" /></p>

    <asp:GridView ID="gvHistory" runat="server"
                  AutoGenerateColumns="false"
                  CssClass="data-table"
                  EmptyDataText="No payroll history for this employee.">
        <Columns>
            <asp:BoundField DataField="CalculatedAt"  HeaderText="Date"    DataFormatString="{0:yyyy-MM-dd}" />
            <asp:BoundField DataField="PeriodLabel"   HeaderText="Period" />
            <asp:BoundField DataField="RegularHours"  HeaderText="Reg Hrs" DataFormatString="{0:N2}" ItemStyle-CssClass="num" />
            <asp:BoundField DataField="OvertimeHours" HeaderText="OT Hrs"  DataFormatString="{0:N2}" ItemStyle-CssClass="num" />
            <asp:BoundField DataField="GrossPay"      HeaderText="Gross"   DataFormatString="{0:N2}" ItemStyle-CssClass="num" />
            <asp:BoundField DataField="IncomeTax"     HeaderText="Tax"     DataFormatString="{0:N2}" ItemStyle-CssClass="num" />
            <asp:BoundField DataField="NetPay"        HeaderText="Net"     DataFormatString="{0:N2}" ItemStyle-CssClass="num" />
        </Columns>
    </asp:GridView>

    <table class="data-table" style="margin-top: 1rem;">
        <tr><th>Total Gross</th>    <td class="num"><asp:Label ID="lblTotalGross" runat="server" /></td></tr>
        <tr><th>Total Income Tax</th><td class="num"><asp:Label ID="lblTotalTax"   runat="server" /></td></tr>
        <tr><th>Total Net Pay</th>  <td class="num"><asp:Label ID="lblTotalNet"   runat="server" /></td></tr>
    </table>

</asp:Content>
