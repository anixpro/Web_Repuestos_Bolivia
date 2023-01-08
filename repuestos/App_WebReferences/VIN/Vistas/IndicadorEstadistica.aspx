<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="IndicadorEstadistica.aspx.cs" Inherits="Vistas_indicadorEstadistica" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
 <script language="javascript" type="text/javascript">
     $(document).ready(function () {
         muestra();
         //FUNCTION QUE CAMBIA LA 'K' A 'k' 
         $("#txtDvr").keyup(function () {
             //alert($("#txtDvr").val());
             if ($("#txtDvr").val() == 'K') {
                 $("#txtDvr").val('k');
             }
         });
         $("#rbtnCodigo").click(function () {
             muestra();
         });
         $("#rbtnOperario").click(function () {
             muestra();
         });
         $("#rbtnTodos").click(function () {
             muestra();
         });
         //SI APRIETO EL BOTON Y ADEMAS ESTÁ CHECKEADO EL BUSCAR POR RUT, VALIDO EL RUT
         $("#btnAceptar").click(function () {
             if ($("#rbtnOperario").is(":checked")) {
                 return validaCampos();
             } //SINO, VÁLIDO EL CÓDIGO
             else if ($("#rbtnCodigo").is(":checked")) {
                 validaTxtCodigo();
             }
         });

     });
     
     function validaCampos() {
         validaTxtRut();
         mje = "";
         mje = mje_rut;
         if (mje != "") {
             alert(mje_rut); return false;
         }
         else {
             return true;
        }
     }
     function muestra() 
     {
         if ($("#rbtnCodigo").is(":checked")) 
         {
             $("#rowCodigo").show();
             $("#rowRut").hide();
         }
         else if ($("#rbtnOperario").is(":checked")) {
       
             $("#rowCodigo").hide();
             $("#rowRut").show();
         }
         else if ($("#rbtnTodos").is(":checked")) {
             $('#rowCodigo').hide();
             $('#rowRut').hide();
         }
     }

     function validaTxtCodigo() {
        // Funcion deprecada
     }

     function validaTxtRut() 
     {

         $("#lblRut").css("color", "#696969");
         cajaTexto = $("#txtRut").val();
         mje_rut = '';
         if (cajaTexto == "") 
         {
             mje_rut = 'debe llenar Rut\n';
             $("#lblRut").css("color", "red");
             $("#txtRut").val("");
         }
         else if (isNaN(cajaTexto)) 
         {
             mje_rut = 'solo numeros en Rut\n';
             $("#lblRut").css("color", "red");
             $("#txtRut").val("");
         }
         else 
         {
             var rut = $("#txtRut").val();
             var largo = rut.length;
             var i = 0;
             var dv = $("#txtDvr").val();
             var mult = 2;
             var suma = 0;
             largo--;
             while (largo >= 0) 
             {
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
             else if (digito == 11) 
             {
                 digito = 0;
             }
             if (digito != dv) 
             {
                 mje_rut = 'Rut inválido\n';
                 $("#lblRut").css("color", "red");
                 $("#txtRut").val("");
             }
         }

         return (mje_rut);
     } //fin validaRut
</script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="titulo">
        Indicador Estadistica de Compra
    </div>
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <ul style="float:left;">
        <li><asp:HyperLink ID="hlkNivel" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/indicadorNivel.aspx">Nivel de compra</asp:HyperLink></li>
        <li><asp:HyperLink ID="hlkEstadistica" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/indicadorEstadistica.aspx">Estadistica de compra</asp:HyperLink></li>
        <li><asp:HyperLink ID="hlkEvoluvion" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/indicadorEvolucion.aspx">Evolucion de compra</asp:HyperLink></li>
        <li><asp:HyperLink ID="hlkMetas" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/verIndicador.aspx">Metas</asp:HyperLink></li>
        <li><asp:HyperLink ID="hlkCotizacion" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/indicadorCotizacion.aspx" Visible="false">Cotizacion de compra</asp:HyperLink></li>
        <li><asp:HyperLink ID="hlkFallidas" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/indicadorFallidas.aspx" Visible="false">Ventas fallidas</asp:HyperLink></li>
    </ul>
        <div class="center">
        
            <asp:Label ID="lblConcesionario" runat="server" ClientIDMode="Static"></asp:Label>
            <br />
            
    <div class="avisoInsert">
    <asp:Label ID="lblAviso" runat="server" ClientIDMode="Static" ForeColor="#99CCFF" ></asp:Label><br /><br />
    </div>
    <asp:RadioButton ID="rbtnCodigo" runat="server" ClientIDMode="Static" Text="Codigo" GroupName="grupo" Checked="true" />
    <asp:RadioButton ID="rbtnOperario" runat="server" ClientIDMode="Static" Text="Operario" GroupName="grupo"  />
    <asp:RadioButton ID="rbtnTodos" runat="server" ClientIDMode="Static" Text="Todos" GroupName="grupo"  />
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

            <asp:TableCell ID="TableCell2" runat="server" ClientIDMode="Static"><asp:TextBox ID="txtCodigo" runat="server" ClientIDMode="Static"></asp:TextBox>
</asp:TableCell>
        </asp:TableRow>

<asp:TableRow ID="rowRut" runat="server" ClientIDMode="Static" Style="display:none;">
            <asp:TableCell ID="TableCell5" runat="server" ClientIDMode="Static"><asp:Label ID="lblRut" runat="server" ClientIDMode="Static" Text="Rut operario"></asp:Label>
</asp:TableCell>

            <asp:TableCell ID="TableCell6" runat="server" ClientIDMode="Static"><asp:TextBox ID="txtRut" Width="60" runat="server" ClientIDMode="Static"></asp:TextBox><asp:Label ID="lblGuion" runat="server" ClientIDMode="Static" Text="-"></asp:Label><asp:TextBox ID="txtDvr" runat="server" Width="20" MaxLength="1" ClientIDMode="Static"></asp:TextBox>
</asp:TableCell>
        </asp:TableRow>
            
<asp:TableRow ID="rowMes" runat="server" ClientIDMode="Static">

            <asp:TableCell ID="TableCell9" runat="server" ClientIDMode="Static">
            <asp:Label ID="lblDesde" runat="server" ClientIDMode="Static">Desde</asp:Label>

            <asp:DropDownList ID="ddlAnoDesde" runat="server" ClientIDMode="Static">
            </asp:DropDownList>
            
</asp:TableCell>

            <asp:TableCell ID="TableCell10" runat="server" ClientIDMode="Static">

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
        </asp:TableRow>

        <asp:TableRow ID="TableRow5" runat="server" ClientIDMode="Static">

            <asp:TableCell ID="TableCell11" runat="server" ClientIDMode="Static">
            <asp:Label ID="Label1" runat="server" ClientIDMode="Static">Hasta</asp:Label>

            <asp:DropDownList ID="ddlAnoHasta" runat="server" ClientIDMode="Static">
            </asp:DropDownList>
            
</asp:TableCell>

            <asp:TableCell ID="TableCell12" runat="server" ClientIDMode="Static">

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
</div>

                <!--Gridview resumen por Código -->
                <asp:GridView ID="gridDetallePedidoPorCodigoResumen" runat="server" ClientIDMode="Static"
                AutoGenerateColumns="False" EnableModelValidation="True" Style="text-align:center;" CellPadding="10" 
                ForeColor="#333333" GridLines="None" >
                 <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                 <Columns >
                   <asp:BoundField DataField="codigo" HeaderText="codigo"/>
                   <asp:BoundField DataField="descripcion" HeaderText="descripcion"/>
                   <asp:BoundField DataField="cantidad" HeaderText="cantidad"/>
                   <asp:BoundField DataField="precio" HeaderText="precio"/>
                   <asp:BoundField DataField="precioTotal" HeaderText="Total"/>
                  </Columns>
                
                 <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                 <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                 <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                 <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                 <EditRowStyle BackColor="#999999" />
                 <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                
            </asp:GridView>

            <!--Gridview resumen por rut -->
                <asp:GridView ID="gridDetallePedidoPorRutResumen" runat="server" ClientIDMode="Static"
                AutoGenerateColumns="False" EnableModelValidation="True" Style="text-align:center;" CellPadding="10" 
                ForeColor="#333333" GridLines="None" >
                 <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                 <Columns >
                   <asp:BoundField DataField="rut" HeaderText="rut"/>
                   <asp:BoundField DataField="nombre" HeaderText="nombre"/>
                   <asp:BoundField DataField="cantidad" HeaderText="cantidad"/>
                   <asp:BoundField DataField="precioTotal" HeaderText="Total"/>
                  </Columns>
                
                 <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                 <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                 <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                 <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                 <EditRowStyle BackColor="#999999" />
                 <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                
            </asp:GridView>

            <!--Gridview por Código -->
                <asp:GridView ID="gridDetallePedidoPorCodigo" runat="server" ClientIDMode="Static" AllowPaging="True" AllowSorting="True"
                AutoGenerateColumns="False" DataSourceID="sqldDetallePedidoPorCodigo"
                EnableModelValidation="True" Style="text-align:center;" CellPadding="10" 
                ForeColor="#333333" GridLines="None" >
                 <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                 <Columns >
                   <asp:BoundField DataField="codigo" HeaderText="codigo"/>
                   <asp:BoundField DataField="descripcion" HeaderText="descripcion"/>
                   <asp:BoundField DataField="cantidad" HeaderText="cantidad"/>
                   <asp:BoundField DataField="precio" HeaderText="precio"/>
                   <asp:BoundField DataField="fecha" HeaderText="fecha"/>
                  </Columns>
                
                 <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                 <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                 <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                 <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                 <EditRowStyle BackColor="#999999" />
                 <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                
            </asp:GridView>
            <!--Gridview por rut -->
            <asp:GridView ID="gridDetallePedidoPorRut" runat="server" ClientIDMode="Static" AllowPaging="True" AllowSorting="True"
                AutoGenerateColumns="False" DataSourceID="sqldDetallePedidoPorRut"
                EnableModelValidation="True" Style="text-align:center;" CellPadding="10" 
                ForeColor="#333333" GridLines="None" >
                 <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                 <Columns >
                   <asp:BoundField DataField="rut" HeaderText="rut"/>
                   <asp:BoundField DataField="nombre" HeaderText="nombre"/>
                   <asp:HyperLinkField DataTextField="codigo" HeaderText="codigo"  DataNavigateUrlFields="codigo" DataNavigateUrlFormatString="indicadorEstadistica.aspx?codigo={0}" />
                   <asp:BoundField DataField="cantidad" HeaderText="cantidad"/>
                   <asp:BoundField DataField="precio" HeaderText="precio"/>
                   <asp:BoundField DataField="fecha" HeaderText="fecha"/>
                 </Columns>
                
                 <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                 <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                 <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                 <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                 <EditRowStyle BackColor="#999999" />
                 <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                
            </asp:GridView>      

            <!--Gridview para todos -->
            <asp:GridView ID="gridDetallePedidoPorTodos" runat="server" ClientIDMode="Static" AllowPaging="True" AllowSorting="True"
                AutoGenerateColumns="False" DataSourceID="sqldDetallePedidoPorTodos"
                EnableModelValidation="True" Style="text-align:center;" CellPadding="10" 
                ForeColor="#333333" GridLines="None" >
                 <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                 <Columns >
                   <asp:HyperLinkField DataTextField="rut" HeaderText="Rut"  DataNavigateUrlFields="rut" DataNavigateUrlFormatString="indicadorEstadistica.aspx?rut={0}" />
                   <asp:BoundField DataField="nombre" HeaderText="nombre"/>
                   <asp:HyperLinkField DataTextField="codigo" HeaderText="codigo"  DataNavigateUrlFields="codigo" DataNavigateUrlFormatString="indicadorEstadistica.aspx?codigo={0}" />
                   <asp:BoundField DataField="descripcion" HeaderText="descripcion"/>
                   <asp:BoundField DataField="cantidad" HeaderText="cantidad"/>
                   <asp:BoundField DataField="precio" HeaderText="precio"/>
                   <asp:BoundField DataField="fecha" HeaderText="fecha"/>
                 </Columns>
                
                 <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                 <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                 <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                 <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                 <EditRowStyle BackColor="#999999" />
                 <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                
            </asp:GridView>   
            <asp:SqlDataSource ID="sqldDetallePedidoPorCodigo" runat="server" 
                ConnectionString="<%$ ConnectionStrings:skbergeConnectionString %>" ></asp:SqlDataSource>&nbsp;

            <asp:SqlDataSource ID="sqldDetallePedidoPorRut" runat="server" 
                ConnectionString="<%$ ConnectionStrings:skbergeConnectionString %>" ></asp:SqlDataSource>&nbsp;

                <asp:SqlDataSource ID="sqldDetallePedidoPorTodos" runat="server" 
                ConnectionString="<%$ ConnectionStrings:skbergeConnectionString %>" ></asp:SqlDataSource>&nbsp;

                  <div class="footDer">
    <asp:HyperLink ID="hlkConcesionario" runat="server" ClientIDMode="Static" 
            NavigateUrl="~/Vistas/Indicadores.aspx">Subir Indicador</asp:HyperLink>
    </div>

    <div id="chart_div"></div>
    
    
    <div class="pie">
    
    </div>
</asp:Content>

