<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Seguimiento.aspx.cs" Inherits="Vistas_Seguimiento" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script src="../js/jquery-ui-1.8.14.custom.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            $("#tabs").tabs();
			
        });
		
		function showfilter(tipo){
		 $("#MainContent_tipoPedido").val(tipo);
		 
		}
			
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
      <div id="msjesError" runat="server" clientidmode="Static"></div> 
	<div class="titulo">Seguimiento</div>
	<table id="Table1">
		<tbody>
			<tr id="TableRow2">
				<td id="TableCell1">Pedido :
                </td>
				<td id="TableCell2">
					 <asp:TextBox ID="txtPedido" runat="server" ClientIDMode="Static"></asp:TextBox>
					 <input type="hidden" runat="server" name="tipoPedido" id="tipoPedido"  value="pedido" ></td><td>
					 <asp:Button ID="btnBuscar" runat="server" ClientIDMode="Static" Text="Buscar" OnClick="btnBuscar_Click" CssClass="button"/>
				</td>
			</tr>
		</tbody>
	</table>
        <div id="tabs">
	        <ul>
		        <li><a href="#tabs-1" onclick="showfilter('pedido');">Pedidos</a></li>
                <li><a href="#tabs-5" onclick="showfilter('');">Indicadores</a></li>
                <li><a href="#tabs-6" onclick="showfilter('');">Materiales por pedido</a></li>
	        </ul>
	        <div id="tabs-1">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <asp:GridView ID="GridView1" runat="server" AllowPaging="True" 
                            AutoGenerateColumns="False" CellPadding="4" DataKeyNames="ID_PEDIDO" 
                            DataSourceID="SqlDataSourcePedido" ForeColor="#333333" GridLines="None" 
                            AllowSorting="True" PageSize="13">
                            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                            <Columns>
                                <asp:BoundField DataField="E_VBELN" HeaderText="Pedido" 
                                    SortExpression="E_VBELN" />
                                <asp:BoundField DataField="I_BNDDT" HeaderText="Validez" 
                                    SortExpression="I_BNDDT" />
                                <asp:BoundField DataField="I_KUNNR" HeaderText="Cliente" 
                                    SortExpression="I_KUNNR" />
                                <asp:BoundField DataField="I_KUNNR2" HeaderText="Dest." 
                                    SortExpression="I_KUNNR2" />
                                <asp:BoundField DataField="I_VKORG" HeaderText="Org Vtas" 
                                    SortExpression="I_VKORG" />
                                <asp:BoundField DataField="I_VTWEG" HeaderText="Canal" 
                                    SortExpression="I_VTWEG" />
                                <asp:BoundField DataField="SOLICITADO_POR" HeaderText="Solicitado" 
                                    SortExpression="SOLICITADO_POR" />
                                <asp:BoundField DataField="FECHA_SOLICITUD" HeaderText="Fecha" 
                                    SortExpression="FECHA_SOLICITUD" />
                                <asp:BoundField DataField="TOTAL_NETO" HeaderText="Total" 
                                    SortExpression="TOTAL_NETO" />
                                <asp:BoundField DataField="E_VBELN_PEDIDO" HeaderText="Pedido" 
                                    SortExpression="E_VBELN_PEDIDO" />
                                <asp:BoundField DataField="I_AUART_PEDIDO" HeaderText="Tipo" 
                                    SortExpression="I_AUART_PEDIDO" />
                                <asp:BoundField DataField="I_LPRIO_PEDIDO" HeaderText="Prioridad" 
                                    SortExpression="I_LPRIO_PEDIDO" />
                            </Columns>
                            <EditRowStyle BackColor="#999999" />
                            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                            <SortedAscendingCellStyle BackColor="#E9E7E2" />
                            <SortedAscendingHeaderStyle BackColor="#506C8C" />
                            <SortedDescendingCellStyle BackColor="#FFFDF8" />
                            <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
                        </asp:GridView>
                        <asp:SqlDataSource ID="SqlDataSourcePedido" runat="server" 
                            ConnectionString="<%$ ConnectionStrings:skbergeConnectionString %>" 
                            ></asp:SqlDataSource>
                    </ContentTemplate>
                    <Triggers>
                    </Triggers>
                </asp:UpdatePanel>
                <asp:UpdateProgress ID="UpdateProgress1" runat="server" 
                    AssociatedUpdatePanelID="UpdatePanel1">
                    <ProgressTemplate>
                        Cargando...
                    </ProgressTemplate>
                </asp:UpdateProgress>
	        </div>
            <div id="tabs-5">
                <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                    <ContentTemplate>
                        <asp:GridView ID="GridView5" runat="server" AllowPaging="True" 
                            AllowSorting="True" AutoGenerateColumns="False" CellPadding="4" DataSourceID="SqlDataSourceIndicadores" 
                            ForeColor="#333333" GridLines="None" DataKeyNames="idIndicador" PageSize="23">
                            <AlternatingRowStyle BackColor="White" ForeColor="#284775"/>
                            <Columns>
                                <asp:BoundField DataField="nombreMarca" HeaderText="Marca" SortExpression="nombreMarca" />
                                <asp:BoundField DataField="nombreConcesionario" HeaderText="Marca" SortExpression="nombreConcesionario" />
                                <asp:BoundField DataField="ano" HeaderText="Año" SortExpression="ano" />
                                <asp:BoundField DataField="mes" HeaderText="Mes" SortExpression="mes" />
                                <asp:BoundField DataField="compras" HeaderText="Compras" 
                                    SortExpression="compras" />
                                <asp:BoundField DataField="metas" HeaderText="Metas" SortExpression="metas" />
                            </Columns>
                            <EditRowStyle BackColor="#999999" />
                            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                            <SortedAscendingCellStyle BackColor="#E9E7E2" />
                            <SortedAscendingHeaderStyle BackColor="#506C8C" />
                            <SortedDescendingCellStyle BackColor="#FFFDF8" />
                            <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
                        </asp:GridView>
                        <asp:SqlDataSource ID="SqlDataSourceIndicadores" runat="server" 
                            ConnectionString="<%$ ConnectionStrings:skbergeConnectionString %>" 
                            SelectCommand="select idIndicador,nombreMarca,nombreConcesionario,ano,mes,compras,metas from [indicador] inner join concesionarioMarca on indicador.idConcesionarioMarca = concesionarioMarca.idConcesionarioMarca order by ano desc"></asp:SqlDataSource>
                    </ContentTemplate>
                </asp:UpdatePanel>
	        </div>
            <div id="tabs-6">
                <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                    <ContentTemplate>
                        <asp:GridView ID="GridView6" runat="server" AutoGenerateColumns="False" 
                            CellPadding="4" DataSourceID="SqlDataSourceMateriales" ForeColor="#333333" 
                            GridLines="None" PageSize="13" AllowSorting="True" AllowPaging="true">
                            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                            <Columns>
                                <asp:BoundField DataField="marca" 
                                    HeaderText="Marca" SortExpression="marca" />
                                <asp:BoundField DataField="codigo" HeaderText="Codigo" 
                                    SortExpression="codigo" />
                                <asp:BoundField DataField="descripcion" HeaderText="Descripción" 
                                    SortExpression="descripcion" />
                                <asp:BoundField DataField="strGrupoMateriales" HeaderText="Grupo" 
                                    SortExpression="strGrupoMateriales" />
                                <asp:BoundField DataField="cantidad" HeaderText="Cantidad" 
                                    SortExpression="cantidad" />
                                <asp:BoundField DataField="stock" HeaderText="Stock" SortExpression="stock" />
                                <asp:BoundField DataField="valor" HeaderText="Valor" SortExpression="valor" />
                                <asp:BoundField DataField="total" HeaderText="Total" SortExpression="total" />
                            </Columns>
                            <EditRowStyle BackColor="#999999" />
                            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                            <SortedAscendingCellStyle BackColor="#E9E7E2" />
                            <SortedAscendingHeaderStyle BackColor="#506C8C" />
                            <SortedDescendingCellStyle BackColor="#FFFDF8" />
                            <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
                        </asp:GridView>
                        <asp:SqlDataSource ID="SqlDataSourceMateriales" runat="server" 
                            ConnectionString="<%$ ConnectionStrings:skbergeConnectionString %>" 
                            SelectCommand="SELECT * FROM [MATERIALES_PEDIDO] order by id_Pedido desc"></asp:SqlDataSource>
                    </ContentTemplate>
                </asp:UpdatePanel>
	        </div>
        </div>
</asp:Content>
