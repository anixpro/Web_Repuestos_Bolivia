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
//using pe.com.skberge.crm;
//using SaveVFC = pe.com.skberge.crm.SaveVFC; //cl.humano2.skbergecl.SaveVFC;
//using SaveVFC2 = pe.com.skberge.crm.SaveVFC;
using System.Xml;
using System.Xml.Linq;

public partial class Vistas_ReporteCotizacion : System.Web.UI.Page
{

    ControlBD _controlBD;
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
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_ReporteCotizacion));
    string num = "";
    public string queryexcel;
    public static string linkExcelExport = string.Empty;
    static int numSolicitud;
    static int marcaEdit = 0;

    //Nuevas Variables
    SqlConnection con, con1;
    SqlCommand cmd, cmd1;
    public string gl_marca = "-1", gl_local = "", gl_numsoli = "", gl_codre = "", gl_user = "", gl_fecdesde = "", gl_fechasta = "", gl_vin = "", gl_estadoSol = "";
    ControlVfc _controlVFC;
    //private SaveVFC saveVfc;
    //private SaveVFC2 saveVFC2;
    private List<String> identificadoresWS;
    private List<String> detallesWS;

    private String _usuario, _contrasena, _vin, _idEntity, _chasis, _anio, _marca, _modelo, _version;
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

        DataSet DsCrit = _ControlBD.ObtenerDatosFiltrados("SELECT nombre, criticidad FROM persona WHERE rut = '" + usuario + "'");
        //string criticidad = "";
        var nombre = "";
        foreach (DataRow campo in DsCrit.Tables[0].Rows)
        {
            Session["Criticidad"] = campo["criticidad"].ToString();
            nombre = campo["nombre"].ToString();
        }

        // var nombre = _ControlBD.usuario(query);
        txtusuario.Text = nombre;
        mjsError.Visible = false;
        mjsError.InnerText = "";

        //txtdescrip.Visible = false;

        btnVolver.Click += volver;
        //btnBuscarSoli.Click += BuscaSolicitud;

        if (!IsPostBack)
        {
            marcaEdit = 0;

            //string script = "$(document).ready(function () { $('[id*=BtnConfirmar]').click(); });";
            //ClientScript.RegisterStartupScript(this.GetType(), "load", script, true);
            LlenarComboMarcas();
            LlenarComboLocal();
            string VFC = ""; // Session["VFCNro"].ToString();

            if (VFC != "")
            {
                msjesError.Visible = true;
                msjesError.InnerText = "Se generaron los VFC Solicitados (" + VFC + "), Favor revisar su correo con detalle";
                Session["VFCNro"] = "";
            }
        }
        else
        {
            Session["VFCNro"] = "";
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
    protected void LimpiaCampos()
    {
        txtCodRep.Text = "";
        txtVin.Text = "";
        txtnumsoli.Text = "";
        combomarcas.Enabled = true;
        //combomarcas.SelectedValue = "0";
        txtdesde.Text = "";
        txtHasta.Text = "";
        //Combotipotrans.SelectedValue = "0";
        mjsError.Visible = false;
    }
    public string TipoEnvio(string marca)
    {
        string query1 = "", query2;
        string marc = marca;
        int idmar = 0;

        if (marca == "CHRYSLER" || marca == "JEEP" || marca == "RAM" || marca == "DODGE")
        {
            idmar = 1;
            query2 = "SELECT nom_Centro FROM t_Envios WHERE id_envio = '" + idmar + "'";
            query1 = Convert.ToString(_ControlBD.Proveedornom(query2));
        }
        if (marca == "MITSUBISHI")
        {
            idmar = 5;
            query2 = "SELECT nom_Centro FROM t_Envios WHERE id_envio = '" + idmar + "'";
            query1 = Convert.ToString(_ControlBD.Proveedornom(query2));
        }
        if (marca == "CHERY")
        {
            idmar = 6;
            query2 = "SELECT nom_Centro FROM t_Envios WHERE id_envio = '" + idmar + "'";
            query1 = Convert.ToString(_ControlBD.Proveedornom(query2));
        }
        if (marca == "TATA")
        {
            idmar = 7;
            query2 = "SELECT nom_Centro FROM t_Envios WHERE id_envio = '" + idmar + "'";
            query1 = Convert.ToString(_ControlBD.Proveedornom(query2));
        }
        if (marca == "MG")
        {
            idmar = 8;
            query2 = "SELECT nom_Centro FROM t_Envios WHERE id_envio = '" + idmar + "'";
            query1 = Convert.ToString(_ControlBD.Proveedornom(query2));
        }
        if (marca == "MASERATI" || marca == "ALFA ROMEO")
        {
            idmar = 4;
            query2 = "SELECT nom_Centro FROM t_Envios WHERE id_envio = '" + idmar + "'";
            query1 = Convert.ToString(_ControlBD.Proveedornom(query2));
        }
        if (marca == "FIAT")
        {
            idmar = 2;
            query2 = "SELECT nom_Centro FROM t_Envios WHERE id_envio = '" + idmar + "'";
            query1 = Convert.ToString(_ControlBD.Proveedornom(query2));
        }
        if (marca == "SSANGYONG")
        {
            idmar = 10;
            query2 = "SELECT nom_Centro FROM t_Envios WHERE id_envio = '" + idmar + "'";
            query1 = Convert.ToString(_ControlBD.Proveedornom(query2));
        }
        return query1;
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
    public int GetTotal()
    {
        return TotalUnitPrice;
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
            //con.Close(); comentado por maikol 04102021(se produce error al cerra conexion)
            return null;
        }
    }

    protected void volver(object sender, EventArgs e)
    {

        btnBuscarSoli.Visible = true;
        btnVolver.Visible = false;

    }
    private String crearXml(String cantidad, String codigo, String detalle, String marca, String vin,
                           String creador, String descripcionUsuario, String dealer,
                           String direccion, String tipoPedido, String opcionvfc, String km, String nSiniestro, String numsoli, String viaImp, String CC, String codsap, String codsapclt, String precio, String critVFC, String obCritVFC)
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
                                new XElement("vin", vin),
                                new XElement("creador", creador),
                                new XElement("descripcionUsuario", descripcionUsuario),
                                new XElement("dealer", dealer),
                                new XElement("direccion", direccion),
                                new XElement("tipoPedido", tipoPedido),
                                new XElement("file", ""),
                                new XElement("opcionvfc", opcionvfc),
                                new XElement("km", km),
                                new XElement("nsiniestro", nSiniestro),
                                new XElement("CC", CC),
                                new XElement("codsap", codsap),
                                new XElement("nrocotiza", numsoli),
                                new XElement("viaimportacion", viaImp),
                                new XElement("codsapclte", codsapclt),
                                new XElement("preciorepuesto", precio),
                                new XElement("critico", critVFC),
                                new XElement("obsvfc", obCritVFC)

                                //new XElement("nrocotiza",)
                            )
                    )
               );
        return miXML.ToString();

    }//fin metodoescribeXml
    private String getHacerVin(String xmlDatosAEnviar, String userVFC, String us3rVF)
    {
        //saveVFC2 = new SaveVFC2();
        try
        {
            //saveVFC2.setVFC(xmlDatosAEnviar, _usuarioVFC, _contrasenaVFC);
            //saveVFC2.setVFC obj = new saveVFC2.SaveVFCSoapClient();
            //return saveVfc.setVFC(xmlDatosAEnviar, _usuarioVFC, _contrasenaVFC);
            //return qas.Body._xmlData =xmlDatosAEnviar;
            return ""; // saveVFC2.setVFC(xmlDatosAEnviar, _usuarioVFC, _contrasenaVFC);
        }
        catch
        {
            return "";
        }
    }
    protected void btnBuscarSoli_Click(object sender, EventArgs e)
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
                Response.AddHeader("content-disposition", "attachment;filename=ReporteCotizaciones.xlsx");
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
}
