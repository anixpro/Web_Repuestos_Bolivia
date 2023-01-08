<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="Devolucion_CDR_Realizadas.aspx.cs" Inherits="Vistas_Devolucion_CDR_Realizadas" %>

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

    <div class="titulo" id="titCotizar">
        Solicitudes de devolución Realizadas
    </div>
    <div id="msjError" runat="server" clientidmode="Static" style="color: #FF0000; font-size: 25px;">
    </div>
    <div id="msjesError" runat="server" clientidmode="Static">
    </div>
    <div style="overflow-x: auto; width: 100%">
               <asp:GridView ID="grSolicitudesDetalle" runat="server" EmptyDataText="No se encontraron registros" AutoGenerateColumns="false" AllowPaging="True" PageSize="20">
                    <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                    <RowStyle Font-Size="10px" BackColor="#EEEEEE" ForeColor="Black" />
                    <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                    <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" Font-Names="Tahoma"
                        Font-Size="XX-Small" />
                    <Columns>
                        <asp:BoundField DataField="id_solicitud" HeaderText="N° Solicitud" InsertVisible="False"
                            ReadOnly="True" />
                        <asp:BoundField DataField="zona" HeaderText="Zona" InsertVisible="False"
                            ReadOnly="True" />
                        <asp:BoundField DataField="nro_factura" HeaderText="Numero Factura" InsertVisible="False"
                            ReadOnly="True" />
                        <asp:BoundField DataField="pos_factura" HeaderText="Poscición" InsertVisible="False"
                            ReadOnly="True" />                       
                        <asp:BoundField DataField="cantidad" HeaderText="Cantidad" InsertVisible="False"
                            ReadOnly="True" />
                        <asp:BoundField DataField="codigo_respuesto" HeaderText="Codigo Repuesto" InsertVisible="False"
                            ReadOnly="True" />
                        <asp:BoundField DataField="descripcion_repuesto" HeaderText="Descripción" InsertVisible="False" ReadOnly="True" />
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
                        <asp:BoundField DataField="fecha_recepcion_cdr" HeaderText="Fecha Recepción CDR"
                            InsertVisible="False" ReadOnly="True" />
                        <asp:BoundField DataField="responsable_cdr" HeaderText="Responsable CDR"
                            InsertVisible="False" ReadOnly="True" />
                        <asp:BoundField DataField="observacion_cdr" HeaderText="Observación CDR"
                            InsertVisible="False" ReadOnly="True" />
                        <asp:BoundField DataField="nro_nota_credito" HeaderText="N° Pedido NC" InsertVisible="False"
                            ReadOnly="True" />
                        <asp:BoundField DataField="aprobacion_final_crd" HeaderText="Aprueba CDR" InsertVisible="False"
                            ReadOnly="True" />
                        <asp:BoundField DataField="fecha_aprob_final_cdr" HeaderText="Fecha Aprueba/Rechaza CDR"
                            InsertVisible="False" ReadOnly="True" />
                        <asp:BoundField DataField="link_nc" HeaderText="Nota Credito" InsertVisible="False"
                            ReadOnly="True" />
                            <asp:BoundField DataField="RECHAZO" HeaderText="Motivo" InsertVisible="False"
                            ReadOnly="True" />
                            <asp:BoundField DataField="DIAS_FAC" HeaderText="Dias desde solicitud hasta aprobación" InsertVisible="False"
                            ReadOnly="True" />
                            <asp:BoundField DataField="DIAS_FAC_CDR" HeaderText="Dias desde llegada a CDR hasta aprobación" InsertVisible="False"
                            ReadOnly="True" />
                    </Columns>
                </asp:GridView>
                </div>
 <div align="right">
     <asp:HyperLink ID="HyperLink2" NavigateUrl="~/Vistas/Devolucion_CDR.aspx" runat="server">Volver</asp:HyperLink>
    </div>
    <table >
                    <tr>
                                                <td align="left">
                            <asp:Button ID="btnExportar" runat="server" Text="Exportar" onclick="btnExportar_Click" 
                               />
                        </td>
                    </tr>
                </table>
   
</asp:Content>
