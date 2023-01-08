<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="EditorLinkAccesorios.aspx.cs" Inherits="Vistas_EditorLinkAccesorios" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">

<div class="titulo">
        Editar archivo descargable de Accesorios 
            </div>
            <!-- Acá se despliegan los mensajes de error -->
            <div id="msjesError" runat="server" clientidmode="Static">
            </div>

            <asp:FileUpload ID="fudEligeCatalogo" runat="server" ClientIDMode="Static" />
            <asp:Button ID="btnSubir" runat="server"
                Text="Subir Catalogo"  ClientIDMode="Static" 
        onclick="btnSubir_Click" />
</asp:Content>

