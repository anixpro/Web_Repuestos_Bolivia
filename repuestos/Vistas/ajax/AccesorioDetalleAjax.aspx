<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AccesorioDetalleAjax.aspx.cs" Inherits="Vistas_AccesorioDetalleAjax" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <!-- Acá se despliegan los mensajes de error -->
            <div id="msjesError" runat="server" clientidmode="Static"></div>
            <table>
                <tr>
                    <td colspan="2">Ficha técnica</td>
                </tr>
                <tr>
                    <td>Nombre: </td>
                    <td><asp:Label ID="lblNombre" Text="text" runat="server" /></td>
                </tr>
                <tr>
                    <td>Código: </td>
                    <td><asp:Label ID="lblCodigo" Text="text" runat="server" /></td>
                </tr>
                <tr>
                    <td>Stock: </td>
                    <td><asp:Label ID="lblStock" Text="text" runat="server" /></td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
