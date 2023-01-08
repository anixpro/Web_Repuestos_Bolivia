<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Cotizar.aspx.cs" Inherits="Vistas_cotizar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script type="text/javascript">
        function confirma() {
            $("#btnGenerarPedido").attr("disabled", true);

        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <asp:Panel ID="PanelCotizacion" runat="server">
        <!--GridView con el resumen de una generación de cotización-->
       <asp:GridView  ID="GridViewResumen" runat="server" AutoGenerateColumns="False" 
            ShowFooter="True" BackColor="White" GridLines="Vertical" Font-Size="12px"
                 Width="100%" CellPadding="3" BorderColor="#999999" Caption="Resumen del Pedido">
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
                <asp:BoundField DataField="valor" ItemStyle-HorizontalAlign="Center" HeaderText="Valor" ReadOnly="True">
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                </asp:BoundField>
                <asp:TemplateField HeaderText="Total" FooterStyle-Font-Bold="True" >
                    <ItemTemplate >
                        <asp:Label ID="total" runat="server" Text="$"></asp:Label> <%# GetUnitPrice(double.Parse(Eval("total").ToString())).ToString("N")%>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:Label ID="total2" runat="server" Text="$"></asp:Label> <%# GetTotal().ToString("N") %>
                    </FooterTemplate>
                </asp:TemplateField>                  
            </Columns>
        </asp:GridView>
        <asp:Button ID="btnGenerarPDF" runat="server" Text="Generar PDF" 
            onclick="btnGenerarPDF_Click" />
    </asp:Panel><br />
    <!--Panel que permite confirmar un pedido a SAP-->
   
    <!--Fin resumen cotización-->

    <asp:Panel ID="PanelSinStock" runat="server">
        <asp:GridView  ID="GridViewNoStock" runat="server" AutoGenerateColumns="False" 
            ShowFooter="True" BackColor="White" GridLines="Vertical" Font-Size="12px"
            Width="100%" CellPadding="3" BorderColor="#999999" Caption="Repuestos con stock insuficiente"
            Visible="false">
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

       
    </asp:Panel>
    <center>
        <p>
            
            <i>El o los repuestos enviados a reserva se facturarán inmediatamente una vez arribado(s) a bodega</i>
        </p>
    </center>
</asp:Content>
