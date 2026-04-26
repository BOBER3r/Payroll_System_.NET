<%@ Page Title="Home" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
         CodeBehind="Default.aspx.cs" Inherits="PayrollSystem.Web.Default" %>
<asp:Content ID="MainBody" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Welcome</h2>
    <p>Use the navigation above to manage employees and run payroll.</p>
    <ul>
        <li><a href="<%= ResolveUrl("~/Employees.aspx") %>">Manage Employees</a></li>
    </ul>
</asp:Content>
