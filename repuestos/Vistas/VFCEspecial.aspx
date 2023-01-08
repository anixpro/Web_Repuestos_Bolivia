<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="VFCEspecial.aspx.cs" Inherits="Vistas_VFCEspecial" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script src="../js/jquery.numeric.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#txtCantidad").numeric({ decimal: false, negative: false }, function () { alert("Solo enteros positivos"); this.value = ""; this.focus(); });
   
//            if ($("#RadioButtonList1_0").attr("checked")) {
//                $("#trVIN").hide();
//                $("#trReserva").show();
//            }

//            if ($("#RadioButtonList1_1").attr("checked")) {
//                $("#trVIN").show();
//                $("#trReserva").hide();
//            }

//            $("#RadioButtonList1_0").click(function () {
//                $("#txtVin").attr("value", "");
//                $("#trVIN").hide();
//                $("#trReserva").show();
//            });

//            $("#RadioButtonList1_1").click(function () {
//                $("#trVIN").show();
//                $("#trReserva").hide();
//            });

            $("#btnVolver").click(function () {
                window.location.href = "buscarRepto2.aspx";
            });

            // Inicio: Probador VIN
            $("#btnComprobarVIN_previo").click(function () {
                $("#vfcTester").dialog("open");
                $("#inVin").val($("#txtVin").val());
            });

            $("#vfcTester").dialog({
                autoOpen: false,
                modal: true,
                resizable: false,
                width: 600,
                height: 400,
                title: 'Comprobar VIN',
                position: 'center'
            });

            /* $("#btnComprobarVIN").click(function () {
            $("#infoVIN").html("<marquee><b>Cargando datos de VFC</b></marquee>");
            var solicitudAjax = $.ajax({
            type: "GET",
            cache: false,
            url: "ConsultaVIN.aspx",
            data: { vin: $("#inVin").val(), marca : $("#txtMarca").val()  },
            dataType: "html"
            });

            solicitudAjax.done(
            function (data) {
            $("#infoVIN").html(data);
            }
            );

            solicitudAjax.fail(function (jqXHR, textStatus) {
            $("#infoVIN").html("Error! Favor contactar al administrador. Descripción del error del navegador: " + textStatus);
            });
            });*/
            $("#btnComprobarVIN").click(function () {
                $("#infoVIN").html("<img src='../img/lightbox-ico-loading.gif'><b>Comprobando ...</b>");
                if  ($("#inVin").val().length != 17) {
                    alert("Ingrese un código VIN de 17 caracteres");
                    $("#infoVIN").html("");
                    return false;
                }
                alert($("#MainContent_txtMarca").val());
                var checkVinAjax = $.ajax({
                    type: "GET",
                    cache: false,
                    url: "ValidaVin.aspx",
                    data: { vin: $("#inVin").val(), marca: $("#MainContent_txtMarca").val() },
                    dataType: "html"
                });

                var bool = "";
                checkVinAjax.done(
                    function (data) {
                        if (data == 'false') {
                            alert("El VIN no fue encontrado en SAP"); //alert("Ingrese un código VIN valido");
                            $("#infoVIN").html("");
                            return;
                        } else {

                            $("#infoVIN").html("<marquee><b>Cargando datos de VFC</b></marquee>");
                            var solicitudAjax = $.ajax({
                                type: "GET",
                                cache: false,
                                url: "ConsultaVIN.aspx",
                                data: { vin: $("#inVin").val(), marca: $("#txtMarca").val() },
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

            });

            $("#btnSalir").click(function () {
                $("#infoVIN").html("");
                $("#vfcTester").dialog("close");
            });
            // Fin: Probador VIN
        });


    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<%--    <div class="titulo">
        Solicitud especial de VFC
    </div>--%>
    <div class="titulo">
        Formulario de solicitud de VFC 
    </div>
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <div class="contenedor">
        <p>
            Complete los datos solicitados para generar una pedido de reserva para la búsqueda realizada.<br />
            Al finalizar, haga clic en <b>Aceptar</b>
        </p>
        <table>
            <thead>
                <tr>
                    <th>Parámetro</th>
                    <th>valor</th>
                </tr>
            </thead>
            <tbody>
<%--                <tr>
                    <!--<td>Tipo de reserva</td>-->
                    <td>
                        <asp:RadioButtonList ClientIDMode="Static" ID="RadioButtonList1" Visible="true" 
                            runat="server" Font-Names="tahoma" Font-Size="X-Small" 
                            RepeatDirection="Horizontal">
                            <asp:ListItem Value="RESERVA">Reserva</asp:ListItem>
                            <asp:ListItem Value="NORMAL" Selected="True">VFC</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>--%>
                <tr id="trReserva">
                    <td colspan="2"><b>Favor incluir el VIN en el box Detalle/Referencia.</b>
                     </td>
                </tr>

<%--                <tr id="trVIN">
                    <td>VIN</td>
                    <td>
                        <asp:TextBox ID="txtVin" runat="server" ClientIDMode="Static"></asp:TextBox>&nbsp;
                        <input type="button" id="btnComprobarVIN_previo" value="Probar VIN" class="button" />
                    </td>
                </tr>--%>
              
                <tr>
                    <td>Marca</td>
                    <td>
                        <asp:TextBox ID="txtMarca" runat="server" ControlToValidate="txtMarca"></asp:TextBox>
                        <asp:RequiredFieldValidator ControlToValidate="txtMarca" ID="rfvMarca" runat="server" ErrorMessage="Debe ingresar una marca"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>Código repuesto</td>
                    <td><asp:TextBox ID="txtCodigo" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Cantidad</td>
                    <td><asp:TextBox ID="txtCantidad" runat="server" ClientIDMode="Static"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Detalle /<br />Referencia</td>
                    <td><asp:TextBox ID="txtDetalle" runat="server" TextMode="MultiLine"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Creador</td>
                    <td><asp:TextBox ID="txtCreador" runat="server" Text="<%=creador%>" Enabled="False"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Nombre Usuario</td>
                    <td><asp:TextBox ID="txtNombreUsuario" runat="server" Text="<%=nombreUser%>" Enabled="False"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Concesionario</td>
                    <td><asp:TextBox ID="txtDealer" runat="server" Text="<%=dealer%>" Enabled="False"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Direccion</td>
                    <td>
                        <asp:TextBox ID="txtDirección" runat="server" Text="<%=direccion%>" Enabled="False"></asp:TextBox>
                        <asp:DropDownList ID="ddlSucursales" runat="server"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>Tipo de pedido</td>
                    <td>
                        <asp:DropDownList ID="comboTipoPed" runat="server">
                            <asp:ListItem Value="normal" Text="Normal"></asp:ListItem>
                            <asp:ListItem Value="garantia" Text="Garantía"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
            </tbody>
        </table>
        <asp:Panel ID="PanelBotonera" runat="server">
            <asp:Button ID="btnAceptar" runat="server" OnClientClick="this.disabled = true; this.value = 'Guardando...';" UseSubmitBehavior="false"  Text="Aceptar" onclick="btnAceptar_Click" CssClass="button"/>
            <input id="btnCancelar" class="button" type="button" value="Cancelar" onclick="javascript:if(confirm('¿Cancelar solicitud?')){window.location='buscarrepto2.aspx';}" />
        </asp:Panel>
        <br/>
        <input type="button" id="btnVolver" value="Volver" class="button"/>
    </div>
    <div id="vfcTester">
        <p>Ingrese el VIN que desee chequear</p>
        <p>
            <input type="text" id="inVin" />
            <input type="button" id="btnComprobarVIN" value="Comprobar VIN" class="button"/>
        </p>
        <div id="infoVIN" style="width:200px;"></div>
        <p>
            <input type="button" id="btnSalir" value="Salir" class="button"/>
        </p>
    </div>
</asp:Content>
