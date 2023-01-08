<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="MantenedorUsuarioEliminados.aspx.cs" Inherits="mantenedorUsuarioEliminados" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script language="javascript" type="text/javascript">
        function validaTxtRut() {
            document.getElementById('ctl00_MainContent_lblRut').style.color = "#696969";
            cajaTexto = document.getElementById('ctl00_MainContent_txtRut').value;
            mje_rut = '';
            if (cajaTexto == "") {
                mje_rut = 'debe llenar Rut\n';
                document.getElementById('ctl00_MainContent_lblRut').style.color = "red";
                document.getElementById('ctl00_MainContent_txtRut').value = '';
            }
            else if (isNaN(cajaTexto)) {
                mje_rut = 'solo numeros en Rut\n';
                document.getElementById('ctl00_MainContent_lblRut').style.color = "red";
                document.getElementById('ctl00_MainContent_txtRut').value = '';
            }
            else {
                var rut = document.getElementById('ctl00_MainContent_txtRut').value; ;
                var largo = rut.length;
                var i = 0;
                var dv = document.getElementById('ctl00_MainContent_txtDvr').value; ;
                var mult = 2;
                var suma = 0;
                largo--;
                while (largo >= 0) {
                    suma = suma + (rut.charAt(largo) * mult);
                    if (mult > 6)
                        mult = 2;
                    else
                        mult++;
                    largo--;
                }
                var resto = suma % 11;
                var digito = 11 - resto
                if (digito == 10) {
                    digito = "k";
                }
                else if (digito == 11) {
                    digito = 0;
                }
                if (digito != dv) {
                    mje_rut = 'Rut inválido\n';
                    document.getElementById('ctl00_MainContent_lblRut').style.color = "red";
                    document.getElementById('ctl00_MainContent_txtRut').value = '';
                }
            }
            return (mje_rut);
        }

        function confirmar(rut) {
            var pregunta = confirm("¿Está seguro?");
            if (pregunta) {
                return location.href = "mantenedorUsuarioEliminados.aspx?ev=rehacer&&rut=" + rut;
            }
            else {
                return false
            }
        }

        function validaBlancos() {
            validaTxtRut();
            mensaje = '';
            mensaje = mje_rut;
            if (mensaje != '') {
                window.alert(mensaje);
                return location.href = "";
            }
            else {
                return true
            }
        }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="titulo">
        Mantenedor de usuarios: Lista de eliminados
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
<div class="center"><br/>
    <div class="avisoInsert">
    <asp:Label ID="lblAviso" runat="server" ClientIDMode="Static" ForeColor="#99CCFF" ></asp:Label>
    </div>
    <asp:Table ID="Table1" runat="server" ClientIDMode="Static">
        <asp:TableRow ID="TableRow1" runat="server" ClientIDMode="Static">
            <asp:TableCell ID="TableCell1" runat="server" ClientIDMode="Static"><asp:Label ID="lblRut" runat="server" ClientIDMode="Static">Rut</asp:Label></asp:TableCell>
            <asp:TableCell ID="TableCell2" runat="server" ClientIDMode="Static"><asp:TextBox MaxLength="8" ID="txtRut" runat="server" ClientIDMode="Static"></asp:TextBox></asp:TableCell>
            <asp:TableCell ID="TableCell3" runat="server" ClientIDMode="Static"><asp:TextBox MaxLength="1" ID="txtDvr" runat="server" ClientIDMode="Static" Width="26"></asp:TextBox></asp:TableCell>
            <asp:TableCell><asp:Button ID="btnBuscar" runat="server" ClientIDMode="Static" Text="Buscar" OnClick="btnBuscar_Click"/></asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    </div>
    <asp:GridView ID="gridUsuarios" runat="server" ClientIDMode="Static" AllowPaging="True" AllowSorting="True"
        AutoGenerateColumns="False" DataSourceID="sqldPersona" Style="text-align:center;"
        EmptyDataText="No hay registros en este estado" CellPadding="10" 
        ForeColor="#333333" GridLines="None">
        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
        <Columns >
        <asp:BoundField DataField="shipCode" HeaderText="shipCode"  />
        <asp:HyperLinkField DataTextField="rut" HeaderText="rut"  DataNavigateUrlFields="rut" DataNavigateUrlFormatString="mantenedorUsuarioUpdate.aspx?rut={0}" />
        <asp:BoundField DataField="nombre" HeaderText="nombre"/>
        <asp:BoundField DataField="email" HeaderText="email"/>
        <asp:BoundField DataField="telefono" HeaderText="telefono"/>
        <asp:BoundField DataField="cargo" HeaderText="cargo"/>    
        <asp:TemplateField FooterText="Eliminar" HeaderText="Rehabilitar">
            <ItemTemplate>
                <a onclick="javascript:confirmar('<%# Eval("rut") %>');" href="#" >Rehabilitar</a>
            </ItemTemplate>
        </asp:TemplateField>
        </Columns>
        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
        <EditRowStyle BackColor="#999999" />
        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
        </asp:GridView>
        <asp:SqlDataSource ID="sqldPersona" runat="server" 
            ConnectionString="<%$ ConnectionStrings:skbergeConnectionString %>" ></asp:SqlDataSource>
    <div class="center"></div>
    <div class="footDer">
        <asp:HyperLink ID="hlkConcesionario" runat="server" ClientIDMode="Static" 
        NavigateUrl="~/Vistas/mantenedorConcesionario.aspx">Ir a concesionario</asp:HyperLink>
        <br />
        <asp:HyperLink ID="HyperLink1" runat="server" ClientIDMode="Static" 
        NavigateUrl="~/Vistas/modificaUsuario.aspx">Modificar usuario</asp:HyperLink>
    </div>
</asp:Content>

