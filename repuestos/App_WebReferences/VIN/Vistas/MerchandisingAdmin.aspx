<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="MerchandisingAdmin.aspx.cs" Inherits="Vistas_MerchandisingAdmin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="titulo">
        Agregar un nuevo Merchandising
    </div>
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <div class="nuevoAccesorio">
        <table>
            <tr>
                <td>Marca: </td>
                <td>
                    <asp:DropDownList ID="cbxMarcas" ClientIDMode="Static" runat="server">
                        <asp:ListItem>Seleccione Marca</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>Código: </td>
                <td>
                    <asp:TextBox MaxLength="25" ID="txtCodigo" runat="server" ClientIDMode="Static"></asp:TextBox>
                    <asp:Label ID="lblValidaCodigo" runat="server" Text="Debe agregar el código del repuesto" ForeColor="Black" BackColor="Red"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>Descripción: </td>
                <td>
                    <asp:TextBox MaxLength="70" ID="txtDescripcion" runat="server" 
                        ClientIDMode="Static"></asp:TextBox>
                    <asp:Label ID="lblValidaDescrip" runat="server" Text="Debe agregar una descripcion" ForeColor="Black" BackColor="Red"></asp:Label>
                </td>
            </tr>
            <tr>
            <td colspan="2">
                <asp:FileUpload ID="subirImagen" runat="server" ClientIDMode="Static" /></td>
            </tr>
            <tr>        
                <td colspan="2" align="center">
                    <asp:Button ID="btnAgregaAccesorio" runat="server" Text="Agregar" 
                        ClientIDMode="Static" onclick="btnAgregaAccesorio_Click" />
                    &nbsp;
                    <asp:Button ID="btnRestaurar" runat="server" Text="Restaurar" 
                        ClientIDMode="Static" onclick="btnRestaurar_Click" />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
