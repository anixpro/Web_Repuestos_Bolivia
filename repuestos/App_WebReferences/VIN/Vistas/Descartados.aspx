<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Descartados.aspx.cs" Inherits="Vistas_Descartados" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
        ConnectionString="<%$ ConnectionStrings:skbergeConnectionString %>" >
    </asp:SqlDataSource>
    <asp:GridView ID="GridViewDescartados" runat="server" AutoGenerateColumns="False" BackColor="White"
            BorderColor="#999999" BorderStyle="None" BorderWidth="1px" CellPadding="3" AllowPaging="true" AllowSorting="true"
            Font-Names="Tahoma" Font-Size="XX-Small" GridLines="Vertical" Width="100%" DataSourceID="SqlDataSource1">
            <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
            <Columns>
                <asp:BoundField DataField="id_pedido" HeaderText="Pedido N°" ReadOnly="True" />
                <asp:BoundField DataField="codigo_rep" HeaderText="Código repuesto" ReadOnly="True" />
                <asp:BoundField DataField="cantidad" HeaderText="Cantidad" ReadOnly="True" />
                <asp:BoundField DataField="detalle_rep" HeaderText="Descaripción repuesto" ReadOnly="True" />
            </Columns>
            <RowStyle BackColor="#EEEEEE" ForeColor="Black" Font-Names="Tahoma" Font-Size="8pt" />
            <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
            <HeaderStyle BackColor="#000084" Font-Bold="True" Font-Names="Tahoma" Font-Size="XX-Small"
                ForeColor="White" />
            <AlternatingRowStyle BackColor="Gainsboro" />
        </asp:GridView>
</asp:Content>

