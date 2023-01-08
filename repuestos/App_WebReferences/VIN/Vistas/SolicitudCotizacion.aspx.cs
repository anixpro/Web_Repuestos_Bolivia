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

public partial class Vistas_SolicitudCotizacion : System.Web.UI.Page
{
    ControlBDSolicitud _ControlBD = new ControlBDSolicitud();
    ConsultaRepuesto _consultaRep = new ConsultaRepuesto();
    PdfHelper _pdf;

    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_SolicitudCotizacion));

    string rut, _marca;
    int idsilo = 0;
    int _cantidad = 0, inicio = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        btnNuevaSoli.Visible = true;
        gvSolicitud.Visible = true;
        mjsError.Visible = false;
        descrepto.Visible = false;
        txtdescripcion.Visible = false;
        mjsError.InnerText = "";
        string query = "";
        _pdf = new PdfHelper(Server.MapPath("~\\doc"));

        btnAgregar.Click += AgregarRepto;
        btnCreaSolicitud.Click += CrearSolicitud;
        if (!IsPostBack)
        {
            LlenarComboMarcas();
            inicio = 0;
            query = "DELETE t_SolicitudCotizacion_TMP";
            _ControlBD.EjecutaQuery(query);
        }
        ComboMarcas.TabIndex = 0;
        txtCodigo.TabIndex = 1;
        txtCantidad.TabIndex = 2;

        rut = Session["rut"].ToString();
        string sesion = Session["idSession"].ToString();
        try
        {
            //_marca = ComboMarcas.SelectedItem.Text;
            //_cantidad = int.Parse(txtCantidad.Text.Trim());
        }
        catch (NullReferenceException NullEx)
        {
            logger.Error("NullReferenceException en Page Load al asignar marca y cantidad. Inner: " + NullEx.InnerException + ". Stack: " + NullEx.StackTrace);
        }
        catch (Exception ex)
        {
            logger.Error("Exception en Page Load al asignar marca y cantidad. Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
        }
        //string query = "TRUNCATE TABLE WebRepuesto..t_SolicitudCotizacion_TMP";
        //_ControlBD.EjecutaQuery(query);
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

    protected void AgregarRepto(object sender, EventArgs e)
    {
        gvSolicitud.DataSource = null;
        gvSolicitud.DataBind();
        gvSolicitud.Visible = true;
        string marca, sesion, query = "", codigo, descr = "", vin, envio;
        ControlMarca _controlMarca = new ControlMarca();
        string tipo = CombotipoSolici.SelectedValue;
        int cantidad = 0;
        rut = Session["rut"].ToString();

        //Validaciones de los campos
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

        //7 Validacion de Descripcion
        if (txtdescripcion.Visible = true)
        {
            descr = txtdescripcion.Text;
        }

        //se pregunta si cod de repuesto existe

        query = "select top 1 codigo from Repuestos where codigo=" + "'" + txtCodigo.Text + "'";
        string result = Convert.ToString(_ControlBD.ExistenciaRep(query));

        //Validacion de que exista el Repuesto
        if (result == "" && descr == "")
        {
            //si no existe se habilita campo de ingreso de descripcion del repuesto
            mjsError.InnerText = "No existe repuesto, Debe Ingresar Descripcion";
            mjsError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            txtdescripcion.Visible = true;
            descrepto.Visible = true;
            ComboMarcas.Enabled = false;
            //txtCodigo.Enabled = false;
            //txtCantidad.Enabled = false;
            //txtVin.Enabled = false;
            //CombotipoSolici.Enabled = false;
            //Combotipotrans.Enabled = false;
            return;
        }
        //else if(result == 1)
        //{ 
        //    //valida stock en SAP
        //    marca = ComboMarcas.SelectedValue;
        //    codigo = txtCodigo.Text;
        //    cantidad = Convert.ToInt32(txtCantidad.Text);

        //}
        codigo = txtCodigo.Text;
        cantidad = Convert.ToInt32(txtCantidad.Text);
        sesion = Session["idSession"].ToString();
        vin = txtVin.Text;
        envio = Combotipotrans.SelectedValue;
        descr = txtdescripcion.Text;

        if (ComboMarcas.Enabled == true)
        {
            query = "insert into WebRepuestos..t_Cotiza (fecha_cotiza,rut_cotiza, sesion,vin)";
            query = query + "Values(GETDATE()," + "'" + rut + "'" + "," + "'" + sesion + "'" + "," + "'" + vin + "'" + ")";
            _ControlBD.EjecutaQuery(query);
            string ver = "";
            if (descr.Length > 0)
            {
                ver = "exec sp_CreaSolicitudCotizacion_TMP " + "'" + rut + "'" + "," + "'" + marca + "'" + "," + "'" + descr + "'" + "," + "'" + cantidad + "'" + "," + "'" + tipo + "'" + "," + "'" + sesion + "'" + "," + "'" + vin + "'" + "," + "'" + envio + "'";
                _ControlBD.EjecutaQuery(ver);
            }
            else
            {
                ver = "exec sp_CreaSolicitudCotizacion_TMP " + "'" + rut + "'" + "," + "'" + marca + "'" + "," + "'" + codigo + "'" + "," + "'" + cantidad + "'" + "," + "'" + tipo + "'" + "," + "'" + sesion + "'" + "," + "'" + vin + "'" + "," + "'" + envio + "'";
                _ControlBD.EjecutaQuery(ver);
                txtdescripcion.Visible = false;
            }
            //SqlDataSource1.SelectCommand = "SELECT tmp.numeroSolicitud, tmp.fecha, tmp.marca, tmp.codRepto, tmp.cantidad, tmp.usuario, tmp.concesionario, tmp.tipo FROM t_SolicitudCotizacion_TMP tmp FULL JOIN WebRepuestos..t_Cotiza co ON tmp.numeroSolicitud = co.id_cotiza WHERE tmp.numeroSolicitud = co.id_cotiza";
            try
            {
                gvSolicitud.DataSource = _ControlBD.ObtenerDatosFiltrados("SELECT tmp.numeroSolicitud, tmp.fecha, tmp.marca, tmp.codRepto, tmp.cantidad, tmp.usuario, tmp.concesionario, tmp.tipo, tmp.vin, tmp.envio FROM t_SolicitudCotizacion_TMP tmp FULL JOIN WebRepuestos..t_Cotiza co ON tmp.numeroSolicitud = co.id_cotiza WHERE tmp.numeroSolicitud = co.id_cotiza");
                gvSolicitud.DataBind();
            }
            catch (Exception ex)
            {
                mjsError.InnerText = "Hubo un error con la Base de datos. Favor notificar al administrador. Rogamos disculpas las molestias";
                mjsError.Visible = true;
                logger.Error("Problemas con los reemplazos. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                return;
            }
            PanelSolicitud.Visible = true;
            ComboMarcas.Enabled = false;
            txtdescripcion.Visible = false;
        }
        else
        {
            string ver = "";
            string query2 = "SELECT numeroSolicitud FROM t_SolicitudCotizacion_TMP";
            string resol = Convert.ToString(_ControlBD.Existencia(query2));
            if (resol =="")
            {
                query = "insert into WebRepuestos..t_Cotiza (fecha_cotiza,rut_cotiza, sesion,vin)";
                query = query + "Values(GETDATE()," + "'" + rut + "'" + "," + "'" + sesion + "'" + "," + "'" + vin + "'" + ")";
                _ControlBD.EjecutaQuery(query);
                if (descr.Length > 0)
                {
                    ver = "exec sp_CreaSolicitudCotizacion_TMP " + "'" + rut + "'" + "," + "'" + marca + "'" + "," + "'" + descr + "'" + "," + "'" + cantidad + "'" + "," + "'" + tipo + "'" + "," + "'" + sesion + "'" + "," + "'" + vin + "'" + "," + "'" + envio + "'";
                    _ControlBD.EjecutaQuery(ver);
                }
                else
                {
                    ver = "exec sp_CreaSolicitudCotizacion_TMP " + "'" + rut + "'" + "," + "'" + marca + "'" + "," + "'" + codigo + "'" + "," + "'" + cantidad + "'" + "," + "'" + tipo + "'" + "," + "'" + sesion + "'" + "," + "'" + vin + "'" + "," + "'" + envio + "'";
                    _ControlBD.EjecutaQuery(ver);
                }
                //ver = "exec sp_CreaSolicitudCotizacion_TMP " + "'" + rut + "'" + "," + "'" + marca + "'" + "," + "'" + codigo + "'" + "," + "'" + cantidad + "'" + "," + "'" + tipo + "'" + "," + "'" + sesion + "'";
                //_ControlBD.EjecutaQuery(ver);
                //SqlDataSource1.SelectCommand = "SELECT tmp.numeroSolicitud, tmp.fecha, tmp.marca, tmp.codRepto, tmp.cantidad, tmp.usuario, tmp.concesionario, tmp.tipo FROM t_SolicitudCotizacion_TMP tmp FULL JOIN WebRepuestos..t_Cotiza co ON tmp.numeroSolicitud = co.id_cotiza WHERE tmp.numeroSolicitud = co.id_cotiza";
                try
                {
                    gvSolicitud.DataSource = _ControlBD.ObtenerDatosFiltrados("SELECT tmp.numeroSolicitud, tmp.fecha, tmp.marca, tmp.codRepto, tmp.cantidad, tmp.usuario, tmp.concesionario, tmp.tipo, tmp.vin, tmp.envio FROM t_SolicitudCotizacion_TMP tmp FULL JOIN WebRepuestos..t_Cotiza co ON tmp.numeroSolicitud = co.id_cotiza WHERE tmp.numeroSolicitud = co.id_cotiza");
                    gvSolicitud.DataBind();
                }
                catch (Exception ex)
                {
                    mjsError.InnerText = "Hubo un error con la Base de datos. Favor notificar al administrador. Rogamos disculpas las molestias";
                    mjsError.Visible = true;
                    logger.Error("Problemas con los reemplazos. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
                    ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                    return;
                }
                PanelSolicitud.Visible = true;
            }
            else
            {
                if (descr.Length > 0)
                {
                    ver = "exec sp_CreaSolicitudCotizacion_TMP " + "'" + rut + "'" + "," + "'" + marca + "'" + "," + "'" + descr + "'" + "," + "'" + cantidad + "'" + "," + "'" + tipo + "'" + "," + "'" + sesion + "'" + "," + "'" + vin + "'" + "," + "'" + envio + "'";
                    _ControlBD.EjecutaQuery(ver);
                }
                else
                {
                    ver = "exec sp_CreaSolicitudCotizacion_TMP " + "'" + rut + "'" + "," + "'" + marca + "'" + "," + "'" + codigo + "'" + "," + "'" + cantidad + "'" + "," + "'" + tipo + "'" + "," + "'" + sesion + "'" + "," + "'" + vin + "'" + "," + "'" + envio + "'";
                    _ControlBD.EjecutaQuery(ver);
                    txtdescripcion.Visible = false;
                }
                //ver = "exec sp_CreaSolicitudCotizacion_TMP " + "'" + rut + "'" + "," + "'" + marca + "'" + "," + "'" + codigo + "'" + "," + "'" + cantidad + "'" + "," + "'" + tipo + "'" + "," + "'" + sesion + "'";
                //_ControlBD.EjecutaQuery(ver);
                //SqlDataSource1.SelectCommand = "SELECT tmp.numeroSolicitud, tmp.fecha, tmp.marca, tmp.codRepto, tmp.cantidad, tmp.usuario, tmp.concesionario, tmp.tipo FROM t_SolicitudCotizacion_TMP tmp FULL JOIN WebRepuestos..t_Cotiza co ON tmp.numeroSolicitud = co.id_cotiza WHERE tmp.numeroSolicitud = co.id_cotiza";
                try
                {
                    gvSolicitud.DataSource = _ControlBD.ObtenerDatosFiltrados("SELECT tmp.numeroSolicitud, tmp.fecha, tmp.marca, tmp.codRepto, tmp.cantidad, tmp.usuario, tmp.concesionario, tmp.tipo, tmp.vin, tmp.envio FROM t_SolicitudCotizacion_TMP tmp FULL JOIN WebRepuestos..t_Cotiza co ON tmp.numeroSolicitud = co.id_cotiza WHERE tmp.numeroSolicitud = co.id_cotiza");
                    gvSolicitud.DataBind();
                }
                catch (Exception ex)
                {
                    mjsError.InnerText = "Hubo un error con la Base de datos. Favor notificar al administrador. Rogamos disculpas las molestias";
                    mjsError.Visible = true;
                    logger.Error("Problemas con los reemplazos. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
                    ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                    return;
                }
                PanelSolicitud.Visible = true;
                descrepto.Visible = false;
            }
        }
    }

    protected void btnNuevaSoli_Click(object sender, EventArgs e)
    {
        string query = "";
        try
        {
            query = "DELETE t_SolicitudCotizacion_TMP";
            _ControlBD.EjecutaQuery(query);
        }
        catch (Exception ex)
        {
            logger.Error("Error en [btnNuevaSoli_Click] al hacer una nueva Solicitud. Message: " + ex.Message + ".Inner:" + ex.Message + ". Stack: " + ex.StackTrace);
            return;
        }
        Response.Redirect("SolicitudCotizacion.aspx");
    }

    protected void CrearSolicitud(object sender, EventArgs e)
    {
        string query, numero, query2;
        try
        {
            numero = Convert.ToString(gvSolicitud.Rows[0].Cells[1].Text);
            Convert.ToInt32(numero);
            query = "exec sp_CreaSolicitudFinal " + numero;
            _ControlBD.EjecutaQuery(query);
            query2 = "DELETE t_SolicitudCotizacion_TMP";
            _ControlBD.EjecutaQuery(query);
            Response.Redirect("EdicionSolicitud.aspx");
        }
        catch (Exception ex)
        {
            logger.Error("Error al Crear La Solicitud de Cotizacion. Message: " + ex.Message + ".Inner:" + ex.Message + ". Stack: " + ex.StackTrace);
            return;
        }
    }
}