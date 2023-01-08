<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="SucursalesLista.aspx.cs" Inherits="Vistas_SucursalesLista" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="titulo">
        Lista de sucursales
    </div>
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <asp:SqlDataSource ID="SqlDataSourceConcesionarios" runat="server" 
        ConnectionString="<%$ ConnectionStrings:skbergeConnectionString %>" 
        SelectCommand="SELECT [numeroFactura], [nombreConcesionario] FROM [concesionario] order by nombreConcesionario">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDataSourceSucursales" runat="server" 
        ConnectionString="<%$ ConnectionStrings:skbergeConnectionString %>" 
        SelectCommand="SELECT [idSucursal], [numeroFactura], [direccionSucursal], [shipCode] FROM [sucursal] WHERE ([numeroFactura] = @numeroFactura)">
        <SelectParameters>
            <asp:ControlParameter ControlID="DropDownList1" Name="numeroFactura" 
                PropertyName="SelectedValue" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>
    <div class="contenidoCentral">
        Concesionario:&nbsp;
        <asp:DropDownList ID="DropDownList1" runat="server" 
            DataSourceID="SqlDataSourceConcesionarios" DataTextField="nombreConcesionario" 
            DataValueField="numeroFactura" AutoPostBack="True">
        </asp:DropDownList>
        <br />
        <br />
        <asp:GridView ID="GridView1" runat="server" AllowPaging="True" 
            AllowSorting="True" AutoGenerateColumns="False" CellPadding="4" 
            DataKeyNames="idSucursal" DataSourceID="SqlDataSourceSucursales" 
            EmptyDataText="No hay sucursal para el concesionario seleccionado" 
            ForeColor="#333333" GridLines="None" CssClass="grillaSucursales"
            OnRowCommand="gridSucursales_RowCommand" >
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            <Columns>
                <asp:BoundField DataField="idSucursal" HeaderText="idSucursal"
                    HeaderStyle-CssClass="invi" ItemStyle-CssClass="invi" ControlStyle-CssClass="invi"
                    InsertVisible="False" ReadOnly="True" SortExpression="idSucursal" />
                <asp:BoundField DataField="numeroFactura" HeaderText="Código Concesionario (SAP)" 
                    SortExpression="numeroFactura" />
                <asp:BoundField DataField="direccionSucursal" HeaderText="Dirección" 
                    SortExpression="direccionSucursal" />
                <asp:BoundField DataField="shipCode" HeaderText="Destinatario Mercancía" 
                    SortExpression="shipCode" />
                <asp:ButtonField ButtonType="Image" ImageUrl="../img/edit.png" HeaderText="Editar" CommandName="editar"></asp:ButtonField>
                <asp:CommandField HeaderText="Quitar" ShowSelectButton="true" ButtonType="Image"  SelectImageUrl="~/img/gtk-no.png" ControlStyle-Width="15px" ControlStyle-Height="15px" ItemStyle-HorizontalAlign="Center" />
            </Columns>
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <PagerSettings Mode="NextPreviousFirstLast" />
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <SortedAscendingCellStyle BackColor="#E9E7E2" />
            <SortedAscendingHeaderStyle BackColor="#506C8C" />
            <SortedDescendingCellStyle BackColor="#FFFDF8" />
            <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
        </asp:GridView>
        <br />
        <asp:HyperLink ID="hlkSucursal" runat="server" ClientIDMode="Static" 
            NavigateUrl="~/Vistas/SucursalInsert.aspx">Nueva Sucursal</asp:HyperLink><br />
        <asp:HyperLink ID="HyperLink1" runat="server" ClientIDMode="Static" 
            NavigateUrl="~/Vistas/MantenedorConcesionario.aspx">Volver a Concesionarios</asp:HyperLink>
    </div>    
</asp:Content>

