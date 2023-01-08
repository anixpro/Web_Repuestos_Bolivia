<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="SolicitudCotizacion.aspx.cs" Inherits="Vistas_SolicitudCotizacion" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit.HTMLEditor"
    TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../css/jqueryUItheme/jquery.ui.all.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-ui-1.8.14.custom.min.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        function allowOnlyNumber(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode
            if (charCode > 31 && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }

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
        function Mensaje() {

            var numero = document.getElementById("numerohidden");
            alert("Se ha creado la solicitud de cotizacion numero : " + numero + "");
        }

        function validarn(e) { // 1

            tecla = (document.all) ? e.keyCode : e.which; // 2
            if (tecla == 8) return true; // 3
            if (tecla == 9) return true; // 3
            if (tecla == 11) return true; // 3
            patron = /[A-Za-zñÑ'áéíóúÁÉÍÓÚàèìòùÀÈÌÒÙâêîôûÂÊÎÔÛÑñäëïöüÄËÏÖÜ\s\t]/; // 4


            te = String.fromCharCode(tecla); // 5
            return patron.test(te); // 6
        }


        function isNumberOrLetter(evt) {

            tecla = (document.all) ? e.keyCode : e.which;
            if (tecla == 8) return true;
            patron = /[A-Za-z]/;
            te = String.fromCharCode(tecla);
            return patron.test(te);
        }

        function validar(e) {
            tecla = (document.all) ? e.keyCode : e.which;
            if (tecla == 8) return true; //Tecla de retroceso (para poder borrar) 
            // dejar la línea de patron que se necesite y borrar el resto 
            patron = /[A-Za-z]/; // Solo acepta letras 
            //patron = /\d/; // Solo acepta números 
            //patron = /\w/; // Acepta números y letras 
            //patron = /\D/; // No acepta números 
            // 
            te = String.fromCharCode(tecla);
            return patron.test(te);
        } 

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ToolkitScriptManager ID="ScriptManager1" runat="server">
    </asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="updatepanel1" runat="server">
        <Triggers>
            <asp:PostBackTrigger ControlID="btnCreaPdf" />
            <asp:PostBackTrigger ControlID="btnAgregar" />
        </Triggers>
        <ContentTemplate>
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ConnectionStrings:skbergeConnectionString%>">
            </asp:SqlDataSource>
            <div class="mantenedorUsuariosIzquierda">
                <center>
                    <b><u>Cotización</u></b></center>
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
            <div class="titulo" id="titSolici">
                Solicitud de Cotizacion
            </div>
            <div id="mjsError" runat="server" clientidmode="Static" style="color: red">
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
                                            AutoPostBack="true" OnSelectedIndexChanged="ComboMarcas_SelectedIndexChanged">
                                        </asp:DropDownList>
                                        <br />
                                        <asp:LinkButton ID="btnNuevaSoli" Visible="false" runat="server" OnClientClick="javascript:return(confirm('Realizar una nueva Solicitud eliminará la Solicitud Actual. ¿Esta seguro?'))"
                                            OnClick="btnNuevaSoli_Click" TabIndex="1">Nueva Solicitud</asp:LinkButton>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Código Repuesto:
                                    </td>
                                    <td>
                                        <asp:TextBox MaxLength="25" ID="txtCodigo" runat="server" Width="120px" ClientIDMode="Static"
                                            AutoCompleteType="Disabled" TabIndex="2"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="descrepto" Text="Descripción del Repuesto" runat="server"></asp:Label>
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
                                        <asp:TextBox CssClass="txtCantidad" onkeypress="return allowOnlyNumber(event);" ID="txtCantidad" runat="server" MaxLength="3"
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
                                            <asp:ListItem Value="seguro">Compañía Seguro</asp:ListItem>
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
                                <tr>
                                    <td>
                                        Detalle:
                                    </td>
                                    <td>
                                        <asp:TextBox MaxLength="30" ID="txtDetalle" runat="server" Width="120px" ClientIDMode="Static"
                                            AutoCompleteType="Disabled" TabIndex="2" OnTextChanged="txtDetalle_TextChanged"
                                            AutoPostBack="true" onkeypress="return validar(event)"></asp:TextBox>
                                        <asp:Label ID="lblDetalle" runat="server" Text="Debe ingresar detalle" Visible="false"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Ingrese VIN:
                                    </td>
                                    <td>
                                        <asp:TextBox CssClass="txtVin" ID="txtVin" runat="server" MaxLength="17" Width="180px"
                                            ClientIDMode="Static" TabIndex="3">
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
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
                                        </asp:DropDownList>
                                    </td>
                                </tr>

                                <tr>
                                    <td>
                                        <asp:TextBox runat="server" ID="textProve" Enabled="false" CssClass="textEntry" Width="170px"
                                            ClientIDMode="Static"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:Label runat="server" ID="dias" Text="Dias"></asp:Label>&nbsp;&nbsp;
                                        <asp:TextBox runat="server" ID="txtdias" Enabled="false" CssClass="textEntry" Width="60px"
                                            ClientIDMode="Static"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label runat="server" ID="aviso" Visible="false"><strong>Los días estipulados en el plazo de importación, se consideran a partir de la fecha de generación del VFC</strong></asp:Label>
                                    </td>
                                    <td colspan="2" align="right">
                                        <asp:Button ID="btnAgregar" runat="server" ClientIDMode="Static" TabIndex="5" Height="30px"
                                            Text="Agregar" CssClass="button" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <div id="mensaje" runat="server" clientidmode="Static">
                            </div>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:Panel ID="pnSoliCoti" style="overflow-x:auto; overflow-y: auto; width:100%" runat="server" Visible="true">
                <p style="text-align: center; font-size: large; font-weight: bold">
                    Cotizar Repuesto sin Stock
                </p>
                <hr />
                <asp:GridView ID="gvSolicitud" runat="server" AutoGenerateColumns="false" ShowFooter="true"
                    BackColor="White" GridLines="Vertical" Font-Size="12px" widht="100%" CellPadding="3"
                    BorderColor="#999999" Caption="Cotizar Repuesto sin Stock" OnRowCommand="OnEliminarDelCArro">
                    <FooterStyle BackColor="SkyBlue" ForeColor="Black" />
                    <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                    <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                    <HeaderStyle BackColor="#000084" Font-Bold="true" ForeColor="White" Font-Names="Tahoma"
                        Font-Size="XX-Small" />
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
                        <asp:BoundField DataField="Descripcion" ItemStyle-HorizontalAlign="Center" HeaderText="Descripcion"
                            ReadOnly="true">
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Descripcion" FooterStyle-CssClass="invi" ItemStyle-CssClass="invi"
                            HeaderStyle-CssClass="invi" ItemStyle-HorizontalAlign="Center" HeaderText="Descripcion"
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
                        <asp:BoundField DataField="dias" ItemStyle-HorizontalAlign="Center" HeaderText="Dias"
                            ReadOnly="true">
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:BoundField>
                        <asp:BoundField DataField="dias" FooterStyle-CssClass="invi" ItemStyle-CssClass="invi"
                            HeaderStyle-CssClass="invi" ItemStyle-HorizontalAlign="Center" HeaderText="Dias"
                            ReadOnly="true">
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:BoundField>
                        <asp:BoundField DataField="motivo" ItemStyle-HorizontalAlign="Center" HeaderText="Motivo"
                            ReadOnly="true">
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:BoundField>
                        <asp:ButtonField ButtonType="Image" HeaderText="Eliminar" ItemStyle-HorizontalAlign="Center"
                            CommandName="quitar" Visible="true" ImageUrl="~/img/quitar.png" />
                        
                    </Columns>
                </asp:GridView>
                <br />
                <asp:Button runat="server" ID="btnCreaSolicitud" ClientIDMode="Static" Text="Crear Solicitud"
                    class="button" />
                <asp:Button runat="server" ID="btnCancelar" ClientIDMode="Static" Text="Cancelar Solicitud"
                    class="button" OnClick="btnCancelar_Click" />
                <asp:Button runat="server" ID="btnCreaPdf" Text="PDF" class="button" OnClick="btnCreaPdf_Click" />
                <br />
            </asp:Panel>
        </ContentTemplate>
        
    </asp:UpdatePanel>
</asp:Content>
