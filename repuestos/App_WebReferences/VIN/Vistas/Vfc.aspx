<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Vfc.aspx.cs" Inherits="Vistas_vfc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <div>
        <asp:Button ID="Button1" runat="server" onclick="Button1_Click" Text="Button" />
        <br />
        <br />
        <asp:Label ID="Label1" runat="server" Text="Vin"></asp:Label>
        <asp:TextBox ID="ttxVin" runat="server" Width="200px">MAT464051ASL01037</asp:TextBox>
        <br />
        <asp:Label ID="Label3" runat="server" Text="Label"></asp:Label>
        <asp:TextBox ID="TextBox2" runat="server"></asp:TextBox>
        <br />
        <asp:Label ID="Label2" runat="server" Text="Label"></asp:Label>
        <asp:TextBox ID="TextBox3" runat="server"></asp:TextBox>
        <br />
        <asp:Button ID="btnVfc" runat="server" onclick="btnVfc_Click" 
            Text="Hacer Vfc" />
        <br />
        <br />
        <asp:Button ID="hacerBo" runat="server" ClientIDMode="Static" 
            onclick="hacerBo_Click" Text="hacerBo" />
        <br />
        <br />
        <asp:Button ID="btnCheckVFC" runat="server" ClientIDMode="Static" 
            onclick="checkVFC_Click" Text="check vfc" />
    
    </div>
</asp:Content>
