<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="BuscarRepto2.aspx.cs" Inherits="Vistas_buscarRepto" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script src="../js/jquery.numeric.js" type="text/javascript"></script>
    
      <script type="text/javascript">

        function KeyBackspace(keyStroke) {
            isNetscape = (document.layers);
            eventChooser = (isNetscape) ? keyStroke.which : event.keyCode;
            if (eventChooser == 13) {
                return false;
            }
        }
        document.onkeypress = KeyBackspace;

        document.onkeydown = function () {
            if (window.event && window.event.keyCode == 8) {
                window.event.keyCode = 505;
            }
            if (window.event && window.event.keyCode == 505) {
                return false;
            }
        }


        var txtVFc;
        var rbVFC;

        function pageLoad(sender, args) {



            $(".txtCantidad").numeric({ decimal: false, negative: false }, function () { alert("Solo enteros positivos"); this.value = ""; this.focus(); });
            $("#txtCodigo").keyup(function () {
                //$("#txtCodigo").val($("#txtCodigo").val().toUpperCase());
            });

            $("#PanelPreferido").hide();

            $("#vfcTester").dialog({
                autoOpen: false,
                modal: true,
                resizable: false,
                width: 600,
                height: 400,
                title: 'Comprobar VIN',
                position: 'center'
            });

            $(".rbVFC").click(function (e) {
                e.preventDefault();
                txtVFc = $(this).next();
                rbVFC = $(this).children("input");
                /*alert ($(this).parent().parent().parent().find('td:first').next().next().children().html());*/
                $("#vfcTester").dialog("open");
                $("#inMarca").val($(this).parent().parent().parent().find('td:first').next().next().children().html());
            });

            /* $("#btnOK").click(function () {
            
						   		
            });
            */

            $("#btnSalir").click(function () {
                $("#infoVIN").html("");
                $("#vfcTester").dialog("close");
            });

            $("#btnActivarPreferido").click(function () {
                $("#PanelPreferido").show(1000);
                $("#btnActivarPreferido").hide(1000);
            });

            $("#btnCancelarPreferido").click(function () {
                $("#btnActivarPreferido").show(1000);
                $("#PanelPreferido").hide(1000);
            });


            $("form input").keypress(function (e) {
                if ((e.which && e.which == 13) || (e.keyCode && e.keyCode == 13)) {
                    $('#btnBuscarCodigo').click();
                    return false;
                } else {
                    return true;
                }
            });
            /*$("#btnComprobarVIN").click(function () {
            checkvin();
            });*/
        }
        function checkvin() {
            $("#infoVIN").html("<img src='../img/lightbox-ico-loading.gif'><b>Comprobando ...</b>");

            if ($("#inVin").val().length < 6) {
                alert("Ingrese un código VIN más largo");
                $("#infoVIN").html("");
                return false;
            }

            var checkVinAjax = $.ajax({
                type: "GET",
                cache: false,
                url: "ValidaVin.aspx",
                data: { vin: $("#inVin").val(), marca: $("#inMarca").val() },
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
                                data: { vin: $("#inVin").val() },
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

        function checkvinok() {
            if ($("#inVin").val() == "") {
                $("#infoVIN").html("<b>Debe un ingresar código VIN porfavor</b>");
            } else {
                $("#infoVIN").html("<img src='../img/lightbox-ico-loading.gif'><b>Comprobando ...</b>");
                if ($("#inVin").val().length < 6) {
                    alert("Ingrese un código VIN más largo");
                    $("#infoVIN").html("");
                    return false;
                }
                var checkVinAjax = $.ajax({
                    type: "GET",
                    cache: false,
                    url: "ValidaVin.aspx",
                    data: { vin: $("#inVin").val(), marca: $("#inMarca").val() },
                    dataType: "html"
                });

                var bool = "";
                checkVinAjax.done(
								function (data) {
								    if (data == 'false') {
								        alert("Ingrese un código VIN valido");
								        $("#infoVIN").html("");
								        return;
								    } else {

								        $("#infoVIN").html("");
								        $("#vfcTester").dialog("close");
								        $(txtVFc).val($("#inVin").val());
								        $(rbVFC).attr("checked", "checked");


								    }
								}
                );
            }
        }	
			
    </script>
    <style>
        .ColumnaOculta {display:none;}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" >
        <ContentTemplate>
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:skbergeConnectionString %>">
            </asp:SqlDataSource>
            <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:skbergeConnectionString %>">
            </asp:SqlDataSource>
            <!-- Inputs ocultos -->
            <asp:Label ID="lblCantidad" CssClass="invi" runat="server" Text="0"></asp:Label>
            <asp:Label ID="lblNumeroPedido" runat="server" CssClass="invi" Text=""></asp:Label>
            <input id="hiddenPrioridad" type="text" runat="server" class="invi" value="" />
            <asp:HiddenField ID="ultimaMarca" runat="server" />
            <asp:HiddenField ID="ultimoCodigo" runat="server" />
            <asp:HiddenField ID="ultimaCantidad" runat="server" />
            <asp:HiddenField ID="ultimaDescripcion" runat="server" />
            <!-- Fin inputs ocultos -->
            <div class="titulo" id="titCotizar">
                Búsqueda de Repuestos
            </div>
            <!-- Acá se despliegan los mensajes de error -->
            <div id="msjesError" runat="server" clientidmode="Static">
            </div>
            <!--Sección Buscar. Todas las grillas y botones para realizar busquedas de repuestos-->
            <asp:Panel ID="PanelSeccionBusqueda" runat="server">
                <table>
                    <tr>
                        <td class="tdBuscaRepuesto" runat="server" clientidmode="Static" id="tdBuscaRepusto">
                            <table>
                                <tr>
                                    <td>
                                        Marca:
                                    </td>
                                    <td style="font-family: Arial, Helvetica, sans-serif" class="style1">
                                        <asp:DropDownList Width="120px" ID="ComboMarcas" runat="server" ClientIDMode="Static"
                                            AutoPostBack="false">
                                        </asp:DropDownList>
                                        <br />
                                        <asp:LinkButton ID="btnNewSearch" Visible="false" runat="server" OnClientClick="javascript:return(confirm('Realizar una nueva búsqueda eliminará todos los elementos actuales. ¿Esta seguro?'))"
                                            OnClick="btnNewSearch_Click" TabIndex="1">Nueva Búsqueda</asp:LinkButton>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Código:
                                    </td>
                                    <td>
                                        <asp:TextBox MaxLength="25" ID="txtCodigo" runat="server" Width="120px" ClientIDMode="Static"
                                            AutoCompleteType="Disabled" TabIndex="2"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Cantidad:
                                    </td>
                                    <td>
                                        <asp:TextBox CssClass="txtCantidad" ID="txtCantidad" runat="server" MaxLength="4"
                                            Width="50px" ClientIDMode="Static" TabIndex="3"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblNombre" runat="server" Text="Nombre Repuesto: "></asp:Label>
                                    </td>
                                    <td class="style1">
                                        <asp:TextBox MaxLength="70" ID="txtNombre" Width="120px" runat="server" ClientIDMode="Static"
                                            TabIndex="4"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" align="right">
                                    <Triggers>
                                        <asp:ImageButton ID="btnBuscarCodigo" runat="server" ImageUrl="~/img/buscarBtn.png"
                                            ClientIDMode="Static" TabIndex="5" OnClick="btnBuscarCodigo_Click" Height="25px" />
                                            </Triggers>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <asp:Panel ID="PanelAnteriores" runat="server">
                                            <asp:HyperLink ID="btnVerAnteriores" runat="server" NavigateUrl="~/Vistas/consultarpedido.aspx">Consultar estado</asp:HyperLink>
                                        </asp:Panel>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" align="center">
                                        <asp:Panel ID="PanelVerPreferido" runat="server" ClientIDMode="Static">
                                            <table style="border: thin groove #333333" id="tblVerPreferido">
                                                <tr>
                                                    <td colspan="2" align="center">
                                                        Ver Preferido
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        Referencia:
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtNPref" Width="120px" runat="server"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2" align="center">
                                                        <asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click"
                                                            class="button" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td class="tdResultadoBusqueda" id="tdResultadoBusqueda" runat="server">
                            <!-- Div que despliega mensaje que ofrece transformar a VFC busqueda de repuesto -->
                            <div id="VFCbusqueda" runat="server" class="vfcBuscarRepuesto" clientidmode="Static">
                                <center>
                                    <b>Repuesto no tiene stock.<br />
                                        ¿Desea transformarlo en una solicitud de VFC ?</b>
                                    <p>
                                        <asp:Button ID="btnVFCsi" Text="Sí" runat="server" OnClick="btnVFCsi_Click" />
                                        &nbsp;
                                        <asp:Button ID="btnVFCno" Text="No" runat="server" OnClick="btnVFCno_Click" />
                                    </p>
                                </center>
                            </div>
                            <!-- GridView con los resultados de todos los preferidos-->
                            <asp:GridView ID="GridViewTodosPreferidos" runat="server" AutoGenerateColumns="False"
                                EmptyDataText="La búsqueda no arrojó resultados" ShowFooter="True" BackColor="White"
                                GridLines="Vertical" Font-Size="12px" Width="100%" CellPadding="3" BorderColor="#999999"
                                Caption="Pedidos preferidos" Style="margin-left: 0px" Visible="false">
                                <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                                <RowStyle Font-Size="10px" BackColor="#EEEEEE" ForeColor="Black" />
                                <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                                <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" Font-Names="Tahoma"
                                    Font-Size="XX-Small" />
                                <AlternatingRowStyle BackColor="Gainsboro" />
                                <Columns>
                                    <asp:BoundField DataField="idPreferido" ItemStyle-HorizontalAlign="Center" HeaderText="N° de preferido"
                                        ReadOnly="True">
                                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="textoDescrip" ItemStyle-HorizontalAlign="Center" HeaderText="Descripción"
                                        ReadOnly="True">
                                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="fechaCreacion" ItemStyle-HorizontalAlign="Center" HeaderText="Fecha Creación"
                                        ReadOnly="True">
                                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:CommandField SelectText="" ShowSelectButton="True" FooterStyle-HorizontalAlign="Center"
                                        ButtonType="Image" SelectImageUrl="~/img/mas.png" />
                                </Columns>
                            </asp:GridView>
                            <!-- Fin todos preferidos-->
                            <!-- GridView con los resultados de la busqueda de reptos-->
                            <asp:GridView ID="GridViewListaRep" runat="server" AutoGenerateColumns="False" EmptyDataText="Seleccione marca, ingrese código y cantidad. Otra opcion es buscar por nombre de repuesto. En la segunda opción, puede realizar búsquedas parciales poniendo un * al final del nombre"
                                ShowFooter="True" BackColor="White" GridLines="Vertical" Font-Size="11px" Width="100%"
                                CellPadding="3" BorderColor="#999999" DataSourceID="SqlDataSource1" Caption="Resultado de búsqueda"
                                Style="margin-left: 0px" AllowPaging="true" AllowSorting="true" OnPageIndexChanging="ResultadoBusqueda_PageIndexChanging"
                                OnRowDataBound="GridViewListaRep_RowDataBound">
                                <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                                <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                                <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                                <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" Font-Names="Tahoma"
                                    Font-Size="XX-Small" />
                                <AlternatingRowStyle BackColor="Gainsboro" />
                                <Columns>
                                    <asp:BoundField DataField="MARCA" FooterStyle-CssClass="invi" ItemStyle-CssClass="invi"
                                        HeaderStyle-CssClass="invi" ItemStyle-HorizontalAlign="Center" HeaderText="Marca"
                                        ReadOnly="True">
                                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CODIGO" ItemStyle-HorizontalAlign="Center" HeaderText="Código"
                                        ReadOnly="True">
                                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="DESCRIP" ItemStyle-HorizontalAlign="Center" HeaderText="Descripción"
                                        ReadOnly="True">
                                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="GRUPO_MAT" FooterStyle-CssClass="invi" ItemStyle-CssClass="invi"
                                        HeaderStyle-CssClass="invi" ItemStyle-HorizontalAlign="Center" HeaderText="Grupo Material"
                                        ReadOnly="True">
                                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="PRECIO_LISTA" ItemStyle-HorizontalAlign="Center" HeaderText="Precio Lista Sugerido"
                                        ReadOnly="True">
                                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="PRECION_CONCE" ItemStyle-HorizontalAlign="Center" HeaderText="Precio Concesionario"
                                        ReadOnly="True">
                                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField FooterStyle-CssClass="invi" ItemStyle-CssClass="invi" HeaderStyle-CssClass="invi"
                                        DataField="STOCK" ItemStyle-HorizontalAlign="Center" HeaderText="STOCK" ReadOnly="True">
                                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField FooterStyle-CssClass="invi" ItemStyle-CssClass="invi" HeaderStyle-CssClass="invi"
                                        DataField="CANTIDAD" ItemStyle-HorizontalAlign="Center" HeaderText="CANTIDAD"
                                        ReadOnly="True">
                                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="DESCUENTO" ItemStyle-HorizontalAlign="Center" HeaderText="Descuento"
                                        ReadOnly="true">
                                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                    </asp:BoundField>
                                     <asp:BoundField ItemStyle-HorizontalAlign="Center" HeaderText="Tiene Stock" ReadOnly="True">
                                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="GRUPO" ItemStyle-HorizontalAlign="Center" HeaderText="Grupo" ReadOnly="True">
                                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:CommandField SelectText="" ShowSelectButton="True" FooterStyle-HorizontalAlign="Center"
                                        ButtonType="Image" SelectImageUrl="~/img/cart.png" />
                                </Columns>
                            </asp:GridView>
                            <!-- Fin busqueda de reptos-->
                            <br />
                            Si el repuesto buscado no tiene precio (S/P) contáctese con su Administrador de Ventas vía correo. Si solicita importación VFC, su respaldo de precio será el correo.
                            <br />
                            <!--gridViewReemplazos-->
                            <asp:GridView ID="GridViewReemplazos" Caption="Cadena de reemplazo" runat="server"
                                AutoGenerateColumns="False" EmptyDataText="" BackColor="White"
                                GridLines="Vertical" Font-Size="12px" Width="100%" CellPadding="3" BorderColor="#999999"
                                AllowPaging="true" AllowSorting="true" DataSourceID="SqlDataSource2" OnRowDataBound="GridViewReemplazos_RowDataBound">
                                <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                                <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                                <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" Font-Names="Tahoma"
                                    Font-Size="XX-Small" />
                                <AlternatingRowStyle BackColor="Gainsboro" />
                                <Columns>
                                    <asp:BoundField DataField="MARCA" FooterStyle-CssClass="invi" ItemStyle-CssClass="invi"
                                        HeaderStyle-CssClass="invi" ItemStyle-HorizontalAlign="Center" HeaderText="Marca"
                                        ReadOnly="True">
                                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="T_MFRPN" ItemStyle-HorizontalAlign="Center" HeaderText="Código"
                                        ReadOnly="True">
                                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="T_MAKTX" ItemStyle-HorizontalAlign="Center" HeaderText="Descripción"
                                        ReadOnly="True">
                                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="T_KONDM" FooterStyle-CssClass="invi" ItemStyle-CssClass="invi"
                                        HeaderStyle-CssClass="invi" ItemStyle-HorizontalAlign="Center" HeaderText="Grupo Material"
                                        ReadOnly="True">
                                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="T_KBETR2" ItemStyle-HorizontalAlign="Center" HeaderText="Precio"
                                        ReadOnly="True" DataFormatString="{0:c}" HeaderStyle-CssClass="ColumnaOculta">
                                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField FooterStyle-CssClass="invi" ItemStyle-CssClass="invi" HeaderStyle-CssClass="invi"
                                        DataField="T_KBETR1" ItemStyle-HorizontalAlign="Center" HeaderText="Precio Concesionario"
                                        ReadOnly="True" DataFormatString="{0:c}">
                                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField FooterStyle-CssClass="invi" ItemStyle-CssClass="invi" HeaderStyle-CssClass="invi"
                                        DataField="STOCK" ItemStyle-HorizontalAlign="Center" HeaderText="STOCK" ReadOnly="True">
                                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField FooterStyle-CssClass="invi" ItemStyle-CssClass="invi" HeaderStyle-CssClass="invi"
                                        DataField="CANTIDAD" ItemStyle-HorizontalAlign="Center" HeaderText="CANTIDAD"
                                        ReadOnly="True">
                                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                    </asp:BoundField>
                                     <asp:BoundField ItemStyle-HorizontalAlign="Center" HeaderText="Tiene Stock" ReadOnly="True">
                                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                    </asp:BoundField>
                                     <asp:BoundField DataField="GRUPO" ItemStyle-HorizontalAlign="Center" HeaderText="Grupo" ReadOnly="True">
                                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:CommandField SelectText="" ShowSelectButton="True" FooterStyle-HorizontalAlign="Center"
                                        ButtonType="Image" SelectImageUrl="~/img/cart.png" HeaderStyle-CssClass="ColumnaOculta" />
                                </Columns>
                            </asp:GridView>
                            <center>
                                <asp:Label ID="lblTextoResultado" Text="" runat="server"></asp:Label>
                            </center>
                            <!--Fin reemplazos-->
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <!--Fin Sección Buscar-->
            <!--Panel Carro. Todas los componentes del carro de compras-->
            <asp:Panel ID="PanelCarroDeCompras" runat="server" Visible="false">
                <p style="text-align: center; font-size: large; font-weight: bold">
                    Carro de Compra
                </p>
                <hr />
                <!--GridView con el carro-->
                <div id="divGrid" style="overflow: auto; width: 770px; height: 300px">
                    <p >
                        <asp:Label ID="lblmensajecarro" runat="server" Text="Label" Visible="false" BackColor="Red"></asp:Label>
                    </p>
                    <asp:GridView ID="GridViewCarro" runat="server" AutoGenerateColumns="false" ShowFooter="true"
                        BackColor="White" GridLines="Vertical" Font-Size="12px" Width="100%" CellPadding="4"
                        BorderColor="#999999" Caption="Lista de repuestos Para Pedido">
                        <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                        <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                        <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                        <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" Font-Names="Tahoma"
                            Font-Size="XX-Small" />
                        <AlternatingRowStyle BackColor="Gainsboro" />
                        <Columns>
                            <asp:BoundField DataField="marca" FooterStyle-CssClass="invi" ItemStyle-CssClass="invi"
                                HeaderStyle-CssClass="invi" ItemStyle-HorizontalAlign="Center" HeaderText="Marca"
                                ReadOnly="True">
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="codigo" ItemStyle-HorizontalAlign="Center" HeaderText="Código"
                                ReadOnly="True">
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="descripcion" ItemStyle-HorizontalAlign="Center" HeaderText="Descripción"
                                ReadOnly="True">
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="Cantidad" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtCantidad" ReadOnly="false" runat="server" Text='<%# Eval("cantidad") %>'
                                        Width="60px"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="precioL" ItemStyle-HorizontalAlign="Center" HeaderText="Precio Lista Sugerido"
                                ReadOnly="True">
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="precioC" ItemStyle-HorizontalAlign="Center" HeaderText="Precio Concesionario"
                                ReadOnly="True">
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="Total" FooterStyle-Font-Bold="True">
                                <ItemTemplate>
                                    $<%# GetUnitPrice2(double.Parse(Eval("totalC").ToString())).ToString("N")%>
                                </ItemTemplate>
                                <FooterTemplate>
                                    $<%# GetTotal2().ToString("N") %>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="precioL" FooterStyle-CssClass="invi" ItemStyle-CssClass="invi"
                                HeaderStyle-CssClass="invi" ItemStyle-HorizontalAlign="Center" HeaderText="Precio Lista Sugerido"
                                ReadOnly="True">
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="precioC" FooterStyle-CssClass="invi" ItemStyle-CssClass="invi"
                                HeaderStyle-CssClass="invi" ItemStyle-HorizontalAlign="Center" HeaderText="Precio Concesionario"
                                ReadOnly="True">
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="descuento" ItemStyle-HorizontalAlign="Center" HeaderText="Descuento"
                                ReadOnly="true">
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:BoundField>
                            <asp:BoundField DataField="descuento" FooterStyle-CssClass="invi" ItemStyle-CssClass="invi"
                                HeaderStyle-CssClass="invi" ItemStyle-HorizontalAlign="Center" HeaderText="Descuento"
                                ReadOnly="true">
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:BoundField>
                            
                            <asp:CommandField HeaderText="Quitar" ShowSelectButton="true" ButtonType="Image"
                                SelectImageUrl="~/img/gtk-no.png" ControlStyle-Width="10px" ControlStyle-Height="10px"
                                ItemStyle-HorizontalAlign="Center" />

                            <%--                        <asp:TemplateField HeaderText="Stock">
                            <ItemTemplate>
                                <%#determinaStock(Eval("stock").ToString(), Eval("cantidad").ToString())%>
                            </ItemTemplate>
                        </asp:TemplateField>--%>
                        </Columns>
                    </asp:GridView>
                    <asp:Button ID="btnUdpCarro" runat="server" Text="Actualizar Carro" 
                        onclick="btnUdpCarro_Click" />
                </div>
                <asp:Panel ID="PanelCotizarComun" runat="server" Visible="false">
                    <asp:Button ID="btnGenerarPdf" runat="server" Text="Generar PDF" OnClick="btnGenerarPdf_Click" />
                    <asp:Button ID="btnLimpiarCarro" runat="server" Text="Limpiar Carro" OnClick="btnLimpiarCarro_Click" />
                </asp:Panel>
            </asp:Panel>
            <!--Fin carro-->
            <!--GridViewNoStock. Si un repuesto no tiene stock aqui apareceran los componentes para procesar ese evento-->
            <asp:Panel ID="PanelNoStock" runat="server" Visible="false">
                <asp:GridView ID="GridViewNoStock" runat="server" AutoGenerateColumns="False" ShowFooter="True"
                    BackColor="White" GridLines="Vertical" Font-Size="12px" Width="100%" CellPadding="3"
                    BorderColor="#999999" Caption="Lista de repuestos SIN Stock a solicitar">
                    <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                    <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                    <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                    <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                    <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" Font-Names="Tahoma"
                        Font-Size="XX-Small" />
                    <AlternatingRowStyle BackColor="Gainsboro" />
                    <Columns>
                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="Reserva" Visible="true" >
                            <ItemTemplate>
                                <asp:RadioButton GroupName="noStock" ID="rbSeleccionarBO" runat="server" Text="" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="VFC">
                            <ItemTemplate>
                                <asp:RadioButton GroupName="noStock" ID="rbSeleccionarVFC" runat="server" Text=""
                                    CssClass="rbVFC" />
                                <asp:TextBox ID="txtVin" Width="100px" runat="server"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="Marca">
                            <ItemTemplate>
                                <div id="marca[]" class="marcaclass" name="marca[]">
                                    <%# Eval("marca") %></div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="Descartar">
                            <ItemTemplate>
                                <asp:RadioButton GroupName="noStock" Checked="true" ID="rbSeleccionarDes" runat="server"
                                    Text="" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="codigo" ItemStyle-HorizontalAlign="Center" HeaderText="Código"
                            ReadOnly="True">
                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        </asp:BoundField>
                        <asp:BoundField DataField="descripcion" ItemStyle-HorizontalAlign="Center" HeaderText="Descripción"
                            ReadOnly="True">
                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Cantidad" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:TextBox ID="txtCantidad" ReadOnly="true" runat="server" Text='<%# Eval("cantidad") %>'
                                    Width="40px" onkeypress="KeyBackspace"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="precioL" ItemStyle-HorizontalAlign="Center" HeaderText="Precio Lista Sugerido"
                            ReadOnly="True">
                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        </asp:BoundField>
                        <asp:BoundField DataField="precioC" ItemStyle-HorizontalAlign="Center" HeaderText="Precio Concesionario"
                            ReadOnly="True">
                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Total" FooterStyle-Font-Bold="True">
                            <ItemTemplate>
                                $<%# GetUnitPrice(double.Parse(Eval("totalC").ToString())).ToString("N")%>
                            </ItemTemplate>
                            <FooterTemplate>
                                $<%# GetTotal().ToString("N") %>
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <center>
                    <i>NOTA: Por defecto los repuestos SIN Stock no serán solicitados en VFC. De requerirlos, activar el botón VFC e ingresar el VIN del vehículo para confirmar la importación</i></center>
            </asp:Panel>
            <!--Fin No Stock-->
            <asp:Panel ID="PanelCotizarSap" runat="server" Visible="false">
                <hr />
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
                                        <asp:DropDownList ID="ListaSucursales" runat="server" Visible="false" Width="400px">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Señale si pedido es normal o garantía:
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlTipoDePedido" runat="server">
                                            <asp:ListItem>Seleccione</asp:ListItem>
                                            <asp:ListItem Value="normal">Normal</asp:ListItem>
                                           
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Motivo del Pedido:
                                    </td>
                                    <td>
                                        <asp:DropDownList runat="server" ID="ddlmotivodepedido">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td colspan="2">
                            <asp:Button ID="btnCotizar" runat="server" Text="Generar Pedido" OnClick="btnCotizar_Click"
                                CssClass="button" />
                            <asp:Button ID="btnLimpiarCarroC" runat="server" Text="Limpiar Carro" CssClass="button"
                                OnClick="btnLimpiarCarroC_Click" OnClientClick="javascript:return(confirm('¿Seguro que desea limpiar el carro?'))" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <input type="button" id="btnActivarPreferido" value="Crear Preferido" class="button" />
                            <asp:Panel ID="PanelPreferido" runat="server" ClientIDMode="Static">
                                <table>
                                    <tr>
                                        <td colspan="2">
                                            Guardar Como Pedido Preferido
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Referencia:
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtRefPrefe" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <asp:Button ID="btnGuardarPref" runat="server" Text="Guardar Como Preferido" OnClick="btnGuardarPref_Click"
                                                CssClass="button" />
                                            &nbsp;
                                            <input id="btnCancelarPreferido" type="button" value="Cancelar" class="button" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:Panel ID="PanelSoloHNoStock" runat="server" Visible="false">
                <table>
                    <tr>
                        <td>
                            Las reservas y VFCs serán enviado a:
                        </td>
                        <td>
                            <asp:Label ID="lblDestinatarioNoStock" Visible="False" runat="server" Width="400px"></asp:Label>
                            <asp:DropDownList ID="ddlSucursalesNoStock" runat="server" Visible="false" Width="400px">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Señale si las reservas/VFCs son normal o garantía:
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlTipoPedidoReserva" runat="server">
                                <asp:ListItem>Seleccione</asp:ListItem>
                                <asp:ListItem Value="normal">Normal</asp:ListItem>
                                <asp:ListItem Value="garantia">Garantía</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnEnviarNoStock" runat="server" Text="Enviar" OnClick="btnEnviarNoStock_Click"
                                CssClass="button" />
                        </td>
                        <td>
                            <asp:Button ID="btnLimpiarSoloNoStock" runat="server" Text="Limpiar" CssClass="button"
                                OnClick="btnLimpiarSoloNoStock_Click" OnClientClick="javascript:return(confirm('¿Seguro que desea limpiar?'))" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnGenerarPdf" />
        </Triggers>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UP1" runat="server" DisplayAfter="0" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <script type="text/javascript">
                document.write("<div class='UpdateProgressBackground'></div>");
            </script>
            <center>
                <div class="UpdateProgressContent">
                    Cargando...</div>
            </center>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <div id="vfcTester">
        <p>
            Ingrese VIN</p>
        <p>
            <input type="text" id="inVin" /><input type="hidden" id="inMarca" />
            <input type="button" id="btnComprobarVIN" value="Comprobar VIN" class="button" onclick="checkvin();" />
        </p>
        <div id="infoVIN">
        </div>
        <p>
            <input type="button" id="btnOK" value="Aceptar" class="button" onclick="checkvinok();" />
            <input type="button" id="btnSalir" value="Salir" class="button" />
        </p>
    </div>
</asp:Content>
