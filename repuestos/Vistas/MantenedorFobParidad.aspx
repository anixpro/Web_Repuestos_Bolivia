<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="MantenedorFobParidad.aspx.cs" Inherits="Vistas_MantenedorFobParidad" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script language="javascript">
        function isNumberKey(evt, obj) {
            var charCode = (evt.which) ? evt.which : evt.keyCode;

            var value = obj.value;
            var dotcontains = value.indexOf(",") != -1;
            if (dotcontains)
                if (charCode == 44) return false;
            if (charCode == 44) return true;

            if (charCode > 31 && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="titulo">
        Mantenedor paridad
    </div>

    <div id="mjsError" runat="server" clientidmode="Static" style="background-color:coral">
    </div>

    <div class="mantenedorConcesionariosIzquierda">
        <center><b><u>Opciones</u></b></center>
        <ul>
            <li>
                <asp:HyperLink ID="HyperLink1" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/MantenedorFobCargaMasiva.aspx">Carga masiva</asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="HyperLink6" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/MantenedorFobValores.aspx">Mantenedor SKU</asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="HyperLink2" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/MantenedorFobParidad.aspx">Mantenedor Paridad</asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="HyperLink3" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/MantenedorFobVolumenes.aspx">Mantenedor Volumenes</asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="HyperLink4" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/MantenedorFobFactorUtilidad.aspx">Mantenedor factores de utilidad</asp:HyperLink>
            </li>
        </ul>
    </div>

    <div class="mantenedorConcesionariosDerecha">
         <b><u>Registrar valores de monedas</u></b>
        <asp:Table ID="Table1" runat="server" ClientIDMode="Static" Width="95%">

            <asp:TableRow ID="TableRow1" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell3" runat="server" ClientIDMode="Static">
                    Moneda :
                </asp:TableCell>
                <asp:TableCell ID="TableCell4" runat="server" ClientIDMode="Static">
                    <asp:DropDownList ID="ddlMoneda" runat="server"></asp:DropDownList>
                </asp:TableCell>
                 <asp:TableCell>
                    <asp:Button ID="btnVer" runat="server" Text="Ver" OnClick="btnVer_Click" />
                </asp:TableCell>
           </asp:TableRow>

            <asp:TableRow ID="TableRow2" runat="server" ClientIDMode="Static">

                <asp:TableCell ID="TableCell1" runat="server" ClientIDMode="Static">
                    Sigla :
                </asp:TableCell>
                <asp:TableCell ID="TableCell2" runat="server" ClientIDMode="Static">
                    <asp:TextBox ID="txtSigla" runat="server" ClientIDMode="Static" MaxLength="5"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell ID="TableCell10" runat="server" ClientIDMode="Static">
                    <asp:Label ID="lblSigla" runat="server" Text="" Visible="true"></asp:Label>
                </asp:TableCell>

            </asp:TableRow>

            <asp:TableRow ID="TableRow3" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell5" runat="server" ClientIDMode="Static">
                    Valor :
                </asp:TableCell>
                <asp:TableCell ID="TableCell6" runat="server" ClientIDMode="Static">
                    <asp:TextBox ID="txtValor" runat="server" ClientIDMode="Static" MaxLength="7" onkeypress="return isNumberKey(event, this)"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell ID="TableCell11" runat="server" ClientIDMode="Static">
                    <asp:Label ID="lblValor" runat="server" Text="" Visible="true"></asp:Label>
                </asp:TableCell>
            </asp:TableRow>

            <asp:TableRow ID="TableRow4" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell7" runat="server" ClientIDMode="Static" HorizontalAlign="Left">
                    <asp:Button ID="btnGrabar" runat="server" Text="Grabar" OnClick="btnGrabar_Click" />
                </asp:TableCell>
                <asp:TableCell>
                    &nbsp;
                </asp:TableCell>
                <asp:TableCell ID="TableCell9" runat="server" ClientIDMode="Static" HorizontalAlign="Right">
                    <asp:Button ID="btnVerLog" runat="server" Text="Ver Log" OnClick="btnVerLog_Click" />
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>

        <br />

        <asp:Table ID="Table2" runat="server" ClientIDMode="Static" Width="95%">
            <asp:TableRow ID="TableRow5" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell8" runat="server" ClientIDMode="Static">
                    <asp:GridView ID="dgvMonedas" runat="server" EmptyDataText="No se encontraron registros"
                    AutoGenerateColumns="false" AllowPaging="false" PageSize="20" Width="100%">
                    <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                    <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                    <HeaderStyle BackColor="#004B95" Font-Bold="True" ForeColor="White" Font-Names="Tahoma"
                    Font-Size="Small" />
                        <Columns>
                            <asp:BoundField DataField="descripcion" HeaderText="Moneda" InsertVisible="False" ReadOnly="True" />
                            <asp:BoundField DataField="valor" HeaderText="Valor" InsertVisible="False" ReadOnly="True" />
                            <asp:BoundField DataField="sigla" HeaderText="Sigla" InsertVisible="False" ReadOnly="True" />
                        </Columns>
                    </asp:GridView>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>

        <br />

        <asp:Table ID="Table3" runat="server" ClientIDMode="Static" Width="95%">
            <asp:TableRow>
                <asp:TableCell>
                    <asp:GridView ID="grvLog" runat="server" EmptyDataText="No se encontraron registros"
                    AutoGenerateColumns="false" AllowPaging="false" PageSize="5" Width="100%">
                    <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                    <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                    <HeaderStyle BackColor="#004B95" Font-Bold="True" ForeColor="White" Font-Names="Tahoma"
                    Font-Size="Small" />
                        <Columns>
                            <asp:BoundField DataField="nombre" HeaderText="Usuario" InsertVisible="False" ReadOnly="True" />
                            <asp:BoundField DataField="fecha" HeaderText="Fecha" InsertVisible="False" ReadOnly="True" dataformatstring="{0:dd-MM-yy}" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="hora" HeaderText="Hora" InsertVisible="False" ReadOnly="True" />
                            <asp:BoundField DataField="moneda" HeaderText="Moneda" InsertVisible="False" ReadOnly="True" />
                            <asp:BoundField DataField="valor_inicial" HeaderText="$ vigente" InsertVisible="False" ReadOnly="True" />
                            <asp:BoundField DataField="sigla_inicial" HeaderText="Vigente" InsertVisible="False" ReadOnly="True" />
                            <asp:BoundField DataField="valor_cambio" HeaderText="$ reemplazo" InsertVisible="False" ReadOnly="True" />
                            <asp:BoundField DataField="sigla_cambio" HeaderText="Reemplazo" InsertVisible="False" ReadOnly="True" />
                        </Columns>
                    </asp:GridView>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>

   </div>
</asp:Content>

