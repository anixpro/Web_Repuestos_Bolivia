<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ConsultaVIN.aspx.cs" Inherits="Vistas_ConsultaVIN" %>

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
            <table border="1">
                <tr>
                    <td colspan="2">Informacion VIN</td>
                </tr>
                <tr>
                    <td>Año</td>
                    <td><%=ano%></td>
                </tr>
                <tr>
                    <td>VIN</td>
                    <td><%=vin%></td>
                </tr>
                <tr>
                    <td>Chasis</td>
                    <td><%=chasis%></td>
                </tr>
                <tr>
                    <td>Marca</td>
                    <td><%=marca%></td>
                </tr>
                <tr>
                    <td>Modelo</td>
                    <td><%=modelo%></td>
                </tr>
                
            </table>
        </div>
    </form>
</body>
</html>
