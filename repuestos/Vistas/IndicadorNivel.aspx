<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="IndicadorNivel.aspx.cs" Inherits="Vistas_indicadorNivel" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script language="javascript" type="text/javascript">
     function muestra() 
     {
         if (document.getElementById('rbtnMes').checked) 
         {
             document.getElementById('rowMes').style.display = '';
             document.getElementById('rowTrimestre').style.display = 'none';
         }
         else if (document.getElementById('rbtnTrimestre').checked) 
         {

             document.getElementById('rowTrimestre').style.display = '';
             document.getElementById('rowMes').style.display = 'none';
         }
     }
</script>

 <!--Load the AJAX API-->
    <script type="text/javascript" src="https://www.google.com/jsapi"></script>
    <script type="text/javascript">

        // Load the Visualization API and the piechart package.
        google.load('visualization', '1', { 'packages': ['corechart'] });

        // Set a callback to run when the Google Visualization API is loaded.
        //google.setOnLoadCallback(drawChart);

        // Callback that creates and populates a data table, 
        // instantiates the pie chart, passes in the data and
        // draws it.
        function drawChartTrimestre() {

             // Create our data table.
            var data = new google.visualization.DataTable();
            data.addColumn('string', 'Mes');
            data.addColumn('number', 'Compras');
            var columnas =<%=_cantidadTrimestres %>+1;

            //GUARDO LAS VARIABLES DE LOS MESES C# EN UN ARREGLO DE JS
            var compraTrimestre= new Array(4);
             compraTrimestre[0]='<%=_primerTrimestreCompras %>';
             compraTrimestre[1]='<%=_segundoTrimestreCompras %>';
             compraTrimestre[2]='<%=_tercerTrimestreCompras %>';
             compraTrimestre[3]='<%=_cuartoTrimestreCompras %>';

            //MESES A MOSTRAR
            var trimestres= new Array(4);
            trimestres[0]='Primero';
            trimestres[1]='Segundo';
            trimestres[2]='Tercero';
            trimestres[3]='Cuarto';

            var desde=<%=_triDesde %>;
            //alert(columnas);
            //alert(metaMes);
            //alert(desde);

            //WHILE HASTA QUE ME ENCUENTRE CON UN DATO VACIO OSEA EN 0 PARA NO SEGUIR ESCRIBIENDO MESES
            var indice=0;
            while(parseInt(compraTrimestre[indice])!=0 && indice!=4)
            {
                //alert(compraMes[indice]);
                indice++;
            }

            //alert(indice+'  '+columnas);
            var rowsDelGrafico=0;
            //SI LAS COLUMNAS SON MAYOR QUE INDICE(HASTA DONDE HAY DATOS), ADIERO INDICE COMO DATAROWS
            if(columnas>indice)
            {
                rowsDelGrafico=indice
                //alert(rowsDelGrafico);
                data.addRows(rowsDelGrafico); //este el largo de la cantidad columnas una fila columna contiene varias barras
            }
            else
            {
                rowsDelGrafico=columnas;
                //alert(rowsDelGrafico);
                data.addRows(rowsDelGrafico);
            }
            for (i=0; i<rowsDelGrafico; i++)
            {
                //alert(i);
                data.setValue(i, 0, trimestres[desde]);
                data.setValue(i, 1, parseInt(compraTrimestre[desde]));
                desde++;
            }

            var chart = new google.visualization.ColumnChart(document.getElementById('chart_div'));
            chart.draw(data, { width: 700, height: 240, title: 'Cumplimiento metas <%=_nombreMarca %> <%=_ano.ToString() %> $',
                hAxis: { title: 'Trimestre', titleTextStyle: { color: 'red'} }, vAxis: { title: 'Pesos', titleTextStyle: { color: 'red'} }
            });
            return false;
        }

        function drawChartMeses() {


            // Create our data table.
            var data = new google.visualization.DataTable();
            data.addColumn('string', 'Mes');
            //data.addColumn('number', 'metas');
            data.addColumn('number', '<%=_nombreMarca %>');
            var columnas =<%=_cantidadMeses %>+1;

            //GUARDO LAS VARIABLES DE LOS MESES C# EN UN ARREGLO DE JS
            var metaMes= new Array(12);
             metaMes[0]='<%=_metaEnero %>';
             metaMes[1]='<%=_metaFebrero %>';
             metaMes[2]='<%=_metaMarzo %>';
             metaMes[3]='<%=_metaAbril %>';
             metaMes[4]='<%=_metaMayo %>';
             metaMes[5]='<%=_metaJunio %>';
             metaMes[6]='<%=_metaJulio %>';
             metaMes[7]='<%=_metaAgosto %>';
             metaMes[8]='<%=_metaSeptiembre %>';
             metaMes[9]='<%=_metaOctubre %>';
             metaMes[10]='<%=_metaNoviembre %>';
             metaMes[11]='<%=_metaDiciembre %>';


             //GUARDO LAS VARIABLES DE LOS MESES C# EN UN ARREGLO DE JS
            var compraMes= new Array(12);
             compraMes[0]='<%=_compraEnero %>';
             compraMes[1]='<%=_compraFebrero %>';
             compraMes[2]='<%=_compraMarzo %>';
             compraMes[3]='<%=_compraAbril %>';
             compraMes[4]='<%=_compraMayo %>';
             compraMes[5]='<%=_compraJunio %>';
             compraMes[6]='<%=_compraJulio %>';
             compraMes[7]='<%=_compraAgosto %>';
             compraMes[8]='<%=_compraSeptiembre %>';
             compraMes[9]='<%=_compraOctubre %>';
             compraMes[10]='<%=_compraNoviembre %>';
             compraMes[11]='<%=_compraDiciembre %>';

            //MESES A MOSTRAR
            var meses= new Array(12);
            meses[0]='Enero';
            meses[1]='Febrero';
            meses[2]='Marzo';
            meses[3]='Abril';
            meses[4]='Mayo';
            meses[5]='Junio';
            meses[6]='Julio';
            meses[7]='Agosto';
            meses[8]='Septiembre';
            meses[9]='Octubre';
            meses[10]='Noviembre';
            meses[11]='Diciembre';

            var desde=<%=_iDesde %>;
            //alert(columnas);
            //alert(compraMes);
            //alert(desde);

            //WHILE HASTA QUE ME ENCUENTRE CON UN DATO VACIO OSEA EN 0 PARA NO SEGUIR ESCRIBIENDO MESES
            var indice=0;
            while(parseInt(compraMes[indice])!=0 && indice!=12)
            {
                //alert(compraMes[indice]);
                indice++;
            }

            //alert(indice+'  '+columnas);
            var rowsDelGrafico=0;
            //SI LAS COLUMNAS SON MAYOR QUE INDICE(HASTA DONDE HAY DATOS), ADIERO INDICE COMO DATAROWS
            if(columnas>indice)
            {
                rowsDelGrafico=indice
                //alert(rowsDelGrafico);
                data.addRows(rowsDelGrafico); //este el largo de la cantidad columnas una fila columna contiene varias barras
            }
            else
            {
                rowsDelGrafico=columnas;
                //alert(rowsDelGrafico);
                data.addRows(rowsDelGrafico);
            }

            for (i=0; i<rowsDelGrafico; i++)
            {
                //alert(i);
                data.setValue(i, 0, meses[desde]);
                //data.setValue(i, 1, parseInt(metaMes[desde]));
                data.setValue(i, 1, parseInt(compraMes[desde]));
                desde++;
            }

            var chart = new google.visualization.ColumnChart(document.getElementById('chart_div'));
            chart.draw(data, { width: 700, height: 240, title: 'Nivel de Compras <%=_nombreMarca %> <%=_ano.ToString() %> $',
                hAxis: { title: 'Meses', titleTextStyle: { color: 'red'} }, vAxis: { title: 'Compras', titleTextStyle: { color: 'red'} }
            });
            return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="titulo">
        Indicador Nivel de compras
    </div>
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <div class="indicadoresIzquierda">
        <center><b><u>Indicadores:</u></b></center>
        <ul>
            <li><asp:HyperLink ID="hlkNivel" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/indicadorNivel.aspx"><b>Nivel de compra</b></asp:HyperLink></li>
            <li><asp:HyperLink ID="hlkEstadistica" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/indicadorEstadistica.aspx">Estadistica de compra</asp:HyperLink></li>
            <li><asp:HyperLink ID="hlkEvoluvion" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/indicadorEvolucion.aspx">Evolucion de compra</asp:HyperLink></li>
            <li runat="server" id="liFallidas" Visible="false"><asp:HyperLink ID="hlkFallidas" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/indicadorFallidas.aspx">Ventas fallidas</asp:HyperLink></li>
            <li><asp:HyperLink ID="hlkMetas" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/verIndicador.aspx">Metas</asp:HyperLink></li>
            <li runat="server" id="liCotizacion" Visible="false"><asp:HyperLink ID="hlkCotizacion" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/indicadorCotizacion.aspx">Cotizacion v/s compras</asp:HyperLink></li>
        </ul>
        <asp:HyperLink ID="hlkConcesionario" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/Indicadores.aspx">Subir Indicador</asp:HyperLink>
    </div>
    <div class="indicadoresDerecha">
        <div class="avisoInsert">
            <asp:Label ID="lblAviso" runat="server" ClientIDMode="Static" ForeColor="#99CCFF" ></asp:Label>
        </div>
        Concesionario: 
        <asp:Label ID="lblConcesionario" runat="server" ClientIDMode="Static" CssClass="nombreConcesionario"></asp:Label>
        <asp:DropDownList ID="ddlConcesionario" runat="server" ClientIDMode="Static"> 
        </asp:DropDownList>
        <asp:Table ID="tblDatos" runat="server" ClientIDMode="Static">
            <asp:TableRow ID="TableRow4" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell7" runat="server" ClientIDMode="Static">
                    Marca :
                </asp:TableCell>
                <asp:TableCell ID="TableCell8" runat="server" ClientIDMode="Static">
                    <asp:DropDownList ID="ddlMarca" runat="server" ClientIDMode="Static"></asp:DropDownList>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow3" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell5" runat="server" ClientIDMode="Static">
                    Año :
                </asp:TableCell>
                <asp:TableCell ID="TableCell6" runat="server" ClientIDMode="Static">
                    <asp:DropDownList ID="ddlAno" runat="server" ClientIDMode="Static"></asp:DropDownList>
                </asp:TableCell>
            </asp:TableRow>
                <asp:TableRow ID="TableRow1" runat="server" ClientIDMode="Static">
                    <asp:TableCell ID="TableCell1" runat="server" ClientIDMode="Static">
                        <asp:RadioButton ID="rbtnMes" runat="server" ClientIDMode="Static" Text="Mes" GroupName="grupo" Checked="true" OnClick="muestra();" />
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell2" runat="server" ClientIDMode="Static">
                        <asp:RadioButton ID="rbtnTrimestre" runat="server" ClientIDMode="Static" Text="Trimestre" GroupName="grupo" OnClick="muestra();"/>
                    </asp:TableCell>
                </asp:TableRow>
            <asp:TableRow ID="rowMes" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell9" runat="server" ClientIDMode="Static"><asp:Label ID="lblDesde" runat="server" ClientIDMode="Static">Desde</asp:Label>
                    <asp:DropDownList ID="ddlDesde" runat="server" ClientIDMode="Static">
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
                <asp:TableCell ID="TableCell10" runat="server" ClientIDMode="Static"><asp:Label ID="lblHasta" runat="server" ClientIDMode="Static">Hasta</asp:Label>
                    <asp:DropDownList ID="ddlHasta" runat="server" ClientIDMode="Static"><asp:ListItem>Enero</asp:ListItem>
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
            <asp:TableRow ID="rowTrimestre" runat="server" ClientIDMode="Static" style="display:none">
                <asp:TableCell ID="TableCell11" runat="server" ClientIDMode="Static"><asp:Label ID="lblTriDesde" runat="server" ClientIDMode="Static">Desde</asp:Label><br />
                    <asp:DropDownList ID="ddlTriDesde" runat="server" ClientIDMode="Static">
                        <asp:ListItem>Primero</asp:ListItem>
                        <asp:ListItem>Segundo</asp:ListItem>
                        <asp:ListItem>Tercero</asp:ListItem>
                        <asp:ListItem>Cuarto</asp:ListItem>
                    </asp:DropDownList>
                </asp:TableCell>
                <asp:TableCell ID="TableCell12" runat="server" ClientIDMode="Static">
                    <asp:Label ID="lblTriHasta" runat="server" ClientIDMode="Static">Hasta</asp:Label>
                    <asp:DropDownList ID="ddlTriHasta" runat="server" ClientIDMode="Static">
                        <asp:ListItem>Primero</asp:ListItem>
                        <asp:ListItem>Segundo</asp:ListItem>
                        <asp:ListItem>Tercero</asp:ListItem>
                        <asp:ListItem>Cuarto</asp:ListItem>
                    </asp:DropDownList>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow2" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell4" runat="server" ClientIDMode="Static" ColumnSpan="2" HorizontalAlign="Center">
                    <asp:Button ID="btnAceptar" runat="server" ClientIDMode="Static" Text="Aceptar"  CssClass="button"/>
                    <asp:Button ID="btnAceptarTrimestre" runat="server" ClientIDMode="Static" Text="Aceptar" Style="display:none;" CssClass="button"/>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </div>
    <div id="chart_div">
    </div>
</asp:Content>
