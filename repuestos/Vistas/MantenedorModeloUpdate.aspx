<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="MantenedorModeloUpdate.aspx.cs" Inherits="mantenedorModeloUpdate" %>

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

        function confirmar(modelo) {

            //alert(rut);
            var pregunta = confirm("¿Está seguro de eliminar " + modelo + " y todos sus datos?");
            if (pregunta) {
                return location.href = "mantenedorModelo.aspx?ev=elimina&&modelo=" + modelo;
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
                return location.href = "";
            }
            else {
                return true
            }
        }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="title">
        Mantenedor modelo
    </div>
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <div class="avisoInsert">
            <asp:Label ID="lblAviso" runat="server" ClientIDMode="Static" ForeColor="#99CCFF" ></asp:Label>
        </div>
    <div class="center"><br/>
        <asp:Table ID="Table1" runat="server" ClientIDMode="Static">
            <asp:TableRow ID="TableRow1" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell1" runat="server" ClientIDMode="Static"><asp:Label ID="lblModelo" runat="server" ClientIDMode="Static">Modelo</asp:Label></asp:TableCell>
                <asp:TableCell ID="TableCell2" runat="server" ClientIDMode="Static"><asp:TextBox ID="txtModelo" runat="server" ClientIDMode="Static"></asp:TextBox></asp:TableCell>
                <asp:TableCell><asp:Button ID="btnBuscar" runat="server" ClientIDMode="Static" Text="Buscar" OnClick="btnBuscar_Click"/></asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </div>
    <div class="center">
        <asp:GridView ID="gridMarcas" runat="server" ClientIDMode="Static" AllowPaging="True" AllowSorting="True"
                AutoGenerateColumns="False" DataSourceID="sqldPersona"
                EnableModelValidation="True" Style="text-align:center;"
                CellPadding="10" ForeColor="#333333" GridLines="None" >
                 <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                 <Columns >
                   <asp:BoundField DataField="marca" HeaderText="marca" />
                   <asp:HyperLinkField DataTextField="modelo" HeaderText="modelo"  DataNavigateUrlFields="modelo" DataNavigateUrlFormatString="mantenedorModeloUpdate.aspx?modelo={0}" />
                   <asp:BoundField DataField="descripcion" HeaderText="descripcion"/>
                   
                     <asp:TemplateField FooterText="Eliminar" HeaderText="Eliminar">
                     <ItemTemplate>
                     <a onclick="javascript:confirmar('<%# Eval("modelo") %>');" href="#" >Eliminar</a>
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
                <div class="center">
                    <asp:Table ID="tblDatos" runat="server" ClientIDMode="Static">
        <asp:TableRow ID="rowDireccion" runat="server" ClientIDMode="Static">
            <asp:TableCell ID="t12" runat="server" ClientIDMode="Static"><asp:Label ID="lblEmail" runat="server" ClientIDMode="Static" Text="Nuevo Email" ></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="t13" runat="server" ClientIDMode="Static"><asp:TextBox MaxLength="30" ID="txtEmail" runat="server" ClientIDMode="Static"></asp:TextBox>
            </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow6" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell13" runat="server" ClientIDMode="Static">
                    <asp:Label ID="lblTelefono" runat="server" ClientIDMode="Static" Text="Nuevo telefono"></asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="TableCell14" runat="server" ClientIDMode="Static">
                    <asp:TextBox MaxLength="8" ID="txtTelefono" runat="server" ClientIDMode="Static"></asp:TextBox>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow7" runat="server" ClientIDMode="Static">
            <asp:TableCell ID="TableCell15" runat="server" ClientIDMode="Static"></asp:TableCell>
            <asp:TableCell ID="TableCell16" runat="server" ClientIDMode="Static"><asp:Button ID="btnAgregar" runat="server" ClientIDMode="Static" Text="Aceptar" />
            </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </div>
    <div class="footDer">
         <asp:HyperLink ID="hlkConcesionario" runat="server" ClientIDMode="Static" 
            NavigateUrl="~/Vistas/mantenedorConcesionario.aspx">Ir a concesionario</asp:HyperLink>
    </div>
</asp:Content>

