<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="SolicitudLlave.aspx.cs" Inherits="Vistas_SolicitudLlave" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script src="../js/jquery.numeric.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#txtCantidad").numeric({ decimal: false, negative: false }, function () { alert("Solo enteros positivos"); this.value = ""; this.focus(); });

            $("#btnVolver").click(function () {
                window.location.href = "buscarRepto2.aspx";
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="titulo">Solicitud llave code</div>
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <p>
        Estimado operario, acá usted puede realizar solicitud de llaves code rápidamente.
        <br />
        <br />
        Favor ingresar los datos que se solicitan a continuación. Los campos con (*) son obligatorios.
    </p>
    <table>
        <thead>
            <tr>
                <th>Parámetro</th>
                <th>valor</th>
            </tr>
        </thead>
        <tbody>
            <tr id="trVIN">
                <td><b>*</b>VIN</td>
                <td><asp:TextBox ID="txtVin" runat="server" ClientIDMode="Static" Columns="30"></asp:TextBox></td>
            </tr>
            <tr>
                <td>Marca</td>
                <td>
                    <asp:DropDownList runat="server" DataSourceID="SqlDataSourceMarcas" 
                        DataTextField="nombreMarca" DataValueField="nombreMarca" ID="ddlMarcas"></asp:DropDownList>
                    <asp:SqlDataSource ID="SqlDataSourceMarcas" runat="server" 
                        ConnectionString="<%$ ConnectionStrings:skbergeConnectionString %>" 
                        SelectCommand="SELECT [nombreMarca] FROM [marca]"></asp:SqlDataSource>
                </td>
            </tr>
            <tr>
                <td><b>*</b>Código de repuesto de la Llave</td>
                <td><asp:TextBox ID="txtCodigo" runat="server" Columns="30"></asp:TextBox></td>
            </tr>
            <tr>
                <td>Cantidad</td>
                <td><asp:TextBox ID="txtCantidad" runat="server" Columns="30" ClientIDMode="Static"></asp:TextBox></td>
            </tr>
            <tr>
                <td>Detalle</td>
                <td><asp:TextBox ID="txtDetalle" runat="server" Enabled="false" Columns="30">SOLICITUD LLAVE CODE</asp:TextBox></td>
            </tr>
            <tr>
                <td>Creador</td>
                <td><asp:TextBox ID="txtCreador" runat="server" Columns="30" Enabled="False"></asp:TextBox></td>
            </tr>
            <tr>
                <td>Nombre Usuario</td>
                <td><asp:TextBox ID="txtNombreUsuario" runat="server" Columns="30" Enabled="False"></asp:TextBox></td>
            </tr>
            <tr>
                <td>Concesionario</td>
                <td><asp:TextBox ID="txtDealer" runat="server" Columns="30" Enabled="False"></asp:TextBox></td>
            </tr>
            <tr>
                <td>Direccion</td>
                <td>
                    <asp:TextBox ID="txtDirección" runat="server" Columns="30" Enabled="False"></asp:TextBox>
                    <asp:DropDownList ID="ddlSucursales" runat="server"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>Tipo de pedido</td>
                <td>
                    <asp:DropDownList ID="comboTipoPed" runat="server">
                        <asp:ListItem Value="normal" Text="Normal"></asp:ListItem>
                        <asp:ListItem Value="garantia" Text="Garantía"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
        </tbody>
    </table>
    <asp:Panel ID="PanelBotonera" runat="server">
        <asp:Button ID="btnAceptar" runat="server" Text="Aceptar" onclick="btnAceptar_Click" CssClass="button"/>
        <input id="btnCancelar" class="button" type="button" value="Cancelar" onclick="javascript:if(confirm('¿Cancelar solicitud?')){window.location='buscarrepto2.aspx';}" />
    </asp:Panel>
    <br/>
    <input type="button" id="btnVolver" value="Volver" class="button"/>
</asp:Content>
