<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="ConcesionarioEditar.aspx.cs" Inherits="Vistas_ConcesionarioEditar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <asp:HiddenField ID="hdIdConcesionario" runat="server"/>
    <asp:HiddenField ID="hdAntiguoNombreConcesionario" runat="server"/>
    <div class="titulo">
        Editando Concesionario
    </div>
    <div class="contenidoCentral">
        <div id="msjesError" class="msjesError" runat="server">
        </div>
        <table align="left" style="width: 565px">
            <thead>
                <tr>
                    <th>Dato</th>
                    <th>Valor</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td>Nombre :</td>
                    <td><asp:TextBox ID="nombre" runat="server" MaxLength="50"></asp:TextBox></td>

                </tr>
                <tr>
                    <td>RUT :</td>
                    <td><asp:TextBox ID="RUT" runat="server" MaxLength="11" style="text-align: left"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Cod. Cliente :</td>
                    <td><asp:TextBox ID="txtNumFactura" runat="server" MaxLength="15" 
                            style="text-align: left"></asp:TextBox></td>
                </tr>

                <tr>
                    <td>Administrador :</td>
                    <td>  <asp:DropDownList ID="ddlAdministrador" runat="server" ClientIDMode="Static">
                    </asp:DropDownList></td>
                </tr>
                <tr>
                    <td>Supervisor :</td>
                    <td><asp:DropDownList ID="ddlSupervisor" runat="server" ClientIDMode="Static">
                    </asp:DropDownList></td>
                </tr>
                <tr>
                    <td>Correos VFC: <span class="gridViewRowSmall"><br />
                        (correo1@skberge.cl,correo2@skberge.cl)</span> </td>
                    <td><asp:TextBox MaxLength="350" Width="200" id="txtCorreoVFC" runat="server" 
                            ClientIDMode="Static" style="text-align: left"></asp:TextBox></td>
                </tr>

                <tr>
                <td></td>
                <td><asp:Button ID="btnGuardar" runat="server" Text="Guardar cambios"  
                onclick="btnGuardar_Click" CssClass="button"/><asp:Button ID="btnCancelar" runat="server" Text="Volver" 
                onclick="btnCancelar_Click" CssClass="button" /></td>
                </tr>
            </tbody>
        </table>
        <br />
        <br />
       
    </div>
</asp:Content>
