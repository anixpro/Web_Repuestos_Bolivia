<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="VerIndicador.aspx.cs" Inherits="Vistas_verIndicador" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
     <script language="javascript" type="text/javascript">
         $(document).ready(function () {
             muestra();
             muestraTipo();

             $("#rbtnMes").click(function () {
                 muestra();
             });

             $("#rbtnTrimestre").click(function () {
                 muestra();
             });

             $("#rbtnNormal").click(function () {
                 muestraTipo();
             });

             $("#rbtnUnoAUno").click(function () {
                 muestraTipo();
             });

             function muestra() 
             {
                 if ($("#rbtnMes").is(":checked")) 
                 {
                     $('#rowMes').show();
                     $('#rowTrimestre').hide();
                 }
                 else if ($('#rbtnTrimestre').is(':checked')) 
                 {
                     $('#rowTrimestre').show();
                     $('#rowMes').hide();
                 }
             }
         });

         function muestraTipo() 
         {
             if ($('#rbtnUnoAUno').is(':checked')) 
             {
                 $('#tblComparacion').show();
                 $('#tblDatos').hide();
             }
             else if ($('#rbtnNormal').is(':checked')) 
             {
                 $('#tblDatos').show();
                 $('#tblComparacion').hide();
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
            data.addColumn('number', 'Metas');
            data.addColumn('number', 'Compras');
            var columnas =<%=_cantidadTrimestres %>+1;

            //GUARDO LAS VARIABLES DE LOS MESES C# EN UN ARREGLO DE JS
            var metaTrimestre= new Array(4);
             metaTrimestre[0]='<%=_primerTrimestreMetas %>';
             metaTrimestre[1]='<%=_segundoTrimestreMetas %>';
             metaTrimestre[2]='<%=_tercerTrimestreMetas %>';
             metaTrimestre[3]='<%=_cuartoTrimestreMetas %>';

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

                //este el largo de la cantidad columnas una fila columna contiene varias barras
                data.addRows(rowsDelGrafico);
            }
            else
            {
                rowsDelGrafico=columnas;
                data.addRows(rowsDelGrafico);
            }
            for (i=0; i<rowsDelGrafico; i++)
            {
                data.setValue(i, 0, trimestres[desde]);
                data.setValue(i, 1, parseInt(metaTrimestre[desde]));
                data.setValue(i, 2, parseInt(compraTrimestre[desde]));
                desde++;
            }

            var chart = new google.visualization.ColumnChart(document.getElementById('chart_div'));
            chart.draw(data, { width: 700, height: 240, title: 'Cumplimiento metas <%=_nombreMarca %> <%=_ano.ToString() %> $',
                hAxis: { title: 'Trimestre', titleTextStyle: { color: 'red'} }, vAxis: { title: 'Pesos', titleTextStyle: { color: 'red'} },
                colors: ['#DF0101', '#0404B4']
            });
            return false;
        }


        function drawChartMeses() {
            // Create our data table.
            var data = new google.visualization.DataTable();
            data.addColumn('string', 'Mes');
            data.addColumn('number', 'Metas');
            data.addColumn('number', 'Compras');
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
            //WHILE HASTA QUE ME ENCUENTRE CON UN DATO VACIO OSEA EN 0 PARA NO SEGUIR ESCRIBIENDO MESES
            var indice=0;
            while(parseInt(compraMes[indice])!=0 && indice!=12)
            {
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
                    data.setValue(i, 1, parseInt(metaMes[desde]));
                    data.setValue(i, 2, parseInt(compraMes[desde]));
                    desde++;
                }

            var chart = new google.visualization.ColumnChart(document.getElementById('chart_div'));
            chart.draw(data, { width: 700, height: 240, title: 'Cumplimiento metas <%=_nombreMarca %> <%=_ano.ToString() %> $',
                hAxis: { title: 'Meses', titleTextStyle: { color: 'red'} }, vAxis: { title: 'Pesos', titleTextStyle: { color: 'red'} },
                colors: ['#DF0101', '#0404B4']
            });
            return false;
        }

        function drawChartComparacion() {
            // Create our data table.
            var data = new google.visualization.DataTable();
            data.addColumn('string', 'Mes');
            data.addColumn('number', 'Metas');
            data.addColumn('number', 'Compras');

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

            var primerMes='<%=ddlComparacionMes1.SelectedIndex %>';
            var segundoMes='<%=ddlComparacionMes2.SelectedIndex %>';

            data.addRows(2);

            //alert(segundoMes);
            data.setValue(0, 0, meses[primerMes]+' '+'<%=_anoComparacion1%>');
            data.setValue(0, 1, <%=_metaMes1 %>);
            data.setValue(0, 2, <%=_compraMes1 %>);
              
            data.setValue(1, 0, meses[segundoMes]+' '+'<%=_anoComparacion2%>');
            data.setValue(1, 1, <%=_metaMes2 %>);
            data.setValue(1, 2, <%=_compraMes2 %>);

            var chart = new google.visualization.ColumnChart(document.getElementById('chart_div'));
            chart.draw(data, { width: 700, height: 240, title: 'Comparacion Meses <%=_nombreMarca %>',
                hAxis: { title: 'Meses', titleTextStyle: { color: 'red'} }, vAxis: { title: 'Pesos', titleTextStyle: { color: 'red'} },
                colors: ['#DF0101', '#0404B4']
            });
            return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="titulo">
        Indicador Metas
    </div>
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <ul style="float:left;">
        <li><asp:HyperLink ID="hlkMetas" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/verIndicador.aspx">Metas</asp:HyperLink></li>
        <li><asp:HyperLink ID="hlkEstadistica" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/indicadorEstadistica.aspx">Estadistica</asp:HyperLink></li>
        <li><asp:HyperLink ID="hlkEvoluvion" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/indicadorEvolucion.aspx">Evolucion</asp:HyperLink></li>
        <li><asp:HyperLink ID="hlkNivel" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/indicadorNivel.aspx">Nivel</asp:HyperLink></li>
        <li id="liCotizacion" runat="server"><asp:HyperLink ID="hlkCotizacion" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/indicadorCotizacion.aspx" Visible="false">Cotizacion</asp:HyperLink></li>
        <li id="liFallidas" runat="server"><asp:HyperLink ID="hlkFallidas" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/indicadorFallidas.aspx" Visible="false">Fallidas</asp:HyperLink></li>
    </ul>
    <div class="centerGrande">
        <asp:Label ID="lblConcesionario" runat="server" ClientIDMode="Static"></asp:Label>
        <asp:DropDownList ID="ddlConcesionario" runat="server" ClientIDMode="Static"> 
        </asp:DropDownList>
        <br />
        <asp:RadioButton ID="rbtnNormal" CssClass="rbtnNormal" runat="server" ClientIDMode="Static" GroupName="tipoGrafico" Text="Grafico Normal" Checked="true" /><asp:RadioButton ID="rbtnUnoAUno" CssClass="rbtnUnoAUno" runat="server" ClientIDMode="Static" GroupName="tipoGrafico" Text="Grafico uno a uno"/>
        <br />
        <br/>
    <asp:Table ID="tblDatos" CssClass="tblDatos" runat="server" ClientIDMode="Static">
        <asp:TableRow ID="TableRow4" runat="server" ClientIDMode="Static">
            <asp:TableCell ID="TableCell7" runat="server" ClientIDMode="Static">
                <asp:Label ID="lblMarca" runat="server" ClientIDMode="Static" Text="Marca"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell8" runat="server" ClientIDMode="Static">
                <asp:DropDownList ID="ddlMarca" runat="server" ClientIDMode="Static"></asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow3" runat="server" ClientIDMode="Static">
            <asp:TableCell ID="TableCell5" runat="server" ClientIDMode="Static">
                <asp:Label ID="lblAno" runat="server" ClientIDMode="Static" Text="Año"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell6" runat="server" ClientIDMode="Static">
                <asp:DropDownList ID="ddlAno" runat="server" ClientIDMode="Static">
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow1" runat="server" ClientIDMode="Static">
            <asp:TableCell ID="TableCell1" runat="server" ClientIDMode="Static">
                <asp:RadioButton ID="rbtnMes"  CssClass="rbtnMes" runat="server" ClientIDMode="Static" Text="Mes" GroupName="grupo" Checked="true"  />
            </asp:TableCell>
            <asp:TableCell ID="TableCell2" runat="server" ClientIDMode="Static">
                <asp:RadioButton ID="rbtnTrimestre"  CssClass="rbtnTrimestre" runat="server" ClientIDMode="Static" Text="Trimestre" GroupName="grupo" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="rowMes" runat="server" ClientIDMode="Static" CssClass="rowMes">
            <asp:TableCell ID="TableCell9" runat="server" ClientIDMode="Static"><asp:Label ID="lblDesde" runat="server" ClientIDMode="Static">Desde</asp:Label><asp:DropDownList ID="ddlDesde" runat="server" ClientIDMode="Static">
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
        <asp:TableRow ID="rowTrimestre" runat="server" ClientIDMode="Static" style="display:none" CssClass="rowTrimestre">
            <asp:TableCell ID="TableCell11" runat="server" ClientIDMode="Static"><asp:Label ID="lblTriDesde" runat="server" ClientIDMode="Static">Desde</asp:Label><br />
                <asp:DropDownList ID="ddlTriDesde" runat="server" ClientIDMode="Static">
                    <asp:ListItem>Primero</asp:ListItem>
                    <asp:ListItem>Segundo</asp:ListItem>
                    <asp:ListItem>Tercero</asp:ListItem>
                    <asp:ListItem>Cuarto</asp:ListItem>
                </asp:DropDownList>
            </asp:TableCell>
            <asp:TableCell ID="TableCell12" runat="server" ClientIDMode="Static"><asp:Label ID="lblTriHasta" runat="server" ClientIDMode="Static">Hasta</asp:Label><br />
                <asp:DropDownList ID="ddlTriHasta" runat="server" ClientIDMode="Static">
                    <asp:ListItem>Primero</asp:ListItem>
                    <asp:ListItem>Segundo</asp:ListItem>
                    <asp:ListItem>Tercero</asp:ListItem>
                    <asp:ListItem>Cuarto</asp:ListItem>
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow2" runat="server" ClientIDMode="Static">                                                                                  
            <asp:TableCell ID="TableCell3" runat="server" ClientIDMode="Static">
            </asp:TableCell>
            <asp:TableCell ID="TableCell4" runat="server" ClientIDMode="Static">
                <asp:Button ID="btnAceptar" runat="server" ClientIDMode="Static" Text="Aceptar"  /><asp:Button ID="btnAceptarTrimestre" runat="server" ClientIDMode="Static" Text="Aceptar" Style="display:none;"/>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <asp:Table ID="tblComparacion" runat="server" ClientIDMode="Static" style="display:none" CssClass="tblComparacion">
        <asp:TableRow ID="TableRow5" runat="server" ClientIDMode="Static">
            <asp:TableCell ID="TableCell13" runat="server" ClientIDMode="Static">
                <asp:Label ID="Label1" runat="server" ClientIDMode="Static" Text="Marca"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell14" runat="server" ClientIDMode="Static">
                <asp:DropDownList ID="ddlMarcaComparacion" runat="server" ClientIDMode="Static"></asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow8" runat="server" ClientIDMode="Static">
            <asp:TableCell ID="TableCell19" runat="server" ClientIDMode="Static">
                <asp:DropDownList ID="ddlAñoComparacion1" runat="server" ClientIDMode="Static"></asp:DropDownList>
            </asp:TableCell>
            <asp:TableCell ID="TableCell20" runat="server" ClientIDMode="Static">
                <asp:DropDownList ID="ddlComparacionMes1" runat="server" ClientIDMode="Static"><asp:ListItem>Enero</asp:ListItem>
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
        <asp:TableRow ID="TableRow9" runat="server" ClientIDMode="Static">
            <asp:TableCell ID="TableCell21" runat="server" ClientIDMode="Static">
                <asp:DropDownList ID="ddlAñoComparacion2" runat="server" ClientIDMode="Static">
                </asp:DropDownList>
            </asp:TableCell>
            <asp:TableCell ID="TableCell22" runat="server" ClientIDMode="Static">
                <asp:DropDownList ID="ddlComparacionMes2" runat="server" ClientIDMode="Static">
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
        <asp:TableRow ID="TableRow10" runat="server" ClientIDMode="Static">                                                                                  
            <asp:TableCell ID="TableCell23" runat="server" ClientIDMode="Static">
            </asp:TableCell>
            <asp:TableCell ID="TableCell24" runat="server" ClientIDMode="Static"><asp:Button ID="Button1" runat="server" ClientIDMode="Static" Text="Aceptar"  /><asp:Button ID="Button2" runat="server" ClientIDMode="Static" Text="Aceptar" Style="display:none;"/></asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    &nbsp;<asp:HyperLink ID="hlkExcel" runat="server" ClientIDMode="Static" NavigateUrl="~/doc/archivosCsv/2011/CH 2011.csv">Descargar Excel</asp:HyperLink>
    </div>
    <div class="footDer">
        <asp:HyperLink ID="hlkConcesionario" runat="server" ClientIDMode="Static" 
            NavigateUrl="~/Vistas/Indicadores.aspx">Subir Indicador</asp:HyperLink>
    </div>
    <div id="chart_div"></div>
    <div class="pie"></div>
</asp:Content>