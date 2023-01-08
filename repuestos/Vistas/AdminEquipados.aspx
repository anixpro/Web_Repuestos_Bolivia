<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="AdminEquipados.aspx.cs" Inherits="Vistas_AdminEquipados" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script src="../js/galleria-1.2.5.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {

            $("#btnAgregarNuevaPromo").click(function () {
                return validaFormulario();
            });

            function obtenerExtencion() {
                var cadena = $('#subirImagen').val().split('.').pop().toLowerCase()
                mje_ext = "";
                if (cadena != 'jpg' && cadena != 'png') {
                    mje_ext = "Formato de imagen solo puede ser: jpg o png\n";
                    $('#subirImagen').val("");
                    return false;
                }
                else {
                    return true;
                }
            }
            function validaTxtTitulo() {
                var cajaTexto = $("#txtTitulo").val();
                mje_tlo = "";
                if (cajaTexto == "") {
                    mje_tlo = "Debe agregar un título\n";
                    $('#txtTitulo').val("");
                    return false;
                }
                else {
                    return true;
                }
            }

            function validaFormulario() {
                validaTxtTitulo();
                obtenerExtencion();
                mje = "";
                mje = mje_ext + mje_tlo;
                if (mje != "") { alert(mje); return false; }
                else { return true; }

            }

            $('.lightbox2').lightBox();
            $("#divPromosAgregar").hide();
            $("#divPromosVerTodas").hide();
            $(".verPromo").hide();
            Galleria.loadTheme('../css/myGalleriaTheme/galleria.classic.js');
            $('#imagenes').galleria({
                width: 570,
                height: 370,
                lightbox: true
            });
            $("#btnVerUltimas").click(function () {
                $(".verPromo").hide();
                $("#divPromosVerTodas").hide(1000);
                $("#divPromosAgregar").hide(1000);
                $("#divPromoFirstTime").show(1000);
            });
            $("#btnVerTodas").click(function () {
                $(".verPromo").hide();
                $("#divPromoFirstTime").hide(1000);
                $("#divPromosAgregar").hide(1000);
                $("#divPromosVerTodas").show(1000);
            });
            $("#btnAgregar").click(function () {
                $(".verPromo").hide();
                $("#divPromoFirstTime").hide(1000);
                $("#divPromosVerTodas").hide(1000);
                $("#divPromosAgregar").show(1000);
            });
            $(".verPromoDetalle").click(function (e) {
                e.preventDefault();
                $(".verPromo").hide();
                var url = $(this).parent().next().next().next().html();
                $("#visorPromo").attr("src", url);
                $(".lightbox2").attr("href", url);
                $(".verPromo").show(1000);
            });
            $("#selectAll").click(function () {
                if ($(this).attr("checked")) {
                    $("input[type=checkbox]").attr("checked", true);
                } else {
                    $("input[type=checkbox]").attr("checked", false);
                }
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="titulo">
        Galería de automóviles equipados
    </div>
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <div class="costadoIzquierdo" style="overflow:auto; height:194px;">
        <center>
        <br /><br />
        <input id="btnVerUltimas" type="button" value="Ver últimos" class="button" />
        <br /><br />
        <input id="btnVerTodas" type="button" value="Ver todos" class="button" />
        <br /><br />
        <input id="btnAgregar" type="button" value="Agregar" class="button" />
        </center>
    </div>
    <div class="costadoDerecho" style="height:424px;">
        <p>
            <b>Autos equipados</b>
        </p>
        <div id="divPromoFirstTime">
            <div id="divConPromos" runat="server">
                <b>Estas son los últimos autos equipados subidos por administradores</b>
                <div id="imagenes">
                    <img src="" id="imgPromo1" alt='Slide 1' runat="server"/>
                    <img src="" id="imgPromo2" alt='Slide 2' runat="server"/>
                    <img src="" id="imgPromo3" alt='Slide 3' runat="server"/>
                    <img src="" id="imgPromo4" alt='Slide 4' runat="server"/>
                </div>
            </div>
            <div id="divSinPromos" runat="server">
                <center>
                    <p>
                        <b>** No hay autos equipados **</b>
                    </p>
                </center>
            </div>
        </div>
        <div id="divPromosVerTodas" class="verPromos" style="height:373px;">
            Aqui se listan todos los autos equipados a mostrar en la galería
            <asp:GridView ID="GridView1" runat="server" AllowPaging="True" 
                AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="idContenido" 
                DataSourceID="SqlDataSource1" CellPadding="4" ForeColor="#333333" 
                GridLines="None" EmptyDataText="No hay Promociones"
                OnRowCommand="postbackGridview">
                <AlternatingRowStyle BackColor="White" />
                <Columns>
                    <asp:CommandField ShowSelectButton="True" SelectText="Ver" >
                        <ControlStyle CssClass="verPromoDetalle" />
                    </asp:CommandField>
                    <asp:BoundField DataField="idContenido" HeaderText="idContenido" 
                        InsertVisible="False" ReadOnly="True" SortExpression="idContenido" ControlStyle-CssClass="invi" ItemStyle-CssClass="invi" HeaderStyle-CssClass="invi">
                        <ControlStyle CssClass="invi" />
                        <HeaderStyle CssClass="invi"></HeaderStyle>
                        <ItemStyle CssClass="invi"></ItemStyle>
                    </asp:BoundField>
                    <asp:BoundField DataField="fecha" HeaderText="Fecha" SortExpression="fecha" />
                    <asp:BoundField DataField="html" HeaderText="html" SortExpression="html" >
                    <ControlStyle CssClass="invi" />
                    <HeaderStyle CssClass="invi" />
                    <ItemStyle CssClass="invi" />
                    </asp:BoundField>
                    <asp:BoundField DataField="titulo" HeaderText="Titulo" 
                        SortExpression="titulo" />
                    <asp:BoundField DataField="tipo" HeaderText="tipo" SortExpression="tipo" 
                        Visible="False" />
                    <asp:ButtonField ButtonType="Image" ImageUrl="~/img/cross.png" 
                        CommandName="elimina" Text="Eliminar" />
                </Columns>
                <EditRowStyle BackColor="#2461BF" />
                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                <RowStyle BackColor="#EFF3FB" />
                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                <SortedAscendingCellStyle BackColor="#F5F7FB" />
                <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                <SortedDescendingCellStyle BackColor="#E9EBEF" />
                <SortedDescendingHeaderStyle BackColor="#4870BE" />
            </asp:GridView>
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
                ConnectionString="<%$ ConnectionStrings:skbergeConnectionString %>" 
                SelectCommand="SELECT idContenido,cast(fecha as varchar) as fecha, html, titulo,tipo FROM [contenido] WHERE ([tipo] = @tipo)">
                <SelectParameters>
                    <asp:Parameter DefaultValue="199" Name="tipo" Type="Int32" />
                </SelectParameters>
            </asp:SqlDataSource>
        </div>
        <div class="verPromo">
            <a href="" class="lightbox2">
                <img id="visorPromo" alt="Imagen de la promocion" src="" width="270px" height="280px" />
            </a>
        </div>
        <div id="divPromosAgregar" class="nuevaPromo" style="height:270px">
            <table>
                <tbody>
                    <tr>
                        <td colspan="2" align="center">
                            <b>Agregar nuevo automóvil equipado</b>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Titulo:
                        </td>
                        <td>
                            <asp:TextBox MaxLength="70" ID="txtTitulo" ClientIDMode="Static" runat="server"></asp:TextBox><img src="../img/help.png" alt="Título de la promoción" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Imagen:
                        </td>
                        <td>
                            <asp:FileUpload ID="subirImagen" runat="server" ClientIDMode="Static" /><img src="../img/help.png" alt="Seleccione imágen" title="Seleccione una imágen"/>
                        </td>
                    </tr>
                    <!--
                    <tr>
                        <td colspan="2" align="center">
                            Marcas:
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center">
                            <input type="checkbox" id="selectAll"/>Des/Seleccionar todo
                            <asp:CheckBoxList ID="CheckBoxListMarcas" runat="server" 
                                RepeatDirection="Vertical" ClientIDMode="Static" RepeatColumns="4">  
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    -->
                    <tr>
                        <td colspan="2">
                            <asp:Button ID="btnAgregarNuevaPromo" runat="server" CssClass="button"  ClientIDMode="Static"
                                Text = "Agregar"
                                onclick="btnAgregarNuevaPromo_Click"/>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Label Text="" ID="lblMensaje" runat="server"></asp:Label>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</asp:Content>
