<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Contacto.aspx.cs" Inherits="Vistas_Contacto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="titulo">Consultas al administrador</div>
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <div id="FormularioContacto" >
        <table class="contacto">
        <tr>
            <td colspan="2">Acá usted puede contactar al administrador del sitio para hacer consultas y sugerencias (1000 caract. max)</td>
        </tr>
        <tr>
        <td colspan="2">
            <asp:TextBox ID="txtMensaje" runat="server" TextMode="MultiLine" Height="144px" 
                Width="351px" MaxLength="1000"></asp:TextBox>
        </td>
        </tr>
        <tr>
        <td>
            <asp:Button ID="btnEnviar" runat="server" Text="Enviar" 
                onclick="btnEnviar_Click" />  
        </td>
        <td>
            <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" 
                onclick="btnLimpiar_Click" />
        </td>
        </tr>
        </table>
    
    </div>


</asp:Content>

