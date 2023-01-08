<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="ModificaUsuario.aspx.cs" Inherits="Vistas_modificaUsuario" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script language="javascript" type="text/javascript">

    /*
        $(document).ready(function () {
            $(".btnElimina").click(function (event) {
                event.preventDefault();
                $(".btnElimina").attr("disabled", "true");
                return confirm("Esta seguro?");
            });
        });
      */  
        function validaTxtRut() {
            document.getElementById('lblRut').style.color = "#696969";
            cajaTexto = document.getElementById('txtRut').value;
            mje_rut = '';
            if (cajaTexto == "") {
                mje_rut = 'Debe llenar Rut\n';
                document.getElementById('lblRut').style.color = "red";
                document.getElementById('txtRut').value = '';
            }
            else if (isNaN(cajaTexto)) {
                mje_rut = 'Solo numeros en Rut\n';
                document.getElementById('lblRut').style.color = "red";
                document.getElementById('txtRut').value = '';
            }
            else {

            }
            return (mje_rut);
        }

        function validaBlancos() {

            validaTxtRut();
            mensaje = '';
            mensaje = mje_rut;
            if (mensaje != '') {
                window.alert(mensaje);
                return false;
            }
            else {
                return true
            }
        }

</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="titulo">
        Mantenedor de usuarios: Buscar, editar y eliminar usuarios
    </div>
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <div class="mantenedorUsuariosEditarIzquierda">
        <center><b><u>Mantención Usuarios</u></b></center>
        <ul>
            <li>
                <asp:HyperLink ID="hlkModificaUsuario" runat="server" ClientIDMode="Static" 
                    NavigateUrl="~/Vistas/modificaUsuario.aspx">Buscar, modificar y eliminar
                </asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="hlkNuevoUsuario" runat="server" ClientIDMode="Static" 
                    NavigateUrl="~/Vistas/Mantenedor.aspx">Crear Usuario
                </asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="hlkUsuarioDoble" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/mantenedorUsuarioDoble.aspx">Doble Permiso</asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="hlkEliminados" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/mantenedorUsuarioEliminados.aspx">Ver Eliminados</asp:HyperLink><br />
            </li>
        </ul>
    </div>
    <div class="mantenedorBuscaPersona">
        <div class="avisoInsert">
        <asp:Label ID="lblAviso" runat="server" ClientIDMode="Static" ForeColor="#99CCFF" ></asp:Label>
        </div>
        <center><b><u>Busqueda de usuarios</u></b></center>
        <asp:Table ID="tablaBuscaUsuarios" runat="server" ClientIDMode="Static" CssClass="tablaBuscadorPersonas">
            <asp:TableRow runat="server" ClientIDMode="Static">
                <asp:TableCell runat="server" ClientIDMode="Static"><asp:Label ID="lblRut" runat="server" ClientIDMode="Static">Rut</asp:Label></asp:TableCell>
                <asp:TableCell ID="TableCell1" runat="server" ClientIDMode="Static"><asp:TextBox ID="txtRut" runat="server" ClientIDMode="Static"></asp:TextBox></asp:TableCell>
                <asp:TableCell><asp:Button ID="btnBuscar" runat="server" ClientIDMode="Static" Text="Buscar" OnClick="btnBuscar_Click"/></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell11" runat="server" ClientIDMode="Static"><asp:Label ID="lblConcesionario" runat="server" ClientIDMode="Static">Concesionario</asp:Label></asp:TableCell>
                <asp:TableCell ColumnSpan="3" ID="TableCell22" runat="server" ClientIDMode="Static"><asp:DropDownList ID="ddlConcesionario" runat="server" ClientIDMode="Static" onselectedindexchanged="ddlConcesionario_SelectedIndexChanged" AutoPostBack="True"></asp:DropDownList></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow1" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell3" runat="server" ClientIDMode="Static"><asp:Label ID="lblZona" runat="server" ClientIDMode="Static">Zona</asp:Label></asp:TableCell>
                <asp:TableCell ColumnSpan="3" ID="TableCell4" runat="server" ClientIDMode="Static"><asp:DropDownList ID="ddlZona" runat="server" ClientIDMode="Static" OnSelectedIndexChanged="ddlZona_SelectedIndexChanged" AutoPostBack="True"></asp:DropDownList></asp:TableCell>
            </asp:TableRow>    
        </asp:Table>
    </div>
    <asp:GridView ID="gridUsuarios" runat="server" ClientIDMode="Static" AllowPaging="True" AllowSorting="True"
        AutoGenerateColumns="False" DataSourceID="sqldPersona"
        EnableModelValidation="True" Style="text-align:center;"
        EmptyDataText="No se encontraron usuarios" CellPadding="10" 
        ForeColor="#333333" GridLines="None" CssClass="floatLeft"
        OnRowCommand="gridUsuarios_RowCommand">
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <Columns >
                <asp:BoundField DataField="shipCode" HeaderText="Destinatario Mercancía"  />
                <asp:BoundField DataField="rut" HeaderText="rut" ItemStyle-CssClass="invi" HeaderStyle-CssClass="invi" FooterStyle-CssClass="invi"/>
                <asp:HyperLinkField DataTextField="rut" HeaderText="RUT"  DataNavigateUrlFields="rut" DataNavigateUrlFormatString="mantenedorUsuarioUpdate.aspx?rut={0}" />
                <asp:BoundField DataField="nombre" HeaderText="Usuario"/>
                <asp:BoundField DataField="email" HeaderText="Correo Electrónico"/>
                <asp:BoundField DataField="telefono" HeaderText="Teléfono"/>
                <asp:BoundField DataField="cargo" HeaderText="Cargo"/>
                <asp:ButtonField ButtonType="Image" ImageUrl="~/img/cross.png" CommandName="elimina" Text="Elimina" HeaderText="Eliminar" ControlStyle-CssClass="btnElimina" />
            </Columns>
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <EditRowStyle BackColor="#999999" />
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
    </asp:GridView>
    <asp:SqlDataSource ID="sqldPersona" runat="server" 
        ConnectionString="<%$ ConnectionStrings:skbergeConnectionString %>" >
    </asp:SqlDataSource>
</asp:Content>

