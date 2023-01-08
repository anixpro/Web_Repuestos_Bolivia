<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="SucursalesEditar.aspx.cs" Inherits="Vistas_SucursalesEditar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <asp:HiddenField ID="hdIdSucursal" runat="server"></asp:HiddenField>
    <asp:HiddenField ID="hdNumeroFactura" runat="server"></asp:HiddenField>
    <asp:HiddenField ID="hdShipCodeAntiguo" runat="server"></asp:HiddenField>
    <div class="titulo">
        Editando sucursal
    </div>
    <div id="msjesError" class="msjesError" runat="server"></div>
    <div class="contenidoCentralJustificado">
        <table align="center" style="width: 454px">
            <thead>
                <tr>
                    <th>Dato</th>
                    <th>Valor</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td>Destinatario Mercancía :</td>
                    <td><asp:TextBox ID="shipCode" runat="server" MaxLength="40" Width="300px"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Dirección :</td>
                    <td><asp:TextBox ID="direccionSucursal" runat="server" MaxLength="70" Width="300px"></asp:TextBox></td>
                </tr>
            </tbody>
        </table>
        <br />
        <br />
        <center>
            <asp:Button ID="btnEditar" runat="server" Text="Editar Sucursal y Volver" 
                onclick="btnEditar_Click"/>
            &nbsp;
            <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" 
                onclick="btnCancelar_Click"/>
        </center>
    </div>
</asp:Content>

