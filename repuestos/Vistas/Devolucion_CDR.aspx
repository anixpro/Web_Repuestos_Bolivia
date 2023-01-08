<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="Devolucion_CDR.aspx.cs" Inherits="Vistas_Devolucion_CDR" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="titulo" id="titCotizar">
        Solicitudes Devoluciones
    </div>
    <div id="msjError" runat="server" clientidmode="Static" style="color: #FF0000; font-size: 25px;">
    </div>
    <div id="msjesError" runat="server" clientidmode="Static">
    </div>
    <b><u>Solicitudes Devolución</u></b>
    <table width="100%">
        <tr>
            <td width="12%">
                N° Aprobación
            </td>
            <td width="15%">
                <asp:TextBox ID="txtAprobacion" runat="server"></asp:TextBox>
            </td>
            <td width="12%">
                Ship Code
            </td>
            <td width="15%">
                <asp:TextBox ID="txtshipcode" runat="server"></asp:TextBox>
            </td>
            <td width="12%">
                Fecha Recepción
            </td>
            <td width="15%">
                <asp:TextBox ID="txtfecha" runat="server" Width="80px" Style="text-transform: uppercase"
                    AutoPostBack="true"></asp:TextBox>
                <asp:ImageButton runat="Server" ID="btnDesde" ImageUrl="~/img/calendarIcon.png" AlternateText="Click here to display calendar" />
                <asp:CalendarExtender ID="fecDesde" runat="server" TargetControlID="txtfecha" PopupButtonID="btnDesde"
                    Format="yyyy-MM-dd" />
            </td>
            <td>
                <asp:ImageButton ID="Buscar" runat="server" ImageUrl="~/img/buscarlupa.png" OnClick="Buscar_Click" />
            </td>
    </table>
    <div align="right">
        <asp:ImageButton ID="btnRefrescar" runat="server" ImageUrl="~/img/refrescar.png"
            OnClick="btnRefrescar_Click" />
    </div>
    <table>
        <tr>
            <td align="left">
                <asp:Button ID="btnGuardarDevolucion" runat="server" Text="Guardar" OnClick="btnGuardarDevolucion_Click" />
            </td>
            <td align="left">
                <asp:Button ID="btnExportar" runat="server" Text="Exportar" OnClick="btnExportar_Click" />
            </td>
        </tr>
    </table>
    <div style="overflow-x: auto; width: 100%">
        <asp:Panel ID="Panel3" runat="server">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:GridView ID="grSolicitudesDetalleDevolucion" runat="server" AutoGenerateColumns="false"
                        EmptyDataText="No se encontraron registros">
                        <RowStyle Font-Size="10px" BackColor="#EEEEEE" ForeColor="Black" />
                        <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                        <RowStyle HorizontalAlign="Center" />
                        <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" Font-Names="Tahoma"
                            Font-Size="XX-Small" />
                        <Columns>
                            <asp:TemplateField Visible="true" HeaderText="Confirmar">
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkConfirmarDev" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="id_solicitud" HeaderText="N° Solicitud" InsertVisible="False"
                                ReadOnly="True" />
                            <asp:BoundField DataField="nro_factura" HeaderText="Numero Factura" InsertVisible="False"
                                ReadOnly="True" />
                            <asp:BoundField DataField="pos_factura" HeaderText="Poscición" InsertVisible="False"
                                ReadOnly="True" />
                            <asp:BoundField DataField="zona" HeaderText="SHIP CODE" InsertVisible="False" ReadOnly="True">
                                <ControlStyle Width="20px" />
                                <FooterStyle Width="20px" Wrap="False" />
                                <HeaderStyle Width="20px" Wrap="False" />
                                <ItemStyle Width="110px" Wrap="False" />
                            </asp:BoundField>
                            <asp:BoundField DataField="nombreconcesionario" HeaderText="Cliente" InsertVisible="False"
                                ReadOnly="True">
                                <ControlStyle Width="20px" />
                                <FooterStyle Width="20px" Wrap="False" />
                                <HeaderStyle Width="20px" Wrap="False" />
                                <ItemStyle Width="110px" Wrap="False" />
                            </asp:BoundField>
                            <asp:BoundField DataField="usu_aprueba_comercial" HeaderText="Supervisor" InsertVisible="False"
                                ReadOnly="True" />
                            <asp:BoundField DataField="PrefijoMarca" HeaderText="Marca" InsertVisible="False"
                                ReadOnly="True" />
                            <asp:BoundField DataField="codigo_respuesto" HeaderText="Codigo Repuesto" InsertVisible="False"
                                ReadOnly="True" />
                            <asp:BoundField DataField="descripcion_repuesto" HeaderText="Descripción" InsertVisible="False"
                                ReadOnly="True">
                                <ControlStyle Width="20px" />
                                <FooterStyle Width="20px" Wrap="False" />
                                <HeaderStyle Width="20px" Wrap="False" />
                                <ItemStyle Width="110px" Wrap="False" />
                            </asp:BoundField>
                            <asp:BoundField DataField="cantidad_RD" HeaderText="Cantidad Devolución" InsertVisible="False"
                                ReadOnly="True" />
                            <asp:BoundField DataField="nro_nota_credito" HeaderText="N° Aprobación" InsertVisible="False"
                                ReadOnly="True" />
                            <asp:BoundField DataField="fecha_solicitud" HeaderText="Fecha Solicitud" InsertVisible="False"
                                ReadOnly="True">
                                <ControlStyle Width="20px" />
                                <FooterStyle Width="20px" Wrap="False" />
                                <HeaderStyle Width="20px" Wrap="False" />
                                <ItemStyle Width="110px" Wrap="False" />
                            </asp:BoundField>
                            <asp:BoundField DataField="desc_motivo" HeaderText="Motivo" InsertVisible="False"
                                ReadOnly="True" />
                            <asp:TemplateField Visible="true" HeaderText="Evidencia">
                                <ItemTemplate>
                                    <asp:HyperLink ID="HyperLink1" runat="server" Target="_blank" NavigateUrl='<%# Eval("evidencia") %>'>Ver</asp:HyperLink>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="fecha_factura" HeaderText="Fecha Factura" InsertVisible="False"
                                ReadOnly="True">
                                <ControlStyle Width="20px" />
                                <FooterStyle Width="20px" Wrap="False" />
                                <HeaderStyle Width="20px" Wrap="False" />
                                <ItemStyle Width="110px" Wrap="False" />
                            </asp:BoundField>
                            <asp:BoundField DataField="DIAS_FAC_1" HeaderText="Dias Emisión Factura" InsertVisible="False"
                                ReadOnly="True" ItemStyle-ForeColor="Red" ItemStyle-Font-Bold="true" />
                            <asp:TemplateField Visible="true" HeaderText="URL Factura">
                                <ItemTemplate>
                                    <asp:HyperLink ID="hypDocND" runat="server" Target="_blank" NavigateUrl='<%# Eval("url_factura") %>'>Ver</asp:HyperLink>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="true" HeaderText="Recepción Documentación">
                                <ItemTemplate>
                                    <asp:CheckBox ID="chRecibo" runat="server" AutoPostBack="true" Checked='<%#Convert.ToBoolean(Eval("recepcion_cdr")) %>'
                                        OnCheckedChanged="chRecibo_CheckedChanged" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="true" HeaderText="Recepción Material">
                                <ItemTemplate>
                                    <asp:CheckBox ID="chRecibomaterial" runat="server" AutoPostBack="true" Checked='<%#Convert.ToBoolean(Eval("recepcion_cdr_material")) %>'
                                        OnCheckedChanged="chRecibomaterial_CheckedChanged" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="fecha_recepcion_cdr_material" HeaderText="Fecha Recepción Material"
                                InsertVisible="False" ReadOnly="True">
                                <ControlStyle Width="20px" />
                                <FooterStyle Width="20px" Wrap="False" />
                                <HeaderStyle Width="20px" Wrap="False" />
                                <ItemStyle Width="110px" Wrap="False" />
                            </asp:BoundField>
                            <asp:TemplateField Visible="true" HeaderText="Aprobar Material">
                                <ItemTemplate>
                                    <asp:CheckBox ID="chAprobmaterial" runat="server" AutoPostBack="true" Checked='<%#Convert.ToBoolean(Eval("aprobar_recepcion_cdr_material")) %>'
                                        OnCheckedChanged="chAprobmaterial_CheckedChanged" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Fecha_CDR" HeaderText="Dias desde llegada a CDR" InsertVisible="False"
                                ReadOnly="True" />
                            <asp:BoundField DataField="responsable_cdr" HeaderText="Responsable" InsertVisible="False"
                                ReadOnly="True" />
                            <asp:TemplateField Visible="true" HeaderText="Observación">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtobservacionGR" runat="server" Text='<%# Eval("observacion_cdr") %>'></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="true" HeaderText="Aprobar">
                                <ItemTemplate>
                                    <asp:RadioButton ID="rdAprobarDev" runat="server" GroupName="ARDev" AutoPostBack="true"
                                        OnCheckedChanged="rdAprobarDev_CheckedChanged" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="true" HeaderText="Rechazar">
                                <ItemTemplate>
                                    <asp:RadioButton ID="rdRechazarDev" runat="server" GroupName="ARDev" AutoPostBack="true"
                                        OnCheckedChanged="rdRechazarDev_CheckedChanged" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="true" HeaderText="Motivo Rechazo">
                                <ItemTemplate>
                                    <asp:DropDownList ID="drMoRechazoDev" runat="server" Width="60px">
                                    </asp:DropDownList>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </ContentTemplate>
            </asp:UpdatePanel>
        </asp:Panel>
    </div>
</asp:Content>
