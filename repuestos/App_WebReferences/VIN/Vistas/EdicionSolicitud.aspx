<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="EdicionSolicitud.aspx.cs" Inherits="Vistas_EdicionSolicitud" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        //        function grabarPrecio() {
        //            window.open('precio.aspx', 'precio', 'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=yes, resizable=no, width=500, height=200, top=200, left=190')
        //            document.forms.action = 'precio.aspx';
        //            document.forms.target = 'Ingrese Precio'
        //        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="updatePanel1" runat="server">
        <ContentTemplate>
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%ConnectionStrings:skbergeConnectionString%>">
            </asp:SqlDataSource>
            <div class="Titulo" id="titEdiSoli">
                Listado de Solicitudes de Cotizacion
            </div>
            <div id="mjsError" runat="server" clientidmode="Static">
            </div>
            <div>
                <asp:Panel ID="panelBuscador" runat="server">
                    <table>
                        <tr>
                            <td class="tdBuscaRepuesto" runat="server" clientidmode="Static" id="tdbuscasoli">
                                <table>
                                    <tr>
                                        <td>
                                            Seleccione Marca
                                        </td>
                                        <td style="font-family: Arial, Helvetica, Sans-Serif" class="style1">
                                            <asp:DropDownList Width="150px" ID="combomarcas" runat="server" ClientIDMode="Static"
                                                AutoPostBack="false" TabIndex="1">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Numero de Solicitud
                                        </td>
                                        <td style="font-family: Arial, Helvetica, Sans-Serif" class="style1">
                                            <asp:TextBox ID="txtnumsoli" Width="150px" runat="server" MaxLength="25" ClientIDMode="Static"
                                                AutoCompleteType="Disabled" TabIndex="2"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Fecha Desde
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtdesde" runat="server" Width="80px" Style="text-transform: uppercase"
                                                AutoPostBack="true"></asp:TextBox>&nbsp;&nbsp;&nbsp; Fecha Hasta
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtHasta" runat="server" Width="80px" Style="text-transform: uppercase"
                                                AutoPostBack="true"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Codigo Repuesto
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtCodRep" runat="server" MaxLength="25" ClientIDMode="Static" Width="150px"
                                                AutoCompleteType="Disabled"></asp:TextBox>
                                        </td>
                                        <td align="center">
                                            VIN
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtVin" runat="server" MaxLength="17" ClientIDMode="Static" Width="150px"
                                                AutoCompleteType="Disabled"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Usuario Creador
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtusuario" runat="server" MaxLength="15" Width="150px" ClientIDMode="Static"
                                                AutoCompleteType="Disabled" Enabled="false"></asp:TextBox>
                                        </td>
                                        <td colspan="2" align="center">
                                            <asp:Button ID="btnBuscarSoli" runat="server" Text="Buscar" class="button" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:Panel ID="PanelListado" Visible="true" runat="server">
                    <p style="text-align: center; font-size: large; font-weight: bold">
                        Listado de Solicitudes de Cotizacion
                    </p>
                    <hr />
                    <div id="divGrd" style="overflow: auto; width: 770px; height: 300px">
                        <asp:GridView ID="gvListSolicitud" runat="server" AutoGenerateColumns="false" BackColor="White"
                            GridLines="Vertical" Font-Size="12px" widht="100%" CellPadding="3" BorderColor="#999999"
                            Caption="" Width="100%">
                            <FooterStyle BackColor="SkyBlue" ForeColor="Black" />
                            <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                            <HeaderStyle BackColor="#000084" Font-Bold="true" ForeColor="White" Font-Names="Tahoma"
                                Font-Size="X-Small" />
                            <AlternatingRowStyle BackColor="Gainsboro" />
                            <Columns>
                                <asp:BoundField DataField="numeroSolicitud" ItemStyle-HorizontalAlign="Center" HeaderText="Numero Solicitud"
                                    ReadOnly="true">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="numeroSolicitud" FooterStyle-CssClass="invi" ItemStyle-CssClass="invi"
                                    HeaderStyle-CssClass="invi" ItemStyle-HorizontalAlign="Center" HeaderText="Numero Solicitud"
                                    ReadOnly="true">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="fecha" ItemStyle-HorizontalAlign="Center" HeaderText="Fecha Solicitud"
                                    ReadOnly="true">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="fecha" FooterStyle-CssClass="invi" ItemStyle-CssClass="invi"
                                    HeaderStyle-CssClass="invi" ItemStyle-HorizontalAlign="Center" HeaderText="Fecha Solicitud"
                                    ReadOnly="true">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="marca" ItemStyle-HorizontalAlign="Center" HeaderText="Marca"
                                    ReadOnly="true">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="marca" FooterStyle-CssClass="invi" ItemStyle-CssClass="invi"
                                    HeaderStyle-CssClass="invi" ItemStyle-HorizontalAlign="Center" HeaderText="Marca"
                                    ReadOnly="true">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="codRepto" ItemStyle-HorizontalAlign="Center" HeaderText="Repuesto"
                                    ReadOnly="true">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="codRepto" FooterStyle-CssClass="invi" ItemStyle-CssClass="invi"
                                    HeaderStyle-CssClass="invi" ItemStyle-HorizontalAlign="Center" HeaderText="Repuesto"
                                    ReadOnly="true">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="cantidad" ItemStyle-HorizontalAlign="Center" HeaderText="Cantidad"
                                    ReadOnly="true">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="cantidad" FooterStyle-CssClass="invi" ItemStyle-CssClass="invi"
                                    HeaderStyle-CssClass="invi" ItemStyle-HorizontalAlign="Center" HeaderText="Cantidad"
                                    ReadOnly="true">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="usuario" ItemStyle-HorizontalAlign="Center" HeaderText="Usuario"
                                    ReadOnly="true">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="usuario" FooterStyle-CssClass="invi" ItemStyle-CssClass="invi"
                                    HeaderStyle-CssClass="invi" ItemStyle-HorizontalAlign="Center" HeaderText="Usuario"
                                    ReadOnly="true">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="concesionario" ItemStyle-HorizontalAlign="Center" HeaderText="Concesionario"
                                    ReadOnly="true">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="concesionario" FooterStyle-CssClass="invi" ItemStyle-CssClass="invi"
                                    HeaderStyle-CssClass="invi" ItemStyle-HorizontalAlign="Center" HeaderText="Concesionario"
                                    ReadOnly="true">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="tipo" ItemStyle-HorizontalAlign="Center" HeaderText="Tipo Solicitud"
                                    ReadOnly="true">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="tipo" FooterStyle-CssClass="invi" ItemStyle-CssClass="invi"
                                    HeaderStyle-CssClass="invi" ItemStyle-HorizontalAlign="Center" HeaderText="Tipo Solicitud"
                                    ReadOnly="true">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="vin" ItemStyle-HorizontalAlign="Center" HeaderText="VIN"
                                    ReadOnly="true">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="vin" FooterStyle-CssClass="invi" ItemStyle-CssClass="invi"
                                    HeaderStyle-CssClass="invi" ItemStyle-HorizontalAlign="Center" HeaderText="VIN"
                                    ReadOnly="true">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="envio" ItemStyle-HorizontalAlign="Center" HeaderText="Transporte"
                                    ReadOnly="true">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="envio" FooterStyle-CssClass="invi" ItemStyle-CssClass="invi"
                                    HeaderStyle-CssClass="invi" ItemStyle-HorizontalAlign="Center" HeaderText="Transporte"
                                    ReadOnly="true">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="precio_Solicitud" ItemStyle-HorizontalAlign="Center" HeaderText="Precio"
                                    ReadOnly="true">
                                    <ItemStyle HorizontalAlign="Right" />
                                </asp:BoundField>
                                <asp:BoundField DataField="precio_Solicitud" FooterStyle-CssClass="invi" ItemStyle-CssClass="invi"
                                    HeaderStyle-CssClass="invi" ItemStyle-HorizontalAlign="Center" HeaderText="Precio"
                                    ReadOnly="true">
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <%--                                <asp:TemplateField HeaderText="Precio">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="btnPrecio" runat="server" ImageUrl="~/img/peso.png" ClientIDMode="Static"
                                            OnClick="IngresaPrecio" />
                                    </ItemTemplate>
                                </asp:TemplateField>--%>
                                <asp:CommandField SelectText="Precio" ShowSelectButton="true" FooterStyle-HorizontalAlign="Center"
                                    ButtonType="Image" SelectImageUrl="~/img/peso.png" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </asp:Panel>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
