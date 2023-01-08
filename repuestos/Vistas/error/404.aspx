<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="404.aspx.cs" Inherits="Vistas_error_404" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <p>
        <b>La página solicitada... ¡NO EXISTE!</b>
    </p>
    <p>
        Lo sentimos...
        <br /><br /><br />
        La página solicitada, ya no existe o nunca existió.
    </p>
    <center>
        <asp:Image ID="Image1" runat="server" ImageUrl="~/img/car.jpg" />
    </center>
</asp:Content>
