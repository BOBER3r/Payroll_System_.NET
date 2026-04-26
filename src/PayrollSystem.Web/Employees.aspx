<%@ Page Title="Employees" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
         CodeBehind="Employees.aspx.cs" Inherits="PayrollSystem.Web.Employees" %>
<asp:Content ID="MainBody" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Employees</h2>

    <p>
        <asp:HyperLink ID="lnkAdd" runat="server" CssClass="btn"
                       NavigateUrl="~/EmployeeEdit.aspx">Add Employee</asp:HyperLink>
    </p>

    <asp:Label ID="lblMessage" runat="server" Visible="false" />

    <asp:GridView ID="gvEmployees" runat="server"
                  AutoGenerateColumns="false"
                  DataKeyNames="Id"
                  CssClass="data-table"
                  OnRowCommand="gvEmployees_RowCommand"
                  EmptyDataText="No employees yet.">
        <Columns>
            <asp:BoundField DataField="FullName"       HeaderText="Name" />
            <asp:BoundField DataField="Position"       HeaderText="Position" />
            <asp:BoundField DataField="BaseHourlyRate" HeaderText="Base Rate"
                            DataFormatString="{0:N2}" ItemStyle-CssClass="num" />
            <asp:TemplateField HeaderText="Actions">
                <ItemTemplate>
                    <asp:HyperLink ID="lnkEdit" runat="server"
                                   NavigateUrl='<%# "~/EmployeeEdit.aspx?id=" + Eval("Id") %>'>Edit</asp:HyperLink>
                    &nbsp;|&nbsp;
                    <asp:HyperLink ID="lnkHistory" runat="server"
                                   NavigateUrl='<%# "~/EmployeeHistory.aspx?id=" + Eval("Id") %>'>History</asp:HyperLink>
                    &nbsp;|&nbsp;
                    <asp:LinkButton ID="btnDelete" runat="server"
                                    CommandName="DeleteEmployee"
                                    CommandArgument='<%# Eval("Id") %>'
                                    OnClientClick="return confirm('Delete this employee?');">Delete</asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>
