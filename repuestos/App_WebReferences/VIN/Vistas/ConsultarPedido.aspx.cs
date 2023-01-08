using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using log4net;
using log4net.Config;
using System.Configuration;

public partial class Vistas_ConsultarPedido : System.Web.UI.Page
{
    SapAPI _sapApi;
    ControlBD _controlBD = new ControlBD();

    //Objeto que se recibe parametros que se envian a SAP
    ConfirmarPedido _confirPedido;

    //Clase para generar pedidos, cotizaciones y busqueda de repuestos
    RealizarPedido _pedido;

    //Clase que envia Mail
    SendMail_helper _mail = new SendMail_helper();


    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_ConsultarPedido));

    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;
        _sapApi = new SapAPI();
        //comboConsultar.SelectedIndexChanged += OnSelectBusqueda; //Modifique PO
        GridViewCotizaciones.SelectedIndexChanged += OnConsultarDesdeGrillaCotizacion;
        GridViewPedidos.SelectedIndexChanged += OnConsultarDesdeGrillaPedido;
        PanelBusqueda.Visible = true;
        lblNumBuscar.Text = "Ingrese N° de pedido: ";
    }

    protected void ConsultarEstadoPedido(string numero)
    {
        string ambiente = "";
        ambiente = ConfAmbiente.ambiente;
        if (ambiente == "ERP")
        {
            #region Ambiente ERP
            try
            {
                //Aca Configurar Otro USER/PASS
                string WsUser = ConfAmbiente.user; // "INTSOA_SKB";
                string WsPassword = ConfAmbiente.pass; // "intERPsoa2013";

                //Si la entrada de número esta vacia se muestra una grilla con todos los datos disponibles para el usuario logeado
                if (numero.Length == 0)
                {
                    GridViewPedidos.DataSource = _controlBD.ObtenerDatosFiltrados(@"select * from pedido where solicitado_por = '" + Session["rut"].ToString() + "' and E_VBELN_PEDIDO is not null");
                    GridViewPedidos.DataBind();
                    PanelTodosPedidos.Visible = true;
                    PanelInfoPedido.Visible = false;
                    return;
                }

                PanelTodosPedidos.Visible = false;
                PanelTodasCotizaciones.Visible = false;

                // Variables asociadas al WS
                ERP.SI_Consulta_Estado_Pedido_Venta_OutService wsConsultaPedido = new ERP.SI_Consulta_Estado_Pedido_Venta_OutService();
                //ERQ.ZB_CONS_PED wsConsultaPedido = new ERQ.ZB_CONS_PED();
                ERP.DT_Consulta_Estado_Pedido_Venta_ResponseT_POS wsDataConsulta = new ERP.DT_Consulta_Estado_Pedido_Venta_ResponseT_POS();
                //ERQ.DT_Web_PdoVtas_Consul wsDataConsulta = new ERQ.DT_Web_PdoVtas_Consul();
                ERP.DT_Consulta_Estado_Pedido_Venta_Request wsDataConsultaPedido = new ERP.DT_Consulta_Estado_Pedido_Venta_Request();
                //ERQ.DT_Web_PdoVtas_ConsulPedido wsDataConsultaPedido = new ERQ.DT_Web_PdoVtas_ConsulPedido();

                ERP.DT_Consulta_Estado_Pedido_Venta_Response WsResponse = new ERP.DT_Consulta_Estado_Pedido_Venta_Response();


                // Variables de la respuesta del pedido
                //ERQ.DT_ERP_PdoVtas_Consul WsResponse = new ERQ.DT_ERP_PdoVtas_Consul();
                //ERQ.DT_ERP_PdoVtas_ConsulPedido wsResponsePedido = new ERQ.DT_ERP_PdoVtas_ConsulPedido();
                //ERQ.DT_ERP_PdoVtas_ConsulPedidoErrores wsResponseErrores = new ERQ.DT_ERP_PdoVtas_ConsulPedidoErrores();

                //Se asigna el E_VBELN

                wsDataConsultaPedido.I_VBELN = numero;
                //wsDataConsultaPedido = wsDataConsultaPedido;

                //Se ejecuta la llamada al WS =)
                wsConsultaPedido.Credentials = new System.Net.NetworkCredential(WsUser, WsPassword);
                wsConsultaPedido.PreAuthenticate = true;
                WsResponse = wsConsultaPedido.SI_Consulta_Estado_Pedido_Venta_Out(wsDataConsultaPedido);

                if (WsResponse.E_AUDAT == "00000000")
                {
                    msjesError.InnerText = "Error, número de pedido no valido.";
                    msjesError.Visible = true;
                    return;
                }


                //Se asignan los datos a los campos
                cmpy_code.Text = _sapApi.GetPrefijoMarcaByOrganizacionDeVentas(WsResponse.E_VKORG);
                name_text.Text = WsResponse.E_VTEXT.Trim();
                porder_num.Text = WsResponse.E_VBELN.Trim();
                order_date.Text = string.Format(WsResponse.E_AUDAT, "{0:d}").Trim();
                goods_amt.Text = "$" + System.Convert.ToString(System.Convert.ToDouble(WsResponse.E_NETWR.Trim())).ToString();
                tax_amt.Text = "$" + System.Convert.ToString(System.Convert.ToDouble(WsResponse.E_KZWI5.Trim())).ToString();
                total_amt.Text = "$" + System.Convert.ToString(System.Convert.ToDouble(WsResponse.E_TOTAL.Trim())).ToString();
                numCotizacion.Text = _sapApi.GetNumCotizacionPorNpedido(WsResponse.E_VBELN.Trim());
                //numOrden.Text = SapApi.GetDataFromPedidoByPedidoId(idPedido, "ID_PEDIDO", "I");
                try
                {
                    ERQ.DT_Consulta_Estado_Pedido_Venta_ResponseT_POS[] wsResponseItemPedidos = new ERQ.DT_Consulta_Estado_Pedido_Venta_ResponseT_POS[WsResponse.T_POS.Length];
                }
                catch (NullReferenceException ex)
                {
                    msjesError.InnerText = "El pedido no se pudo rescatar desde SKBergé, contacte a su admin, el detalle de este error es el siguiente: " + ex.Message;
                    msjesError.Visible = true;
                    logger.Error("NullReferenceException en [ConsultarEstadoPedido] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
                    _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "NullReferenceException en [ConsultarEstadoPedido_ConsultarEstadoPedido] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);

                }
                GridView2.DataSource = WsResponse.T_POS;
                GridView2.DataBind();

                //PanelBusqueda.Visible = false;
                PanelInfoPedido.Visible = true;
                txtNumCotizacion.Text = "";
                txtNumPedido.Text = "";

                trrHideMe.Visible = false;
            }
            catch (System.Net.WebException ex)
            {
                msjesError.InnerText = "El numero de pedido no está registrado en la página. Disculpe las molestias";
                msjesError.Visible = true;
                logger.Error("WebException en [ConsultarEstadoPedido] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "WebException en [ConsultarEstadoPedido_ConsultarEstadoPedido] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);

            }
            #endregion
        }

        else if (ambiente == "ERQ")
        {
            #region Ambiente ERQ
            try
            {
                //Aca Configurar Otro USER/PASS
                string WsUser = ConfAmbiente.user; // "INTSOA_SKB";
                string WsPassword = ConfAmbiente.pass; // "intERPsoa2013";

                //Si la entrada de número esta vacia se muestra una grilla con todos los datos disponibles para el usuario logeado
                if (numero.Length == 0)
                {
                    GridViewPedidos.DataSource = _controlBD.ObtenerDatosFiltrados(@"select * from pedido where solicitado_por = '" + Session["rut"].ToString() + "' and E_VBELN_PEDIDO is not null");
                    GridViewPedidos.DataBind();
                    PanelTodosPedidos.Visible = true;
                    PanelInfoPedido.Visible = false;
                    return;
                }

                PanelTodosPedidos.Visible = false;
                PanelTodasCotizaciones.Visible = false;

                // Variables asociadas al WS
                ERQ.SI_Consulta_Estado_Pedido_Venta_OutService wsConsultaPedido = new ERQ.SI_Consulta_Estado_Pedido_Venta_OutService();
                //ERQ.ZB_CONS_PED wsConsultaPedido = new ERQ.ZB_CONS_PED();
                ERQ.DT_Consulta_Estado_Pedido_Venta_ResponseT_POS wsDataConsulta = new ERQ.DT_Consulta_Estado_Pedido_Venta_ResponseT_POS();
                //ERQ.DT_Web_PdoVtas_Consul wsDataConsulta = new ERQ.DT_Web_PdoVtas_Consul();
                ERQ.DT_Consulta_Estado_Pedido_Venta_Request wsDataConsultaPedido = new ERQ.DT_Consulta_Estado_Pedido_Venta_Request();
                //ERQ.DT_Web_PdoVtas_ConsulPedido wsDataConsultaPedido = new ERQ.DT_Web_PdoVtas_ConsulPedido();

                ERQ.DT_Consulta_Estado_Pedido_Venta_Response WsResponse = new ERQ.DT_Consulta_Estado_Pedido_Venta_Response();

                
                // Variables de la respuesta del pedido
                //ERQ.DT_ERP_PdoVtas_Consul WsResponse = new ERQ.DT_ERP_PdoVtas_Consul();
                //ERQ.DT_ERP_PdoVtas_ConsulPedido wsResponsePedido = new ERQ.DT_ERP_PdoVtas_ConsulPedido();
                //ERQ.DT_ERP_PdoVtas_ConsulPedidoErrores wsResponseErrores = new ERQ.DT_ERP_PdoVtas_ConsulPedidoErrores();

                //Se asigna el E_VBELN
                
                wsDataConsultaPedido.I_VBELN = numero;
                //wsDataConsultaPedido = wsDataConsultaPedido;

                //Se ejecuta la llamada al WS =)
                wsConsultaPedido.Credentials = new System.Net.NetworkCredential(WsUser, WsPassword);
                wsConsultaPedido.PreAuthenticate = true;
                WsResponse = wsConsultaPedido.SI_Consulta_Estado_Pedido_Venta_Out(wsDataConsultaPedido);

                if (WsResponse.E_AUDAT == "00000000")
                {
                    msjesError.InnerText = "Error, número de pedido no valido.";
                    msjesError.Visible = true;
                    return;
                }


                //Se asignan los datos a los campos
                cmpy_code.Text = _sapApi.GetPrefijoMarcaByOrganizacionDeVentas(WsResponse.E_VKORG);
                name_text.Text = WsResponse.E_VTEXT.Trim();
                porder_num.Text = WsResponse.E_VBELN.Trim();
                order_date.Text = string.Format(WsResponse.E_AUDAT, "{0:d}").Trim();
                goods_amt.Text = System.Convert.ToString(System.Convert.ToDouble(WsResponse.E_NETWR.Trim())).ToString(); 
                tax_amt.Text = System.Convert.ToString(System.Convert.ToDouble(WsResponse.E_KZWI5.Trim())).ToString();
                total_amt.Text = System.Convert.ToString(System.Convert.ToDouble(WsResponse.E_TOTAL.Trim())).ToString();
                numCotizacion.Text = _sapApi.GetNumCotizacionPorNpedido(WsResponse.E_VBELN.Trim());
                //numOrden.Text = SapApi.GetDataFromPedidoByPedidoId(idPedido, "ID_PEDIDO", "I");
                try
                {
                    ERQ.DT_Consulta_Estado_Pedido_Venta_ResponseT_POS[] wsResponseItemPedidos = new ERQ.DT_Consulta_Estado_Pedido_Venta_ResponseT_POS[WsResponse.T_POS.Length];
                }
                catch (NullReferenceException ex)
                {
                    msjesError.InnerText = "El pedido no se pudo rescatar desde SKBergé, contacte a su admin, el detalle de este error es el siguiente: " + ex.Message;
                    msjesError.Visible = true;
                    logger.Error("NullReferenceException en [ConsultarEstadoPedido] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
                    _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "NullReferenceException en [ConsultarEstadoPedido_ConsultarEstadoPedido] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);

                }
                GridView2.DataSource = WsResponse.T_POS;
                GridView2.DataBind();

                //PanelBusqueda.Visible = false;
                PanelInfoPedido.Visible = true;
                txtNumCotizacion.Text = "";
                txtNumPedido.Text = "";
                
                trrHideMe.Visible = false; 
            }
            catch (System.Net.WebException ex)
            {
                msjesError.InnerText = "El numero de pedido no está registrado en la página. Disculpe las molestias";
                msjesError.Visible = true;
                logger.Error("WebException en [ConsultarEstadoPedido] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "WebException en [ConsultarEstadoPedido_ConsultarEstadoPedido] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);

            }
            #endregion
        }
    
    }

    protected void btnConsPed_Click(object sender, EventArgs e)
    {
        try
        {
            PanelTodasCotizaciones.Visible = false;
            ConsultarEstadoPedido(txtNumPedido.Text);
        }
        catch (System.Net.WebException ex) 
        {
            msjesError.InnerText = "El numero de pedido no está registrado en la página. Disculpe las molestias";
            msjesError.Visible = true;
            logger.Error("WebException en [ConsultarEstadoPedido] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "WebException en [ConsultarPedido_btnConsPed_Click] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);



        }
    }


    protected void closebtn_Click(object sender, EventArgs e)
    {
        Response.Redirect("consultarpedido.aspx");
    }

    //Seleccionar tipo de búsqueda
    protected void OnSelectBusqueda(object o, EventArgs e)
    {
        //if (comboConsultar.SelectedItem.Text == "Pedido")
        //{
            PanelBusqueda.Visible = true;
            btnConsPed.Visible = true;
            btnConsCoti.Visible = false;
            lblNumBuscar.Text = "Ingrese N° de pedido: ";
            PanelInfoCotiza.Visible = false;
            txtNumCotizacion.Text = "";
            txtNumPedido.Text = "";
        //}
    }

    //Metodo para consultar cotización
    protected void ConsultarCotizacion(string numero)
    {
        msjesError.InnerText = "";
        msjesError.Visible = false;
        btnGenerarPedido.Enabled = true;
        btnDescartar.Enabled = true;
        btnResgueCart.Enabled = true; 
        LblMensajeCotiz.Text = "";
        try
        {
            _controlBD.UpdateFechasExpiracion("SP_CADUCAR_COTIZACION", numero);
            //Si la entrada de número esta vacia se muestra una grilla con todos los datos disponibles para el usuario logeado
            if (numero.Length == 0)
            {
                GridViewCotizaciones.DataSource = _controlBD.ObtenerDatosFiltrados(@"select * from pedido where solicitado_por = '" + Session["rut"].ToString() + "'");
                GridViewCotizaciones.DataBind();
                PanelTodasCotizaciones.Visible = true;
                PanelInfoCotiza.Visible = false;
                return;
            }

            PanelTodosPedidos.Visible = false;
            PanelTodasCotizaciones.Visible = false;

            txtDealer.Text = _sapApi.GetNombreDealer(Session["rut"].ToString());
            DataSet ds = _controlBD.ObtenerDatosFiltrados("select * from pedido where E_VBELN = '" + numero + "'");
            foreach (DataRow campos in ds.Tables[0].Rows)
            {
                txtNumCotizacion.Text = campos["E_VBELN"].ToString();
                txtNumCoti.Text = campos["E_VBELN"].ToString();
                txtCreacion.Text = campos["FECHA_SOLICITUD"].ToString();
                txtExpira.Text = campos["FECHA_EXPIRACION"].ToString();
                txtTotalNeto.Text = campos["TOTAL_NETO"].ToString();



                if (campos["estado"].ToString() == "VIGENTE")
                {
                    PanelConfirmaDescarta.Visible = true;
                }

                if (campos["estado"].ToString() == "CADUCADO_SAP")
                {
                    msjesError.InnerText = "Cotización no valida";
                    msjesError.Visible = true;
                    MessageBox.Show("Cotización no valida");
                    PanelConfirmaDescarta.Visible = false;
                    return; 
                }

                if (campos["estado"].ToString() == "CADUCADO_SYS")
                {
                    //_controlBD.InsertarDatos("delete from pedido where E_VBELN = '" + campos["E_VBELN"].ToString() + "'");
                    msjesError.InnerText = "Cotización caducada";
                    msjesError.Visible = true;
                    MessageBox.Show("Cotización caducada");
                    PanelInfoCotiza.Visible = false;
                    PanelConfirmaDescarta.Visible = false;
                    txtNumPedido.Text = "";
                    return;
                }

                if (campos["E_VBELN_PEDIDO"].ToString() != "")//si tiene pedido
                {
                    //hay pedido para la cotizacion
                    btnGenerarPedido.Enabled = false;
                    btnDescartar.Enabled = false;
                    btnResgueCart.Enabled = false;
                    LblMensajeCotiz.Text = "La cotización " + campos["E_VBELN"].ToString() + ", tiene pedido " + campos["E_VBELN_PEDIDO"].ToString() + " asociado.";
                 
                }

                if (campos["I_VTWEG"].ToString() == "B7") //Pedido Normal
                {
                    comboTipoPed.Text = "Normal";
                }
                else if (campos["I_VTWEG"].ToString() == "B6") //Pedido Garantia
                {
                    comboTipoPed.Text = "Garantía";
                }
                else {
                    msjesError.InnerText = "ERROR. N° Cotización esta mal creada, favor consultar";
                    msjesError.Visible = true;
                    MessageBox.Show("ERROR. N° Cotización esta mal creada, favor consultar");
                }

            }

            if (ds.Tables[0].Rows.Count == 0)
            {
                PanelInfoCotiza.Visible = false;
                PanelConfirmaDescarta.Visible = false;
                txtNumPedido.Text = "";
                msjesError.InnerText = "ERROR. N° Cotización no existe";
                msjesError.Visible = true;
                MessageBox.Show("ERROR. N° Cotización no existe");
                return;
            }

            GridViewMateriales.DataSource = _controlBD.ObtenerDatosFiltrados("select * from MATERIALES_PEDIDO where id_pedido = '" + _sapApi.getIdPedidoPorE_VBELN(txtNumCotizacion.Text) + "'");
            GridViewMateriales.DataBind();
            //txtNumCotizacion.Text = "";
            txtNumPedido.Text = "";
            PanelInfoCotiza.Visible = true;
        }
        catch (Exception ex)
        {
            logger.Error("En [ConsultaPedido] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [ConsultaPedido_ConsultarCotizacion] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);
            msjesError.InnerText = "ERROR: " + ex.Message;
            msjesError.Visible = true;
            MessageBox.Show(ex.Message);
            return;
        }
    }


    protected void btnConsCoti_Click(object sender, EventArgs e)
    {
        //consulta cotizacion 
        try
        {
            if (txtNumPedido.Text == "") 
            {
                msjesError.InnerText = "ERROR. Debe Ingresar N° de Cotización";
                msjesError.Visible = true;
                MessageBox.Show("Debe Ingresar N° de Cotización");
                return; 
            }
            PanelTodosPedidos.Visible = false;
            ConsultarCotizacion(txtNumPedido.Text);
        }
        catch (Exception ex)
        {
            logger.Error("En [ConsultaPedido] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [ConsultaPedido_btnConsCoti_Click] .Message: " + ex.Message + " .Inner: " + ex.InnerException + " .Stack: " + ex.StackTrace);
            msjesError.InnerText = "ERROR: " + ex.Message;
            msjesError.Visible = true;
            MessageBox.Show("ERROR: " + ex.Message);

        }
    }
    //Consultar estado de cotizacion desde la grilla
    protected void OnConsultarDesdeGrillaCotizacion(object o, EventArgs e)
    {
        ConsultarCotizacion(GridViewCotizaciones.SelectedRow.Cells[0].Text);
        GridViewCotizaciones.DataSource = "";
        GridViewCotizaciones.DataBind();
        PanelTodasCotizaciones.Visible = false;
    }

    //Consultar estado de pedido desde la grilla
    protected void OnConsultarDesdeGrillaPedido(object o, EventArgs e)
    {
        ConsultarEstadoPedido(GridViewPedidos.SelectedRow.Cells[0].Text);
        GridViewPedidos.DataSource = "";
        GridViewPedidos.DataBind();
        PanelTodosPedidos.Visible = false;
    }
    protected void btnSalir_Click(object sender, EventArgs e)
    {
        Response.Redirect("buscarRepto.aspx");
    }


    protected void btnDescartar_Click(object sender, EventArgs e)
    {
        try
        {

            string numCot = txtNumCotizacion.Text;
            string estado = "DESCARTADO";
            _controlBD.InsertarDatos("update pedido set estado = '"+estado+"' where E_VBELN = '"+numCot.Trim()+"'");
            msjesError.InnerText = "La cotización número " + txtNumCotizacion.Text + " ha sido descartada con exito";
            msjesError.Visible = true;
            MessageBox.Show("La cotización número " + txtNumCotizacion.Text + " ha sido descartada con exito");
            Response.Redirect("consultarpedido.aspx");
        }
         catch (Exception ex)
        {
            logger.Error("En [ConsultaPedido_btnDescartar_Click] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [ConsultaPedido_btnDescartar_Click] .Message: " + ex.Message + " .Inner: " + ex.InnerException + " .Stack: " + ex.StackTrace);
            msjesError.InnerText = "ERROR: " + ex.Message;
            msjesError.Visible = true;
            MessageBox.Show("ERROR: " + ex.Message);

        }
    }
    protected void btnGenerarPedido_Click(object sender, EventArgs e)
    {
        _confirPedido = new ConfirmarPedido();
        _pedido = new RealizarPedido();
        
        try
        {
            //VAlidar que se seleccione un tipo de pedido
            if (comboTipoPed.SelectedItem.Text == "")
            {
                MessageBox.Show("Debe seleccionar un tipo de pedido");
                return;
            }
            //Extraer el codigo de cotización
            _confirPedido.NumCotizacion = txtNumCoti.Text;

            //Extraer el tipo de pedido
            if (comboTipoPed.SelectedItem.Text == "Normal")
            {
                _confirPedido.TipoPedido = "ZBPE";
            }
            if (comboTipoPed.SelectedItem.Text == "Garantía")
            {
                _confirPedido.TipoPedido = "ZBPG";
            }


            //Determinar prioridad de pedido: 2 es normal, 1 es alta
            _confirPedido.Prioridad = RadioButtonListPrio.SelectedValue.ToString();

            if (_confirPedido.NumCotizacion == "" || _confirPedido.NumCotizacion == null) {
                msjesError.InnerText = "No se puede realizar pedido, numero de Cotizacion esta Vacio " + _confirPedido.NumCotizacion;
                msjesError.Visible = true;
                MessageBox.Show("No se puede realizar pedido, numero de Cotizacion esta Vacio " + _confirPedido.NumCotizacion);
                return;
            }

            if (_confirPedido.TipoPedido == "" || _confirPedido.TipoPedido == null)
            {
                msjesError.InnerText = "No se puede realizar pedido, tipo pedido esta Vacio " + _confirPedido.TipoPedido;
                msjesError.Visible = true;
                MessageBox.Show("No se puede realizar pedido, tipo pedido esta Vacio " + _confirPedido.TipoPedido);                
                return;
            }
            if (_confirPedido.Prioridad == "" || _confirPedido.Prioridad == null)
            {
                msjesError.InnerText = "No se puede realizar pedido, Prioridad esta Vacio " + _confirPedido.Prioridad;
                msjesError.Visible = true;
                MessageBox.Show("No se puede realizar pedido, Prioridad esta Vacio " + _confirPedido.Prioridad);
                return;
            }

             
            string errorP;
            if (!_pedido.ConfirmarPedido(_confirPedido, out errorP))
            {
                //Sin Resultados :( SAP no responde
                msjesError.InnerText = "ERROR SAP no Responde " + errorP;
                msjesError.Visible = true;
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "1- En [ConsultarPedido_(mtd)btnGenerarPedido_Click] Message: No se puede realizar pedido, Error SAP al confirmar pedido, SAP no responde .Num Cotizacion + " + _confirPedido.NumCotizacion + "  .Tipo Pedido: " + _confirPedido.TipoPedido + ".Prioridad: " + _confirPedido.Prioridad + " .Num Pedido " + _pedido.ResultPedido.NumPedido + " .Error SAP:" + errorP);
                MessageBox.Show("ERROR SAP no Responde " + errorP);
                return;
            }
            else
            {
                //Envío de correo
                msjesError.InnerText = "Su pedido ha sido confirmado con el número: " + _pedido.ResultPedido.NumPedido + ". Se enviará el detalle del pedido a su E-Mail";
                msjesError.Visible = true;
                MessageBox.Show("Su pedido ha sido confirmado con el número: " + _pedido.ResultPedido.NumPedido + ". Se enviará el detalle del pedido a su E-Mail");
                string para = _sapApi.GetCorreoUsuario(Session["rut"].ToString());
                string asunto = "Pedido de repuestos SKBERGE confirmado [NO RESPONDER]";
                string textoCorreo = "";

                textoCorreo = "Usted ha confirmado un pedido de repuestos a SAP con el código de seguimiento N°" + _pedido.ResultPedido.NumPedido;
                textoCorreo += "\nConcesionario: " + _sapApi.GetNombreDealer(Session["rut"].ToString());
                textoCorreo += "\nDireccion sucursal: " + _sapApi.GetDireccionSucursal(Session["rut"].ToString());
                textoCorreo += "\nMonto total del pedido: $" + _sapApi.GetTotalNeto(_pedido.ResultPedido.NumPedido);

                try
                {

                _controlBD.InsertarDatos(@"update pedido set E_VBELN_PEDIDO = '" + _pedido.ResultPedido.NumPedido + "' , " +
                                        " I_LPRIO_PEDIDO = " + _confirPedido.Prioridad.Trim() + " , " +
                                        " I_AUART_PEDIDO = '" + _confirPedido.TipoPedido.Trim() + "' , estado = 'CONFIRMADO' " +
                                        " where E_VBELN = '" + txtNumCoti.Text.Trim() + "'");



                }
                catch (Exception ex)
                {
                    _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "2- En [ConsultarPedido_btnGenerarPedido_Click_Error_Update_Pedido] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace + " .Num Cotizacion: " + txtNumCoti.Text.Trim() );
                    msjesError.InnerText = "Error:" + ex.Message;
                    msjesError.Visible = true;
                    MessageBox.Show("Error: " + ex.Message);
                }
                
                
                _mail.EnviarCorreo(para, asunto, textoCorreo);

                PanelInfoCotiza.Visible = false;
                //comboConsultar.SelectedIndex = 0; //Modifique PO

            }
        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "3- En [ConsultarPedido_btnGenerarPedido_Click] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);
            msjesError.InnerText = "Error: " + ex.Message ;
            msjesError.Visible = true;
            MessageBox.Show("Error: " + ex.Message);
            return;
        }
    }

    /// <summary>
    /// Este método recupera desde una cotización generada
    /// </summary>
    public void NuevoCarro()
    {
        ConsultaRepuesto _consul = new ConsultaRepuesto();
        _pedido = new RealizarPedido();
        string idSession = Session["idSession"].ToString();
        int stock = 0;
        string marca = "";

        //Obtengo la marca
        DataSet dsMar = _controlBD.ObtenerDatosFiltrados("select top 1 * from MATERIALES_PEDIDO where id_pedido = '" + _sapApi.getIdPedidoPorE_VBELN(txtNumCotizacion.Text) + "'");
        foreach (DataRow drMarca in dsMar.Tables[0].Rows)
        {
            marca = drMarca["marca"].ToString();
        }
        

        //rescatar desde la tabla MATERIALES_PEDIDO los codigos de los repuestos
        DataSet ds = _controlBD.ObtenerDatosFiltrados("select * from MATERIALES_PEDIDO where id_pedido = '" + _sapApi.getIdPedidoPorE_VBELN(txtNumCotizacion.Text) + "'");
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            //Se obtiene el grupo de material segun la marca seleccionada
            _consul.GrupoMaterial = _sapApi.GetGrupoMaterialesByMarca(marca);
            if (_consul.GrupoMaterial == "")
            {
                MessageBox.Show("Error al obtener el grupo Materiales");
                return;
            }

            //Se obtiene el codigo del repuesto
            _consul.CodRepuesto = _sapApi.GetPrefijoMarcaByGrupoMateriales(_consul.GrupoMaterial,marca) + dr["codigo"].ToString(); //se agrega marca

            //Se busca el destinatario de mercancia para este usuario
            _consul.DestinaMercacia = _sapApi.GetDestinatarioMercanciaByUserid(_sapApi.GetNombreDealer(Session["rut"].ToString()));
            if (_consul.DestinaMercacia == "")
            {
                MessageBox.Show("ERROR, no se pudo obtener el prefijo de marca");
                return;
            }

            //Se obtiene el DocVentas y el canal de distribución
            _consul.DocVentas = _sapApi.GetVkorgByGrupoMaterial(_consul.GrupoMaterial,marca);
            _consul.CanalDistribucion = _sapApi.CanalDeDistribucionPedido;
            _consul.TextoRep = "";

            if (!_pedido.BuscarRepuesto(_consul, idSession, int.Parse(dr["cantidad"].ToString()), "", marca))
            {
                MessageBox.Show("No hubo resultados en la búsqueda");
            }
            else
            {
                //Obtengo los datos desde la la tabla LISTA_BUSQUEDA_TMP
                DataSet dsR = _controlBD.ObtenerDatosFiltrados("select * from LISTA_BUSQUEDA_TMP where ID_SESSION = '" + idSession + "'");
                foreach (DataRow drR in dsR.Tables[0].Rows)
                {
                    double totalC = double.Parse(drR["precion_conce"].ToString()) * int.Parse(drR["cantidad"].ToString());
                    double totalL = double.Parse(drR["precio_lista"].ToString()) * int.Parse(drR["cantidad"].ToString());

                    string totalCF = System.Convert.ToString(totalC);
                    string totalLF = System.Convert.ToString(totalL);
                    
                    //Se insertan los datos en la tabla carro
                    stock = int.Parse(drR["stock"].ToString());
                    _controlBD.InsertarDatos(@"insert into carro(idSession,marca,grupoMat,codigo,descripcion,cantidad,stock,
                                                        precioC,totalC,precioL,totalL)
                                   values('" + idSession + "','" + marca + "', '" + _sapApi.GetGrupoMaterialesByMarca(marca) + "','" + drR["codigo"].ToString() + "','" + drR["descrip"].ToString() + "', " +
                                            "  " + drR["cantidad"].ToString() + ", " + stock + " , " + "convert(float,replace('" + drR["precion_conce"].ToString() + "',',','.'))" + " , " + "convert(float,replace('" + totalCF + "',',','.'))" + ",  " +
                                            "  " + "convert(float,replace('" + drR["precio_lista"].ToString() + "',',','.'))" + " , " + "convert(float,replace('" + totalLF + "',',','.'))" + ")");
                }

            }
        }


        Response.Redirect("buscarrepto.aspx?visible=true");

    }


    protected void btnResgueCart_Click(object sender, EventArgs e)
    {
        //se rescatan los repuestos de la cotización generada para agragarlos al carro
        //y dar la posibilidad de generar una nueva cotización
        NuevoCarro();

    }
}