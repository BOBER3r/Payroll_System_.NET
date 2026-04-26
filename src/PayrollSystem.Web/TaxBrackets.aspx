<%@ Page Title="Tax Brackets" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
         CodeBehind="TaxBrackets.aspx.cs" Inherits="PayrollSystem.Web.TaxBrackets" %>
<asp:Content ID="MainBody" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Tax Brackets</h2>

    <asp:Label ID="lblMessage" runat="server" Visible="false" />

    <asp:GridView ID="gvBrackets" runat="server"
                  AutoGenerateColumns="false"
                  AutoGenerateEditButton="true"
                  DataKeyNames="Id"
                  CssClass="data-table"
                  OnRowEditing="gvBrackets_RowEditing"
                  OnRowCancelingEdit="gvBrackets_RowCancelingEdit"
                  OnRowUpdating="gvBrackets_RowUpdating">
        <Columns>
            <asp:TemplateField HeaderText="Lower">
                <ItemTemplate>
                    <%# ((decimal)Eval("LowerBound")).ToString("N2", System.Globalization.CultureInfo.InvariantCulture) %>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtLower" runat="server"
                        Text='<%# ((decimal)Eval("LowerBound")).ToString("0.##", System.Globalization.CultureInfo.InvariantCulture) %>' />
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Upper">
                <ItemTemplate>
                    <%# Eval("UpperBound") == null ? "" : ((decimal)Eval("UpperBound")).ToString("N2", System.Globalization.CultureInfo.InvariantCulture) %>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtUpper" runat="server"
                        Text='<%# Eval("UpperBound") == null ? "" : ((decimal)Eval("UpperBound")).ToString("0.##", System.Globalization.CultureInfo.InvariantCulture) %>' />
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Rate">
                <ItemTemplate>
                    <%# ((decimal)Eval("Rate")).ToString("P2", System.Globalization.CultureInfo.InvariantCulture) %>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtRate" runat="server"
                        Text='<%# (((decimal)Eval("Rate")) * 100m).ToString("0.##", System.Globalization.CultureInfo.InvariantCulture) %>' />
                </EditItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

</asp:Content>
