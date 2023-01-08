<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Indicadores.aspx.cs" Inherits="Vistas_Indicadores" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
 <script language="javascript" type="text/javascript">

     function validaTxtExcel() {
         var cadena = $('#fldExcel').val().split('.').pop().toLowerCase();
         mje_exl = '';
         if (cadena != 'csv') 
         {
             mje_exl = 'Solo puede subir archivos .csv\n';
         }
         return (mje_exl);
     }

     function validaAno() {

         var cantidad=<%=abreviados.Length %>;
         var abreviados = new Array(cantidad);

         <%
            for(int j=0; j<abreviados.Length; j++) 
            {
         %>
            x=<%=j %>;
            abreviados[x]='<%=abreviados[j] %>';
            
         <%  
            }
         %>

         var nombreArchivo = $('#fldExcel').val().split('\\').pop();
         var cadena = nombreArchivo.split('.');
         if(!cadena.length)
         {
            alert('Archivo inválido');
            return;
         }
         
         var sinExtension = cadena[0];
         var soloMarca = sinExtension.split(' ');
         mje_ano ='';

         if (isNaN(soloMarca[1])) 
         {
             mje_ano = 'Debe indicar ano por ejemplo:MARCA 2011\n';

         }
         var algunaMarca=0;
         for (i=0;i<abreviados.length;i++)
         {
            if(soloMarca[0]!=abreviados[i])
            {
                algunaMarca++;
            }
            if(algunaMarca==abreviados.length)
            {                
                mje_ano = 'Solo marcas establecidas:\n'+abreviados;
            }
         }
         return (mje_ano);
     }

     function validaBlancos() {
         validaTxtExcel();
         validaAno();
         mensaje = '';
         mensaje = mje_exl + mje_ano;
         if (mensaje != '') {
             window.alert(mensaje);
             document.getElementById('fldExcel').value = '';
             return false;
         }
         else {
             return true;
         }
     }

</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="titulo">
        Indicadores&nbsp;
    </div>
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <div class="center">
        <asp:Table ID="tblDatos" runat="server" ClientIDMode="Static">
            <asp:TableRow ID="TableRow3" runat="server" ClientIDMode="Static">
                <asp:TableCell ColumnSpan="2">
                    Seleccione un archivo de ventas
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow1" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell2" runat="server" ColumnSpan="2"><asp:FileUpload id="fldExcel" ClientIDMode="Static" runat="server"/>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow2" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell3" runat="server" ClientIDMode="Static">
                </asp:TableCell>
                <asp:TableCell ID="TableCell4" runat="server" ClientIDMode="Static">
                    <asp:Button ID="btnAceptar" runat="server" ClientIDMode="Static" Text="Aceptar" OnClick="btnAceptar_Click" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow4" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell8" runat="server" ClientIDMode="Static" ColumnSpan="2">
                    <asp:HyperLink ID="hlkEjemplo" runat="server" ClientIDMode="Static" NavigateUrl="~/doc/excel/ejemploMetas.xlsx" >Descargar ejemplo</asp:HyperLink>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </div>
</asp:Content>