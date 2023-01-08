<%@ Page Title="Editor de Boletines" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="EditorBoletines.aspx.cs" Inherits="Vistas_editorBoletines" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
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

            $("#idDemoBol").dialog({
                autoOpen: false,
                modal: true,
                resizable: false,
                width: 670,
                heigth: 395,
                title: 'Ejemplo para crear boletín'
            });

            $("#demoBol").click(function (e) {
                e.preventDefault();
                $("#idDemoBol").dialog("open");
            });
        }); //fin jquery
</script>


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="titulo">
        Agregar un boletín
    </div>
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <div class="costadoIzquierdo">
        Aca puede agregar nuevos boletines informativos
    </div>
    <div class="costadoDerecho">
        <table>
            <tr>
                <td>Titulo: </td>
                <td>
                    <asp:TextBox MaxLength="30" ID="txtTitulo" runat="server" ClientIDMode="Static"></asp:TextBox>
                    <asp:Label ID="lblValidaTitulo" runat="server" Text="Debe agregar un titulo" ForeColor="Black" BackColor="Red" Font-Size="11px"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>Descripcion: </td>
                <td>
                    <asp:TextBox MaxLength="200" ID="txtDescripcion" runat="server" 
                        TextMode="MultiLine" ClientIDMode="Static"></asp:TextBox>
                    <asp:Label ID="lblValidaDescrip" runat="server" Text="Debe agregar una descripción" ForeColor="Black" BackColor="Red" Font-Size="11px"></asp:Label>
                </td>
            </tr>
        
            <tr>
            <td colspan="2">
                <asp:FileUpload ID="subirDoc" runat="server" ClientIDMode="Static"  />
                <asp:Label ID="lblValidaDoc" runat="server"  Text="Debe agregar un documento (.pdf, .doc)" ForeColor="Black" BackColor="Red" Font-Size="11px"></asp:Label>
                </td>
            </tr>
            <tr>        
            <td colspan="2" align="center">
                <asp:Label Visible="False" ID="lblRuta" runat="server" Text="Label" 
                    ClientIDMode="Static"></asp:Label>
                <asp:Button ID="btnAgregaBoletin" runat="server" Text="Agregar" 
                    onclick="AgregarBoletin_Click" ClientIDMode="Static" />
                    <a href="#" id="demoBol" 
                style="font-family: Arial, Helvetica, sans-serif; font-size: 10px; color: #003A75">Demo</a></td>
            </tr>
        </table>
        <div id="idDemoBol">
            <object style="height: 390px; width: 640px">
                <param name="movie" value="http://www.youtube.com/v/DmY9ayOwtIw?version=3"/>
                <param name="allowFullScreen" value="true"><param name="allowScriptAccess" value="always">
                <embed src="http://www.youtube.com/v/DmY9ayOwtIw?version=3" type="application/x-shockwave-flash" allowfullscreen="true" allowScriptAccess="always" width="640" height="360">
            </object>
        </div>
    </div>
</asp:Content>

