<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ResultadosEncuesta.aspx.cs" Inherits="Vistas_resultadosEncuesta" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="https://www.google.com/jsapi"></script>
    <script type="text/javascript">
    
        // Load the Visualization API and the piechart package.
        google.load('visualization', '1.0', { 'packages': ['corechart'] });

        // Set a callback to run when the Google Visualization API is loaded.
        google.setOnLoadCallback(drawChart);

        // Callback that creates and populates a data table, 
        // instantiates the pie chart, passes in the data and
        // draws it.
        function drawChart() {

            // Create the data table.
            var data = new google.visualization.DataTable();
            data.addColumn('string', 'Respuestas');
            data.addColumn('number', 'Votos');
            data.addRows([
                ['<%=pregunta1%>', <%=respuesta1%>],
                ['<%=pregunta2%>', <%=respuesta2%>],
                ['<%=pregunta3%>', <%=respuesta3%>],
                ['<%=pregunta4%>', <%=respuesta4%>]
            ]);

            // Set chart options
            var options = { 'title': '<%=pregunta%>',
                'width': 242,
                'height': 232,
                'legend': 'none',
                'colors': ['green','red','blue','black']
            };

            // Instantiate and draw our chart, passing in some options.
            var chart = new google.visualization.BarChart(document.getElementById('chart_div'));
            chart.draw(data, options);
        }
    </script>
    
</head>
<body>
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <form id="form1" runat="server">
        <div id="chart_div" style="margin-top:-23px;"> 
        </div>
        <div id="error" runat="server" visible="false">
        </div>
		<div style="">
		<asp:ImageButton ID="btnVolver" runat="server" ImageUrl="~/img/volver.png" />
		</div>
    </form>

</body>
</html>
