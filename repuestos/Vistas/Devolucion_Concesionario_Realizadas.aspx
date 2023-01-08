<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="Devolucion_Concesionario_Realizadas.aspx.cs" Inherits="Vistas_Devolucion_Concesionario_Realizadas" %>
    <%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .ModalBackgroud
        {
            background-color: Gray;
            filter: alpha(opacity=50);
            opacity: 0.5;
        }
    </style>

    <script type="text/javascript">
        function format(input) {
            var num = input.value.replace(/\./g, '');
            if (!isNaN(num)) {
                input.value = num;
            }
            else {
                input.value = input.value.replace(/[^\d\.]*/g, '');
            }
        }   
    
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="titulo" id="titCotizar">
        Solicitudes Devolución Realizadas
    </div>
    <div id="msjError" runat="server" clientidmode="Static" style="color: #FF0000; font-size: 25px;">
    </div>
    <div id="msjesError" runat="server" clientidmode="Static">
    </div>
    <div class="mantenedorUsuariosIzquierda">
        <center>
            <b><u>Solicitudes Devoluciones</u></b></center>
        <ul>
            <li>
                <asp:HyperLink ID="hlkSolCotizacion" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/Devolucion_Concesionario.aspx">Devoluciones
                </asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="hlkConsultaCotizacion" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/Devolucion_Concesionario_Realizadas.aspx">Realizadas
                </asp:HyperLink>
            </li>
        </ul>
    </div>

    <div style="margin: 10px">
        <table>
            <tr>
                <td>
                    N° Solicitud
                </td>
                <td>
                    <asp:TextBox ID="txtSolId" onkeyup='format(this);' placeholder="N° Solicitud" runat="server"></asp:TextBox>
                </td>
                <td>
                    N° Factura
                </td>
                <td>
                    <asp:TextBox ID="txtNumFactura" onkeyup='format(this);' placeholder="N° Factura" runat="server"></asp:TextBox>
                </td>
                <td>
                    <asp:Button runat="server" ID="btnFiltrar" CssClass="button" Text="Filtrar"  OnClick="btnFiltrar_Click" />
                </td>
            </tr>
        </table>
    </div>
    <asp:GridView ID="grSolicitudes" runat="server" EmptyDataText="No se encontraron registros" AutoGenerateColumns="false" OnRowCommand="grSolicitudes_RowCommand" AllowPaging="True" PageSize="20" OnPageIndexChanging="grSolicitudes_PageIndexChanging">
        <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
        <RowStyle Font-Size="10px" BackColor="#EEEEEE" ForeColor="Black" />
        <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
        <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" Font-Names="Tahoma"
            Font-Size="XX-Small" />
        <Columns>
            <asp:BoundField DataField="id_solicitud" HeaderText="Solicitud" InsertVisible="False"
                ReadOnly="True" />
            <asp:BoundField DataField="nro_factura" HeaderText="Numero Factura" InsertVisible="False"
                ReadOnly="True" />
            <asp:BoundField DataField="nombre" HeaderText="Usuario Solicitud" InsertVisible="False"
                ReadOnly="True" />
            <asp:BoundField DataField="NombreConcesionario" HeaderText="Concesionario" InsertVisible="False"
                ReadOnly="True" />
            <asp:ButtonField ButtonType="Image" HeaderText="" ItemStyle-HorizontalAlign="Center"
                CommandName="detalle" Visible="true" ImageUrl="~/img/ver.gif" />
        </Columns>
    </asp:GridView>
    <asp:Panel ID="Panel1" runat="server" CssClass="modalPopup" align="center" Style="display: none;
        width: 80%; height: auto">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <table width="80%">
                    <tr>
                        <td align="right">
                            <asp:Button ID="btnCerrar" runat="server" Text="X" />
                        </td>
                    </tr>
                </table>
                <asp:GridView ID="grSolicitudesDetalle" runat="server" AutoGenerateColumns="false">
                    <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                    <RowStyle Font-Size="10px" BackColor="#EEEEEE" ForeColor="Black" />
                    <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                    <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" Font-Names="Tahoma"
                        Font-Size="XX-Small" />
                    <Columns>
                        <asp:BoundField DataField="id_solicitud" HeaderText="N° Solicitud" InsertVisible="False"
                            ReadOnly="True" />
                        <asp:BoundField DataField="nro_factura" HeaderText="Numero Factura" InsertVisible="False"
                            ReadOnly="True" />
                        <asp:BoundField DataField="pos_factura" HeaderText="Poscición" InsertVisible="False"
                            ReadOnly="True" />
                        <asp:BoundField DataField="valor_neto" HeaderText="Neto" InsertVisible="False" ReadOnly="True" />
                        <asp:BoundField DataField="cantidad" HeaderText="Cantidad" InsertVisible="False"
                            ReadOnly="True" />
                        <asp:BoundField DataField="codigo_respuesto" HeaderText="Codigo Repuesto" InsertVisible="False"
                            ReadOnly="True" />
                        <asp:BoundField DataField="descripcion_repuesto" HeaderText="Descripción" InsertVisible="False"
                            ReadOnly="True" />
                        <asp:BoundField DataField="cantidad_RD" HeaderText="Cantidad Solicitada" InsertVisible="False"
                            ReadOnly="True" />
                        <asp:BoundField DataField="fecha_solicitud" HeaderText="Fecha Solicitud" InsertVisible="False"
                            ReadOnly="True" />
                        <asp:BoundField DataField="desc_motivo" HeaderText="Motivo" InsertVisible="False"
                            ReadOnly="True" />
                        <asp:BoundField DataField="evidencia" HeaderText="Evidencias" InsertVisible="False"
                            ReadOnly="True" />
                        <asp:BoundField DataField="aprueba_comercial" HeaderText="Aprobado" InsertVisible="False"
                            ReadOnly="True" />
                        <asp:BoundField DataField="fecha_aprob_comercial" HeaderText="Fecha Aprobación" InsertVisible="False"
                            ReadOnly="True" />
                        <asp:BoundField DataField="flag_doble_aprobacion" HeaderText="Aprobacion Adicional"
                            InsertVisible="False" ReadOnly="True" />
                        <asp:BoundField DataField="aprueba_comercial_2" HeaderText="Aprobado Jefe" InsertVisible="False"
                            ReadOnly="True" />
                        <asp:BoundField DataField="fecha_aprob_comercial_2" HeaderText="Fecha Aprobación Jefe"
                            InsertVisible="False" ReadOnly="True" />
                        <asp:BoundField DataField="nro_nota_credito" HeaderText="N° Pedido Devolución" InsertVisible="False"
                            ReadOnly="True" />
                    </Columns>
                </asp:GridView>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>
    <asp:Panel ID="Panel2" runat="server" Visible="true">
        <asp:Button ID="Button1" runat="server" Text="" />
    </asp:Panel>
    <asp:ModalPopupExtender ID="ModalPopupExtender1" runat="server" PopupControlID="Panel1"
        CancelControlID="btnCerrar" BackgroundCssClass="ModalBackgroud" TargetControlID="Button1">
    </asp:ModalPopupExtender>
</asp:Content>
