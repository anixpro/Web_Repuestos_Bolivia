<%@ Page Language="C#" Debug="true" AutoEventWireup="true" CodeFile="CambiaContrasena.aspx.cs" Inherits="cambiaContrasena" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en">
<head id="Head1" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>..::ASTARA::..</title>
    <link href="css/Estilo.css" rel="stylesheet" type="text/css" />
    <script type='text/javascript' src='js/jquery-1.6.2.min.js'></script>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            $('body').css('display', 'inline');
        });

        function validaTxtConf() {
            document.getElementById('lblConfirma').style.color = "#696969";
            cajaTextoConf = document.getElementById('txtConfirma').value;
            cajaTextoCon = document.getElementById('txtNuevaContrasena').value;
            mje_conf= '';
            if (cajaTextoConf != cajaTextoCon) {
                mje_conf = 'Las contrasenas deben coincidir\n';
                document.getElementById('lblConfirma').style.color = "red";
                document.getElementById('txtConfirma').value = '';
            }
            return (mje_conf);
        }

        function validaBlancos() {
            validaTxtConf();
            mensaje = '';
            mensaje = mje_conf;
            if (mensaje != '') {
                window.alert(mensaje);
                return false;
            }
            else {
                return true
            }
        }
    </script>
</head>
<body>
    <form id="Form1" runat="server">
    <div class="page">
        <div class="header">
            <div class="title">
                <h1>
                    <img alt='Sistema de Repustos 2.0' src='img/logosk-new.png' style='height:104px;margin:0px 0px 0px 80px;'/>
                </h1>
            </div>
            <div class="loginDisplay">
                
            </div>
            <div class="clear hideSkiplink">
            </div>
        </div>
       <div class="center"><br/>
    <div class="avisoInsert">
        Cambio de contraseña
    </div>
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <asp:Table ID="tblDatos" runat="server">
        <asp:TableRow ID="TableRow6" runat="server">
            <asp:TableCell ID="TableCell11" runat="server">
                <asp:Label ID="lblNuevaContrasena" runat="server" Text="Nueva contrasena"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell12" runat="server">
                <asp:TextBox MaxLength="50" ID="txtNuevaContrasena" runat="server" TextMode="Password"></asp:TextBox>
            </asp:TableCell>
            </asp:TableRow>
               <asp:TableRow ID="TableRow4" runat="server">
            <asp:TableCell ID="TableCell7" runat="server">
                <asp:Label ID="lblConfirma" runat="server" Text="Confirmar contrasena" ></asp:Label>
</asp:TableCell>
            <asp:TableCell ID="TableCell8" runat="server"><asp:TextBox MaxLength="50" ID="txtConfirma" runat="server" TextMode="Password"></asp:TextBox>
</asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow7" runat="server">
            <asp:TableCell ID="TableCell13" runat="server"></asp:TableCell>
            <asp:TableCell ID="TableCell14" runat="server"><asp:Button ID="btnAgregar" runat="server" Text="Aceptar" OnClick="btnAgregar_Click"/>
</asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    </div>
    </div>
        <div class="clear">
        </div>
    <div class="footer">
    </div>
    </form>
</body>
</html>
