<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="Devolucion_Comercial.aspx.cs" Inherits="Vistas_Devolucion_Comercial" %>

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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="titulo" id="titCotizar">
        Solicitudes Devolución
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
                <asp:HyperLink ID="hlkSolCotizacion" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/Devolucion_Comercial.aspx">Devoluciones
                </asp:HyperLink>
            </li>
          </ul>
    </div>
    <asp:GridView ID="grSolicitudes" runat="server" AutoGenerateColumns="false" EmptyDataText="No se encontraron registros" OnRowCommand="grSolicitudes_RowCommand" AllowPaging="True" PageSize="20">
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
            <asp:BoundField DataField="DIAS_FAC" HeaderText="Dias desde fecha factura" InsertVisible="False"
                ReadOnly="True" />
            <asp:BoundField DataField="usu_aprueba_comercial" HeaderText="Supervisor (aprueba)"
                InsertVisible="False" ReadOnly="True" />
            <asp:BoundField DataField="fecha_aprob_comercial" HeaderText="Fecha Aprobación" InsertVisible="False"
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
                        <asp:TemplateField Visible="true" HeaderText="Confirmar">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkConfirmar" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
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
                        <asp:TemplateField Visible="true" HeaderText="Evidencia">
                            <ItemTemplate>
                                <asp:HyperLink ID="HyperLink1" runat="server" Target="_blank" NavigateUrl='<%# Eval("Evidencia") %>'>Ver</asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField Visible="true" HeaderText="Aprobar">
                            <ItemTemplate>
                                <asp:RadioButton ID="rdAprobar" runat="server" GroupName="AR" AutoPostBack="true"
                                    OnCheckedChanged="rdAprobar_CheckedChanged" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField Visible="true" HeaderText="Rechazar">
                            <ItemTemplate>
                                <asp:RadioButton ID="rdRechazar" runat="server" GroupName="AR" AutoPostBack="true"
                                    OnCheckedChanged="rdRechazar_CheckedChanged" />
                            </ItemTemplate>
                        </asp:TemplateField>
                         <asp:BoundField DataField="nro_nota_credito" HeaderText="N° Pedido Devolución" InsertVisible="False"
                            ReadOnly="True" />                       
                        <asp:TemplateField Visible="true" HeaderText="Motivo Rechazo">
                            <ItemTemplate>
                                <asp:DropDownList ID="drMoRechazo" runat="server" Width="60px">
                                </asp:DropDownList>
                            </ItemTemplate>
                        </asp:TemplateField>
                         <asp:BoundField DataField="observacion_comercial" HeaderText="Observación" InsertVisible="False"
                            ReadOnly="True" />
                            <asp:BoundField DataField="prefijo" HeaderText="" InsertVisible="False"
                            ReadOnly="True" FooterStyle-CssClass="invi" ItemStyle-CssClass="invi" HeaderStyle-CssClass="invi" />
                    </Columns>
                </asp:GridView>
                <table width="80%">
                    <tr>
                        <td align="left">
                            <asp:Button ID="btnGuardar" runat="server" Text="Guardar" OnClick="btnGuardar_Click" />
                        </td>
                    </tr>
                </table>
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
