<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="MantenedorFob.aspx.cs" Inherits="Vistas_mantenedorFob" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="titulo">
        Mantenedor valores FOB
    </div>
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <div class="mantenedorConcesionariosIzquierda">
        <center><b><u>Opciones</u></b></center>
        <ul>
            <li>
                <asp:HyperLink ID="HyperLink1" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/MantenedorFobCargaMasiva.aspx">Carga masiva</asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="HyperLink6" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/MantenedorFobValores.aspx">Mantenedor SKU</asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="HyperLink2" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/MantenedorFobParidad.aspx">Mantenedor Paridad</asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="HyperLink3" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/MantenedorFobVolumenes.aspx">Mantenedor Volumenes</asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="HyperLink4" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/MantenedorFobFactorUtilidad.aspx">Mantenedor factores de utilidad</asp:HyperLink>
            </li>
        </ul>
    </div>
</asp:Content>