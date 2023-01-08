<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Encuesta.aspx.cs" Inherits="Vistas_encuesta" %>
<%@ OutputCache Location="None" VaryByParam="None" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <style type="text/css">
    /* botones sexys */
        .button {
           border-top: 1px solid #c78bc7;
           background: #65a9d7;
           background: -webkit-gradient(linear, left top, left bottom, from(#3e779d), to(#65a9d7));
           background: -webkit-linear-gradient(top, #3e779d, #65a9d7);
           background: -moz-linear-gradient(top, #3e779d, #65a9d7);
           background: -ms-linear-gradient(top, #3e779d, #65a9d7);
           background: -o-linear-gradient(top, #3e779d, #65a9d7);
           padding: 3.5px 7px;
           -webkit-border-radius: 34px;
           -moz-border-radius: 34px;
           border-radius: 34px;
           -webkit-box-shadow: rgba(0,0,0,1) 0 1px 0;
           -moz-box-shadow: rgba(0,0,0,1) 0 1px 0;
           box-shadow: rgba(0,0,0,1) 0 1px 0;
           text-shadow: rgba(0,0,0,.4) 0 1px 0;
           color: white;
           font-size: 17px;
           font-family: Georgia, Serif;
           text-decoration: none;
           vertical-align: middle;
        }
        .button:hover {
           border-top-color: #28597a;
           background: #28597a;
           color: #ccc;
        }
        .button:active {
           border-top-color: #1b435e;
           background: #1b435e;
        }
        /* fin botones sexys */
    </style>
</head>
<body style="width:220px;height:230;">
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <form id="form1" runat="server">
    <div>
        <table style="width:220px;height:100%;">
            <tr>
                <td colspan="2" style="width:100%; font-family: Arial, Helvetica, sans-serif; font-size: 15px; font-weight: bold; color: #0C3763; ">
                    Encuesta
                </td>
            </tr>
        </table>

        <!--Repeater encuesta -->
        <asp:Label Visible="true" ID="lblEncuesta" runat="server" Text="Label">
        <table style="width:220px;height:100%; background-color:#DFEFFF;">
            <tr>
                <td colspan="2" style="border-width: thin; font-family: Arial, Helvetica, sans-serif; font-size: 11px; font-weight: bold; color: #1C1F20; border-top-style: dashed;">
                    <asp:Label ID="lblPregunta" runat="server" Text=""
                    ForeColor="#1E1F22"></asp:Label>
                    <asp:Label ID="lblIdEnc" runat="server" Visible="false" Text="Label"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="2" style="font-family: Arial, Helvetica, sans-serif; font-size: 11px; color: #333333">
                    <asp:RadioButton ID="RbtnRespuesta1" runat="server" Text="" ForeColor="#0C2F49" GroupName="resp" />
                    <asp:Label ID="lblRes1" visible="false" runat="server" Text="Label"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="2" style="font-family: Arial, Helvetica, sans-serif; font-size: 11px; color: #333333">
                    <asp:RadioButton ID="RbtnRespuesta2" runat="server" Text=""
                    ForeColor="#0C2F49" GroupName="resp"  />
                    <asp:Label ID="lblRes2" visible="false" runat="server" Text="Label"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="2" style="font-family: Arial, Helvetica, sans-serif; font-size: 11px; color: #333333">
                    <asp:RadioButton ID="RbtnRespuesta3" runat="server" Text="" 
                        ForeColor="#0C2F49" GroupName="resp"  />
                    <asp:Label ID="lblRes3" visible="false" runat="server" Text="Label"></asp:Label>
                </td>
            </tr>
            <tr>        
            <td colspan="2" style="font-family: Arial, Helvetica, sans-serif; font-size: 11px; color: #333333; border-bottom-style: dashed; border-bottom-width: thin;">
                <asp:RadioButton ID="RbtnRespuesta4" runat="server" Text=""
                    ForeColor="#0C2F49" GroupName="resp"  />
                    <asp:Label ID="lblRes4" visible="false" runat="server" Text="Label"></asp:Label>
                </td>
            </tr>
        </table>
    <!--Fin repeater encuesta -->

        <table style="width:220px;">
            <tr>
                <td>
                    <asp:ImageButton ID="btnVerResult"   runat="server" 
                    onclick="btnVerResult_Click" ImageUrl="~/img/btn_ver_resultados.png" />
                </td>
                <td>
                    <asp:ImageButton ID="btnResponder" runat="server" ImageUrl="~/img/votar.png" 
                    onclick="btnResponder_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="2" align="center">
                    <asp:Button ID="btnAgregar" runat="server" CssClass="button"
                    Text="Crear Nueva Encuesta" onclick="btnCreaNueva_Click" />
                </td>
            </tr>
        </table>
    </asp:Label>
    <!--Fin repeater resultados -->  
    </div>
    </form>
</body>
</html>
