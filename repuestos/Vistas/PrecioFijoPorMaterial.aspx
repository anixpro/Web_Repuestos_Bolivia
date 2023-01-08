<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="PrecioFijoPorMaterial.aspx.cs" Inherits="Vistas_PrecioFijoPorMaterial" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="titulo">
       Mantenedor de Grupos Técnicos y/o Frecuencias
    </div>
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <div class="mantenedorConcesionariosIzquierda">
        <center><b><u>Opciones</u></b></center>
        <ul>
            <li>
                <asp:HyperLink ID="HyperLink1" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/MantenedorGruposFrecuencias.aspx">Sku</asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="HyperLink6" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/MantenedorFrecuencias.aspx">Frecuencias</asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="HyperLink5" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/MantenedorGrupoTecnico.aspx">Grupos técnicos</asp:HyperLink><br />    
            </li>
            <li>
                <asp:HyperLink ID="HyperLink2" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/MantenedorMarcasGT.aspx">Marcas</asp:HyperLink><br />    
            </li>
            <li>
                <asp:HyperLink ID="HyperLink3" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/PrecioFijoPorMaterial.aspx">Carga masiva precios fijos</asp:HyperLink><br />    
            </li>
        </ul>
    </div>
    
    <div class="mantenedorConcesionariosDerecha" style="width:auto;">
        <b><u>Carga de Precio Fijo</u></b>
        <asp:Table ID="Table1" runat="server" ClientIDMode="Static">
            <asp:TableRow ID="TableRow1" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell1" runat="server" ClientIDMode="Static">
                    Marca :
                </asp:TableCell>
                <asp:TableCell ID="TableCell4" runat="server" ClientIDMode="Static">
                    <asp:DropDownList ID="ddlMarca" runat="server"></asp:DropDownList>
                </asp:TableCell>
            </asp:TableRow>

            <asp:TableRow ID="TableRow2" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell2" runat="server" ClientIDMode="Static">
                    Archivo :
                </asp:TableCell>
                <asp:TableCell ID="TableCell3" runat="server" ClientIDMode="Static">
                    <asp:FileUpload ID="fupArchivo" runat="server" />
                </asp:TableCell>
                <asp:TableCell>
                    <asp:Button ID="btnCargar" runat="server" ClientIDMode="Static" Text="Cargar Archivo" OnClick="btnCargar_Click"/>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </div></asp:Content>