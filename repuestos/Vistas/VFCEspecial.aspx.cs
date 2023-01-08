using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using log4net.Config;
using System.Configuration;

public partial class Vistas_VFCEspecial : System.Web.UI.Page
{
    public string cantidad = "";
    public string codigo = "";
    public string detalle = "";
    public string marca = "";
    public string marcaAbreviada = "";
    public string creador = "";
    public string nombreUser = "";
    public string dealer = "";
    public string direccion = "";
    public string tipoPed = "";
    public string prioridad = "";
    public string numBO = "";
    public string numBO2 = "";
    public string idPedido = "";
    SapAPI _sapApi;
    RutHelper _rutHelper;
    ControlVfc _controlVFC;
    ControlBD _controlBD;
    SendMail_helper _mail = new SendMail_helper();

    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_VFCEspecial));

    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;
        msjesError.InnerText = "";
        _sapApi = new SapAPI();
        _rutHelper = new RutHelper();
        ddlSucursales.Enabled = false;
        ddlSucursales.Visible = false;

        _controlBD = new ControlBD();

        if (!IsPostBack)
        {
            if (Request.QueryString["marca"] != null && Request.QueryString["codigo"] != null)
            {
                marca = Request.QueryString["marca"].ToString();
                codigo = Request.QueryString["codigo"].ToString();
                cantidad = Request.QueryString["cantidad"].ToString();
                txtDetalle.Text = Request.QueryString["detalle"].ToString();
                txtCreador.Text = _rutHelper.GetRutConDigito(Session["rut"].ToString());
                txtCreador.Enabled = false;
                txtDealer.Text = _sapApi.GetNombreDealer(Session["rut"].ToString());
                txtDealer.Enabled = false;
                txtDirección.Text = _sapApi.GetDireccionSucursal(Session["rut"].ToString());
                txtDirección.Enabled = false;
                txtNombreUsuario.Text = _sapApi.GetNombreUsuario(Session["rut"].ToString());
                txtNombreUsuario.Enabled = false;
                txtCodigo.Text = codigo;
                txtCodigo.Enabled = false;
                txtMarca.Text = marca;
                txtMarca.Enabled = false;
                try 
	            {	        
		            int a = int.Parse(cantidad);
                    txtCantidad.Text = cantidad;
	            }
	            catch (Exception)
	            {
		
	            }
                
                marcaAbreviada = _sapApi.GetAbreviadoMarca(marca);
                // Si usuario es multiSucursal, se muestra direcciones multiples
                if (_sapApi.DeterminarMultiSucursal(Session["rut"].ToString()))
                {
                    ddlSucursales.Enabled = true;
                    ddlSucursales.Visible = true;
                    txtDirección.Enabled = false;
                    txtDirección.Visible = false;

                    if (ddlSucursales.Items.Count == 0)
                    {
                        ddlSucursales.Items.Clear();
                        ddlSucursales.Items.Add(new ListItem("Seleccionar", "0"));
                        //obtengo las sucursales y lleno la lista con las direcciones de despacho
                        //Elección de ship code
                        foreach (string lista in _controlBD.ObtenerSucursalesPorConcesionario(_sapApi.GetNombreDealer(Session["rut"].ToString())))
                        {
                            ddlSucursales.Items.Add(lista);
                        }
                    }
                }
       
            }
        }
        else
        {
            // Si usuario es multiSucursal, se muestra direcciones multiples
            if (_sapApi.DeterminarMultiSucursal(Session["rut"].ToString()))
            {
                ddlSucursales.Enabled = true;
                ddlSucursales.Visible = true;
                txtDirección.Enabled = false;
                txtDirección.Visible = false;

                if (ddlSucursales.Items.Count == 0)
                {
                    ddlSucursales.Items.Clear();
                    ddlSucursales.Items.Add(new ListItem("Seleccionar", "0"));
                    //obtengo las sucursales y lleno la lista con las direcciones de despacho
                    //Elección de ship code
                    foreach (string lista in _controlBD.ObtenerSucursalesPorConcesionario(_sapApi.GetNombreDealer(Session["rut"].ToString())))
                    {
                        ddlSucursales.Items.Add(lista);
                    }
                }
            }
           
        }

        
    }
    
    protected void btnAceptar_Click(object sender, EventArgs e)
    {
        try
        {
           _controlVFC = new ControlVfc();
           _controlBD = new ControlBD();

           if (ddlSucursales.SupportsDisabledAttribute)
           {
                if (ddlSucursales.SelectedValue == "0")
                {
                    msjesError.InnerText = "Debe seleccionar una destinatario de mercancía (dirección)";
                    msjesError.Visible = true;
                    return;
                }
            }

           //txtVin


            if (txtDetalle.Text.Trim() == "")
            {
                msjesError.InnerText = "Debe ingresar el detalle";
                msjesError.Visible = true;
                return;
            }
            if (txtCodigo.Text.Trim() == "")
            {
                msjesError.InnerText = "Debe escoger un código al cotizar";
                msjesError.Visible = true;
                return;
            }
            string direccionfinal = "";
            string cantidad = txtCantidad.Text;
            string codigo = txtCodigo.Text;
            string detalle = txtDetalle.Text;
            string marca = txtMarca.Text;
            marca = _sapApi.GetCmpCod(marca);
		    string marca2 = txtMarca.Text;
            string creador = txtCreador.Text;
            string nombreUser = txtNombreUsuario.Text;
            string dealer = txtDealer.Text;
            if (ddlSucursales.SupportsDisabledAttribute)
            {
                direccionfinal = ddlSucursales.SelectedValue.ToString();
            }
            else 
            {
                direccionfinal = txtDirección.Text;
            }
		    if (direccionfinal=="")
            {
			    direccionfinal = txtDirección.Text;
		    }
            string tipoPed = "RESERVA";
            //se cambiara la prioridad a tipo pedido, por lo urgente del tema no se realizara otra variable o dejar la mejor solucion; esto se debe hacer en un futuro.--RAUF
            //string prioridad = "NORMAL";
            string opcionvfc = comboTipoPed.SelectedValue.ToString();
            string vin = ""; // txtVin.Text;
		    string chasis;
            string numResp = "";
            string detalleVFCCRM = "";
            string numResp2 = "";
            string idPedido = "000000";
            string detalleSolicitudMail = "";
            string km = "0";
            string nSiniestro = "0";
            string cc = _sapApi.GetCorreosVFCUsuario(Session["rut"].ToString());
            string codsap = _sapApi.GetCodigoShipCode(Session["rut"].ToString());

            
            //Se declaran las variables de respuestas del WS
            List<String> identificadores = new List<String>();
            List<String> detalles = new List<String>();

            //Para VFC//////////////////////////////////////////////////
//            if (RadioButtonList1.SelectedValue == "NORMAL")
//            {
//                try
//                {
//                    // Si deja vin en blanco, no permite confirmar
//                    if (vin == "")
//                    {
//                        msjesError.Visible = true;
//                        msjesError.InnerText = "Advertencia: Debe ingresar un código VIN en caso de solicitar VFC. Gracias";
//                        return;
//                    }

//                    if (vin.Count() != 17)
//                    {
//                        msjesError.Visible = true;
//                        msjesError.InnerText = "Advertencia: Debe ingresar un código VIN de 17 digitos en caso de solicitar VFC. Gracias";
//                        return;
//                    }

//                    bool vinvalido = true;
//                    _controlVFC.Vin = vin;
//                    _controlVFC.ObtieneDatosVfc();
//                    chasis = _controlVFC.getChasis();
//                    string marcavalidar = "";

//                    //NUEVO

//                    VIN.SI_Consulta_VehiculosService consulta = new VIN.SI_Consulta_VehiculosService();
//                    VIN.DT_Consulta_Vehiculos_RequestVehiculos consultaReq = new VIN.DT_Consulta_Vehiculos_RequestVehiculos();
//                    VIN.DT_Consulta_Vehiculos_RequestVehiculos[] consultaReq_2 = new VIN.DT_Consulta_Vehiculos_RequestVehiculos[1];
//                    VIN.DT_Consulta_Vehiculos_Response VinResponse = new VIN.DT_Consulta_Vehiculos_Response();
//                    VIN.DT_Consulta_Vehiculos_ResponseCaracteristicas_Vehiculos VinResponseCarc = new VIN.DT_Consulta_Vehiculos_ResponseCaracteristicas_Vehiculos();
//                    VIN.DT_Consulta_Vehiculos_ResponseT_RETURN VinResponseCarcRet = new VIN.DT_Consulta_Vehiculos_ResponseT_RETURN();



//                    consulta.Credentials = new System.Net.NetworkCredential("INT_SPI_SBCL", "5k82017PoPcl");
//                    consulta.PreAuthenticate = true;

//                    consultaReq.VHVIN = vin;
//                    consultaReq_2[0] = consultaReq;

//                    VinResponse = consulta.Consulta_Vehiculos(consultaReq_2);

//                    if (VinResponse.T_RETURN != null)
//                    {
//                        //hacer nada
//                    }
//                    else
//                    {
//                        foreach (var wsdl in VinResponse.Caracteristicas_Vehiculos)
//                        {
//                            string NomMarca = wsdl.NOMBRE.ToString();

//                            if (NomMarca == "SKB_MARCA")
//                            {
//                                marcavalidar = wsdl.VALOR.ToString();

//                                if (marcavalidar == "MITSUBISHI FUSO")
//                                {
//                                    marcavalidar = "";
//                                    marcavalidar = "FUSO";
//                                }
//                            }
//                        } 
//                    }
                   
//                    //FIN NUEVO


//                    if (marcavalidar == "")
//                    {
//                        vinvalido = false;
//                    }
//                    else if (_controlVFC.VinGrupoporMarca(marca2) != _controlVFC.VinGrupoporMarca(marcavalidar))
//                    {
//                        vinvalido = false;
//                    }

//                    if (vinvalido == false)
//                    {
//                        msjesError.Visible = true;
//                        msjesError.InnerText = "Advertencia: El VIN no se encontro en SAP. Gracias";
//                        return;
//                    }

//                }
//                catch (Exception ex)
//                {
//                    msjesError.Visible = true;
//                    msjesError.InnerText = "Error: al ingresar VFC, favor volver a intentarlo"  ;
//                    _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "1 - En [VFCEspecial_btnAceptar_Click] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException + ". cantidad:" + cantidad + ". codigo:" + codigo + ". detalle:" + detalle + ". marca:" + marca + ". vin:" + vin + ". creador:" + creador + ". nombreUser:" + nombreUser + ". dealer:" + dealer + ". direccionfinal:" + direccionfinal + ". tipoPed:" + tipoPed + ". opcionvfc:" + opcionvfc + ". km:" + km);
//                    return;
//                }

//            ////////////////////////////////////////////////////////////////////////////////
//                try
//                {
                    
//                    //Se llama al método que inserta el vfc
//                    _controlVFC.hacerVfc(cantidad, codigo, detalle, marca, vin, creador, nombreUser, dealer, direccionfinal, tipoPed, opcionvfc, km, nSiniestro, cc, codsap);
                    
//                    identificadores = _controlVFC.getIdentificadores();
//                    detalles = _controlVFC.getDetalles(); 
//                    for(int i=0;i<identificadores.Count;i++)
//                    {
//                        if (identificadores[i].Length > 9)
//                        {
//                            if (identificadores[i].Substring(0, 8) == "IdEntity")
//                            {
//                                numResp = identificadores[i].Substring(9, identificadores[i].Length - 9);
//                                numResp = numResp.Trim();
//                                numResp2 = identificadores[i].Substring(0, 8);
//                                detalleVFCCRM = detalles[i];
//                                break;
//                            }
//                        }
//                    }


//                    if (numResp == "")
//                    {
//                        detalleVFCCRM = detalles[0]; 
//                        msjesError.Visible = true;
//                        msjesError.InnerText = "Advertencia: VFC no se cargo en CRM, vuelva a intentar, si problemas persisten dar aviso del error. ";
//                        _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [VFCEspecial_btnAceptar_Click_CRM_VFC] Message: Error al insertar VFC en el CRM Error:" + detalleVFCCRM + ". cantidad:" + cantidad + ". codigo:" + codigo + ". detalle:" + detalle + ". marca:" + marca + ". vin:" + vin + ". creador:" + creador + ". nombreUser:" + nombreUser + ". dealer:" + dealer + ". direccionfinal:" + direccionfinal + ". tipoPed:" + tipoPed + ". opcionvfc:" + opcionvfc + ". km:" + km + " nSiniestro:" + nSiniestro + ". Error WS H2 VFC:" + detalleVFCCRM);

//                        return; 
//                    }

//                } catch (Exception ex)
//                {
//                    msjesError.Visible = true;
//                    msjesError.InnerText = "Error: indice VFC :  " + numResp ;
//                    _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [VFCEspecial_btnAceptar_Click_Insert_VFC] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException + ". cantidad:" + cantidad + ". codigo:" + codigo + ". detalle:" + detalle + ". marca:" + marca + ". vin:" + vin + ". creador:" + creador + ". nombreUser:" + nombreUser + ". dealer:" + dealer + ". direccionfinal:" + direccionfinal + ". tipoPed:" + tipoPed + ". opcionvfc:" + opcionvfc + ". km:" + km + " .detalleVFCCRM " + detalleVFCCRM + " .numResp " + numResp);
//                    return;
//                }

//                    try
//                {
//                    //se insertan los datos en la tabla VFC
//                    _controlBD.InsertarDatos(@"insert into VFC(num_VFC,id_pedido,rut_user,fecha_creacion,
//                                            codigo_rep,marca,cantidad,detalle_rep,cod_vin)
//                                            values('" + numResp.Trim() + "','" + idPedido + "','" + creador + "' , GETDATE(), " +
//                                            " '" + codigo + "' , '" + marca + "' , " + cantidad + " , '" + detalle + "','" + vin + "')");

//                    detalleSolicitudMail += "1. " + " | VFC | " + numResp + " | " + marca + " | " + codigo + " | " + cantidad + " | " + detalle + " | " + vin + " |\n";
//                }
//                catch (Exception ex)
//                {
//                    msjesError.Visible = true;
//                    msjesError.InnerText = "Error: indice :  " + numResp ;
//                    string consultasql = "insert into VFC(num_VFC,id_pedido,rut_user,fecha_creacion, codigo_rep,marca,cantidad,detalle_rep,cod_vin) values('" + numResp.Trim() + "','" + idPedido + "','" + creador + "' , GETDATE(), " ;
//                     consultasql= consultasql + " '" + codigo + "' , '" + marca + "' , " + cantidad + " , '" + detalle + "','" + vin + "')"; 
//                    _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [VFCEspecial_btnAceptar_Click_Insert_VFC] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException + ". cantidad:" + cantidad + ". codigo:" + codigo + ". detalle:" + detalle + ". marca:" + marca + ". vin:" + vin + ". creador:" + creador + ". nombreUser:" + nombreUser + ". dealer:" + dealer + ". direccionfinal:" + direccionfinal + ". tipoPed:" + tipoPed + ". opcionvfc:" + opcionvfc + ". km:" + km + " .Insert: " + consultasql);
//                    return;
//                }
			
//            }
            // tipo de pedido garantia
            
            try
            {
                //Se llama al método que inserta el vfc
                //vin = "1";
                _controlVFC.hacerBo(cantidad, codigo, detalle, marca, creador, nombreUser, dealer, direccionfinal, tipoPed, opcionvfc, km, nSiniestro,cc,codsap);
                identificadores = _controlVFC.getIdentificadores();
                detalles = _controlVFC.getDetalles(); 

                for (int i = 0; i < identificadores.Count; i++)
                {
                    if (identificadores[i].Length > 9)
                    {
                        if (identificadores[i].Substring(0, 8) == "IdEntity")
                        {
                            numResp = identificadores[i].Substring(9, identificadores[i].Length - 9);
                            numResp = numResp.Trim();
                            numResp2 = identificadores[i].Substring(0, 8);
                            break;
                        }
                    }
                }

                if (numResp == "")
                {
                    detalleVFCCRM = detalles[0]; 
                    msjesError.Visible = true;
                    msjesError.InnerText = "Advertencia: VFC no se cargo en CRM, vuelva a intentar, si problemas persisten dar aviso del error." ;
                    _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [VFCEspecial_btnAceptar_Click_CRM_VFC] Message: Error al insertar VFC en el CRM Error:" + detalleVFCCRM + ". cantidad:" + cantidad + ". codigo:" + codigo + ". detalle:" + detalle + ". marca:" + marca + ". vin:" + vin + ". creador:" + creador + ". nombreUser:" + nombreUser + ". dealer:" + dealer + ". direccionfinal:" + direccionfinal + ". tipoPed:" + tipoPed + ". opcionvfc:" + opcionvfc + ". km:" + km + ". Error WS H2 RESERVA:" + detalleVFCCRM);
                    return;
                }


                //se insertan los datos en la tabla BACKORDER
                try
                {
                    _controlBD.InsertarDatos(@"insert into BACKORDER(num_backOrder,id_pedido,rut_user,fecha_creacion,
                                            codigo_rep,marca,cantidad,detalle_rep)
                                            values('" + numResp.Trim() + "','" + idPedido + "','" + creador + "' , GETDATE(), " +
                                                    " '" + codigo + "' , '" + marca + "' , " + cantidad + " , '" + detalle + "')");

                    detalleSolicitudMail += "1. " + " | Reserva | " + numResp + " | " + marca + " | " + codigo + " | " + cantidad + " | " + detalle + "|\n";
                }
                catch (Exception ex)
                {
                    msjesError.Visible = true;
                    msjesError.InnerText = "Error: Al insertar reserva, favor volver a intentar" ;

                    string consultasql = "insert into BACKORDER(num_backOrder,id_pedido,rut_user,fecha_creacion, codigo_rep,marca,cantidad,detalle_rep)values('" + numResp.Trim() + "','" + idPedido + "','" + creador + "' , GETDATE(), "; 
                            consultasql= consultasql + " '" + codigo + "' , '" + marca + "' , " + cantidad + " , '" + detalle + "')";

                    _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [VFCEspecial_btnAceptar_Click_insert_BACKORDER] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException + ". NumRep: " + numResp + ". cantidad:" + cantidad + ". codigo:" + codigo + ". detalle:" + detalle + ". marca:" + marca + ". vin:" + vin + ". creador:" + creador + ". nombreUser:" + nombreUser + ". dealer:" + dealer + ". direccionfinal:" + direccionfinal + ". tipoPed:" + tipoPed + ". opcionvfc:" + opcionvfc + ". km:" + km + " .Insert " + consultasql );
                    return;
                }

            }
            catch (Exception ex)
            {
                msjesError.Visible = true;
                msjesError.InnerText = "Error: indice Reserva :  " + numResp ;
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [VFCEspecial_btnAceptar_Click] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException + ". cantidad:" + cantidad + ". codigo:" + codigo + ". detalle:" + detalle + ". marca:" + marca + ". vin:" + vin + ". creador:" + creador + ". nombreUser:" + nombreUser + ". dealer:" + dealer + ". direccionfinal:" + direccionfinal + ". tipoPed:" + tipoPed + ". opcionvfc:" + opcionvfc + ". km:" + km + " .detalleVFCCRM " + detalleVFCCRM + " .numResp " + numResp);
                return;
            }
            

        // Se envía mail
        // Se envía correo con reservas 
            String textoCorreo = "";

        try
        {
            string para = _sapApi.GetCorreoUsuario(Session["rut"].ToString());
            
            string asunto = "SKBERGE: Solicitud de reserva confirmado [NO RESPONDER]";
            // Se envía un correo con los repuestos sin stock solicitados
            

            textoCorreo = "Usted ha realizado solicitud de reserva de repuestos a SKBergé. Le recordamos que las reservas se despacharán y facturarán una vez arribados los repuestos a nuestra bodega";
            textoCorreo += "\nConcesionario: " + _sapApi.GetNombreDealer(Session["rut"].ToString());
            //textoCorreo += "\nDireccion sucursal: " + _sapApi.GetDireccionSucursalByShipCode(direccion);
            textoCorreo += "\nDireccion destino: " + direccionfinal;
            textoCorreo += "\n\nDetalle del pedido";
            textoCorreo += "\n" + detalleSolicitudMail;
            _mail.EnviarCorreo(para + "," + "czurita@Skberge.cl", asunto, textoCorreo, cc);
           // _mail.EnviarCorreo("porosteguig@skberge.cl", asunto, textoCorreo, cc); //Cambio Momentaneo
           // _mail.EnviarCorreo("porosteguig@Skberge.cl", asunto, textoCorreo, cc); //Cambio Momentaneo
        }
        catch (Exception ex)
        {
            logger.Error("En [VFCEspecial_btnAceptar_Click] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException + ". cantidad:" + cantidad + ". codigo:" + codigo + ". detalle:" + detalle + ". marca:" + marca + ". vin:" + vin + ". creador:" + creador + ". nombreUser:" + nombreUser + ". dealer:" + dealer + ". direccionfinal:" + direccionfinal + ". tipoPed:" + tipoPed + ". opcionvfc:" + opcionvfc + ". km:" + km + ". Correo" + textoCorreo);
            msjesError.InnerText = "Advertencia, hubo un problema en el envío de correo. Los pedidos se han procesado de todas maneras. El número de la solicitud es: " + numResp; 
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [VFCEspecial_btnAceptar_Click_Error envio Correo] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException + ". cantidad:" + cantidad + ". codigo:" + codigo + ". detalle:" + detalle + ". marca:" + marca + ". vin:" + vin + ". creador:" + creador + ". nombreUser:" + nombreUser + ". dealer:" + dealer + ". direccionfinal:" + direccionfinal + ". tipoPed:" + tipoPed + ". opcionvfc:" + opcionvfc + ". km:" + km  +". Correo" + textoCorreo + ". El número de la solicitud es: " + numResp );
            msjesError.Visible = true;
            return;
        }
        
        msjesError.Visible = true;
        msjesError.InnerText = "Transacción satisfactoria. El número de la solicitud es: " + numResp; 
        PanelBotonera.Visible = false;

        }
        catch (Exception ex)

        {
            msjesError.Visible = true;
            msjesError.InnerText = "Error: " + ex.Message;
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "2 - En [VFCEspecial_btnAceptar_Click] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException + ". cantidad:" + cantidad + ". codigo:" + codigo + ". detalle:" + detalle + ". marca:" + marca +  ". creador:" + creador + ". nombreUser:" + nombreUser + ". dealer:" + dealer +  ". tipoPed:" + tipoPed );
            return;
        }
        }


}