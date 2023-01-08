<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="reporteInfoAdicionalPedido.aspx.cs" Inherits="Vistas_reporteInfoAdicionalPedido" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<%--    <link rel="stylesheet" href="//netdna.bootstrapcdn.com/bootstrap/3.1.0/css/bootstrap.min.css">
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.4.1/css/bootstrap.min.css" integrity="sha384-Vkoo8x4CGsO3+Hhxv8T/Q5PaXtkKtu6ug5TOeNV6gBiFeWPGFN9MuhOf23Q9Ifjh" crossorigin="anonymous">--%>
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ToolkitScriptManager>
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <br />
    <center>
    <div class="container">
    <table class="table">
        <tr>
            <td>
                Marca
            </td>
            <td>
                <asp:DropDownList ID="ddlMarca" runat="server" AutoPostBack="true" CssClass="form-control" Width="100"></asp:DropDownList>
            </td>
            <td>
                Fecha Desde
            </td>
            <td class="table">
                <table>
                    <tr>
                        <td><asp:TextBox ID="txtDesde" runat="server" Width="70px" Style="text-transform: uppercase" AutoPostBack="true" CssClass="form-control"></asp:TextBox></td>
                        <td><asp:ImageButton runat="Server" ID="btnDesde" ImageUrl="~/img/calendarIcon.png" /></td>
                        <td><asp:CalendarExtender ID="fecDesde" runat="server" TargetControlID="txtdesde" PopupButtonID="btnDesde" Format="dd-MM-yyyy" /></td>                        
                    </tr>
                </table>
            </td>
            <td>
                Fecha Hasta
            </td>
            <td class="table">
                <table>
                    <tr>
                        <td><asp:TextBox ID="txtHasta" runat="server" Width="70px" Style="text-transform: uppercase" AutoPostBack="true" CssClass="form-control"></asp:TextBox></td>
                        <td><asp:ImageButton runat="server" ID="btnHasta" ImageUrl="~/img/calendarIcon.png" /></td>
                        <td><asp:CalendarExtender ID="fechasta" runat="server" TargetControlID="txtHasta" PopupButtonID="btnHasta" Format="dd-MM-yyyy" /></td>                        
                    </tr>
                </table>
            </td>
            <td>
                <asp:Button ID="btnBuscarSoli" runat="server" Text="Buscar" OnClick="btnBuscarSoli_Click" CssClass="btn btn-primary"/>
            </td>
            <td>
                <asp:Button ID="btnExcel" runat="server" Text="Exportar" OnClick="btnExcel_Click" CssClass="btn btn-secondary"/>
            </td>
        </tr>
        
        <!--
        <tr><td>&nbsp;</td></tr>
        -->

        <tr>
            <td colspan="12">
                <asp:GridView ID="dgvPedidosSubClientes" runat="server" ClientIDMode="Static" AllowPaging="True" AllowSorting="True"
                AutoGenerateColumns="False" EnableModelValidation="True" Style="text-align:center;" 
                EmptyDataText="No hay resultados para la busqueda." 
                CellPadding="10" ForeColor="#333333" GridLines="Both" 
                OnPageIndexChanging="dgvPedidosSubClientes_PageIndexChanging">
                <Columns>

                    <asp:BoundField DataField="id_pedido" HeaderText="Pedido" HeaderStyle-CssClass="table-dark"/>
                    <asp:BoundField DataField="e_vbeln" HeaderText="Pedido SAP" HeaderStyle-CssClass="table-dark"/>
                    <asp:BoundField DataField="solicitado_por" HeaderText="Usuario" HeaderStyle-CssClass="table-dark"/>
                    <asp:BoundField DataField="fecha_solicitud" HeaderText="Fecha" HeaderStyle-CssClass="table-dark" DataFormatString="{0:d}"/>
                    <asp:BoundField DataField="estado" HeaderText="Estado" HeaderStyle-CssClass="table-dark"/>
                    <asp:BoundField DataField="total_neto" HeaderText="Total" HeaderStyle-CssClass="table-dark"/>
                    <asp:BoundField DataField="compania" HeaderText="Compañia" HeaderStyle-CssClass="table-dark"/>
                    <asp:BoundField DataField="tipo_sustento" HeaderText="Tipo Sustento" DataFormatString="{0:dd/MM/yyyy}" HeaderStyle-CssClass="table-dark"/>
                    <asp:BoundField DataField="archivo" HeaderText="Archivos" HtmlEncode="False" DataFormatString="<a target='_blank' href='../doc/infoAdicionalPedido/{0}'>Ver</a>" />

                </Columns>
                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <EditRowStyle BackColor="#999999" />
                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                </asp:GridView>
            </td>
        </tr>
    </table>
    </div>
    </center>
</asp:Content>

