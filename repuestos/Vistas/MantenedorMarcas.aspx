<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="MantenedorMarcas.aspx.cs" Inherits="mantenedorMarcas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script language="javascript" type="text/javascript">
        function validaTxtRut() {
            document.getElementById('ctl00_MainContent_lblRut').style.color = "#696969";
            cajaTexto = document.getElementById('ctl00_MainContent_txtRut').value;
            mje_rut = '';
            if (cajaTexto == "") {
                mje_rut = 'debe llenar Rut\n';
                document.getElementById('ctl00_MainContent_lblRut').style.color = "red";
                document.getElementById('ctl00_MainContent_txtRut').value = '';
            }
            else if (isNaN(cajaTexto)) {
                mje_rut = 'solo numeros en Rut\n';
                document.getElementById('ctl00_MainContent_lblRut').style.color = "red";
                document.getElementById('ctl00_MainContent_txtRut').value = '';
            }
            else {
                var rut = document.getElementById('ctl00_MainContent_txtRut').value; ;
                var largo = rut.length;
                var i = 0;
                var dv = document.getElementById('ctl00_MainContent_txtDvr').value; ;
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
                    document.getElementById('ctl00_MainContent_lblRut').style.color = "red";
                    document.getElementById('ctl00_MainContent_txtRut').value = '';
                }
            }
            return (mje_rut);
        }
        function confirmar(marca) {

            //alert(rut);
            var pregunta = confirm("¿Está seguro de eliminar "+marca+" y todos sus datos?");
            if (pregunta) {
                return location.href = "mantenedorMarcas.aspx?ev=elimina&&marca=" + marca;
            }
            else {
                return false
            }
        }
            // REQ - Cotizaciones Automaticas Marzo 2022 
        function habDes(abreviado) {
        //alert(abreviado);
        var porciones = abreviado.split(',');
        var prefijo = porciones[0];
        var marca = porciones[1];
        //alert(marca);
        //alert(abreviado);
        return location.href = "mantenedorMarcas.aspx?ev=cotiAuto&&marca=" + marca+"&&abreviado="+prefijo;
        }


        function validaBlancos() {

            validaTxtRut();

            mensaje = '';
            mensaje = mje_rut;
            if (mensaje != '') {
                window.alert(mensaje);
                return false;
            }
            else {
                return true
            }
        }

</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="titulo">
        Mantenedor de marcas: Lista de marcas
    </div>
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <div class="mantenedorConcesionariosIzquierda">
        <center><b><u>Mantención marcas</u></b></center>
        <ul>
            <li>
                <asp:HyperLink ID="HyperLink5" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/mantenedorMarcasInsert.aspx">Insertar marca</asp:HyperLink><br />    
            </li>
            <li>
                <asp:HyperLink ID="HyperLink6" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/mantenedorModelo.aspx">Consultar modelos</asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="HyperLink1" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/mantenedorModeloInsert.aspx">Insertar modelo</asp:HyperLink>
            </li>
        </ul>
    </div>
    <div class="mantenedorConcesionariosDerecha">
        <b><u>Lista marcas, filtradas por concesionario</u></b>
        <asp:Table ID="Table1" runat="server" ClientIDMode="Static">
            <asp:TableRow ID="TableRow1" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell1" runat="server" ClientIDMode="Static">
                    Nombre :
                </asp:TableCell>
                <asp:TableCell ID="TableCell2" runat="server" ClientIDMode="Static">
                    <asp:TextBox ID="txtMarca" runat="server" ClientIDMode="Static"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell>
                    <asp:Button ID="btnBuscar" runat="server" ClientIDMode="Static" Text="Buscar" OnClick="btnBuscar_Click" CssClass="button"/>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow2" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell11" runat="server" ClientIDMode="Static">
                    Concesionario :
                </asp:TableCell>
                <asp:TableCell ID="TableCell22" runat="server" ClientIDMode="Static" ColumnSpan="2">
                    <asp:DropDownList ID="ddlConcesionario" runat="server" ClientIDMode="Static" onselectedindexchanged="ddlConcesionario_SelectedIndexChanged" AutoPostBack="True"></asp:DropDownList>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <asp:GridView ID="gridMarcas" runat="server" ClientIDMode="Static" AllowPaging="True" AllowSorting="True"
            AutoGenerateColumns="False"  
            EnableModelValidation="True" Style="text-align:center;"
            EmptyDataText="No hay registros en este estado" CellPadding="10" 
            ForeColor="#333333" GridLines="None" CssClass="tablaMarcas" PagerSettings-Mode="NextPreviousFirstLast">
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <Columns>
                <asp:BoundField DataField="abreviado" HeaderText="Abreviación de marca"/>
                <asp:BoundField DataField="marca" HeaderText="Marca"/>
                <asp:BoundField DataField="descripcion" HeaderText="Descripción"/>
                <asp:BoundField DataField="orgVentas" HeaderText="Org. de Venta"/>
                <%-- // REQ - Cotizaciones Automaticas Marzo 2022   --%>
                 <asp:BoundField DataField="cotizacionAutomatica" HeaderText="¿Cotización automática?" ReadOnly="True"/>
                <asp:TemplateField HeaderText="Act/Desc">
                    <ItemTemplate>
                        <a onclick="javascript:habDes('<%# Eval("abreviado") + "," + Eval("marca") %>');" href="#" ><img src="../img/cambia2.jpg" /></a>
                    </ItemTemplate>
                </asp:TemplateField>
                  <%-- // REQ - Cotizaciones Automaticas Marzo 2022   --%>
                <asp:TemplateField FooterText="Eliminar" HeaderText="Eliminar">
                    <ItemTemplate>
                        <a onclick="javascript:confirmar('<%# Eval("marca") %>');" href="#" ><img alt='Eliminar' src="../img/cross.png" /></a>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <EditRowStyle BackColor="#999999" />
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
        </asp:GridView>
    </div>
    <asp:SqlDataSource ID="sqldPersona" runat="server" ConnectionString="<%$ ConnectionStrings:skbergeConnectionString %>" ></asp:SqlDataSource>
</asp:Content>
