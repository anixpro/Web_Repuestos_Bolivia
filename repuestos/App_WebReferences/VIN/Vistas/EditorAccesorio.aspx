<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="EditorAccesorio.aspx.cs" Inherits="Vistas_EditorAccesorio" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script language="javascript" type="text/javascript">
    $(document).ready(function () {
		$("#modeloAsociados").multiselect({ open : true }).multiselectfilter();
        $("#btnAgregaAccesorio").click(function () {
            return obtenerExtencion();
        });
        function obtenerExtencion() {
            var cadena = $('#subirImagen').val().split('.').pop().toLowerCase()
            if (cadena != 'jpeg' && cadena != 'jpg' && cadena != 'png' && cadena != 'gif' && cadena != 'bmp') {
                alert("Formato de imagen solo puede ser:jpg, jpeg, png, gif o bmp");
                $('#subirImagen').val("");
                return false;
            }
            else {
				//alert($("#modeloAsociados").val());
                return true;
            }
        }
    });    //fin jquery
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="titulo">
        Agregar un nuevo accesorio
    </div>
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <div class="nuevoAccesorio" style="width:600px;">
        <table>
			<tr> 
                <td>Modelos: </td>
                <td>
					<div style="height:190px;overflow-x:hidden; overflow-y:scroll;">  
					  <asp:CheckBoxList ID="CheckBoxListModelos" runat="server" 
                                RepeatDirection="Horizontal" ClientIDMode="Static" RepeatColumns="1">  
                      </asp:CheckBoxList>
					</div>  
				</td>
            </tr>	
            <tr>
                <td>Código: </td>
                <td>
                    <asp:TextBox MaxLength="25" ID="txtCodigo" runat="server" ClientIDMode="Static"></asp:TextBox>
                    <asp:Label ID="lblValidaCodigo" runat="server" Text="Debe agregar el código del repuesto" ForeColor="Black" BackColor="Red"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>Descripción: </td>
                <td>
                    <asp:TextBox MaxLength="70" ID="txtDescripcion" runat="server" 
                        ClientIDMode="Static"></asp:TextBox>
                    <asp:Label ID="lblValidaDescrip" runat="server" Text="Debe agregar una descripcion" ForeColor="Black" BackColor="Red"></asp:Label>
                </td>
            </tr>
            <tr>
            <td colspan="2">
                <asp:FileUpload ID="subirImagen" runat="server" ClientIDMode="Static" /></td>
            </tr>
            <tr>        
                <td colspan="2" align="center">
                    <asp:Button ID="btnAgregaAccesorio" runat="server" Text="Agregar" 
                        onclick="btnAgregaAccesorio_Click" ClientIDMode="Static" />
                    &nbsp;
                    <asp:Button ID="btnRestaurar" runat="server" Text="Restaurar" 
                        onclick="btnRestaurar_Click" ClientIDMode="Static" />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>

