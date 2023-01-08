<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="BancoRepuestos.aspx.cs" Inherits="Vistas_BancoRepuestos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script type="text/javascript">
        function pageLoad(sender, args) {
            $('.lightbox2').lightBox();

            $("#txtCodRepuesto").keyup(function () {
                $("#txtCodRepuesto").val($("#txtCodRepuesto").val().toUpperCase());
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="titulo">
        Banco de repuestos
    </div>
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
        <asp:Label ID="lblSelectedMarcaModelo" CssClass="invi" Text="" runat="server"/>
        <asp:Label ID="lblSelectedDepth" CssClass="invi" Text="" runat="server"/>
        <asp:Label ID="lblCurrentPage" CssClass="invi" Text="0" runat="server"/>
        <!-- Acá se despliegan los mensajes de error -->
        <div id="msjesError" runat="server" clientidmode="Static"></div>
        <div class="costadoIzquierdo" style="overflow:auto; height:500px;"> 
            <asp:TreeView ID="TreeViewAcc" runat="server" ExpandDepth="0" ForeColor="#333333" Font-Size="11px">
            </asp:TreeView>
            <asp:ImageButton runat="server" ID="agregarRepuesto" onclick="agregarRepuesto_Click" ImageUrl="~/img/add.png"/>&nbsp;Agregar Repuesto al Banco<br />
            <asp:ImageButton runat="server" ID="verMisRepuestos" onclick="verMisRepuestos_Click" ImageUrl="~/img/misReptos.png"/>&nbsp;Ver repuestos de <asp:Label Text="" ID="lblNombreConcesionario" runat="server" />
            <br />
            <asp:DropDownList runat="server" ID="ddlMisMarcas" onselectedindexchanged="ddlMisMarcas_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList><br />
            <asp:DropDownList Enabled="false" runat="server" ID="ddlMisModelos"></asp:DropDownList>
        </div>
        <div class="costadoDerecho" style="height:680px;overflow:auto;">
        <div id="divCatalogoRepuestos" runat="server">
            <asp:panel id="panelBienvenida" runat="server">
                <center>
                    <p>
                        <b>Bienvenidos a la sección Banco de Repuestos</b>
                    </p>
                    <p>
                        *** Sección en <i>marcha blanca</i> ***
                    </p>
                    <p>
                        En esta sección, operarios y cotizadores podrán subir repuestos que deseen vender a otros concesionarios. Para buscar respuestos haga clic en las marcas del panel de la derecha
                    </p>
                    <p>
                        SKBergé <b>NO se hará responsable</b> por los anuncios ni por los repuestos vendidos
                    </p>
                    <p>
                        Departamento de Repuestos<br />
                        SKBergé Automotriz
                    </p>
                </center>
            </asp:panel>
            <asp:Panel runat="server" ID="panelFiltraCodigo" CssClass="panelFiltraCodigoBancoRepto">
                <b><i>Opcional:</i></b>
                <br />
                Filtrar por código de repuesto : 
                <asp:TextBox runat="server" ID="txtCodRepuesto" ClientIDMode="Static"/>&nbsp;
                <asp:Button runat="server" ID="btnFiltrarCodRepuesto" Text="Filtrar" 
                    onclick="btnFiltrarCodRepuesto_Click"/>
            </asp:Panel>
            <div class="btnPaginacionAccesorio">
                <asp:button id="cmdPrev" runat="server" text=" << " OnClick="cmdPrev_Click"></asp:button>
                <asp:Label ID="lblPaginaActual" runat="server" Text="1"></asp:Label>
                <asp:Label ID="lblSlash" runat="server" Text="/"></asp:Label>
                <asp:Label ID="lblPaginaTotal" runat="server" Text="1"></asp:Label>
                <asp:button id="cmdNext" runat="server" text=" >> " OnClick="cmdNext_Click"></asp:button>
            </div>
            <asp:Repeater ID="RepMerch" runat="server" onitemcommand="EditarRegistro">
                <ItemTemplate>
                    <div class="divItemBancoReptos">
                        <table class="tablaBancoReptos">
                            <thead style="visibility:hidden">
                                <tr>
                                    <th style="width:70px"></th>
                                    <th style="width:70px"></th>
                                    <th style="width:200px"></th>
                                </tr>
                            </thead>
                            <tbody>
                            <tr>
                                <%
                                    if (int.Parse(Session["permisos"].ToString()) == 1)
                                    {
                                            %>
                                <td colspan='3'>
                                    <b>Código:</b>&nbsp;<asp:Label ID='Label4' runat='server' Text='<%# Eval("Codigo") %>' Font-Bold='true' ForeColor='#000000' Font-Size='14px'></asp:Label>
                                    <span style="float:right">
                                        <asp:LinkButton ID='LinkButton1' CommandArgument='<%# Eval("id") %>' runat='server' CommandName="eliminar" OnClientClick="javascript:return(confirm('¿Seguro que desea eliminar?'))">
                                            <img alt="Eliminar" src='../img/gtk-no.png' width='15px' height='15px' />
                                        </asp:LinkButton>
                                    </span>
                                    <hr />
                                </td>
                                <%
                                    }
                                    else
                                    {
                                  %>
                                <td colspan='3'>
                                    <b>Código:</b>&nbsp;<asp:Label ID='Label8' runat='server' Text='<%# Eval("Codigo") %>' Font-Bold='true' ForeColor='#000000' Font-Size='14px'></asp:Label><hr />
                                </td>
                                  <%
                                    }
                                    %>
                            </tr>
                            <tr>
                                <td rowspan="8" style="width:30px;">
                                    <a href="<%# Eval("imagen") %>" class="lightbox2">
                                        <asp:ImageButton ImageUrl='<%# Eval("imagen") %>' ID="ImageButton1" runat="server" Width="120px" Height="105px" AlternateText="Sin Imágen"/>
                                    </a>
                                </td>
                                <td><b>Descripción</b></td>
                                <td>
                                    <asp:Label ID="Label3" runat="server" Text='<%# Eval("descripcion") %>'>
                                    </asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td><b>Marca</b></td>
                                <td><asp:Label ID="Label6" runat="server" Text='<%# Eval("marca") %>'></asp:Label></td>
                            </tr>
                            <tr>
                                <td><b>Modelo</b></td>
                                <td><asp:Label ID="Label1" runat="server" Text='<%# Eval("modelo") %>'></asp:Label></td>
                            </tr>
                            <tr>
                                <td><b>Concesionario</b></td>
                                <td><asp:Label ID="Label5" runat="server" Text='<%# Eval("concesionario") %>'></asp:Label></td>
                            </tr>
                            <tr>
                                <td><b>Persona Contacto</b></td>
                                <td><asp:Label ID="Label7" runat="server" Text='<%# Eval("contacto") %>'></asp:Label></td>
                            </tr>
                            <tr>
                                <td><b>Telefono Contacto</b></td>
                                <td><%# Eval("codigoArea") %>-<%# Eval("telefono") %></td>
                            </tr>
                            <tr>
                                <td><b>Correo Contacto</b></td>
                                <td><%# Eval("correo") %></td>
                            </tr>
                            <tr>
                                <td><b>¿Es nuevo?</b></td>
                                <td><asp:Label ID="Label9" runat="server" Text='<%# determinaNuevo(Eval("esNuevo").ToString()) %>'></asp:Label></td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    &nbsp;
                                </td>
                            </tr>
                            </tbody>
                        </table>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <asp:Panel ID="panelEditaMerchandising" runat="server">
            <asp:HiddenField ID="hddIdRepto" runat="server"/>
            <asp:Label Text="" ID="lblMarcaModelo" runat="server" Font-Bold="true"/>
            <table>
                <tr>
                    <td>Código:</td>
                    <td>
                        <asp:Textbox ID="txtEditaCodigo" runat="server" MaxLength="40"></asp:Textbox>
                    </td>
                    <td>
                        Ejemplo: <i>YMC65TFS</i>
                    </td>
                </tr>
                <tr>
                    <td>Descripción:</td>
                    <td>
                        <asp:Textbox ID="txtEditaDescripcion" runat="server" MaxLength="200" TextMode="MultiLine"></asp:Textbox>
                    </td>
                    <td>
                        Ejemplo: <i>Spoiler delantero en excelente estado</i>
                    </td>
                </tr>
                <tr>
                    <td>Contacto:</td>
                    <td>
                        <asp:Textbox ID="txtEditaContacto" runat="server" MaxLength="200" TextMode="MultiLine"></asp:Textbox>
                    </td>
                    <td>
                        Ejemplo: <i>Daniel Gonzalez</i>
                    </td>
                </tr>
                <tr>
                    <td>Es nuevo:</td>
                    <td>
                        <asp:CheckBox Text="Sí, el repuesto es nuevo" ID="chkEditaReptoEsNuevo" runat="server" />
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>Telefono Contacto:</td>
                    <td>
                        <asp:TextBox ID="txtEditaCodigoArea" MaxLength="3" Columns="4" runat="server" />-
                        <asp:TextBox ID="txtEditaTelefono" MaxLength="30" runat="server" />
                    </td>
                    <td>
                        Ejemplo: <i>02 555555</i>
                    </td>
                </tr>
                <tr>
                    <td>Correo Contacto:</td>
                    <td>
                        <asp:TextBox ID="txtEditaCorreo" MaxLength="30" runat="server" />
                    </td>
                    <td>
                        Ejemplo: <i>correo@dominio.com</i>
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        Imagen actual:<br />
                        <asp:Image ID="imgImagen" Height="150px" Width="150px" runat="server"/>
                    </td>
                </tr>
                <tr>
                    <td>Nueva imagen:</td>
                    <td colspan="2"><asp:FileUpload ID="subeArchivo" runat="server"/></td>
                </tr>
                <tr>
                    <td colspan="3" align="center">
                        <asp:Button ID="btnEditarMerchandisingo" Text="Actualizar" runat="server" 
                            onclick="btnEditarMerchandisingo_Click"/>
                            &nbsp;
                        <asp:Button ID="btnCancelar" Text="Cancelar" runat="server" ClientIDMode="Static"
                            onclick="btnCancelarEditarMerchandisingo_Click"/>
                    </td>
                </tr>
            </table>
            </asp:Panel>
                <asp:Panel runat="server" ID="PanelAgregarRepuesto">
                    <p>
                        <b>Agregar un nuevo repuesto al banco</b>
                    </p>
                    <p>
                        Estimado operario<br /><br />
                        Para subir un nuevo repuesto, favor completar la información solicitada.
                        Los campos con asterisco (*) son obligatorios.<br />
                        Para mayor información, contacte al departamento de repuestos
                    </p>
                    <p>
                        Atte.<br />
                        El equipo de SKBergé
                    </p>
                    <fieldset>
                    <legend>Formulario ingreso de repuesto al banco</legend>
                        <table>
                            <tr>
                                <td><b>*</b>&nbsp;Marca:</td>
                                <td>
                                    <asp:DropDownList runat="server" ID="ddlMarcas" AutoPostBack="true"
                                        onselectedindexchanged="ddlMarcas_SelectedIndexChanged" >
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td><b>*</b>Modelo:</td>
                                <td>
                                    <asp:DropDownList runat="server" ID="ddlModelo" Enabled="False">
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td><b>*</b>&nbsp;Contacto:</td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtContacto" MaxLength="200" TextMode="MultiLine" />
                                </td>
                                <td>
                                    Ejemplo: <i>Juan Gonzalez</i>
                                </td>
                            </tr>
                            <tr>
                                <td><b>*</b>Descripción:</td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtDescripcion" MaxLength="200" TextMode="MultiLine" />
                                </td>
                                <td>
                                    Ejemplo: <i>Spoiler delantero en excelente estado</i>
                                </td>
                            </tr>
                            <tr>
                                <td><b>*</b>Cód. Repuesto:</td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtCodigoRepto" MaxLength="30"/>
                                </td>
                                <td>
                                    Ejemplo: <i>YMC65TFS</i>
                                </td>
                            </tr>
                            <tr>
                                <td>¿Es nuevo?</td>
                                <td>
                                    <asp:CheckBox Text="Sí, el repuesto es nuevo" runat="server" ID="chkbxEsNuevo"/>
                                </td>
                                <td>
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td><b>*</b>Telefono Contacto:</td>
                                <td>
                                    <asp:TextBox ID="txtCodigoArea" MaxLength="3" Columns="3" runat="server"/>-
                                    <asp:TextBox ID="txtTelefono" MaxLength="30" Columns="5" runat="server" />
                                </td>
                                <td>
                                    Ejemplo: <i>02-55670872</i>
                                </td>
                            </tr>
                            <tr>
                                <td><b>*</b>Correo Contacto:</td>
                                <td>
                                    <asp:TextBox ID="txtCorreo" MaxLength="30" runat="server" />
                                </td>
                                <td>
                                    Ejemplo: <i>correo@dominio.com</i>
                                </td>
                            </tr>
                            <tr>
                                <td>Imágen:</td>
                                <td colspan="2">
                                    <asp:FileUpload ID="fileImagenRepto" runat="server"/>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    <center>
                                        <asp:Button ID="btnAceptarAgregar" Text="Aceptar" runat="server" 
                                            onclick="btnAceptarAgregar_Click" />&nbsp;
                                        <asp:Button ID="btnCancelarCancelar" Text="Cancelar" runat="server" 
                                            onclick="btnCancelarCancelar_Click" />
                                    </center>
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                </asp:Panel>
                <asp:Panel runat="server" ID="panelMisRepuestos" Visible="false">
                    <asp:Label ID="lblMensajeMisReptos" Font-Bold="true" runat="server"></asp:Label>
                    <asp:Repeater ID="RepeaterMisReptos" runat="server" onitemcommand="EditarRegistro">
                        <ItemTemplate>
                            <div class="divItemBancoReptos">
                                <table class="tablaBancoReptos">
                                    <thead style="visibility:hidden">
                                        <tr>
                                            <th style="width:70px"></th>
                                            <th style="width:70px"></th>
                                            <th style="width:200px"></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                    <tr>
                                        <td>
                                            <asp:Label ID='Label2' runat='server' Text='<%# Eval("Codigo") %>' Font-Bold='true'
                                            ForeColor='#000000' Font-Size='14px'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:LinkButton ID='LinkButton1' CommandArgument='<%# Eval("id") %>' runat='server' CommandName="eliminar">
                                                <img alt="(X)" src='../img/gtk-no.png' width='15px' height='15px' />
                                            </asp:LinkButton>
                                        </td>
                                        <td>
                                            <asp:LinkButton ID='LinkButton2' CommandArgument='<%# Eval("id") %>' runat='server' CommandName="editar">
                                                <img alt="Editar" src='../img/edit.png' width='15px' height='15px' />
                                            </asp:LinkButton>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td rowspan="8">
                                            <a href="<%# Eval("imagen") %>" class="lightbox2">
                                                <asp:ImageButton ImageUrl='<%# Eval("imagen") %>' ID="ImageButton1" runat="server" Width="120px" Height="105px" AlternateText="Sin Imágen"/>
                                            </a>
                                        </td>
                                        <td><b>Descripción</b></td>
                                        <td>
                                            <asp:Label ID="Label3" runat="server" Text='<%# Eval("descripcion") %>'>
                                            </asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Marca</b></td>
                                        <td><asp:Label ID="Label6" runat="server" Text='<%# Eval("marca") %>'></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td><b>Modelo</b></td>
                                        <td><asp:Label ID="Label1" runat="server" Text='<%# Eval("modelo") %>'></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td><b>Concesionario</b></td>
                                        <td><asp:Label ID="Label5" runat="server" Text='<%# Eval("concesionario") %>'></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td><b>Contacto</b></td>
                                        <td><%# Eval("contacto") %></td>
                                    </tr>
                                    <tr>
                                        <td><b>Correo</b></td>
                                        <td><%# Eval("correo") %></td>
                                    </tr>
                                    <tr>
                                        <td><b>Telefono</b></td>
                                        <td><%# Eval("codigoArea") %>-<%# Eval("telefono") %></td>
                                    </tr>
                                    <tr>
                                        <td><b>¿Es nuevo?</b></td>
                                        <td><asp:Label ID="Label9" runat="server" Text='<%# determinaNuevo(Eval("esNuevo").ToString()) %>'></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td colspan="3">
                                            &nbsp;
                                        </td>
                                    </tr>
                                    </tbody>
                                </table>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </asp:Panel>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID = "btnAceptarAgregar" />
            <asp:PostBackTrigger ControlID = "btnEditarMerchandisingo" />
            <asp:PostBackTrigger ControlID = "agregarRepuesto" />
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

