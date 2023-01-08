<%@ Page Title="" Language="C#" Debug="true" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="EditorAccesorioCantidad.aspx.cs" Inherits="EditorAccesorioCantidad" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script language="javascript" type="text/javascript">
    /*function AcceptNum(evt){
        var nav4 = window.Event ? true : false;

        var key = nav4 ? evt.which : evt.keyCode;

        return (key <= 13 || (key >= 48 && key <= 57) || key == 44);

    }*/

    function saveCant(id) {
        var cant = $("#repuesto-" + id).val();
        var actionData = { id: id, cant: cant };
        $.getJSON('ajax/actualizaCantidad.aspx', actionData, function (data) {
            if (data == true) {
                alert('Guardado con exito');
            }
            else if (data == false) {
                alert('Error al guardar');
            }
        });
    }

    function validaBlancos() {
        if ($("#txtCodigo").val() != "")
            return true;
        else return false;
    }          
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="titulo">
        Cantidades Minimas de Adquisicion de Repuestos: Lista de Repuestos
    </div>
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <div class="mantenedorConcesionariosDerecha">
        <asp:Table ID="Table1" runat="server" ClientIDMode="Static">
			<asp:TableRow ID="TableRow2" runat="server" ClientIDMode="Static">
				<asp:TableCell ID="TableCell11" runat="server" ClientIDMode="Static">
                    Marcas :
                </asp:TableCell>
                <asp:TableCell ID="TableCell22" runat="server" ClientIDMode="Static" ColumnSpan="2">
                    <asp:DropDownList ID="ddlMarca" runat="server" ClientIDMode="Static" onselectedindexchanged="ddlMarca_SelectedIndexChanged" AutoPostBack="True"></asp:DropDownList>
                </asp:TableCell>
				<asp:TableCell ID="TableCell1" runat="server" ClientIDMode="Static">
                    Codigo :
                </asp:TableCell>
                <asp:TableCell ID="TableCell2" runat="server" ClientIDMode="Static">
                    <asp:TextBox ID="txtCodigo" runat="server" ClientIDMode="Static"></asp:TextBox>
                </asp:TableCell>
				<asp:TableCell>
                    <asp:Button ID="btnBuscar" runat="server" ClientIDMode="Static" Text="Buscar" OnClick="btnBuscar_Click" CssClass="button"/>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <asp:GridView ID="gridRepuestos" runat="server" ClientIDMode="Static" AllowPaging="True" AllowSorting="True"
            AutoGenerateColumns="False" DataSourceID="sqldRepuestos"
            EnableModelValidation="True" Style="text-align:center;"
            EmptyDataText="Utilize el Buscador para encontrar el Repuesto que necesita" CellPadding="10" 
            ForeColor="#333333" GridLines="None" CssClass="tablaMarcas" PagerSettings-Mode="NextPreviousFirstLast">
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <Columns>
                <asp:BoundField DataField="marca" HeaderText="Marca"/>
                <asp:BoundField DataField="codigo" HeaderText="Codigo"/>
                <asp:TemplateField FooterText="Eliminar" HeaderText="Cantidad Minima de Adquisicion ">
                    <ItemTemplate>
                             <input type="text"  id='repuesto-<%# Eval("id") %>'  value='<%# Eval("cantidadMin") %>' size="2" maxlength="2" >
                             <img src="../img/save.png"   onclick='saveCant("<%# Eval("id") %>")' />
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
    <asp:SqlDataSource ID="sqldRepuestos" runat="server" ConnectionString="<%$ ConnectionStrings:skbergeConnectionString %>" ></asp:SqlDataSource>
</asp:Content>
