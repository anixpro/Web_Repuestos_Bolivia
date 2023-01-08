<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="Devolucion_CDR_Control.aspx.cs" Inherits="Vistas_Devolucion_CDR_Control" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="titulo" id="titCotizar">
        Solicitudes Reclamos y Devoluciones Pendientes
    </div>
    <div id="msjError" runat="server" clientidmode="Static" style="color: #FF0000; font-size: 25px;">
    </div>
    <div id="msjesError" runat="server" clientidmode="Static">
    </div>
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
            <td align="right">
                <asp:HyperLink ID="hyCerradasDevoluciones" NavigateUrl="~/Vistas/Devolucion_CDR_Realizadas_2.aspx"
                    runat="server">Ver Cerradas</asp:HyperLink>
            </td>
        </tr>
    </table>
    <div align="right">
        <asp:ImageButton ID="btnRefrescar" runat="server" ImageUrl="~/img/refrescar.png"
            OnClick="btnRefrescar_Click" />
    </div>
    <div style="overflow-x: auto; width: 100%">
        <asp:GridView ID="grVistaControl" runat="server" AutoGenerateColumns="false" EmptyDataText="No se encontraron registros"
            AllowPaging="True" PageSize="20">
            <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
            <RowStyle Font-Size="10px" BackColor="#EEEEEE" ForeColor="Black" />
            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
            <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" Font-Names="Tahoma"
                Font-Size="XX-Small" />
            <Columns>
                <asp:BoundField DataField="id_solicitud" HeaderText="N° Solicitud" InsertVisible="False"
                    ReadOnly="True" />
                <asp:BoundField DataField="reclamo_devolucion" HeaderText="Tipo Solicitud" InsertVisible="False"
                    ReadOnly="True" />
                <asp:BoundField DataField="nro_factura" HeaderText="N° Factura" InsertVisible="False"
                    ReadOnly="True" />
                <asp:BoundField DataField="Zona" HeaderText="Zona" InsertVisible="False" ReadOnly="True" />
                <asp:BoundField DataField="PrefijoMarca" HeaderText="Marca" InsertVisible="False"
                    ReadOnly="True" />
                <asp:BoundField DataField="usu_aprueba_comercial" HeaderText="VB Comercial" InsertVisible="False"
                    ReadOnly="True" />
                <asp:BoundField DataField="nombreconcesionario" HeaderText="Concesionario" InsertVisible="False"
                    ReadOnly="True" />
                <asp:BoundField DataField="codigo_respuesto" HeaderText="Repuesto" InsertVisible="False"
                    ReadOnly="True" />
                <asp:BoundField DataField="descripcion_repuesto" HeaderText="Descripción Repuesto"
                    InsertVisible="False" ReadOnly="True" />
                <asp:BoundField DataField="fecha_factura" HeaderText="Fecha Factura" InsertVisible="False"
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
                <asp:TemplateField Visible="true" HeaderText="URL Factura">
                    <ItemTemplate>
                        <asp:HyperLink ID="hypDocND" runat="server" Target="_blank" NavigateUrl='<%# Eval("url_factura") %>'>Ver</asp:HyperLink>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="DIAS_FAC" HeaderText="N° Dias fecha factura" InsertVisible="False"
                    ReadOnly="True" />
                <asp:BoundField DataField="Fecha_CDR" HeaderText="N° Dias llegada CDR" InsertVisible="False"
                    ReadOnly="True" />
                <asp:BoundField DataField="responsable_cdr" HeaderText="Responsable CDR" InsertVisible="False"
                    ReadOnly="True" />
                <asp:BoundField DataField="observacion_cdr" HeaderText="Observación CDR" InsertVisible="False"
                    ReadOnly="True" />
                <asp:BoundField DataField="recepcion_cdr" HeaderText="Recepcionado CDR" InsertVisible="False"
                    ReadOnly="True" />
                <asp:BoundField DataField="fecha_recepcion_cdr" HeaderText="Fecha Recepción" InsertVisible="False"
                    ReadOnly="True" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
