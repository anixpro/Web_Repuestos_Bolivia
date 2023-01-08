<%@ Page Language="C#" AutoEventWireup="true" CodeFile="detalleSolicitud.aspx.cs" Inherits="ajax_detalleSolicitud" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="font-size:small">
        <asp:GridView ID="gvDetalleSolicitud" runat="server" BackColor="White" 
            BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" 
            ForeColor="Black" GridLines="Vertical" AutoGenerateColumns="false">
            <AlternatingRowStyle BackColor="White" />
            <FooterStyle BackColor="#CCCC99" />
            <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
            <RowStyle BackColor="#F7F7DE" />
            <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
            <SortedAscendingCellStyle BackColor="#FBFBF2" />
            <SortedAscendingHeaderStyle BackColor="#848384" />
            <SortedDescendingCellStyle BackColor="#EAEAD3" />
            <SortedDescendingHeaderStyle BackColor="#575357" />
            <Columns>
                <asp:BoundField DataField="id_seguimiento" HeaderText="Seguimiento"/>
                <asp:BoundField DataField="marca" HeaderText="Org. Ventas"/>
                <asp:BoundField DataField="codigo_repuesto" HeaderText="Repuesto"/>
                <asp:BoundField DataField="estado" HeaderText="Estado"/>
                <asp:BoundField DataField="fecha_movimiento" HeaderText="Fecha Modfic."/>
                <asp:BoundField DataField="fecha_eta" HeaderText="Fecha Estimada"/>
                <asp:BoundField DataField="observacion" HeaderText="Observación"/>
            </Columns>
        </asp:GridView>
    </div>
    </form>
</body>
</html>
