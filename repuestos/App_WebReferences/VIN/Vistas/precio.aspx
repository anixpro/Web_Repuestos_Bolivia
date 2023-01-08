<%@ Page Language="C#" AutoEventWireup="true" CodeFile="precio.aspx.cs" Inherits="Vistas_precio" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>.: Ingresar Precio :.</title>
    <style type="text/css">
    <!--
    .style2 
    {
        font-family: Arial, Helvetica, Sans-Serif;
        font-size: 11px;
        color: #00275B;
    }
    .style3 
    {
	    font-family: Arial, Helvetica, sans-serif;
	    font-size: 13px;
	    font-weight: bold;
	    color: #ffffff; text-align:left; padding-left:12px;
    }
    .style8 
    {
        font-family: Arial, Helvetica, sans-serif;
        font-size: 12px;
	    font-weight: bold;
	    color: #204C79; 
        text-align: left; 
        padding-left:3px; 
    }      
    -->    
    </style>
</head>
<body bgcolor="#F3F4F7">
    <form id="formPrecio" name="formulario" runat="server">
    <table width="500" border="0" align="center" cellpadding="0" cellspacing="0">
        <tr>
            <td width="178" height="20" bgcolor="#204C79" class="style3">
                Ingreso Precio de Repuesto
            </td>
            <td width="322" style="border-bottom: 1px solid #ccccc">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="2">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <table width="500" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td class="style8" align="right">
                            Ingrese Precio de Repuesto
                        </td>
                        <td>
                            <asp:TextBox ID="txtPrecio" runat="server"></asp:TextBox>
                        </td>
<%--                        <td colspan="2" align="right">
                            <asp:ImageButton ID="enviar" runat="server" />
                        </td>--%>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnPrecio" runat="server" ClientIDMode="Static" 
                                Text="Ingresar Precio" onclick="btnPrecio_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
