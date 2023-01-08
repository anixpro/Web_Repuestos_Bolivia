<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="MantenedorZona.aspx.cs" Inherits="mantenedorZona" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script language="javascript" type="text/javascript">
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <div class="center"><br/>
    <div class="avisoInsert">
    <asp:Label ID="lblAviso" runat="server" ClientIDMode="Static" ForeColor="#99CCFF" ></asp:Label>
    </div>
    <asp:Table ID="tblDatos" runat="server" ClientIDMode="Static">

        <asp:TableRow ID="TableRow2" runat="server" ClientIDMode="Static">
            <asp:TableCell ID="TableCell3" runat="server" ClientIDMode="Static"><asp:Label ID="lblZona" runat="server" ClientIDMode="Static" Text="Nueva zona"></asp:Label>
</asp:TableCell>
            <asp:TableCell ID="TableCell4" runat="server" ClientIDMode="Static">
            <asp:TextBox MaxLength="20" ID="txtZona" runat="server" ClientIDMode="Static"></asp:TextBox>
</asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow3" runat="server" ClientIDMode="Static">
            <asp:TableCell ID="TableCell5" runat="server" ClientIDMode="Static"></asp:TableCell>
            <asp:TableCell ID="TableCell6" runat="server" ClientIDMode="Static"><asp:Button ID="btnAgregar" runat="server" ClientIDMode="Static" Text="Agregar" onclick="btnAgregar_Click"/>
</asp:TableCell>
        </asp:TableRow>



    </asp:Table>
    </div>
    <div class="footDer">
    <asp:HyperLink ID="hlkSucursal" runat="server" ClientIDMode="Static" 
            NavigateUrl="~/Vistas/SucursalInsert.aspx">Ir a sucursal</asp:HyperLink><br/>
             <asp:HyperLink ID="HyperLink1" runat="server" ClientIDMode="Static" 
            NavigateUrl="~/Vistas/MantenedorConcesionarioInsert.aspx">Insertar nuevo Concesionario</asp:HyperLink>

    </div>
</asp:Content>

