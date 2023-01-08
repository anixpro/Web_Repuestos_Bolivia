using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.IO;
using ClosedXML.Excel;
using OfficeOpenXml;
using NativeExcel;
using Excel1 = Microsoft.Office.Interop.Excel;
using log4net;
using System.Collections.Generic;
using Label = System.Web.UI.WebControls.Label;
using Page = System.Web.UI.Page;
using TextBox = System.Web.UI.WebControls.TextBox;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Diagnostics;
using System.Drawing;
using System.Security.Policy;
using System.Windows.Forms;
using System.Globalization;

public partial class Vistas_EdicionSolicitud : System.Web.UI.Page
{


    ControlBDSolicitud _ControlBD = new ControlBDSolicitud();
    PdfHelper _pdf;
    SendMail_helper _mail = new SendMail_helper();
    ConsultaRepuesto _consultaRep = new ConsultaRepuesto();
    SapAPI _sapApi = new SapAPI();
    RealizarPedido _pedido = new RealizarPedido();
    public static int numero = 0;
    public static string cod = string.Empty;
    public static int cantidad = 0;
    public int TotalUnitPrice;
    public decimal TotalUnitPriceDecimal;
    public string crea = string.Empty;
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_EdicionSolicitud));
    string num = "";
    public string queryexcel;
    public static string linkExcelExport = string.Empty;

    //Nuevas Variables
    SqlConnection con, con1;
    SqlCommand cmd, cmd1;
    public string gl_marca = "-1", gl_numsoli = "", gl_codre = "", gl_user = "", gl_fecdesde = "", gl_fechasta = "", gl_vin = "", gl_estadoSol = "";


    protected void Page_Load(object sender, EventArgs e)
    {

        num = Request.QueryString["numero"];
        _pdf = new PdfHelper(Server.MapPath("~\\doc"));
        string usuario = "";
        usuario = Session["rut"].ToString();
        string query = "SELECT nombre FROM persona WHERE rut = '" + usuario + "'";

        btnTerminar.Click += TerminarEdicion;
        btnBuscarSoli.Click += BuscaSolicitud;
        btnCancelar.Click += CancerlaEdicion;
        btnAgregar.Click += AgregarRepto;
        btnAddNew.Click += InsertarNewRepto;
        btnedittip.Click += HabilitarCombo;

        if (!IsPostBack)
        {
            var dato = _ControlBD.usuario(query);
            txtusuario.Text = dato;
            mjsError.Visible = false;
            mjsError.InnerText = "";
            PanelListado.Visible = false;
            PanelEdicion.Visible = false;
            btnCreaPdf.Visible = false;
            txtCodigo.Enabled = false;
            btnExportExell.Visible = false;
            //txtdescrip.Visible = false;
            txtCantidad.Enabled = false;
            txtVinAdd.Enabled = false;


            LlenarComboMarcas();
            llenarComboTipoTransporte();

        }
        try
        {
        }
        catch (NullReferenceException NullEx)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Se presento un error al ingresar] Message: " + NullEx.Message + ". Stack: " + NullEx.StackTrace + ". Inner: " + NullEx.InnerException);
            //logger.Error("NullReferenceException en Page Load al asignar marca y cantidad. Inner: " + NullEx.InnerException + ". Stack: " + NullEx.StackTrace);
        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Se presento un error al ingresar] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
            //logger.Error("Exception en Page Load al asignar marca y cantidad. Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
        }
    }
    private string MessageBox(string p, int p_2)
    {
        throw new NotImplementedException();
    }
    private void LlenarComboMarcas()
    {
        combomarcas.Items.Clear();
        combomarcas.Items.Add(new ListItem("Seleccionar", "-1"));

        //Llenar el combo box con las marcas
        foreach (string marca in _ControlBD.CrearMarcas(Session["rut"].ToString()))
        {
            combomarcas.Items.Add(marca);
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
    protected void BuscaSolicitud(object sender, EventArgs e)
    {


        gl_marca = combomarcas.Text;
        gl_numsoli = txtnumsoli.Text;
        gl_fecdesde = txtdesde.Text;
        gl_fechasta = txtHasta.Text;
        gl_codre = txtCodRep.Text;
        gl_user = txtusuario.Text;
        gl_vin = txtVin.Text;
        gl_estadoSol = ddlEstadoSol.SelectedValue;

        BuscaCotizaciones(1, gl_marca, gl_numsoli, gl_fecdesde, gl_fechasta, gl_codre, gl_vin, gl_estadoSol, "");


    }
    protected void btnCreaPdf_Click(object sender, EventArgs e)
    {
        string query = "";
        string marca;
        string text;
        string numsoli, codre, user, fecdesde, fechasta, vin, usu_rut;
        marca = combomarcas.Text;
        numsoli = txtnumsoli.Text;
        fecdesde = txtdesde.Text;
        fechasta = txtHasta.Text;
        codre = txtCodRep.Text;
        user = txtusser.Text;
        vin = txtVin.Text;
        usu_rut = Session["rut"].ToString();
        //PanelEdicion.Visible = true;
        //DataSet ds = new DataSet();

        // buscador segun filtros
        int? nro_error = null;
        string msg_error = null;

        RepuestosModelDataContext ctx = new RepuestosModelDataContext();
        var resultado = ctx.webr_datos_pdf_edita(usu_rut, marca, codre, vin, fecdesde, fechasta, user, numsoli, ref nro_error, ref msg_error).ToList();

        if (resultado.Count == 0)
        {
            Response.Redirect("EdicionSolicitud.aspx");
        }

        gvListSolicitud.DataSource = resultado;
        gvListSolicitud.DataBind();
        string soli = "";
        foreach (var fila in resultado)
        {
            soli = fila.numeroSolicitud.ToString(); // campo["numeroSolicitud"].ToString();
        }

        // se obtiene usuario creador de cotizacion
        string usuacreador = "";
        string sesioncoti = "";
        foreach (var fila in resultado)
        {
            usuacreador = fila.usuario.ToString();
            sesioncoti = fila.sesion.ToString();
        }

        string rut = "";
        DataSet dsrut = new DataSet();
        dsrut = _ControlBD.ObtenerDatosFiltrados("SELECT rut_cotiza FROM t_Cotiza WHERE sesion = '" + sesioncoti + "'");
        foreach (DataRow ru in dsrut.Tables[0].Rows)
        {
            rut = ru["rut_cotiza"].ToString();
        }

        string sess = "";
        query = "SELECT sesion FROM t_SolicitudCotizacion WHERE numeroSolicitud = '" + numsoli + "'";
        sess = Convert.ToString(_ControlBD.sesion(query));

        string filePath = _pdf.CrearSolicitudPdfComunFinal(sesioncoti, soli, rut);
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
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Se presento un error al ingresar] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
            //logger.Error("En generar PDF FileNotFoundException. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
            mjsError.Visible = true;
            mjsError.InnerText = "Error al generar documento PDF, favor contacte al administrador";
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Se presento un error al ingresar] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
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
    protected void editLinea(object sender, GridViewCommandEventArgs e)
    {
        string query, query2, query3, query4;
        string correo;
        string dat;
        string usuario;
        string rut = Session["rut"].ToString();
        string sesion = Session["idSession"].ToString();

        //Captura datos para mantener Busqueda
        gl_marca = combomarcas.Text;
        gl_numsoli = txtnumsoli.Text;
        gl_fecdesde = txtdesde.Text;
        gl_fechasta = txtHasta.Text;
        gl_codre = txtCodRep.Text;
        gl_user = txtusuario.Text;
        gl_vin = txtVin.Text;
        gl_estadoSol = ddlEstadoSol.SelectedValue;

        if (e.CommandName == "copiar")
        {
            int indice = Convert.ToInt32(e.CommandArgument);
            GridViewRow selectRow = gvListSolicitud.Rows[indice];
            Label numero = selectRow.Cells[1].FindControl("numeroSolicitud") as Label;
            Label vi = selectRow.Cells[9].FindControl("vin") as Label;
            string vin = Convert.ToString(vi.Text);
            int idsoli = Convert.ToInt32(numero.Text);
            int num = 0;
            string miSesion = Session["idSession"].ToString();
            try
            {
                con = new SqlConnection();
                cmd = new SqlCommand();

                con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
                con.Open();
                cmd.Connection = con;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "sp_CopiaSolicitud";
                cmd.CommandTimeout = 10;
                cmd.Parameters.Add("@i_numsoli", SqlDbType.Int).Value = idsoli;
                cmd.Parameters.Add("@i_session", SqlDbType.VarChar).Value = Convert.ToString(Session["idSession"].ToString());
                cmd.Parameters.Add("@o_numerosol", SqlDbType.VarChar, 10).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.ExecuteReader();

                if (System.Convert.ToInt32(cmd.Parameters["@o_nro_error"].Value) != 0)
                {
                    string text = "Se genero el siguiente error " + cmd.Parameters["@o_msg_error"].Value + "";
                    System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AlertBox", "alert('" + text + "');", true);
                }
                else
                {
                    string text = "Se creo nueva solicitud de cotizacion numero " + cmd.Parameters["@o_numerosol"].Value + "";
                    string filePath = _pdf.CrearSolicitudPdfComun(Session["idSession"].ToString(), Convert.ToString(cmd.Parameters["@o_numerosol"].Value), Session["rut"].ToString());
                    //_mail.EnviarCorreoAdjunto(correo, "Solicitud de Cotizacion", text, filePath);
                    System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AlertBox", "alert('" + text + "');", true);
                    LimpiaCampos();
                    PanelEdicion.Visible = false;
                    BuscaCotizaciones(1, gl_marca, gl_numsoli, gl_fecdesde, gl_fechasta, gl_codre, gl_vin, gl_estadoSol, "");
                    PanelListado.Visible = true;
                }

                con.Close();

            }
            catch (Exception)
            {
                throw;
            }

        }
        if (e.CommandName == "editar")
        {
            int indice = Convert.ToInt32(e.CommandArgument);
            GridViewRow selectRow = gvListSolicitud.Rows[indice];
            Label numero = selectRow.Cells[1].FindControl("numeroSolicitud") as Label;
            int idsoli = Convert.ToInt32(numero.Text);
            Label marca = selectRow.Cells[3].FindControl("marca") as Label;
            string mar = Convert.ToString(marca.Text);
            Label fec = selectRow.Cells[2].FindControl("fecha") as Label;
            string date = Convert.ToString(fec.Text);

            Label envio = selectRow.Cells[10].FindControl("envio") as Label;

            //TableCell user = selectRow.Cells[13];
            //string usser = Convert.ToString(user.Text);
            string creador = "SELECT usuario FROM t_SolicitudCotizacion Where numeroSolicitud = '" + Convert.ToInt32(idsoli) + "'";
            creador = _ControlBD.usuariocotizacion(creador);
            crea = creador;
            //string useradd = Convert.ToString(txtusuario.Text);
            txtnumero.Text = Convert.ToString(idsoli);
            Label tip = selectRow.Cells[8].FindControl("tipo") as Label;
            string tipo = Convert.ToString(tip.Text);
            Label can = selectRow.Cells[6].FindControl("cantidad") as Label;
            string canti = Convert.ToString(can.Text);
            Label soli = selectRow.Cells[9].FindControl("envio") as Label;
            string solicitud = Convert.ToString(soli.Text);
            txtmarca.Text = mar;
            txtfecsol.Text = date;
            txtusser.Text = creador.ToString();
            txttipnew.Text = tipo;
            txttransnew.Text = solicitud;
            query = "SELECT * FROM t_SolicitudCotizacion WHERE numeroSolicitud= '" + idsoli + "'";
            gvDetalleSolicitud.DataSource = _ControlBD.ObtenerDatosFiltrados(query);
            gvDetalleSolicitud.DataBind();
            gvDetalleSolicitud.Visible = true;
            PanelEdicion.Visible = true;
            PanelListado.Visible = false;
            btnCreaPdf.Visible = true;
            btnExportExell.Visible = false;

            var item = Combotipotrans.Items.FindByText(envio.Text);
            if (item != null)
            {
                Combotipotrans.SelectedValue = item.Value;
            }


            //PanelPDF.Visible = true;
        }
        if (e.CommandName == "sacar")
        {
            int indice = Convert.ToInt32(e.CommandArgument);
            GridViewRow selectRow = gvListSolicitud.Rows[indice];
            Label numero = selectRow.Cells[1].FindControl("numeroSolicitud") as Label;
            int num = Convert.ToInt32(numero.Text);
            string user = txtusuario.Text;
            string text = "Se ha Eliminado la Solicitud N° " + num;
            _ControlBD.InsertarDatos("delete from t_SolicitudCotizacion where numeroSolicitud = '" + num + "'");
            System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AlertBox", "alert('" + text + "');", true);
            gvListSolicitud.DataSource = "";
            gvListSolicitud.DataBind();
            //gvListSolicitud.DataSource = _ControlBD.ObtenerDatosFiltrados("select * from t_SolicitudCotizacion where usuario = '" + user + "'");
            //gvListSolicitud.DataBind();
            BuscaCotizaciones(1, gl_marca, gl_numsoli, gl_fecdesde, gl_fechasta, gl_codre, gl_vin, gl_estadoSol, "");
            PanelEdicion.Visible = false;
            PanelListado.Visible = true;
        }
    }
    protected void LimpiaCampos()
    {
        txtCodRep.Text = "";
        txtVin.Text = "";
        txtnumsoli.Text = "";
        PanelEdicion.Visible = false;
        gvListSolicitud.Visible = true;
        combomarcas.Enabled = true;
        //combomarcas.SelectedValue = "0";
        txtdesde.Text = "";
        txtHasta.Text = "";
        Combotipotrans.Enabled = true;
        //Combotipotrans.SelectedValue = "0";
        mjsError.Visible = false;
    }

    protected string TipoEnvio(string marc, string env)
    {
        string dias = "";
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
                    dias = result.Dias;
                }
            }
            else
            {
                mjsError.InnerText = "Transporte No Registrado";
                mjsError.Visible = true;
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                dias = "0";
            }
        }
        else
        {
            mjsError.InnerText = "Marca no Registrada";
            mjsError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            dias = "0";
        }

        return dias;
    }

    public string diasEnvio(string proveedor, string envio)
    {
        string query = "";
        string provee = proveedor;
        string tipo = envio;

        query = "SELECT " + tipo + " FROM t_Envios WHERE nom_Centro = '" + provee + "'";
        query = _ControlBD.Dias(query, tipo);
        return query;
    }
    protected void editLine(object sender, GridViewCommandEventArgs e)
    {
        string query = "";
        string estado = "";
        if (e.CommandName == "editar")
        {
            int id = 0;
            for (int i = 0; i < gvDetalleSolicitud.Rows.Count; i++)
            {
                //int indice = Convert.ToInt32(e.CommandArgument);
                GridViewRow selectRow = gvDetalleSolicitud.Rows[i];
                Label num = selectRow.Cells[1].FindControl("numsoli") as Label;
                id = Convert.ToInt32(num.Text);
                Label cod = selectRow.Cells[2].FindControl("codrepto") as Label;
                string codigo = Convert.ToString(cod.Text);
                TextBox canti = selectRow.Cells[4].FindControl("txtcantgrid") as TextBox;
                int qty = Convert.ToInt32(canti.Text);
                TextBox valor = selectRow.Cells[5].FindControl("txtvalorgrid") as TextBox;
                string preci = Convert.ToString(valor.Text);
                int total = 0;
                total = qty * Convert.ToInt32(preci);
                if (total == 0)
                {
                    estado = "A";
                }
                else
                {
                    estado = "C";
                }
                query = "UPDATE t_SolicitudCotizacion SET cantidad = '" + qty + "', precio_Solicitud = '" + total + "', Estado = '" + estado + "' WHERE numeroSolicitud = '" + id + "' AND codRepto = '" + codigo + "'";
                _ControlBD.EjecutaQuery(query);
                gvDetalleSolicitud.Visible = true;
                PanelEdicion.Visible = true;
                PanelListado.Visible = false;
            }
            gvDetalleSolicitud.DataSource = _ControlBD.ObtenerDatosFiltrados("Select * from t_SolicitudCotizacion Where numeroSolicitud = '" + id + "'");
            gvDetalleSolicitud.DataBind();
            GetUnitPrice(TotalUnitPrice);
        }
        if (e.CommandName == "sacar")
        {
            int indice = Convert.ToInt32(e.CommandArgument);
            GridViewRow selectRow = gvDetalleSolicitud.Rows[indice];
            Label numero = selectRow.Cells[1].FindControl("numsoli") as Label;
            int num = Convert.ToInt32(numero.Text);
            Label codigo = selectRow.Cells[2].FindControl("codrepto") as Label;
            string cod = Convert.ToString(codigo.Text);
            string user = txtusuario.Text;
            string text = "Se ha Eliminado el Repuesto " + cod;
            _ControlBD.InsertarDatos("DELETE FROM t_SolicitudCotizacion WHERE numeroSolicitud = '" + num + "' AND codRepto = '" + cod + "'");
            System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AlertBox", "alert('" + text + "');", true);
            gvDetalleSolicitud.DataSource = "";
            gvDetalleSolicitud.DataBind();
            gvDetalleSolicitud.DataSource = _ControlBD.ObtenerDatosFiltrados("select * from t_SolicitudCotizacion where numeroSolicitud = '" + num + "'");
            gvDetalleSolicitud.DataBind();
            gvDetalleSolicitud.Visible = true;
            PanelEdicion.Visible = true;
            PanelListado.Visible = false;
        }

        //if (e.CommandName == "anular")
        //{
        //    int indice = Convert.ToInt32(e.CommandArgument);
        //    GridViewRow selectRow = gvDetalleSolicitud.Rows[indice];
        //    Label numero = selectRow.Cells[1].FindControl("numsoli") as Label;
        //    int num = Convert.ToInt32(numero.Text);
        //    Label codigo = selectRow.Cells[2].FindControl("codrepto") as Label;
        //    string cod = Convert.ToString(codigo.Text);
        //    TextBox comentariosgr = selectRow.Cells[12].FindControl("txtvalorgridCome") as TextBox;
        //    string user = txtusuario.Text;
        //    string text = "Se ha alunado el Repuesto " + cod;
        //    _ControlBD.InsertarDatos("UPDATE t_SolicitudCotizacion SET Comentario = '" + comentariosgr.Text + "', anulada = 'SI', estado = 'C', precio_Solicitud = '0', preciounitario = 0 WHERE numeroSolicitud = '" + num + "' AND codRepto = '" + cod + "'");
        //    System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AlertBox", "alert('" + text + "');", true);
        //    gvDetalleSolicitud.DataSource = "";
        //    gvDetalleSolicitud.DataBind();
        //    gvDetalleSolicitud.DataSource = _ControlBD.ObtenerDatosFiltrados("select * from t_SolicitudCotizacion where numeroSolicitud = '" + num + "'");
        //    gvDetalleSolicitud.DataBind();
        //    gvDetalleSolicitud.Visible = true;
        //    PanelEdicion.Visible = true;
        //    PanelListado.Visible = false;
        //}

    }
    protected void TerminarEdicion(object sender, EventArgs e)
    {
        string query, query2, query3, query4;
        string text, text2;
        string correo, correo2;
        int numero = Convert.ToInt32(txtnumero.Text);
        string tipo = "";
        string envio = "";
        string rut = Session["rut"].ToString();
        string marca = txtmarca.Text;
        string filePath = "";

        int? nro_error = null;
        string msg_error = null;

        RepuestosModelDataContext ctx = new RepuestosModelDataContext();
        var registro = (from i in ctx.webr_valida_estado_cotizacion(numero,
                                                                ref nro_error,
                                                                ref msg_error)
                    select i).FirstOrDefault();

        if (registro != null )
        {
            var cerrada = registro.Estado;
            var confirmada = registro.confirmada;
            if (cerrada == "C" && confirmada == true)
            {
                return;
            }
        }


        //nuevo: validacion monto en 0
        int contador = 0;
        for (int i = 0; i < gvDetalleSolicitud.Rows.Count; i++)
        {
            GridViewRow row = gvDetalleSolicitud.Rows[i];
            //Label num = row.Cells[1].FindControl("numsoli") as Label;
            TextBox valor = row.Cells[5].FindControl("txtvalorgrid") as TextBox;
            string preci = Convert.ToString(valor.Text);

            System.Web.UI.WebControls.CheckBox chkanulada = row.Cells[11].FindControl("anulada") as System.Web.UI.WebControls.CheckBox;
            
            if (preci != null && preci != "")
            {
                preci = preci.Replace(",","");
                if (decimal.Parse(preci) == 0 && chkanulada.Checked == false)
                {
                    contador++;
                }
            }
        }

        if (contador > 0)
        {
            string textoSalida = "";
            textoSalida = "No puede terminar la cotización teniendo montos en 0 no rechazados.";
            System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AlertBox",
                                                                  "alert('" + textoSalida + "');", true);
            return;
        }

        DataSet dsDet = _ControlBD.ObtenerDatosFiltrados("select top 1 detalle from t_Cotiza where id_cotiza = " + numero + " ");
        string detalleNombre = "";
        foreach (DataRow campo in dsDet.Tables[0].Rows)
        {
            detalleNombre = campo["detalle"].ToString();
        }

        if (CombotipoSolici.SelectedValue != "0")
        {
            tipo = CombotipoSolici.SelectedValue;
        }
        //nuevo
        if (Combotipotrans.SelectedValue == "0")
        {
            mjsError.InnerText = "Debe Seleccionar tipo Transporte";
            mjsError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }
        else
        {
            envio = Combotipotrans.SelectedValue;
            //TipoEnvio(marca, envio);
        }
        //

        string proveedor = TipoEnvio(marca,envio);



        string dias = proveedor; // diasEnvio(proveedor, envio);
        //if (tipo == "" || envio == "")
        //{
        //text = "No se actualizo El Tipo de Solcitud y El Tipo de Envió";
        //System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AlertBox", "alert('" + text + "');", true);
        query2 = "SELECT * FROM t_SolicitudCotizacion WHERE numeroSolicitud= '" + numero + "'";
        gvListSolicitud.DataSource = _ControlBD.ObtenerDatosFiltrados(query2);
        gvListSolicitud.DataBind();
        query = "SELECT email FROM persona WHERE rut = '" + rut + "'";
        correo = _ControlBD.Email(query);
        //text2 = "Cotizacon Existosa " + detalleNombre.ToString();
        text2 = "Se ha respondido su solicitud de cotización número: " + numero.ToString();

        query3 = "select per.email from persona as per ";
        query3 = query3 + "inner join t_Cotiza as tc ";
        query3 = query3 + "ON per.rut = tc.rut_cotiza ";
        query3 = query3 + "Where tc.id_Cotiza='" + numero + "'";

        correo2 = _ControlBD.Email(query3);
        filePath = _pdf.CrearSolicitudPdfComunFinal(Session["idSession"].ToString(), Convert.ToString(numero), Session["rut"].ToString());
        _mail.EnviarCorreoAdjunto(correo, "Cotización Exitosa: " + detalleNombre.ToString(), text2, filePath);
        _mail.EnviarCorreoAdjunto(correo2, "Cotización Exitosa: " + detalleNombre.ToString(), text2, filePath);
        //cierra cotizacion al momento de terminar edidcion
        int id = 0;

        for (int i = 0; i < gvDetalleSolicitud.Rows.Count; i++)
        {
            GridViewRow row = gvDetalleSolicitud.Rows[i];
            Label num = row.Cells[1].FindControl("numsoli") as Label;
            id = Convert.ToInt32(num.Text);
            TextBox valor = row.Cells[5].FindControl("txtvalorgrid") as TextBox;
            string preci = Convert.ToString(valor.Text);

            if (preci != "0")
            {
                query4 = "UPDATE t_SolicitudCotizacion SET Estado = 'C' WHERE numeroSolicitud='" + id + "'";
                _ControlBD.EjecutaQuery(query4);

            }
        }
        gl_marca = combomarcas.Text;
        gl_numsoli = txtnumsoli.Text;
        gl_fecdesde = txtdesde.Text;
        gl_fechasta = txtHasta.Text;
        gl_codre = txtCodRep.Text;
        gl_user = txtusuario.Text;
        gl_vin = txtVin.Text;
        gl_estadoSol = ddlEstadoSol.SelectedValue;
        BuscaCotizaciones(1, gl_marca, gl_numsoli, gl_fecdesde, gl_fechasta, gl_codre, gl_vin, gl_estadoSol, "");

        //Nueva Prueba POROSTEGUI
        System.IO.File.Delete(filePath);
        PanelEdicion.Visible = false;
        PanelListado.Visible = true;
        btnExportExell.Visible = true;

        //TRACKING COTIZACIONES POROSTEGUI
        try
        {
            _ControlBD.InsertarDatos("INSERT INTO TRACKING_COTIZA_EDITA(usuario,id_cotizacion,fecha_edicion,via_importacion)VALUES('" + Session["rut"].ToString() + "','" + numero + "',GETDATE(),'" + envio + "')");
        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [TRACKING_EDITA_COTIZACION] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);
        }
    }
    protected void CancerlaEdicion(object sender, EventArgs e)
    {

        gl_marca = combomarcas.Text;
        gl_numsoli = txtnumsoli.Text;
        gl_fecdesde = txtdesde.Text;
        gl_fechasta = txtHasta.Text;
        gl_codre = txtCodRep.Text;
        gl_user = txtusuario.Text;
        gl_vin = txtVin.Text;
        gl_estadoSol = ddlEstadoSol.SelectedValue;

        BuscaCotizaciones(1, gl_marca, gl_numsoli, gl_fecdesde, gl_fechasta, gl_codre, gl_vin, gl_estadoSol, "");

    }
    protected void AgregarRepto(object sender, EventArgs e)
    {
        string text;
        txtCodigo.Text = "";
        txtCantidad.Text = "";
        txtVinAdd.Text = "";
        txtdescrip.Text = "";
        if (gvDetalleSolicitud.Rows.Count >= 14)
        {
            text = "La Solicitud Contiene 14 Repuestos";
            System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AlertBox", "alert('" + text + "');", true);
        }
        else
        {
            txtCodigo.Enabled = true;
            txtdescrip.Enabled = true;
            txtCantidad.Enabled = true;
            txtVinAdd.Enabled = true;
            PanelEdicion.Visible = true;
            PanelListado.Visible = false;
        }
    }
    protected void InsertarNewRepto(object sender, EventArgs e)
    {
        string query, query1, query2, query3, query4, query5, queryMotivo;
        string id = txtnumero.Text;
        string marca = txtmarca.Text;
        query1 = "SELECT tipo FROM t_SolicitudCotizacion WHERE numeroSolicitud = '" + id + "'";
        string tipo = _ControlBD.tipoSoli(query1);
        query2 = "SELECT envio FROM t_SolicitudCotizacion WHERE numeroSolicitud = '" + id + "'";
        string envio = _ControlBD.envio(query2);
        string codigo = txtCodigo.Text;
        string cantidad = txtCantidad.Text;
        string vin = txtVinAdd.Text;
        queryMotivo = "SELECT motivo FROM t_SolicitudCotizacion WHERE numeroSolicitud = '" + id + "'";
        string motivo = _ControlBD.motivoSoli(queryMotivo);
        query3 = "SELECT usuario FROM t_SolicitudCotizacion WHERE numeroSolicitud = '" + id + "'";
        string user = _ControlBD.User(query3);
        query4 = "SELECT concesionario FROM t_SolicitudCotizacion WHERE numeroSolicitud = '" + id + "'";
        string conce = _ControlBD.Concecio(query4);
        _consultaRep.GrupoMaterial = _sapApi.GetGrupoMaterialesByMarca(marca);

        try
        {
            if (_consultaRep.GrupoMaterial == "")
            {
                mjserroradd.InnerText = "Error al obtener grupo de material. Favor informar al administrador del sitio";
                mjserroradd.Visible = true;
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "Error en buscar repuesto. Usuario " + Session["rut"].ToString() + " buscando la marca no tiene grupo de materiales");
                //logger.Error("Error en buscar repuesto. Usuario " + Session["rut"].ToString() +" buscando la marca no tiene grupo de materiales");
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)",
                                                        true);
                return;
            }
            if (txtCodigo.Text.Trim().Length > 0)
            {
                //Se busca el prefijo marca de mercancia para este usuario por grupo de material
                string prefijoMarca = _sapApi.GetPrefijoMarcaByGrupoMateriales(_consultaRep.GrupoMaterial);
                if (prefijoMarca == "")
                {
                    mjserroradd.InnerText =
                        "Error al obtener grupo el prefijo de la marca. Favor informar al administrador del sitio";
                    mjserroradd.Visible = true;
                    _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "Error en buscar repuesto. No hay prefijo de marca. Usuario " + Session["rut"].ToString() + " utilizando el grupo de material " + _consultaRep.GrupoMaterial);
                    //logger.Error("Error en buscar repuesto. No hay prefijo de marca. Usuario " +Session["rut"].ToString() + " utilizando el grupo de material " +_consultaRep.GrupoMaterial);
                    ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)",
                                                            true);
                    return;
                }
                //Se concatena el prefijo de marca con el I_MFRPN
                _consultaRep.CodRepuesto = prefijoMarca.Trim().ToUpper() + txtCodigo.Text.ToUpper();
            }
            string shipCode = _sapApi.GetCodigoClienteSapByRut(Session["rut"].ToString());
            if (shipCode == "X")
            {
                shipCode = "IBC05";
            }

            _consultaRep.DestinaMercacia = shipCode;

            if (_consultaRep.DestinaMercacia == "")
            {
                mjserroradd.InnerText =
                    "Error al obtener destinatario de marcancía. Favor informar al administrador del sitio";
                mjserroradd.Visible = true;
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "Error en buscar repuesto. No hay destinatario de mercancía. Usuario " + Session["rut"].ToString());
                //logger.Error("Error en buscar repuesto. No hay destinatario de mercancía. Usuario " +Session["rut"].ToString());
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)",
                                                        true);
                return;
            }

            //Se obtiene el DocVentas y el canal de distribución
            _consultaRep.DocVentas = _sapApi.GetVkorgByGrupoMaterial(_consultaRep.GrupoMaterial);
            _consultaRep.CanalDistribucion = _sapApi.CanalDeDistribucionPedido;
            _consultaRep.TextoRep = txtdescrip.Text.ToUpper();
            _consultaRep.CantidadRep = int.Parse(txtCantidad.Text);
            Boolean stoc;
            stoc =
                Convert.ToBoolean(_pedido.BuscarRepuestoSolicitud(_consultaRep, Session["idSession"].ToString(),
                                                                  int.Parse(txtCantidad.Text.Trim()), txtdescrip.Text,
                                                                  marca));
            if (stoc == true)
            {
                mjserroradd.InnerText = "Repuesto tiene stock en SAP; Favor de realizar Cotizacion Directa.";
                mjserroradd.Visible = true;
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)",
                                                        true);
                return;
            }
            if (!_pedido.disponibilidadServicio)
            {
                mjserroradd.InnerText = _pedido.mensajeError;
                mjserroradd.Visible = true;
                mjserroradd.Style["background-color"] = "red";
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)",
                                                        true);
            }
            string result = _pedido.BuscarRepuestoDescripcion(_consultaRep, Session["idSession"].ToString(),
                                                              int.Parse(txtCantidad.Text.Trim()), txtdescrip.Text, marca);
            string descr = "", dato = "";
            if (txtdescrip.Enabled == true)
            {
                descr = txtdescrip.Text;
            }
            //Validacion de que exista el Repuesto
            if (result == "" && descr == "")
            {
                //si no existe se habilita campo de ingreso de descripcion del repuesto
                string text = "";
                text = "No existe repuesto, Debe Ingresar Descripcion";
                System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AlertBox",
                                                                      "alert('" + text + "');", true);
                //mjserroradd.InnerText = "No existe repuesto, Debe Ingresar Descripcion";
                //mjserroradd.Visible = true;
                //ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                txtdescrip.Visible = true;
                txtdescrip.Enabled = true;
                txtVinAdd.Enabled = true;
                txtCodigo.Enabled = true;
                txtCantidad.Enabled = true;
                lblDescripcion.Visible = true;
                PanelEdicion.Visible = true;
            }
            else
            {
                dato = result;

                if (dato == "")
                {
                    descr = txtdescrip.Text;
                }
                else
                {
                    descr = dato;
                    txtdescrip.Text = dato;
                }
                if (result != "")
                {
                    codigo = txtCodigo.Text;
                }
                else
                {
                    codigo = txtCodigo.Text;
                }
                string precio = "0";
                query5 = "SELECT dias FROM t_SolicitudCotizacion WHERE numeroSolicitud ='" + id + "'";
                string dia = _ControlBD.Dias(query5);
                query =
                    "INSERT INTO t_SolicitudCotizacion(numeroSolicitud, fecha, marca, codRepto, descripcion, cantidad, usuario, concesionario, precio_Solicitud, tipo, vin, envio,sesion,dias,Estado, motivo)";
                query = query + "VALUES ('" + id + "', GETDATE(),'" + marca + "','" + codigo + "','" + descr + "','" +
                        cantidad + "','" + user + "','" + conce + "','" + precio + "','" + tipo + "','" + vin + "','" +
                        envio + "','" + Convert.ToString(Session["idSession"].ToString()) + "','" + dia + "' ,'A', '" + motivo + "')";
                _ControlBD.EjecutaQuery(query);
                string query6 = "";
                query6 = "SELECT * FROM t_SolicitudCotizacion WHERE numeroSolicitud= '" + id + "'";
                gvDetalleSolicitud.DataSource = _ControlBD.ObtenerDatosFiltrados(query6);
                string texto = "Se ha Ingresado un Nuevo Itema a la Solicitud";
                System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AlertBox",
                                                                      "alert('" + texto + "');", true);
                gvDetalleSolicitud.DataBind();
                PanelEdicion.Visible = true;
                txtCodigo.Text = "";
                txtCantidad.Text = "";
                txtVinAdd.Text = "";
                txtdescrip.Text = "";
                txtdescrip.Visible = false;
                lblDescripcion.Visible = false;
            }
        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [TRACKING_EDITA_COTIZACION] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);
        }
    }
    public int GetUnitPrice(int Price)
    {
        TotalUnitPrice += Price;
        return Price;
    }
    public string GetUnitPriceDecimal(string PriceD)
    {
        TotalUnitPriceDecimal += decimal.Parse(PriceD.Replace('.',','));
        return PriceD;
    }
    public string GetTotal()
    {
        return Util.convertToMoneyPeru(TotalUnitPriceDecimal); 
    }
    protected void HabilitarCombo(object sender, EventArgs e)
    {
        txttipnew.Visible = false;
        txttransnew.Visible = false;
        //CombotipoSolici.Visible = true;
        Combotipotrans.Visible = true;
        gvDetalleSolicitud.Visible = true;
        PanelEdicion.Visible = true;
        PanelListado.Visible = false;

    }
    protected void btnExportExell_Click(object sender, EventArgs e)
    {
        DataSet dt = new DataSet();
        dt = getData();
        if (dt != null)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                //wb.Worksheets.Add(dt, "Cotizaciones");
                wb.Worksheets.Add(dt);
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=Cotizaciones.xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
        }

    }
    public DataSet getData()
    {

        string query = "";
        string marca;
        string numsoli, codre, user, fecdesde, fechasta, vin, estadoSol;
        marca = combomarcas.Text;
        numsoli = txtnumsoli.Text;
        DataSet ds2 = new DataSet();


        fecdesde = txtdesde.Text;
        fechasta = txtHasta.Text;
        codre = txtCodRep.Text;
        user = txtusuario.Text;
        vin = txtVin.Text;
        estadoSol = ddlEstadoSol.SelectedValue;


        //NUEVO METODO FILTROS POROSTEGUI
        con = new SqlConnection();
        cmd = new SqlCommand();
        try
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
            con.Open();
            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "webr_obtiene_cotizacion";
            cmd.CommandTimeout = 10;
            cmd.Parameters.Add("@i_identificador", SqlDbType.Int).Value = 5;
            cmd.Parameters.Add("@i_usuario", SqlDbType.VarChar).Value = Session["rut"].ToString();
            cmd.Parameters.Add("@i_marca", SqlDbType.VarChar).Value = marca;
            cmd.Parameters.Add("@i_nro_solicitud", SqlDbType.VarChar).Value = numsoli;
            cmd.Parameters.Add("@i_fecha_desde", SqlDbType.VarChar).Value = fecdesde;
            cmd.Parameters.Add("@i_fecha_hasta", SqlDbType.VarChar).Value = fechasta;
            cmd.Parameters.Add("@i_cod_repuesto", SqlDbType.VarChar).Value = codre;
            cmd.Parameters.Add("@i_vin", SqlDbType.VarChar).Value = vin;
            cmd.Parameters.Add("@i_estado", SqlDbType.VarChar).Value = estadoSol;
            cmd.Parameters.Add("@i_concesionario", SqlDbType.VarChar).Value = "todos";
            cmd.Parameters.Add("@i_local", SqlDbType.VarChar).Value = "";
            cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            da.Fill(ds2);
            con.Close();

            if (System.Convert.ToInt32(cmd.Parameters["@o_nro_error"].Value) != 0)
            {

            }
            else
            {
                PanelListado.Visible = true;
                btnExportExell.Visible = true;
            }
            //return ds2;
            con.Close();
        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [TRACKING_EDITA_COTIZACION] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);
            con.Close();
            ds2 = null;
        }
        return ds2;
    }
    protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
    {
        string query = "";
        string estado = "";
        int pendientes = 0;

        int? nro_error = null;
        string msg_error = null;
        int numero = Convert.ToInt32(txtnumero.Text);
        RepuestosModelDataContext ctx = new RepuestosModelDataContext();
        var registro = (from i in ctx.webr_valida_estado_cotizacion(numero,
                                                                ref nro_error,
                                                                ref msg_error)
                        select i).FirstOrDefault();

        if (registro != null)
        {
            var cerrada = registro.Estado;
            var confirmada = registro.confirmada;
            if (cerrada == "C" && confirmada == true)
            {
                return;
            }
        }

        for (int i = 0; i < gvDetalleSolicitud.Rows.Count; i++)
        {
            GridViewRow selectRow = gvDetalleSolicitud.Rows[i];
            TextBox valor = selectRow.Cells[5].FindControl("txtvalorgrid") as TextBox;
            if (valor.Text == null || valor.Text == "")
                pendientes += 1;
        }
        int id = 0;
        if (pendientes > 0)
        {
            mjsError.InnerText = "Falta completar al menos 1 fila con el precio del producto.";
            mjsError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
        }
        else
        {
            for (int i = 0; i < gvDetalleSolicitud.Rows.Count; i++)
            {
                //int indice = Convert.ToInt32(e.CommandArgument);
                GridViewRow selectRow = gvDetalleSolicitud.Rows[i];
                Label num = selectRow.Cells[1].FindControl("numsoli") as Label;
                id = Convert.ToInt32(num.Text);
                Label cod = selectRow.Cells[2].FindControl("codrepto") as Label;
                string codigo = Convert.ToString(cod.Text);
                TextBox canti = selectRow.Cells[4].FindControl("txtcantgrid") as TextBox;
                int qty = Convert.ToInt32(canti.Text);
                TextBox valor = selectRow.Cells[5].FindControl("txtvalorgrid") as TextBox;

                System.Web.UI.WebControls.CheckBox chkanulada = selectRow.Cells[11].FindControl("anulada") as System.Web.UI.WebControls.CheckBox;
                TextBox comentariosgr = selectRow.Cells[10].FindControl("txtvalorgridCome") as TextBox;
                int anulada = 0;

   
                decimal d2 = Convert.ToDecimal("745.25");
                decimal d3 = Convert.ToDecimal("758,15");

                string preci = Convert.ToString(valor.Text);
                decimal total = 0;
                preci = preci.Replace(",", "");
                preci = preci.Replace(".", ",");
                total = qty * Convert.ToDecimal(preci);
                //anulado
                if (total == 0)
                {
                    estado = "A";
                }
                else
                {
                    estado = "A";
                }

                if (chkanulada.Checked)
                {
                    anulada = 1;
                    query = "UPDATE t_SolicitudCotizacion SET Comentario = '" + comentariosgr.Text + "', anulada = " + anulada + ", estado = 'C', precio_Solicitud = '0', preciounitario = '0' WHERE numeroSolicitud = '" + id + "' AND codRepto = '" + codigo + "'";
                }
                else
                {
                    anulada = 0;
                    preci = preci.Replace(',', '.');
                    string nTotal = total.ToString().Replace(',', '.');
                    query = "UPDATE t_SolicitudCotizacion SET anulada = " + anulada + ", cantidad = '" + qty + "', precio_Solicitud = '" + nTotal + "', Estado = '" + estado + "', precioUnitario = '" + preci + "', Comentario = '" + comentariosgr.Text + "'" + " WHERE numeroSolicitud = '" + id + "' AND codRepto = '" + codigo + "'";
                }


                _ControlBD.EjecutaQuery(query);
                //gvDetalleSolicitud.Visible = true;
                //PanelEdicion.Visible = true;
                //PanelListado.Visible = false;

                //gvDetalleSolicitud.Visible = false;
                //PanelEdicion.Visible = false;
                //PanelListado.Visible = false;
                //btnCreaPdf.Visible = false;
                //btnExportExell.Visible = false;
                mjsError.InnerText = "";
                mjsError.Visible = false;
            }

            string marca = txtmarca.Text;
            string envio = "";

            //nuevo
            if (Combotipotrans.SelectedValue == "0")
            {
                mjsError.InnerText = "Debe Seleccionar tipo Transporte";
                mjsError.Visible = true;
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                return;
            }
            else
            {
                envio = Combotipotrans.SelectedValue;
                TipoEnvio(marca, envio);
            }
            //
            string dias = TipoEnvio(marca, envio);

            query = "UPDATE t_SolicitudCotizacion SET envio = '" + Combotipotrans.SelectedItem + "', dias = '" + dias + "' WHERE numeroSolicitud= '" + id + "'";
            _ControlBD.EjecutaQuery(query);

            gvDetalleSolicitud.DataSource = _ControlBD.ObtenerDatosFiltrados("Select * from t_SolicitudCotizacion Where numeroSolicitud = '" + id + "'");
            gvDetalleSolicitud.DataBind();
            GetUnitPrice(TotalUnitPrice);
        }
        

    }
    public void BuscaCotizaciones(int identificador, string marca, string numsoli, string fecdesde, string fechasta, string codre, string vin, string estadoSol, string concesionario)
    {

        try
        {
            string usuarioRut = Session["rut"].ToString();
            int? nro_error = null;
            string msg_error = null;

            RepuestosModelDataContext ctx = new RepuestosModelDataContext();
            var lista = from i in ctx.webr_obtiene_cotizacion_v2(identificador,
                                                                usuarioRut,
                                                                marca,
                                                                numsoli,
                                                                fecdesde,
                                                                fechasta,
                                                                codre,
                                                                vin,
                                                                estadoSol,
                                                                "",
                                                                "",
                                                                ref nro_error,
                                                                ref msg_error)
                        select i;

            if (lista != null)
            {
                gvListSolicitud.EmptyDataText = "No se Encontraron Datos";
                gvListSolicitud.DataSource = lista;
                gvListSolicitud.DataBind();

                PanelListado.Visible = true;
                btnExportExell.Visible = true;
                gvDetalleSolicitud.Visible = false;
                PanelEdicion.Visible = false;
                btnCreaPdf.Visible = false;
                mjsError.InnerText = "";
                mjsError.Visible = false;
            }

        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [TRACKING_EDITA_COTIZACION] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);
            con.Close();
        }
    }
    protected void CancelarEdicion(object o, EventArgs e)
    {
        string query = "";
        string query2 = "";
        int i = 0;
        DataSet ds;
        string estado = "";
        try
        {
            GridViewRow gridView = gvDetalleSolicitud.Rows[i];
            Label num = gridView.Cells[1].FindControl("numSoli") as Label;
            int numsoli = Convert.ToInt32(num.Text);

            query2 = "SELECT Estado FROM t_SolicitudCotizacion where numeroSolicitud = '" + numsoli + "'";
            ds = _ControlBD.ObtenerDatosFiltrados(query2);

            foreach (DataRow campo in ds.Tables[0].Rows)
            {
                estado = campo["Estado"].ToString();
            }

            if (estado == "A")
            {
                query = "UPDATE t_SolicitudCotizacion SET precio_Solicitud = '0', PrecioUnitario = '0', Estado ='A' WHERE numeroSolicitud = '" + numsoli + "'";
                _ControlBD.EjecutaQuery(query);
            }
            else
            {
                mjsError.InnerText = "No se puede abrir cotizacion";
                mjsError.Visible = true;
            }

            gvDetalleSolicitud.Visible = false;
            PanelEdicion.Visible = false;
            PanelListado.Visible = true;
            btnCreaPdf.Visible = false;
            btnExportExell.Visible = true;
            mjsError.InnerText = "";
            mjsError.Visible = false;
        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En Cancelar Edicion Solicitud Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);
        }
    }

    public void gvDetalleSolicitud_RowDataBound(Object sender, GridViewRowEventArgs e)
    {

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var row = e.Row;
            DataRowView dr = (DataRowView)e.Row.DataItem;

            Label lblSubTotal = (Label)row.FindControl("lblSubTotal");
            TextBox txtvalorgrid = (TextBox)row.FindControl("txtvalorgrid");
            if (lblSubTotal != null)
            {
                string text = dr["precio_Solicitud"].ToString();
                if (text == null || text == "")
                {
                    text = "0";
                }
                else
                {
                    text = text.Replace('.', ',');
                }
                GetUnitPriceDecimal(text);
                lblSubTotal.Text = Util.convertToMoneyPeru(decimal.Parse(text));
            }
            if (txtvalorgrid != null)
            {
                string text = dr["PrecioUnitario"].ToString();
                if (text == null || text == "")
                {
                    text = "0";
                }
                
                txtvalorgrid.Text = Util.convertToMoneyPeru(decimal.Parse(text)).Replace("$","");
            }

        }

    }
}