<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Mantenedor.aspx.cs" Inherits="Vistas_Mantenedor" EnableEventValidation="false"%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            $("#hdnSucursal").val($('option:selected', '#ddlSucursal').text());
            $("#hdnISucursal").val($('option:selected', '#ddlSucursal').index());
            $("#hdnValSucursal").val($('option:selected', '#ddlSucursal').val());

            //filtros de sucursal
            filtraSucursal($("option:selected", "#ddlConcesionario").text());

            $("#ddlConcesionario").change(function () {
                filtraSucursal($(this).val());
            }); //evento change de select

            $("#ddlSucursal").change(function () {

                $("#hdnSucursal").val($('option:selected', '#ddlSucursal').text());
                $("#hdnISucursal").val($('option:selected', '#ddlSucursal').index());
                $("#hdnValSucursal").val($('option:selected', '#ddlSucursal').val());

            }); //evento change de select

            function filtraSucursal(str) {
                if (str == "") {
                    $("#ddlSucursal").html("");
                    return;
                }

                // code for IE7+, Firefox, Chrome, Opera, Safari
                if (window.XMLHttpRequest) {
                    xmlhttp = new XMLHttpRequest();
                }

                // code for IE6, IE5
                else {
                    xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
                }
                xmlhttp.onreadystatechange = function () {
                    if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {
                        $("#ddlSucursal").html(xmlhttp.responseText);
                        $("#spnSucursal").html("");
                        $("#hdnSucursal").val($('option:selected', '#ddlSucursal').text());
                        $("#hdnISucursal").val($('option:selected', '#ddlSucursal').index());
                        $('#btnAgregar').removeAttr("disabled", "disabled");
                        $("#ddlSucursal").val('<%=_sucursal %>');
                    }
                    else {
                        $("#spnSucursal").html("Cargando...");
                        $('#btnAgregar').attr("disabled", "disabled");
                    }
                }
                xmlhttp.open("GET", "ajax/mantenedorAjax.aspx?op=1&&q=" + encodeURIComponent(str), true);
                xmlhttp.send();
            }
            //fin filtros

            ocultarCampos();
            //funcion que trnasforma la K a k
            $("#txtDvr").keyup(function () {
                if ($("#txtDvr").val() == 'K') {
                    $("#txtDvr").val('k');
                }
            });

            $("#ddlTipoUsuario").change(function () {
                ocultarCampos();
            });
            //evento change de select

            $("#btnAgregar").click(function () {
                return validaBlancos();
            });
            //click

            //ocultaCampos
            function ocultarCampos() {
                var index = 0;
                index = $("option:selected", "#ddlTipoUsuario").index();
                switch (index) {
                    // administrador        
                    case 0:
                        $("#rowConcesionario").hide();
                        $("#rowMultiple").hide();
                        $("#rowSucursal").hide();
                        $("#rowReemplazo").hide();
                        $("#rowAprobVFC").hide();
                        $("#rowCriticidadVfc").hide();
                        $("#rowDevRec").hide();
                        break;
                    // gerente        
                    case 1:
                        $("#rowConcesionario").show();
                        $("#rowMultiple").hide();
                        $("#rowSucursal").show();
                        $("#rowReemplazo").hide();
                        $("#rowAprobVFC").hide();
                        $("#rowCriticidadVfc").hide();
                        $("#rowDevRec").hide();
                        break;
                    // operador        
                    case 2:
                        $("#rowConcesionario").show();
                        $("#rowSucursal").show();
                        $("#rowMultiple").show();
                        $("#rowReemplazo").hide();
                        $("#rowAprobVFC").show();
                        $("#rowCriticidadVfc").show();
                        $("#rowDevRec").show();
                        break;
                    // cotizador        
                    case 3:
                        $("#rowConcesionario").show();
                        $("#rowMultiple").hide();
                        $("#rowSucursal").show();
                        $("#rowReemplazo").hide();
                        $("#rowAprobVFC").hide();
                        $("#rowCriticidadVfc").hide();
                        $("#rowDevRec").hide();
                        break;
                    //CDR
                    case 5:
                        $("#rowConcesionario").show();
                        $("#rowSucursal").show();
                        $("#rowMultiple").show();
                        $("#rowReemplazo").hide();
                        $("#rowAprobVFC").show();
                        $("#rowCriticidadVfc").show();
                        $("#rowDevRec").show();
                        break;
                    // supervisor      
                    case 4:
                        $("#rowConcesionario").hide();
                        $("#rowMultiple").hide();
                        $("#rowSucursal").hide();
                        $("#rowReemplazo").show();
                        $("#rowAprobVFC").hide();
                        $("#rowCriticidadVfc").hide();
                        $("#rowDevRec").hide();
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
                    mje_nom = 'Debe ingresar un nombre\n';
                    $("#lblNombre").css("color", "red");
                }
                else if (!isNaN(cajaTexto)) {
                    mje_nom = 'Campo "nombre" no acepta números \n';
                    $("#lblNombre").css("color", "red");
                    $("#txtNombre").val("");
                }
                else if (cajaTexto.length < 4 || cajaTexto.length > 50) {
                    mje_nom = 'Campo "nombre" debe contener entre 4 y 50 caracteres\n';
                    $("#lblNombre").css("color", "red");
                    $("#txtNombre").val("");
                }
                return (mje_nom);
            } //fin validaNombre

            function validaTxtRut() {
                $("#lblRut").css("color", "#696969");
                cajaTexto = $("#txtRut").val();
                mje_rut = '';
                if (cajaTexto == "") {
                    mje_rut = 'Debe ingresar un rut\n';
                    $("#lblRut").css("color", "red");
                    $("#txtRut").val("");
                }
                else if (isNaN(cajaTexto)) {
                    mje_rut = 'Campo "rut" solo acepta números\n';
                    $("#lblRut").css("color", "red");
                    $("#txtRut").val("");
                }
                /*else {
                    var rut = $("#txtRut").val();
                    var largo = rut.length;
                    var i = 0;
                    var dv = $("#txtDvr").val();
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
                        mje_rut = 'Rut erroneo\n';
                        $("#lblRut").css("color", "red");
                        $("#txtRut").val("");
                    }
                }*/
                return (mje_rut);
            }
            //fin validaRut

            function validaTxtEmail() {
                $("#lblEmail").css("color", "#696969");
                cajaTexto = $("#txtEmail").val();
                mje_email = '';
                if (cajaTexto == "") {
                    mje_email = 'Debe completar el campo "E-mail"\n';
                    $("#lblEmail").css("color", "red");
                    $("#txtEmail").val("");
                }
                return (mje_email);
            }
            //fin validaEmail

            function validaTxtTelefono() {
                $("#lblTelefono").css("color", "#696969");
                cajaTexto = $("#txtTelefono").val();
                mje_fono = '';
                if (cajaTexto == "") {
                    mje_fono = 'Debe completar el campo "teléfono"\n';
                    $("#lblTelefono").css("color", "red");
                    $("#txtTelefono").val("");
                }
                return (mje_fono);
            }
            //fin validaFono

            function validaTxtEpresa() {
                $("#lblEmpresaCotizadora").css("color", "#696969");
                cajaTexto = $("#txtEmpresaCotizadora").val();
                mje_emp = '';
                if ($("option:selected", "#ddlTipoUsuario").index() == 0) {
                    if (cajaTexto == "") {
                        mje_emp = 'Debe completar el campo "empresa"\n';
                        $("#lblEmpresaCotizadora").css("color", "red");
                        cajaTexto = $("#txtEmpresaCotizadora").val("");
                    }
                }
                return (mje_emp);
            }
            //fin validaEmpresa

            function validaConcesionario() {
                $("#lblConcesionario").css("color", "#696969");
                mje_con = '';
                if ($("option:selected", "#ddlTipoUsuario").index() == 1 || $("option:selected", "#ddlTipoUsuario").index() == 2) {
                    if ($("option:selected", "#ddlConcesionario").index() == 0) {
                        mje_con = 'Debe seleccionar un concesionario\n';
                        $("#lblConcesionario").css("color", "red");
                    }
                }
                return (mje_con);
            }
            //fin concesionario

            function validaBlancos() {
                validaTxtEpresa();
                validaTxtNombre();
                validaTxtRut();
                validaTxtTelefono();
                validaTxtEmail();

                mensaje = '';
                mensaje = mje_nom + mje_rut + mje_email + mje_fono + mje_emp;
                if (mensaje != '') {
                    alert(mensaje);
                    return false;
                }
                else {
                    return true;
                }
            } //fin validaBlancos
        });     //fin jquery
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <input type="hidden" id="hdnSucursal" runat="server" clientidmode="Static"/>
    <input type="hidden" id="hdnISucursal" runat="server" clientidmode="Static"/>
    <input type="hidden" id="hdnValSucursal" runat="server" clientidmode="Static"/>
    <div class="titulo">
        Mantenedor de usuarios: Crear un nuevo usuario
    </div>
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <!-- Menu lado izquierdo -->
    <div class="mantenedorUsuariosIzquierda">
        <center><b><u>Mantención Usuarios</u></b></center>
        <ul>
            <li>
                <asp:HyperLink ID="hlkModificaUsuario" runat="server" ClientIDMode="Static" 
                    NavigateUrl="~/Vistas/modificaUsuario.aspx">Buscar, modificar y eliminar
                </asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="hlkNuevoUsuario" runat="server" ClientIDMode="Static" 
                    NavigateUrl="~/Vistas/Mantenedor.aspx">Crear Usuario
                </asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="hlkUsuarioDoble" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/mantenedorUsuarioDoble.aspx">Doble Permiso</asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="hlkEliminados" runat="server" ClientIDMode="Static" NavigateUrl="~/Vistas/mantenedorUsuarioEliminados.aspx">Ver Eliminados</asp:HyperLink><br />
            </li>
        </ul>
    </div>
    <!-- Contenido lado derecho (centro) -->
    <div class="mantenedorUsuariosDerecha" style="height:100%">
        <b><u>Formulario de ingreso</u></b>
        <asp:Table ID="tblDatos" runat="server" ClientIDMode="Static" CssClass="tablaCrearUsuario">
            <asp:TableRow ID="TableRow23" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell3" runat="server" ClientIDMode="Static"><asp:Label ID="lblTipo" runat="server" ClientIDMode="Static" Text="Tipo Usuario" CssClass="lblTipo"></asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="TableCell4" runat="server" ClientIDMode="Static">
                    <asp:DropDownList ID="ddlTipoUsuario" runat="server" ClientIDMode="Static" CssClass="selectConcesionario">
                        <asp:ListItem Value="1">Administrador</asp:ListItem>
                        <asp:ListItem Value="2">Gerente</asp:ListItem>
                        <asp:ListItem Value="3">Operario</asp:ListItem>
                        <asp:ListItem Value="4">Cotizador</asp:ListItem>
                        <asp:ListItem Value="5">Supervisor</asp:ListItem>
                        <asp:ListItem Value="7">CDR</asp:ListItem>
                    </asp:DropDownList>
                    <img alt="Ejemplo: 'Cotizador'" title="Escoja tipo de usuario. Estos pueden ser Cotizador, Genrente, " src="../img/help.png" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="rowConcesionario" runat="server" ClientIDMode="Static" style="display:none" CssClass="rowConcesionario">
                <asp:TableCell ID="TableCell5" runat="server" ClientIDMode="Static">
                    <asp:Label ID="lblConcesionario" runat="server" ClientIDMode="Static" Text="Concesionario" CssClass="lblConcesionario">
                    </asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="TableCell6" runat="server" ClientIDMode="Static">
                    <asp:DropDownList ID="ddlConcesionario" runat="server" ClientIDMode="Static"> 
                    </asp:DropDownList>
                    <img alt="Ejemplo: 'PALD'" title="Debe ingresar nombre de concesionario. Ej: 'PALD'" src="../img/help.png" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="rowSucursal" runat="server" ClientIDMode="Static" style="display:none" CssClass="rowSucursal">
                <asp:TableCell ID="TableCell7" runat="server" ClientIDMode="Static">
                    <asp:Label ID="lblSucursal" runat="server" ClientIDMode="Static" Text="Sucursal" CssClass="lblSucursal" ></asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="TableCell8" runat="server" ClientIDMode="Static">
                    <asp:DropDownList ID="ddlSucursal" runat="server" ClientIDMode="Static">
                    </asp:DropDownList><span id="spnSucursal"></span>
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
            <asp:TableRow ID="rowReemplazo" runat="server" ClientIDMode="Static" style="display:none" CssClass="rowMultiple">
                <asp:TableCell ID="TableCell13" runat="server" ClientIDMode="Static">
                    <asp:Label ID="Label2" runat="server" ClientIDMode="Static" Text="Reemplazo" CssClass="lblMultiple"></asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="TableCell14" runat="server" ClientIDMode="Static">
                    <asp:CheckBox ID="chkReemplazo" runat="server" ClientIDMode="Static" CssClass="cbxMultipleSucursal" />
                    <img alt="Reemplazo Supervisor" title="Marque esta opcion si desa que el supervisor vea todos los concesionarios" src="../img/help.png" />
                </asp:TableCell>
            </asp:TableRow>

            <asp:TableRow ID="rowAprobVFC" runat="server" ClientIDMode="Static" style="display:none" CssClass="rowMultiple">
                <asp:TableCell ID="TableCell15" runat="server" ClientIDMode="Static">
                    <asp:Label ID="Label3" runat="server" ClientIDMode="Static" Text="Aprobar VFC" CssClass="lblMultiple"></asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="TableCell16" runat="server" ClientIDMode="Static">
                    <asp:CheckBox ID="chkAprobVFC" runat="server" ClientIDMode="Static" CssClass="cbxMultipleSucursal" />
                    <img alt="Aprobar VFC" title="Marque esta opción si desea que el operador visualice todas las cotizaciones de su concesionario." src="../img/help.png" />
                </asp:TableCell>
            </asp:TableRow>

            <asp:TableRow ID="rowCriticidadVfc" runat="server" ClientIDMode="Static" style="display:none" CssClass="rowMultiple">
                <asp:TableCell ID="TableCell17" runat="server" ClientIDMode="Static">
                    <asp:Label ID="Label4" runat="server" ClientIDMode="Static" Text="Indica Criticidad VFC" CssClass="lblMultiple"></asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="TableCell18" runat="server" ClientIDMode="Static">
                    <asp:CheckBox ID="chkCriticidadVfc" runat="server" ClientIDMode="Static" CssClass="cbxMultipleSucursal" />
                    <img alt="Indica Criticidad" title="Marque esta opción si desea que el operador pueda indicar criticidad a un VFC" src="../img/help.png" />
                </asp:TableCell>
            </asp:TableRow>

            <asp:TableRow ID="rowDevRec" runat="server" ClientIDMode="Static" style="display:none" CssClass="rowMultiple">
                <asp:TableCell ID="TableCell19" runat="server" ClientIDMode="Static">
                    <asp:Label ID="Label5" runat="server" ClientIDMode="Static" Text="Solicitudes Devoluciones Reclamos" CssClass="lblMultiple"></asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="TableCell20" runat="server" ClientIDMode="Static">
                    <asp:CheckBox ID="chkDevRec" runat="server" ClientIDMode="Static" CssClass="cbxMultipleSucursal" />
                    <img alt="Devoluciones/Reclamos" title="Marque esta opcion si desea habilitar solicitudes de devoluciones y reclamos" src="../img/help.png" />
                </asp:TableCell>
            </asp:TableRow>

            <asp:TableRow ID="TableRow1" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell1" runat="server" ClientIDMode="Static">
                    <asp:Label ID="lblNombre" runat="server" ClientIDMode="Static" Text="Nombre" CssClass="lblNombre"></asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="TableCell2" runat="server" ClientIDMode="Static">
                    <asp:TextBox MaxLength="50" ID="txtNombre" runat="server" ClientIDMode="Static" CssClass="txtNombre"></asp:TextBox>
                    <img alt="Ejemplo: 'Daniela'" title="Escriba un nombre. Ejemplo:'Juan'" src="../img/help.png" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow3" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell21" runat="server" ClientIDMode="Static">
                    <asp:Label ID="Label1" runat="server" ClientIDMode="Static" Text="Contraseña" CssClass="lblNombre"></asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="TableCell22" runat="server" ClientIDMode="Static">
                    <asp:TextBox MaxLength="50" ID="contrasena" runat="server" ClientIDMode="Static" CssClass="txtNombre"></asp:TextBox>
                    <img alt="Ejemplo: 'hola123'" title="Escriba una contraseña. Ejemplo:'hola123'" src="../img/help.png" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow4" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell23" runat="server" ClientIDMode="Static">
                    <asp:Label ID="lblRut" runat="server" ClientIDMode="Static" Text="Rut" CssClass="lblRut"></asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="TableCell24" runat="server" ClientIDMode="Static">
                    <asp:TextBox MaxLength="12" ID="txtRut" runat="server" ClientIDMode="Static" Width="110px" CssClass="txtRut"></asp:TextBox>
                    <%--<asp:Label ID="lblGuion" runat="server" ClientIDMode="Static" Text="-"></asp:Label>
                    <asp:TextBox MaxLength="1" ID="txtDvr" runat="server" ClientIDMode="Static" Width="23px" CssClass="txtDvr"></asp:TextBox>
                    <img alt="Ejemplo: '6892966-0'" title="Escriba un rut válido. Ejemplo:'6892966-0'" src="../img/help.png" />--%>
                </asp:TableCell>
            </asp:TableRow>
           
           <asp:TableRow ID="TableRow5" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell25" runat="server" ClientIDMode="Static">
                    <asp:Label ID="lblEmail" runat="server" ClientIDMode="Static" Text="E- Mail" CssClass="lblEmail"></asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="TableCell26" runat="server" ClientIDMode="Static">
                    <asp:TextBox MaxLength="100" ID="txtEmail" runat="server" ClientIDMode="Static" CssClass="txtEmail"></asp:TextBox>
                    <img alt="Ejemplo: 'yo@concesionario.cl'" title="Concesionario. Ejemplo:'yo@concesionario.cl'" src="../img/help.png" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow6" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell27" runat="server" ClientIDMode="Static"><asp:Label ID="lblTelefono" runat="server" ClientIDMode="Static" Text="Telefono" CssClass="lblTelefono"></asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="TableCell28" runat="server" ClientIDMode="Static">
                    <asp:TextBox MaxLength="5" ID="txtCodigoArea" runat="server" ClientIDMode="Static" Width="23" CssClass="txtTelefono"></asp:TextBox><asp:Label ID="lblGuionFono" runat="server" ClientIDMode="Static" Text="-"></asp:Label>
                        <asp:TextBox MaxLength="10" ID="txtTelefono" runat="server" ClientIDMode="Static" Width="110" CssClass="txtTelefono">
                        </asp:TextBox>
                    <img alt="Ejemplo: '2-5564338'" title="Teléfono. Ejemplo:'2 5563438'" src="../img/help.png" />
                </asp:TableCell>
            </asp:TableRow>

            <asp:TableRow>
                <asp:TableCell ID="TableCell112" runat="server" ClientIDMode="Static">
                    <asp:Label ID="lblidllave" runat="server" ClientIDMode="Static" Text="Id Llave" CssClass="lblidllave"></asp:Label>
                </asp:TableCell><asp:TableCell ID="TableCell12" runat="server" ClientIDMode="Static">
                    <asp:TextBox MaxLength="50" ID="txtIdllave" runat="server" ClientIDMode="Static" CssClass="txtidllave"></asp:TextBox>
                    <img alt="Ejemplo: '12233423'" title="Escriba un id. Ejemplo:'7893302'" src="../img/help.png" />
                </asp:TableCell>
            </asp:TableRow>

            <asp:TableRow  runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell11" runat="server" ClientIDMode="Static" ColumnSpan="2" HorizontalAlign="Center">
                    <asp:Button ID="btnAgregar" runat="server" ClientIDMode="Static" Text="Agregar nuevo usuario" CssClass="button" onclick="btnAgregar_Click"/>
                </asp:TableCell>
            </asp:TableRow>
            
        </asp:Table>
    </div>
</asp:Content>