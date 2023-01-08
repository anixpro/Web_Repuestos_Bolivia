<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="MantenedorConcesionarioInsertaMarca.aspx.cs" Inherits="mantenedorConcesionarioInsertaMarca" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script language="javascript" type="text/javascript">

        function obtenerExtencion() {
            var cadena = document.getElementById('ctl00_MainContent_fldImagen').value.split('.');
            if (cadena[1] != 'jpg') {
                window.alert("Formato de imagen solo puede ser:\njpg");
                document.getElementById('ctl00_MainContent_fldImagen').value = '';
                return false;
            }
            else {
                return true;
            }
        }
        function cambiaImagen() {

            //var ruta = document.getElementById('imgConcesionario').src.split('/');
            //var foto = ruta[ruta.length - 1].split('.');
            var concesionario = document.getElementById('ctl00_MainContent_ddlConcesionario').options[document.getElementById('ctl00_MainContent_ddlConcesionario').selectedIndex].text.replace('.', '').replace('.', '').replace('.', '');
            //window.alert(concesionario);
            //window.alert(document.getElementById('imgConcesionario').src);
            document.getElementById('imgConcesionario').src = "../doc/imgConcesionarios/" + concesionario + ".jpg";
        }
        function imagenConcesionario() {

            //var ruta = document.getElementById('imgConcesionario').src.split('/');
            //var foto = ruta[ruta.length - 1].split('.');
            var concesionario = '<%=_concesionario %>';
            //window.alert(concesionario);
            //window.alert(document.getElementById('imgConcesionario').src);
            document.getElementById('imgConcesionario').src = "../doc/imgConcesionarios/" + concesionario + ".jpg";
        }

        function validaCheck() 
        {

            var numeroDeChecks = '<%=cbxlMarcas.Items.Count %>';
            var checkTotal = 'ctl00_MainContent_cbxlMarcas_';
            var nombreCheck = '';
            var aux = 0;
            //alert(numeroDeChecks);
            for (x = 0; x < numeroDeChecks; x++) 
            {

                nombreCheck = checkTotal + x;
                //alert(nombreCheck);
                if (!document.getElementById(nombreCheck).checked) 
                {

                    aux += 1;

                }
            }

            //alert(aux);
            if (aux == numeroDeChecks) 
            {
                alert('Debe seleccionar al menos una marca');
                return false;
            }
            else 
            {
                return true;
            }
        }

</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="titulo">
        Mantenedor de concesionarios: Asociar marcas a un conecesionario
    </div>
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <div class="avisoInsert">
        <asp:Label ID="lblAviso" runat="server" ClientIDMode="Static" ForeColor="#99CCFF" >
        </asp:Label>
    </div>
    <div class="mantenedorConcesionariosIzquierda">
        <center><b><u>Mantención Concesionarios</u></b></center>
        <ul>
            <li>
                <asp:HyperLink ID="hlkNuevoConcesionario" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/MantenedorConcesionarioInsert.aspx">Insertar nuevo</asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="hlkInsertaMarcas" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/MantenedorConcesionarioInsertaMarca.aspx">Asociar marcas</asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="hlkEliminaMarcas" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/MantenedorConcesionarioEliminaMarca.aspx">Desasociar marcas</asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="hlkSucursal" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/SucursalesLista.aspx">Ver Sucursales</asp:HyperLink>
            </li>
        </ul>
    </div>
    <div class="mantenedorConcesionariosDerecha">
        <b><u>Marcas del concesionario:</u></b>
        <asp:CheckBoxList ID="cbxlMisMarcas" runat="server" ClientIDMode="Static" RepeatDirection="Vertical" Enabled="false"  RepeatColumns="4"></asp:CheckBoxList>
        <asp:Table ID="tblDatos" runat="server" ClientIDMode="Static">
            <asp:TableRow ID="TableRow1" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell1" runat="server" ClientIDMode="Static"><asp:Label ID="lblConcesionario" runat="server" ClientIDMode="Static" Text="Concesionario"></asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="TableCell2" runat="server" ClientIDMode="Static"><asp:DropDownList ID="ddlConcesionario" runat="server" ClientIDMode="Static" AutoPostBack="true"></asp:DropDownList>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell ColumnSpan="2">
                    <b><u>Marcas para agregar:</u></b>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow2" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell4" ColumnSpan="2">
                    <asp:CheckBoxList ID="cbxlMarcas" runat="server" ClientIDMode="Static" RepeatDirection="Horizontal" RepeatColumns="4"></asp:CheckBoxList>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow3" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell6" runat="server" ClientIDMode="Static" ColumnSpan="2" HorizontalAlign="Center">
                    <asp:Button ID="btnAgregar" runat="server" ClientIDMode="Static" Text="Aceptar" onclick="btnAgregar_Click" CssClass="button"/>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </div>
</asp:Content>

