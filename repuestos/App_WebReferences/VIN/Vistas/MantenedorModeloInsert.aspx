<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="MantenedorModeloInsert.aspx.cs" Inherits="mantenedorModeloInsert" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script language="javascript" type="text/javascript">
        function validaTxtNombre() {
            document.getElementById('lblConcesionario').style.color = "#696969";
            cajaTexto = document.getElementById('txtNombre').value;
            mje_nom = '';
            if (cajaTexto == "") {
                mje_nom = 'debe llenar Nombre\n';
                document.getElementById('lblConcesionario').style.color = "red";
                document.getElementById('txtNombre').value = '';
            }
            return (mje_nom);
        }

        function obtenerExtencion() {

            document.getElementById('lblImagen').style.color = "#696969";
            var cadena = document.getElementById('fldImagen').value.split('.');
            mje_foto = '';
            if (cadena[1] != 'jpg') {
                mje_foto = "Formato de imagen solo puede ser:jpg\n";
                document.getElementById('lblImagen').style.color = "red";
                document.getElementById('fldImagen').value = '';
            }
            return (mje_foto);
        }

        function validaBlancos() {
            validaTxtNombre();
            obtenerExtencion();
            validaCheck();
            mensaje = '';
            mensaje = mje_nom + mje_foto + mje_check;
            if (mensaje != '') {
                window.alert(mensaje);
                return false;
            }
            else {
                return true;
            }
        }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
	  <div class="mantenedorConcesionariosIzquierda">
        <center><b><u>Mantención marcas</u></b></center>
        <ul>
            <li>
                <asp:HyperLink ID="HyperLink5" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/mantenedorMarcasInsert.aspx">Insertar marca</asp:HyperLink><br />    
            </li>
            <li>
                <asp:HyperLink ID="HyperLink6" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/mantenedorModelo.aspx">Consultar modelos</asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="HyperLink1" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/mantenedorModeloInsert.aspx">Insertar modelo</asp:HyperLink>
            </li>
        </ul>
    </div>
   <div class="center">
    <div class="avisoInsert">
    <asp:Label ID="lblAviso" runat="server" ClientIDMode="Static" ForeColor="#99CCFF" ></asp:Label>
     <br/>
     <br/>
    </div>
    <asp:Table ID="tblDatos" runat="server" ClientIDMode="Static">
        <asp:TableRow ID="TableRow1" runat="server" ClientIDMode="Static">
            <asp:TableCell ID="TableCell1" runat="server" ClientIDMode="Static"><asp:Label ID="lblNombreMarca" runat="server" ClientIDMode="Static" Text="Marca"></asp:Label>
</asp:TableCell>
            <asp:TableCell ID="TableCell2" runat="server" ClientIDMode="Static"><asp:DropDownList  id="ddlMarca" runat="server" ClientIDMode="Static"></asp:DropDownList>
</asp:TableCell>
        </asp:TableRow>
                <asp:TableRow ID="TableRow2" runat="server" ClientIDMode="Static">
            <asp:TableCell ID="TableCell3" runat="server" ClientIDMode="Static"><asp:Label ID="lblModelo" runat="server" ClientIDMode="Static" Text="Modelo"></asp:Label>
</asp:TableCell>
            <asp:TableCell ID="TableCell4" runat="server" ClientIDMode="Static">
            <asp:TextBox MaxLength="25" id="txtModelo" runat="server" ClientIDMode="Static"></asp:TextBox>
</asp:TableCell>
        </asp:TableRow>
                        <asp:TableRow ID="TableRow3" runat="server" ClientIDMode="Static">
            <asp:TableCell ID="TableCell5" runat="server" ClientIDMode="Static"><asp:Label ID="lblDescripcion" runat="server" ClientIDMode="Static" Text="Descripcion"></asp:Label>
</asp:TableCell>
            <asp:TableCell ID="TableCell6" runat="server" ClientIDMode="Static">
            <asp:TextBox ID="txtDescripcion" runat="server" ClientIDMode="Static">
            </asp:TextBox>
</asp:TableCell>
        </asp:TableRow>

        <asp:TableRow ID="TableRow5" runat="server" ClientIDMode="Static">
            <asp:TableCell ID="TableCell9" runat="server" ClientIDMode="Static"></asp:TableCell>
            <asp:TableCell ID="TableCell10" runat="server" ClientIDMode="Static"><asp:Button ID="btnAgregar" runat="server" ClientIDMode="Static" Text="Aceptar" onclick="btnAgregar_Click"/>
</asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    &nbsp;</div>
    <div class="footDer">
    <asp:HyperLink ID="hlkSucursal" runat="server" ClientIDMode="Static" 
            NavigateUrl="~/Vistas/mantenedorMarcas.aspx">Ir a Marcas</asp:HyperLink><br/>

    </div>
</asp:Content>

