<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="Devolucion_CDR_Reclamos.aspx.cs" Inherits="Vistas_Devolucion_CDR_Reclamos" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="updPagina" UpdateMode="Conditional" runat="server">
        <ContentTemplate>
            Solicitudes Reclamos </div>
            <div id="msjError" runat="server" clientidmode="Static" style="color: #FF0000; font-size: 25px;">
            </div>
            <div id="msjesError" runat="server" clientidmode="Static">
            </div>
            <b><u>Solicitudes Reclamos</u></b>
            <div align="right">
                <asp:HyperLink ID="hyCerradasDevoluciones" NavigateUrl="~/Vistas/Devolucion_CDR_Reclamos_Realizadas.aspx"
                    runat="server">Ver Cerradas</asp:HyperLink>
            </div>
            <div style="overflow-x: auto; width: 100%">
                <asp:Panel ID="Panel1" runat="server">
                    <asp:GridView ID="grSolicitudesDetalleReclamos" runat="server" AutoGenerateColumns="False"
                        AllowPaging="false" PageSize="20">
                        <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                        <RowStyle Font-Size="10px" BackColor="#EEEEEE" ForeColor="Black" />
                        <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                        <RowStyle HorizontalAlign="Center" />
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
                            <asp:BoundField DataField="desc_motivo" HeaderText="Motivo Reclamo" InsertVisible="False"
                                ReadOnly="True" HeaderStyle-HorizontalAlign="Center">
                                <ControlStyle Width="20px" />
                                <FooterStyle Width="20px" Wrap="False" />
                                <HeaderStyle Width="20px" Wrap="False" />
                                <ItemStyle Width="110px" Wrap="False" />
                            </asp:BoundField>
                            <asp:BoundField DataField="fecha_solicitud" HeaderText="Fecha Reclamo" InsertVisible="False"
                                ReadOnly="True" HeaderStyle-HorizontalAlign="Center">
                                <ControlStyle Width="20px" />
                                <FooterStyle Width="20px" Wrap="False" />
                                <HeaderStyle Width="20px" Wrap="False" />
                                <ItemStyle Width="110px" Wrap="False" />
                            </asp:BoundField>
                            <asp:BoundField DataField="zona" HeaderText="SHIP CODE" InsertVisible="False" ReadOnly="True"
                                HeaderStyle-HorizontalAlign="Center">
                                <ControlStyle Width="20px" />
                                <FooterStyle Width="20px" Wrap="False" />
                                <HeaderStyle Width="20px" Wrap="False" />
                                <ItemStyle Width="110px" Wrap="False" />
                            </asp:BoundField>
                            <asp:BoundField DataField="nombreconcesionario" HeaderText="Cliente" InsertVisible="False"
                                ReadOnly="True" HeaderStyle-HorizontalAlign="Center">
                                <ControlStyle Width="20px" />
                                <FooterStyle Width="20px" Wrap="False" />
                                <HeaderStyle Width="20px" Wrap="False" />
                                <ItemStyle Width="110px" Wrap="False" />
                            </asp:BoundField>
                            <asp:BoundField DataField="nro_factura" HeaderText="Numero Factura" InsertVisible="False"
                                ReadOnly="True" HeaderStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="pos_factura" HeaderText="Poscición" InsertVisible="False"
                                ReadOnly="True" HeaderStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="PrefijoMarca" HeaderText="Marca" InsertVisible="False"
                                ReadOnly="True" HeaderStyle-HorizontalAlign="Center" />
                            <%--  <asp:BoundField DataField="valor_neto" HeaderText="Neto" InsertVisible="False" ReadOnly="True" />--%>
                            <%--<asp:BoundField DataField="cantidad" HeaderText="Cantidad" InsertVisible="False"
                            ReadOnly="True" />--%>
                            <asp:BoundField DataField="codigo_respuesto" HeaderText="Codigo Repuesto" InsertVisible="False"
                                ReadOnly="True" HeaderStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="descripcion_repuesto" HeaderText="Descripción" InsertVisible="False"
                                ReadOnly="True" HeaderStyle-HorizontalAlign="Center">
                                <ControlStyle Width="20px" />
                                <FooterStyle Width="20px" Wrap="False" />
                                <HeaderStyle Width="20px" Wrap="False" />
                                <ItemStyle Width="110px" Wrap="False" />
                            </asp:BoundField>
                            <asp:BoundField DataField="cantidad_RD" HeaderText="Cantidad Reclamada" InsertVisible="False"
                                ReadOnly="True" HeaderStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="nrotransporte" HeaderText="N° Boleto Transporte" InsertVisible="False"
                                ReadOnly="True" HeaderStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="fecha_factura" HeaderText="Fecha Factura" InsertVisible="False"
                                ReadOnly="True" HeaderStyle-HorizontalAlign="Center">
                                <ControlStyle Width="5px" />
                                <FooterStyle Width="5px" Wrap="False" />
                                <HeaderStyle Width="5px" Wrap="False" />
                                <ItemStyle Width="10px" Wrap="False" />
                            </asp:BoundField>
                            <asp:BoundField DataField="DIAS_FAC" HeaderText="Dias desde reclamo" InsertVisible="False"
                                ReadOnly="True" HeaderStyle-HorizontalAlign="Center" />
                            <asp:TemplateField Visible="true" HeaderText="Evidencia">
                                <ItemTemplate>
                                    <asp:HyperLink ID="HyperLink1" runat="server" Target="_blank" NavigateUrl='<%# Eval("Evidencia") %>'>Ver</asp:HyperLink>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="true" HeaderText="Espera Analista">
                                <ItemTemplate>
                                    <asp:CheckBox ID="rdAnalista" runat="server" AutoPostBack="true" OnCheckedChanged="rdAnalista_CheckedChanged1"
                                        Checked='<%# Eval("espera_analista") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="true" HeaderText="Aprobar">
                                <ItemTemplate>
                                    <asp:RadioButton ID="rdAprobar" runat="server" GroupName="AR" AutoPostBack="true"
                                        OnCheckedChanged="rdAprobar_CheckedChanged" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="true" HeaderText="Responsable">
                                <ItemTemplate>
                                    <asp:DropDownList ID="drResponsable" runat="server">
                                        <asp:ListItem Value="CDR">CDR</asp:ListItem>
                                        <asp:ListItem Value="CONCESIONARIO">CONCESIONARIO</asp:ListItem>
                                        <asp:ListItem Value="FABRICA">FABRICA</asp:ListItem>
                                        <asp:ListItem Value="TRANSPORTEXPO">TRANSPORTE XPO</asp:ListItem>
                                        <asp:ListItem Value="TRANSPORTECOURIER">TRANSPORTE COURIER</asp:ListItem>
                                    </asp:DropDownList>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="true" HeaderText="Rechazar">
                                <ItemTemplate>
                                    <asp:RadioButton ID="rdRechazar" runat="server" GroupName="AR" AutoPostBack="true"
                                        OnCheckedChanged="rdRechazar_CheckedChanged" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="true" HeaderText="Motivo Rechazo">
                                <ItemTemplate>
                                    <asp:DropDownList ID="drMoRechazo" runat="server" Width="60px">
                                    </asp:DropDownList>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="true" HeaderText="Comentario">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtComentario" TextMode="MultiLine" runat="server"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <table>
                        <tr>
                            <td align="left">
                                <asp:Button ID="btnGuardarReclamo" runat="server" Text="Guardar" OnClick="btnGuardarReclamo_Click" />
                            </td>
                            <td align="left">
                                <asp:Button ID="btnExportar" runat="server" Text="Exportar" OnClick="btnExportar_Click" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div class="titulo" id="titCotizar">
</asp:Content>
