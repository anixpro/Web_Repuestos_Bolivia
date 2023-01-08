<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
CodeFile="SolConsultaCotizacion.aspx.cs" Inherits="Vistas_SolConsultaCotizacion" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit.HTMLEditor"
    TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../css/jqueryUItheme/jquery.ui.all.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-ui-1.8.14.custom.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        function allowOnlyNumber(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode
            if (charCode > 31 && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }

        function format(input) {
            var num = input.value.replace(/\./g, '');
            if (!isNaN(num)) {
                num = num.toString().split('').reverse().join('').replace(/(?=\d*\.?)(\d{3})/g, '$1.');
                num = num.split('').reverse().join('').replace(/^[\.]/, '');
                input.value = num;
            }

            else {
                alert('Solo se permiten numeros');
                input.value = input.value.replace(/[^\d\.]*/g, '');
            }
        }
    
    </script>
    <style type="text/css">
        .modal
        {
            position: fixed;
            top: 0;
            left: 0;
            background-color: black;
            z-index: 99;
            opacity: 0.8;
            filter: alpha(opacity=80);
            -moz-opacity: 0.8;
            min-height: 100%;
            width: 100%;
        }
        .loading
        {
            font-family: Arial;
            font-size: 10pt;
            border: 5px solid White;
            width: 200px;
            height: 100px;
            display: none;
            position: fixed;
            background-color: White;
            z-index: 999;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        function ShowProgress() {
            setTimeout(function () {
                var modal = $('<div />');
                modal.addClass("modal");
                $('body').append(modal);
                var loading = $(".loading");
                loading.show();
                var top = Math.max($(window).height() / 2 - loading[0].offsetHeight / 2, 0);
                var left = Math.max($(window).width() / 2 - loading[0].offsetWidth / 2, 0);
                loading.css({ top: top, left: left });
            }, 200);
        }
        $('form').live("submit", function () {
//            ShowProgress();
        });
    </script>
    <asp:ToolkitScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="False">
    </asp:ToolkitScriptManager>
    <asp:Panel ID="Panel1" runat="server" Visible="false">
        <asp:HiddenField ID="hdPrueba" runat="server" />
    </asp:Panel>
    <div id="Panel_Procesando" class="modalBackground">
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ConnectionStrings:skbergeConnectionString%>">
        </asp:SqlDataSource>
        <div class="Titulo" id="titEdiSoli">
            Listado de Solicitudes de Cotizacion
        </div>
        <div id="mjsError" runat="server" clientidmode="Static">
            <asp:Label ID="lblmensaje" runat="server" Text="Label"></asp:Label>
        </div>
        <div id="msjesError" runat="server" clientidmode="Static">
        </div>
        <div id="NVFC" runat="server" clientidmode="Static">
        </div>
        <div>
            <div class="mantenedorUsuariosIzquierda">
                <div style="text-align: center;">
                    <b><u>Cotización</u></b></div>
                <ul>
                    <li>
                        <asp:HyperLink ID="hlkSolCotizacion" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/SolicitudCotizacion.aspx">Solicitud Cotización
                        </asp:HyperLink>
                    </li>
                    <li>
                        <asp:HyperLink ID="hlkConsultaCotizacion" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/SolConsultaCotizacion.aspx">Consulta Cotización
                        </asp:HyperLink>
                    </li>
                    <li>
                        <asp:HyperLink ID="hlReporteCotizacion" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/ReporteCotizacion.aspx">Reporte Cotización
                        </asp:HyperLink>
                    </li>
                </ul>
            </div>
            <asp:Panel ID="panelBuscador" runat="server" Height="350px">
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
                                    <td>
                                        Estado Solicitud
                                    </td>
                                    <td style="font-family: Arial, Helvetica, Sans-Serif" class="style1">
                                        <asp:DropDownList Width="150px" ID="ddlEstadoSol" runat="server" ClientIDMode="Static"
                                            AutoPostBack="false" TabIndex="1">
                                            <asp:ListItem Value="">...</asp:ListItem>
                                            <asp:ListItem Value="C">Cerrada</asp:ListItem>
                                            <asp:ListItem Value="A">Abierta</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Numero de Solicitud
                                    </td>
                                    <td style="font-family: Arial, Helvetica, Sans-Serif" class="style1">
                                        <asp:TextBox onkeypress="return allowOnlyNumber(event);" ID="txtnumsoli" Width="150px" runat="server" MaxLength="25" ClientIDMode="Static"
                                            AutoCompleteType="Disabled" TabIndex="2"></asp:TextBox>
                                    </td>
                                    <%

                                        if (System.Convert.ToBoolean(Session["aprobVFC"]))
                                        {
                         
                                    %>
                                    <td>
                                        Sucursal
                                    </td>
                                    <td style="font-family: Arial, Helvetica, Sans-Serif" class="style1">
                                        <asp:DropDownList Width="150px" ID="combLocal" runat="server" ClientIDMode="Static"
                                            AutoPostBack="false" TabIndex="1">
                                        </asp:DropDownList>
                                    </td>
                                    <%
                                        }

                                    %>
                                </tr>
                                <tr>
                                    <td>
                                        Fecha Desde
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtdesde" runat="server" Width="80px" Style="text-transform: uppercase"></asp:TextBox>
                                        <asp:ImageButton runat="Server" ID="btnDesde" ImageUrl="~/img/calendarIcon.png" AlternateText="Click here to display calendar" />
                                        <asp:CalendarExtender ID="fecDesde" runat="server" TargetControlID="txtdesde" PopupButtonID="btnDesde"
                                            Format="yyyy-MM-dd" />
                                    </td>
                                    <td>
                                        Fecha Hasta
                                        <asp:TextBox ID="txtHasta" runat="server" Width="80px" Style="text-transform: uppercase"></asp:TextBox>
                                        <asp:ImageButton runat="server" ID="btnHasta" ImageUrl="~/img/calendarIcon.png" AlternateText="Click here to display calendar" />
                                        <asp:CalendarExtender ID="fechasta" runat="server" TargetControlID="txtHasta" PopupButtonID="btnHasta"
                                            Format="yyyy-MM-dd" />
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
                                        <asp:Button ID="btnVolver" runat="server" Text="Volver" class="button" Visible="false" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <div id="pnlEditCotizacion" align="center" style="display:block; width:100%; overflow-x: auto; overflow-y: auto">
                <asp:Panel ID="PanelEdicion" runat="server">
                    <asp:UpdatePanel ID="asd" runat="server">
                        <ContentTemplate>
                            <p style="text-align: center; font-size: medium; font-weight: bold">
                                Detalle de Solicitud
                            </p>
                            <hr />
                            <div >
                                <asp:GridView ID="gvDetalleSolicitud" runat="server" AutoGenerateColumns="false"
                                    BackColor="White" BorderColor="GreenYellow" Caption="" CellPadding="3" Font-Size="12px"
                                    GridLines="Vertical" OnRowDataBound="gvDetalleSolicitud_RowDataBound" ShowFooter="true"
                                    Visible="false" Width="100%">
                                    <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                                    <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                                    <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                                    <HeaderStyle BackColor="Brown" Font-Bold="true" Font-Names="Tahoma" Font-Size="X-Small"
                                        ForeColor="White" />
                                    <AlternatingRowStyle BackColor="Gainsboro" />
                                    <Columns>
                                        <asp:TemplateField HeaderText="Numero" Visible="true">
                                            <ItemTemplate>
                                                <asp:Label ID="numsoli" runat="server" Text='<%# Eval("numeroSolicitud") %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="20px" Wrap="true" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Codigo" Visible="true">
                                            <ItemTemplate>
                                                <asp:Label ID="codrepto" runat="server" Text='<%# Eval("codRepto") %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="20px" Wrap="true" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Descripcion" Visible="true">
                                            <ItemTemplate>
                                                <asp:Label ID="desc" runat="server" Text='<%# Eval("descripcion") %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="25px" Wrap="true" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Cantidad" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtcantgrid" runat="server" Text='<%# Eval("cantidad") %>' Width="50"></asp:TextBox>
                                                <%-- <asp:Label ID="txtcantgrid" runat="server" Text='<%# Eval("cantidad") %>'></asp:Label>--%>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" Width="25px" />
                                        </asp:TemplateField>
                                        <asp:TemplateField FooterStyle-Font-Bold="true" HeaderText="Precio P/U" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:Label ID="txtvalorgrid" runat="server" Width="100px" ></asp:Label>
                                            </ItemTemplate>
                                            <FooterStyle Font-Bold="True" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="VIN" Visible="true">
                                            <ItemTemplate>
                                                <asp:Label ID="vin" runat="server" Text='<%# Eval("vin") %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="25px" Wrap="true" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Envio" Visible="true">
                                            <ItemTemplate>
                                                <asp:Label ID="envio" runat="server" Text='<%# Eval("envio") %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="85px" Wrap="true" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Dias Envio" Visible="true">
                                            <ItemTemplate>
                                                <asp:Label ID="dias" runat="server" Text='<%# Eval("dias") %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="55px" Wrap="true" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Tipo Solicitud" Visible="true">
                                            <ItemTemplate>
                                                <asp:Label ID="tipo" runat="server" Text='<%# Eval("tipo") %>'></asp:Label>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <b>Total :</b>
                                            </FooterTemplate>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="80px" Wrap="true" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Sub Total" Visible="true">
                                            <ItemTemplate>
                                                <asp:Label ID="subtotal" runat="server" Width="100px" ></asp:Label>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <b><%# GetTotal() %></b>
                                            </FooterTemplate>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="80px" Wrap="true" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Marca" Visible="true">
                                            <ItemTemplate>
                                                <asp:Label ID="marcadet" runat="server" Text='<%# Eval("marca") %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Height="10px" HorizontalAlign="Center" VerticalAlign="Middle" Width="40px"
                                                Wrap="true" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Confirmar" Visible="true">
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkConfirmar" runat="server" Checked='<%#Convert.ToBoolean(Eval("confirmada")) %>' />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="55px" Wrap="true" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="¿Es garantia?" Visible="true">
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkGarantia" runat="server" Checked='<%#Convert.ToBoolean(Eval("tipo_vfc")) %>' />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="55px" Wrap="true" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Criticidad" Visible="false">
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkCriticidad" runat="server" Checked='<%#Convert.ToBoolean(Eval("criticidad")) %>' />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="55px" Wrap="true" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Obs.Criticidad" ItemStyle-HorizontalAlign="Center"
                                            Visible="false">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtObsCriticidad" runat="server" Text='<%# Eval("obs_criticidad") %>'
                                                    Width="50"></asp:TextBox>
                                                <%-- <asp:Label ID="txtcantgrid" runat="server" Text='<%# Eval("cantidad") %>'></asp:Label>--%>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" Width="25px" />
                                        </asp:TemplateField>
                                        <asp:TemplateField Visible="True" HeaderText="Motivo">
                                            <ItemTemplate>
                                                <asp:Label ID="Motivo" runat="server" Text='<%# Eval("motivo") %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Height="15px" Width="20px"
                                                Wrap="True" />
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                            <asp:CheckBox ID="multisucursal" Visible="False" Enabled="False" runat="server" />
                            <div id="divgrid" style="width: 770px; height: 300px">
                            </div>
                            <div align="left">
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="../img/boton-agregar.jpg"
                                    OnClick="ImageButton1_Click1" ImageAlign="Left" />
                            </div>
                            <br />
                            <br />
                            <asp:Panel ID="pnlSucursales" runat="server" Visible="false" HorizontalAlign="Left">
                                <table>
                                    <tr>
                                        <td colspan="2">
                                            <table style="max-width: 780px">
                                                <tr>
                                                    <td class="style3">
                                                        El pedido sera enviado a:
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lblDireccion" Visible="False" runat="server" Width="400px"></asp:Label>
                                                        <asp:DropDownList ID="ListaSucursales" runat="server" Visible="false" Width="400px"
                                                            DataTextField="direccionSucursal" DataValueField="shipCode" AppendDataBoundItems="true">
                                                            <asp:ListItem Value="" Text="--Seleccione Dirección--"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <div align="left">
                                <asp:Button ID="BtnConfirmar" runat="server" CssClass="button" Height="30px" Text="Confirmar"
                                    OnClick="BtnConfirmar_Click" />
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </asp:Panel>
            </div>
            <div id="loadingdiv" class="loading" align="center" style="display: none;">
                Cargando...<br />
                <br />
                <img src="../img/loading.gif" alt="" />
            </div>
            <asp:Panel ID="PanelListado" Visible="true" runat="server" style="display:block; width:100%; overflow-x: auto; overflow-y: auto;">
                <p style="text-align: center; font-size: large; font-weight: bold">
                    Listado de Solicitudes de Cotizacion
                </p>
                <hr />
                <div id="divGrd" style="overflow-x: hidden; overflow-y: visible; width: 800px; height: 550px">
                    <asp:GridView ID="gvListSolicitud" runat="server" AutoGenerateColumns="false" BackColor="White"
                        GridLines="Vertical" Font-Size="10px" widht="900px" CellPadding="3" BorderColor="#999999"
                        Caption="" Width="770px" OnRowCommand="editLinea" OnRowCreated="gvListSolicitud_RowCreated">
                        <FooterStyle BackColor="SkyBlue" ForeColor="Black" />
                        <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                        <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                        <HeaderStyle BackColor="#000084" Font-Bold="true" ForeColor="White" Font-Names="Tahoma"
                            Font-Size="Small" />
                        <AlternatingRowStyle BackColor="Gainsboro" />
                        <Columns>
                            <asp:TemplateField Visible="true" HeaderText="Numero">
                                <ItemTemplate>
                                    <asp:Label ID="numeroSolicitud" runat="server" Text='<%#Eval("numeroSolicitud") %>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" Width="30px" Height="20px" VerticalAlign="Middle"
                                    Wrap="true" />
                            </asp:TemplateField>

                            <asp:TemplateField Visible="true" HeaderText="Fecha">
                                <ItemTemplate>
                                    <asp:Label ID="fecha" runat="server" Text='<%#Eval("fecha") %>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" Width="60px" Height="10px" VerticalAlign="Middle"
                                    Wrap="true" />
                            </asp:TemplateField>

                            <asp:TemplateField Visible="true" HeaderText="Marca">
                                <ItemTemplate>
                                    <asp:Label ID="marca" runat="server" Text='<%# Eval("marca") %>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" Width="40px" Height="10px" VerticalAlign="Middle"
                                    Wrap="true" />
                            </asp:TemplateField>

                            <asp:TemplateField Visible="false" HeaderText="Codigo">
                                <ItemTemplate>
                                    <asp:Label ID="codRepto" runat="server" Text='<%# Eval("codRepto") %>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" Width="25px" Height="10px" VerticalAlign="Middle"
                                    Wrap="true" />
                            </asp:TemplateField>

                            <asp:TemplateField Visible="false" HeaderText="Detalle">
                                <ItemTemplate>
                                    <asp:Label ID="descripcion" runat="server" Text='<%# Eval("descripcion") %>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="20px" Height="10px"
                                    Wrap="true" />
                            </asp:TemplateField>

                            <asp:TemplateField Visible="false" HeaderText="Cantidad">
                                <ItemTemplate>
                                    <asp:Label ID="cantidad" runat="server" Text='<%# Eval("cantidad") %>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Height="10px" Width="20px"
                                    Wrap="true" />
                            </asp:TemplateField>

                            <asp:TemplateField Visible="true" HeaderText="Concesionario">
                                <ItemTemplate>
                                    <asp:Label ID="concesionario" runat="server" Text='<%# Eval("concesionario") %>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Height="10px" Width="50px"
                                    Wrap="true" />
                            </asp:TemplateField>

                            <asp:TemplateField Visible="true" HeaderText="Tipo">
                                <ItemTemplate>
                                    <asp:Label ID="tipo" runat="server" Text='<%# Eval("tipo") %>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Height="10px" Width="40px"
                                    Wrap="true" />
                            </asp:TemplateField>

                            <asp:TemplateField Visible="true" HeaderText="VIN">
                                <ItemTemplate>
                                    <asp:Label ID="vin" runat="server" Text='<%# Eval("vin") %>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Height="10px" Width="50px"
                                    Wrap="true" />
                            </asp:TemplateField>

                            <asp:TemplateField Visible="true" HeaderText="Transporte">
                                <ItemTemplate>
                                    <asp:Label ID="envio" runat="server" Text='<%# Eval("envio") %>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Height="20px" Width="40px"
                                    Wrap="true" />
                            </asp:TemplateField>

                            <asp:TemplateField Visible="true" HeaderText="Dias">
                                <ItemTemplate>
                                    <asp:Label ID="dias" runat="server" Text='<%# Eval("dias") %>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Height="10px" Width="20px"
                                    Wrap="true" />
                            </asp:TemplateField>

                            <asp:TemplateField Visible="false" HeaderText="Precio">
                                <ItemTemplate>
                                    <asp:Label ID="precio" runat="server" Text='<%# Eval("precio_Solicitud") %>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Height="10px" Width="20px"
                                    Wrap="true" />
                            </asp:TemplateField>
                            <asp:TemplateField Visible="True" HeaderText="Estado">
                                <ItemTemplate>
                                    <asp:Label ID="Estado" runat="server" Text='<%# Eval("Estado") %>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Height="15px" Width="20px"
                                    Wrap="True" />
                            </asp:TemplateField>

                            <asp:TemplateField Visible="True" HeaderText="Confirmada">
                                <ItemTemplate>
                                    <asp:Label ID="Finalizada" runat="server" Text='<%# Eval("Confirmada") %>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Height="15px" Width="20px"
                                    Wrap="True" />
                            </asp:TemplateField>
                            <asp:TemplateField Visible="True" HeaderText="Sucursal">
                                <ItemTemplate>
                                    <asp:Label ID="Sucursal" runat="server" Text='<%# Eval("direccionSucursal") %>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Height="15px" Width="20px"
                                    Wrap="True" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="">
                                <ItemTemplate>
                                    <asp:Label ID="lblRechazo" runat="server" Text="✖" ToolTip='<%# Eval("comentario") %>'
                                        Visible="false"></asp:Label>
                                    <asp:LinkButton ID="btnEditar" runat="server" OnClick="btnEditar_Click" CommandArgument='<%# Eval("anulada") %>'
                                        Text="Editar"></asp:LinkButton>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <br />
                    <table align="left">
                        <tr>
                            <td align="left">
                                &nbsp;
                                <asp:Button runat="server" ID="btnExportExell" Text="Excel" class="button" OnClick="btnExportExell_Click" />
                            </td>
                        </tr>
                    </table>
                </div>
                <br />
            </asp:Panel>
        </div>
    </div>
</asp:Content>
