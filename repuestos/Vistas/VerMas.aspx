<%@ Page Title="" Language="C#" ValidateRequest="false" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="VerMas.aspx.cs" Inherits="Vistas_VerMas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="titulo">
        Vista previa
    </div>
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <div class="noticiaPreview">
        <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
    </div>
    <div style="text-align:right">
        <asp:Button ID="btnVolver" Text="Aceptar" CssClass="button" runat="server"/>
        <asp:Button ID="btnEditar" Text="Editar" CssClass="button" runat="server"/>
        <asp:Button ID="btnEliminar" Text="Eliminar" CssClass="button" runat="server" />
    </div>
</asp:Content>

