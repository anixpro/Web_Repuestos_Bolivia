<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="IndicadorCotizacion.aspx.cs" Inherits="Vistas_indicadorCotizacion" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
 <script language="javascript" type="text/javascript">

     $(document).ready(function () {

         muestraTipo();
         drawChartCodigo();

         //FUNCTION QUE CAMBIA LA 'K' A 'k' 
         $("#txtDvr").keyup(function () {
             //alert($("#txtDvr").val());
             if ($("#txtDvr").val() == 'K') {
                 $("#txtDvr").val('k');
             }
         });
         $("#rbtnCodigo").click(function () {
             muestraTipo();
         });
         $("#rbtnOperario").click(function () {
             muestraTipo();
         });
         $("#rbtnTodos").click(function () {
             muestraTipo();
         });
         $("#rbtnCodigoDestinatario").click(function () {
             muestraTipo();
         });
         rbtnCodigoDestinatario
         //SI APRIETO EL BOTON Y ADEMAS ESTÁ CHECKEADO EL BUSCAR POR RUT, VALIDO EL RUT
         $("#btnAceptar").click(function () {
             if ($("#rbtnOperario").is(":checked")) {
                 return validaCampos();
             } //SINO, VÁLIDO EL CÓDIGO
             else if ($("#rbtnCodigo").is(":checked")) {
                 validaTxtCodigo();
             }
         });

         function validaCampos() {
             validaTxtRut();
             mje = "";
             mje = mje_rut;
             if (mje != "") {
                 alert(mje_rut); return false;
             } else
             { return true; }
         }
         function muestraTipo() {
             if ($("#rbtnCodigo").is(":checked")) {
                 $("#rowCodigo").show();
                 $("#rowRut").hide();
                 $("#rowCodigoDespacho").hide();
             }
             else if ($("#rbtnOperario").is(":checked")) {

                 $("#rowCodigo").hide();
                 $("#rowRut").show();
                 $("#rowCodigoDespacho").hide();
             }
             else if ($("#rbtnTodos").is(":checked")) {
                 $('#rowCodigo').hide();
                 $('#rowRut').hide();
                 $("#rowCodigoDespacho").hide();
             }
             else if ($("#rbtnCodigoDestinatario").is(":checked")) {
                 $('#rowCodigo').hide();
                 $('#rowRut').hide();
                 $("#rowCodigoDespacho").show();
             }
         }

         function validaTxtCodigo()
         { }

         function validaTxtRut() {

             $("#lblRut").css("color", "#696969");
             cajaTexto = $("#txtRut").val();
             mje_rut = '';
             if (cajaTexto == "") {
                 mje_rut = 'debe llenar Rut\n';
                 $("#lblRut").css("color", "red");
                 $("#txtRut").val("");
             }
             else if (isNaN(cajaTexto)) {
                 mje_rut = 'solo numeros en Rut\n';
                 $("#lblRut").css("color", "red");
                 $("#txtRut").val("");
             }
             else {
                 var rut = $("#txtRut").val();
                 var largo = rut.length;
                 var i = 0;
                 var dv = $("#txtDvr").val();
                 var mult = 2;
                 var suma = 0;
                 largo--;
                 while (largo >= 0) {
                     suma = suma + (rut.charAt(largo) * mult);
                     if (mult > 6)
                         mult = 2;
                     else
                         mult++;
                     largo--;
                 }

                 var resto = suma % 11;
                 var digito = 11 - resto
                 if (digito == 10) {
                     digito = "k";
                 }
                 else if (digito == 11) {
                     digito = 0;
                 }
                 if (digito != dv) {
                     mje_rut = 'Rut inválido\n';
                     $("#lblRut").css("color", "red");
                     $("#txtRut").val("");
                 }
             }

             return (mje_rut);
         } //fin validaRut


     });           //fin jquery


     

</script>

 <!--Load the AJAX API-->
    <script type="text/javascript" src="https://www.google.com/jsapi"></script>
    <script type="text/javascript">

        // Load the Visualization API and the piechart package.
        google.load('visualization', '1', { 'packages': ['corechart'] });

        // Set a callback to run when the Google Visualization API is loaded.
        //google.setOnLoadCallback(drawChartCodigo);

        // Callback that creates and populates a data table, 
        // instantiates the pie chart, passes in the data and
        // draws it.
       function drawChartCodigo() {

            var data = new google.visualization.DataTable();
        data.addColumn('string', 'Tipo');
        data.addColumn('number', 'Cotizacion');
        data.addColumn('number', 'Compra');
        data.addRows(1);//este el largo de la cantidad columnas una fila columna contiene varias barras
        data.setValue(0, 0, 'Cantidad');
        data.setValue(0, 1, <%=_cotizacionVsCompra[1] %>);
        data.setValue(0, 2, <%=_cotizacionVsCompra[0] %>);
		
	

            var chart = new google.visualization.ColumnChart(document.getElementById('chart_div'));
            chart.draw(data, { width: 700, height: 240, title: 'Cotizacion vs compra $',
                hAxis: { title: 'Cotizacion vs Compra', titleTextStyle: { color: 'red'} }, vAxis: { title: 'Pesos', titleTextStyle: { color: 'red'} },
                colors: ['#DF0101', '#0404B4']
            });
            return false;
        }

        
        
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="titulo">
        Indicador Cotizacion vs Compras
    </div>
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <ul style="float:left;">
        <li><asp:HyperLink ID="hlkMetas" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/verIndicador.aspx">Metas</asp:HyperLink></li>
        <li><asp:HyperLink ID="hlkEstadistica" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/indicadorEstadistica.aspx">Estadistica</asp:HyperLink></li>
        <li><asp:HyperLink ID="hlkEvoluvion" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/indicadorEvolucion.aspx">Evolucion</asp:HyperLink></li>
        <li><asp:HyperLink ID="hlkNivel" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/indicadorNivel.aspx">Nivel</asp:HyperLink></li>
        <li id="liCotizacion" runat="server" ><asp:HyperLink ID="hlkCotizacion" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/indicadorCotizacion.aspx" Visible="false">Cotizacion</asp:HyperLink></li>
        <li id="liFallidas" runat="server" ><asp:HyperLink ID="hlkFallidas" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/indicadorFallidas.aspx" Visible="false">Fallidas</asp:HyperLink></li>
    </ul>
        <div class="centerGrande">
            <asp:Label ID="lblConcesionario" runat="server" ClientIDMode="Static"></asp:Label>

    <div class="avisoInsert">
    <asp:Label ID="lblAviso" runat="server" ClientIDMode="Static" ForeColor="#99CCFF" ></asp:Label>
    </div>

    <div style="margin-left:auto;margin-right:auto;width:700px;">
    <asp:RadioButton ID="rbtnCodigo" runat="server" ClientIDMode="Static" Text="Codigo" GroupName="tipo" Checked="true" />
    <asp:RadioButton ID="rbtnCodigoDestinatario" runat="server" ClientIDMode="Static" Text="Destinatario" GroupName="tipo"  />
    <asp:RadioButton ID="rbtnOperario" runat="server" ClientIDMode="Static" Text="Operario" GroupName="tipo"  />
    <asp:RadioButton ID="rbtnTodos" runat="server" ClientIDMode="Static" Text="Todos" GroupName="tipo"  />
    </div>

    <asp:Table ID="tblDatos" runat="server" ClientIDMode="Static">
                <asp:TableRow ID="TableRow4" runat="server" ClientIDMode="Static">
            <asp:TableCell ID="TableCell7" runat="server" ClientIDMode="Static"><asp:Label ID="lblMarca" runat="server" ClientIDMode="Static" Text="Marca"></asp:Label>
</asp:TableCell>

            <asp:TableCell ID="TableCell8" runat="server" ClientIDMode="Static"><asp:DropDownList ID="ddlMarca" runat="server" ClientIDMode="Static"></asp:DropDownList>
</asp:TableCell>
        </asp:TableRow>

<asp:TableRow ID="rowCodigo" runat="server" ClientIDMode="Static">
            <asp:TableCell ID="TableCell1" runat="server" ClientIDMode="Static"><asp:Label ID="lblCodigo" runat="server" ClientIDMode="Static" Text="Código"></asp:Label>
</asp:TableCell>

            <asp:TableCell ID="TableCell2" runat="server" ClientIDMode="Static"><asp:TextBox ID="txtCodigo" Width="80" runat="server" ClientIDMode="Static"></asp:TextBox>
</asp:TableCell>
        </asp:TableRow>

<asp:TableRow ID="rowRut" runat="server" ClientIDMode="Static" Style="display:none;">
            <asp:TableCell ID="TableCell5" runat="server" ClientIDMode="Static"><asp:Label ID="lblRut" runat="server" ClientIDMode="Static" Text="Rut operario"></asp:Label>
</asp:TableCell>

            <asp:TableCell ID="TableCell6" runat="server" ClientIDMode="Static"><asp:TextBox ID="txtRut" Width="80" runat="server" ClientIDMode="Static"></asp:TextBox><asp:Label ID="lblGuion" runat="server" ClientIDMode="Static" Text="-"></asp:Label><asp:TextBox ID="txtDvr" runat="server" Width="20" MaxLength="1" ClientIDMode="Static"></asp:TextBox>
</asp:TableCell>
        </asp:TableRow>

<asp:TableRow ID="rowCodigoDespacho" runat="server" ClientIDMode="Static" Style="display:none;">
            <asp:TableCell ID="TableCell13" runat="server" ClientIDMode="Static"><asp:Label ID="lblCodigoDespacho" runat="server" ClientIDMode="Static" Text="Código despacho"></asp:Label>
</asp:TableCell>

            <asp:TableCell ID="TableCell14" runat="server" ClientIDMode="Static"><asp:TextBox ID="txtCodigoDespacho" Width="80" runat="server" ClientIDMode="Static"></asp:TextBox>
</asp:TableCell>
        </asp:TableRow>
            
<asp:TableRow ID="rowMes" runat="server" ClientIDMode="Static">

            <asp:TableCell ID="TableCell9" runat="server" ClientIDMode="Static">
            <asp:Label ID="lblAño" runat="server" ClientIDMode="Static">Año</asp:Label>

           
            
</asp:TableCell>

            <asp:TableCell ID="TableCell10" runat="server" ClientIDMode="Static">

            

            <asp:DropDownList ID="ddlAno" runat="server" ClientIDMode="Static">
            </asp:DropDownList>
            
            
</asp:TableCell>
        </asp:TableRow>

        <asp:TableRow ID="TableRow5" runat="server" ClientIDMode="Static">

            <asp:TableCell ID="TableCell11" runat="server" ClientIDMode="Static">

            <asp:Label ID="lblMesDesde" runat="server" ClientIDMode="Static">Desde</asp:Label>

            <asp:DropDownList ID="ddlMesDesde" runat="server" ClientIDMode="Static">
            <asp:ListItem>Enero</asp:ListItem>
            <asp:ListItem>Febrero</asp:ListItem>
            <asp:ListItem>Marzo</asp:ListItem>
            <asp:ListItem>Abril</asp:ListItem>
            <asp:ListItem>Mayo</asp:ListItem>
            <asp:ListItem>junio</asp:ListItem>
            <asp:ListItem>Julio</asp:ListItem>
            <asp:ListItem>Agosto</asp:ListItem>
            <asp:ListItem>Septiembre</asp:ListItem>
            <asp:ListItem>Octubre</asp:ListItem>
            <asp:ListItem>Noviembre</asp:ListItem>
            <asp:ListItem>Diciembre</asp:ListItem>
            </asp:DropDownList>

            
</asp:TableCell>

            <asp:TableCell ID="TableCell12" runat="server" ClientIDMode="Static">
            
            <asp:Label ID="lblHasta" runat="server" ClientIDMode="Static">Hasta</asp:Label>

            <asp:DropDownList ID="ddlMesHasta" runat="server" ClientIDMode="Static">
            <asp:ListItem>Enero</asp:ListItem>
            <asp:ListItem>Febrero</asp:ListItem>
            <asp:ListItem>Marzo</asp:ListItem>
            <asp:ListItem>Abril</asp:ListItem>
            <asp:ListItem>Mayo</asp:ListItem>
            <asp:ListItem>junio</asp:ListItem>
            <asp:ListItem>Julio</asp:ListItem>
            <asp:ListItem>Agosto</asp:ListItem>
            <asp:ListItem>Septiembre</asp:ListItem>
            <asp:ListItem>Octubre</asp:ListItem>
            <asp:ListItem>Noviembre</asp:ListItem>
            <asp:ListItem>Diciembre</asp:ListItem>
            </asp:DropDownList>
            
</asp:TableCell>
        </asp:TableRow>

                <asp:TableRow ID="TableRow2" runat="server" ClientIDMode="Static">
                                                                                    
                                                                                          
<asp:TableCell ID="TableCell3" runat="server" ClientIDMode="Static">

</asp:TableCell>
            <asp:TableCell ID="TableCell4" runat="server" ClientIDMode="Static"><asp:Button ID="btnAceptar" runat="server" ClientIDMode="Static" Text="Aceptar"  /><asp:Button ID="btnAceptarTrimestre" runat="server" ClientIDMode="Static" Text="Aceptar" Style="display:none;"/></asp:TableCell>
        </asp:TableRow>
    </asp:Table>


    


    &nbsp;<asp:HyperLink ID="hlkExcel" runat="server" ClientIDMode="Static" NavigateUrl="~/doc/archivosCsv/2011/CH 2011.csv">Descargar Excel</asp:HyperLink>
    </div>
    <div class="footDer">
    <asp:HyperLink ID="hlkConcesionario" runat="server" ClientIDMode="Static" 
            NavigateUrl="~/Vistas/Indicadores.aspx">Subir Indicador</asp:HyperLink>
    </div>

    <div id="chart_div"></div>
    
    
    <div class="pie">
    
    </div>
</asp:Content>

