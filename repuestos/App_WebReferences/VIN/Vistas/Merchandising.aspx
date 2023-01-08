<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Merchandising.aspx.cs" Inherits="Vistas_Merchandising" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script src="../js/galleria-1.2.5.min.js" type="text/javascript"></script>
    <script src="../css/myGalleriaTheme/galleria.classic.js" type="text/javascript"></script>
    <script type="text/javascript">
        function pageLoad(sender, args) {
            $('.lightbox2').lightBox();

            //Galleria.loadTheme('../css/myGalleriaTheme/galleria.classic.js');
            $('#imagenes').galleria({
                width: 500,
                height: 300,
                lightbox: true
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="titulo">
        Merchandising
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
            <div class="costadoIzquierdo" style="overflow:auto; height:300px;"> 
                <asp:TreeView ID="TreeViewAcc" runat="server" ExpandDepth="0" ForeColor="#333333" Font-Size="11px">
                </asp:TreeView>
            </div>
            <div class="costadoDerecho" style="height:462px;">
                <div id="divCatalogoRepuestos" runat="server">
                    <asp:panel id="panelBienvenida" runat="server">
                        <p>
                            <b>Bienvenidos a la sección Merchandising</b>
                        </p>
                        <center>
                            <p>
                                Haga clic en alguna marca para ver merchandising
                            </p>
                        </center>
                    </asp:panel>
                    <div class="btnPaginacionAccesorio">
                        <asp:button id="cmdPrev" runat="server" text=" << " OnClick="cmdPrev_Click"></asp:button>
                        <asp:Label ID="lblPaginaActual" runat="server" Text="1"></asp:Label>
                        <asp:Label ID="lblSlash" runat="server" Text="/"></asp:Label>
                        <asp:Label ID="lblPaginaTotal" runat="server" Text="1"></asp:Label>
                        <asp:button id="cmdNext" runat="server" text=" >> " OnClick="cmdNext_Click"></asp:button>
                    </div>
                    <asp:Repeater ID="RepMerch" runat="server" onitemcommand="EditarRegistro">
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
                                            <asp:LinkButton ID='LinkButton1' CommandArgument='<%# Eval("idMerchandising") %>' runat='server'>
                                                <img alt="(X)" src='../img/gtk-no.png' width='15px' height='15px' />
                                            </asp:LinkButton>
                                        </td>
                                        <td>
                                            <asp:LinkButton ID='LinkButton2' CommandArgument='<%# Eval("idMerchandising") %>' runat='server' CommandName="editar">
                                                <img alt="Editar" src='../img/edit.png' width='15px' height='15px' />
                                            </asp:LinkButton>
                                        </td>
                                    <%
                                    } else {
                                    %>
                                        <td colspan='3'>
                                            <asp:Label ID='Label4' runat='server' Text='<%# Eval("Codigo") %>' Font-Bold='true' ForeColor='#000000' Font-Size='14px'></asp:Label><hr />
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
                                        <td colspan="3"><asp:Label ID="Label6" runat="server" Text='<%# Eval("nombreMarca") %>'></asp:Label></td>
                                    </tr>
                                </table>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
                <asp:Panel ID="panelEditaMerchandising" runat="server">
                    <asp:HiddenField ID="hddIdMerchandising" runat="server"/>
                    <asp:HiddenField ID="hddMarca" runat="server"/>
                    <asp:HiddenField ID="hddModelo" runat="server"/>
                    <table>
                        <tr>
                            <td>Código :</td>
                            <td>
                                <asp:Textbox ID="txtEditaCodigo" runat="server"></asp:Textbox>
                                <asp:Label Visible="false" ID="lblCodigo" Text="Ingrese un código" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td>Descripción :</td>
                            <td>
                                <asp:Textbox ID="txtEditaDescripcion" runat="server"></asp:Textbox>
                                <asp:Label Visible="false" ID="lblDescripcion" Text="Ingrese una descripcion" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                Imagen actual<br />
                                <asp:Image ID="imgImagen" Height="150px" Width="150px" runat="server"/>
                            </td>
                        </tr>
                        <tr>
                            <td>Nueva imagen:</td>
                            <td><asp:FileUpload ID="fileImagenMerchandising" runat="server"/></td>
                        </tr>
                        <tr>
                            <td colspan="2" align="center">
                                <asp:Button ID="btnEditarMerchandisingo" Text="Actualizar" runat="server" 
                                    onclick="btnEditarMerchandisingo_Click"/>
                                <asp:Button ID="btnCancelar" Text="Cancelar" runat="server" ClientIDMode="Static"
                                    onclick="btnCancelarEditarMerchandisingo_Click"/>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </div>
        </ContentTemplate>
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
