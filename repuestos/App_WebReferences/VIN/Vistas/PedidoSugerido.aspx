<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PedidoSugerido.aspx.cs" Inherits="Vistas_PedidoSujerido" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body style="background-color:#CEE3F6;">
<!-- Acá se despliegan los mensajes de error -->
<div id="msjesError" runat="server" clientidmode="Static"></div>
<center>
    <form id="form1" runat="server">
    <div id="sugeridos" style="width:600px;" >
    <h1>Repuestos más comprados de los ultimos 30 días</h1>
        <asp:GridView ID="GridViewSugerido" runat="server" AutoGenerateColumns="False" EmptyDataText="Lista vacia..."  
                        ShowFooter="True" BackColor="White" GridLines="Vertical" Font-Size="12px"
                        Width="100%" CellPadding="3" BorderColor="#999999" >

                    <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                    <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                    <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                    <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                    <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" Font-Names="Tahoma"
                        Font-Size="XX-Small" />
                    <AlternatingRowStyle BackColor="Gainsboro" />

                    <Columns>
                        <asp:BoundField DataField="marca" ItemStyle-HorizontalAlign="Center" HeaderText="Marca" ReadOnly="True">
                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        </asp:BoundField>
                        <asp:BoundField DataField="codigo" ItemStyle-HorizontalAlign="Center" HeaderText="Codigo" ReadOnly="True">
                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        </asp:BoundField>
                        <asp:BoundField DataField="descripcion" ItemStyle-HorizontalAlign="Center" HeaderText="Descripción" ReadOnly="True">
                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        </asp:BoundField> 
                        <asp:BoundField DataField="cantidadSum" ItemStyle-HorizontalAlign="Center" HeaderText="Cantidad" ReadOnly="True">
                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        </asp:BoundField>
                     </Columns>
                    </asp:GridView>

    

    </div>
    </form>
    </center>
</body>
</html>
