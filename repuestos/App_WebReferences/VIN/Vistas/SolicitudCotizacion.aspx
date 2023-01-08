<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="SolicitudCotizacion.aspx.cs" Inherits="Vistas_SolicitudCotizacion" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script language="javascript" type="text/javascript">
        function checkvin() {
            $("#infoVIN").html("<img src='../img/lightbox-ico-loading.gif'><b>Comprobando ...</b>");

            if ($("#txtVin").val().length < 6) {
                alert("Ingrese un código VIN más largo");
                $("#infoVIN").html("");
                return false;
            }

            var checkVinAjax = $.ajax({
                type: "GET",
                cache: false,
                url: "ValidaVin.aspx",
                data: { vin: $("#txtVin").val(), marca: $("#inMarca").val() },
                dataType: "html"
            });

            var bool = "";
            checkVinAjax.done(
                    function (data) {
                        if (data == 'false') {
                            alert("Ingrese un código VIN valido");
                            $("#infoVIN").html("");
                            return false;
                        } else {

                            $("#infoVIN").html("<marquee><b>Cargando datos de VFC</b></marquee>");
                            var solicitudAjax = $.ajax({
                                type: "GET",
                                cache: false,
                                url: "ConsultaVIN.aspx",
                                data: { vin: $("#txtVin").val() },
                                dataType: "html"
                            });

                            solicitudAjax.done(
								function (data) {
								    $("#infoVIN").html(data);
								    $("#btnAceptarVIN").show();
								}
							);

                            solicitudAjax.fail(function (jqXHR, textStatus) {
                                $("#infoVIN").html("Error! Favor contactar al administrador. Descripción del error del navegador: " + textStatus);
                            });

                        }
                    }
                );

        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="updatepanel1" runat="server">
        <ContentTemplate>
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%ConnectionStrings:skbergeConnectionString%>">
            </asp:SqlDataSource>
            <div class="titulo" id="titSolici">
                Cotizar Repuesto sin Stock
            </div>
            <div id="mjsError" runat="server" clientidmode="Static">
            </div>
            <asp:Panel ID="PanelSolicitud" runat="server">
                <table>
                    <tr>
                        <td class="tdIngrRepto" runat="server" clientidmode="static" id="tdIngrRepto">
                            <table>
                                <tr>
                                    <td>
                                        Marca:
                                    </td>
                                    <td style="font-family: Arial, Helvetica, Sans-Serif" class="style1">
                                        <asp:DropDownList Width="120px" ID="ComboMarcas" runat="server" ClientIDMode="Static"
                                            AutoPostBack="false">
                                        </asp:DropDownList>
                                        <br />
                                        <asp:LinkButton ID="btnNuevaSoli" Visible="false" runat="server" OnClientClick="javascript:return(confirm('Realizar una nueva Solicitud eliminará la Solicitud Actual. ¿Esta seguro?'))"
                                            OnClick="btnNuevaSoli_Click" TabIndex="1">Nueva Solicitud</asp:LinkButton>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Codigo Repuesto:
                                    </td>
                                    <td>
                                        <asp:TextBox MaxLength="25" ID="txtCodigo" runat="server" Width="120px" ClientIDMode="Static"
                                            AutoCompleteType="Disabled" TabIndex="2"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="descrepto" Text="Descripcion del Repuesto" runat="server"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox MAs="100" ID="txtdescripcion" runat="server" Width="120px" ClientIDMode="Static"
                                            AutoCompleteType="Disabled"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Cantidad:
                                    </td>
                                    <td>
                                        <asp:TextBox CssClass="txtCantidad" ID="txtCantidad" runat="server" MaxLength="3"
                                            Width="50px" ClientIDMode="Static" TabIndex="3"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Tipo de Solicitud
                                    </td>
                                    <td style="font-family: Arial, Helvetica, Sans-Serif" class="style1">
                                        <asp:DropDownList Width="120px" ID="CombotipoSolici" runat="server">
                                            <asp:ListItem Value="0">Seleccione</asp:ListItem>
                                            <asp:ListItem Value="normal">Normal</asp:ListItem>
                                            <asp:ListItem Value="garantia">Garantía</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Ingrese VIN:
                                    </td>
                                    <td>
                                        <asp:TextBox CssClass="txtVin" ID="txtVin" runat="server" MaxLength="17" Width="180px"
                                            ClientIDMode="Static" TabIndex="3">
                                        </asp:TextBox><input type="hidden" id="inMarca" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <input type="button" id="btnComprobarVIN" value="Comprobar VIN" class="button" onclick="checkvin();" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Tipo de Transporte
                                    </td>
                                    <td style="font-family: Arial, Helvatica, Sans-Serif" class="style1">
                                        <asp:DropDownList Width="120px" ID="Combotipotrans" runat="server">
                                            <asp:ListItem Value="0">seleccione</asp:ListItem>
                                            <asp:ListItem Value="Aereo Normal">Aereo Normal</asp:ListItem>
                                            <asp:ListItem Value="Maritimo">Maritimo</asp:ListItem>
                                            <asp:ListItem Value="Courier">Courier</asp:ListItem>
                                            <asp:ListItem Value="Aereo Urgente">Aereo Urgente</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <%--                                        <asp:LinkButton ID="btnNuevaSoli" Visible="false" runat="server" OnClientClick="javascript:return(confirm('Realizar una nueva Solicitud eliminará la Solicitud Actual. ¿Esta seguro?'))"
                                            OnClick="btnNuevaSoli_Click" TabIndex="1">Nueva Solicitud</asp:LinkButton>--%>
                                    </td>
                                    <td colspan="2" align="right">
                                        <asp:Button ID="btnAgregar" runat="server" ClientIDMode="Static" TabIndex="5" Height="30px"
                                            Text="Agregar" CssClass="button" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <%--                        <td class="tdBuscaRepuesto" id="tdResultadoBusqueda" runat="server">
                            <asp:GridView ID="gvSoliCoti" runat="server" AutoGenerateColumns="false" EmptyDataText="Debe Seleccionar marca, Ingrese codigo y cantidad"
                                ShowFooter="true" BackColor="White" GridLines="Vertical" Font-Size="12px" Width="100%"
                                CellPadding="3" BorderColor="#999999" DataSourceID="" Caption="Solicitud Cotizacion"
                                Style="margin-left: 0px" AllowPaging="true" AllowSorting="true">
                            </asp:GridView>
                        </td>--%>
                    </tr>
                </table>
            </asp:Panel>
            <asp:Panel ID="pnSoliCoti" runat="server" Visible="true">
                <p style="text-align: center; font-size: large; font-weight: bold">
                    Cotizar Repuesto sin Stock
                </p>
                <hr />
                <asp:GridView ID="gvSolicitud" runat="server" AutoGenerateColumns="false" BackColor="White"
                    GridLines="Vertical" Font-Size="12px" widht="100%" CellPadding="3" BorderColor="#999999"
                    Caption="Cotizar Repuesto sin Stock" Width="100%">
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
                    </Columns>
                </asp:GridView>
                <br />
                <asp:Button runat="server" ID="btnCreaSolicitud" ClientIDMode="Static" Text="Crear Solicitud"
                    class="button" />
                <br />
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
