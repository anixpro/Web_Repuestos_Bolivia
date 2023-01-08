<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="ListarSolicitudes.aspx.cs" Inherits="Vistas_ListarSolicitudes" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit.HTMLEditor"
    TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script type="text/javascript">
        $(document).ready(function () {
            if ($("#hiddenType").val() == 0) {
                $("#divBusquedaFiltros").hide();
            } else {
                $("#divBusquedaExacta").hide();
            }

            $("#lnkTipoBusqueda").click(function () {
                $("#divBusquedaExacta").hide(500);
                $("#divBusquedaFiltros").show(500);
            });

            $("#btnCancelarBusquedaFiltros").click(function () {
                $("#divBusquedaFiltros").hide(500);
                $("#divBusquedaExacta").show(500);
            });

            $("#divDetalleSolicitud").dialog({
                autoOpen: false,
                modal: true,
                resizable: false,
                width: 760,
                height: 200,
                title: 'Detalle de Solicitud',
                position: 'top'
            });

            $(".lnkIdSolicitud").click(function (e) {
                e.preventDefault();
                $("#divDetalleSolicitud").html("<img src='../img/lightbox-ico-loading.gif'/>");
                $("#divDetalleSolicitud").dialog("open");
                $.ajax({
                    type: "GET",
                    cache: false,
                    url: "ajax/detalleSolicitud.aspx",
                    data: { id: $(this).html() },
                    dataType: "html",
                    success: function (data) {
                        $("#divDetalleSolicitud").html(data);
                    }
                });
            });

        });

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:skbergeConnectionString %>"
        SelectCommand="select persona.rut,persona.nombre from persona
	inner join personaPermisos on personaPermisos.rut = persona.rut
		inner join sucursal on sucursal.shipCode = persona.shipCode
			where sucursal.numeroFactura = (select top 1 sucursal.numeroFactura from sucursal
			inner join persona on sucursal.shipCode = persona.shipCode
				where persona.rut=@rut)
			and personaPermisos.cargo = 3">
        <SelectParameters>
            <asp:SessionParameter Name="rut" SessionField="rut" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:HiddenField ID="hiddenType" runat="server" ClientIDMode="Static" Value="0" />
    <div class="titulo">
        Estado de solicitudes de Reserva y VFC
    </div>
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static">
    </div>
    <div id="divBusquedaExacta">
        <table>
            <thead>
                <tr>
                    <th colspan="2">
                        Busqueda por identificador de solicitud
                    </th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td>
                        Identificador:
                    </td>
                    <td>
                        <asp:TextBox ID="txtIdSolicitud" runat="server" MaxLength="20"></asp:TextBox>
                    </td>
                    <td colspan="2">
                        <asp:Button runat="server" ID="btnBuscarExacto" Text="Buscar" OnClick="btnBuscarExacto_Click" />
                    </td>
                </tr>
            </tbody>
            <tfoot>
            </tfoot>
        </table>
        <asp:GridView ID="gvBusquedaExacta" runat="server" AutoGenerateColumns="False" CellPadding="4"
            ForeColor="#333333" GridLines="None">
            <AlternatingRowStyle BackColor="White" />
            <Columns>
                <asp:BoundField DataField="id_seguimiento" HeaderText="Seguimiento" />
                <asp:BoundField DataField="marca" HeaderText="Org. Ventas" />
                <asp:BoundField DataField="codigo_repuesto" HeaderText="Repuesto" />
                <asp:BoundField DataField="estado" HeaderText="Estado" />
                <asp:BoundField DataField="fecha_movimiento" HeaderText="Fecha Modfic." />
                <asp:BoundField DataField="fecha_eta" HeaderText="Fecha Estimada" />
                <asp:BoundField DataField="observacion" HeaderText="Obs." />
            </Columns>
            <EditRowStyle BackColor="#2461BF" />
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#EFF3FB" />
            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
            <SortedAscendingCellStyle BackColor="#F5F7FB" />
            <SortedAscendingHeaderStyle BackColor="#6D95E1" />
            <SortedDescendingCellStyle BackColor="#E9EBEF" />
            <SortedDescendingHeaderStyle BackColor="#4870BE" />
        </asp:GridView>
        <asp:Panel ID="pnlCriticidad" runat="server" Visible="false">
            <table>
                <thead>
                    <tr>
                        <td>
                            Criticidad
                        </td>
                        <td>
                            <asp:DropDownList ID="drCriticidad" runat="server">
                                <asp:ListItem Value="1">SI</asp:ListItem>
                                <asp:ListItem Value="0">NO</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>
                            Observación:
                        </td>
                        <td>
                            <asp:TextBox ID="txtObservacion" runat="server" MaxLength="25"></asp:TextBox>
                        </td>
                        <td colspan="2">
                            <asp:Button runat="server" ID="btnCriticidad" Text="Grabar" OnClick="btnCriticidad_Click" />
                        </td>
                    </tr>
                </tbody>
            </table>
        </asp:Panel>
        <%--<p>
            <a href="#" id="lnkTipoBusqueda">Realizar búsqueda por filtros</a>
        </p>--%>
    </div>
    <div id="divBusquedaFiltros">
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
        </asp:ToolkitScriptManager>
        <table>
            <tr>
                <td colspan="2">
                    <b>Filtros</b>
                </td>
            </tr>
            <tr id="trRut" runat="server">
                <td>
                    Rut:
                </td>
                <td>
                    <asp:TextBox ID="txtRut" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr id="trRutColegas" runat="server">
                <td>
                    Operarios
                </td>
                <td>
                    <asp:DropDownList ID="ddlOperarios" runat="server" DataSourceID="SqlDataSource1"
                        DataTextField="nombre" DataValueField="rut">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    Fecha Inicio:
                </td>
                <td>
                    <asp:TextBox ID="txtFechaBusquedaInicio" runat="server" ClientIDMode="Static"></asp:TextBox>
                    <asp:CalendarExtender ID="BuscarBoleCal" runat="server" TargetControlID="txtFechaBusquedaInicio"
                        PopupButtonID="ImageButton1" Format="yyyy-MM-dd" />
                    <asp:ImageButton runat="Server" ID="ImageButton1" ImageUrl="~/img/calendarIcon.png"
                        AlternateText="Haga clic aqui para desplegar el calendario" />
                </td>
            </tr>
            <tr>
                <td>
                    Fecha Fin:
                </td>
                <td>
                    <asp:TextBox ID="txtFechaBusquedaFin" runat="server" ClientIDMode="Static"></asp:TextBox>
                    <asp:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="txtFechaBusquedaFin"
                        PopupButtonID="ImageButton2" Format="yyyy-MM-dd" />
                    <asp:ImageButton runat="Server" ID="ImageButton2" ImageUrl="~/img/calendarIcon.png"
                        AlternateText="Haga clic aqui para desplegar el calendario" />
                </td>
            </tr>
            <tr>
                <td>
                    Tipo de Pedido:
                </td>
                <td>
                    <asp:DropDownList ID="ddTipoPedido" runat="server">
                        <asp:ListItem Value="">Todas</asp:ListItem>
                        <asp:ListItem Value="RESERVA">Reserva</asp:ListItem>
                        <asp:ListItem Value="NORMAL">VFC</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    Estado:
                </td>
                <td>
                    <asp:DropDownList ID="ddEstado" runat="server">
                        <asp:ListItem Value="">Todos</asp:ListItem>
                        <asp:ListItem Value="BACKORDER">Reserva</asp:ListItem>
                        <asp:ListItem Value="DISPONIBLE">Disponible</asp:ListItem>
                        <asp:ListItem Value="DESARME">VFC</asp:ListItem>
                        <asp:ListItem Value="PENDIENTE">Pendiente</asp:ListItem>
                        <asp:ListItem Value="EN PROCESO">En proceso</asp:ListItem>
                        <asp:ListItem Value="SOLICITUD IMPORTACIÓN">En solictud de importación</asp:ListItem>
                        <asp:ListItem Value="PEDIDO FABRICA">Pedido Fábrica</asp:ListItem>
                        <asp:ListItem Value="CERRADO">Cerrado</asp:ListItem>
                        <asp:ListItem Value="RECHAZO">Rechazo</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    Tipo de venta:
                </td>
                <td>
                    <asp:DropDownList ID="ddTipoVenta" runat="server">
                        <asp:ListItem Value="">Todas</asp:ListItem>
                        <asp:ListItem Value="NORMAL">Normal</asp:ListItem>
                        <asp:ListItem Value="GARANTIA">Garantía</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    Marca:
                </td>
                <td>
                    <asp:DropDownList ID="ddMarcas" runat="server">
                        <asp:ListItem Value="">Todas</asp:ListItem>
                        <asp:ListItem Value="SSANGYONG">SsangYong</asp:ListItem>
                        <asp:ListItem Value="MMCC">Mitsubishi</asp:ListItem>
                        <asp:ListItem Value="FIAT">Fiat</asp:ListItem>
                        <asp:ListItem Value="CHRYSLER">Chrysler</asp:ListItem>
                        <asp:ListItem Value="RAM">Ram</asp:ListItem>
                        <asp:ListItem Value="CHERY">Chery</asp:ListItem>
                        <asp:ListItem Value="MG">MG</asp:ListItem>
                        <asp:ListItem Value="TATA">TATA</asp:ListItem>
                        <asp:ListItem Value="JEEP">Jeep</asp:ListItem>
                        <asp:ListItem Value="DODGE">Dodge</asp:ListItem>
                        <asp:ListItem Value="ALFA ROMEO">Alfa Romeo</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
        </table>
        <asp:Button CssClass="button" ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" />
        &nbsp;
        <input class="button" type="button" value="Cancelar" id="btnCancelarBusquedaFiltros" />
        <asp:GridView ID="gvSolicitudes" runat="server" Caption="Lista de solicitudes" AutoGenerateColumns="False"
            AllowPaging="True" AllowSorting="True" BackColor="White" BorderColor="#999999"
            BorderStyle="None" BorderWidth="1px" CellPadding="1" GridLines="Vertical">
            <AlternatingRowStyle BackColor="#DCDCDC" />
            <Columns>
                <asp:HyperLinkField DataTextField="Id_solicitud" HeaderText="Solicitud" ItemStyle-HorizontalAlign="Center"
                    NavigateUrl="ListarSolicitudes.aspx" ControlStyle-CssClass="lnkIdSolicitud" />
                <asp:BoundField HeaderText="Producto" ItemStyle-HorizontalAlign="Center" DataField="producto">
                </asp:BoundField>
                <asp:BoundField HeaderText="VIN" ItemStyle-CssClass="gridViewRowSmall" ItemStyle-HorizontalAlign="Center"
                    DataField="vin"></asp:BoundField>
                <asp:BoundField HeaderText="Marca" ItemStyle-HorizontalAlign="Center" DataField="marca">
                </asp:BoundField>
                <asp:BoundField HeaderText="Fec.Est." ItemStyle-HorizontalAlign="Center" DataField="fecha_eta">
                </asp:BoundField>
                <asp:BoundField HeaderText="Creado" ItemStyle-HorizontalAlign="Center" DataField="fecha_creacion">
                </asp:BoundField>
                <asp:BoundField HeaderText="Venta" ItemStyle-HorizontalAlign="Center" DataField="tipovfc">
                </asp:BoundField>
                <asp:BoundField HeaderText="Dirección" ItemStyle-HorizontalAlign="Center" DataField="direccion">
                </asp:BoundField>
                <asp:BoundField HeaderText="Tipo" ItemStyle-HorizontalAlign="Center" DataField="tipopedido">
                </asp:BoundField>
                <asp:BoundField HeaderText="Estado" ItemStyle-HorizontalAlign="Center" DataField="estado">
                </asp:BoundField>
            </Columns>
            <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
            <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
            <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
            <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
            <SortedAscendingCellStyle BackColor="#F1F1F1" />
            <SortedAscendingHeaderStyle BackColor="#0000A9" />
            <SortedDescendingCellStyle BackColor="#CAC9C9" />
            <SortedDescendingHeaderStyle BackColor="#000065" />
        </asp:GridView>
        <center>
            <asp:Label ID="lblTexto" runat="server"></asp:Label>
            <br />
            <asp:Button CssClass="button" ID="btnExcel" Text="Generar Excel" runat="server" OnClick="btnExcel_Click" />
        </center>
        <div id="divDetalleSolicitud">
            <marquee>Cargando detalle de solicitud, Espere porfavor</marquee>
        </div>
    </div>
</asp:Content>
