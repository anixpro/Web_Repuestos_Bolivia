<%@ Page Title="Editando contenido" Language="C#" Trace="false" ValidateRequest="false" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="EditorContenido.aspx.cs" Inherits="Vistas_EditorContenido" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script type="text/javascript" src="../tinymce/jscripts/tiny_mce/tiny_mce.js"></script>
    <script type="text/javascript">
        function pageLoad(sender, args) {
            tinyMCE.init({
                // General options
                language: "es",
                theme_advanced_path: false,
                mode: "textareas",
                theme: "advanced",
                plugins: "safari,pagebreak,style,layer,table,save,advhr,advimage,advlink,emotions,iespell,inlinepopups,insertdatetime,preview,media,searchreplace,print,contextmenu,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,xhtmlxtras,template",

                // Botones personalizados (JGED)
                theme_advanced_buttons1: "newdocument,undo,redo,preview,fullscreen,|,bold,italic,underline,strikethrough,|,justifyleft,justifycenter,justifyright,justifyfull,formatselect,fontselect,fontsizeselect",
                theme_advanced_buttons2: "cut,copy,paste,pastetext,pasteword,|,search,replace,|,bullist,numlist,|,outdent,indent,blockquote,|,link,unlink,anchor,image,cleanup,help,code,|,insertdate,inserttime,|,forecolor,backcolor",
                theme_advanced_buttons3: "tablecontrols,|,hr,removeformat,visualaid,|,sub,sup,|,charmap,emotions,iespell,media,|,print,|,ltr,rtl",

                theme_advanced_toolbar_location: "top",
                theme_advanced_toolbar_align: "left",
                theme_advanced_statusbar_location: "bottom",
                theme_advanced_resizing: false,

                // Example content CSS (should be your site CSS)
                content_css: "css/content.css",

                // Drop lists for link/image/media/template dialogs
                template_external_list_url: "lists/template_list.js",
                external_link_list_url: "lists/link_list.js",
                external_image_list_url: "lists/image_list.js",
                media_external_list_url: "lists/media_list.js",

                // Replace values for the template plugin
                template_replace_values: {
                    username: "Some User",
                    staffid: "991234"
                }
            });

            $("#btnCopy").click(function () {
                return obtenerExtencion();
            });

            function obtenerExtencion() {
                var cadena = $('#cargadorImagenes').val().split('.').pop().toLowerCase();
                if (cadena != 'jpeg' && cadena != 'jpg' && cadena != 'png' && cadena != 'mp3' && cadena != 'avi' && cadena != 'swf' && cadena != 'mpg' && cadena != 'mpeg') {
                    alert("Formato de imagen solo puede ser:jpg, png, mp3, avi, swf, mpg, mpeg");
                    $('#cargadorImagenes').val("");
                    return false;
                }
                else {
                    return true;
                }
            }

            $("#selectAll").click(function () {
                if ($(this).attr("checked")) {
                    $("input[type=checkbox][class!=chkNoticia]").attr("checked", true);
                } else {
                    $("input[type=checkbox][class!=chkNoticia]").attr("checked", false);
                }
            });
        }
	function back(){
		location.href ="CasaMatriz.aspx";
		//javascript:history.back() 
	}
    </script><!-- /TinyMCE -->
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
        <div class="titulo">
            Editor de contenidos
        </div>
        <!-- Acá se despliegan los mensajes de error -->
        <div id="msjesError" runat="server" clientidmode="Static"></div>
        <asp:Panel ID="panelEditor" runat="server">
        <b><u>Instrucciones</u></b>
        <ol>
            <li>Escriba la noticia en el editor utilizando las opciones que ofrece el procesador de texo, tales como
            <ul>
                <li><b>Negrita</b>, <i>cursiva</i> y <u>subrayado</u></li>
                <li>Títulos, subtítulos y tipos de fuente disponibles</li>
                <li>Sangrías, párrafos</li>
                <li>Tablas</li>
            </ul>
            </li>
            <li>Si necesita de subir imágenes, videos o música, haga clic en "examinar...", escoja el archivo y haga clic en "Subir archivo". Luego, en el editor de texto, haga clic en el ícono <img alt="Insertar imágen" src="../img/insertImage.png" /> y en el diálogo de la ruta de la imágen presione CTRL+V, o clic derecho y seleccionar opción "pegar"
            </li>
        </ol>
        <asp:HiddenField ID="hdNoticia" runat="server" ClientIDMode="Static"/>
        <div class="editorContainer">
            <asp:TextBox ID="txtContenido" Rows="15" Columns="90" TextMode="MultiLine" 
                runat="server" ClientIDMode="Static" />
        </div>
        <br />
	    <div id="InsertContenido" class="editorAdd">
            <asp:FileUpload ID="cargadorImagenes" runat="server" ClientIDMode="Static" />
            <asp:Button ID="btnCopy" runat="server"
                Text="Subir archivo" onclick="Click_CopiarRuta" ClientIDMode="Static" />
            <img alt="Subir Archivo" title="Debe navegar y seleccionar un archivo de imágen, video o audio" src="../img/help.png" />
	        <table id="tblTitulo" runat="server">
                <tr>
                    <td><asp:Label ID="lblTitulo" runat="server" Text="Titulo"></asp:Label></td>
                    <td><asp:TextBox MaxLength="50" ID="txtTitulo" runat="server"></asp:TextBox>
                        <asp:Label ID="lblValidaTitulo"
                            runat="server" Text="Debe agregar un titulo"
                            ForeColor="Black" BackColor="Red" ClientIDMode="Static"></asp:Label>
                    </td>
                </tr>                   
            </table>
            <table>
                <tr>
                    <td style="font-family: Arial, Helvetica, sans-serif; font-size: 11px; color: #1E1E1E">
                        <div id="divListaMarcas" runat="server">
                            <input type="checkbox" id="selectAll"/>Des/Seleccionar todas las marcas
                                <asp:CheckBoxList ID="CheckBoxListMarcas" runat="server" 
                                RepeatDirection="Horizontal" ClientIDMode="Static"
                                CssClass="chkbxsMarcas">
                                </asp:CheckBoxList>
                        </div>
                    </td>
                </tr>
                <tr id="trNoticiaImportante" runat="server">
                    <td>
                        <input type="checkbox" class="chkNoticia" id="chkNoticiaImportante" runat="server" /> <b>Esta noticia es importante</b>
                        <img alt="Ejemplo: '6892966-0'" title="Si marca como noticia importante, obligará a que los usuarios la lean" src="../img/help.png" />
                    </td>
                </tr>
                <tr id="trActualizandoNoticia" runat="server">
                    <td>
                        <asp:Label ID="infoNoticia" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
            </table>
            <asp:Button CssClass="button" ID="btnAgregar" runat="server" Text="Vista Previa" onclick="btnAgregar_Click1"/>
            &nbsp;&nbsp;
            <asp:Button CssClass="button" ID="btnCancelar" Text="Cancelar" runat="server"  OnClientClick="return back();"/>
        </div>
    </asp:Panel>
    <asp:Panel runat="server" ID="panelVistaPrevia" BorderStyle="Groove" Visible="false">
        <b><u>Vista previa de la noticia</u></b>
        <div runat="server" id="divVistaPrevia"></div>
        <center>
            <asp:Button ID="btnPreviaAcepta" Text="Publicar" runat="server" CssClass="button"
                onclick="btnPreviaAcepta_Click" />
                &nbsp;
            <asp:Button ID="btnPreviaCancela" Text="Cancelar" runat="server" CssClass="button"
                onclick="btnPreviaCancela_Click" />
        </center>
    </asp:Panel>
    <asp:Panel runat="server" ID="panelFin" Visible ="false">
        <center>
            <asp:Button ID="btnFin" runat="server" />
        </center>
    </asp:Panel>
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