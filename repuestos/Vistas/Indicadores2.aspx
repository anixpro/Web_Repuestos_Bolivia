<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Indicadores2.aspx.cs" Inherits="Vistas_Indicadores2" %>
<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script type="text/javascript">
        function pageLoad(sender, args) {
            $("#txtEstadisticaCodigo").keyup(function () {
                $("#txtEstadisticaCodigo").val($("#txtCodigo").val().toUpperCase());
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="titulo">Indicadores</div>
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <!-- Acá se despliegan los mensajes de error -->
            <div id="msjesError" runat="server" clientidmode="Static">
            </div>
            <asp:Label ID="lblNombreConcesionario" Text="" CssClass="invi" runat="server"></asp:Label>
            <div class="costadoIzquierdo" style="overflow:auto; height:300px;"> 
                Indicadores disponibles:
                <ul>
                    <li>
                        <asp:LinkButton Text="Nivel de compra" runat="server" ID="lnkBtnNivelDe" 
                            onclick="lnkBtnNivelDe_Click"></asp:LinkButton>
                    </li>
                    <li id="liEstadistica" runat="server">
                        <asp:LinkButton Text="Estadística de compra" runat="server" ID="lnkBtnEstadisticaDeCompra" 
                            onclick="lnkBtnEstadisticaDeCompra_Click"></asp:LinkButton>
                    </li>
                    <li>
                        <asp:LinkButton Text="Evolución de compra" runat="server" 
                            ID="lnkBtnEvolucionCompra" onclick="lnkBtnEvolucionCompra_Click"></asp:LinkButton>
                    </li>
                    <li id="liVtasFallidas" runat="server">
                        <asp:LinkButton Text="Ventas Fallidas" runat="server" ID="lnkBtnVentasFallidas" 
                            onclick="lnkBtnVentasFallidas_Click"></asp:LinkButton>
                    </li>
                    <li>
                        <asp:LinkButton Text="Metas" runat="server" ID="lnkBtnMetas" 
                            onclick="lnkBtnMetas_Click"></asp:LinkButton>
                    </li>
                    <li id="liCotiCompra" runat="server">
                        <asp:LinkButton Text="Cotización V/S Compra" runat="server" 
                            ID="lnkBtnCotizacionCompras" onclick="lnkBtnCotizacionCompras_Click"></asp:LinkButton>
                    </li>
                </ul>
            </div>
            <div class="costadoDerecho" style="height:720px;overflow:auto">
                <asp:Panel runat="server" ID="panelBienvenida">  
                    <p>
                        <b>Indicadores</b>
                    </p>
                    <p>
                        Favor seleccionar el indicador que se desee revisar
                    </p>
                </asp:Panel>
                <asp:Panel runat="server" ID="panelNivelDeCompra">  
                    <p>
                        <b>Nivel de compra</b>
                    </p>
                    <p>
                        Completar los filtros de información:
                    </p>
                    <table border="0" cellpadding="0" cellspacing="0">
                        <tr>
                            <td>
                                Concesionario:
                            </td>
                            <td>
                                <asp:DropDownList runat="server" ID="ddlNivelCompraConcesionarios" 
                                    DataSourceID="SqlDataSourceConcesionarios" DataTextField="nombreConcesionario" 
                                    DataValueField="idConcesionario">
                                    </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Marca:
                            </td>
                            <td>
                                <asp:DropDownList runat="server" ID="ddlNivelCompraNombreMarca" 
                                    DataSourceID="SqlDataSourceMarcas" DataTextField="nombreMarca" 
                                    DataValueField="idMarca"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Fecha Inicio:
                            </td>
                            <td>
                                <table>
                                    <tr>
                                        <td>Año :</td>
                                        <td>
                                            <asp:DropDownList ID="ddlNivelComprasAnioIncio" runat="server">
                                                <asp:ListItem Text="2011" Value="2011" Selected="True"/>
                                                <asp:ListItem Text="2012" Value="2012"/>
                                                <asp:ListItem Text="2013" Value="2013"/>
                                                <asp:ListItem Text="2014" Value="2014"/>
                                                <asp:ListItem Text="2015" Value="2015"/>
                                                <asp:ListItem Text="2016" Value="2016"/>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Mes :</td>
                                        <td>
                                            <asp:DropDownList ID="ddlNivelComprasMesInicio" runat="server">
                                                <asp:ListItem Text="Enero" Value="1" Selected="True"/>
                                                <asp:ListItem Text="Febrero" Value="2"/>
                                                <asp:ListItem Text="Marzo" Value="3"/>
                                                <asp:ListItem Text="Abril" Value="4"/>
                                                <asp:ListItem Text="Mayo" Value="5"/>
                                                <asp:ListItem Text="Junio" Value="6"/>
                                                <asp:ListItem Text="Julio" Value="7"/>
                                                <asp:ListItem Text="Agosto" Value="8"/>
                                                <asp:ListItem Text="Septiembre" Value="9"/>
                                                <asp:ListItem Text="Octubre" Value="10"/>
                                                <asp:ListItem Text="Noviembre" Value="11"/>
                                                <asp:ListItem Text="Diciembre" Value="12"/>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Fecha término:
                            </td>
                            <td>
                                <table>
                                    <tr>
                                        <td>Año :</td>
                                        <td>
                                            <asp:DropDownList ID="ddlNivelComprasAnioTermino" runat="server">
                                                <asp:ListItem Text="2011" Value="2011"/>
                                                <asp:ListItem Text="2012" Value="2012" Selected="True"/>
                                                <asp:ListItem Text="2013" Value="2013"/>
                                                <asp:ListItem Text="2014" Value="2014"/>
                                                <asp:ListItem Text="2015" Value="2015"/>
                                                <asp:ListItem Text="2016" Value="2016"/>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Mes :</td>
                                        <td>
                                            <asp:DropDownList ID="ddlNivelComprasMesTermino" runat="server">
                                                <asp:ListItem Text="Enero" Value="1"/>
                                                <asp:ListItem Text="Febrero" Value="2"/>
                                                <asp:ListItem Text="Marzo" Value="3"/>
                                                <asp:ListItem Text="Abril" Value="4"/>
                                                <asp:ListItem Text="Mayo" Value="5"/>
                                                <asp:ListItem Text="Junio" Value="6"/>
                                                <asp:ListItem Text="Julio" Value="7"/>
                                                <asp:ListItem Text="Agosto" Value="8"/>
                                                <asp:ListItem Text="Septiembre" Value="9"/>
                                                <asp:ListItem Text="Octubre" Value="10"/>
                                                <asp:ListItem Text="Noviembre" Value="11"/>
                                                <asp:ListItem Text="Diciembre" Value="12" Selected="True"/>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="center">
                                <asp:Button Text="Generar gráfica" runat="server" ID="btnNivelCompraGeneraGraf" 
                                    onclick="btnNivelCompraGeneraGraf_Click" CssClass="button"/>
                                    &nbsp;
                                <asp:Button Text="Descarga Excel" runat="server" ID="btnNivelCompraGeneraExcel" 
                                    onclick="btnNivelCompraGeneraExcel_Click" CssClass="button"/>
                            </td>
                        </tr>
                    </table>
                    <asp:Chart ID="ChartNivelDeCompra" runat="server" Width="600px" Height="400px">
                        <Series>
                            <asp:Series ChartArea="ChartArea1" ChartType="Column" Name="Metas"
                                XValueMember="mes1" YValueMembers="compras" Legend="Legend1">
                            </asp:Series>
                        </Series>
                        <ChartAreas>
                            <asp:ChartArea Name="ChartArea1">
                            </asp:ChartArea>
                        </ChartAreas>
                    </asp:Chart>
                </asp:Panel>

                <!-- SECCION ESTADISTICA DE COMPRA -->

                <asp:Panel runat="server" ID="panelEstadisticaDeCompra">
                    <p>
                        <b>Estadística de compra</b>
                    </p>
                    <p>
                        Completar los filtros. Activar haciendo clic en el checkbox
                    </p>
                    <table>
                        <tr>
                            <td>
                                <asp:CheckBox Text="" runat="server" 
                                    oncheckedchanged="chkEstadisticaFechas_CheckedChanged" 
                                    ID="chkEstadisticaFechas" Checked="True" AutoPostBack="true"/>
                            </td>
                            <td>Fecha Inicio :</td>
                            <td>
                                <table>
                                    <tr>
                                        <td>Año :</td>
                                        <td>
                                            <asp:DropDownList ID="ddlEstadisticaCompraAnioInicio" runat="server">
                                                <asp:ListItem Text="2011" Value="2011" Selected="True"/>
                                                <asp:ListItem Text="2012" Value="2012"/>
                                                <asp:ListItem Text="2013" Value="2013"/>
                                                <asp:ListItem Text="2014" Value="2014"/>
                                                <asp:ListItem Text="2015" Value="2015"/>
                                                <asp:ListItem Text="2016" Value="2016"/>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Mes :</td>
                                        <td>
                                            <asp:DropDownList ID="ddlEstadisticaCompraMesInicio" runat="server">
                                                <asp:ListItem Text="Enero" Value="1" Selected="True"/>
                                                <asp:ListItem Text="Febrero" Value="2"/>
                                                <asp:ListItem Text="Marzo" Value="3"/>
                                                <asp:ListItem Text="Abril" Value="4"/>
                                                <asp:ListItem Text="Mayo" Value="5"/>
                                                <asp:ListItem Text="Junio" Value="6"/>
                                                <asp:ListItem Text="Julio" Value="7"/>
                                                <asp:ListItem Text="Agosto" Value="8"/>
                                                <asp:ListItem Text="Septiembre" Value="9"/>
                                                <asp:ListItem Text="Octubre" Value="10"/>
                                                <asp:ListItem Text="Noviembre" Value="11"/>
                                                <asp:ListItem Text="Diciembre" Value="12"/>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td></td>
                            <td>Fecha Fin :</td>
                            <td>
                                <table>
                                    <tr>
                                        <td>Año :</td>
                                        <td>
                                            <asp:DropDownList ID="ddlEstadisticaCompraAnioFin" runat="server">
                                                <asp:ListItem Text="2011" Value="2011"/>
                                                <asp:ListItem Text="2012" Value="2012" Selected="True"/>
                                                <asp:ListItem Text="2013" Value="2013"/>
                                                <asp:ListItem Text="2014" Value="2014"/>
                                                <asp:ListItem Text="2015" Value="2015"/>
                                                <asp:ListItem Text="2016" Value="2016"/>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Mes :</td>
                                        <td>
                                            <asp:DropDownList ID="ddlEstadisticaCompraMesFin" runat="server">
                                                <asp:ListItem Text="Enero" Value="1"/>
                                                <asp:ListItem Text="Febrero" Value="2"/>
                                                <asp:ListItem Text="Marzo" Value="3"/>
                                                <asp:ListItem Text="Abril" Value="4"/>
                                                <asp:ListItem Text="Mayo" Value="5"/>
                                                <asp:ListItem Text="Junio" Value="6"/>
                                                <asp:ListItem Text="Julio" Value="7"/>
                                                <asp:ListItem Text="Agosto" Value="8"/>
                                                <asp:ListItem Text="Septiembre" Value="9"/>
                                                <asp:ListItem Text="Octubre" Value="10"/>
                                                <asp:ListItem Text="Noviembre" Value="11"/>
                                                <asp:ListItem Text="Diciembre" Value="12" Selected="True"/>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <asp:RadioButton ID="rbEstadisticaCompraBuscaPorConcesSucursal" 
                                Text="Buscar por concesionario/sucursal" GroupName="EstadisticaCompraPersona" 
                                runat="server" Checked="True" AutoPostBack="true"
                                oncheckedchanged="rbEstadisticaCompraBuscaPorConcesSucursal_CheckedChanged" />
                            </td>
                        </tr>
                        <tr>
                            <td></td>
                            <td>Concesionario :</td>
                            <td>
                                <asp:DropDownList runat="server" ID="ddlEstadisticaCompraConcesionario" 
                                    DataSourceID="SqlDataSourceEstadistcaConcesionarios" 
                                    DataTextField="nombreConcesionario" DataValueField="idConcesionario" AppendDataBoundItems="true"
                                    AutoPostBack="true" onselectedindexchanged="ddlEstadisticaCompraConcesionario_SelectedIndexChanged">
                                    <asp:ListItem Selected="True" Text="Seleccione un concesionario" Value=""></asp:ListItem>
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="SqlDataSourceEstadistcaConcesionarios" runat="server" 
                                    ConnectionString="<%$ ConnectionStrings:skbergeConnectionString %>" 
                                    SelectCommand="SELECT [nombreConcesionario], [idConcesionario] FROM [concesionario]">
                                </asp:SqlDataSource>
                            </td>
                        </tr>
                        <tr>
                            <td></td>
                            <td>Sucursal :</td>
                            <td>
                                <asp:DropDownList runat="server" ID="ddlEstadisticaCompraSucursal" 
                                    DataSourceID="SqlDataSourceEstasticaSucursales" 
                                    DataTextField="direccionSucursal" DataValueField="shipCode"
                                    AutoPostBack="true"
                                    onselectedindexchanged="ddlEstadisticaCompraSucursal_SelectedIndexChanged" 
                                    ondatabound="ddlEstadisticaCompraSucursal_DataBound">
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="SqlDataSourceEstasticaSucursales" runat="server" 
                                    ConnectionString="<%$ ConnectionStrings:skbergeConnectionString %>" 
                                    SelectCommand="SELECT [direccionSucursal], [shipCode] FROM [sucursal] WHERE ([nombreConcesionario] = @nombreConcesionario)">
                                    <SelectParameters>
                                        <asp:ControlParameter ControlID="lblNombreConcesionario" 
                                            Name="nombreConcesionario" PropertyName="Text" Type="String" />
                                    </SelectParameters>
                                </asp:SqlDataSource>
                            </td>
                        </tr>
                        <tr>
                            <td></td>
                            <td>Operario :</td>
                            <td>
                                <asp:DropDownList runat="server" ID="ddlEstadisticaCompraOperario" 
                                    DataSourceID="SqlDataSourceEstadisticaOperario" DataTextField="nombre" 
                                    DataValueField="rut" AutoPostBack="true"
                                    onselectedindexchanged="ddlEstadisticaCompraOperario_SelectedIndexChanged" 
                                    ondatabound="ddlEstadisticaCompraOperario_DataBound" >
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="SqlDataSourceEstadisticaOperario" runat="server" 
                                    ConnectionString="<%$ ConnectionStrings:skbergeConnectionString %>"                                     
                                    SelectCommand="
                                    SELECT [nombre], persona.rut
                                    FROM [persona] inner join personaPermisos on personaPermisos.rut=persona.rut
                                    WHERE ([shipCode] = @shipCode) and cargo=3">
                                    <SelectParameters>
                                        <asp:ControlParameter ControlID="ddlEstadisticaCompraSucursal" Name="shipCode" 
                                            PropertyName="SelectedValue" Type="String" />
                                    </SelectParameters>
                                </asp:SqlDataSource>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:RadioButton ID="rbEstadisticaCompraBuscaPorRut" Text="Buscar por RUT" 
                                    GroupName="EstadisticaCompraPersona" runat="server" AutoPostBack="true"
                                    oncheckedchanged="rbEstadisticaCompraBuscaPorRut_CheckedChanged" />
                            </td>
                            <td>Rut :</td>
                            <td>
                                <asp:TextBox runat="server" ID="txtEstadisticaRut" Enabled="False"/>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:CheckBox Text="" runat="server" ID="chkEstadisticaMarca" 
                                    oncheckedchanged="chkEstadisticaMarca_CheckedChanged" Checked="false"
                                    AutoPostBack="true"/>
                            </td>
                            <td>Marca :</td>
                            <td>
                                <asp:DropDownList runat="server" ID="ddlEstadisticaMarca" 
                                    DataSourceID="SqlDataSourceEstadisticaMarcas" DataTextField="nombreMarca" 
                                    DataValueField="idMarca" AutoPostBack="true" onselectedindexchanged="ddlEstadisticaMarca_SelectedIndexChanged"
                                    Enabled = "false"></asp:DropDownList>
                                <asp:SqlDataSource ID="SqlDataSourceEstadisticaMarcas" runat="server" 
                                    ConnectionString="<%$ ConnectionStrings:skbergeConnectionString %>" 
                                    SelectCommand="SELECT [idMarca], [nombreMarca] FROM [marca]">
                                </asp:SqlDataSource>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:CheckBox Text="" runat="server" ID="chkEstadisticaCodigo" AutoPostBack="true"
                                oncheckedchanged="chkEstadisticaCodigo_CheckedChanged" Checked="false"/>
                            </td>
                            <td>
                                Código :
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtEstadisticaCodigo" ClientIDMode="Static"/>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <asp:Button ID="btnEstadisticaContar" Text="Consultar cantidad resultados" 
                                    CssClass="button" runat="server" onclick="btnEstadisticaContar_Click"/>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <asp:Button Text="Generar Tabla" runat="server" id="btnEstadisticaGeneraData" 
                                onclick="btnEstadisticaGeneraData_Click" CssClass="button"/>
                                &nbsp;
                                <asp:Button Text="Descagar Datos en Excel" runat="server" CssClass="button"
                                id="btnEstadisticaCompraDescargarDatos" onclick="btnEstadisticaCompraDescargarDatos_Click"/>
                            </td>
                        </tr>
                    </table>
                    <asp:SqlDataSource ID="SqlDataSourceEstadisticaCompra" runat="server"></asp:SqlDataSource>
                    <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AllowSorting="True"
                    Caption="Resultado Búsqueda" EmptyDataText="No hay resultados">
                    </asp:GridView>
                </asp:Panel>
                <asp:Panel runat="server" ID="panelEvolucionCompra">
                    <p>
                        <b>Evolución de compra</b>
                    </p>
                    <p>
                        Favor, completar los filtros para generar gráfica y datos
                    </p>
                    <table>
                        <tr>
                            <td>Fecha Inicio :</td>
                            <td>
                                <table>
                                    <tr>
                                        <td>Año :</td>
                                        <td>
                                            <asp:DropDownList ID="ddlEvolucionAnioIncio" runat="server">
                                                <asp:ListItem Text="2011" Value="2011" Selected="True"/>
                                                <asp:ListItem Text="2012" Value="2012"/>
                                                <asp:ListItem Text="2013" Value="2013"/>
                                                <asp:ListItem Text="2014" Value="2014"/>
                                                <asp:ListItem Text="2015" Value="2015"/>
                                                <asp:ListItem Text="2016" Value="2016"/>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Mes :</td>
                                        <td>
                                            <asp:DropDownList ID="ddlEvolucionMesInicio" runat="server">
                                                <asp:ListItem Text="Enero" Value="1" Selected="True"/>
                                                <asp:ListItem Text="Febrero" Value="2"/>
                                                <asp:ListItem Text="Marzo" Value="3"/>
                                                <asp:ListItem Text="Abril" Value="4"/>
                                                <asp:ListItem Text="Mayo" Value="5"/>
                                                <asp:ListItem Text="Junio" Value="6"/>
                                                <asp:ListItem Text="Julio" Value="7"/>
                                                <asp:ListItem Text="Agosto" Value="8"/>
                                                <asp:ListItem Text="Septiembre" Value="9"/>
                                                <asp:ListItem Text="Octubre" Value="10"/>
                                                <asp:ListItem Text="Noviembre" Value="11"/>
                                                <asp:ListItem Text="Diciembre" Value="12"/>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>Fecha Fin :</td>
                            <td>
                                <table>
                                    <tr>
                                        <td>Año :</td>
                                        <td>
                                            <asp:DropDownList ID="ddlEvolucionAnioTermino" runat="server">
                                                <asp:ListItem Text="2011" Value="2011"/>
                                                <asp:ListItem Text="2012" Value="2012" Selected="True"/>
                                                <asp:ListItem Text="2013" Value="2013"/>
                                                <asp:ListItem Text="2014" Value="2014"/>
                                                <asp:ListItem Text="2015" Value="2015"/>
                                                <asp:ListItem Text="2016" Value="2016"/>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Mes :</td>
                                        <td>
                                            <asp:DropDownList ID="ddlEvolucionMesTermino" runat="server">
                                                <asp:ListItem Text="Enero" Value="1"/>
                                                <asp:ListItem Text="Febrero" Value="2"/>
                                                <asp:ListItem Text="Marzo" Value="3"/>
                                                <asp:ListItem Text="Abril" Value="4"/>
                                                <asp:ListItem Text="Mayo" Value="5"/>
                                                <asp:ListItem Text="Junio" Value="6"/>
                                                <asp:ListItem Text="Julio" Value="7"/>
                                                <asp:ListItem Text="Agosto" Value="8"/>
                                                <asp:ListItem Text="Septiembre" Value="9"/>
                                                <asp:ListItem Text="Octubre" Value="10"/>
                                                <asp:ListItem Text="Noviembre" Value="11"/>
                                                <asp:ListItem Text="Diciembre" Value="12" Selected="True"/>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Concesionario :
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlEvolucionConcesionario"
                                DataSourceID="SqlDataSourceConcesionarios" runat="server"
                                DataTextField="nombreConcesionario" DataValueField="idConcesionario" 
                                ondatabound="ddlEvolucionConcesionario_DataBound" onselectedindexchanged="ddlEvolucionConcesionario_SelectedIndexChanged"
                                AutoPostBack="true">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Marca :
                            </td>
                            <td>
                                <asp:CheckBoxList ID="chkbxlstMarcas" runat="server" RepeatColumns="4"></asp:CheckBoxList>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:Button Text="Generar Gráfica" runat="server" CssClass="button"
                                    ID="btnEvolucionGeneraGrafica" onclick="btnEvolucionGeneraGrafica_Click"/>
                                &nbsp;
                                <asp:Button Text="Generar Documento" runat="server" CssClass="button"
                                    ID="btnEvolucionGeneraExcel" onclick="btnEvolucionGeneraExcel_Click"/>
                            </td>
                        </tr>
                    </table>
                    <asp:Chart ID="ChartEvolucion" runat="server" Width="600px" Height="400px">
                        <Series>
                        </Series>
                        <ChartAreas>
                            <asp:ChartArea Name="ChartArea1">
                            </asp:ChartArea>
                        </ChartAreas>
                        <Legends>
                            <asp:Legend DockedToChartArea="ChartArea1" Name="Legend1">
                            </asp:Legend>
                        </Legends>
                    </asp:Chart>
                </asp:Panel>

                <!-- SECCION VENTAS FALLIDAS -->

                <asp:Panel runat="server" ID="panelVentasFallidas">
                    <p>
                        <b>Ventas Fallidas</b>
                    </p>
                    <p>
                        Favor, completar los filtros para generar gráfica y datos
                    </p>
                    <table border="0" cellpadding="0" cellspacing="0">
                        <tr>
                            <td>
                                Fecha Inicio:
                            </td>
                            <td>
                                <table>
                                    <tr>
                                        <td>Año :</td>
                                        <td>
                                            <asp:DropDownList ID="ddlFallidasAnioInicio" runat="server">
                                                <asp:ListItem Text="2011" Value="2011" Selected="True"/>
                                                <asp:ListItem Text="2012" Value="2012"/>
                                                <asp:ListItem Text="2013" Value="2013"/>
                                                <asp:ListItem Text="2014" Value="2014"/>
                                                <asp:ListItem Text="2015" Value="2015"/>
                                                <asp:ListItem Text="2016" Value="2016"/>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Mes :</td>
                                        <td>
                                            <asp:DropDownList ID="ddlFallidasMesInicio" runat="server">
                                                <asp:ListItem Text="Enero" Value="1" Selected="True"/>
                                                <asp:ListItem Text="Febrero" Value="2"/>
                                                <asp:ListItem Text="Marzo" Value="3"/>
                                                <asp:ListItem Text="Abril" Value="4"/>
                                                <asp:ListItem Text="Mayo" Value="5"/>
                                                <asp:ListItem Text="Junio" Value="6"/>
                                                <asp:ListItem Text="Julio" Value="7"/>
                                                <asp:ListItem Text="Agosto" Value="8"/>
                                                <asp:ListItem Text="Septiembre" Value="9"/>
                                                <asp:ListItem Text="Octubre" Value="10"/>
                                                <asp:ListItem Text="Noviembre" Value="11"/>
                                                <asp:ListItem Text="Diciembre" Value="12"/>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Fecha término:
                            </td>
                            <td>
                                <table>
                                    <tr>
                                        <td>Año :</td>
                                        <td>
                                            <asp:DropDownList ID="ddlFallidasAnioTermino" runat="server">
                                                <asp:ListItem Text="2011" Value="2011"/>
                                                <asp:ListItem Text="2012" Value="2012" Selected="True"/>
                                                <asp:ListItem Text="2013" Value="2013"/>
                                                <asp:ListItem Text="2014" Value="2014"/>
                                                <asp:ListItem Text="2015" Value="2015"/>
                                                <asp:ListItem Text="2016" Value="2016"/>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Mes :</td>
                                        <td>
                                            <asp:DropDownList ID="ddlFallidasMesTermino" runat="server">
                                                <asp:ListItem Text="Enero" Value="1"/>
                                                <asp:ListItem Text="Febrero" Value="2"/>
                                                <asp:ListItem Text="Marzo" Value="3"/>
                                                <asp:ListItem Text="Abril" Value="4"/>
                                                <asp:ListItem Text="Mayo" Value="5"/>
                                                <asp:ListItem Text="Junio" Value="6"/>
                                                <asp:ListItem Text="Julio" Value="7"/>
                                                <asp:ListItem Text="Agosto" Value="8"/>
                                                <asp:ListItem Text="Septiembre" Value="9"/>
                                                <asp:ListItem Text="Octubre" Value="10"/>
                                                <asp:ListItem Text="Noviembre" Value="11"/>
                                                <asp:ListItem Text="Diciembre" Value="12" Selected="True"/>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Marca :
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlFallidasMarcas" runat="server"
                                    DataSourceID="SqlDataSourceFallidasMarcas" DataTextField="nombreMarca" 
                                    DataValueField="abreviado" AppendDataBoundItems="true">
                                    <asp:ListItem Text="Todas las marcas" Value=""></asp:ListItem>
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="SqlDataSourceFallidasMarcas" runat="server" 
                                    ConnectionString="<%$ ConnectionStrings:skbergeConnectionString %>" 
                                    SelectCommand="SELECT [nombreMarca],[abreviado] FROM [marca]">
                                </asp:SqlDataSource>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Código :
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtFallidasCodigo" MaxLength="15"/>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:Button Text="Generar Excel" runat="server" ID="btnFallidasGenerarExcel" 
                                    onclick="btnFallidasGenerarExcel_Click" CssClass="button"/>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <!-- SECCION COTIZACION V/S COMPRAS -->
                <asp:Panel runat="server" ID="panelMetas">
                    <p>
                        <b>Metas</b>
                    </p>
                    <p>
                        Favor, completar los filtros para generar gráfica y datos
                    </p>
                    <table border="0" cellpadding="0" cellspacing="0">
                        <tr>
                            <td>
                                Concesionario:
                            </td>
                            <td>
                                <asp:DropDownList runat="server" ID="ddlMetasConcesionarios" 
                                    DataSourceID="SqlDataSourceConcesionarios" DataTextField="nombreConcesionario" 
                                    DataValueField="idConcesionario"></asp:DropDownList>
                                <asp:SqlDataSource ID="SqlDataSourceConcesionarios" runat="server" 
                                    ConnectionString="<%$ ConnectionStrings:skbergeConnectionString %>" 
                                    SelectCommand="SELECT [nombreConcesionario], [idConcesionario] FROM [concesionario]">
                                </asp:SqlDataSource>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Marca:
                            </td>
                            <td>
                                <asp:DropDownList runat="server" ID="ddlMetasMarca" 
                                    DataSourceID="SqlDataSourceMarcas" DataTextField="nombreMarca" 
                                    DataValueField="idMarca"></asp:DropDownList>
                                <asp:SqlDataSource ID="SqlDataSourceMarcas" runat="server" 
                                    ConnectionString="<%$ ConnectionStrings:skbergeConnectionString %>" 
                                    SelectCommand="SELECT [nombreMarca], [idMarca] FROM [marca]">
                                </asp:SqlDataSource>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Fecha Inicio:
                            </td>
                            <td>
                                <table>
                                    <tr>
                                        <td>Año :</td>
                                        <td>
                                            <asp:DropDownList ID="ddlMetasAnioInicio" runat="server">
                                                <asp:ListItem Text="2011" Value="2011" Selected="True"/>
                                                <asp:ListItem Text="2012" Value="2012"/>
                                                <asp:ListItem Text="2013" Value="2013"/>
                                                <asp:ListItem Text="2014" Value="2014"/>
                                                <asp:ListItem Text="2015" Value="2015"/>
                                                <asp:ListItem Text="2016" Value="2016"/>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Mes :</td>
                                        <td>
                                            <asp:DropDownList ID="ddlMetasMesInicio" runat="server">
                                                <asp:ListItem Text="Enero" Value="1" Selected="True"/>
                                                <asp:ListItem Text="Febrero" Value="2"/>
                                                <asp:ListItem Text="Marzo" Value="3"/>
                                                <asp:ListItem Text="Abril" Value="4"/>
                                                <asp:ListItem Text="Mayo" Value="5"/>
                                                <asp:ListItem Text="Junio" Value="6"/>
                                                <asp:ListItem Text="Julio" Value="7"/>
                                                <asp:ListItem Text="Agosto" Value="8"/>
                                                <asp:ListItem Text="Septiembre" Value="9"/>
                                                <asp:ListItem Text="Octubre" Value="10"/>
                                                <asp:ListItem Text="Noviembre" Value="11"/>
                                                <asp:ListItem Text="Diciembre" Value="12"/>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Fecha término:
                            </td>
                            <td>
                                <table>
                                    <tr>
                                        <td>Año :</td>
                                        <td>
                                            <asp:DropDownList ID="ddlMetasAnioFin" runat="server">
                                                <asp:ListItem Text="2011" Value="2011"/>
                                                <asp:ListItem Text="2012" Value="2012" Selected="True"/>
                                                <asp:ListItem Text="2013" Value="2013"/>
                                                <asp:ListItem Text="2014" Value="2014"/>
                                                <asp:ListItem Text="2015" Value="2015"/>
                                                <asp:ListItem Text="2016" Value="2016"/>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Mes :</td>
                                        <td>
                                            <asp:DropDownList ID="ddlMetasMesFin" runat="server">
                                                <asp:ListItem Text="Enero" Value="1"/>
                                                <asp:ListItem Text="Febrero" Value="2"/>
                                                <asp:ListItem Text="Marzo" Value="3"/>
                                                <asp:ListItem Text="Abril" Value="4"/>
                                                <asp:ListItem Text="Mayo" Value="5"/>
                                                <asp:ListItem Text="Junio" Value="6"/>
                                                <asp:ListItem Text="Julio" Value="7"/>
                                                <asp:ListItem Text="Agosto" Value="8"/>
                                                <asp:ListItem Text="Septiembre" Value="9"/>
                                                <asp:ListItem Text="Octubre" Value="10"/>
                                                <asp:ListItem Text="Noviembre" Value="11"/>
                                                <asp:ListItem Text="Diciembre" Value="12" Selected="True"/>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="center">
                                <asp:Button Text="Generar gráfica" runat="server" ID="btnGenerarMetas" 
                                    onclick="btnGenerarMetas_Click" CssClass="button"/>
                                    &nbsp;
                                <asp:Button Text="Descagar Datos en Excel" runat="server" CssClass="button"
                                    id="btnMetasDescarcagaExcel" onclick="btnMetasDescarcagaExcel_Click"/>
                            </td>
                        </tr>
                    </table>
                    <asp:Chart ID="ChartMetas" runat="server"  Width="600px" Height="400px"
                        AlternateText="Gráfico Metas">
                        <Series>
                            <asp:Series ChartArea="ChartArea1" ChartType="Column" Name="Metas" 
                                XValueMember="mes1" YValueMembers="metas" Legend="Legend1">
                            </asp:Series>
                            <asp:Series Name="Compras" ChartType="StackedColumn" XValueMember="mes1" 
                                YValueMembers="compras" Legend="Legend1">
                            </asp:Series>
                        </Series>
                        <ChartAreas>
                            <asp:ChartArea Name="ChartArea1">
                            </asp:ChartArea>
                        </ChartAreas>
                        <Legends>
                            <asp:Legend DockedToChartArea="ChartArea1" Name="Legend1">
                            </asp:Legend>
                        </Legends>
                    </asp:Chart>
                    <asp:SqlDataSource ID="SqlDSnivelDeCompra" runat="server" 
                        ConnectionString="<%$ ConnectionStrings:skbergeConnectionString %>"></asp:SqlDataSource>
                </asp:Panel>

                <!-- SECCION COTIZACION V/S COMPRAS -->

                <asp:Panel runat="server" ID="panelCotizacionesCompras">
                    <p>
                        <b>Cotizacion v/s compra</b>
                    </p>
                    <p>
                        Favor, completar los filtros para generar gráfica y datos
                    </p>
                    <table>
                        <tr>
                            <td>
                                Fecha Inicio:
                            </td>
                            <td>
                                <table>
                                    <tr>
                                        <td>Año :</td>
                                        <td>
                                            <asp:DropDownList ID="ddlCotiCompraAnioInicio" runat="server">
                                                <asp:ListItem Text="2011" Value="2011" Selected="True"/>
                                                <asp:ListItem Text="2012" Value="2012"/>
                                                <asp:ListItem Text="2013" Value="2013"/>
                                                <asp:ListItem Text="2014" Value="2014"/>
                                                <asp:ListItem Text="2015" Value="2015"/>
                                                <asp:ListItem Text="2016" Value="2016"/>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Mes :</td>
                                        <td>
                                            <asp:DropDownList ID="ddlCotiCompraMesInicio" runat="server">
                                                <asp:ListItem Text="Enero" Value="1" Selected="True"/>
                                                <asp:ListItem Text="Febrero" Value="2"/>
                                                <asp:ListItem Text="Marzo" Value="3"/>
                                                <asp:ListItem Text="Abril" Value="4"/>
                                                <asp:ListItem Text="Mayo" Value="5"/>
                                                <asp:ListItem Text="Junio" Value="6"/>
                                                <asp:ListItem Text="Julio" Value="7"/>
                                                <asp:ListItem Text="Agosto" Value="8"/>
                                                <asp:ListItem Text="Septiembre" Value="9"/>
                                                <asp:ListItem Text="Octubre" Value="10"/>
                                                <asp:ListItem Text="Noviembre" Value="11"/>
                                                <asp:ListItem Text="Diciembre" Value="12"/>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Fecha término:
                            </td>
                            <td>
                                <table>
                                    <tr>
                                        <td>Año :</td>
                                        <td>
                                            <asp:DropDownList ID="ddlCotiCompraAnioFin" runat="server">
                                                <asp:ListItem Text="2011" Value="2011"/>
                                                <asp:ListItem Text="2012" Value="2012" Selected="True"/>
                                                <asp:ListItem Text="2013" Value="2013"/>
                                                <asp:ListItem Text="2014" Value="2014"/>
                                                <asp:ListItem Text="2015" Value="2015"/>
                                                <asp:ListItem Text="2016" Value="2016"/>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Mes :</td>
                                        <td>
                                            <asp:DropDownList ID="ddlCotiCompraMesFin" runat="server">
                                                <asp:ListItem Text="Enero" Value="1"/>
                                                <asp:ListItem Text="Febrero" Value="2"/>
                                                <asp:ListItem Text="Marzo" Value="3"/>
                                                <asp:ListItem Text="Abril" Value="4"/>
                                                <asp:ListItem Text="Mayo" Value="5"/>
                                                <asp:ListItem Text="Junio" Value="6"/>
                                                <asp:ListItem Text="Julio" Value="7"/>
                                                <asp:ListItem Text="Agosto" Value="8"/>
                                                <asp:ListItem Text="Septiembre" Value="9"/>
                                                <asp:ListItem Text="Octubre" Value="10"/>
                                                <asp:ListItem Text="Noviembre" Value="11"/>
                                                <asp:ListItem Text="Diciembre" Value="12" Selected="True"/>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>Concesionario :</td>
                            <td>
                                <asp:DropDownList runat="server" ID="ddlCotiCompraConcesionario" 
                                    DataSourceID="SqlDataSourceCotiCompraConcesionario" 
                                    DataTextField="nombreConcesionario" DataValueField="numeroFactura"
                                    AutoPostBack="true" AppendDataBoundItems="true">
                                        <asp:ListItem Text="Todos los concesionarios" Value=""/>
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="SqlDataSourceCotiCompraConcesionario" runat="server" 
                                    ConnectionString="<%$ ConnectionStrings:skbergeConnectionString %>" 
                                    SelectCommand="SELECT [nombreConcesionario], [numeroFactura] FROM [concesionario]">
                                </asp:SqlDataSource>
                            </td>
                        </tr>
                        <tr>
                            <td>Sucursal :</td>
                            <td>
                                <asp:DropDownList runat="server" ID="ddlCotiCompraSucursal" 
                                    DataSourceID="SqlDataSourceCotiCompraSucursal" 
                                    DataTextField="direccionSucursal" DataValueField="shipCode"
                                    AutoPostBack="true" AppendDataBoundItems="true">
                                    <asp:ListItem Text="Todas las sucursales" Value=""/>
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="SqlDataSourceCotiCompraSucursal" runat="server" 
                                    ConnectionString="<%$ ConnectionStrings:skbergeConnectionString %>" 
                                    SelectCommand="SELECT [direccionSucursal], [shipCode] FROM [sucursal] WHERE ([numeroFactura] = @numeroFactura)">
                                    <SelectParameters>
                                        <asp:ControlParameter ControlID="ddlCotiCompraConcesionario" 
                                            Name="numeroFactura" PropertyName="SelectedValue" Type="String" />
                                    </SelectParameters>
                                </asp:SqlDataSource>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:CheckBox runat="server" ID="chkBxCotiCompraSoloCotizaciones" Text="Traer solo cotizaciones sin confirmar al generar Excel" Checked="false"/>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:Button Text="Generar Gráfica" runat="server" CssClass="button"
                                    ID="btnCotiCompraGenerarGrafica" onclick="btnCotiCompraGenerarGrafica_Click" />
                                &nbsp;
                                <asp:Button Text="Generar Excel" runat="server" ID="btnCotiCompraGeneraExcel" 
                                    onclick="btnCotiCompraGeneraExcel_Click" CssClass="button"/>
                            </td>
                        </tr>
                    </table>
                    <asp:Chart ID="ChartCotiCompra" runat="server"  Width="600px" Height="400px">
                        <Series>
                        </Series>
                        <ChartAreas>
                            <asp:ChartArea></asp:ChartArea>
                        </ChartAreas>
                    </asp:Chart>
                </asp:Panel>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnNivelCompraGeneraExcel" />
            <asp:PostBackTrigger ControlID="btnEstadisticaCompraDescargarDatos" />
            <asp:PostBackTrigger ControlID="btnMetasDescarcagaExcel" />
            <asp:PostBackTrigger ControlID="btnEvolucionGeneraExcel" />
            <asp:PostBackTrigger ControlID="btnFallidasGenerarExcel" />
            <asp:PostBackTrigger ControlID="btnCotiCompraGeneraExcel" />
        </Triggers>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UP1" runat="server" DisplayAfter="0" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <script type="text/javascript">
                document.write("<div class='UpdateProgressBackground'></div>");
            </script>
            <center>
                <div class="UpdateProgressContent">Cargando...</div>
            </center>
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>
