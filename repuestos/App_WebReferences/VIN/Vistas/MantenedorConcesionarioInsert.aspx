<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="MantenedorConcesionarioInsert.aspx.cs" Inherits="mantenedorConcesionarioInsert" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script language="javascript" type="text/javascript">

        $(document).ready(function () {

            $("#btnAgregar").click(function () {
                return validaBlancos();
            });

            //funcion que trnasforma la K a k
            $("#txtDvr").keyup(function () {
                if ($("#txtDvr").val() == 'K') {
                    $("#txtDvr").val('k');
                }
            });

            function validaAdministrador() {
                cajaTexto = document.getElementById('ddlAdministrador').value;
                mje_adm = '';
                if (cajaTexto == "0") {
                    mje_adm = 'debe llenar Administrador\n';
                    document.getElementById('lblAdministrador').style.color = "red";
                    //document.getElementById('ddlAdministrador').value = '0';
                }
                return (mje_adm);
            }

            function validaSupervisor() {
                cajaTexto = document.getElementById('ddlSupervisor').value;
                mje_sup = '';
                if (cajaTexto == "0") {
                    mje_sup = 'debe llenar Supervisor\n';
                    document.getElementById('lblSupervisor').style.color = "red";
                    //document.getElementById('ddlSupervisor').value = '0';
                }
                return (mje_sup);
            }
            //lblCorreoVFC


            function validaCorreoVFC() {
                document.getElementById('lblCorreoVFC').style.color = "#696969";
                cajaTexto = document.getElementById('txtCorreoVFC').value;
                mje_corrvfc = '';

                if (cajaTexto != "") {
                    expr = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;

                    if (cajaTexto.substr((cajaTexto.length - 1), 1) == ",") {
                        alert("Error: Las direcciones de correo deben ir sin , al final ");

                        //alert("Error: La dirección de correo " + cajaTexto.substr(0,(cajaTexto.length-1)) + " es incorrecta.");
                    }
                    var mySplitResult = cajaTexto.split(",");

                    for (i = 0; i < mySplitResult.length; i++) {

                        email = mySplitResult[i];
                        if (email != "") {
                            if (!expr.test(email)) {
                                alert("Error: La dirección de correo " + email + " es incorrecta.");
                                mje_corrvfc = 'La dirección de correo ' + email + ' es incorrecta.\n';
                                document.getElementById('lblCorreoVFC').style.color = "red";
                            }
                        }
                    }







                }

                return (mje_corrvfc);
            }



            function validaTxtNombre() {
                document.getElementById('lblConcesionario').style.color = "#696969";
                cajaTexto = document.getElementById('txtNombre').value;
                mje_nom = '';
                if (cajaTexto == "") {
                    mje_nom = 'debe llenar Nombre\n';
                    document.getElementById('lblConcesionario').style.color = "red";
                    document.getElementById('txtNombre').value = '';
                }

                return (mje_nom);
            }

            function obtenerExtencion() {
                mje_foto = '';
                document.getElementById('lblImagen').style.color = "#696969";
                if (document.getElementById('fldImagen').value == "") {
                    return true;
                }
                var cadena = document.getElementById('fldImagen').value.split('.');
                if (cadena[1] != 'jpg' && cadena[1] != 'JPG' && cadena[1] != 'png' && cadena[1] != 'PNG') {
                    mje_foto = "Formato de imagen solo puede ser:jpg o png\n";
                    document.getElementById('lblImagen').style.color = "red";
                    document.getElementById('fldImagen').value = '';
                }
                return (mje_foto);

            }

            function validaCheck() {

                var numeroDeChecks = '<%=cbxlMarcas.Items.Count %>';
                var checkTotal = 'cbxlMarcas_';
                var nombreCheck = '';
                var aux = 0;
                //alert(numeroDeChecks);
                for (x = 0; x < numeroDeChecks; x++) {

                    nombreCheck = checkTotal + x;
                    //alert(nombreCheck);
                    if (!document.getElementById(nombreCheck).checked) {

                        aux += 1;

                    }
                }

                //alert(aux);
                mje_check = '';
                if (aux == numeroDeChecks) {
                    mje_check = 'Debe seleccionar al menos una marca\n';

                }

                return (mje_check);
            }

            function validaTxtRut() {

                $("#lblRut").css("color", "#696969");
                cajaTexto = $("#txtRut").val();
                mje_rut = '';
                if (cajaTexto == "") {
                    mje_rut = 'debe llenar Rut\n';
                    $("#lblRut").css("color", "red");
                    $("#txtRut").val("");
                }
                else if (isNaN(cajaTexto)) {
                    mje_rut = 'solo numeros en Rut\n';
                    $("#lblRut").css("color", "red");
                    $("#txtRut").val("");
                }
                
                }

                return (mje_rut);
            } //fin validaRut

            function validaBlancos() {
                validaCorreoVFC();
                validaSupervisor()
                validaAdministrador();
                validaTxtNombre();
                obtenerExtencion();
                validaCheck();
                validaTxtRut();
                mensaje = '';
                mensaje = mje_nom + mje_foto + mje_check + mje_rut + mje_adm + mje_sup + mje_corrvfc;
                if (mensaje != '') {
                    window.alert(mensaje);
                    return false;
                }
                else {
                    return true;
                }
            }

        });                //fin jquery
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="titulo">Nuevo concesionario</div>
    <!-- Acá se despliegan los mensajes de error -->
    <div id="msjesError" runat="server" clientidmode="Static"></div>
    <div class="center">
    <asp:Table ID="tblDatos" runat="server" ClientIDMode="Static">
        <asp:TableRow ID="TableRow3" runat="server" ClientIDMode="Static">
            <asp:TableCell ID="TableCell6" runat="server" ClientIDMode="Static">
                <asp:Label ID="lblConcesionario" runat="server" ClientIDMode="Static" Text="Concesionario"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell7" runat="server" ClientIDMode="Static">
                <asp:TextBox MaxLength="25" id="txtNombre" runat="server" ClientIDMode="Static"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow> 
        <asp:TableRow runat="server" ClientIDMode="Static">
            <asp:TableCell ID="TableCell5" runat="server" ClientIDMode="Static" ColumnSpan="2">
                <asp:CheckBoxList ID="cbxlMarcas" runat="server" ClientIDMode="Static" RepeatDirection="Horizontal" RepeatColumns="4">
                </asp:CheckBoxList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow1" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell1" runat="server" ClientIDMode="Static"><asp:Label ID="lblZona" runat="server" ClientIDMode="Static" Text="Zona"></asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="TableCell2" runat="server" ClientIDMode="Static">
                <asp:DropDownList ID="ddlZona" runat="server" ClientIDMode="Static">
                </asp:DropDownList>
                </asp:TableCell>
        </asp:TableRow>
            <asp:TableRow ID="TableRow2" runat="server" ClientIDMode="Static">
                <asp:TableCell ID="TableCell3" runat="server" ClientIDMode="Static"><asp:Label ID="lblCodigoCliente" runat="server" ClientIDMode="Static" Text="Código cliente"></asp:Label>
                </asp:TableCell>
                <asp:TableCell ID="TableCell4" runat="server" ClientIDMode="Static">
                <asp:TextBox MaxLength="15" ID="txtCodigoCliente" runat="server" ClientIDMode="Static">
                </asp:TextBox>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow4" runat="server" ClientIDMode="Static">
            <asp:TableCell ID="TableCell8" runat="server" ClientIDMode="Static">
                <asp:Label ID="lblRut" runat="server" ClientIDMode="Static" Text="Rut" CssClass="lblRut"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell9" runat="server" ClientIDMode="Static">
                <asp:TextBox MaxLength="12" ID="txtRut" runat="server" ClientIDMode="Static" Width="110px" CssClass="txtRut"></asp:TextBox>
                </asp:TableCell>
        </asp:TableRow>

                <asp:TableRow ID="TableRow5" runat="server" ClientIDMode="Static">
                    <asp:TableCell ID="TableCell10" runat="server" ClientIDMode="Static">
                    <asp:Label ID="lblAdministrador" runat="server" ClientIDMode="Static" Text="Administrador"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell11" runat="server" ClientIDMode="Static">
                    <asp:DropDownList ID="ddlAdministrador" runat="server" ClientIDMode="Static">
                    </asp:DropDownList>
                    </asp:TableCell>
                </asp:TableRow>

                <asp:TableRow ID="TableRow6" runat="server" ClientIDMode="Static">
                    <asp:TableCell ID="TableCell12" runat="server" ClientIDMode="Static">
                    <asp:Label ID="lblSupervisor" runat="server" ClientIDMode="Static" Text="Supervisor"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell ID="TableCell13" runat="server" ClientIDMode="Static">
                    <asp:DropDownList ID="ddlSupervisor" runat="server" ClientIDMode="Static">
                    </asp:DropDownList>
                    </asp:TableCell>
                </asp:TableRow>

                        <asp:TableRow ID="TableRow7" runat="server" ClientIDMode="Static">
            <asp:TableCell ID="TableCell14" runat="server" ClientIDMode="Static">
                <asp:Label ID="lblCorreoVFC" runat="server" ClientIDMode="Static" Text="Correos VFC"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="TableCell15" runat="server" ClientIDMode="Static">
                <asp:TextBox MaxLength="350" Width="200" id="txtCorreoVFC" runat="server" ClientIDMode="Static"></asp:TextBox>
                 <img alt="Ejemplo: 'correo1@skberge.cl,correo2@skberge.cl'" title="Escriba correos validos.  'correo1@skberge.cl, correo2@skberge.cl'" src="../img/help.png" />
            </asp:TableCell>
        </asp:TableRow> 



        <asp:TableRow runat="server" ClientIDMode="Static">
            <asp:TableCell runat="server" ClientIDMode="Static"><asp:Label ID="lblImagen" runat="server" ClientIDMode="Static" Text="Logo"></asp:Label>
            </asp:TableCell>
            <asp:TableCell runat="server" ClientIDMode="Static">
                <asp:FileUpload ID="fldImagen" runat="server" ClientIDMode="Static"/>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow runat="server" ClientIDMode="Static">
            <asp:TableCell runat="server" ClientIDMode="Static"></asp:TableCell>
            <asp:TableCell runat="server" ClientIDMode="Static"><asp:Button ID="btnAgregar" runat="server" ClientIDMode="Static" Text="Aceptar" onclick="btnAgregar_Click"/>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    &nbsp;</div>
    <div class="footDer">
        <asp:HyperLink ID="hlkSucursal" runat="server" ClientIDMode="Static" 
            NavigateUrl="~/Vistas/SucursalInsert.aspx">Ir a sucursal</asp:HyperLink>
        <asp:HyperLink ID="hlkVolver" runat="server" NavigateUrl="~/Vistas/mantenedorConcesionario.aspx">Volver</asp:HyperLink>
    </div>
</asp:Content>

