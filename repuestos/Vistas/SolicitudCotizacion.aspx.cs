using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using System.IO;
using log4net;
using log4net.Config;
using System.Data.SqlClient;
using System.Globalization;

public partial class Vistas_SolicitudCotizacion : System.Web.UI.Page
{
    ControlBDSolicitud _ControlBD = new ControlBDSolicitud();
    ControlBD _controlBD = new ControlBD();
    PdfHelper _pdf;
    ControlMarca _controlMarca = new ControlMarca();
    ConsultaRepuesto _consultaRep = new ConsultaRepuesto();
    ControlRepuestos _ctrlRepto = new ControlRepuestos();
    RespConsultaRepuesto _resConsulRepuesto = new RespConsultaRepuesto();
    RealizarPedido _pedido = new RealizarPedido();
    SapAPI _sapApi = new SapAPI();
    //SapAPI _sapApi = new SapAPI();
    SendMail_helper _mail = new SendMail_helper();

    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_SolicitudCotizacion));

    string rut, _marca;
    int idsilo = 0;
    int _cantidad = 0, inicio = 0;
    // cambio dia 28
    protected void Page_Load(object sender, EventArgs e)
    {
        Session["NoSucursal"] = "";
        ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
        scriptManager.RegisterPostBackControl(this.btnCreaSolicitud);

        btnNuevaSoli.Visible = true;
        pnSoliCoti.Visible = false;
        gvSolicitud.Visible = true;
        mjsError.Visible = false;
        descrepto.Visible = false;
        txtdescripcion.Visible = false;
        //comboenvio.Visible = false;
        //txtdesc.Visible = false;
        textProve.Visible = false;
        txtdias.Visible = false;
        dias.Visible = false;
        mjsError.InnerText = "";
        string query = "";
        _pdf = new PdfHelper(Server.MapPath("~\\doc"));

        btnAgregar.Click += AgregarRepto;
        btnCreaSolicitud.Click += CrearSolicitud;
        //gvSolicitud.SelectedIndexChanged += OnEliminarDelCArro;
        //btnCreaPdf.Click += GeneraPDF;
        //gvSolicitud.DataBound += new EventHandler(gvSolicitud_DataBound);
        if (!IsPostBack)
        {
            LlenarComboMarcas();
            llenarComboTipoTransporte();
            inicio = 0;
            query = "DELETE t_SolicitudCotizacion_TMP WHERE sesion = '" + Session["idSession"].ToString() + "'";//quitar...
            _ControlBD.EjecutaQuery(query);

            CargarMotivoPedido();
        }
        ComboMarcas.TabIndex = 0;
        txtCodigo.TabIndex = 1;
        txtCantidad.TabIndex = 2;

        rut = Session["rut"].ToString();
        string sesion = Session["idSession"].ToString();
        try
        {
            _marca = ComboMarcas.SelectedItem.Text;
            //_cantidad = int.Parse(txtCantidad.Text.Trim());
        }
        catch (NullReferenceException NullEx)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "NullReferenceException en Page Load al asignar marca y cantidad. Message: " + NullEx.Message + ". Stack: " + NullEx.StackTrace + ". Inner: " + NullEx.InnerException);
            //logger.Error("NullReferenceException en Page Load al asignar marca y cantidad. Inner: " + NullEx.InnerException + ". Stack: " + NullEx.StackTrace);
        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "Exception en Page Load al asignar marca y cantidad. Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
            //logger.Error("Exception en Page Load al asignar marca y cantidad. Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
        }
    }

    private void LlenarComboMarcas()
    {
        ComboMarcas.Items.Clear();
        ComboMarcas.Items.Add(new ListItem("Seleccionar", "-1"));

        //Llenar el combo box con las marcas
        foreach (string marca in _ControlBD.CrearMarcas(Session["rut"].ToString()))
        {
            ComboMarcas.Items.Add(marca);
        }
    }

    private void llenarComboTipoTransporte()
    {
        Combotipotrans.Items.Clear();
        Combotipotrans.Items.Add(new ListItem("Seleccionar", "0"));

        int? nro_error = null;
        string msg_error = null;

        RepuestosModelDataContext ctx = new RepuestosModelDataContext();
        var tipos = ctx.webr_obtiene_tipo_transporte(2,ref nro_error, ref msg_error).ToList();
        //Llenar el combo box con las marcas
        foreach (var tipo in tipos)
        {
            ListItem nuevoItem = new ListItem(tipo.Nombre, tipo.Id.ToString());
            Combotipotrans.Items.Add(nuevoItem);
        }
    }

    protected void AgregarRepto(object sender, EventArgs e)
    {
        pnSoliCoti.Visible = true;
        //gvSolicitud.DataSource = null;
        //gvSolicitud.DataBind();
        gvSolicitud.Visible = true;
        string marca, sesion, query = "", codigo, descr = "", vin, envio, envioText;
        string dato, dia;
        if (txtCantidad.Text == "" || txtCantidad.Text == null)
        {
            mjsError.InnerText = "Debe ingresar la cantidad de productos.";
            mjsError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }
        string tipo = CombotipoSolici.SelectedValue;
        int cantidad = 0;
        rut = Session["rut"].ToString();

        string detalle = txtDetalle.Text;

        marca = ComboMarcas.SelectedItem.Text;

        DropDownList ddlmotivodepedido = (DropDownList)PanelSolicitud.FindControl("ddlmotivodepedido");
        string motivo = "";

        //Validar que se escoja un Motivo de pedido
        if (ddlmotivodepedido.SelectedValue == "-1")
        {
            mjsError.InnerText = "Debes seleccionar un Motivo de Pedido de la lista, por favor";
            mjsError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }
        else
        {
            motivo = ddlmotivodepedido.SelectedItem.Text;
        }

        Marca marcaVehiculo = _controlMarca.obtenerMarcaPorNombre(marca);
        if (txtDetalle.Text == "" || txtDetalle.Text == null)
        {
            mjsError.InnerText = "Favor ingrese detalle";
            mjsError.Visible = true;
            return;
        }

        if (marcaVehiculo == null)
        {
            throw new Exception("La marca no ha sido ingresada al sistema. Contacte al administrador");
        }

        _consultaRep.GrupoMaterial = _sapApi.GetGrupoMaterialesByMarca(marca);
        if (_consultaRep.GrupoMaterial == "")
        {
            mjsError.InnerText = "Error al obtener grupo de material. Favor informar al administrador del sitio";
            mjsError.Visible = true;
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "Error en buscar repuesto. Usuario " + Session["rut"].ToString() + " buscando la marca " + _marca + " no tiene grupo de materiales");
            //logger.Error("Error en buscar repuesto. Usuario " + Session["rut"].ToString() + " buscando la marca " + _marca + " no tiene grupo de materiales");
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }
        //Se concatena el código ingresado por el usuario con el prefijo de la marca
        if (txtCodigo.Text.Trim().Length > 0)
        {
            //Se busca el prefijo marca de mercancia para este usuario por grupo de material
            string prefijoMarca = _sapApi.GetPrefijoMarcaByGrupoMateriales(_consultaRep.GrupoMaterial, ComboMarcas.SelectedItem.Text);
            if (prefijoMarca == "")
            {
                mjsError.InnerText = "Error al obtener grupo el prefijo de la marca. Favor informar al administrador del sitio";
                mjsError.Visible = true;
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "Error en buscar repuesto. No hay prefijo de marca. Usuario " + Session["rut"].ToString() + " utilizando el grupo de material " + _consultaRep.GrupoMaterial);
                //logger.Error("Error en buscar repuesto. No hay prefijo de marca. Usuario " + Session["rut"].ToString() + " utilizando el grupo de material " + _consultaRep.GrupoMaterial);
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                return;
            }
            //Se concatena el prefijo de marca con el I_MFRPN
            _consultaRep.CodRepuesto = prefijoMarca.Trim().ToUpper() + txtCodigo.Text.ToUpper();
        }
        string shipCode = _sapApi.GetCodigoClienteSapByRut(Session["rut"].ToString());

        // Si el destinatario de mercancía es "X", quiere decir que se trata de admin de SKBERGE
        if (shipCode == "X")
        {
            shipCode = "IBC05";
        }

        _consultaRep.DestinaMercacia = shipCode;

        if (_consultaRep.DestinaMercacia == "")
        {
            mjsError.InnerText = "Error al obtener destinatario de marcancía. Favor informar al administrador del sitio";
            mjsError.Visible = true;
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "Error en buscar repuesto. No hay destinatario de mercancía. Usuario " + Session["rut"].ToString());
            //logger.Error("Error en buscar repuesto. No hay destinatario de mercancía. Usuario " + Session["rut"].ToString());
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }

        //Se obtiene el DocVentas y el canal de distribución
        _consultaRep.DocVentas = _sapApi.GetVkorgByGrupoMaterial(_consultaRep.GrupoMaterial, ComboMarcas.SelectedItem.Text);
        _consultaRep.CanalDistribucion = _sapApi.CanalDeDistribucionPedido;
        _consultaRep.TextoRep = txtdescripcion.Text.ToUpper();
        _consultaRep.CantidadRep = int.Parse(txtCantidad.Text);

        /*Boolean stoc;
        stoc = Convert.ToBoolean(_pedido.BuscarRepuestoSolicitud(_consultaRep, Session["idSession"].ToString(), int.Parse(txtCantidad.Text.Trim()), txtdescripcion.Text, marca));
        if (stoc == true)
        {
            mjsError.InnerText = "Repuesto o cadena de reemplazo tiene stock en SAP; Favor de realizar Cotizacion Directa.";
            mjsError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }
        if (!_pedido.disponibilidadServicio)
        {
            mjsError.InnerText = _pedido.mensajeError;
            mjsError.Visible = true;
            mjsError.Style["background-color"] = "red";
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
        }
        */

        //Validaciones de los campos
        if (gvSolicitud.Rows.Count >= 14)
        {
            mjsError.InnerText = "ha Alcanzado el limite de repuestos a solicitar en la misma Solicitud";
            mjsError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }
        //1 validacion de marca
        if (ComboMarcas.SelectedValue == "-1")
        {
            mjsError.InnerText = "Debes seleccionar una marca de la lista, por favor";
            mjsError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }
        marca = ComboMarcas.SelectedItem.Text;
        Marca marcavehiculo = _controlMarca.obtenerMarcaPorNombre(marca);
        if (marcavehiculo == null)
        {
            throw new Exception("La marca no ha sido ingresada al sistema. Contacte al administrador");
        }

        //2 validacion codigo material
        if (txtCodigo.Text.Length == 0)
        {
            mjsError.InnerText = "Debe Ingresar Codigo de Material";
            mjsError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }

        //3 Validacion cantidad de material
        if (txtCantidad.Text.Length == 0)
        {
            mjsError.InnerText = "Debe Ingresar Cantidad de Material";
            mjsError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }

        //4 Validacion tipo de solicitud
        if (CombotipoSolici.SelectedValue == "0")
        {
            mjsError.InnerText = "Debe Seleccionar tipo de Solicitud";
            mjsError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }

        //5 Validacion de Vin
        if (txtVin.Text.Length == 0 || txtVin.Text.Length > 17)
        {
            mjsError.InnerText = "Debe Ingresar un Vin Valido";
            mjsError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }

        //6 Validacion de transporte
        if (Combotipotrans.SelectedValue == "0")
        {
            mjsError.InnerText = "Debe Seleccionar tipo Transporte";
            mjsError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }
        else
        {
            marca = ComboMarcas.SelectedValue;
            envio = Combotipotrans.SelectedValue;
            envioText = Combotipotrans.SelectedItem.Text;
            TipoEnvio(marca, envio);
        }
        if (txtdescripcion.Enabled == true)
        {
            descr = txtdescripcion.Text;
        }
        //se pregunta si cod de repuesto existe

        query = "select top 1 codigo from Repuestos where codigo=" + "'" + txtCodigo.Text + "'";
        string result = _pedido.BuscarRepuestoDescripcion(_consultaRep, Session["idSession"].ToString(), int.Parse(txtCantidad.Text.Trim()), txtdescripcion.Text, marca);

        //Validacion de que exista el Repuesto
        if (result == "" && descr == "")
        {
            //si no existe se habilita campo de ingreso de descripcion del repuesto
            mjsError.InnerText = "No existe repuesto o no se conoce su descripción. Favor ingresar el dato.";
            mjsError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            txtdescripcion.Visible = true;
            descrepto.Visible = true;
            ComboMarcas.Enabled = false;
            CombotipoSolici.Enabled = false;
            ddlmotivodepedido.Enabled = false;//NUEVO PO
            return;
        }
        else
        {
            dato = result;
        }

        gvSolicitud.DataSource = null;
        gvSolicitud.DataBind();

        if (dato == "")
        {
            descr = txtdescripcion.Text;
        }
        else
        {
            descr = dato;
            txtdescripcion.Text = dato;
        }
        if (result != "")
        {
            codigo = txtCodigo.Text;
        }
        else
        {
            codigo = txtCodigo.Text;
        }
        cantidad = Convert.ToInt32(txtCantidad.Text);
        sesion = Session["idSession"].ToString();
        vin = txtVin.Text;
        if (Combotipotrans.SelectedValue != "0")
        {
            envio = Combotipotrans.Text;
        }

        dia = txtdias.Text;
        Combotipotrans.Enabled = false;

        string querycod, unidad, queryMotivo;
        int update = 0;
        querycod = "SELECT cantidad FROM t_SolicitudCotizacion_TMP WHERE codRepto = '" + codigo + "'";
        unidad = _ControlBD.ConsultarExiRepto(querycod);

        //Obtiene codigo del motivo
        queryMotivo = "SELECT MP_codigo FROM Motivo_Pedido WHERE mp_Marca = '" + ComboMarcas.SelectedValue + "' AND MP_Descripcion = '" + ddlmotivodepedido.SelectedValue + "' ";
        motivo = _ControlBD.consultarCodigoMotivoMarca(queryMotivo);


        if (ComboMarcas.Enabled == true)
        {
            query = "insert into t_Cotiza (fecha_cotiza,rut_cotiza, sesion,vin, detalle)";
            query = query + "Values(GETDATE()," + "'" + rut + "'" + "," + "'" + sesion + "'" + "," + "'" + vin + "'" + "," + "'" + detalle + "'" + ")";
            _ControlBD.EjecutaQuery(query);
            string ver = "";
            if (descr != "")
            {
                ver = "exec sp_CreaSolicitudCotizacion_TMP " + "'" + rut + "'" + "," + "'" + marca + "'" + "," + "'" + codigo + "'" + "," + "'" + descr + "'" + "," + "'" + cantidad + "'" + "," + "'" + tipo + "'" + "," + "'" + sesion + "'" + "," + "'" + vin + "'" + "," + "'" + envioText + "'" + "," + "'" + dia + "'" + "," + "'" + motivo + "'";
                _ControlBD.EjecutaQuery(ver);
                txtdescripcion.Text = descr;
                Combotipotrans.Enabled = false;
            }
            else
            {
                ver = "exec sp_CreaSolicitudCotizacion_TMP " + "'" + rut + "'" + "," + "'" + marca + "'" + "," + "'" + codigo + "'" + "," + "'" + descr + "'" + "," + "'" + cantidad + "'" + "," + "'" + tipo + "'" + "," + "'" + sesion + "'" + "," + "'" + vin + "'" + "," + "'" + envioText + "'" + "," + "'" + dia + "'" + "," + "'" + motivo + "'";
                _ControlBD.EjecutaQuery(ver);
                txtdescripcion.Visible = false;
                descrepto.Visible = false;
                Combotipotrans.Enabled = false;
            }
            try
            {
                aviso.Visible = true;
                //gvSolicitud.DataSource = _ControlBD.ObtenerDatosFiltrados("SELECT tmp.numeroSolicitud, CONVERT(VARCHAR,tmp.fecha,103) + ' ' + CONVERT(nvarchar(10), GETDATE(), 108) as fecha , tmp.marca, tmp.codRepto, tmp.descripcion, tmp.cantidad, tmp.usuario, tmp.concesionario, tmp.tipo, tmp.vin, tmp.envio, tmp.dias, tmp.motivo FROM t_SolicitudCotizacion_TMP tmp FULL JOIN WebRepuestos..t_Cotiza co ON tmp.numeroSolicitud = co.id_cotiza WHERE tmp.numeroSolicitud = co.id_cotiza AND tmp.sesion = '" + sesion + "'");
                gvSolicitud.DataSource = _ControlBD.ObtenerDatosFiltrados("SELECT tmp.numeroSolicitud, CONVERT(VARCHAR,tmp.fecha,103) + ' ' + CONVERT(nvarchar(10), GETDATE(), 108) as fecha , tmp.marca, tmp.codRepto, tmp.descripcion, tmp.cantidad, tmp.usuario, tmp.concesionario, tmp.tipo, tmp.vin, tmp.envio, tmp.dias, (SELECT MP_Descripcion FROM Motivo_Pedido WHERE mp_Marca = tmp.marca AND LTRIM(RTRIM(MP_codigo)) = LTRIM(RTRIM(tmp.motivo)) AND MP_Flag = '1') as motivo FROM t_SolicitudCotizacion_TMP tmp FULL JOIN t_Cotiza co ON tmp.numeroSolicitud = co.id_cotiza WHERE tmp.numeroSolicitud = co.id_cotiza AND tmp.sesion = '" + sesion + "'");
                gvSolicitud.DataBind();
            }
            catch (Exception ex)
            {
                mjsError.InnerText = "Hubo un error con la Base de datos. Favor notificar al administrador. Rogamos disculpas las molestias";
                mjsError.Visible = true;
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "Problemas con los reemplazos. Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
                //logger.Error("Problemas con los reemplazos. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                return;
            }
            PanelSolicitud.Visible = true;
            gvSolicitud.Visible = true;
            ComboMarcas.Enabled = false;
            CombotipoSolici.Enabled = false;
            ddlmotivodepedido.Enabled = false;//NUEVO PO
            txtdescripcion.Visible = false;
            Combotipotrans.Enabled = false;
            aviso.Visible = true;
        }
        else
        {
            string ver = "";
            string query2 = "SELECT numeroSolicitud FROM t_SolicitudCotizacion_TMP WHERE sesion = '" + sesion + "'"; //Cambio 1
            string resol = Convert.ToString(_ControlBD.Existencia(query2));
            if (resol == "")
            {
                query = "insert into t_Cotiza (fecha_cotiza,rut_cotiza, sesion,vin, detalle)";
                query = query + "Values(GETDATE()," + "'" + rut + "'" + "," + "'" + sesion + "'" + "," + "'" + vin + "'" + "," + "'" + detalle + "'" + ")";
                _ControlBD.EjecutaQuery(query);
                if (descr != "")
                {
                    ver = "exec sp_CreaSolicitudCotizacion_TMP " + "'" + rut + "'" + "," + "'" + marca + "'" + "," + "'" + codigo + "'" + "," + "'" + descr + "'" + "," + "'" + cantidad + "'" + "," + "'" + tipo + "'" + "," + "'" + sesion + "'" + "," + "'" + vin + "'" + "," + "'" + envioText + "'" + "," + "'" + dia + "'" + "," + "'" + motivo + "'";
                    _ControlBD.EjecutaQuery(ver);
                    txtdescripcion.Text = descr;
                    Combotipotrans.Enabled = false;
                }
                else
                {
                    ver = "exec sp_CreaSolicitudCotizacion_TMP " + "'" + rut + "'" + "," + "'" + marca + "'" + "," + "'" + codigo + "'" + "," + "'" + descr + "'" + "," + "'" + cantidad + "'" + "," + "'" + tipo + "'" + "," + "'" + sesion + "'" + "," + "'" + vin + "'" + "," + "'" + envioText + "'" + "," + "'" + dia + "'" + "," + "'" + motivo + "'";
                    _ControlBD.EjecutaQuery(ver);
                    txtdescripcion.Visible = false;
                    descrepto.Visible = false;
                    Combotipotrans.Enabled = false;
                }
                try
                {
                    aviso.Visible = true;
                    //cambio 2
                    //gvSolicitud.DataSource = _ControlBD.ObtenerDatosFiltrados("SELECT tmp.numeroSolicitud, CONVERT(VARCHAR,tmp.fecha,103) + ' ' + CONVERT(nvarchar(10), GETDATE(), 108) as fecha , tmp.marca, tmp.codRepto, tmp.descripcion, tmp.cantidad, tmp.usuario, tmp.concesionario, tmp.tipo, tmp.vin, tmp.envio, tmp.dias, tmp.motivo FROM t_SolicitudCotizacion_TMP tmp FULL JOIN WebRepuestos..t_Cotiza co ON tmp.numeroSolicitud = co.id_cotiza WHERE tmp.numeroSolicitud = co.id_cotiza AND tmp.sesion = '" + sesion + "'");
                    gvSolicitud.DataSource = _ControlBD.ObtenerDatosFiltrados("SELECT tmp.numeroSolicitud, CONVERT(VARCHAR,tmp.fecha,103) + ' ' + CONVERT(nvarchar(10), GETDATE(), 108) as fecha , tmp.marca, tmp.codRepto, tmp.descripcion, tmp.cantidad, tmp.usuario, tmp.concesionario, tmp.tipo, tmp.vin, tmp.envio, tmp.dias, (SELECT MP_Descripcion FROM Motivo_Pedido WHERE mp_Marca = tmp.marca AND LTRIM(RTRIM(MP_codigo)) = LTRIM(RTRIM(tmp.motivo)) AND MP_Flag = '1') as motivo FROM t_SolicitudCotizacion_TMP tmp FULL JOIN t_Cotiza co ON tmp.numeroSolicitud = co.id_cotiza WHERE tmp.numeroSolicitud = co.id_cotiza AND tmp.sesion = '" + sesion + "'");
                    gvSolicitud.DataBind();
                }
                catch (Exception ex)
                {
                    mjsError.InnerText = "Hubo un error con la Base de datos. Favor notificar al administrador. Rogamos disculpas las molestias";
                    mjsError.Visible = true;
                    _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "Problemas con los reemplazos. Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
                    //logger.Error("Problemas con los reemplazos. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
                    ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                    return;
                }
                PanelSolicitud.Visible = true;
                gvSolicitud.Visible = true;
                descrepto.Visible = true;
                txtdescripcion.Visible = true;
                Combotipotrans.Enabled = false;
                aviso.Visible = true;
            }
            else
            {
                if (descr != "")
                {
                    ver = "exec sp_CreaSolicitudCotizacion_TMP " + "'" + rut + "'" + "," + "'" + marca + "'" + "," + "'" + codigo + "'" + "," + "'" + descr + "'" + "," + "'" + cantidad + "'" + "," + "'" + tipo + "'" + "," + "'" + sesion + "'" + "," + "'" + vin + "'" + "," + "'" + envioText + "'" + "," + "'" + dia + "'" + "," + "'" + motivo + "'";
                    _ControlBD.EjecutaQuery(ver);
                    txtdescripcion.Text = "";
                    Combotipotrans.Enabled = false;
                }
                else
                {
                    ver = "exec sp_CreaSolicitudCotizacion_TMP " + "'" + rut + "'" + "," + "'" + marca + "'" + "," + "'" + codigo + "'" + "," + "'" + descr + "'" + "," + "'" + cantidad + "'" + "," + "'" + tipo + "'" + "," + "'" + sesion + "'" + "," + "'" + vin + "'" + "," + "'" + envioText + "'" + "," + "'" + dia + "'" + "," + "'" + motivo + "'";
                    _ControlBD.EjecutaQuery(ver);
                    txtdescripcion.Visible = false;
                    descrepto.Visible = false;
                    Combotipotrans.Enabled = false;
                }
                try
                {
                    aviso.Visible = true;
                    //cambio 3
                    string queryConsulta;
                    //queryConsulta = "SELECT tmp.numeroSolicitud, CONVERT(VARCHAR,tmp.fecha,103) + ' ' + CONVERT(nvarchar(10), GETDATE(), 108) as fecha , tmp.marca, tmp.codRepto, tmp.descripcion, tmp.cantidad, tmp.usuario, tmp.concesionario, tmp.tipo, tmp.vin, tmp.envio, tmp.dias, tmp.motivo FROM t_SolicitudCotizacion_TMP tmp FULL JOIN WebRepuestos..t_Cotiza co ON tmp.numeroSolicitud = co.id_cotiza WHERE tmp.numeroSolicitud = co.id_cotiza AND tmp.sesion = '" + sesion + "'";
                    queryConsulta = "SELECT tmp.numeroSolicitud, CONVERT(VARCHAR,tmp.fecha,103) + ' ' + CONVERT(nvarchar(10), GETDATE(), 108) as fecha , tmp.marca, tmp.codRepto, tmp.descripcion, tmp.cantidad, tmp.usuario, tmp.concesionario, tmp.tipo, tmp.vin, tmp.envio, tmp.dias, (SELECT MP_Descripcion FROM Motivo_Pedido WHERE mp_Marca = tmp.marca AND LTRIM(RTRIM(MP_codigo)) = LTRIM(RTRIM(tmp.motivo)) AND MP_Flag = '1') as motivo FROM t_SolicitudCotizacion_TMP tmp FULL JOIN t_Cotiza co ON tmp.numeroSolicitud = co.id_cotiza WHERE tmp.numeroSolicitud = co.id_cotiza AND tmp.sesion = '" + sesion + "'";
                    gvSolicitud.DataSource = _ControlBD.ObtenerDatosFiltrados(queryConsulta);
                    gvSolicitud.DataBind();
                }
                catch (Exception ex)
                {
                    mjsError.InnerText = "Hubo un error con la Base de datos. Favor notificar al administrador. Rogamos disculpas las molestias";
                    mjsError.Visible = true;
                    _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "Problemas con los reemplazos. Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
                    //logger.Error("Problemas con los reemplazos. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
                    ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                    return;
                }
                PanelSolicitud.Visible = true;
                descrepto.Visible = true;
                txtdescripcion.Visible = true;
                Combotipotrans.Enabled = false;
                aviso.Visible = true;
            }
        }
    }

    protected void btnNuevaSoli_Click(object sender, EventArgs e)
    {
        string query = "";
        try
        {
            query = "DELETE t_SolicitudCotizacion_TMP WHERE sesion = '" + Session["idSession"].ToString() + "'";
            _ControlBD.EjecutaQuery(query);
        }
        catch (Exception ex)
        {
            logger.Error("Error en [btnNuevaSoli_Click] al hacer una nueva Solicitud. Message: " + ex.Message + ".Inner:" + ex.Message + ". Stack: " + ex.StackTrace);
            return;
        }
        txtCodigo.Text = "";
        txtdescripcion.Text = "";
        txtCantidad.Text = "";
        txtVin.Text = "";
        textProve.Text = "";
        txtdias.Text = "";
        btnNuevaSoli.Visible = true;
        pnSoliCoti.Visible = false;
        gvSolicitud.Visible = true;
        ComboMarcas.Enabled = true;
        CombotipoSolici.Enabled = true;
        //ComboMarcas.SelectedValue = "0";
        ddlmotivodepedido.Enabled = true;
        CombotipoSolici.Enabled = true;
        //CombotipoSolici.SelectedValue = "0";
        Combotipotrans.Enabled = true;
        //Combotipotrans.SelectedValue = "0";
        mjsError.Visible = false;
        descrepto.Visible = false;
        txtdescripcion.Visible = false;
        //comboenvio.Visible = false;
        //txtdesc.Visible = false;
        textProve.Visible = false;
        txtdias.Visible = false;
        dias.Visible = false;
    }

    protected void CrearSolicitud(object sender, EventArgs e)
    {
        string query, numero, query2;
        string sesion = Session["idSession"].ToString();
        string rut = Session["rut"].ToString();
        string correo, dia;
        int can = 0;
        DateTime fecha;
        string marca, cod, descr, user, conce, precio, tipo, vin, envio, ses, estado, motivo;
        try
        {
            int gvCount = gvSolicitud.Rows.Count;
            if (gvCount > 0)
            {
                string fechaNow, queryMotivo = "";
                DateTime localDate = DateTime.Now;
                foreach (GridViewRow gvRow in gvSolicitud.Rows)
                {
                    numero = gvRow.Cells[1].Text;
                    //fecha = DateTime.ParseExact(gvRow.Cells[3].Text,"dd/MM/yyyy",CultureInfo.InvariantCulture);
                    marca = gvRow.Cells[5].Text;
                    cod = gvRow.Cells[7].Text;
                    descr = gvRow.Cells[9].Text;
                    can = Convert.ToInt32(gvRow.Cells[11].Text);
                    user = gvRow.Cells[13].Text;
                    conce = gvRow.Cells[15].Text;
                    precio = "0";
                    tipo = gvRow.Cells[17].Text;
                    vin = gvRow.Cells[19].Text;
                    envio = gvRow.Cells[21].Text;
                    dia = gvRow.Cells[23].Text;
                    motivo = gvRow.Cells[24].Text;
                    ses = Session["idSession"].ToString();
                    Convert.ToInt32(numero);
                    estado = "A";

                    //Obtiene codigo del motivo
                    queryMotivo = "SELECT MP_codigo FROM Motivo_Pedido WHERE mp_Marca = '" + ComboMarcas.SelectedValue + "' AND MP_Descripcion = '" + motivo + "' ";
                    motivo = _ControlBD.consultarCodigoMotivoMarca(queryMotivo);

                    query = "INSERT INTO t_SolicitudCotizacion(numeroSolicitud, fecha, marca, codRepto, descripcion, cantidad, usuario, concesionario, precio_Solicitud, tipo, vin, envio,sesion,dias,Estado, motivo)";
                    query = query + " VALUES ('" + numero + "',convert(date,'" + localDate + "',103), '" + marca + "', '" + cod + "', '" + descr + "', '" + can + "', '" + user + "', '" + conce + "', '" + precio + "', '" + tipo + "', '" + vin + "', '" + envio + "', '" + ses + "','" + dia + "','" + estado + "','" + motivo + "')";
                    _ControlBD.EjecutaQuery(query);

                    motivo = "";
                }
                numero = gvSolicitud.Rows[0].Cells[1].Text.ToString();
                Convert.ToInt32(numero);
                query2 = "SELECT email FROM persona WHERE rut = '" + rut + "'";
                correo = _ControlBD.Email(query2);
                string text = "Se creó nueva solicitud de cotización número " + numero + "";
                string filePath = _pdf.CrearSolicitudPdfComun(Session["idSession"].ToString(), numero, Session["rut"].ToString());
                _mail.EnviarCorreoAdjunto(correo, "Solicitud de Cotización Realizada: " + txtDetalle.Text, text, filePath);
                query2 = "DELETE t_SolicitudCotizacion_TMP WHERE sesion = '" + Session["idSession"].ToString() + "'";
                _ControlBD.EjecutaQuery(query2);

                string mjs = "Solicitud de Cotizacion creada con numero :" + numero;
                System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AlertBox", "alert('" + mjs + "');", true);
                LimpiaCampos();
            }
            else
            {
                string mjs = "Para crear la solicitud debe tener al menos 1 detalle registrado.";
                System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AlertBox", "alert('" + mjs + "');", true);
            }
            

        }
        catch (Exception ex)
        {
            mjsError.InnerText = "Hubo un error al crear la Solicitud de Cotizacion" + ex.Message;
            logger.Error("Error al Crear La Solicitud de Cotizacion. Message: " + ex.Message + ".Inner:" + ex.Message + ". Stack: " + ex.StackTrace);
            mjsError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Error Crea Solicitud Cotización] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
            return;
        }

    }

    protected void OnEliminarDelCArro(object sender, GridViewCommandEventArgs e)
    {

        if (e.CommandName == "quitar")
        {
            int indice = Convert.ToInt32(gvSolicitud.SelectedRow);
            GridViewRow row = gvSolicitud.Rows[indice];

            string num = row.Cells[1].Text.ToString();
            string cod = row.Cells[6].Text.ToString();
            string canti = row.Cells[11].Text.ToString();
            int cantidad = Convert.ToInt32(canti);
            string sesion = Session["idSession"].ToString();
            _ControlBD.InsertarDatos("delete from t_SolicitudCotizacion_TMP where sesion = '" + sesion + "' and numeroSolicitud = '" + num + "' and codRepto = '" + cod + "' and cantidad = '" + cantidad + "'");

            gvSolicitud.DataSource = "";
            gvSolicitud.DataBind();
            gvSolicitud.DataSource = _ControlBD.ObtenerDatosFiltrados("select * from t_SolicitudCotizacion_TMP where sesion = '" + sesion + "'");
            gvSolicitud.DataBind();

            if (gvSolicitud.Rows.Count == 0)
            {
                pnSoliCoti.Visible = false;
            }
            pnSoliCoti.Visible = true;
            textProve.Visible = true;
            dias.Visible = true;
            txtdias.Visible = true;

        }
        //obtengo el codigo del repuesto a eliminar del carro

    }

    protected void TipoEnvio(string marc, string env)
    {

        int? nro_error = null;
        string msg_error = null;

        if (marc != "-1")
        {
            if (env != "0")
            {
                RepuestosModelDataContext ctx = new RepuestosModelDataContext();
                var result = ctx.webr_obtiene_plazo_envio(marc, env, ref nro_error, ref msg_error).SingleOrDefault();
                if (result != null)
                {
                    txtdias.Text = result.Dias;
                    textProve.Text = result.Nombre;
                    textProve.Visible = false;
                    txtdias.Visible = true;
                    dias.Visible = true;
                }             
            }
            else
            {
                mjsError.InnerText = "Transporte No Registrado";
                mjsError.Visible = true;
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                return;
            }
        }
        else
        {
            mjsError.InnerText = "Marca no Registrada";
            mjsError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }
    }

    protected void btnCreaPdf_Click(object sender, EventArgs e)
    {
        DataSet ds = _ControlBD.ObtenerDatosFiltrados("select * from t_SolicitudCotizacion_TMP where sesion = '" + Session["idSession"].ToString() + "'");
        if (ds.Tables[0].Rows.Count == 0)
        {
            Response.Redirect("SolicitudCotizacion.aspx");
        }
        string soli = "";
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            soli = campo["numeroSolicitud"].ToString();
        }
        //gvSolicitud.DataSource = "";
        //gvSolicitud.DataBind();

        string filePath = _pdf.CrearSolicitudPdfComun(Session["idSession"].ToString(), soli, Session["rut"].ToString());
        Boolean siGeneraArchivo = false;
        try
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(string.Format("Disculpa, el archivo '{0}' fue eliminado del servidor!", filePath));
            }
            siGeneraArchivo = true;
        }
        catch (FileNotFoundException ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En generar PDF FileNotFoundException. Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
            //logger.Error("En generar PDF FileNotFoundException. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
            mjsError.Visible = true;
            mjsError.InnerText = "Error al generar documento PDF, favor contacte al administrador";
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En generar PDF FileNotFoundException. Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
            //logger.Error("En generar PDF Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
            mjsError.Visible = true;
            mjsError.InnerText = "Error al generar documento PDF, favor contacte al administrador";
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
        }

        var fi = new FileInfo(filePath);
        Response.Clear();
        Response.AddHeader("Content-Disposition", String.Format("attachment; filename=\"{0}\"", "CotizacionRespuestos.pdf"));
        Response.AddHeader("Content-Length", fi.Length.ToString());
        Response.ContentType = "application/octet-stream";
        Response.WriteFile(fi.FullName);
        Response.End();
    }

    protected void LimpiaCampos()
    {
        txtCodigo.Text = "";
        txtdescripcion.Text = "";
        txtCantidad.Text = "";
        txtVin.Text = "";
        textProve.Text = "";
        txtdias.Text = "";
        btnNuevaSoli.Visible = true;
        pnSoliCoti.Visible = false;
        gvSolicitud.Visible = true;
        ComboMarcas.Enabled = true;
        CombotipoSolici.Enabled = true;
        //ComboMarcas.SelectedValue = "0";
        CombotipoSolici.Enabled = true;
        //CombotipoSolici.SelectedValue = "0";
        Combotipotrans.Enabled = true;
        //Combotipotrans.SelectedValue = "0";
        mjsError.Visible = false;
        descrepto.Visible = false;
        txtdescripcion.Visible = false;
        //comboenvio.Visible = false;
        //txtdesc.Visible = false;
        textProve.Visible = false;
        txtdias.Visible = false;
        dias.Visible = false;
        txtDetalle.Text = "";
    }

    protected void btnCancelar_Click(object sender, EventArgs e)
    {
        string query2 = "";
        query2 = "DELETE t_SolicitudCotizacion_TMP WHERE sesion = '" + Session["idSession"].ToString() + "'";
        _ControlBD.EjecutaQuery(query2);
        LimpiaCampos();

    }

    protected void txtDetalle_TextChanged(object sender, EventArgs e)
    {
        if (txtDetalle.Text == "" || txtDetalle.Text == null)
        {
            lblDetalle.Visible = true;
        }
    }

    private void CargarMotivoPedido()
    {

        DropDownList ddlmotivodepedido = (DropDownList)PanelSolicitud.FindControl("ddlmotivodepedido");
        _marca = ComboMarcas.SelectedItem.Text;
        ddlmotivodepedido.Items.Clear();
        ddlmotivodepedido.Items.Add(new ListItem("Seleccionar", "-1"));
        foreach (string pedido in _controlBD.TipoPedido(_marca))
        {
            ddlmotivodepedido.Items.Add(pedido);
        }
    }

    protected void ComboMarcas_SelectedIndexChanged(object sender, EventArgs e)
    {
        CargarMotivoPedido();
    }
}