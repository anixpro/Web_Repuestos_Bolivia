<%@ Page Language="C#" Debug="true" AutoEventWireup="true" CodeFile="indexMnt.aspx.cs" Inherits="indexMnt" %>
<%@ Register Assembly="MSCaptcha" Namespace="MSCaptcha" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en">
    <head id="Head1" runat="server">
        <meta http-equiv="X-UA-Compatible" content="IE=edge" />
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
        <title></title>
        <style type="text/css">
	        body {
		        background-color: #FFFFFF;
	        }
	        .style3 {
		        color: #FFFFFF;
		        font-size: 12px;
	        }
	        a:link {
		        color: #FFFFFF;
		        text-decoration: none;
	        }
	        a:visited {
		        text-decoration: none;
		        color: #FFFFFF;
	        }
	        a:hover {
		        text-decoration: none;
	        }
	        a:active {
		        text-decoration: none;
	        }
	        body,td,th {
		        font-family: Verdana, Arial, Helvetica, sans-serif;
		        font-size: 12px;
	        }
	        a {
		        font-size: 12px;
	        }
        </style>

        <script type='text/javascript' src='js/jquery-1.6.2.min.js'></script>
        <script language="javascript" type="text/javascript">

            $(document).ready(function () {

                $("#hlkRecuperaContrasena").click(function () {
                    return obtieneRut();
                });
                function confirmar(rutRecibido) {

                    var rutEntero = rutRecibido.split('-');

                    var rut = rutEntero[0];
                    var largo = rut.length;

                    var pregunta = confirm(rutRecibido + "\n\nSu contraseña será reestablecida (quedará como su primera contraseña)\n¿Está seguro(a)?");
                    if (pregunta) {
                        return location.href = "index.aspx?evento=recupera&&rut=" + rut;
                    }
                    else {
                        return false;
                    }
                }

                function obtieneRut() {
                    var rut = "";
                    rut = prompt('Digite su rut con guion. Ejemplo 6892966-0 ("k" minuscula)');
                    while (rut == '') {
                        alert('Debe Escribir su rut');
                        rut = prompt('Digite su rut');
                    }
                    if (rut == null) {
                        return false;
                    }
                    else {
                        return validaRut(rut);
                    }
                } //fin obtieneRut

                function validaRut(rutRecibido) {

                    var rutEntero = rutRecibido.split('-');
                    //alert(rutEntero);
                    mje_rut = '';

                    var rut = rutEntero[0];
                    var largo = rut.length;
                    var i = 0;
                    var dv = rutEntero[1];
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
                    }

                    if (mje_rut != "")
                    { alert(mje_rut); return false; }
                    else {return confirmar(rutRecibido); }
                }

            });//fin de jquery
        </script>
    </head>
    <body>
		<div align="center">
			<table width="1000" border="0" cellspacing="0" cellpadding="0">
				<tr>
					<td width="1000" height="710" valign="top" background="img/imgLogin/bg.png">
						<div align="center">
							<table width="100%" border="0" cellspacing="0" cellpadding="0">
								<tr>
									<td width="215" rowspan="3">&nbsp;</td>
									<td height="130">
                                        <br />
                                       
                                    </td>
									<td rowspan="3">&nbsp;</td>
								</tr>
								<tr>
									<td width="700" height="295">
										<table width="100%" border="1" cellspacing="0" cellpadding="0">
											<tr>
												<td width="700" height="295" style="border: 1px solid #ffffff" background="img/imgLogin/bg-box2.png">
													<table width="100%" border="0" cellspacing="0" cellpadding="0">
														<tr>
															<td width="51%" align="center">
																<img src="img/logosk-new.png" width="357" height="106" />
															</td>
														</tr>
														<tr>
															<td height="102" style="font-family: Verdana; font-size: xx-large; color: #FFFFFF" align="center">
                                                                Página en mantención, disculpe las molestias.
															</td>
															
														</tr>
														<tr>
															<td>
																
															</td>
														</tr>
													</table>
												</td>
											</tr>
										</table>
									</td>
								</tr>
								<tr>
									<td>&nbsp;</td>
								</tr>
							</table>
						</div>
					</td>
				</tr>
			</table>
			<br />
            <table>
                <tr>
                    <td>Navegador: </td>
                    <td><asp:Label ID="lblNav" runat="server" Text=""></asp:Label></td>                
                </tr>
                <tr>
                    <td>Versión: </td>
                    <td><asp:Label ID="lblVersion" runat="server" Text=""></asp:Label></td>                
                </tr>
                 <tr>
                    <td>Versión Web Repuestos: </td>
                    <td>
                        <% ConfAmbiente.ConfCredenciales(); Response.Write(ConfAmbiente.version); %></td>                
                </tr>
            </table>
		</div>
    </body>
</html>
