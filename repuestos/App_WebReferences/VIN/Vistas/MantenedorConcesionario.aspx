<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="MantenedorConcesionario.aspx.cs" Inherits="mantenedorConcesionario" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script language="javascript" type="text/javascript">

        function obtenerExtencion()
        {
            if (document.getElementById('fldImagen').value != '') {
                var cadena = document.getElementById('fldImagen').value.split('.');
                if (cadena[1] != 'jpg' && cadena[1] != 'JPG' && cadena[1] != 'png' && cadena[1] != 'PNG') {
                    window.alert("Formato de imagen solo puede ser:\njpg o png");
                    document.getElementById('fldImagen').value = '';
                    return false;
                }
                else {
                    return true;
                }
            }
        }
        function cambiaImagen() 
        {
            var concesionario = document.getElementById('ddlConcesionario').options[document.getElementById('ddlConcesionario').selectedIndex].text.replace('.','').replace('.','').replace('.','');
            document.getElementById('imgConcesionario').src = "../doc/imgConcesionarios/"+concesionario+".jpg";
        }
        function imagenConcesionario()
         {
            //var ruta = document.getElementById('imgConcesionario').src.split('/');
            //var foto = ruta[ruta.length - 1].split('.');
            var concesionario = '<%=_concesionario %>';
            //window.alert(concesionario);
            //window.alert(document.getElementById('imgConcesionario').src);
            document.getElementById('imgConcesionario').src = "../doc/imgConcesionarios/" + concesionario + ".jpg";
        }

</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="titulo">
        Mantención de concesionarios
    </div>
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <div class="avisoInsert">
        <asp:Label ID="lblAviso" runat="server" ClientIDMode="Static" ForeColor="#99CCFF" ></asp:Label>
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
        <b><u>Formulario de ingreso</u></b>
        <br />
        <asp:Table ID="tblDatos" runat="server" ClientIDMode="Static">
            <asp:TableRow runat="server" ClientIDMode="Static">
                <asp:TableCell ColumnSpan="3" HorizontalAlign="Center">
                    <asp:Label ID="lblMiConcesionario" runat="server" ClientIDMode="Static" Text="Concesionario"></asp:Label>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell runat="server" ClientIDMode="Static">
                    Concesionario:
                </asp:TableCell>
                <asp:TableCell runat="server" ClientIDMode="Static">
                    <asp:DropDownList ID="ddlConcesionario" runat="server" ClientIDMode="Static" onchange="cambiaImagen();" visible="false"></asp:DropDownList>
                </asp:TableCell>
                <asp:TableCell>
                    <asp:Button ID="btnModificaDatos" Text="Editar" runat="server" ClientIDMode="Static" onclick="btnModificaDatos_Click" CssClass="button"/>
                    <asp:Button ID="btnEliminar" OnClientClick="javascript:return(confirm('Se va a eliminar el concesionario de la lista y del sistema. ¿Esta seguro?'))" Text="Eliminar" runat="server" ClientIDMode="Static" CssClass="button"/>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow runat="server" ClientIDMode="Static">
                <asp:TableCell runat="server" ClientIDMode="Static">
                    Cambiar Imagen:
                </asp:TableCell>
                <asp:TableCell runat="server" ClientIDMode="Static">
                    <asp:FileUpload ID="fldImagen" runat="server" ClientIDMode="Static"/>
                </asp:TableCell>
                <asp:TableCell>
                    <asp:Button ID="btnAgregar" runat="server" ClientIDMode="Static" Text="Aceptar" onclick="btnAgregar_Click" CssClass="button"/>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow2" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell3" runat="server" ClientIDMode="Static" ColumnSpan="3" HorizontalAlign="Center">
                    Imagen actual:
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell ColumnSpan="3" HorizontalAlign="Center">
                    <img id="imgConcesionario" src="../doc/imgConcesionarios/APC.jpg" height="100px"  width="100px" alt="Concesionario sin imágen. Asigne una"/>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </div>
</asp:Content>

