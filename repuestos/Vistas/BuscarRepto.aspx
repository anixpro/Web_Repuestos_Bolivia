<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="BuscarRepto.aspx.cs" Inherits="Vistas_buscarRepto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">

    <div class="titulo" id="titCotizar">
        <table>
            <tr>
                <td width="65%">Búsqueda de Repuestos</td>
                <td><a href="../Documentacion/Catálogo de Productos y Unidades de Medida.xlsx" style="font-size: small; color: #FFFFFF;">Catálogo de Productos y Unidades de Medida</a></td>
            </tr>
        </table>
    </div>

    <div id="msjesError" runat="server" clientidmode="Static"></div>

    <br />
    <br />

    <div align="center">
       <table>
            <tr>
                <td>
                    <asp:Label ID="lblCanaVenta" runat="server" Text="Canal de venta"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlCanalVenta" runat="server" AutoPostBack="true" OnTextChanged="ddlCanalVenta_TextChanged"></asp:DropDownList>
                </td>
            </tr>

           <tr>
                <td>
                    <asp:Label ID="lblSubCliente" runat="server" Text="Sub Cliente"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlSubCliente" runat="server" AutoPostBack="true"></asp:DropDownList>
                </td>
           </tr>

           <tr><td>&nbsp;</td></tr>

           <tr>
               <td colspan="2" align="center">
                   <asp:Button ID="btnContinuar" runat="server" Text="Continuar" OnClick="btnContinuar_Click" />
               </td>
           </tr>
        </table>
    </div>

    <br />
    <br />

</asp:Content>
