<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="MantenedorUsuarioUpdate.aspx.cs" Inherits="Vistas_mantenedorUsuarioUpdate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script language="javascript" type="text/javascript">

        $(document).ready(function () {

            ocultarCampos();
          
            $("#ddlTipoUsuario").change(function () {
                ocultarCampos();
            }); //evento change de select

            $("#btnAgregar").click(function () {
                return validaBlancosq();

            }); //click

            //ocultaCampos
            function ocultarCampos() {
                var index = 0;
                index = $("option:selected", "#ddlTipoUsuario").index();
                switch (index) {
                    case 0:
                        $("#rowCotizador").hide();
                        $("#rowConcesionario").hide();
                        $("#rowSucursal").hide();
                        $("#rowMultiple").hide();

                        break;
                    case 1:
                        $("#rowCotizador").hide();
                        $("#rowConcesionario").show();
                        $("#rowMultiple").hide();
                        $("#rowSucursal").show();

                        break;
                    case 2:
                        $("#rowCotizador").hide();
                        $("#rowConcesionario").show();
                        $("#rowMultiple").show();
                        $("#rowSucursal").show();

                        break;
                    case 3:
                        $("#rowCotizador").show();
                        $("#rowConcesionario").show();
                        $("#rowMultiple").hide();
                        $("#rowSucursal").show();

                        break;
                    default: break;
                } //fin switch
            }

            //funciones para validar campos vacios
            function validaTxtNombre() {
                $("#lblNombre").css("color", "#696969");
                cajaTexto = $("#txtNombre").val();
                mje_nom = '';
                if (cajaTexto == '') {
                    mje_nom = 'Debe completar el campo nombre\n';
                    $("#lblNombre").css("color", "red");
                }
                else if (!isNaN(cajaTexto)) {
                    mje_nom = 'Solo letras en el campo nombre\n';
                    $("#lblNombre").css("color", "red");
                    $("#txtNombre").val("");
                }
                else if (cajaTexto.length < 4 || cajaTexto.length > 50) {
                    mje_nom = 'Nombre entre 4 y 50 caracteres\n';
                    $("#lblNombre").css("color", "red");
                }
                return (mje_nom);
            } //fin validaNombre

            function validaTxtEmail() {
                $("#lblEmail").css("color", "#696969");
                cajaTexto = $("#txtEmail").val();
                mje_email = '';
                if (cajaTexto == "") {
                    mje_email = 'Debe completar correo electronico\n';
                    $("#lblEmail").css("color", "red");
                    $("#txtEmail").val("");

                }

                return (mje_email);
            } //fin validaEmail

            function validaTxtTelefono() {
                $("#lblTelefono").css("color", "#696969");
                cajaTexto = $("#txtTelefono").val();
                mje_fono = '';
                if (cajaTexto == "") {
                    mje_fono = 'Debe completar Telefono\n';
                    $("#lblTelefono").css("color", "red");
                    $("#txtTelefono").val("");

                }

                return (mje_fono);
            } //fin validaFono

            function validaConcesionario() {
                $("#lblConcesionario").css("color", "#696969");
                mje_con = '';
                if ($("option:selected", "#ddlTipoUsuario").index() == 1 || $("option:selected", "#ddlTipoUsuario").index() == 2) {
                    if ($("option:selected", "#ddlConcesionario").index() == 0) {
                        mje_con = 'Debe seleccionar concesionario y sucursal\n';
                        $("#lblConcesionario").css("color", "red");
                    }
                }
                return (mje_con);
            } //fin concesionario

             function validaTxtRut() {

                $("#lblRut").css("color", "#696969");
                cajaTexto = $("#txtRut").val();
                mje_rut = '';
                if (cajaTexto == "") {
                    mje_rut = 'Debe completar el RUT\n';
                    $("#lblRut").css("color", "red");
                    $("#txtRut").val("");
                }
                else if (isNaN(cajaTexto)) {
                    mje_rut = 'Ingrese RUT válido\n';
                    $("#lblRut").css("color", "red");
                    $("#txtRut").val("");
                }

                return (mje_rut);
            } //fin validaRut

            function validaBlancosq() {
                validaTxtNombre();
                validaTxtTelefono();
                validaTxtEmail();
                validaConcesionario();

                mensaje = '';
                mensaje = mje_nom + mje_email + mje_fono + mje_con;
                if (mensaje != '') {
                    alert(mensaje);
                    return false;
                }
                else {
                    return true;
                }
            } //fin validaBlancos

        });   //fin jquery

        function validaTxtRutBusqueda() {

            document.getElementById('lblRut').style.color = "#696969";
            cajaTexto = document.getElementById('txtRut').value;
            mje_rut = '';
            if (cajaTexto == "") {
                mje_rut = 'Favor, complete el RUT\n';
                document.getElementById('lblRut').style.color = "red";
                document.getElementById('txtRut').value = '';
            }
            else if (isNaN(cajaTexto)) {
                mje_rut = 'Ingrese un RUT válido\n';
                document.getElementById('lblRut').style.color = "red";
                document.getElementById('txtRut').value = '';
            }

            return (mje_rut);
        }

        function confirmar(rut) {

            //alert(rut);
            var pregunta = confirm("¿Está seguro?");
            if (pregunta) {
                return location.href="mantenedorUsuarioUpdate.aspx?ev=elimina&&rut="+rut;
            }
            else 
            {
                return false
            }
        }
       
        function validaBlancos() {
  
            validaTxtRutBusqueda();
   
            mensaje = '';
            mensaje =mje_rut;
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
        Mantenedor de Usuarios: Modificando usuario
    </div>
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <div class="avisoInsert">
        <asp:Label ID="lblAviso" runat="server" ClientIDMode="Static" ForeColor="#99CCFF" ></asp:Label>
    </div>
    <div class="center">
        <asp:Table ID="Table1" runat="server" ClientIDMode="Static">
            <asp:TableRow ID="TableRow1" runat="server" ClientIDMode="Static" HorizontalAlign="Center">
                <asp:TableCell ID="TableCell1" runat="server" ClientIDMode="Static"><asp:Label ID="lblRut" runat="server" ClientIDMode="Static">Rut</asp:Label></asp:TableCell>
                <asp:TableCell ID="TableCell2" runat="server" ClientIDMode="Static"><asp:TextBox ID="txtRut" runat="server" ClientIDMode="Static"></asp:TextBox></asp:TableCell>
                <asp:TableCell><asp:Button ID="btnBuscar" runat="server" ClientIDMode="Static" Text="Buscar" OnClick="btnBuscar_Click"/></asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </div>
    <center>
        <b>- Modifique los campos: -</b>
    </center>
    <asp:SqlDataSource ID="sqldPersona" runat="server" 
        ConnectionString="<%$ ConnectionStrings:skbergeConnectionString %>" ></asp:SqlDataSource>
    <div class="usuarioUpdate">
        <asp:HiddenField ID="hddIdHumano2" runat="server"/>
        <asp:Table ID="tblDatos" runat="server" ClientIDMode="Static" Height="202px" 
            Width="283px">
            <asp:TableRow ID="rowConcesionario" runat="server" ClientIDMode="Static" style="display:none" CssClass="rowConcesionario">
                <asp:TableCell ID="TableCell5" runat="server" ClientIDMode="Static">
                    <asp:Label ID="lblConcesionario" runat="server" ClientIDMode="Static" Text="Concesionario" CssClass="lblConcesionario">
                    </asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="TableCell6" runat="server" ClientIDMode="Static">
                    <asp:DropDownList ID="ddlConcesionario" runat="server" onselectedindexchanged="ddlConcesionario_SelectedIndexChanged" ClientIDMode="Static"  AutoPostBack="True"> 
                    </asp:DropDownList>
                    <img alt="Ejemplo: 'PALD'" title="Debe ingresar nombre de concesionario. Ej: 'PALD'" src="../img/help.png" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="rowSucursal" runat="server" ClientIDMode="Static" style="display:none" CssClass="rowSucursal">
                <asp:TableCell ID="TableCell7" runat="server" ClientIDMode="Static">
                    <asp:Label ID="lblSucursal" runat="server" ClientIDMode="Static" Text="Sucursal" CssClass="lblSucursal" ></asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="TableCell8" runat="server" ClientIDMode="Static">
                    <asp:DropDownList ID="ddlSucursal" runat="server" ClientIDMode="Static" onselectedindexchanged="ddlConcesionario_SelectedIndexChanged" AutoPostBack="false">
                    </asp:DropDownList>
                    <img alt="Ejemplo: 'Oriente'" title="Debe ingresar nombre de sucursar. Ej: 'Sur'" src="../img/help.png" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="rowMultiple" runat="server" ClientIDMode="Static" style="display:none" CssClass="rowMultiple">
                <asp:TableCell ID="TableCell9" runat="server" ClientIDMode="Static">
                    <asp:Label ID="lblMultiple" runat="server" ClientIDMode="Static" Text="multiSucursal" CssClass="lblMultiple"></asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="TableCell10" runat="server" ClientIDMode="Static">
                    <asp:CheckBox ID="cbxMultipleSucursal" runat="server" ClientIDMode="Static" CssClass="cbxMultipleSucursal" />
                    <img alt="Multiple sucursal" title="Marque esta opcion si deseea que la persona tenga la capacidad de generar cotizacion a diferentes sucursales" src="../img/help.png" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow runat="server" ClientIDMode="Static">
                <asp:TableCell runat="server" ClientIDMode="Static"><asp:Label ID="lblTipo" runat="server" ClientIDMode="Static" Text="Tipo Usuario" CssClass="lblTipo"></asp:Label>
                </asp:TableCell>
                <asp:TableCell runat="server" ClientIDMode="Static">
                    <asp:DropDownList ID="ddlTipoUsuario" runat="server" ClientIDMode="Static" CssClass="selectConcesionario">
                        <asp:ListItem Value="1">Administrador</asp:ListItem>
                        <asp:ListItem Value="2">Gerente</asp:ListItem>
                        <asp:ListItem Value="3">Operario</asp:ListItem>
                        <asp:ListItem Value="4">Cotizador</asp:ListItem>
                        <asp:ListItem Value="5">Supervisor</asp:ListItem>
                        <asp:ListItem Value="6">Administrador Noticias</asp:ListItem>
                    </asp:DropDownList>
                    <img alt="Ejemplo: 'Cotizador'" title="Escoja tipo de usuario. Estos pueden ser Cotizador, Genrente, " src="../img/help.png" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow runat="server" ClientIDMode="Static">
                <asp:TableCell runat="server" ClientIDMode="Static">
                    <asp:Label ID="lblNombre" runat="server" ClientIDMode="Static" Text="Nombre" CssClass="lblNombre"></asp:Label>
                </asp:TableCell>
                <asp:TableCell runat="server" ClientIDMode="Static">
                    <asp:TextBox MaxLength="50" ID="txtNombre" runat="server" ClientIDMode="Static" CssClass="txtNombre"></asp:TextBox>
                    <img alt="Ejemplo: 'Daniela'" title="Escriba un nombre. Ejemplo:'Juan'" src="../img/help.png" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow runat="server" ClientIDMode="Static">
                <asp:TableCell runat="server" ClientIDMode="Static">
                    <asp:Label ID="lblEmail" runat="server" ClientIDMode="Static" Text="E- Mail" CssClass="lblEmail"></asp:Label>
                </asp:TableCell>
                <asp:TableCell runat="server" ClientIDMode="Static">
                    <asp:TextBox MaxLength="100" ID="txtEmail" runat="server" ClientIDMode="Static" CssClass="txtEmail"></asp:TextBox>
                    <img alt="Ejemplo: 'yo@concesionario.cl'" title="Concesionario. Ejemplo:'yo@concesionario.cl'" src="../img/help.png" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow runat="server" ClientIDMode="Static">
                <asp:TableCell runat="server" ClientIDMode="Static">
                    Telefono
                </asp:TableCell>
                <asp:TableCell runat="server" ClientIDMode="Static">
                    <asp:TextBox MaxLength="5" ID="txtCodigoArea" runat="server" ClientIDMode="Static" Width="23" CssClass="txtTelefono"></asp:TextBox><asp:Label ID="lblGuionFono" runat="server" ClientIDMode="Static" Text="-"></asp:Label>
                        <asp:TextBox MaxLength="10" ID="txtTelefono" runat="server" ClientIDMode="Static" Width="110" CssClass="txtTelefono">
                        </asp:TextBox>
                    <img alt="Ejemplo: '2-5564338'" title="Teléfono. Ejemplo:'2 5563438'" src="../img/help.png" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>
                    Contraseña
                </asp:TableCell>
                <asp:TableCell>
                    <asp:TextBox runat="server" ID="txtContrasena" MaxLength="15"  Enabled="true"/>
                </asp:TableCell></asp:TableRow><asp:TableRow>
                <asp:TableCell runat="server" ClientIDMode="Static">
                    <asp:Label ID="lblidllave" runat="server" ClientIDMode="Static" Text="Id Llave" CssClass="lblidllave"></asp:Label>
                </asp:TableCell><asp:TableCell ID="TableCell3" runat="server" ClientIDMode="Static">
                    <asp:TextBox MaxLength="50" ID="txtIdllave" runat="server" ClientIDMode="Static" CssClass="txtidllave"></asp:TextBox>
                    <img alt="Ejemplo: '12233423'" title="Escriba un id. Ejemplo:'7893302'" src="../img/help.png" />
                </asp:TableCell></asp:TableRow><asp:TableRow>
                <asp:TableCell>
                    
                    Habilitado
                </asp:TableCell><asp:TableCell>
                    <asp:CheckBox ID="chkHabilitado" runat="server" />
                </asp:TableCell></asp:TableRow><asp:TableRow runat="server" ClientIDMode="Static">
                <asp:TableCell runat="server" ClientIDMode="Static" ColumnSpan="2" HorizontalAlign="Center">
                    <asp:Button ID="btnAgregar" runat="server" ClientIDMode="Static" Text="Aceptar" CssClass="button" onclick="btnAgregar_Click"/>
                    &nbsp;
                    <asp:Button ID="btnVolver" runat="server" ClientIDMode="Static" Text="Volver" CssClass="button" onclick="btnVolver_Click"/>
                </asp:TableCell></asp:TableRow></asp:Table></div></asp:Content>