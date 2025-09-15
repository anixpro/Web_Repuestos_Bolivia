<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="ConsultarPedido.aspx.cs" Inherits="Vistas_ConsultarPedido" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style type="text/css">
        .style1
        {
            width: 151px;
        }
        .style2
        {
            height: 21px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="titulo">
        Consultar estado de pedido
    </div>
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="false">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <!-- Acá se despliegan los mensajes de error -->
            <div id="msjesError" runat="server" clientidmode="Static"></div>
            <div style="background-color:#CEE3F6;">
            <center>
            <table>
                <tr>
                    <td><b>Buscar: Pedido</b></td>
                </tr>
            </table>
            <asp:Panel ID="PanelBusqueda" runat="server" Visible="false">
                <table>
                    <tr>
                        <td colspan="2" align="center" 
                            style="border-color: #333333; font-family: Arial, Helvetica, sans-serif; font-size: 13px; font-weight: bold; color: #0F0F0F; border-bottom-style: groove; border-bottom-width: thin;">Consultar por número de pedido
                        </td>
                    </tr>
                    <tr>
                        <td style="font-family: Arial, Helvetica, sans-serif; font-size: 12px; color: #1E1E1E"  >
                            <asp:Label ID="lblNumBuscar" runat="server" Text=""></asp:Label>
                        </td>
                        <td><asp:TextBox MaxLength="10" ID="txtNumPedido" runat="server" 
                            ClientIDMode="Static"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <asp:Button ID="btnConsPed" runat="server" Text="Consultar Pedido" 
                                 onclick="btnConsPed_Click" Visible="true"/>
                            <asp:Button ID="btnConsCoti" runat="server" Text="Consultar Cotización" 
                                 onclick="btnConsCoti_Click" Visible="false" />
                        </td>
                        <td>
                            <asp:Button ID="btnSalir" runat="server" Text="Salir" Width="100px" 
                                onclick="btnSalir_Click" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>           
            <asp:Panel ID="PanelInfoPedido" runat="server" Visible="false">
                <table>
                    <tr>
                        <td style="vertical-align: top; width: 547px;">
                            <strong>
                                <span style="font-size: 9pt; font-family: Tahoma">
                                    <asp:Label ID="Label7" runat="server" Text="Informacion del Pedido:"></asp:Label>
                                </span>
                            </strong>
                        </td>
                    </tr>
                    <tr>
                        <td style="vertical-align: top; width: 547px;">
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label4" runat="server" Text="Dealer:" Font-Names="Tahoma" Font-Size="8pt"></asp:Label>
                                    </td>
                                    <td>
                                      <%--  <asp:TextBox ID="cmpy_code" runat="server" Enabled="False" CssClass="InputTextBox"
                                            Width="50px"></asp:TextBox>--%>
                                        <asp:TextBox ID="name_text" runat="server" CssClass="InputTextBox" Enabled="False"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr runat="server" id="trrHideMe">
                                    <td>
                                        <asp:Label ID="Label9" runat="server" Font-Names="Tahoma" Font-Size="8pt" Text="Nº Cotización:"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="numCotizacion" runat="server" Enabled="False" CssClass="InputTextBox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label5" runat="server" Font-Names="Tahoma" Font-Size="8pt" Text="Nº Pedido:"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="porder_num" runat="server" Enabled="False" CssClass="InputTextBox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label6" runat="server" Font-Names="Tahoma" Font-Size="8pt" Text="Fecha Orden:"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="order_date" runat="server" Enabled="False" CssClass="InputTextBox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="label1" runat="server" Font-Names="Tahoma" Font-Size="8pt" Text="Neto:"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="goods_amt" runat="server" Enabled="False" CssClass="InputTextBox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label2" runat="server" Font-Names="Tahoma" Font-Size="8pt" Text="IGV:"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="tax_amt" runat="server" Enabled="False" CssClass="InputTextBox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label3" runat="server" Font-Names="Tahoma" Font-Size="8pt" Text="Total:"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="total_amt" runat="server" Enabled="False" CssClass="InputTextBox"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        <asp:Button ID="closebtn" runat="server" Text="Volver" CssClass="InputButton" OnClick="closebtn_Click" />
                        <br />
                        <asp:Label ID="errOrder" runat="server" Font-Bold="True" Font-Size="XX-Small" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                </table>
                <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" BackColor="White"
                    BorderColor="#999999" BorderStyle="None" BorderWidth="1px" CellPadding="3"
                    Font-Names="Tahoma" Font-Size="XX-Small" GridLines="Vertical" Width="100%">
                    <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                    <Columns>
                        <asp:BoundField DataField="MATNR" HeaderText="Codigo" ReadOnly="True" />
                        <asp:BoundField DataField="ARKTX" HeaderText="Descripcion" ReadOnly="True" />
                        <asp:TemplateField HeaderText="Precio">
                            <EditItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("NETPR") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                     <asp:Label ID="Label10" runat="server" Text="$"></asp:Label>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("NETPR") %>'></asp:Label>
                       
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="KWMENG" HeaderText="Cantidad" ReadOnly="True" />
                        <asp:BoundField DataField="BMENG" HeaderText="Reservado" />
                        <asp:BoundField DataField="BMENG2" HeaderText="Pendiente" />
                        <asp:BoundField DataField="RFMNG" HeaderText="Empacado" />
                        <asp:BoundField DataField="RFMNG2" HeaderText="Despachado" />
                    </Columns>
                    <RowStyle BackColor="#EEEEEE" ForeColor="Black" Font-Names="Tahoma" Font-Size="8pt" />
                    <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                    <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                    <HeaderStyle BackColor="#000084" Font-Bold="True" Font-Names="Tahoma" Font-Size="XX-Small"
                        ForeColor="White" />
                    <AlternatingRowStyle BackColor="Gainsboro" />
                </asp:GridView>
            </asp:Panel>
            <asp:Panel ID="PanelInfoCotiza" runat="server" Visible="false">
                <table>
                    <tr>
                        <td colspan="2" align="center" class="style2">Detalle Cotización</td>
                    </tr>
                    <tr>
                        <td class="style1">Dealer: </td>
                        <td>
                            <asp:TextBox ID="txtDealer" runat="server" CssClass="InputTextBox" Enabled="False"
                                Width="300px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="style1">N° Cotización: </td>
                        <td>
                            <asp:TextBox ID="txtNumCotizacion" runat="server" Enabled="False" CssClass="InputTextBox"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                    <td class="style1">Fecha de creación: </td>
                    <td>
                        <asp:TextBox ID="txtCreacion" runat="server" Enabled="False" CssClass="InputTextBox"></asp:TextBox>
                    </td>
                    </tr>

                    <tr>
                    <td class="style1">Fecha de expiración: </td>
                    <td>
                        <asp:TextBox ID="txtExpira" runat="server" Enabled="False" CssClass="InputTextBox"></asp:TextBox>
                    </td>
                    </tr>
                    <tr>
                    <td class="style1">Total neto: </td>
                    <td>
                        <asp:TextBox ID="txtTotalNeto" runat="server" Enabled="False" CssClass="InputTextBox"></asp:TextBox>
                    </td>
                    </tr>
                    </table>
                <asp:GridView ID="GridViewMateriales" runat="server" AutoGenerateColumns="False" BackColor="White"
                        BorderColor="#999999" BorderStyle="None" BorderWidth="1px" CellPadding="3"
                        Font-Names="Tahoma" Font-Size="XX-Small" GridLines="Vertical" Width="100%">
                        <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                        <Columns>
                            <asp:BoundField DataField="codigo" HeaderText="Código" ReadOnly="True" />
                            <asp:BoundField DataField="marca" HeaderText="Marca" ReadOnly="True" />
                            <asp:BoundField DataField="descripcion" HeaderText="Descripción" ReadOnly="True" />
                            <asp:BoundField DataField="cantidad" HeaderText="Cantidad" ReadOnly="True" />
                            <asp:BoundField DataField="valor" HeaderText="Valor" />
                            <asp:BoundField DataField="total" HeaderText="Total" />
                        </Columns>
                        <RowStyle BackColor="#EEEEEE" ForeColor="Black" Font-Names="Tahoma" Font-Size="8pt" />
                        <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                        <HeaderStyle BackColor="#000084" Font-Bold="True" Font-Names="Tahoma" Font-Size="XX-Small"
                            ForeColor="White" />
                        <AlternatingRowStyle BackColor="Gainsboro" />
                    </asp:GridView>

                <asp:Panel ID="PanelConfirmaDescarta" runat="server">
                    <table>
                    <tr>
                    <td><asp:Button ID="btnDescartar" runat="server" Text="Descartar" 
                            onclick="btnDescartar_Click" /></td>
                    <td>
                        <!--Panel que permite confirmar un pedido a SAP-->
                            <asp:Panel ID="PanelConfirmaPedido" runat="server" Visible="true">
                               <table style="border-style: groove" class="tablaConfirmarPedido">
                                 <tr>
                                    <td colspan="3" align="center" style="font-family: Arial, Helvetica, sans-serif; font-size: 15px; font-weight: bold; color: #353535" >
                                        Confirmar Pedido
                                    </td>
                                  </tr>
                                  <tr>
                                        <td style="font-family: Arial, Helvetica, sans-serif; font-size: 12px; color: #333333">
                                            <asp:Label ID="lblTipoPed" runat="server" Text="Tipo de Pedido"></asp:Label>
                                        </td>
                                        <td>            
                                        <asp:DropDownList ID="comboTipoPed" runat="server" Enabled="False">
                                            <asp:ListItem>Normal</asp:ListItem>
                                            <asp:ListItem>Garantía</asp:ListItem>
                                        </asp:DropDownList>
                                        </td>
                                  </tr>
                                  <tr>
                                    <td colspan="3">
                                       <asp:Label ID="lblNumCoti" runat="server" Text="N° cotización:" Font-Size="10px"></asp:Label>
                                       <asp:TextBox ID="txtNumCoti" runat="server" Enabled="False"></asp:TextBox>
                                    </td>
                                  </tr>
                                  <tr>
                                        <td style="font-family: Arial, Helvetica, sans-serif; font-size: 12px; font-weight: bold; color: #333333">
                                                <asp:Label ID="lblPrio" runat="server" Visible="true"
                                                    Text="Indicar Prioridad: "></asp:Label>
                                        </td>
                                        <td>
                                             <asp:RadioButtonList ID="RadioButtonListPrio" Visible="true" runat="server" Font-Names="tahoma" Font-Size="X-Small" RepeatDirection="Horizontal">
                                                <asp:ListItem Value="2" Selected="True">Normal</asp:ListItem>                                                                  
                                                <asp:ListItem Value="1">Aéreo</asp:ListItem>
                                             </asp:RadioButtonList>
                                        </td>
                                        <td>
                                            <img alt="Advertencia" title="Si escoge esta opción, el concesionario se hará cargo del pago del transporte." src="../img/help.png" />
                                            <asp:Button ID="btnGenerarPedido" runat="server" Text="Confirmar Pedido" 
                                                onclick="btnGenerarPedido_Click" /> 
                                        </td>
                                  </tr>
                                  <tr>
                                    <td colspan="3"> 
                                        <asp:Label ID="LblMensajeCotiz" runat="server" Text=""></asp:Label> </td>
                                    
                                 </tr>
                               </table>
                            </asp:Panel>
                        <!--Fin panel Pedido-->            
                    </td>
                    <td> 
                        <asp:Button ID="btnResgueCart" runat="server" Text="Nuevo carro" 
                            onclick="btnResgueCart_Click" /> </td>
                    </tr>
                    </table>           
                </asp:Panel>
                <!--Fin panel descartar pedir--> 
                </asp:Panel>
                <asp:Panel ID="PanelTodasCotizaciones" Visible="false" runat="server">
                    <asp:GridView ID="GridViewCotizaciones" runat="server" AutoGenerateColumns="False" BackColor="White"
                        BorderColor="#999999" BorderStyle="None" BorderWidth="1px" CellPadding="3"
                        Font-Names="Tahoma" Font-Size="XX-Small" GridLines="Vertical" Width="100%">
                        <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                        <RowStyle BackColor="#EEEEEE" ForeColor="Black" Font-Names="Tahoma" Font-Size="8pt" />
                                   
                        <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                        <HeaderStyle BackColor="#000084" Font-Bold="True" Font-Names="Tahoma" Font-Size="XX-Small"
                            ForeColor="White" />
                        <AlternatingRowStyle BackColor="Gainsboro" />
                        <Columns>
                            <asp:BoundField DataField="E_VBELN" HeaderText="N° cotización" ReadOnly="True" />
                            <asp:BoundField DataField="FECHA_SOLICITUD" HeaderText="Fecha Solicitud" ReadOnly="True" DataFormatString="{0:M-dd-yyyy}"/>
                            <asp:BoundField DataField="FECHA_EXPIRACION" HeaderText="Fecha termino" ReadOnly="True" DataFormatString="{0:M-dd-yyyy}" />
                            <asp:BoundField DataField="ESTADO" HeaderText="Estado" ReadOnly="True" />
                            <asp:CommandField SelectText="" ShowSelectButton="True" FooterStyle-HorizontalAlign="Center" ButtonType="Image" SelectImageUrl="~/img/ver.gif" />
                        </Columns>
                      </asp:GridView>
                </asp:Panel>

                <asp:Panel ID="PanelTodosPedidos" Visible="false" runat="server">
                        <asp:GridView ID="GridViewPedidos" runat="server" AutoGenerateColumns="False" BackColor="White"
                            BorderColor="#999999" BorderStyle="None" BorderWidth="1px" CellPadding="3"
                            Font-Names="Tahoma" Font-Size="XX-Small" GridLines="Vertical" Width="100%">
                            <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                            <RowStyle BackColor="#EEEEEE" ForeColor="Black" Font-Names="Tahoma" Font-Size="8pt" />
                                   
                            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                            <HeaderStyle BackColor="#000084" Font-Bold="True" Font-Names="Tahoma" Font-Size="XX-Small"
                                ForeColor="White" />
                            <AlternatingRowStyle BackColor="Gainsboro" />
                            <Columns>
                                <asp:BoundField DataField="E_VBELN_PEDIDO" HeaderText="N° pedido" ReadOnly="True" />
                                <asp:BoundField DataField="FECHA_SOLICITUD" HeaderText="Fecha pedido" ReadOnly="True" DataFormatString="{0:M-dd-yyyy}" />
                                <asp:CommandField SelectText="" ShowSelectButton="True" FooterStyle-HorizontalAlign="Center" ButtonType="Image" SelectImageUrl="~/img/ver.gif" />
                            </Columns>
                         </asp:GridView>
                </asp:Panel>
                </center>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>



