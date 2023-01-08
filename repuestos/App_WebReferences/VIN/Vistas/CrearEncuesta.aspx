<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CrearEncuesta.aspx.cs" Inherits="Vistas_crearEncuesta" %>
<%@ OutputCache Location="None" VaryByParam="None" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <form id="form1" runat="server">
    <div>
         <!--Crear encuesta -->
        <table>

            <tr>
            <td colspan="2" style="font-family: Arial, Helvetica, sans-serif; font-size: 13px; font-weight: bold; color: #072E54; border-bottom-style: dashed; border-bottom-width: thin;">Generar nueva encuesta</td>
            </tr>

            <tr>
            <td style="font-family: Arial, Helvetica, sans-serif; font-size: 11px; font-weight: bold; color: #252829"><asp:Label ID="lblPergunta" runat="server" Text="Pregunta: "></asp:Label></td>
            <td><asp:TextBox MaxLength="150" ID="txtPregunta" runat="server" Width="120px" 
                    ClientIDMode="Static"></asp:TextBox></td>
            </tr>

            <tr>
            <td style="font-family: Arial, Helvetica, sans-serif; font-size: 11px; font-weight: bold; color: #252829"><asp:Label runat="server" Text="Número alternativas: " ID="lblNumAlternativa"></asp:Label></td>
            <td>
                <asp:DropDownList ID="comboNumAlternativas" runat="server" AutoPostBack="True">
                    <asp:ListItem></asp:ListItem>
                    <asp:ListItem>2</asp:ListItem>
                    <asp:ListItem>3</asp:ListItem>
                    <asp:ListItem>4</asp:ListItem>
                </asp:DropDownList>
            </td>
            </tr>

            <tr>
            <td style="font-family: Arial, Helvetica, sans-serif; font-size: 11px; font-weight: normal; color: #252829"><asp:Label Visible="false" runat="server" Text="Alternativa 1: " ID="lblAlter1"></asp:Label></td>
            <td><asp:TextBox MaxLength="100" Visible="false"  ID="txtAlter1" runat="server" 
                    Width="120px" ClientIDMode="Static"></asp:TextBox></td>
            </tr>

            <tr>
                <td style="font-family: Arial, Helvetica, sans-serif; font-size: 11px; font-weight: normal; color: #252829"><asp:Label Visible="false" runat="server" Text="Alternativa 2: " ID="lblAlter2"></asp:Label></td>
                <td><asp:TextBox MaxLength="100" Visible="false" ID="txtAlter2" runat="server" 
                    Width="120px" ClientIDMode="Static"></asp:TextBox></td>
            </tr>

            <tr>
                <td style="font-family: Arial, Helvetica, sans-serif; font-size: 11px; font-weight: normal; color: #252829"><asp:Label Visible="false" runat="server" Text="Alternativa 3: " ID="lblAlter3"></asp:Label></td>
                <td><asp:TextBox MaxLength="100" Visible="false" ID="txtAlter3" runat="server" 
                    Width="120px" ClientIDMode="Static"></asp:TextBox></td>
            </tr>

            <tr>
                <td style="font-family: Arial, Helvetica, sans-serif; font-size: 11px; font-weight: normal; color: #252829"><asp:Label Visible="false" runat="server" Text="Alternativa 4: " ID="lblAlter4"></asp:Label></td>
                <td><asp:TextBox MaxLength="100" Visible="false" ID="txtAlter4" runat="server" 
                    Width="120px" ClientIDMode="Static"></asp:TextBox></td>
            </tr>

            <tr style="display:none">
                <td style="font-family: Arial, Helvetica, sans-serif; font-size: 11px; font-weight: normal; color: #252829">
                    <asp:Label runat="server" Text="Fecha termino: " ID="lblFechaTermino"></asp:Label>
                    <asp:ImageButton runat="Server" ID="ImageButton1" ImageUrl="~/img/calendarIcon.png" AlternateText="Click here to display calendar" />
            
                </td>
                <td>
                <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
                    </asp:ToolkitScriptManager>
                    <asp:CalendarExtender ID="FechaTermino" runat="server"
                    TargetControlID="txtFechaTermino" PopupButtonID="ImageButton1" 
                        Format="yyyy-MM-dd"/> 
                    <asp:TextBox MaxLength="20" ID="txtFechaTermino" runat="server" Width="120px" 
                        ClientIDMode="Static"></asp:TextBox>
                </td>
            </tr>
            <tr>
            <td colspan="2" style="font-family: Arial, Helvetica, sans-serif; font-size: 12px; color: #CC0000; font-weight: bold"><asp:Label ID="lblError" Visible="false" runat="server" Text="ERROR, debe completar todos los campos"></asp:Label></td>
            </tr>
            
            <tr>
            <td>
                <asp:ImageButton ID="btnVolver" runat="server" ImageUrl="~/img/volver.png" /></td>
            <td>
                <asp:ImageButton ID="btnCrear" runat="server" ImageUrl="~/img/btn_Aceptar.png" /></td>
            </tr>
        </table>  
     <!--Fin Crear encuesta -->
    </div>
    </form>
</body>
</html>
