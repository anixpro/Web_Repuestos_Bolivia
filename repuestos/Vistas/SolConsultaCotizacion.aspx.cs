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
using CheckBox = System.Web.UI.WebControls.CheckBox;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Diagnostics;
using System.Drawing;
using System.Security.Policy;
using System.Windows.Forms;
using System.Globalization;
using SaveVFC = QAS.PERU.SaveVFC; //cl.humano2.skbergecl.SaveVFC;
using SaveVFCProductivo =pe.com.skberge.crm.SaveVFC;
using System.Xml;
using System.Xml.Linq;
using System.Data.Common;
using Newtonsoft.Json;

public partial class Vistas_SolConsultaCotizacion : System.Web.UI.Page
{

    ControlBD _controlBD = new ControlBD();
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
    public string crea = string.Empty;
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_SolConsultaCotizacion));
    string num = "";
    public string queryexcel;
    public static string linkExcelExport = string.Empty;
    static int numSolicitud;
    static int marcaEdit = 0;
    static int editFin = 0;
    static int confirmarsucursal = 0;
    static int confirmaregistros = 0;
    string flagIdDealer;
    string shipCodeDealer;
    public int flagRut;

    //Nuevas Variables
    SqlConnection con;
    SqlCommand cmd;
    //private SqlDataReader sda;
    public decimal TotalUnitPriceDecimal;
    public string gl_marca = "-1", gl_local = "", gl_numsoli = "", gl_codre = "", gl_user = "", gl_fecdesde = "", gl_fechasta = "", gl_vin = "", gl_estadoSol = "";
    //ControlVfc _controlVFC;
    private SaveVFC saveVfc;
    private SaveVFCProductivo saveVFCProductivo;
    private List<String> identificadoresWS;
    private List<String> detallesWS;

    private String _usuario, _contrasena;//, _vin, _idEntity, _chasis, _anio, _marca, _modelo, _version;
    private String _usuarioVFC, _contrasenaVFC;

    private String _datos = "";


    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;
        ClientScript.GetPostBackEventReference(this, string.Empty);

        num = Request.QueryString["numero"];
        _pdf = new PdfHelper(Server.MapPath("~\\doc"));
        string usuario = "";
        usuario = Session["rut"].ToString();
        string query = "SELECT nombre FROM persona WHERE rut = '" + usuario + "'";

        DataSet DsCrit = _ControlBD.ObtenerDatosFiltrados("SELECT nombre, criticidad, multipleSucursal FROM persona WHERE rut = '" + usuario + "'");
        //string criticidad = "";
        var nombre = "";
        foreach (DataRow campo in DsCrit.Tables[0].Rows)
        {
            Session["Criticidad"] = campo["criticidad"].ToString();
            nombre = campo["nombre"].ToString();
            Session["multiSucurusal"] = campo["multipleSucursal"].ToString();
        }

        // var nombre = _ControlBD.usuario(query);
        txtusuario.Text = nombre;
        mjsError.Visible = false;
        mjsError.InnerText = "";
        PanelListado.Visible = false;
        PanelEdicion.Visible = false;


        btnExportExell.Visible = false;
        //txtdescrip.Visible = false;


        btnVolver.Click += volver;
        btnBuscarSoli.Click += BuscaSolicitud;
        //BtnConfirmar.Click += ConfirmaVFC; //Cambiar nombre a función



        if (!IsPostBack)
        {
            if (Request.QueryString["cotizacion"] != null)
            {
                txtnumsoli.Text = Request.QueryString["cotizacion"].ToString();
                BuscaCotizaciones(4, gl_marca, txtnumsoli.Text, gl_fecdesde, gl_fechasta, gl_codre, gl_vin, gl_estadoSol, "", gl_local);
            }

            if (Request.QueryString["txtdesde"] != null)
            {
                txtdesde.Text = Request.QueryString["txtdesde"].ToString();
                txtHasta.Text = Request.QueryString["txtHasta"].ToString();
                txtVin.Text = Request.QueryString["txtVin"].ToString();
                ddlEstadoSol.SelectedValue = Request.QueryString["txtEstado"].ToString();

                BuscaCotizaciones(4, gl_marca, gl_numsoli, txtdesde.Text, txtHasta.Text, gl_codre, txtVin.Text, ddlEstadoSol.SelectedValue, "", gl_local);
            }

            if (marcaEdit != 0)
            {

                EditCargaGrilla(numSolicitud);
                gvDetalleSolicitud.Visible = true;
                PanelEdicion.Visible = true;
                PanelListado.Visible = false;
                //txtnumsoli.Text = Convert.ToString(numSolicitud);
                pnlSucursales.Visible = true;
                ListaSucursales.Visible = true;
                LLenaSucursal();
            }

            marcaEdit = 0;

            if (marcaEdit == 0 & editFin == 0)
            {
                pnlSucursales.Visible = false;
                ListaSucursales.Visible = false;
            }
            //if(marcaEdit ==1 & editFin == 1)
            //{
            //    pnlSucursales.Visible = true;
            //    ListaSucursales.Visible = true;
            //    LLenaSucursal();
            //}

            if (confirmarsucursal != 0)
            {

                EditCargaGrilla(numSolicitud);
                gvDetalleSolicitud.Visible = true;
                PanelEdicion.Visible = true;
                PanelListado.Visible = false;
                pnlSucursales.Visible = true;
                ListaSucursales.Visible = true;
                LLenaSucursal();

                //txtnumsoli.Text = Convert.ToString(numSolicitud);

            }

            if (confirmaregistros != 0)
            {

                EditCargaGrilla(numSolicitud);
                gvDetalleSolicitud.Visible = true;
                PanelEdicion.Visible = true;
                PanelListado.Visible = false;
                pnlSucursales.Visible = true;
                ListaSucursales.Visible = true;
                LLenaSucursal();

                //txtnumsoli.Text = Convert.ToString(numSolicitud);

            }

            confirmarsucursal = 0;
            confirmaregistros = 0;
            //string script = "$(document).ready(function () { $('[id*=BtnConfirmar]').click(); });";
            //ClientScript.RegisterStartupScript(this.GetType(), "load", script, true);
            LlenarComboMarcas();
            LlenarComboLocal();

            if (Session["VFCNro"] != null)
            {
                string VFC = Session["VFCNro"].ToString();
                if (VFC != "")
                {
                    msjesError.Visible = true;
                    msjesError.InnerText = "Se generaron los VFC Solicitados (" + VFC + "), Favor revisar su correo con detalle";
                    Session["VFCNro"] = "";
                }
            }
            


            string nosucursal = Session["NoSucursal"].ToString();
            if (nosucursal != "")
            {
                msjesError.Visible = true;
                msjesError.InnerText = nosucursal;
                Session["NoSucursal"] = "";
            }
        }
        else
        {
            Session["VFCNro"] = "";
            Session["NoSucursal"] = "";


        }
        try
        {
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
    private void LlenarComboLocal()
    {
        combLocal.Items.Clear();
        combLocal.Items.Add(new ListItem("Seleccionar", "-1"));

        //Llenar el combo box con las marcas
        foreach (string local in _ControlBD.CrearLocales(Session["rut"].ToString()))
        {
            combLocal.Items.Add(local);
        }
    }
    protected void BuscaSolicitud(object sender, EventArgs e)
    {
        gl_marca = combomarcas.Text;
        gl_local = combLocal.Text;
        gl_numsoli = txtnumsoli.Text;
        gl_fecdesde = txtdesde.Text;
        gl_fechasta = txtHasta.Text;
        gl_codre = txtCodRep.Text;
        gl_user = txtusuario.Text;
        gl_vin = txtVin.Text;
        gl_estadoSol = ddlEstadoSol.SelectedValue;


        BuscaCotizaciones(4, gl_marca, gl_numsoli, gl_fecdesde, gl_fechasta, gl_codre, gl_vin, gl_estadoSol, "", gl_local);

        if (txtnumsoli.Text == "")
        {

        }
        else
        {
            numSolicitud = Int32.Parse(txtnumsoli.Text);
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
        //Combotipotrans.SelectedValue = "0";
        mjsError.Visible = false;
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
    public int GetUnitPrice(int Price)
    {
        TotalUnitPrice += Price;
        return Price;
    }
    public string GetUnitPriceDecimal(string PriceD)
    {
        TotalUnitPriceDecimal += decimal.Parse(PriceD.Replace('.', ','));
        return PriceD;
    }
    public string GetTotal()
    {
        return Util.convertToMoneyPeru(TotalUnitPriceDecimal); 
    }
    //public int GetTotal()
    //{
    //    return TotalUnitPrice;
    //}
    protected void HabilitarCombo(object sender, EventArgs e)
    {

        gvDetalleSolicitud.Visible = true;
        PanelEdicion.Visible = true;
        PanelListado.Visible = false;
    }
    protected void btnExportExell_Click(object sender, EventArgs e)
    {
        DataTable dt = new DataTable();
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
    public DataTable getData()
    {

        string marca;
        string local;
        string numsoli, codre, user, fecdesde, fechasta, vin, estadoSol;
        marca = combomarcas.Text;
        local = combLocal.Text;
        numsoli = txtnumsoli.Text;

        DataTable dt = new DataTable();
        dt.TableName = "Cotizaciones";
        dt.Columns.Add("numeroSolicitud", typeof(string));
        dt.Columns.Add("fecha", typeof(string));
        dt.Columns.Add("marca", typeof(string));
        dt.Columns.Add("codRepto", typeof(string));
        dt.Columns.Add("descripcion", typeof(string));
        dt.Columns.Add("cantidad", typeof(string));
        dt.Columns.Add("Concesionario", typeof(string));
        dt.Columns.Add("precio_solicitud", typeof(string));
        dt.Columns.Add("tipo", typeof(string));
        dt.Columns.Add("vin", typeof(string));
        dt.Columns.Add("envio", typeof(string));
        dt.Columns.Add("sesion", typeof(string));
        dt.Columns.Add("Dias", typeof(string));
        dt.Columns.Add("Estado", typeof(string));
        dt.Columns.Add("Confirmada", typeof(string));
        dt.Columns.Add("direccionSucursal", typeof(string));
        dt.Columns.Add("anulada", typeof(string));
        dt.Columns.Add("comentario", typeof(string));
        dt.Columns.Add("motivo", typeof(string));

        fecdesde = txtdesde.Text;
        fechasta = txtHasta.Text;
        codre = txtCodRep.Text;
        user = txtusuario.Text;
        vin = txtVin.Text;
        estadoSol = ddlEstadoSol.SelectedValue;


        string usuarioRut = Session["rut"].ToString();
        int? nro_error = null;
        string msg_error = null;

        try
        {

            RepuestosModelDataContext ctx = new RepuestosModelDataContext();
            var lista = (from i in ctx.webr_obtiene_cotizacion_consulta(4,
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
                        select i);

            if (lista != null)
            {
                foreach (var item in lista)
                {
                    DataRow dr = dt.NewRow();
                    dr["numeroSolicitud"] = item.numeroSolicitud;
                    dr["fecha"] = item.fecha;
                    dr["marca"] = item.marca;
                    dr["codRepto"] = item.codRepto;
                    dr["descripcion"] = item.descripcion;
                    dr["cantidad"] = item.cantidad;
                    dr["Concesionario"] = item.Concesionario;
                    dr["precio_solicitud"] = item.precio_solicitud;
                    dr["tipo"] = item.tipo;
                    dr["vin"] = item.vin;
                    dr["envio"] = item.envio;
                    dr["sesion"] = item.sesion;
                    dr["Dias"] = item.Dias;
                    dr["Estado"] = item.Estado;
                    dr["Confirmada"] = item.Confirmada;
                    dr["direccionSucursal"] = item.direccionSucursal;
                    dr["anulada"] = item.anulada;
                    dr["comentario"] = item.comentario;
                    dr["motivo"] = item.motivo;

                    dt.Rows.Add(dr);
                }
            }

            return dt;

        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Se presento un error al ingresar] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
            con.Close();
            return null;
        }


        
    }
    public void BuscaCotizaciones(int identificador, string marca, string numsoli, string fecdesde, string fechasta, string codre, string vin, string estadoSol, string concesionario, string local)
    {
        string usuarioRut = Session["rut"].ToString();
        int? nro_error = null;
        string msg_error = null;

        try
        {

            RepuestosModelDataContext ctx = new RepuestosModelDataContext();
            var lista = from i in ctx.webr_obtiene_cotizacion_consulta(identificador,
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
                gvListSolicitud.DataSource = lista;
                gvListSolicitud.DataBind();
                PanelListado.Visible = true;
                btnExportExell.Visible = true;
            }

        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Se presento un error al ingresar] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
            //con.Close();
        }
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
        gl_local = combLocal.Text;
        gl_numsoli = txtnumsoli.Text;
        gl_fecdesde = txtdesde.Text;
        gl_fechasta = txtHasta.Text;
        gl_codre = txtCodRep.Text;
        gl_user = txtusuario.Text;
        gl_vin = txtVin.Text;
        gl_estadoSol = ddlEstadoSol.SelectedValue;

        if (e.CommandName == "editar")
        {
            int indice = Convert.ToInt32(e.CommandArgument);
            GridViewRow selectRow = gvListSolicitud.Rows[indice];

            Label Finalizada = selectRow.Cells[13].FindControl("Finalizada") as Label;
            Label Estado = selectRow.Cells[12].FindControl("Estado") as Label;

            Label numero = selectRow.Cells[1].FindControl("numeroSolicitud") as Label;
            int idsoli = Convert.ToInt32(numero.Text);
            Label marca = selectRow.Cells[3].FindControl("marca") as Label;
            string mar = Convert.ToString(marca.Text);
            Label fec = selectRow.Cells[2].FindControl("fecha") as Label;
            string date = Convert.ToString(fec.Text);
            string creador = "SELECT usuario FROM t_SolicitudCotizacion Where numeroSolicitud = '" + Convert.ToInt32(idsoli) + "'";
            creador = _ControlBD.usuariocotizacion(creador);
            crea = creador;

            Label tip = selectRow.Cells[8].FindControl("tipo") as Label;
            string tipo = Convert.ToString(tip.Text);
            Label can = selectRow.Cells[6].FindControl("cantidad") as Label;
            string canti = Convert.ToString(can.Text);
            Label soli = selectRow.Cells[9].FindControl("envio") as Label;
            string solicitud = Convert.ToString(soli.Text);

            try
            {
                int? nro_error = null;
                string msg_error = null;
                RepuestosModelDataContext ctx = new RepuestosModelDataContext();
                var listaResultado = from i in ctx.webr_obtiene_detalle_Cotizacion(idsoli, ref nro_error, ref msg_error)
                                     select i;
                if (listaResultado != null)
                {
                    gvDetalleSolicitud.EmptyDataText = "No se Encontraron Datos";
                    gvDetalleSolicitud.DataSource = listaResultado;
                    gvDetalleSolicitud.DataBind();
                }

                if (nro_error != 0)
                {
                    msjesError.Visible = true;
                    msjesError.InnerText = "Se ha generado un error SQL Favor contactar con SKBerge";
                    return;
                }


                if (Finalizada.Text == "SI" || Estado.Text == "A")
                {
                    BtnConfirmar.Enabled = false;
                    ImageButton1.Enabled = false;
                }
                else
                {
                    BtnConfirmar.Enabled = true;
                    ImageButton1.Enabled = true;
                }

                gvDetalleSolicitud.Visible = true;
                PanelEdicion.Visible = true;
                PanelListado.Visible = false;

            }
            catch (Exception ex)
            {
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Se presento un error al ingresar] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
                con.Close();
            }
        }
    }

    protected void gvListSolicitud_RowCreated(object sender, GridViewRowEventArgs e)
    {
        int indice = 0;
        string ind = "0";

        try
        {
            if (gvListSolicitud.SelectedRow != null)
            {
                ind = gvListSolicitud.SelectedRow.ToString();
            }


            if (ind != "")
            {
                indice = Convert.ToInt32(ind);
            }


            for (int i = 0; i < gvListSolicitud.Rows.Count; i++)
            {
                LinkButton btnEditar = (LinkButton)gvListSolicitud.Rows[i].FindControl("btnEditar");
                Label lblRechazo = (Label)gvListSolicitud.Rows[i].FindControl("lblRechazo");
                string anulada = btnEditar.CommandArgument.ToString();
                if (anulada != "")
                {
                    if (Convert.ToBoolean(anulada))
                    {
                        btnEditar.Visible = false;
                        lblRechazo.Visible = true;
                    }
                    else
                    {
                        btnEditar.Visible = true;
                        lblRechazo.Visible = false;
                    }
                }

            }
        }
        catch (Exception)
        {
            Response.Redirect("SolConsultaCotizacion.aspx", false);
            return;
        }



    }

    protected void volver(object sender, EventArgs e)
    {

        gvDetalleSolicitud.Visible = false;
        ListaSucursales.DataSource = "";
        ListaSucursales.DataBind();
        PanelEdicion.Visible = false;
        PanelListado.Visible = true;
        btnBuscarSoli.Visible = true;
        btnVolver.Visible = false;

    }

    protected void btnEditar_Click(object sender, EventArgs e)
    {

        string query, query2, query3, query4;
        string correo;
        string dat;
        string usuario;
        string rut = Session["rut"].ToString();
        string sesion = Session["idSession"].ToString();

        //Captura datos para mantener Busqueda
        gl_marca = combomarcas.Text;
        gl_local = combLocal.Text;
        gl_numsoli = txtnumsoli.Text;
        gl_fecdesde = txtdesde.Text;
        gl_fechasta = txtHasta.Text;
        gl_codre = txtCodRep.Text;
        gl_user = txtusuario.Text;
        gl_vin = txtVin.Text;
        gl_estadoSol = ddlEstadoSol.SelectedValue;

        LinkButton imageButton = (LinkButton)sender;
        TableCell tableCell = (TableCell)imageButton.Parent;
        GridViewRow row = (GridViewRow)tableCell.Parent;
        gvListSolicitud.SelectedIndex = row.RowIndex;
        int indice = row.RowIndex;



        //int indice = Convert.ToInt32(gvListSolicitud.SelectedRow);
        GridViewRow selectRow = gvListSolicitud.Rows[indice];

        Label Finalizada = selectRow.Cells[13].FindControl("Finalizada") as Label;
        Label Estado = selectRow.Cells[12].FindControl("Estado") as Label;

        Label numero = selectRow.Cells[1].FindControl("numeroSolicitud") as Label;
        int idsoli = Convert.ToInt32(numero.Text);
        Label marca = selectRow.Cells[3].FindControl("marca") as Label;
        string mar = Convert.ToString(marca.Text);
        Label fec = selectRow.Cells[2].FindControl("fecha") as Label;
        string date = Convert.ToString(fec.Text);
        string creador = "SELECT usuario FROM t_SolicitudCotizacion Where numeroSolicitud = '" + Convert.ToInt32(idsoli) + "'";
        creador = _ControlBD.usuariocotizacion(creador);
        crea = creador;

        Label tip = selectRow.Cells[8].FindControl("tipo") as Label;
        string tipo = Convert.ToString(tip.Text);
        Label can = selectRow.Cells[6].FindControl("cantidad") as Label;
        string canti = Convert.ToString(can.Text);
        Label soli = selectRow.Cells[9].FindControl("envio") as Label;
        string solicitud = Convert.ToString(soli.Text);

        string criticidadVfc = Session["Criticidad"].ToString();

        bool criticidadVfc1 = Convert.ToBoolean(criticidadVfc);

        if (criticidadVfc1 == true)
        {

            gvDetalleSolicitud.Columns[13].Visible = true;
            gvDetalleSolicitud.Columns[14].Visible = true;
        }
        else
        {
            gvDetalleSolicitud.Columns[13].Visible = false;
            gvDetalleSolicitud.Columns[14].Visible = false;
        }

        try
        {
            int? nro_error = null;
            string msg_error = null;
            RepuestosModelDataContext ctx = new RepuestosModelDataContext();
            var listaResultado = from i in ctx.webr_obtiene_detalle_Cotizacion(idsoli,ref nro_error, ref msg_error)
                                 select i;
            if (listaResultado != null){

                gvDetalleSolicitud.DataSource = listaResultado;
                gvDetalleSolicitud.DataBind();
            }

            if (nro_error != 0)
            {
                msjesError.Visible = true;
                msjesError.InnerText = "Se ha generado un error SQL Favor contactar con SKBerge";
                return;
            }

            if (Finalizada.Text == "SI" || Estado.Text == "A")
            {
                BtnConfirmar.Enabled = false;
                ImageButton1.Enabled = false;
            }
            else
            {
                BtnConfirmar.Enabled = true;
                ImageButton1.Enabled = true;
            }

            gvDetalleSolicitud.Visible = true;
            PanelEdicion.Visible = true;
            PanelListado.Visible = false;
            btnBuscarSoli.Visible = false;
            btnVolver.Visible = true;


            if (_sapApi.DeterminarMultiSucursal(Session["rut"].ToString()))
            {
                // Se llena la lista de sucursales
                LLenaSucursal();
                pnlSucursales.Visible = true;
                ListaSucursales.Visible = true;
                flagRut = 1;
            }
            else
            {
                flagRut = 2;
            }
        }
        catch (Exception ex)
        {
            //_mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Se presento un error al ingresar] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
            con.Close();
        }


    }

    private void LLenaSucursal()
    {
        // Se llena la lista de sucursales
        DataSet ds;

        ds = _controlBD.ShipCodeSucursal(_sapApi.GetNombreDealer(Session["rut"].ToString()));


        ListaSucursales.DataSource = ds;
        ListaSucursales.DataTextField = "direccionSucursal";
        ListaSucursales.DataValueField = "shipCode";
        ListaSucursales.DataBind();

    }

    private String crearXml(String cantidad, 
                            String codigo, 
                            String detalle, 
                            String marca,
                            String creador, 
                            String descripcionUsuario, 
                            String dealer,
                            String direccion, 
                            String TipoPedido,
                            String numsoli, 
                            String MailSolicitante,
                            String CC,
                            String codsap, 
                            String codsapclt, 
                            String precio, 
                            String obCritVFC, 
                            String codigomotivo,
                            String bloqueomotivo)
    {

        XDocument miXML = new XDocument(
        new XDeclaration("1.0", "utf-8", "yes"),
        new XComment("Lista de Registros"),
        new XElement("registros",
                                new XElement("registro",
                                new XElement("cantidad", cantidad),
                                new XElement("codigo", codigo),
                                new XElement("detalle", detalle),
                                new XElement("marca", marca),
                                new XElement("vin", ""),
                                new XElement("creador", creador),
                                new XElement("mailcreador", MailSolicitante),
                                new XElement("descripcionUsuario", descripcionUsuario),
                                new XElement("dealer", new XCData(dealer)),
                                new XElement("direccion",direccion),
                                new XElement("tipoPedido", TipoPedido),
                                new XElement("file", ""),
                                new XElement("opcionvfc", ""),
                                new XElement("km", 0),
                                new XElement("nsiniestro", 0),
                                new XElement("CC", CC),
                                new XElement("codsap", codsap),
                                new XElement("nrocotiza", numsoli),
                                new XElement("viaimportacion", ""),
                                new XElement("codsapclte", codsapclt),
                                new XElement("preciorepuesto", precio),
                                new XElement("critico", 0),
                                new XElement("obsvfc", obCritVFC),
                                new XElement("codigobloqueo", codigomotivo),
                                new XElement("bloqueomotivo", bloqueomotivo)

                                //new XElement("nrocotiza",)
                            )
                    )
               );
        
        return miXML.ToString();

    }//fin metodoescribeXml

    private String getHacerVin(String xmlDatosAEnviar, String userVFC, String us3rVF)
    {
        string ambiente = ConfigurationManager.AppSettings["ambiente"].ToString();
        try
        {
            switch (ambiente)
            {
                case "q":
                    saveVfc = new SaveVFC();                    
                   return saveVfc.setVFC(xmlDatosAEnviar, _usuarioVFC, _contrasenaVFC);//revisar claves 
                case "p":
                    saveVFCProductivo = new SaveVFCProductivo();
                    return saveVFCProductivo.setVFC(xmlDatosAEnviar, _usuarioVFC, _contrasenaVFC);//revisar claves

                default:
                    saveVfc = new SaveVFC();
                    return saveVfc.setVFC(xmlDatosAEnviar, _usuarioVFC, _contrasenaVFC);
            }
            
        }
        catch (Exception ex)
        {
            return "";
        }
    }

    public void EditCargaGrilla(int idSolicitud)
    {
        try
        {
            int? nro_error = null;
            string msg_error = null;
            RepuestosModelDataContext ctx = new RepuestosModelDataContext();
            var listaResultado = from i in ctx.webr_obtiene_detalle_Cotizacion(idSolicitud, ref nro_error, ref msg_error)
                                 select i;
            if (listaResultado != null)
            {
                gvDetalleSolicitud.DataSource = listaResultado;
                gvDetalleSolicitud.DataBind();
            }


            if (nro_error != 0)
            {
                msjesError.Visible = true;
                msjesError.InnerText = "Se ha generado un error SQL Favor contactar con SKBerge";
                return;
            }
        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "Se presento un error al editar Grilla. Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
            con.Close();
        }
    }

    protected void ImageButton1_Click1(object sender, ImageClickEventArgs e)
    {
        string query = "";
        string estado = "";


        int id = 0;
        for (int i = 0; i < gvDetalleSolicitud.Rows.Count; i++)
        {

            GridViewRow selectRow = gvDetalleSolicitud.Rows[i];

            Label num = selectRow.Cells[1].FindControl("numsoli") as Label;
            id = Convert.ToInt32(num.Text);
            TextBox canti = selectRow.Cells[4].FindControl("txtcantgrid") as TextBox;
            int qty = Convert.ToInt32(canti.Text);
            Label cod = selectRow.Cells[2].FindControl("codrepto") as Label;
            string codigo = Convert.ToString(cod.Text);

            query = "UPDATE t_SolicitudCotizacion SET cantidad = " + qty + ", precio_Solicitud = " + qty + " * PrecioUnitario  " + " WHERE numeroSolicitud = '" + id + "' AND codRepto = '" + codigo + "'";
            _ControlBD.EjecutaQuery(query);

        }

        if (_sapApi.DeterminarMultiSucursal(Session["rut"].ToString()))
        {
            editFin = 1;
            flagRut = 1;
        }
        else
        {
            flagRut = 2;
            editFin = 0;
        }
        marcaEdit = 1;
        numSolicitud = id;
        EditCargaGrilla(numSolicitud);
        gvDetalleSolicitud.Visible = true;
        PanelEdicion.Visible = true;
        PanelListado.Visible = false;
        //LLenaSucursal();

    }

    public void gvDetalleSolicitud_RowDataBound(Object sender, GridViewRowEventArgs e)
    {

        string criticidadVfc = Session["Criticidad"].ToString();

        bool criticidadVfc1 = Convert.ToBoolean(criticidadVfc);

        for (int i = 0; i < gvDetalleSolicitud.Rows.Count; i++)
        {

            GridViewRow selectRow = gvDetalleSolicitud.Rows[i];

            CheckBox CriticidadRow = selectRow.Cells[13].FindControl("chkCriticidad") as CheckBox;
            TextBox ObsCriticidad = selectRow.Cells[14].FindControl("txtObsCriticidad") as TextBox;


            if (criticidadVfc1 == true)
            {

                gvDetalleSolicitud.Columns[13].Visible = true;
                gvDetalleSolicitud.Columns[14].Visible = true;
            }
            else
            {
                gvDetalleSolicitud.Columns[13].Visible = false;
                gvDetalleSolicitud.Columns[14].Visible = false;
            }

        }

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var row = e.Row;
            //DataRowView dr = (DataRowView)e.Row.DataItem;
            webr_obtiene_detalle_CotizacionResult res = (webr_obtiene_detalle_CotizacionResult)e.Row.DataItem;

            if (res != null)
            {
                Label subtotal = (Label)row.FindControl("subtotal");
                Label txtvalorgrid = (Label)row.FindControl("txtvalorgrid");
                if (subtotal != null)
                {
                    string text = res.precio_Solicitud;
                    if (text == null || text == "")
                    {
                        text = "0";
                    }
                    else
                    {
                        text = text.Replace('.', ',');
                    }
                    GetUnitPriceDecimal(text);
                    subtotal.Text = Util.convertToMoneyPeru(decimal.Parse(text));
                }
                if (txtvalorgrid != null)
                {
                    string text = res.PrecioUnitario.ToString();
                    if (text == null || text == "")
                    {
                        text = "0";
                    }
                    txtvalorgrid.Text = Util.convertToMoneyPeru(decimal.Parse(text));
                }   

            }
           

        }

    }

    protected void BtnConfirmar_Click(object sender, EventArgs e)
    {
        
        BtnConfirmar.Enabled = false;

        if (_sapApi.DeterminarMultiSucursal(Session["rut"].ToString()) && (ListaSucursales.SelectedItem.Value == ""))
        {

            Session["NoSucursal"] = "Debe seleccionar una sucursal de envío";

            int id = 0;
            for (int i = 0; i < gvDetalleSolicitud.Rows.Count; i++)
            {
                GridViewRow selectRow = gvDetalleSolicitud.Rows[i];
                Label num = selectRow.Cells[1].FindControl("numsoli") as Label;
                id = Convert.ToInt32(num.Text);
            }
            BtnConfirmar.Enabled = true;
            confirmarsucursal = 1;
            numSolicitud = id;
            EditCargaGrilla(numSolicitud);
            gvDetalleSolicitud.Visible = true;
            PanelEdicion.Visible = true;
            PanelListado.Visible = false;
        }
        else
        {
            string query = "";
            string estado = "";
            string numResp = "";
            string numResp2 = "";
            string todoNumResp = "";
            string detalleVFCCRM = "";
            string detalleSolicitudMail = "";
            String textoCorreo = "";
            String Garantia = "";
            int tipovfc = 0;
            int criticidad = 0;
            string marcaEmail = "";
            string corAnal, corMar;
            string query5, query6, query7;

            _ControlBD.EjecutaQuery("DELETE cotizacion_final WHERE id_session = '" + Session["idSession"].ToString() + "'");

            int id = 0;
            int z = 1;
            int contadorConfirmadas = 0;
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
                Label valor = selectRow.Cells[5].FindControl("txtvalorgrid") as Label;
                Label vin = selectRow.Cells[5].FindControl("vin") as Label;
                Label envio = selectRow.Cells[5].FindControl("envio") as Label;
                Label desc = selectRow.Cells[5].FindControl("desc") as Label;
                string decodificado = WebUtility.HtmlDecode(desc.Text);// se agrega para remplazar Ñ y tildes en textos UTF8
                decodificado = WebUtility.HtmlDecode(decodificado);// se agrega para remplazar Ñ y tildes en textos UTF8
                Label marcadet = selectRow.Cells[5].FindControl("marcadet") as Label;
                Label dias = selectRow.Cells[5].FindControl("dias") as Label;

                //validacion de vigencia de la solicitud

                //obtengo el tipo de solicitud (normal o garantia). Lo voy a ocupar para la validar los días de vigencia de una solicitud
                DateTime _fechaSolicitud;
                DateTime _fechaSistema = DateTime.Now;
                TimeSpan _diferencia;
                string _fecha = "";
                string _tipoSolicitud = "";
                string _sql = "";
                int _vigencia = 0;
                int _dias = 0;

                Label tipo = selectRow.Cells[9].FindControl("tipo") as Label;

                //_sql = (" SELECT MAX(Convert(varchar,fecha,105)) AS fecha FROM t_SolicitudCotizacion WHERE numeroSolicitud = '" + num.Text  + "' ");
                _sql = (" SELECT MAX(Convert(varchar,fecha,105)) AS fecha FROM t_SolicitudCotizacion WHERE numeroSolicitud = '" + num.Text  + "' ");
                DataSet ds2 = _ControlBD.ObtenerDatosFiltrados(_sql);

                foreach (DataRow dr2 in ds2.Tables[0].Rows){
                    string Fecha = dr2["fecha"].ToString();
                    _fechaSolicitud = DateTime.ParseExact(Fecha, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    //string FechaSolicitudFormato =   _fechaSolicitud.ToString("dd/MM/yyyy");
                    //DateTime FechaDateTime = Convert.ToDateTime(FechaSolicitudFormato, CultureInfo.InvariantCulture);
                    _diferencia = _fechaSistema - _fechaSolicitud;

                    _dias = _diferencia.Days;

                }


                DataSet ds1 = _ControlBD.ObtenerDatosFiltrados(" SELECT nombre, dias FROM validezCotizacion ");

                foreach (DataRow dr1 in ds1.Tables[0].Rows){
                    _tipoSolicitud = dr1["nombre"].ToString();

                    if (tipo.Text.ToLower() == _tipoSolicitud.ToLower()){
                        _vigencia = Convert.ToInt32(dr1["dias"]);
                    }
                }

                if (_dias > _vigencia){
                    msjesError.Visible = true;
                    msjesError.InnerText = ("Desde la fecha de creacion de la solicitud, han pasado "+ _dias + " días. Ya no es posible confirmarla.");
                    return;
                }
                
                //fin validacion de vigencia de la solicitud

                CheckBox chkcriticidad = selectRow.Cells[13].FindControl("chkCriticidad") as CheckBox;
                TextBox txtObsCriticidad = selectRow.Cells[14].FindControl("txtObsCriticidad") as TextBox;
                Label motivo = selectRow.Cells[15].FindControl("Motivo") as Label;

                //string VFC_Criticos = "";
                string obsCriticidad = "";
                string msjCriticoMail = "";

                criticidad = 0;
                if (chkcriticidad.Checked == true)
                {
                    marcaEmail = marcadet.Text;
                    criticidad = 1;
                    obsCriticidad = txtObsCriticidad.Text;
                    msjCriticoMail = "Critico";

                }



                //*** Mejora Urgente 27/09 ALan Cañete Obtencion Dealer para creacion VFC ***//
                string dealer = Util.limpiaPalabras(Util.obtieneNombreConcesionarioPorRUTCualquierUsuario(Session["rut"].ToString()));
                string Delerdecodificado = WebUtility.HtmlDecode(dealer);// se agrega para remplazar Ñ y tildes en textos UTF8
                Delerdecodificado = WebUtility.HtmlDecode(Delerdecodificado);// se agrega para remplazar Ñ y tildes en textos UTF8

                string marca = marcadet.Text;
                marca = _sapApi.GetCmpCod(marca);
                marcaEmail = marca;

                CheckBox chkconfirmada = selectRow.Cells[8].FindControl("chkConfirmar") as CheckBox;
                CheckBox chkGarantia = selectRow.Cells[8].FindControl("chkGarantia") as CheckBox;
                int confirmada = 0;

                //string preci = Convert.ToString(valor.Text);
                //decimal total = 0;
                //preci = preci.Replace(",", "").Replace("$", "").Replace(" ", "");
                //preci.Replace(".", ",");
                //total = qty * Convert.ToDecimal(preci);
                //anulado

                string preci = Convert.ToString(valor.Text);
                decimal total = 0;
                preci = preci.Replace("$", "");
                preci = preci.Replace(",", "");
                preci = preci.Replace(".", ",");
                total = qty * Convert.ToDecimal(preci);

                if (chkconfirmada.Checked)
                {
                    confirmada = 1;
                    query = "UPDATE t_Cotiza SET confirmada = " + confirmada + " WHERE id_cotiza = " + id + "";
                    _ControlBD.EjecutaQuery(query);

                    //ACA VA VFC
                    if (chkGarantia.Checked)
                    {
                        Garantia = "garantia";
                        tipovfc = 1;
                        query7 = "";
                        query7 = "UPDATE t_SolicitudCotizacion SET Tipo = 'Garantia' WHERE numeroSolicitud= '" + id + "' AND codRepto ='" + codigo + "'";
                        _ControlBD.EjecutaQuery(query7);

                    }
                    else
                    {
                        Garantia = "normal";
                        tipovfc = 0;
                        query7 = "";
                        query7 = "UPDATE t_SolicitudCotizacion SET Tipo = 'Normal' WHERE numeroSolicitud= '" + id + "' AND codRepto ='" + codigo + "'";
                        _ControlBD.EjecutaQuery(query7);

                    }

                    try
                    {

                        //Rescatar cod sap cliente
                        //_sapApi.GetCodigoShipCode(Session["rut"].ToString())

                        string codSAPclt = _ControlBD.CodigoSAPConce(_sapApi.GetCodigoShipCode(Session["rut"].ToString()));

                        //Se llama al método que inserta el vfc
                        //hacerVfc(canti.Text, codigo, "PRUEBA", "", vin.Text, "WEB", Session["rut"].ToString(), _sapApi.GetNombreDealer(Session["rut"].ToString()), _sapApi.GetDireccionSucursal(Session["rut"].ToString()), "NORMAL", "normal", "0", "0", num.Text, "PRUEBA", _sapApi.GetCorreosVFCUsuario(Session["rut"].ToString()), _sapApi.GetCodigoShipCode(Session["rut"].ToString()), "", valor.Text);


                        identificadoresWS = new List<String>();
                        detallesWS = new List<String>();

                        identificadoresWS.Clear();
                        detallesWS.Clear();

                        _usuario = "H2WSINTELLICORE";
                        _contrasena = "h2ws";
                        _usuarioVFC = "userVFC";
                        _contrasenaVFC = "us3rVF";

                        //LLAMO A MI METODO CREADO ANTERIORMENTE EL CUAL LLAMA AL METODO DEL WS, Y TOMO EL RETORNO(XML) EN UN STRING
                        //'datosXml' PARA POSTERIORMENTE PARSEARLO Y TIRARLO A UNA LISTA
                        /*le entrego como primer parametro el xml que pide el ws el cual es creado en el metodo 'crearXml' con los datos recibidos y devuelto en un String no en un xml
                         ya que asi lo pide el ws*/
                        //Query obtiene detalle
                        DataSet dsDet1 = _ControlBD.ObtenerDatosFiltrados("select top 1 rut_cotiza from t_Cotiza where id_cotiza = " + id + " ");
                        string rutCotizador = "";
                        foreach (DataRow campo1 in dsDet1.Tables[0].Rows)
                        {
                            rutCotizador = campo1["rut_cotiza"].ToString();
                        }


                        if (_sapApi.DeterminarMultiSucursal(Session["rut"].ToString()))
                        {

                            flagIdDealer = ListaSucursales.SelectedItem.Text;
                            shipCodeDealer = ListaSucursales.SelectedItem.Value;
                            Session["direccion"] = flagIdDealer.ToString();
                        }
                        else
                        {

                            flagIdDealer = Util.limpiaPalabras(_sapApi.GetDireccionSucursal(Session["rut"].ToString()));
                            shipCodeDealer = _sapApi.GetCodigoShipCode(rutCotizador.ToString());
                            Session["direccion"] = flagIdDealer.ToString();
                        }

                        InsertarVFC CrearVFC = new InsertarVFC();
                        ModeloRespuestaInsertaVFC RespuestaIncertaVFC = new ModeloRespuestaInsertaVFC();

                        string codeMotivo = "";
                        string bloqueoMotivo = "";
                        ObtieneMotivoDetalle(marcadet.Text, motivo.Text, out codeMotivo, out bloqueoMotivo);

                        string valorSoli = valor.Text;
                        valorSoli = valorSoli.Replace("$", "");
                        valorSoli = valorSoli.Replace(",", "");
                        String datosXml = "";
                        String xmlCreado = crearXml(canti.Text,
                                                    codigo,
                                                    Util.limpiaPalabras(decodificado),
                                                    marca,
                                                    Util.limpiaPalabras(Session["rut"].ToString()),
                                                    Util.limpiaPalabras(_sapApi.GetNombreUsuario(Session["rut"].ToString())),
                                                    Delerdecodificado,
                                                    flagIdDealer,//direccion
                                                    tipo.Text, //Tipo Pedido
                                                    num.Text,
                                                    _sapApi.GetCorreosVFCUsuario(Session["rut"].ToString()),// Email Solicitante
                                                    _sapApi.GetCorreosVFCConcesionario(Session["rut"].ToString()),// Email CC
                                                    shipCodeDealer, //codsap
                                                    codSAPclt,//consapclt
                                                    valorSoli, //precio
                                                    obsCriticidad, //obs critico vfc
                                                    codeMotivo,//codigomotivo
                                                    bloqueoMotivo); //bloqueomotivo

                         //datosXml = getHacerVin(xmlCreado, "userVFC", "us3rVF").ToString();

                        RespuestaIncertaVFC = CrearVFC.InsertarVFCHuanos(xmlCreado);


                        //Prueba Peru : se comenta el xml de prueba
                        // datosXml = "<?xml version='1.0' encoding='utf-8' ?><sucesos><suceso><identificador>IdEntity: 16413811</identificador> <detalle>Registro VFC Insertado</detalle> </suceso> <suceso><identificador>IdEntity: 16413812</identificador> <detalle>Registro Seguimiento Insertado</detalle> </suceso> <suceso> <identificador>Fin</identificador><detalle>Fin Proceso Integraci?n</detalle> </suceso> </sucesos>";


                        XmlDocument xDoc = new XmlDocument();

                        datosXml = "<?xml version='1.0' encoding='utf-8' ?><sucesos><suceso><identificador>IdEntity:" + RespuestaIncertaVFC.idEntityH2+"</identificador> <detalle>Registro VFC Insertado</detalle> </suceso> <suceso><identificador>IdEntity: 17419440</identificador> <detalle>Registro Seguimiento Insertado</detalle> </suceso> <suceso> <identificador>Fin</identificador><detalle>Fin Proceso Integraci?n</detalle> </suceso> </sucesos>";


                        if (RespuestaIncertaVFC.Message == "An error has occurred.")
                        {
                            //bloque de alerta sin respuesta de servicio Nov 2021
                            msjesError.Visible = true;
                            msjesError.InnerText = "Advertencia: VFC no se cargo en CRM, vuelva a intentar, si problemas persisten dar aviso del error. ";
                            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [BtnConfirmar_Click] Message: Error al insertar VFC en el CRM Error:" + detalleVFCCRM);
                            //bloque de alerta sin respuesta de servicio Nov 2021
                            return;
                        }
                        //leo el xml en formato String con loadXml
                        xDoc.LoadXml(datosXml);



                        XmlNodeList resultados = xDoc.GetElementsByTagName("sucesos");

                        XmlNodeList lista = ((XmlElement)resultados[0]).GetElementsByTagName("suceso");


                        foreach (XmlElement nodo in lista)
                        {
                            identificadoresWS.Add(nodo.ChildNodes[0].InnerText);
                            detallesWS.Add(nodo.ChildNodes[1].InnerText);
                        }

                        for (int j = 0; j < detallesWS.Count; j++)
                        {
                            if (identificadoresWS[j].Length > 9)
                            {
                                if (identificadoresWS[j].Substring(0, 8) == "IdEntity")
                                {
                                    numResp = identificadoresWS[j].Substring(9, identificadoresWS[j].Length - 9);
                                    numResp = numResp.Trim();
                                    todoNumResp += numResp + ", ";
                                    numResp2 = identificadoresWS[j].Substring(0, 8);
                                    detalleVFCCRM = detallesWS[j];
                                    break;
                                }
                            }
                        }


                        if (numResp == "")
                        {
                            detalleVFCCRM = detallesWS[0];
                            msjesError.Visible = true;
                            msjesError.InnerText = "Advertencia: VFC no se cargo en CRM, vuelva a intentar, si problemas persisten dar aviso del error. ";
                            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [BtnConfirmar_Click] Message: Error al insertar VFC en el CRM Error:" + detalleVFCCRM);

                            return;
                        }
                        else
                        {
                            _ControlBD.EjecutaQuery("INSERT cotizacion_final(id_cotiza, cod_repuesto, id_session, num_vfc)VALUES(" + id + ",'" + codigo + "','" + Session["idSession"].ToString() + "','" + numResp.Trim() + "')");

                            _ControlBD.EjecutaQuery(@"insert into VFC(num_VFC,id_pedido,rut_user,fecha_creacion,
                                           codigo_rep,marca,cantidad,detalle_rep,cod_vin, Id_cotiza,tipo_Vfc,criticidad,obs_criticidad)
                                            values('" + numResp.Trim() + "','000000','" + Session["rut"].ToString() + "' , GETDATE(), " +
                                                " '" + codigo + "' , '" + marca + "' , " + canti.Text + " , '" + decodificado + "','" + vin.Text + "'," + id + "," + tipovfc + "," + criticidad + ",'" + obsCriticidad + "' )");

                            //detalleSolicitudMail += z + ". " + " | VFC | " + numResp + " | " + marca + " | " + codigo + " | " + canti.Text + " | " + desc.Text + " | " + vin.Text + " | " + msjCriticoMail + "| <br/>";

                            if (criticidad == 1)
                            {
                                detalleSolicitudMail += z + ". " + " | VFC | " + numResp + " | " + marca + " | " + codigo + " | " + canti.Text + " | " + decodificado + " | " + vin.Text + " | " + msjCriticoMail + "| <br/>";
                                z++;
                            }
                        }


                        contadorConfirmadas++;
                    }
                    catch (Exception ex)
                    {
                        msjesError.Visible = true;
                        msjesError.InnerText = "Error: indice VFC :  " + numResp;
                        _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Error VFC Finalizar Solicitud] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);

                        return;
                    }

                }


            }

            if (contadorConfirmadas == 0) //se valida confirmacion de cotizacion para enviar o no correo
            {
                Session["NoSucursal"] = "Debe tener al menos 1 registro confirmado para procesar la solicitud.";

                int idconf = 0;
                for (int i = 0; i < gvDetalleSolicitud.Rows.Count; i++)
                {
                    GridViewRow selectRow = gvDetalleSolicitud.Rows[i];
                    Label num = selectRow.Cells[1].FindControl("numsoli") as Label;
                    idconf = Convert.ToInt32(num.Text);
                }

                confirmaregistros = 1;
                numSolicitud = idconf;
                EditCargaGrilla(numSolicitud);
                gvDetalleSolicitud.Visible = true;
                PanelEdicion.Visible = true;
                PanelListado.Visible = false; 


                return;
            }
            
            string para = _sapApi.GetCorreoUsuario(Session["rut"].ToString());


            // Glosa antigua
            //string asunto = "SKBERGE: Solicitud de reserva confirmado [NO RESPONDER]";
            string asunto = "SKBERGE: VFC Creado [NO RESPONDER]--> PRUEBAS SISTEMAS";
            // Se envía un correo con los repuestos sin stock solicitados

            if (detalleSolicitudMail != "")
            {
                DataSet dsDet2 = _ControlBD.ObtenerDatosFiltrados("select top 1 rut_cotiza from t_Cotiza where id_cotiza = " + id + " ");
                string rutCotizador = "";
                foreach (DataRow campo2 in dsDet2.Tables[0].Rows)
                {
                    rutCotizador = campo2["rut_cotiza"].ToString();
                }

            }

            msjesError.Visible = true;
            msjesError.InnerText = "Se generaron los VFC Solicitados (" + todoNumResp + "), Favor revisar su correo con detalle";
            //System.Windows.Forms.MessageBox.Show("Se generaron los VFC Solicitados ("+todoNumResp+"), Favor revisar su correo con detalle");
            System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AlertBox", "alert('" + "Se generaron los VFC Solicitados (" + todoNumResp + "), Favor revisar su correo con detalle" + "');", true);
            ScriptManager.RegisterStartupScript(this, GetType(), "unique_key", "element.onclick = function(){ return confirm('Se generaron los VFC Solicitados (" + todoNumResp + "), Favor revisar su correo con detalle'); };", true);
            lblmensaje.Text = "Se generaron los VFC Solicitados (" + todoNumResp + "), Favor revisar su correo con detalle";

            Session["VFCNro"] = todoNumResp;

            _datos = todoNumResp;
            BtnConfirmar.Enabled = false;
            //Query obtiene detalle
            DataSet dsDet = _ControlBD.ObtenerDatosFiltrados("select top 1 detalle from t_Cotiza where id_cotiza = " + id + " ");
            string detalleNombre = "";
            foreach (DataRow campo in dsDet.Tables[0].Rows)
            {
                detalleNombre = campo["detalle"].ToString();
            }
            //Correo cotizacion
            string query4, query2, query3;
            string text, text2;
            string correo, correo2;
            string filePath = "";
            string[] separador;
            string[] separador2;

            query4 = "SELECT email FROM persona WHERE rut = '" + Session["rut"].ToString() + "'";
            correo = _ControlBD.Email(query4);
            text2 = "Cotización Existosa " + detalleNombre.ToString();
            query3 = "select per.email from persona as per ";
            query3 = query3 + "inner join t_Cotiza as tc ";
            query3 = query3 + "ON per.rut = tc.rut_cotiza ";
            query3 = query3 + "Where tc.id_Cotiza='" + numero + "'";

            correo2 = _ControlBD.Email(query3);

            Boolean dato;
            string querym = "SELECT MultipleSucursal FROM Persona where rut = '" + Session["rut"].ToString() + "'";

            dato = Convert.ToBoolean(_ControlBD.MultiSucursal(querym));

            if (dato == true)
            {
                filePath = _pdf.CrearSolicitudPdfMultiSucursal(Session["idSession"].ToString(), Convert.ToString(id), Session["rut"].ToString(), flagIdDealer);
            }
            else
            {
                filePath = _pdf.CrearSolicitudPdfComunFinal2(Session["idSession"].ToString(), Convert.ToString(id), Session["rut"].ToString());
            }

            if (correo != "" || correo != null)
            {
                _mail.EnviarCorreoAdjunto(correo, "Cotizacion Exitosa Confirmada " + detalleNombre.ToString(), text2, filePath);
            }
            //envia correo analistas Criticidad
            if (detalleSolicitudMail != "")
            {
                try
                {
                    textoCorreo = "Estimados, Se han realizado los siguientes VFC críticos: ";
                    textoCorreo += "<br/><br/>Detalle :";
                    textoCorreo += "<br/>" + detalleSolicitudMail;
                    //_mail.EnviarCorreo2VFc(para, asunto, textoCorreo, _sapApi.GetCorreosVFCUsuario(Session["rut"].ToString()));

                    query5 = "SELECT STUFF((SELECT CAST(';' AS VARCHAR(MAX)) + emaiilPersona_vfc FROM dbo.Personas_Email_VFC where orgpersona_vfc = 'X' ORDER BY idPersona_vfc FOR XML PATH('')), 1, 1, '') AS Correos";
                    corAnal = _ControlBD.EmailCriticidad(query5);

                    query6 = "SELECT STUFF((SELECT CAST(',' AS VARCHAR(MAX)) + b.emaiilPersona_vfc FROM marca A INNER JOIN Personas_Email_VFC B ON a.orgventas = b.orgPersona_vfc WHERE substring(a.abreviado,1,2) = '" + marcaEmail + "' FOR XML PATH('')), 1, 1, '') AS Correos";
                    corMar = _ControlBD.EmailCriticidad(query6);


                    //_mail.EnviarCorreo2VFc(corAnal, "VFC Críticos Generados: " + detalleNombre.ToString(), textoCorreo, corMar);


                }
                catch (Exception ex)
                {
                    _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [BtnConfirmar_Click] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
                    //logger.Error("Error al intentar enviar email de criticidad. Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
                }
            }
            //Fin Correo cotizacion
            //Se agrega redirect para llegar a la página de inicio una vez terminado en VFC
            Response.Redirect("SolConsultaCotizacion.aspx", true);

            GetUnitPrice(TotalUnitPrice);

        }
    }

    protected void ObtieneMotivoDetalle(string marca, string motivo, out string code , out string bloqueo)
    {
        string _code="", _bloqueo="";
        try
        {
            int? nro_error = null;
            string msg_error = null;

            RepuestosModelDataContext ctx = new RepuestosModelDataContext();

            var resultado = (from i in ctx.webr_obtiene_motivo_pedido_sol(motivo, marca, ref nro_error, ref msg_error)
                             select i).SingleOrDefault();
            if (resultado != null){
                _bloqueo = resultado.BLOQUEO;
                _code = resultado.CODIGO;
            }

        }
        catch (Exception ex)
        {
        }
        code = _code;
        bloqueo = _bloqueo;
    }
}
