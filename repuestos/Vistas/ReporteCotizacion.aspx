<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
CodeFile="ReporteCotizacion.aspx.cs" Inherits="Vistas_ReporteCotizacion" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit.HTMLEditor"
    TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../css/jqueryUItheme/jquery.ui.all.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-ui-1.8.14.custom.min.js" type="text/javascript"></script>
    <script type="text/javascript">
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
            border: 5px solid white;
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
    <asp:ToolkitScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="False">
    </asp:ToolkitScriptManager>
    <asp:Panel ID="Panel1" runat="server" Visible="false">
        <asp:HiddenField ID="hdPrueba" runat="server" />
    </asp:Panel>
    <div id="Panel_Procesando">
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ConnectionStrings:skbergeConnectionString%>">
        </asp:SqlDataSource>
        <div class="Titulo" id="titEdiSoli">
            Exportación de Reporte Cotización
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
                                        <asp:TextBox ID="txtnumsoli" Width="150px" runat="server" MaxLength="25" ClientIDMode="Static"
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
                                        <asp:Button ID="btnBuscarSoli" runat="server" Text="Excel" class="button" OnClick="btnBuscarSoli_Click" />
                                        <asp:Button ID="btnVolver" runat="server" Text="Volver" class="button" Visible="false" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <%--   <div id="loadingdiv" class="loading" align="center" style="display: none;">
                Cargando...<br />
                <br />
                <img src="../img/loading.gif" alt="" />
            </div>--%>
        </div>
    </div>
</asp:Content>
