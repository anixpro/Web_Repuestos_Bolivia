<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="CrearPedido.aspx.cs" Inherits="Vistas_crearPedido" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <asp:Panel ID="PanelPedidoCreado" runat="server" Visible="false">
        <div class="titulo">Resumen del pedido</div>
        <!-- Acá se despliegan los mensajes de error -->
        <table class="resumenPedido">
            <tr>
                <td><b>N° pedido:</b></td>
                <td class="style1"><asp:Label ID="lblNumPedido" runat="server" Text=""></asp:Label></td>
            </tr>
            <tr>
            <td><b>Concesionario:</b></td>
            <td> <asp:Label ID="lblConcesionario" runat="server" Text=""></asp:Label> </td>
            </tr>
            <tr>
            <td><b>Sucursal:</b></td>
            <td> <asp:Label ID="lblSucursal" runat="server" Text=""></asp:Label> </td>
            </tr>
            <tr>
            <td><b>Total del Pedido:</b></td>
            <td> <asp:Label ID="lblTotalPedido" runat="server" Text=""></asp:Label> </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="PanelResumen" runat="server">
    <center>
        <b>Lista de solicitudes CON STOCK</b>
    </center>
    <asp:GridView  ID="GridViewResumen" runat="server" AutoGenerateColumns="False" 
            ShowFooter="True" BackColor="White" GridLines="Vertical" Font-Size="12px"
                 Width="100%" CellPadding="3" BorderColor="#999999">
        <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
            <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
            <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" Font-Names="Tahoma"
                Font-Size="XX-Small" />
            <AlternatingRowStyle BackColor="Gainsboro" />
            <Columns>
                <asp:BoundField DataField="marca" FooterStyle-CssClass="invi" ItemStyle-CssClass="invi" HeaderStyle-CssClass="invi" ItemStyle-HorizontalAlign="Center" HeaderText="Marca" ReadOnly="True">
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="codigo" ItemStyle-HorizontalAlign="Center" HeaderText="Código" ReadOnly="True">
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="descripcion" ItemStyle-HorizontalAlign="Center" HeaderText="Descripción" ReadOnly="True">
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                </asp:BoundField>
                <asp:TemplateField HeaderText="Cantidad" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:TextBox MaxLength="4" Enabled="false" ID="txtCantidad" runat="server" Text='<%# Eval("cantidad") %>' Width="40px" AutoPostBack="False"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="valor" DataFormatString="{0:c}" ItemStyle-HorizontalAlign="Center" HeaderText="Valor" ReadOnly="True">
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                </asp:BoundField>
                <asp:TemplateField HeaderText="Total" FooterStyle-Font-Bold="True" >
                    <ItemTemplate >
                        <asp:Label ID="total" runat="server" Text="$"></asp:Label> <%# GetUnitPrice(int.Parse(Eval("total").ToString())).ToString("N0")%>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:Label ID="total2" runat="server" Text="$"></asp:Label> <%# GetTotal().ToString("N0") %>
                    </FooterTemplate>
                </asp:TemplateField>                  
            </Columns>
        </asp:GridView>
        
        <table style="width:100%;border-style: groove; border-width: thin;margin-left:auto;margin-right:auto">
            <tr>
                <td align="center"><b>Lista de solicitudes y descartes de repuestos SIN STOCK</b></td>
            </tr>
            <tr>
                <td>
                     <asp:GridView  ID="GridViewReserva" runat="server" AutoGenerateColumns="False" 
                        ShowFooter="True" BackColor="White" GridLines="Vertical" Font-Size="11px"
                        Width="100%" CellPadding="3" BorderColor="#999999" Caption="Repuestos enviados a RESERVA"
                        Visible="false" EmptyDataText="No se enviaron repuestos a reserva">
                        <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                        <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                        <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                        <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" Font-Names="Tahoma"
                            Font-Size="XX-Small" />
                        <AlternatingRowStyle BackColor="Gainsboro" />
                        <Columns>
                            <asp:BoundField DataField="num_backOrder" ItemStyle-HorizontalAlign="Center" HeaderText="N° reserva" ReadOnly="True">
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="codigo_rep" ItemStyle-HorizontalAlign="Center" HeaderText="Código" ReadOnly="True">
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="cantidad" ItemStyle-HorizontalAlign="Center" HeaderText="Cantidad" ReadOnly="True">
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="detalle_rep" ItemStyle-HorizontalAlign="Center" HeaderText="Detalle" ReadOnly="True">
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="fecha_creacion" ItemStyle-HorizontalAlign="Center" HeaderText="Fecha" ReadOnly="True">
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </asp:BoundField>                            
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:GridView ID="GridViewVfc" runat="server" AutoGenerateColumns="False" 
                        ShowFooter="True" BackColor="White" GridLines="Vertical" Font-Size="11px"
                        Width="100%" CellPadding="3" BorderColor="#999999" Caption="Repuestos enviados a DESARME (VFC)"
                        Visible="false" EmptyDataText="No se enviaron repuestos a VFC">
                        <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                        <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                        <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                        <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" Font-Names="Tahoma"
                            Font-Size="XX-Small" />
                        <AlternatingRowStyle BackColor="Gainsboro" />
                        <Columns>
                            <asp:BoundField DataField="num_VFC" ItemStyle-HorizontalAlign="Center" HeaderText="N° Vfc" ReadOnly="True">
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="codigo_rep" ItemStyle-HorizontalAlign="Center" HeaderText="Código" ReadOnly="True">
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="cantidad" ItemStyle-HorizontalAlign="Center" HeaderText="Cantidad" ReadOnly="True">
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="detalle_rep" ItemStyle-HorizontalAlign="Center" HeaderText="Detalle" ReadOnly="True">
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="fecha_creacion" ItemStyle-HorizontalAlign="Center" HeaderText="Fecha" ReadOnly="True">
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </asp:BoundField>                            
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:GridView  ID="GridViewDescartado" runat="server" AutoGenerateColumns="False" 
                        ShowFooter="True" BackColor="White" GridLines="Vertical" Font-Size="11px"
                        Width="100%" CellPadding="3" BorderColor="#999999" Caption="Repuestos DESCARTADOS"
                        Visible="false" EmptyDataText="No se descartaron repuestos sin stock">
                        <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                        <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                        <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                        <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" Font-Names="Tahoma"
                            Font-Size="XX-Small" />
                        <AlternatingRowStyle BackColor="Gainsboro" />
                        <Columns>
                            <asp:BoundField DataField="idDescartado" ItemStyle-HorizontalAlign="Center" HeaderText="N° Descartado" ReadOnly="True">
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="codigo_rep" ItemStyle-HorizontalAlign="Center" HeaderText="Código" ReadOnly="True">
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="cantidad" ItemStyle-HorizontalAlign="Center" HeaderText="Cantidad" ReadOnly="True">
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="detalle_rep" ItemStyle-HorizontalAlign="Center" HeaderText="Detalle" ReadOnly="True">
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="fechaDescarte" ItemStyle-HorizontalAlign="Center" HeaderText="Fecha" ReadOnly="True">
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </asp:BoundField>                            
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
        <center>
            <asp:ImageButton ID="btnVolver" runat="server" ImageUrl="~/img/volver.png" PostBackUrl="~/Vistas/buscarRepto2.aspx" />
        </center>
    </asp:Panel>
</asp:Content>

