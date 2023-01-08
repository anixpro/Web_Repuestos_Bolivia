<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="ReptosSinStock.aspx.cs" Inherits="Vistas_ReptosSinStock" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <asp:GridView  ID="GridViewNoStock" runat="server" AutoGenerateColumns="False" 
        ShowFooter="True" BackColor="White" GridLines="Vertical" Font-Size="12px"
        Width="100%" CellPadding="3" BorderColor="#999999" Caption="Repuestos con stock insuficiente" Visible="false">
        <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
        <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
        <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
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
            <asp:BoundField DataField="cantidad" ItemStyle-HorizontalAlign="Center" HeaderText="Cantidad" ReadOnly="True">
                <ItemStyle HorizontalAlign="Center"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField DataField="precioL" DataFormatString="{0:c}" ItemStyle-HorizontalAlign="Center" HeaderText="Precio Lista" ReadOnly="True">
                <ItemStyle HorizontalAlign="Center"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField DataField="precioC" DataFormatString="{0:c}" ItemStyle-HorizontalAlign="Center" HeaderText="Precio Concesionario" ReadOnly="True">
                <ItemStyle HorizontalAlign="Center"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField DataField="vfc" ItemStyle-HorizontalAlign="Center" HeaderText="vfc" ReadOnly="True">
                <ItemStyle HorizontalAlign="Center"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField DataField="reserva" ItemStyle-HorizontalAlign="Center" HeaderText="reserva" ReadOnly="True">
                <ItemStyle HorizontalAlign="Center"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField DataField="descarte" ItemStyle-HorizontalAlign="Center" HeaderText="descarte" ReadOnly="True">
                <ItemStyle HorizontalAlign="Center"></ItemStyle>
            </asp:BoundField>
        </Columns>
    </asp:GridView>
    <table width="780px" style="border-style: groove; border-width: thin">
        <tr>
            <td colspan="3" align="center">RESUMEN</td>
        </tr>
        <tr>
            <td>
                <asp:GridView  ID="GridViewReserva" runat="server" AutoGenerateColumns="False" 
                    ShowFooter="True" BackColor="White" GridLines="Vertical" Font-Size="11px"
                    Width="260px" CellPadding="3" BorderColor="#999999" Caption="Repuestos a reserva" Visible="false">
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
                        <asp:BoundField DataField="detalle_rep" ItemStyle-HorizontalAlign="Center" HeaderText="Detalle" ReadOnly="True">
                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        </asp:BoundField>
                        <asp:BoundField DataField="fecha_creacion" ItemStyle-HorizontalAlign="Center" HeaderText="Fecha" ReadOnly="True">
                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        </asp:BoundField>                            
                    </Columns>
                </asp:GridView>
            </td>
            <td>
            <asp:GridView  ID="GridViewVfc" runat="server" AutoGenerateColumns="False" 
                    ShowFooter="True" BackColor="White" GridLines="Vertical" Font-Size="11px"
                    Width="260px" CellPadding="3" BorderColor="#999999" Caption="Repuestos a VFC" Visible="false">
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
                        <asp:BoundField DataField="detalle_rep" ItemStyle-HorizontalAlign="Center" HeaderText="Detalle" ReadOnly="True">
                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        </asp:BoundField>
                        <asp:BoundField DataField="fecha_creacion" ItemStyle-HorizontalAlign="Center" HeaderText="Fecha" ReadOnly="True">
                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        </asp:BoundField>                            
                    </Columns>
                </asp:GridView>
            </td>
            <td>
            <asp:GridView  ID="GridViewDescartado" runat="server" AutoGenerateColumns="False" 
                    ShowFooter="True" BackColor="White" GridLines="Vertical" Font-Size="11px"
                    Width="260px" CellPadding="3" BorderColor="#999999" Caption="Repuestos descartados" Visible="false">
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
        <tr>
            <td colspan="3" align="center">
                <asp:ImageButton ID="btnVolver" runat="server" ImageUrl="~/img/volver.png" PostBackUrl="~/Vistas/buscarRepto.aspx" /></td>
        </tr>
    </table>
</asp:Content>

