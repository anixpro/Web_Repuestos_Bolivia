<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="MantenedorModelo.aspx.cs" Inherits="mantenedorModelo" %>

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
        function confirmar(modelo, marca) {
            var pregunta = confirm("¿Está seguro de eliminar " + modelo + " y todos sus datos?");
            if (pregunta) {
                return location.href = "mantenedorModelo.aspx?marca="+marca+"&&ev=elimina&&modelo=" + modelo;
            }
            else {
                return false
            }
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
        Lista de modelos
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
    <div class="center"><br/>
            <asp:Table ID="Table1" runat="server" ClientIDMode="Static">
            <asp:TableRow ID="TableRow1" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell1" runat="server" ClientIDMode="Static"><asp:Label ID="lblModelo" runat="server" ClientIDMode="Static">Modelo</asp:Label></asp:TableCell>
                <asp:TableCell ID="TableCell2" runat="server" ClientIDMode="Static"><asp:TextBox ID="txtModelo" runat="server" ClientIDMode="Static"></asp:TextBox></asp:TableCell>
                <asp:TableCell><asp:Button ID="btnBuscar" runat="server" ClientIDMode="Static" Text="Buscar" OnClick="btnBuscar_Click"/></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow2" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell11" runat="server" ClientIDMode="Static">
                    <asp:Label ID="lblMarca" runat="server" ClientIDMode="Static">Marca</asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="TableCell22" runat="server" ClientIDMode="Static" ColumnSpan="2">
                    <asp:DropDownList ID="ddlMarca" runat="server" ClientIDMode="Static" onselectedindexchanged="ddlMarca_SelectedIndexChanged" AutoPostBack="True"></asp:DropDownList>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </div>
    <div class="center">
    <asp:GridView ID="gridMarcas" runat="server" ClientIDMode="Static" AllowPaging="True" AllowSorting="True"
        AutoGenerateColumns="False" DataSourceID="sqldPersona"
        EnableModelValidation="True" Style="text-align:center;"
        EmptyDataText="No hay registros en este estado" CellPadding="10" 
        ForeColor="#333333" GridLines="None" >
        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
        <Columns>
            <asp:BoundField DataField="marca" HeaderText="marca" />
            <asp:BoundField DataField="modelo" HeaderText="modelo" />
            <asp:BoundField DataField="descripcion" HeaderText="descripcion"/>                   
            <asp:TemplateField FooterText="Eliminar" HeaderText="Eliminar">
            <ItemTemplate>
            <a onclick="javascript:confirmar('<%# Eval("modelo") %>','<%# Eval("marca") %>');" href="#" ><img alt="Eliminar" src="../img/cross.png" /></a>
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
    <asp:SqlDataSource ID="sqldPersona" runat="server" 
        ConnectionString="<%$ ConnectionStrings:skbergeConnectionString %>" ></asp:SqlDataSource>
</asp:Content>

