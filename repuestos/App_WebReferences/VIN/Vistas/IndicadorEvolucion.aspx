<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="IndicadorEvolucion.aspx.cs" Inherits="Vistas_indicadorEvolucion" %>

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
     function validaCheck() {

         var numeroDeChecks='<%=cbxlMarcas.Items.Count %>';
         var checkTotal = 'cbxlMarcas_';
         var nombreCheck = '';
         var aux = 0;
         //alert(numeroDeChecks);
         for (x = 0; x < numeroDeChecks; x++)
         {

             nombreCheck=checkTotal + x;
             //alert(nombreCheck);
             if (!document.getElementById(nombreCheck).checked) 
             {
             
               aux+=1;
                 
             }
         }

       //alert(aux);
       if (aux == numeroDeChecks) 
       {
           alert('Debe seleccionar al menos una marca');
           return false;
       }
       else 
       {
           return true;
       }
         

     }

</script>

 <!--Load the AJAX API-->
    <script type="text/javascript" src="https://www.google.com/jsapi"></script>
    <script type="text/javascript">

        
        // Load the Visualization API and the piechart package.
        google.load('visualization', '1', { 'packages': ['LineChart']/*['CoreChart']*/ });

        // Set a callback to run when the Google Visualization API is loaded.
        //google.setOnLoadCallback(drawChart);

        // Callback that creates and populates a data table, 
        // instantiates the pie chart, passes in the data and
        // draws it.
        

        function drawChartTrimestre() {

       

            var cantidadMarca=<%=_cantidadMarcas %>;
            var marca = new Array(cantidadMarca);     
            var todasLasCompras = new Array(<%=_cantidadTodasLasCompras %>);   
            //alert('hola');
     
            <%
            int z=0;
            foreach(String valor in _marcasSelecionadas)
            {
           
            %>
                
                marca[<%=z %>]='<%=valor %>';
                
            <%
             z++;} 
            %>



            <%
            int y=0;
            foreach(int valor in _comprasTodosLosMeses)
            {
           
            %>
                
                todasLasCompras[<%=y %>]=<%=valor %>;
                
            <%
             y++;} 
            %>
            //alert(todasLasCompras);

            // Create our data table.
            var data = new google.visualization.DataTable();
            data.addColumn('string', 'Mes');
            //data.addColumn('number', 'metas');
            for(var x=0; x<marca.length; x++)
            {
                //alert(marca[x]);
                data.addColumn('number', marca[x]);
            }
            //data.addColumn('number', 'Marca');
            var columnas =<%=_cantidadTrimestres %>+1;

            var trimestres= new Array(4);
            trimestres[0]='Primero';
            trimestres[1]='Segundo';
            trimestres[2]='Tercero';
            trimestres[3]='Cuarto';

            var desde=<%=_triDesde %>;
            //alert(columnas);
            //alert(metaMes);
            //alert(desde);
            //i = meses
            var mes=<%=_triDesde %>;

            aux=mes;
            aux1=desde;
            var valor=new Array(4);
            for(i=0; i<4;i++)
            {
                valor[i]=todasLasCompras[aux]+todasLasCompras[aux+1]+todasLasCompras[aux+2];
                //alert(valor[i]);
                aux+=12;
                    
                aux=aux1+3;
                aux1++;
            }


            
            //WHILE HASTA QUE ME ENCUENTRE CON UN DATO VACIO OSEA EN 0 PARA NO SEGUIR ESCRIBIENDO MESES
            var indice=0;
            while(valor[indice]!=0 && indice!=4)
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






            //alert(rowsDelGrafico);
            //data.addRows(columnas); //este el largo de la cantidad columnas una fila columna contiene varias barras
            for (i=0; i<rowsDelGrafico; i++)
            {
                valorTrimestre=0;
                //alert(i);
                data.setValue(i, 0, trimestres[desde]);
                for(var x=1; x<marca.length+1; x++)
                {
                    
                    valorTrimestre=todasLasCompras[mes]+todasLasCompras[mes+1]+todasLasCompras[mes+2];
                    //alert(valorTrimestre);
                    data.setValue(i,x, valorTrimestre);
                    mes+=12;
                    
                }
                mes=desde+3;
                desde++;
            }
            

           

            var chart = new google.visualization.LineChart(document.getElementById('chart_div'));
            chart.draw(data, { width: 700, height: 240, title: 'Evolucion compras <%=_ano.ToString() %> $',
                hAxis: { title: 'Meses', titleTextStyle: { color: 'red'} } , vAxis: { title: 'Compras', titleTextStyle: { color: 'red'} }
            });
            return false;
        }

    
        function drawChartMeses() {

       

            var cantidadMarca=<%=_cantidadMarcas %>;
            var marca = new Array(cantidadMarca);     
            var todasLasCompras = new Array(<%=_cantidadTodasLasCompras %>);   
            //alert('hola');
     
            <%
            int x=0;
            foreach(String valor in _marcasSelecionadas)
            {
           
            %>
                
                marca[<%=x %>]='<%=valor %>';
                
            <%
             x++;} 
            %>



            <%
            int j=0;
            foreach(int valor in _comprasTodosLosMeses)
            {
           
            %>
                
                todasLasCompras[<%=j %>]=<%=valor %>;
                
            <%
             j++;} 
            %>
            //alert(todasLasCompras);

            // Create our data table.
            var data = new google.visualization.DataTable();
            data.addColumn('string', 'Mes');
            //data.addColumn('number', 'metas');
            for(var x=0; x<marca.length; x++)
            {
                //alert(marca[x]);
                data.addColumn('number', marca[x]);
            }
            //data.addColumn('number', 'Marca');
            var columnas =<%=_cantidadMeses %>+1;

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
            //alert(metaMes);
            //alert(desde);
            //i = meses
            var mes=<%=_iDesde %>;

            //alert(todasLasCompras);
            if (todasLasCompras!='')
            {
                //WHILE HASTA QUE ME ENCUENTRE CON UN DATO VACIO OSEA EN 0 PARA NO SEGUIR ESCRIBIENDO MESES
                var indice=0;
                while(todasLasCompras[indice]!=0 && indice!=12)
                {
                    //alert(compraMes[indice]);
                    indice++;
                }
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


            //data.addRows(columnas); //este el largo de la cantidad columnas una fila columna contiene varias barras
            for (i=0; i<rowsDelGrafico; i++)
            {
                //alert(i);
                data.setValue(i, 0, meses[desde]);
                for(var x=1; x<marca.length+1; x++)
                {
                    
                    //alert(marca[x-1]);
                    data.setValue(i,x, todasLasCompras[mes]);
                    mes+=12;
                    
                }
                mes=desde+1;
                desde++;
            }
            

           

            var chart = new google.visualization.LineChart(document.getElementById('chart_div'));
            chart.draw(data, { width: 700, height: 240, title: 'Evolucion compras <%=_ano.ToString() %> $',
                hAxis: { title: 'Meses', titleTextStyle: { color: 'red'} } , vAxis: { title: 'Compras', titleTextStyle: { color: 'red'} }
            });
            return false;
        }

        $(document).ready(function () 
            {
                $("#cbxTodos").click(function () 
                {
                    if ($(this).attr("checked")) 
                    {
                        $(':checkbox').attr("checked", true);
                    }
                    else 
                    {
                        $(':checkbox').attr("checked", false);
                    }
                });
            });
    </script>



            

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="titulo">
        Indicador Evolucion de Compra
    </div>
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <ul style="float:left;">
        <li><asp:HyperLink ID="hlkMetas" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/verIndicador.aspx">Metas</asp:HyperLink></li>
        <li><asp:HyperLink ID="hlkEstadistica" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/indicadorEstadistica.aspx">Estadistica</asp:HyperLink></li>
        <li><asp:HyperLink ID="hlkEvoluvion" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/indicadorEvolucion.aspx">Evolucion</asp:HyperLink></li>
        <li><asp:HyperLink ID="hlkNivel" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/indicadorNivel.aspx">Nivel</asp:HyperLink></li>
        <li><asp:HyperLink ID="hlkCotizacion" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/indicadorCotizacion.aspx" Visible="false">Cotizacion</asp:HyperLink></li>
        <li><asp:HyperLink ID="hlkFallidas" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/indicadorFallidas.aspx" Visible="false">Fallidas</asp:HyperLink></li>
    </ul>
        <div class="center">
            <asp:Label ID="lblConcesionario" runat="server" ClientIDMode="Static"></asp:Label>
            <asp:DropDownList ID="ddlConcesionario" runat="server" ClientIDMode="Static"> 
                </asp:DropDownList><br />
            <asp:CheckBox ID="cbxTodos" runat="server" ClientIDMode="Static" Text="Todas" style="margin-left:2px;"/><br /><br />
            <asp:CheckBoxList ID="cbxlMarcas" runat="server" ClientIDMode="Static" RepeatDirection="Horizontal">
            </asp:CheckBoxList>
    <div class="avisoInsert">
    <asp:Label ID="lblAviso" runat="server" ClientIDMode="Static" ForeColor="#99CCFF" ></asp:Label>
    </div>
    <asp:Table ID="tblDatos" runat="server" ClientIDMode="Static">
             
            <asp:TableRow ID="TableRow3" runat="server" ClientIDMode="Static">
           <asp:TableCell ID="TableCell5" runat="server" ClientIDMode="Static"><asp:Label ID="lblAno" runat="server" ClientIDMode="Static" Text="Año"></asp:Label>
</asp:TableCell>

            <asp:TableCell ID="TableCell6" runat="server" ClientIDMode="Static"><asp:DropDownList ID="ddlAno" runat="server" ClientIDMode="Static">
                                                                                           </asp:DropDownList>
</asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow1" runat="server" ClientIDMode="Static">
            <asp:TableCell ID="TableCell1" runat="server" ClientIDMode="Static"><asp:RadioButton ID="rbtnMes" runat="server" ClientIDMode="Static" Text="Mes" GroupName="grupo" Checked="true" OnClick="muestra();" />
</asp:TableCell>
            <asp:TableCell ID="TableCell2" runat="server" ClientIDMode="Static"><asp:RadioButton ID="rbtnTrimestre" runat="server" ClientIDMode="Static" Text="Trimestre" GroupName="grupo" OnClick="muestra();"/>
</asp:TableCell>
        </asp:TableRow>
                <asp:TableRow ID="rowMes" runat="server" ClientIDMode="Static">
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

<asp:TableRow ID="rowTrimestre" runat="server" ClientIDMode="Static" style="display:none">
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
            <asp:TableCell ID="TableCell4" runat="server" ClientIDMode="Static"><asp:Button ID="btnAceptar" runat="server" ClientIDMode="Static" Text="Aceptar"  /><asp:Button ID="btnAceptarTrimestre" runat="server" ClientIDMode="Static" Text="Aceptar" Style="display:none;"/></asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    &nbsp;</div>
    <div class="footDer">
    <asp:HyperLink ID="hlkConcesionario" runat="server" ClientIDMode="Static" 
            NavigateUrl="~/Vistas/Indicadores.aspx">Subir Indicador</asp:HyperLink>
    </div>
    <div style="margin-left:120px;">
    <div id="chart_div"></div>
    </div>
    
    <div class="pie">
    
    </div>
</asp:Content>

