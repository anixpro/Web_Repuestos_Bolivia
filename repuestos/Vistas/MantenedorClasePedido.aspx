<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="MantenedorClasePedido.aspx.cs" Inherits="Vistas_MantenedorClasePedido" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
        <title></title>
    </head>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="titulo">
        Mantenedor clase de pedoido
    </div>
    <div class="mantenedorClasePedidoIzquierda">
        <center><b><u>Mantención clases pedido</u></b></center>
        <ul>
             <li>
                 <asp:Button ID="BtnInsertarClase" runat="server" ClientIDMode="Static" Text="Crear clase pedido" OnClick="BtnInsertarClase_Click"/>
                  </li>
            <br />
            <li>
                 <asp:Button ID="BtnEdiatClase" runat="server" ClientIDMode="Static" Text="Editar clase pedido" OnClick="BtnEdiatClase_Click"/>
                </li>
             <br />
            <li>
               <asp:Button ID="BtnDesactivarClase" runat="server" ClientIDMode="Static" Text="Desactivar clase pedido" OnClick="BtnDesactivarClase_Click"/>
            </li>
             <br />
            <li>
                <asp:Button ID="BtnClasePedidoUsuario" runat="server" ClientIDMode="Static" Text="Asignar clase pedido usuario" OnClick="BtnClasePedidoUsuario_Click"/>
            </li>
        </ul>
    </div>

<asp:Panel ID="PanelCrearClase" runat="server">
 <div class="mantenedorClasePedidoDerecha" style="height:100%">
      <div id="msjesErrorInsertar" runat="server" clientidmode="Static" class="msjesError"></div>
        <b><u>Crear Clase Pedido</u></b>
        <br />
        <asp:Table ID="tblDatosClasePedido" runat="server" ClientIDMode="Static" CssClass="tablaCrearClasePedido">
            <asp:TableRow ID="TableRow1" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell1" runat="server" ClientIDMode="Static">
                    <asp:Label ID="lblNombre" runat="server" ClientIDMode="Static" Text="Nombre Clase:" CssClass="lblNombre"></asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="TableCell2" runat="server" ClientIDMode="Static">
                    <asp:TextBox MaxLength="50" ID="txtNombreClase" runat="server" ClientIDMode="Static" CssClass="txtNombre"></asp:TextBox>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow2" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell3" runat="server" ClientIDMode="Static">
                    <asp:Label ID="Label1" runat="server" ClientIDMode="Static" Text="Código Clase:" CssClass="lblNombre"></asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="TableCell4" runat="server" ClientIDMode="Static">
                    <asp:TextBox MaxLength="30" ID="TxtCodigoClase" runat="server" ClientIDMode="Static" CssClass="txtNombre"></asp:TextBox>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow  runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell11" runat="server" ClientIDMode="Static" ColumnSpan="2" HorizontalAlign="Center">
                   
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
      <asp:Button ID="btnAgregarClase" runat="server" ClientIDMode="Static" Text="Guardar Nueva Clase" CssClass="button" OnClick="btnAgregarClase_Click"/>
    </div>
</asp:Panel>
<asp:Panel ID="PanelEditarClase" runat="server">
 <div class="mantenedorClasePedidoDerecha" style="height:100%">
      <div id="msjesErrorEditar" runat="server" clientidmode="Static" class="msjesError"></div>
        <b><u>Editar Clase Pedido</u></b>
        <br />
        <asp:Table ID="Table1" runat="server" ClientIDMode="Static" CssClass="tablaEditarClasePedido">
            <asp:TableRow ID="TableRow3" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell5" runat="server" ClientIDMode="Static">
                    <asp:Label ID="Label2" runat="server" ClientIDMode="Static" Text="Seleccione Clase:" CssClass="lblNombre"></asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="TableCell6" runat="server" ClientIDMode="Static">
                    <asp:DropDownList runat="server" ID="ddlClasePedidoEditar" AutoPostBack="true" OnSelectedIndexChanged="ddlClasePedidoEditar_SelectedIndexChanged"></asp:DropDownList>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow4" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell7" runat="server" ClientIDMode="Static">
                    <asp:Label ID="Label3" runat="server" ClientIDMode="Static" Text="Cambiar nombre clase:" CssClass="lblNombre"></asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="TableCell8" runat="server" ClientIDMode="Static">
                    <asp:TextBox MaxLength="30" ID="TxtNombreClaseEditar" runat="server" ClientIDMode="Static" CssClass="txtNombre"></asp:TextBox>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow  runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell9" runat="server" ClientIDMode="Static" ColumnSpan="2" HorizontalAlign="Center">
                   
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
      <asp:Button ID="BtnEditarClase" runat="server" ClientIDMode="Static" Text="Guardar Datos Clase" CssClass="button" OnClick="BtnEditarClase_Click"/>
    </div>
</asp:Panel>
<asp:Panel ID="PanelDesactivarClase" runat="server">
 <div class="mantenedorClasePedidoDerecha" style="height:100%">
     <div id="msjesErrorDesactivar" runat="server" clientidmode="Static" class="msjesError"></div>
        <b><u>Desactivar Clase Pedido</u></b>
        <br />
        <asp:Table ID="Table2" runat="server" ClientIDMode="Static" CssClass="tablaCrearClasePedido">
            <asp:TableRow ID="TableRow5" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell10" runat="server" ClientIDMode="Static">
                    <asp:Label ID="Label4" runat="server" ClientIDMode="Static" Text="Seleccione Clase:" CssClass="lblNombre"></asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="TableCell12" runat="server" ClientIDMode="Static">
                     <asp:DropDownList runat="server" ID="ddlClasePedidoActivar" AutoPostBack="true" OnSelectedIndexChanged="ddlClasePedidoActivar_SelectedIndexChanged"></asp:DropDownList>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow6" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell13" runat="server" ClientIDMode="Static">
                    <asp:Label ID="Label5" runat="server" ClientIDMode="Static" Text="Estado Clase:" CssClass="lblNombre"></asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="TableCell14" runat="server" ClientIDMode="Static">
                    <asp:RadioButtonList ID="rblEstadoClase" runat="server" RepeatDirection="Horizontal">
                        <asp:ListItem Text="Habilitado" Value="1" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="Inhabilitado" Value="0"></asp:ListItem>
                    </asp:RadioButtonList>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow  runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell15" runat="server" ClientIDMode="Static" ColumnSpan="2" HorizontalAlign="Center">
                   
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
      <asp:Button ID="btnDesactivarClasePedido" runat="server" ClientIDMode="Static" Text="Guardar Estado Clase" CssClass="button" OnClick="btnDesactivarClasePedido_Click"/>
    </div>
</asp:Panel>
<asp:Panel ID="PanelAsignarClasePedido" runat="server">
 <div class="mantenedorClasePedidoDerecha" style="height:100%">
     <div id="msjesErrorAsignar" runat="server" clientidmode="Static" class="msjesError"></div>
        <b><u>Asignar clase pedido ausario</u></b>
        <br />
        <asp:Table ID="Table3" runat="server" ClientIDMode="Static" CssClass="tablaCrearClasePedido">
            <asp:TableRow ID="TableRow7" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell16" runat="server" ClientIDMode="Static">
                    <asp:Label ID="Label6" runat="server" ClientIDMode="Static" Text="Seleccione Usuario:" CssClass="lblNombre"></asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="TableCell17" runat="server" ClientIDMode="Static">
                    <asp:DropDownList runat="server" ID="ddlUsuarioAsignar" AutoPostBack="true" OnSelectedIndexChanged="ddlUsuarioAsignar_SelectedIndexChanged"></asp:DropDownList>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRowEspaciadora" runat="server" ClientIDMode="Static" Height="20">
                <asp:TableCell ID="TableCellEspaciadora" runat="server" ClientIDMode="Static" ColumnSpan="2" BackColor="Transparent">
                    &nbsp; <!-- Espacio invisible -->
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow8" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell18" runat="server" ClientIDMode="Static">
                    <asp:Label ID="Label7" runat="server" ClientIDMode="Static" Text="Código Clase:" CssClass="lblNombre"></asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="TableCell19" runat="server" ClientIDMode="Static">
                   
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow  runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell20" runat="server" ClientIDMode="Static" ColumnSpan="2" HorizontalAlign="Center">
                     <asp:CheckBoxList ID="cbxClasePedido" runat="server" ClientIDMode="Static" RepeatDirection="Horizontal" RepeatColumns="4"></asp:CheckBoxList>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow9" runat="server" ClientIDMode="Static" Height="20">
                <asp:TableCell ID="TableCell21" runat="server" ClientIDMode="Static" ColumnSpan="2" BackColor="Transparent">
                    &nbsp; <!-- Espacio invisible -->
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
      <asp:Button ID="btnAignarClasePedido" runat="server" ClientIDMode="Static" Text="Asignar Clase" CssClass="button" OnClick="btnAignarClasePedido_Click"/>
    </div>
</asp:Panel>
</asp:Content>