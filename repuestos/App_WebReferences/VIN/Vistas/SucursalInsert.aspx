<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="SucursalInsert.aspx.cs" Inherits="Vistas_sucursalInsert" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <div class="center"><br/>
    <div class="avisoInsert">
    <asp:Label ID="lblAviso" runat="server" ClientIDMode="Static" ForeColor="#99CCFF" ></asp:Label>
    </div>
    <asp:Table ID="tblDatos" runat="server" ClientIDMode="Static">
        <asp:TableRow ID="TableRow1" runat="server" ClientIDMode="Static">
            <asp:TableCell ID="TableCell1" runat="server" ClientIDMode="Static">
                <asp:Label ID="lblConcesionario" runat="server" ClientIDMode="Static" Text="Concesionario"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell2" runat="server" ClientIDMode="Static">
                <asp:DropDownList ID="ddlConcesionario" runat="server" ClientIDMode="Static">
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow10" runat="server" ClientIDMode="Static">
            <asp:TableCell ID="TableCell19" runat="server" ClientIDMode="Static">
                <asp:Label ID="lblRegion" runat="server" ClientIDMode="Static" Text="Region"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell20" runat="server" ClientIDMode="Static">
                <asp:DropDownList ID="ddlRegion" runat="server" ClientIDMode="Static" AutoPostBack="true"></asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow11" runat="server" ClientIDMode="Static">
            <asp:TableCell ID="TableCell21" runat="server" ClientIDMode="Static">
                <asp:Label ID="lblProvincia" runat="server" ClientIDMode="Static" Text="Provincia"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell22" runat="server" ClientIDMode="Static">
            <asp:DropDownList ID="ddlProvincia" runat="server" ClientIDMode="Static" AutoPostBack="true"></asp:DropDownList>
        </asp:TableCell>
        </asp:TableRow>
            <asp:TableRow ID="TableRow2" runat="server" ClientIDMode="Static">
            <asp:TableCell ID="TableCell3" runat="server" ClientIDMode="Static"><asp:Label ID="lblComuna" runat="server" ClientIDMode="Static" Text="Comuna"></asp:Label>
        </asp:TableCell>
            <asp:TableCell ID="TableCell4" runat="server" ClientIDMode="Static">
            <asp:DropDownList ID="ddlComuna" runat="server" ClientIDMode="Static"></asp:DropDownList>
        </asp:TableCell>
        </asp:TableRow>   <asp:TableRow ID="TableRow3" runat="server" ClientIDMode="Static">
            <asp:TableCell ID="TableCell5" runat="server" ClientIDMode="Static"><asp:Label ID="lblNombre" runat="server" ClientIDMode="Static" Text="Nombre"></asp:Label>
        </asp:TableCell>
            <asp:TableCell ID="TableCell6" runat="server" ClientIDMode="Static">
            <asp:TextBox MaxLength="40" ID="txtNombre" runat="server" ClientIDMode="Static"></asp:TextBox>
        </asp:TableCell>
        </asp:TableRow> 
        <asp:TableRow ID="TableRow7" runat="server" ClientIDMode="Static">
            <asp:TableCell ID="TableCell13" runat="server" ClientIDMode="Static"><asp:Label ID="lblShipCode" runat="server" ClientIDMode="Static" Text="ShipCode"></asp:Label>
</asp:TableCell>
            <asp:TableCell ID="TableCell14" runat="server" ClientIDMode="Static">
            <asp:TextBox MaxLength="15" ID="txtShipCode" runat="server" ClientIDMode="Static"></asp:TextBox>
</asp:TableCell>
        </asp:TableRow>   
        <asp:TableRow ID="TableRow8" runat="server" ClientIDMode="Static">
            <asp:TableCell ID="TableCell15" runat="server" ClientIDMode="Static"><asp:Label ID="lblDireccion" runat="server" ClientIDMode="Static" Text="Direccion"></asp:Label>
</asp:TableCell>
            <asp:TableCell ID="TableCell16" runat="server" ClientIDMode="Static">
            <asp:TextBox MaxLength="70" ID="txtDireccion" runat="server" ClientIDMode="Static"></asp:TextBox>
</asp:TableCell>
        </asp:TableRow>        
 <asp:TableRow ID="TableRow9" runat="server" ClientIDMode="Static">
            <asp:TableCell ID="TableCell17" runat="server" ClientIDMode="Static">
</asp:TableCell>
            <asp:TableCell ID="TableCell18" runat="server" ClientIDMode="Static">
            <asp:Button ID="btnAceptar" runat="server" ClientIDMode="Static" Text="Aceptar" OnClick="btnAceptar_Click" />
</asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    &nbsp;</div>
    <div class="footDer">
    <asp:HyperLink ID="hlkSucursal" runat="server" ClientIDMode="Static" 
            NavigateUrl="~/Vistas/MantenedorConcesionario.aspx">Ir a Concesionario</asp:HyperLink><br/>

    </div>
</asp:Content>

