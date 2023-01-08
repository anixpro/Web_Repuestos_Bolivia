<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="Devolucion_Comercial_Control.aspx.cs" Inherits="Vistas_Devolucion_Comercial_Control" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="titulo" id="titCotizar">
        Solicitudes Devoluciones Pendientes
    </div>
    <div id="msjError" runat="server" clientidmode="Static" style="color: #FF0000; font-size: 25px;">
    </div>
    <div id="msjesError" runat="server" clientidmode="Static">
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
                <asp:BoundField DataField="nro_factura" HeaderText="N° Factura" InsertVisible="False"
                    ReadOnly="True" />
                <asp:BoundField DataField="nombre" HeaderText="Nombre" InsertVisible="False" ReadOnly="True" />
                <asp:BoundField DataField="NombreConcesionario" HeaderText="Concesionario" InsertVisible="False"
                    ReadOnly="True" />
                <asp:BoundField DataField="DIAS_FAC" HeaderText="N° Dias fecha factura" InsertVisible="False"
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
                <asp:BoundField DataField="DIAS_FAC_SOL" HeaderText="N° Dias desde solicitud" InsertVisible="False"
                    ReadOnly="True" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
