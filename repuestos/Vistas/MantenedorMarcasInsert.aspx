<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="MantenedorMarcasInsert.aspx.cs" Inherits="mantenedorMarcasInsert" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script language="javascript" type="text/javascript">

        $(document).ready(function () {

            $("#btnAgregar").click(function () {
                return validaBlancos();
            });

            function validaTxtMarca() {
                mje_mar = '';
                if ($("#txtNombreMarca").val() == "") { mje_mar = 'debe escribir nombre de la marca\n'; }
                return (mje_mar);
            } //fin valida codigo

            function validaTxtAbreviado() {
                mje_abr = '';
                if ($("#txtAbreviado").val() == "") { mje_abr = 'debe escribir abreviado marca\n'; }
                return (mje_abr);
            } //fin valida cantidad

            function validaTxtDescripcion() {
                mje_des = '';
                if ($("#txtDescripcion").val() == "") { mje_des = 'debe escribir descripcion de la marca\n'; }
                return (mje_des);
            } //fin valida codigo

            function validaTxtOrgVenta() {
                mje_org = '';
                if ($("#txtOrgVenta").val() == "") { mje_org = 'debe escribir organizacion venta\n'; }
                return (mje_org);
            } //fin valida codigo

            function validaBlancos() {
                validaTxtMarca();
                validaTxtAbreviado();
                validaTxtDescripcion();
                validaTxtOrgVenta();
                mensaje = '';
                mensaje = mje_mar + mje_abr + mje_des + mje_org;
                if (mensaje != '') {
                    alert(mensaje);
                    return false;
                }
                else {
                    return true;
                }
            } //fin validaBlancos

        });


       

</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
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
	<div class="center">
        <div class="avisoInsert">
            <asp:Label ID="lblAviso" runat="server" ClientIDMode="Static" ForeColor="#99CCFF" ></asp:Label>
        </div>
        <asp:Table ID="tblDatos" runat="server" ClientIDMode="Static">
            <asp:TableRow ID="TableRow1" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell1" runat="server" ClientIDMode="Static">
                    Nombre :
                </asp:TableCell>
                <asp:TableCell ID="TableCell2" runat="server" ClientIDMode="Static">
                    <asp:TextBox MaxLength="25" id="txtNombreMarca" runat="server" ClientIDMode="Static"></asp:TextBox>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow2" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell3" runat="server" ClientIDMode="Static">
                    Abreviado :
                </asp:TableCell>
                <asp:TableCell ID="TableCell4" runat="server" ClientIDMode="Static">
                    <asp:TextBox MaxLength="25" id="txtAbreviado" runat="server" ClientIDMode="Static"></asp:TextBox>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow3" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell5" runat="server" ClientIDMode="Static">
                    Descripcion :
                </asp:TableCell>
                <asp:TableCell ID="TableCell6" runat="server" ClientIDMode="Static">
                    <asp:TextBox ID="txtDescripcion" runat="server" ClientIDMode="Static">
                    </asp:TextBox>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow4" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell7" runat="server" ClientIDMode="Static">
                    Org. venta :
                </asp:TableCell>
                <asp:TableCell ID="TableCell8" runat="server" ClientIDMode="Static">
                    <asp:TextBox MaxLength="4" id="txtOrgVenta" runat="server" ClientIDMode="Static"></asp:TextBox>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>
                    Grupo de material :
                </asp:TableCell>
                <asp:TableCell>
                    <asp:TextBox ID="txtGrupoMaterial" MaxLength="3" runat="server"></asp:TextBox>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>
                    Código de compañía :
                </asp:TableCell>
                <asp:TableCell>
                    <asp:TextBox ID="txtCodigoCompania" MaxLength="2" runat="server"></asp:TextBox>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>
                    Prefijo :
                </asp:TableCell>
                <asp:TableCell>
                    <asp:TextBox ID="txtPrefijo" MaxLength="4" runat="server"></asp:TextBox>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>
                    ¿Es foráneo? :
                </asp:TableCell>
                <asp:TableCell>
                    <asp:CheckBox ID="chkEsForaneo" runat="server" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow5" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell9" runat="server" ClientIDMode="Static"></asp:TableCell>
                <asp:TableCell ID="TableCell10" runat="server" ClientIDMode="Static"><asp:Button ID="btnAgregar" runat="server" ClientIDMode="Static" Text="Aceptar" onclick="btnAgregar_Click"/>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </div>
    <div class="footDer">
    <asp:HyperLink ID="hlkSucursal" runat="server" ClientIDMode="Static" 
            NavigateUrl="~/Vistas/mantenedorMarcas.aspx">Ir a Marcas</asp:HyperLink><br/>
    </div>
</asp:Content>

