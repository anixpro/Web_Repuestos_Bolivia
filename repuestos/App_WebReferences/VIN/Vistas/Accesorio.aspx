<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Accesorio.aspx.cs" Inherits="Vistas_Accesorio" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script src="../js/galleria-1.2.5.min.js" type="text/javascript"></script>
    <script src="../css/myGalleriaTheme/galleria.classic.js" type="text/javascript"></script>
    <script src="../js/jquery-ui-1.8.14.custom.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        function pageLoad(sender, args) {
            $('.lightbox2').lightBox();
            $('#divDetalleAccesorio').dialog({ autoOpen: false });
            $('#divCarroCompra').dialog({ autoOpen: false });
            $('#idFomrularioEdicionAccesorio').dialog({ autoOpen: false });

            //Galleria.loadTheme('../css/myGalleriaTheme/galleria.classic.js');
            $('#imagenes').galleria({
                width: 500,
                height: 300,
                lightbox: true
            });

            // Detalle de accesorio
            $('#btnCancelarDetalleAccesorio').click(function () {
                $('#divDetalleAccesorio').dialog('close');
            });
            $('.imgLupa').click(function () {
                $('#divDetalleAccesorio').dialog('open');

                // Se trae detalle de accesorio
                var idRepuesto = $(this).parent().parent().parent().find('span').text();
                var idMarca = $('#lblSelectedMarca').text();
                var request = $.ajax({
                    url: "ajax/AccesorioDetalleAjax.aspx",
                    type: "POST",
                    data: { id: idRepuesto, marca: idMarca },
                    dataType: "html"
                });

                request.done(function (msg) {
                    $("#divDetalleAccesorioAjax").html(msg);
                });

                request.fail(function (jqXHR, textStatus) {
                    $("#msjesError").html("Error: " + textStatus);
                });
            });
            // Fin detalle accesorio

            // Carro de compra
            $('#imgCanasta').click(function () {
                $('#divCarroCompra').dialog('open');
            });
            $('#btnCancelarCarroCompra').click(function () {
                $('#divCarroCompra').dialog('close');
            });
            // fin carro de compra
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:Label ID="lblSesion" CssClass="invi" Text="" runat="server"/>
            <asp:Label ID="lblSelectedNode" CssClass="invi" Text="" runat="server"/>
            <asp:Label ID="lblSelectedMarcaModelo" CssClass="invi" Text="" runat="server"/>
            <asp:Label ID="lblSelectedDepth" CssClass="invi" Text="" runat="server"/>
            <asp:Label ID="lblSelectedMarca" CssClass="invi" Text="" runat="server" ClientIDMode="Static"/>
            <asp:Label ID="lblCurrentPage" CssClass="invi" Text="0" runat="server"/>
            <div class="titulo">
                <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:skbergeConnectionString %>" 
                    SelectCommand="SELECT [marca], [codigo], [descripcion], [cantidad], [precioC], [totalC], [precioL], [totalL] FROM [carro] WHERE ([idSession] = @idSession)">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="lblSesion" Name="idSession" 
                            PropertyName="Text" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
                Catálogo de Accesorios SKBergé
            </div>
            <!-- Acá se despliegan los mensajes de error -->
            <div id="msjesError" runat="server" clientidmode="Static">
            </div>
            <div class="costadoIzquierdo" style="overflow:auto; height:300px;"> 
                <asp:TreeView ID="TreeViewAcc" runat="server" ExpandDepth="0" ForeColor="#333333" Font-Size="11px">
                </asp:TreeView>
            </div>
            <div class="costadoDerecho" style="height:550px;">
                <div id="divAccesorioFirstTime" runat="server">
                    <b>Bienvenido al catálogo de accesorios</b>
                    <div id="divConAccesorios" runat="server">
                        <p>
                            Acá se muestran automóviles equipados con los mejores accesorios que puedes solicitar en SKBergé.
                        </p>
                        <p>
                            Te presentamos algunos
                        </p>
                        <div style="margin-left:auto;margin-right:auto;width:502px">
                            <div id="imagenes">
                                <asp:Repeater ID="rptrEquipados" runat="server">
                                    <ItemTemplate>
                                        <img alt="Imagen" src="<%# Eval("html") %>" />
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>
                    </div>
                    <div id="divSinAccesorios" runat="server">
                        <center>
                            <p>
                                <b>** Aun no hay autos equipados cargados **</b>
                            </p>
                            <p>
                                ¡Exijalos al administrador!
                            </p>
                        </center>
                    </div>
                    <center>
                        <asp:Button ID="btnAdminEquipados" runat="server" 
                            CssClass="button" Text="Agregar auto equipado"
                            PostBackUrl="~/Vistas/AdminEquipados.aspx" />
                    </center>
                </div>
                <div id="divCatalogoRepuestos" runat="server" clientidmode="Static">
                    <div class="btnPaginacionAccesorio" id="divPaginacionAccesorios" runat="server" clientidmode="Static">
                        <asp:button id="cmdPrev" runat="server" text=" << " OnClick="cmdPrev_Click"></asp:button>
                        <asp:Label ID="lblPaginaActual" runat="server" Text="1"></asp:Label>/
                        <asp:Label ID="lblPaginaTotal" runat="server" Text="1"></asp:Label>
                        <asp:button id="cmdNext" runat="server" text=" >> " OnClick="cmdNext_Click"></asp:button>
                    </div>
                    <asp:Repeater ID="RepAcce" runat="server" onitemcommand="EditarRegistro">
                        <ItemTemplate>
                            <div class="divItemAccesorio">
                                <table>
                                    <tr>
                                    <%
                                        if(int.Parse(Session["permisos"].ToString())==1)
                                        {
                                            %>
                                            <td>
                                                <asp:Label ID='Label2' runat='server' Text='<%# Eval("Codigo") %>' Font-Bold='true'
                                                ForeColor='#000000' Font-Size='14px'></asp:Label>
                                            </td>
                                            <td>
                                                <asp:LinkButton ID='LinkButton1' CommandArgument='<%# Eval("idAccesorio") %>' runat='server' CommandName="eliminar" OnClientClick="javascript:return(confirm('¿Seguro que desea eliminar?'))">
                                                    <img alt="Eliminar" src='../img/gtk-no.png' width='15px' height='15px' />
                                                </asp:LinkButton>
                                            </td>
                                            <td>
                                                <asp:LinkButton ID='LinkButton2' CommandArgument='<%# Eval("idAccesorio") %>' runat='server' CommandName="editar">
                                                    <img alt="Editar" src='../img/edit.png' width='15px' height='15px' />
                                                </asp:LinkButton>
                                            </td>
                                            <%
                                        } else {
                                                %>
                                                <td>
                                                    <asp:Label ID='Label4' runat='server' Text='<%# Eval("Codigo") %>' Font-Bold='true' ForeColor='#000000' Font-Size='14px'></asp:Label><hr />
                                                </td>
                                                <td colspan="2">
                                                    <a href="#"><img class="imgLupa" alt="Ver detalle" src="../img/lupa.png"/></a>
                                                </td>
                                                <%
                                        }
                                    %>
                                    </tr>
                                    <tr>
                                        <td colspan="3">
                                            <a href="<%# Eval("imagen") %>" class="lightbox2">
                                                <asp:ImageButton ImageUrl='<%# Eval("imagen") %>' ID="ImageButton1" runat="server" Width="120px" Height="105px" />
                                            </a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3"><asp:Label ID="Label3" runat="server" Text='<%# Eval("descripcion") %>'></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td colspan="3"><asp:Label ID="Label6" runat="server" Text='<%# Eval("nombreModelo") %>'></asp:Label></td>
                                    </tr>
                                </table>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
                <asp:Panel runat="server" ID="panelEdicionAccesorio"> 
                    <p>
                        <b>Editando accesorio</b>&nbsp;<asp:Label ID="lblMarcaAccesorio" Text="" runat="server" /> / <asp:Label ID="lblModeloAccesorio" Text="" runat="server" />
                    </p>
                    <asp:HiddenField ID="hddIdAccesorioEditando" runat="server"/>
                    <table>
                        <tr> 
							<td>Modelos: </td>
							<td>
								<div style="height:100px;overflow-x:hidden; overflow-y:scroll;">  
								  <asp:CheckBoxList ID="CheckBoxListModelos" runat="server" 
											RepeatDirection="Horizontal" ClientIDMode="Static" RepeatColumns="1">  
								  </asp:CheckBoxList>
								</div>  
							</td>
						</tr>	 
						
						<tr>
                            <td>Código: </td>
                            <td>
                                <asp:TextBox ID="txtCodigoAccesorio" runat="server" />
                                <asp:Label Visible="false" ID="lblValidaCodigoAcces" Text="Debe ingresar un código" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td>Descripción: </td>
                            <td>
                                <asp:TextBox ID="txtDescripcionAccesorio" runat="server" />
                                <asp:Label Visible="false" ID="lblValidaDescr" Text="Debe ingresar una descripción" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                Imágen actual:<br />
                                <asp:Image ID="imgRepuesto" runat="server" Height="150px" Width="150px" />
                            </td>
                        </tr>
                        <tr>
                            <td>Nueva imágen:</td>
                            <td>
                                <asp:FileUpload ID="fileImagenAccesorio" runat="server"/>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2"></td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Button ID="btnActualizarAccesorio" Text="Actualizar" runat="server" 
                                    onclick="btnActualizarAccesorio_Click" />
                            </td>
                            <td>
                                <asp:Button ID="btnCancelar" Text="Cancelar" runat="server" 
                                    ClientIDMode="Static" onclick="btnCancelar_Click" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </div>

            <!-- Agregar link  -->


            <div style="width: 200px; height: 59px;">
                    <center style="width: 161px">
                        <span style="font-size:11.0pt;font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;;
mso-fareast-font-family:Calibri;mso-fareast-theme-font:minor-latin;mso-bidi-font-family:
&quot;Times New Roman&quot;;mso-ansi-language:ES-CL;mso-fareast-language:ES-CL;
mso-bidi-language:AR-SA"><strong>Descarga los Catálogos de Accesorios AQUÍ</strong></span></center>
                    
			                <div class='slides_container_accesesorios'>
				                <a href='../doc/contenidoAccesorios/LinkAccesorios/Accesorios.rar' target='_blank'>
                                    <img src='../doc/contenidoAccesorios/LinkAccesorios/Accesorios_Link.jpg' width='161px' height='50px' alt='Slide 1'/>
                                </a>
                                
			                </div>
                             <center>
                    <asp:Button ID="btnEditarAccesorio" runat="server" 
                        Text = "Editar contenido" CssClass="button"
                        PostBackUrl="~/Vistas/EditorLinkAccesorios.aspx" UseSubmitBehavior="False" />
                </center>
		                </div>
	              
         

                 <!-- Fin Agregar link  -->


            <asp:Panel runat="server" ID="panelCanasta" Visible="false">
                Ver carrito <a href="#"><img id="imgCanasta" alt="Ver detalle" src="../img/canasta.png"/></a>
            </asp:Panel>
            <div id="divDetalleAccesorio">
                <center>
                    <div id="divDetalleAccesorioAjax">
                        <img alt="Cargando..." src="../img/lightbox-ico-loading.gif" />
                    </div>
                    <p>
                        <input id="btnCancelarDetalleAccesorio" type="button" value="Cerrar"/>
                    </p>
                </center>
            </div>
            <div id="divCarroCompra">
                <h3>
                    Carro de compra
                </h3>
                <asp:GridView ID="GridView1" runat="server" CellPadding="4" 
                    DataSourceID="SqlDataSource1" ForeColor="#333333" GridLines="None" 
                    EmptyDataText="No hay nada en el carro" AutoGenerateColumns="False">
                    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                    <Columns>
                        <asp:BoundField DataField="marca" HeaderText="marca" SortExpression="marca" />
                        <asp:BoundField DataField="codigo" HeaderText="codigo" 
                            SortExpression="codigo" />
                        <asp:BoundField DataField="descripcion" HeaderText="descripcion" 
                            SortExpression="descripcion" />
                        <asp:BoundField DataField="cantidad" HeaderText="cantidad" 
                            SortExpression="cantidad" />
                        <asp:BoundField DataField="precioC" HeaderText="precioC" 
                            SortExpression="precioC" />
                        <asp:BoundField DataField="totalC" HeaderText="totalC" 
                            SortExpression="totalC" />
                        <asp:BoundField DataField="precioL" HeaderText="precioL" 
                            SortExpression="precioL" />
                        <asp:BoundField DataField="totalL" HeaderText="totalL" 
                            SortExpression="totalL" />
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
                <center>
                    <input id="btnCotizar" type="button" value="Cotizar" />
                    <input id="btnCancelarCarroCompra" type="button" value="Cancelar" />
                </center>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnActualizarAccesorio" />
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
