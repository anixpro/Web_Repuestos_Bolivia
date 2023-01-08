<%@ Page Language="C#" AutoEventWireup="true" CodeFile="prueba.aspx.cs" Inherits="prueba" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Panel runat="server" ID="IngresoUsuario"> 
            id: <asp:TextBox runat="server" ID="idUsuario"/><br />
            usua: <asp:TextBox runat="server" ID="usuario"/><br />
            pass: <asp:TextBox runat="server" ID="password"/><br />
            desc: <asp:TextBox runat="server" ID="userdesc"/><br />
            conmc: <asp:TextBox runat="server" ID="concesionariodesc"/><br />
            dest: <asp:TextBox runat="server" ID="destinatariosap"/><br />
            email: <asp:TextBox runat="server" ID="email"/><br />
            <asp:Button Text="text" runat="server" ID="btnAcepta" 
                onclick="btnAcepta_Click"/><br />
            <asp:Label Text="text" runat="server" ID="lblResultado"/>
            <asp:TextBox runat="server" ID="txtConsulta"/>
            <asp:Button ID="btnBuscaExacta" Text="Consulta" runat="server" onclick="Unnamed1_Click" />
        </asp:Panel>
        <asp:Panel runat="server" ID="RescataTodos">
            <asp:Button ID="Button1" Text="de H2 a BD" runat="server" onclick="Unnamed1_Click1" />
        </asp:Panel>
        <asp:Panel runat="server" ID="GrabaTodos">
            <asp:Button ID="btnGrabar" Text="de BD a h2" runat="server" 
                onclick="btnGrabar_Click" />
        </asp:Panel>
    </div>
    </form>
</body>
</html>
