<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="MantenedorGrupoMaterial_Web_Insertar.aspx.cs" Inherits="Vistas_MantenedorGrupoMaterial_Web_Insertar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<div class="titulo">
        Mantenedor de Grupo Material Web
    </div>
<div class="mantenedorUsuariosEditarIzquierda">
        <center><b><u>Mantención Grupo Material</u></b></center>
        <ul>
             <li>
                <asp:HyperLink ID="ActualizaGMW" runat="server" ClientIDMode="Static" 
                    NavigateUrl="~/Vistas/MantenedorGrupoMaterial_Web.aspx">Habilitar, Deshabilitar
                </asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="InsertaGMW" runat="server" ClientIDMode="Static" 
                    NavigateUrl="~/Vistas/MantenedorGrupoMaterial_Web_Insertar.aspx">Insertar
                </asp:HyperLink>
            </li>
           
        </ul>
    </div>

     <div class="mantenedorConcesionariosDerecha">
     <b><u>Formulario de ingreso</u></b>
     <table>
         <tr>
             <td>Codigo Grupo Material Web</td>
             <td><asp:TextBox ID="TxtGMWeb" runat="server"></asp:TextBox></td>
         </tr>
         <tr>
             <td>Descripción</td>
             <td><asp:TextBox ID="TxtGMDesc" runat="server"></asp:TextBox></td>
         </tr>
         <tr>
             <td>Grupo Material SAP</td>
             <td><asp:TextBox ID="TxtGrupoMaterial" runat="server"></asp:TextBox></td>
         </tr>
         <tr>
             <td>Activado</td>
             <td>
             <asp:DropDownList ID="DropDownList1" runat="server">
             <asp:ListItem Value="-1">...</asp:ListItem>
             <asp:ListItem Value="SI">SI</asp:ListItem>
             <asp:ListItem Value="NO">NO</asp:ListItem>
             </asp:DropDownList>
             </td>
         </tr>
          <tr>
             <td></td>
             <td><asp:Button ID="Button1" runat="server" Text="Enviar" onclick="Button1_Click" /></td>
          </tr>
          <tr>
             <td></td>
             <td><asp:Label ID="Label1" runat="server" Text="Label" Visible="false" Font-Bold="true" ForeColor="Red"></asp:Label></td>
          </tr>
     </table>

     </div>

</asp:Content>

