<%@ Page Title="::SKBergé - Repuestos::" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="AgregarLinkInteres.aspx.cs" Inherits="Vistas_AgregarLinkInteres" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script language="javascript" type="text/javascript">
    $(document).ready(function () {

        $("#btnAgregarLink").click(function () {
            return validaBlancos();
        });

        function validaTxtDescripcion() {
            cajaTexto = $("#txtNomLinkInteres").val();
            mje_des = '';
            if (cajaTexto == "") {
                mje_des = 'debe llenar descripcion\n';
                $("#txtNomLinkInteres").val("");
            }
            return (mje_des);
        } //fin validaFono

        function validaBlancos() {
            validaTxtDescripcion();
            mensaje = '';
            mensaje = mje_des;
            if (mensaje != '') {
                alert(mensaje);
                return false;
            }
            else {
                return true;
            }
        } //fin validaBlancos

    });
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="titulo">
        Agregar nuevo Link de interes
    </div>
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <div class="costadoDerecho" style="height:187px;">
        <table>
            <tr>
                <td>Descripción: </td>
                <td>
                    <asp:TextBox ID="txtNomLinkInteres" runat="server" MaxLength="70" ClientIDMode="Static"></asp:TextBox>
                </td>
                <td>
                    <img alt="Ejemplo: Conesionario A" title="Ingrese la descripción de enlace de interés del sitio" src="../img/help.png" />
                </td>
            </tr>
            <tr>
                <td>URL del sitio: </td>
                <td>
                    <asp:TextBox ID="txtLinkInteres" runat="server" Text="http://" MaxLength="100" 
                        ClientIDMode="Static"></asp:TextBox></td>
                <td>
                    <img alt="Ejemplo: http://www.ferrari.cl" title="Ingrese un enlace de interés del sitio. Ejemplo: 'http://www.mg.com'" src="../img/help.png" />
                   <asp:Label ID="lblError" runat="server" ForeColor="Red" Text="Debe completar todos los campos"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="3" align="center">
                    <asp:Button CssClass="button" ID="btnAgregarLink" ClientIDMode="Static" runat="server" Text="Agregar" onclick="btnAgregar_Click"/>
                </td>
            </tr>
        </table>
    </div>
    <div class="pie">
    </div>
</asp:Content>