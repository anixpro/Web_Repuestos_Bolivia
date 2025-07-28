using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Diagnostics;
using log4net;
using log4net.Config;
using System.Data.Sql;
using System.Data.SqlClient;
public partial class Vistas_Devoluciones : System.Web.UI.Page
{
    SQL_DevRec _SQL = new SQL_DevRec();
    SendMail_helper _mail = new SendMail_helper();
    ControlBDSolicitud _ControlBD = new ControlBDSolicitud();
    SapAPI _sapApi = new SapAPI();

    //public static string zona;
    //public static string prefijo;
    //public static string url;
    public static string DetalleSolicitud;

    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_Devoluciones));


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            cargacombos();
        }

    }
    protected void btnBuscarF_Click(object sender, EventArgs e)
    {
        msjesError.Visible = false;
        msjError.Visible = false;
        Panel1.Visible = false;
        btnSolicitar.Enabled = true;

        string zona = "";
        string prefijo = "";
        string url = "";

        HdnPrefijo.Value = "";
        HdnUrl.Value = "";
        HdnZona.Value = "";


        FAC.DT_Consulta_Factura_Request request = new FAC.DT_Consulta_Factura_Request();
        //FAC.DT_Consulta_Factura_RequestI_BUKRS i_burks = new FAC.DT_Consulta_Factura_RequestI_BUKRS();
        FAC.DT_Consulta_Factura_RequestI_FKDAT i_fkdat = new FAC.DT_Consulta_Factura_RequestI_FKDAT();

        FAC.DT_Consulta_Factura_RequestI_VBELN_DEV i_vblen_dev = new FAC.DT_Consulta_Factura_RequestI_VBELN_DEV();
        FAC.DT_Consulta_Factura_RequestI_VBELN_IFAC i_vblen_ifac = new FAC.DT_Consulta_Factura_RequestI_VBELN_IFAC();
        FAC.DT_Consulta_Factura_RequestI_VBELN_PED i_vblen_pedido = new FAC.DT_Consulta_Factura_RequestI_VBELN_PED();

        FAC.DT_Consulta_Factura_RequestI_XBLNR[] i_xblnr = new FAC.DT_Consulta_Factura_RequestI_XBLNR[1]; //<I_XBLNR>
        FAC.DT_Consulta_Factura_RequestI_STCD1[] i_stcd1 = new FAC.DT_Consulta_Factura_RequestI_STCD1[1]; //<I_STCD1>
        FAC.DT_Consulta_Factura_RequestI_BUKRS[] i_burks = new FAC.DT_Consulta_Factura_RequestI_BUKRS[1]; //<I_BUKRS>
        FAC.DT_Consulta_Factura_RequestI_VBELN_IFAC[] i_vbeln_ifac = new FAC.DT_Consulta_Factura_RequestI_VBELN_IFAC[1]; //<I_VBELN_IFAC>

        FAC.DT_Consulta_Factura_Response response = new FAC.DT_Consulta_Factura_Response();
        FAC.SI_Consulta_Factura_OutService consulta = new FAC.SI_Consulta_Factura_OutService();

        int i = 0;
        int coderror = 0;
        string msgerror = "";
        try
        {
            //Inicio Consulta Facturas
            _SQL.borradatossession(1, Session["rut"].ToString(), Session["idSession"].ToString(), out coderror, out msgerror);
            if (coderror != 0)
            {
                msjError.InnerText = "Se a producido un error SQL, informe a SkBergé";
                msjError.Visible = true;
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_limpiatablas] Message: " + msgerror + " Inner: " + coderror);
                return;
            }

            string rutDealer = _SQL.rutconcesionario(2, Session["rut"].ToString(), out coderror, out msgerror);
            //<I_STCD1>
            i_stcd1[i] = new FAC.DT_Consulta_Factura_RequestI_STCD1();
            i_stcd1[i].STCD1_LOW = rutDealer;//"76349970-7"; //Rut Concesionario
            i_stcd1[i].STCD1_HIGH = "";

            //<I_BUKRS>
            i_burks[i] = new FAC.DT_Consulta_Factura_RequestI_BUKRS();
            i_burks[i].BUKRS_LOW = drMarca.SelectedValue;
            i_burks[i].BUKRS_HIGH = "";

            //<I_VBELN_IFAC>
            i_vbeln_ifac[i] = new FAC.DT_Consulta_Factura_RequestI_VBELN_IFAC();
            i_vbeln_ifac[i].VBELN_LOW = txtFactura.Text;//"3201176519";
            i_vbeln_ifac[i].VBELN_HIGH = "";

            request.I_STCD1 = i_stcd1;
            request.I_BUKRS = i_burks;
            request.I_VBELN_IFAC = i_vbeln_ifac;

            consulta.Credentials = new System.Net.NetworkCredential("INT_WREPF_SKB", "5k82017PoP");
            consulta.PreAuthenticate = true;

            response = consulta.SI_Consulta_Factura_Out(request);

            if (response.ET_PED_CAB != null)
            {
                Panel1.Visible = true;
                //Cabecera
                if (response.ET_PED_CAB != null)
                {
                    foreach (FAC.DT_Consulta_Factura_ResponseET_PED_CAB datosfac in response.ET_PED_CAB)
                    {
                        lblNombre.Text = datosfac.NAME1.ToString();
                        lblSociedad.Text = datosfac.BUKRS.ToString();
                        lblRut.Text = datosfac.STCD1.ToString();
                        lblPedido.Text = datosfac.VBELN.ToString();
                    }
                }

                //PEDIDO

                if (response.ET_PED_DET != null)
                {
                    foreach (FAC.DT_Consulta_Factura_ResponseET_PED_DET datosfac in response.ET_PED_DET)
                    {
                        zona = datosfac.LZONE.ToString();
                        HdnZona.Value = datosfac.LZONE.ToString();
                    }
                }
                //Factura
                if (response.ET_PED_FAC != null)
                {

                    foreach (FAC.DT_Consulta_Factura_ResponseET_PED_FAC datosfac in response.ET_PED_FAC)
                    {
                        int Cant = 0; string scant = "";
                        decimal neto = 0; string sneto = "";
                        decimal iva = 0; string siva = "";
                        decimal subtotal = 0; string ssubtotal = "";

                        if (datosfac.VBELN.ToString() == txtFactura.Text)
                        {
                            hypFactura.NavigateUrl = datosfac.URL.ToString();
                            url = datosfac.URL.ToString();
                            HdnUrl.Value = datosfac.URL.ToString();
                            lblFecFac.Text = datosfac.FKDAT.ToString();
                            lblNumeroFactura.Text = datosfac.VBELN.ToString();

                            Cant = datosfac.FKIMG.IndexOf(".");
                            if (Cant >= 0)
                            {
                                scant = datosfac.FKIMG.Remove(Cant, 4).Trim();
                            }


                            sneto = datosfac.NETWR;
                            siva = datosfac.MWSBP;
                            ssubtotal = datosfac.BRTWR;


                            //neto = datosfac.NETWR.IndexOf(".");
                            //if (neto >= 0)
                            //{
                            //    sneto = datosfac.NETWR.Remove(neto, 5).Trim();
                            //}

                            //iva = datosfac.MWSBP.IndexOf(".");
                            //if (iva >= 0)
                            //{
                            //    siva = datosfac.MWSBP.Remove(iva, 5).Trim();
                            //}

                            //subtotal = datosfac.BRTWR.IndexOf(".");
                            //if (subtotal >= 0)
                            //{
                            //    ssubtotal = datosfac.BRTWR.Remove(subtotal, 5).Trim();
                            //}

                            int largo = datosfac.MFRPN.ToString().Length;
                            string repuesto = datosfac.MFRPN.ToString().Substring(3, largo - 3);
                            prefijo = datosfac.MFRPN.ToString().Substring(0, 3);
                            HdnPrefijo.Value = datosfac.MFRPN.ToString().Substring(0, 3);

                            if (datosfac.MFRPN.ToString().Substring(0, 3) == "SPR")
                            {
                                HdnPrefijo.Value = "MGR";
                            }

                            

                            _SQL.InsertaDocumentos(1,
                                                    Session["idSession"].ToString(),
                                                    Session["rut"].ToString(),
                                                    datosfac.VBELV.ToString(),
                                                    datosfac.POSNV.ToString(),
                                                    datosfac.VBELN.ToString(),
                                                    datosfac.POSNN.ToString(),
                                                    Convert.ToInt32(scant),
                                                    datosfac.VRKME.ToString(),
                                                    Convert.ToDecimal(sneto.Replace(".",",")),
                                                    Convert.ToDecimal(siva.Replace(".", ",")),
                                                    Convert.ToDecimal(ssubtotal.Replace(".", ",")),
                                                    datosfac.WAERK.ToString(),
                                                    repuesto,//datosfac.MFRPN.ToString(),
                                                    datosfac.ARKTX.ToString(),
                                                    datosfac.FKDAT.ToString(),
                                                    datosfac.URL.ToString(),
                                                    datosfac.ERNAM.ToString(),
                                                    zona,
                                                    prefijo,
                                                    out coderror,
                                                    out msgerror);

                            if (coderror != 0)
                            {
                                msjError.InnerText = "Error SQL al insertar linea " + datosfac.POSNN.ToString() + ", del repuesto " + datosfac.MFRPN.ToString();
                                msjError.Visible = true;
                                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_Guarda_Datos_FAC] Message: " + msgerror + " Inner: " + coderror);
                            }
                        }

                    }
                }


                //Nota credito
                if (response.ET_PED_NC != null)
                {
                    foreach (FAC.DT_Consulta_Factura_ResponseET_PED_NC datosfac in response.ET_PED_NC)
                    {
                        int Cant = 0; string scant = "";
                        decimal neto = 0; string sneto = "";
                        decimal iva = 0; string siva = "";
                        decimal subtotal = 0; string ssubtotal = "";

                        if (datosfac.VBELN_OR.ToString() == txtFactura.Text)
                        {
                            Cant = datosfac.FKIMG.IndexOf(".");
                            if (Cant >= 0)
                            {
                                scant = datosfac.FKIMG.Remove(Cant, 4).Trim();
                            }

                            //neto = datosfac.NETWR.IndexOf(".");
                            sneto = datosfac.NETWR;
                            //if (neto >= 0)
                            //{
                            //    sneto = datosfac.NETWR.Remove(neto, 5).Trim();
                            //}

                            //iva = datosfac.MWSBP.IndexOf(".");
                            siva = datosfac.MWSBP;
                            //if (iva >= 0)
                            //{
                            //    siva = datosfac.MWSBP.Remove(iva, 5).Trim();
                            //}

                            //subtotal = datosfac.BRTWR.IndexOf(".");
                            ssubtotal = datosfac.BRTWR;
                            //if (subtotal >= 0)
                            //{
                            //    ssubtotal = datosfac.BRTWR.Remove(subtotal, 5).Trim();
                            //}

                            int largo = datosfac.MFRPN.ToString().Length;
                            string repuesto = datosfac.MFRPN.ToString().Substring(3, largo - 3);

                            _SQL.InsertaDocumentos(2,
                                                    Session["idSession"].ToString(),
                                                    Session["rut"].ToString(),
                                                    datosfac.VBELN_OR.ToString(),
                                                    datosfac.POSNR_OR.ToString(),
                                                    datosfac.VBELN.ToString(),
                                                    datosfac.POSNV.ToString(),
                                                    Convert.ToInt32(scant),
                                                    datosfac.VRKME.ToString(),
                                                    Convert.ToDecimal(sneto.Replace(".", ",")),
                                                    Convert.ToDecimal(siva.Replace(".", ",")),
                                                    Convert.ToDecimal(ssubtotal.Replace(".", ",")),
                                                    datosfac.WAERK.ToString(),
                                                    repuesto,//datosfac.MFRPN.ToString(),
                                                    datosfac.ARKTX.ToString(),
                                                    datosfac.FKDAT.ToString(),
                                                    datosfac.URL.ToString(),
                                                    datosfac.ERNAM.ToString(),
                                                    zona,
                                                    "",
                                                    out coderror,
                                                    out msgerror);

                            if (coderror != 0)
                            {
                                msjError.InnerText = "Error SQL al insertar linea " + datosfac.POSNN.ToString() + ", del repuesto " + datosfac.MFRPN.ToString();
                                msjError.Visible = true;
                                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_Guarda_Datos_NC] Message: " + msgerror + " Inner: " + coderror);
                            }
                        }

                    }
                }


                //nota debito
                if (response.ET_PED_ND != null)
                {
                    foreach (FAC.DT_Consulta_Factura_ResponseET_PED_ND datosfac in response.ET_PED_ND)
                    {
                        int Cant = 0; string scant = "";
                        decimal neto = 0; string sneto = "";
                        decimal iva = 0; string siva = "";
                        decimal subtotal = 0; string ssubtotal = "";

                        Cant = datosfac.FKIMG.IndexOf(".");
                        if (Cant >= 0)
                        {
                            scant = datosfac.FKIMG.Remove(Cant, 4).Trim();
                        }
                        sneto = datosfac.NETWR;
                        siva = datosfac.MWSBP;
                        ssubtotal = datosfac.BRTWR;
                        //neto = datosfac.NETWR.IndexOf(".");
                        //if (neto >= 0)
                        //{
                        //    sneto = datosfac.NETWR.Remove(neto, 5).Trim();
                        //}

                        //iva = datosfac.MWSBP.IndexOf(".");
                        //if (iva >= 0)
                        //{
                        //    siva = datosfac.MWSBP.Remove(iva, 5).Trim();
                        //}

                        //subtotal = datosfac.BRTWR.IndexOf(".");
                        //if (subtotal >= 0)
                        //{
                        //    ssubtotal = datosfac.BRTWR.Remove(subtotal, 5).Trim();
                        //}

                        int largo = datosfac.MFRPN.ToString().Length;
                        string repuesto = datosfac.MFRPN.ToString().Substring(3, largo - 3);

                        _SQL.InsertaDocumentos(3,
                                                Session["idSession"].ToString(),
                                                Session["rut"].ToString(),
                                                datosfac.VBELN_OR.ToString(),
                                                datosfac.POSNR_OR.ToString(),
                                                datosfac.VBELN.ToString(),
                                                datosfac.POSNV.ToString(),
                                                Convert.ToInt32(scant),
                                                datosfac.VRKME.ToString(),
                                                Convert.ToDecimal(sneto.Replace(".", ",")),
                                                Convert.ToDecimal(siva.Replace(".", ",")),
                                                Convert.ToDecimal(ssubtotal.Replace(".", ",")),
                                                datosfac.WAERK.ToString(),
                                                repuesto,//datosfac.MFRPN.ToString(),
                                                datosfac.ARKTX.ToString(),
                                                datosfac.FKDAT.ToString(),
                                                datosfac.URL.ToString(),
                                                datosfac.ERNAM.ToString(),
                                                zona,
                                                "",
                                                out coderror,
                                                out msgerror);

                        if (coderror != 0)
                        {
                            msjError.InnerText = "Error SQL al insertar linea " + datosfac.POSNN.ToString() + ", del repuesto " + datosfac.MFRPN.ToString();
                            msjError.Visible = true;
                            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_Guarda_Datos_ND] Message: " + msgerror + " Inner: " + coderror);
                        }
                    }
                }


                grDetalle.DataSource = _SQL.llenaGrillaFactura(1, txtFactura.Text, Session["idSession"].ToString(), Session["rut"].ToString(), out coderror, out msgerror);
                grDetalle.DataBind();

                DateTime d1 = DateTime.Now;
                DateTime d2 = DateTime.ParseExact(lblFecFac.Text, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                double dias = (d1 - d2).TotalDays;
                int aux = 0;

                if (dias > 90)
                {
                    msjError.InnerText = "Factura se encuentra fuera de plazo para realizar solicitud.";
                    msjError.Visible = true;
                    aux = 1;
                    btnSolicitar.Enabled = false;
                }

                for (int j = 0; j < grDetalle.Rows.Count; j++)
                {
                    GridViewRow row = grDetalle.Rows[j];
                    string cantidad = row.Cells[1].Text;
                    RadioButton rdDevolucion = (RadioButton)grDetalle.Rows[j].FindControl("rdDevolucion");
                    RadioButton rdReclamo = (RadioButton)grDetalle.Rows[j].FindControl("rdReclamo");
                    DropDownList drMotivo = (DropDownList)grDetalle.Rows[j].FindControl("drMotivo");
                    TextBox txtCantSol = (TextBox)grDetalle.Rows[j].FindControl("txtCantSol");
                    FileUpload flEvidencias = (FileUpload)grDetalle.Rows[j].FindControl("flEvidencias");
                    TextBox txtObservacion = (TextBox)grDetalle.Rows[j].FindControl("txtObservacion");
                    TextBox txtPdq = (TextBox)grDetalle.Rows[j].FindControl("txtPdq");

                    if (Convert.ToInt32(cantidad) < 1)
                    {
                        rdDevolucion.Enabled = false;
                        rdReclamo.Enabled = false;
                        drMotivo.Enabled = false;
                        txtCantSol.Enabled = false;
                        flEvidencias.Enabled = false;
                        txtObservacion.Enabled = false;
                        txtPdq.Enabled = false;
                    }
                    else
                    {
                        if (aux == 1)
                        {
                            rdDevolucion.Enabled = false;
                            rdReclamo.Enabled = false;
                            drMotivo.Enabled = false;
                            txtCantSol.Enabled = false;
                            flEvidencias.Enabled = false;
                            txtObservacion.Enabled = false;
                            txtPdq.Enabled = false;
                        }
                    }
                }
            }
            else
            {
                msjError.InnerText = "Error: No se encuentran datos en SAP";
                msjError.Visible = true;
            }
        }
        catch (Exception ex)
        {
            logger.Error("En [BotonBuscar] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_Consulta_Factura] Message: " + ex.Message + " Inner: " + ex.InnerException);
        }
    }
    protected void rdDevolucion_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            int coderror = 0;
            string msgerror = "";
            int flag = 0;

            DataSet motivo = new DataSet();
            motivo = _SQL.llenacombomotivo(2, out coderror, out msgerror, out flag);
            if (coderror != 0)
            {
                msjError.InnerText = "Error al cargar combo motivo, contancte al administrador.";
                msjError.Visible = true;
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_CargaCombo_Devolucion] Message: " + msgerror + " Inner: " + coderror);
            }
            else
            {
                for (int i = 0; i < grDetalle.Rows.Count; i++)
                {
                    GridViewRow row = grDetalle.Rows[i];
                    string poscicion = row.Cells[1].Text;
                    RadioButton rdDevolucion = (RadioButton)grDetalle.Rows[i].FindControl("rdDevolucion");
                    DropDownList drMotivo = (DropDownList)grDetalle.Rows[i].FindControl("drMotivo");

                    drMotivo.Focus();

                    if (rdDevolucion.Checked)
                    {
                        drMotivo.Focus();
                        drMotivo.DataSource = motivo;
                        drMotivo.DataValueField = "id_motivo";
                        drMotivo.DataTextField = "desc_motivo";
                        drMotivo.DataBind();
                    }

                }
            }
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Se ha generado un error favor contacte a administrador.";
            logger.Error("En [rdDevolucion_CheckedChanged] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
            msjesError.Visible = true;
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_chkchange_Devolucion] Message: " + ex.Message + " Inner: " + ex.InnerException);
        }



    }
    protected void rdReclamo_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            int coderror = 0;
            string msgerror = "";
            int flag = 0;

            DataSet motivo = new DataSet();
            motivo = _SQL.llenacombomotivo(1, out coderror, out msgerror, out flag);
            if (coderror != 0)
            {
                msjError.InnerText = "Error al cargar combo motivo, contancte al administrador.";
                msjError.Visible = true;
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_CargaCombo_Reclamo] Message: " + msgerror + " Inner: " + coderror);
            }
            else
            {
                for (int i = 0; i < grDetalle.Rows.Count; i++)
                {
                    GridViewRow row = grDetalle.Rows[i];
                    string poscicion = row.Cells[1].Text;
                    RadioButton rdReclamo = (RadioButton)grDetalle.Rows[i].FindControl("rdReclamo");
                    DropDownList drMotivo = (DropDownList)grDetalle.Rows[i].FindControl("drMotivo");

                    drMotivo.Focus();

                    if (rdReclamo.Checked)
                    {
                        drMotivo.Focus();
                        drMotivo.DataSource = motivo;
                        drMotivo.DataValueField = "id_motivo";
                        drMotivo.DataTextField = "desc_motivo";
                        drMotivo.DataBind();
                    }

                }
            }
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Se ha generado un error favor contacte a administrador.";
            logger.Error("En [rdReclamo_ChekedChanged] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
            msjesError.Visible = true;
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_chkchange_Reclamo] Message: " + ex.Message + " Inner: " + ex.InnerException);

        }


    }

    protected void cargacombos()
    {
        try
        {
            int coderror = 0;
            string msgerror = "";

            DataSet motivo = new DataSet();
            motivo = _SQL.cargacombos(1, Session["rut"].ToString(), out coderror, out msgerror);
            if (coderror != 0)
            {
                msjError.InnerText = "Error al cargar combo motivo, contancte al administrador.";
                msjError.Visible = true;
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_CargaCombo_Reclamo] Message: " + msgerror + " Inner: " + coderror);
            }
            else
            {
                drMarca.DataSource = motivo;
                drMarca.DataValueField = "orgventas";
                drMarca.DataTextField = "nombremarca";
                drMarca.DataBind();
            }
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Se ha generado un error favor contacte a administrador.";
            msjesError.Visible = true;
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_CargaCombos] Message: " + ex.Message + " Inner: " + ex.InnerException);
        }

    }
    protected void btnSolicitar_Click(object sender, EventArgs e)
    {
        try
        {
            int coderror = 0;
            string msgerror = "";
            int flag = 0;
            string detalleSolicitudMail = "";
            int z = 1;

            int aux = 0;

            string RoD = "";

            DataSet nulo = new DataSet();
            nulo = _SQL.llenacombomotivo(3, out coderror, out msgerror, out flag);
            flag = flag + 1;
            DateTime fechasol = DateTime.Now;

            //Validacion Previa
            string detalle = "";
            for (int j = 0; j < grDetalle.Rows.Count; j++)
            {
                GridViewRow rowv = grDetalle.Rows[j];
                string poscicion = rowv.Cells[0].Text;
                FileUpload flEvidencias = (FileUpload)grDetalle.Rows[j].FindControl("flEvidencias");
                RadioButton rdReclamo = (RadioButton)grDetalle.Rows[j].FindControl("rdReclamo");
                TextBox txtPdq = (TextBox)grDetalle.Rows[j].FindControl("txtPdq");
                DropDownList drMotivo = (DropDownList)grDetalle.Rows[j].FindControl("drMotivo");
                if (rdReclamo.Checked == true)
                {
                    if (flEvidencias.FileName == "")
                    {
                        if (detalle == "")
                        {
                            detalle = poscicion;
                        }
                        else
                        {
                            detalle = detalle + '-' + poscicion;
                        }

                    }

                    if (txtPdq.Text == "" && drMotivo.SelectedValue == "1")
                    {
                        if (detalle == "")
                        {
                            detalle = poscicion;
                        }
                        else
                        {
                            detalle = detalle + '-' + poscicion;
                        }
                    }
                }
            }
            if (detalle != "")
            {
                msjesError.InnerText = "Las siguientes posiciones no cuentan con evidencias y/o numero PDQ : " + detalle;
                msjesError.Visible = true;
                return;
            }

            for (int i = 0; i < grDetalle.Rows.Count; i++)
            {
                GridViewRow row = grDetalle.Rows[i];

                string nrodoc = txtFactura.Text;
                string poscicion = row.Cells[0].Text;
                decimal neto = Convert.ToDecimal(row.Cells[2].Text);
                int cantidad = Convert.ToInt32(row.Cells[1].Text);
                string codrepto = row.Cells[3].Text;
                string descodrepto = row.Cells[4].Text;
                string fechadoc = lblFecFac.Text;


                RadioButton rdReclamo = (RadioButton)grDetalle.Rows[i].FindControl("rdReclamo");
                RadioButton rdDevolucion = (RadioButton)grDetalle.Rows[i].FindControl("rdDevolucion");
                DropDownList drMotivo = (DropDownList)grDetalle.Rows[i].FindControl("drMotivo");
                FileUpload flEvidencias = (FileUpload)grDetalle.Rows[i].FindControl("flEvidencias");
                TextBox txtCantSol = (TextBox)grDetalle.Rows[i].FindControl("txtCantSol");
                TextBox txtPdq = (TextBox)grDetalle.Rows[i].FindControl("txtPdq");

                if ((rdReclamo.Checked == true || rdDevolucion.Checked == true) && txtCantSol.Text != "")
                {
                    
                    //Subida archivo
                    Boolean fileOK = false;
                    String path = Server.MapPath("~/DocDR/");
                    string ruta = "";

                    if (flEvidencias.HasFile)
                    {
                        String fileExtension = System.IO.Path.GetExtension(flEvidencias.FileName).ToLower();
                        String[] allowedExtensions = { ".gif", ".jpg", ".rar" };
                        for (int k = 0; k < allowedExtensions.Length; k++)
                        {
                            if (fileExtension == allowedExtensions[k])
                            {
                                fileOK = true;
                            }
                        }
                    }
                    if (fileOK)
                    {
                        flEvidencias.PostedFile.SaveAs(path + flEvidencias.FileName);
                        ruta = "~/DocDR/" + flEvidencias.FileName;
                    }



                    aux = 1;
                    if (rdReclamo.Checked)
                    {
                        RoD = "R";
                    }
                    if (rdDevolucion.Checked)
                    {
                        RoD = "D";
                    }

                    string miMarca = drMarca.SelectedItem.Text;
                    _SQL.creasolicitud(1,
                                        flag,
                                        Session["rut"].ToString(),
                                        Session["idSession"].ToString(),
                                        nrodoc,
                                        poscicion,
                                        cantidad,
                                        neto,
                                        codrepto,
                                        descodrepto,
                                        fechadoc,
                                        Convert.ToInt32(txtCantSol.Text),
                                        RoD,
                                        fechasol,
                                        Convert.ToInt32(drMotivo.SelectedValue),
                                        ruta,
                                        txtPdq.Text,
                                        HdnUrl.Value.ToString(),
                                        HdnZona.Value.ToString(),
                                        HdnPrefijo.Value.ToString(),
                                        miMarca,
                                        out coderror,
                                        out msgerror);

                    detalleSolicitudMail += z + ". " + " | SOLICITUD | " + nrodoc + " | " + poscicion + " | " + codrepto + " | " + descodrepto + " | " + txtCantSol.Text + " | " + fechasol + " | " + drMotivo.SelectedItem.ToString() + " <br/>";
                    z++;
                }
            }

            if (aux == 1)
            {
                msjesError.InnerText = "Se a generado la solicitud: " + flag;
                msjesError.Visible = true;
                Panel1.Visible = false;

                String query2 = "SELECT email FROM persona WHERE rut = '" + Session["rut"].ToString() + "'";
                String correo = _ControlBD.Email(query2);
                String cuerpo = "Se ha generado la solicitud numero: " + flag;
                String correoCCR = "";
                String correoCCC = "";
                String textoCorreo = "";

                //Encargado Reclamos
                DataSet dsr = new DataSet();
                dsr = _SQL.cargacombos(3, Session["rut"].ToString(), out coderror, out msgerror);

                if (dsr.Tables.Count != 0)
                {
                    foreach (DataRow campo in dsr.Tables[0].Rows)
                    {
                        correoCCR = campo["Correos"].ToString();
                    } 
                }
                

                //Encargado Concesionario
                DataSet dsc = new DataSet();
                dsc = _SQL.cargacombos(5, Session["rut"].ToString(), out coderror, out msgerror);
                if (dsc.Tables.Count != 0)
                {
                    foreach (DataRow campo in dsc.Tables[0].Rows)
                    {
                        correoCCC = campo["Correos"].ToString();
                    } 
                }
                               

                if (RoD == "R")
                {
                    textoCorreo = "";
                    textoCorreo = "Usted ha realizado solicitud de Reclamo ";
                    textoCorreo += "<br/>Concesionario: " + lblNombre.Text;
                    textoCorreo += "<br/>Numero Factura: " + lblNumeroFactura.Text;
                    textoCorreo += "<br/>Marca: " + lblSociedad.Text;
                    textoCorreo += "<br/><br/>Detalle del Solicitud";
                    textoCorreo += "<br/>" + detalleSolicitudMail;

                    _mail.EnviarCorreoHTML(correo, "Solicitud de Reclamo: " + flag, textoCorreo, correoCCR);
                }
                else
                {
                    textoCorreo = "";
                    textoCorreo = "Usted ha realizado solicitud de Reclamo ";
                    textoCorreo += "<br/>Concesionario: " + lblNombre.Text;
                    textoCorreo += "<br/>Numero Factura: " + lblNumeroFactura.Text;
                    textoCorreo += "<br/>Marca: " + lblSociedad.Text;
                    textoCorreo += "<br/><br/>Detalle del Solicitud";
                    textoCorreo += "<br/>" + detalleSolicitudMail;

                    _mail.EnviarCorreoHTML(correo, "Solicitud de Devolución: " + flag, textoCorreo, correoCCC);
                }

            }
            else
            {
                msjesError.InnerText = "No se a seleccionado ningun registro para devolucion o reclamo.";
                msjesError.Visible = true;
            }
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Se ha generado un error favor contacte a administrador.";
            logger.Error("En [SinStock] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
            msjesError.Visible = true;
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_btnSolicitar] Message: " + ex.Message + " Inner: " + ex.InnerException);
        }
    }
    

    protected void grdetalle_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("save"))
        {
            int coderror = 0;
            string msgerror = "";

            int index = Convert.ToInt32(e.CommandArgument);
            GridViewRow selectedRow = grDetalle.Rows[index];
                        
            int rowIndex = int.Parse(e.CommandArgument.ToString().Trim());
            
            FileUpload flEvidencias2 = (FileUpload)selectedRow.FindControl("flEvidencias");

            FileUpload flEvidencias = (FileUpload)grDetalle.Rows[index].FindControl("flEvidencias");
            //String poscicion = grDetalle.DataKeys[rowIndex].Values["pos_factura"].ToString().Trim();
            string nrofac = lblNumeroFactura.Text;

            Boolean fileOK = false;
            String path = Server.MapPath("~/DocDR/");

            if (flEvidencias.HasFile)
            {
                String fileExtension = System.IO.Path.GetExtension(flEvidencias.FileName).ToLower();
                String[] allowedExtensions = { ".gif", ".jpg", ".rar" };
                for (int j = 0; j < allowedExtensions.Length; j++)
                {
                    if (fileExtension == allowedExtensions[j])
                    {
                        fileOK = true;
                    }
                }
            }
            if (fileOK)
            {
                flEvidencias.PostedFile.SaveAs(path + flEvidencias.FileName);
                //SQL PARA GUARDAR RUTA
                _SQL.EstadosConcesionario(1, Session["rut"].ToString(), Session["idSession"].ToString(), nrofac, "", path + flEvidencias.FileName, out coderror, out msgerror);
            }
        }

        if (e.CommandName.Equals("limpiar"))
        {
            int coderror = 0;
            string msgerror = "";
            int rowIndex = int.Parse(e.CommandArgument.ToString().Trim());


            RadioButton rdReclamo = (RadioButton)grDetalle.Rows[rowIndex].FindControl("rdReclamo");
            RadioButton rdDevolucion = (RadioButton)grDetalle.Rows[rowIndex].FindControl("rdDevolucion");
            DropDownList drMotivo = (DropDownList)grDetalle.Rows[rowIndex].FindControl("drMotivo");
            FileUpload flEvidencias = (FileUpload)grDetalle.Rows[rowIndex].FindControl("flEvidencias");
            TextBox txtCantSol = (TextBox)grDetalle.Rows[rowIndex].FindControl("txtCantSol");
            TextBox txtPdq = (TextBox)grDetalle.Rows[rowIndex].FindControl("txtPdq");
            TextBox txtObservacion = (TextBox)grDetalle.Rows[rowIndex].FindControl("txtObservacion");

            rdReclamo.Checked = false;
            rdDevolucion.Checked = false;
            drMotivo.Items.Clear();
            //flEvidencias.Attributes.Clear();
            txtCantSol.Text = "";
            txtPdq.Text = "";
            txtObservacion.Text = "";
                        
        }
    }
}