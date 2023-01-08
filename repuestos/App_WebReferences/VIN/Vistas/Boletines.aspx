<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Boletines.aspx.cs" Inherits="Vistas_Boletines" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit.HTMLEditor"
    TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="../css/jqueryUItheme/jquery.ui.all.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-ui-1.8.14.custom.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $(".link").click(function (a) {
                a.preventDefault();
                var id = $(this).parent().prev().prev().prev().prev().prev().html()
                $(this).parent().prev().html("Sí");
                $.ajax({
                    type: "GET",
                    cache: false,
                    url: "ConfirmarLecturaBoletin.aspx",
                    data: { id: id },
                    dataType: "html"
                });
                $("#ifrmPDF").attr("src", $(this).attr("href"));
                $("#muestraPdf").dialog('open');
            });

            $("#muestraPdf").dialog({
                autoOpen: false,
                modal: true,
                resizable: true,
                width: 622,
                heigth: 533,
                title: 'Boletín'
            });

            $("#divAgregaBoletin").hide();

            if(
                $("#lblValidaTitulo").length > 0 ||
                $("#lblValidaDescrip").length > 0 ||
                $("#lblValidaDoc").length > 0
            )
            {
                $("#divAgregaBoletin").show();
            }

            if ($("#btnSeccionAgregarBoletin").length > 0) {
                $("#btnSeccionAgregarBoletin").click(function (event) {
                    event.preventDefault();
                    $("#divAgregaBoletin").show();
                });
            }

            $("#btnAgregaBoletin").click(function () {
                return obtenerExtencion();
            });

            function obtenerExtencion() {
                var cadena = $('#subirDoc').val().split('.').pop().toLowerCase()
                if (cadena != 'pdf') {
                    alert("Formato de imagen solo puede ser: pdf");
                    $('#subirDoc').val("");
                    return false;
                }
                else {
                    return true;
                }
            }
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="titulo">
        Boletines
    </div>
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <div class="boletines">
        Ingrese vacío para listar todos
        <!-- Busqueda de boletin por fecha-->
        <table class="tablaBuscarBoletin">
            <tr>        
                <td>
                    Buscar boletín por fecha: 
                    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
                    </asp:ToolkitScriptManager>

                    <asp:ImageButton runat="Server" ID="ImageButton1" ImageUrl="~/img/calendarIcon.png" AlternateText="Click here to display calendar" />
                    <asp:TextBox MaxLength="40" ID="txtFechaBusqueda" runat="server" 
                        ClientIDMode="Static"></asp:TextBox>

                   <asp:CalendarExtender ID="BuscarBoleCal" runat="server"
                    TargetControlID="txtFechaBusqueda" PopupButtonID="ImageButton1" 
                        Format="yyyy-MM-dd"/>
                </td>
                <td>
                   <asp:ImageButton ID="btnBuscar" runat="server" ImageUrl="~/img/buscarBtn.png" 
                        onclick="btnBuscar_CLick"></asp:ImageButton>
                </td>
            </tr>
        </table>
        <!--Fin busqueda por fecha-->
        <p>
            <b>Lista de boletines emitidos</b>
        </p>
        <asp:GridView ID="grillaBoletines" runat="server" HeaderStyle-Font-Size="12px" 
            HeaderStyle-BackColor="#045FB4" HeaderStyle-ForeColor="White" AutoGenerateColumns="false"
            AllowPaging="true" EmptyDataText="No hay boletines"
            HorizontalAlign="Center" Width="597px">
            <Columns>
                <asp:BoundField FooterStyle-CssClass="invi" ItemStyle-CssClass="invi" HeaderStyle-CssClass="invi" HeaderStyle-Width="120px" HeaderText="ID" DataField="idBoletin" />
                <asp:BoundField HeaderStyle-Width="120px" HeaderText="Titulo" DataField="titulo" />
                <asp:BoundField HeaderText="Fecha" DataField="fecha" />
                <asp:BoundField HeaderStyle-Width="200px" HeaderText="Descripción" DataField="descripcion" />
                <asp:BoundField HeaderStyle-Width="50px" HeaderText="Leído" DataField="leido" />
                <asp:HyperLinkField target="_blank" HeaderStyle-Width="50px" HeaderText="Documento" DataTextField="documento" DataNavigateUrlFields="documento" DataNavigateUrlFormatString="{0}" datatextformatstring="Ver" ControlStyle-CssClass="link"/>
                <asp:CommandField SelectText="" ShowSelectButton="True" FooterStyle-HorizontalAlign="Center" ButtonType="Image" SelectImageUrl="../img/cross.png" />
            </Columns>
            <HeaderStyle BackColor="#045FB4" Font-Size="12px" ForeColor="White"></HeaderStyle>
       </asp:GridView>
        <br />
        <asp:Button ID="btnSeccionAgregarBoletin" ClientIDMode="Static" runat="server" Text="Agregar boletín" CssClass="button"/> 
        <div id="divAgregaBoletin" runat="server" clientidmode="Static">
            <center>
            <p>
                <b>Agregear boletin</b>
            </p>
            </center>
            <table class="tablaBuscarBoletin">
                <tr>
                    <td>Titulo: </td>
                    <td>
                        <asp:TextBox MaxLength="30" ID="txtTitulo" runat="server" ClientIDMode="Static"></asp:TextBox>
                        <asp:Label ClientIDMode="Static" ID="lblValidaTitulo" Visible="false" runat="server" Text="Debe agregar un titulo" ForeColor="Black" BackColor="Red" Font-Size="11px"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>Descripcion: </td>
                    <td>
                        <asp:TextBox MaxLength="200" ID="txtDescripcion" runat="server" 
                            TextMode="MultiLine" ClientIDMode="Static"></asp:TextBox>
                        <asp:Label ClientIDMode="Static" ID="lblValidaDescrip" Visible="false" runat="server" Text="Debe agregar una descripción" ForeColor="Black" BackColor="Red" Font-Size="11px"></asp:Label>
                    </td>
                </tr>        
                <tr>
                <td colspan="2">
                    <asp:FileUpload ID="subirDoc" runat="server" ClientIDMode="Static"  />
                    <asp:Label ClientIDMode="Static" ID="lblValidaDoc" runat="server" Visible="false" Text="Debe agregar un documento (.pdf)" ForeColor="Black" BackColor="Red" Font-Size="11px"></asp:Label>
                    </td>
                </tr>
                <tr>        
                    <td colspan="2" align="center">
                        <asp:Button ID="btnAgregaBoletin" runat="server" Text="Agregar" CssClass="button"
                            ClientIDMode="Static" onclick="btnAgregaBoletin_Click" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="muestraPdf" class="ui-widget">
        <iframe id="ifrmPDF" src="" style="width:600px; height:500px;" frameborder="0"></iframe>
    </div>
</asp:Content>