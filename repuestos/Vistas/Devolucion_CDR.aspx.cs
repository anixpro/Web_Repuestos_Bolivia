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
using ClosedXML.Excel;
using System.Linq;
using System.Drawing;
using System.IO;

public partial class Vistas_Devolucion_CDR : System.Web.UI.Page
{
    SQL_DevRec _SQL = new SQL_DevRec();
    SendMail_helper _mail = new SendMail_helper();

    protected void Page_Load(object sender, EventArgs e)
    {
        //Button1.Style["visibility"] = "hidden";
        //Button4.Style["visibility"] = "hidden";
        if (!IsPostBack)
        {
            cargasolicitudesdevolucion();
        }

    }

    private void cargasolicitudesdevolucion()
    {
        try
        {
            int coderror = 0;
            string msgerror = "";

            grSolicitudesDetalleDevolucion.DataSource = _SQL.cargasolicitudes(6, Session["rut"].ToString(), 0, out coderror, out msgerror);

            if (coderror != 0)
            {
                msjesError.InnerText = "Se a generado un error favor contactar a administrador.";
                msjesError.Visible = true;
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_Obtiene_SolicitudesDev] Message: " + msgerror + " Inner: " + coderror);
            }
            else
            {
                grSolicitudesDetalleDevolucion.DataBind();
            }
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Se a generado un error favor contactar a administrador.";
            msjesError.Visible = true;
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_Obtiene_SolicitudesDev_CATCH] Message: " + ex.Message + " Inner: " + ex.InnerException);
        }

    }



    protected void rdAprobarDev_CheckedChanged(object sender, EventArgs e)
    {
        try
        {

            for (int i = 0; i < grSolicitudesDetalleDevolucion.Rows.Count; i++)
            {
                GridViewRow row = grSolicitudesDetalleDevolucion.Rows[i];
                string poscicion = row.Cells[1].Text;
                RadioButton rdAprobarDev = (RadioButton)grSolicitudesDetalleDevolucion.Rows[i].FindControl("rdAprobarDev");
                DropDownList drMoRechazoDev = (DropDownList)grSolicitudesDetalleDevolucion.Rows[i].FindControl("drMoRechazoDev");

                if (rdAprobarDev.Checked)
                {
                    drMoRechazoDev.Items.Clear();
                    drMoRechazoDev.Enabled = false;
                }

            }

        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Se ha generado un error favor contacte a administrador.";
            msjesError.Visible = true;
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_chkchange_RechazoComercial] Message: " + ex.Message + " Inner: " + ex.InnerException);

        }
    }

    protected void rdRechazarDev_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            int coderror = 0;
            string msgerror = "";
            int flag = 0;

            DataSet motivo = new DataSet();
            motivo = _SQL.llenacombomotivo(5, out coderror, out msgerror, out flag);
            if (coderror != 0)
            {
                msjError.InnerText = "Error al cargar combo motivo, contancte al administrador.";
                msjError.Visible = true;
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_CargaCombo_rechazoComercial] Message: " + msgerror + " Inner: " + coderror);
            }
            else
            {
                for (int i = 0; i < grSolicitudesDetalleDevolucion.Rows.Count; i++)
                {
                    GridViewRow row = grSolicitudesDetalleDevolucion.Rows[i];
                    string poscicion = row.Cells[1].Text;
                    RadioButton rdRechazarDev = (RadioButton)grSolicitudesDetalleDevolucion.Rows[i].FindControl("rdRechazarDev");
                    DropDownList drMoRechazoDev = (DropDownList)grSolicitudesDetalleDevolucion.Rows[i].FindControl("drMoRechazoDev");


                    if (rdRechazarDev.Checked)
                    {
                        drMoRechazoDev.Enabled = true;
                        drMoRechazoDev.DataSource = motivo;
                        drMoRechazoDev.DataValueField = "id_motivo";
                        drMoRechazoDev.DataTextField = "desc_motivo";
                        drMoRechazoDev.DataBind();
                    }

                }
            }
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Se ha generado un error favor contacte a administrador.";
            msjesError.Visible = true;
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_chkchange_RechazoComercial] Message: " + ex.Message + " Inner: " + ex.InnerException);

        }
    }

    protected void btnGuardarDevolucion_Click(object sender, EventArgs e)
    {
        try
        {
            int coderror = 0;
            string msgerror = "";
            DateTime fechasol = DateTime.Now;

            for (int i = 0; i < grSolicitudesDetalleDevolucion.Rows.Count; i++)
            {
                GridViewRow row = grSolicitudesDetalleDevolucion.Rows[i];
                int id_solicitud = Convert.ToInt32(row.Cells[1].Text);
                string nrofac = row.Cells[2].Text;
                string posfac = row.Cells[3].Text;
                string aprueba = "";
                int Mrechazo = 0;

                string nroaprobacion = row.Cells[11].Text;


                string marca = row.Cells[7].Text;
                string desrepuesto = row.Cells[9].Text;
                string codrepuesto = row.Cells[8].Text;




                CheckBox chkConfirmarDev = (CheckBox)grSolicitudesDetalleDevolucion.Rows[i].FindControl("chkConfirmarDev");
                RadioButton rdAprobarDev = (RadioButton)grSolicitudesDetalleDevolucion.Rows[i].FindControl("rdAprobarDev");
                RadioButton rdRechazarDev = (RadioButton)grSolicitudesDetalleDevolucion.Rows[i].FindControl("rdRechazarDev");
                DropDownList drMoRechazoDev = (DropDownList)grSolicitudesDetalleDevolucion.Rows[i].FindControl("drMoRechazoDev");
                TextBox txtobservacionGR = (TextBox)grSolicitudesDetalleDevolucion.Rows[i].FindControl("txtobservacionGR");

                if (chkConfirmarDev.Checked && (rdAprobarDev.Checked || rdRechazarDev.Checked))
                {
                    if (rdAprobarDev.Checked)
                    {
                        aprueba = "SI";
                    }
                    else
                    {
                        if (rdRechazarDev.Checked)
                        {
                            aprueba = "NO";
                            Mrechazo = Convert.ToInt32(drMoRechazoDev.SelectedValue);
                        }
                    }
                    _SQL.CambiaEstadosSolicitud(3, Session["rut"].ToString(), id_solicitud, nrofac, posfac, fechasol, aprueba, 0, Mrechazo, "", "", "", txtobservacionGR.Text, "", out coderror, out msgerror);


                    //Obtencion Correos            
                    String correoCCR = "";
                    String correoCCC = "";
                    String correo = "";
                    String correoS = "";
                    //Encargado Reclamos
                    DataSet dsr = new DataSet();
                    dsr = _SQL.cargacombos(4, "", out coderror, out msgerror);

                    if (dsr.Tables.Count != 0)
                    {
                        foreach (DataRow campo in dsr.Tables[0].Rows)
                        {
                            correoCCR = campo["Correos"].ToString();
                        }
                    }

                    //Encargado Reclamos
                    DataSet dsO = new DataSet();
                    dsO = _SQL.cargacombos(6, Convert.ToString(id_solicitud), out coderror, out msgerror);

                    if (dsO.Tables.Count != 0)
                    {
                        foreach (DataRow campo in dsO.Tables[0].Rows)
                        {
                            correo = campo["Correos"].ToString();
                        }
                    }

                    //Encargado Encargado supervisor
                    DataSet dsS = new DataSet();
                    dsS = _SQL.cargacombos(7, Convert.ToString(id_solicitud), out coderror, out msgerror);

                    if (dsS.Tables.Count != 0)
                    {
                        foreach (DataRow campo in dsS.Tables[0].Rows)
                        {
                            correoS = campo["Correos"].ToString();
                        }
                    }

                    //Obtenemos URL nota de credito
                    string rutdealer = "";
                    DataSet dsrut = new DataSet();
                    dsrut = _SQL.cargacombos(8, Convert.ToString(id_solicitud), out coderror, out msgerror);
                    if (dsrut.Tables.Count != 0)
                    {
                        foreach (DataRow campo in dsrut.Tables[0].Rows)
                        {
                            rutdealer = campo["rutHolding"].ToString();
                        }
                    }

                    string sociedadConce = "";
                    DataSet dsSoc = new DataSet();
                    dsSoc = _SQL.cargacombos(9, Convert.ToString(id_solicitud), out coderror, out msgerror);
                    if (dsSoc.Tables.Count != 0)
                    {
                        foreach (DataRow campo in dsSoc.Tables[0].Rows)
                        {
                            sociedadConce = campo["VKORG"].ToString();
                        }
                    }


                    string URLnc = obtieneUrlNC(rutdealer, sociedadConce, nrofac, nroaprobacion);
                    //FIN obtener URL nota credito


                    String cuerpo = "";
                    String cuerpo_cabecera = "";

                    cuerpo_cabecera = "Se ha respondido su solicitud de devolución numero: " + Convert.ToString(id_solicitud) + "<br/><br/>";

                    cuerpo += "Aprobada            : " + aprueba + "<br/>";
                    cuerpo += "Marca               : " + marca + "<br/>";
                    cuerpo += "Número Solicitud    : " + Convert.ToString(id_solicitud) + "<br/>";
                    cuerpo += "Número Factura      : " + nrofac + "<br/>";
                    cuerpo += "Posición Factura    : " + posfac + "<br/>";
                    cuerpo += "Código Repuesto     : " + codrepuesto + "<br/>";
                    cuerpo += "Descripción Repuesto: " + desrepuesto + "<br/>";

                    string motivoRechazo = "No presenta";
                    if (drMoRechazoDev.SelectedItem != null )
                    {
                        motivoRechazo = drMoRechazoDev.SelectedItem.ToString();
                    }
                    if (aprueba == "SI")
                    {
                        //cuerpo += "Motivo              : " + motivo + "<br/>";
                        cuerpo += "URL Nota Credito: " + URLnc + "<br/>";
                    }
                    else
                    {
                        cuerpo += "Motivo Rechazo      : " + motivoRechazo + "<br/>";
                    }

                    cuerpo += "<br/><br/>";

                    cuerpo += "Se ha respondido su solicitud de devolución por: " + aprueba;

                    if (correo == "")
                    {
                        _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Envio correo Solicitud Devolución CDR] Message: " + "La solicitud " + Convert.ToSByte(id_solicitud) + ", No contiene correo destinatario");
                    }
                    else
                    {
                        _mail.EnviarCorreoHTML(correo, "Resolución Devolución: " + aprueba, cuerpo, correoCCR + "," + correoS);
                    }

                } //END IF CHECK CONFIRMADO

            }
            //this.ModalPopupExtender2.Hide();
            cargasolicitudesdevolucion();
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Se a generado un error favor contactar a administrador.";
            msjesError.Visible = true;
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_btnGuardar_CATCH] Message: " + ex.Message + " Inner: " + ex.InnerException);
        }

    }

    private string obtieneUrlNC(string rutdealer, string sociedad, string numerofactura, string nroaprobacion)
    {
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

        string url = "";
        int i = 0;
        try
        {
            //<I_STCD1>
            i_stcd1[i] = new FAC.DT_Consulta_Factura_RequestI_STCD1();
            i_stcd1[i].STCD1_LOW = rutdealer;//76349970-7"; //Rut Concesionario
            i_stcd1[i].STCD1_HIGH = "";

            //<I_BUKRS>
            i_burks[i] = new FAC.DT_Consulta_Factura_RequestI_BUKRS();
            i_burks[i].BUKRS_LOW = sociedad;
            i_burks[i].BUKRS_HIGH = "";

            //<I_VBELN_IFAC>
            i_vbeln_ifac[i] = new FAC.DT_Consulta_Factura_RequestI_VBELN_IFAC();
            i_vbeln_ifac[i].VBELN_LOW = numerofactura;//"3201176519";
            i_vbeln_ifac[i].VBELN_HIGH = "";

            request.I_STCD1 = i_stcd1;
            request.I_BUKRS = i_burks;
            request.I_VBELN_IFAC = i_vbeln_ifac;

            consulta.Credentials = new System.Net.NetworkCredential("INT_WREPF_SKB", "5k82017PoP");
            consulta.PreAuthenticate = true;

            response = consulta.SI_Consulta_Factura_Out(request);

            

            //Nota credito
            if (response.ET_PED_NC != null)
            {
                foreach (FAC.DT_Consulta_Factura_ResponseET_PED_NC datosfac in response.ET_PED_NC)
                {
                    if (datosfac.VBELV.ToString() == nroaprobacion)
                    {
                        url = datosfac.URL.ToString();
                    }
                }
            }           
        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_Obtiene_URL] Message: " + ex.Message + " Inner: " + ex.InnerException + "--Datos extras: " + nroaprobacion + "--" + url);
        }

        return url;        
    }

    protected void chRecibo_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            int coderror = 0;
            string msgerror = "";
            int flag = 0;

            for (int i = 0; i < grSolicitudesDetalleDevolucion.Rows.Count; i++)
            {
                GridViewRow row = grSolicitudesDetalleDevolucion.Rows[i];
                string Solicitud = row.Cells[1].Text;
                string nrofac = row.Cells[2].Text;
                string posfac = row.Cells[3].Text;
                CheckBox chRecibo = (CheckBox)grSolicitudesDetalleDevolucion.Rows[i].FindControl("chRecibo");
                if (chRecibo.Checked)
                {
                    //Ejecutar SQL
                    _SQL.EstadosCDRreclamos(2, Session["rut"].ToString(), Convert.ToInt32(Solicitud), nrofac, posfac, out coderror, out msgerror);
                }

            }

            cargasolicitudesdevolucion();
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Se ha generado un error favor contacte a administrador.";
            msjesError.Visible = true;
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_chRecibo_Recibido] Message: " + ex.Message + " Inner: " + ex.InnerException);

        }
    }


    protected void chRecibomaterial_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            int coderror = 0;
            string msgerror = "";
            int flag = 0;

            for (int i = 0; i < grSolicitudesDetalleDevolucion.Rows.Count; i++)
            {
                GridViewRow row = grSolicitudesDetalleDevolucion.Rows[i];
                string Solicitud = row.Cells[1].Text;
                string nrofac = row.Cells[2].Text;
                string posfac = row.Cells[3].Text;
                CheckBox chRecibomaterial = (CheckBox)grSolicitudesDetalleDevolucion.Rows[i].FindControl("chRecibomaterial");
                if (chRecibomaterial.Checked)
                {
                    //Ejecutar SQL
                    _SQL.EstadosCDRreclamos(3, Session["rut"].ToString(), Convert.ToInt32(Solicitud), nrofac, posfac, out coderror, out msgerror);
                }

            }

            cargasolicitudesdevolucion();
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Se ha generado un error favor contacte a administrador.";
            msjesError.Visible = true;
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_chRecibo_Recibido] Message: " + ex.Message + " Inner: " + ex.InnerException);

        }
    }

    protected void chAprobmaterial_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            int coderror = 0;
            string msgerror = "";
            int flag = 0;

            for (int i = 0; i < grSolicitudesDetalleDevolucion.Rows.Count; i++)
            {
                GridViewRow row = grSolicitudesDetalleDevolucion.Rows[i];
                string Solicitud = row.Cells[1].Text;
                string nrofac = row.Cells[2].Text;
                string posfac = row.Cells[3].Text;
                CheckBox chAprobmaterial = (CheckBox)grSolicitudesDetalleDevolucion.Rows[i].FindControl("chAprobmaterial");
                if (chAprobmaterial.Checked)
                {
                    //Ejecutar SQL
                    _SQL.EstadosCDRreclamos(4, Session["rut"].ToString(), Convert.ToInt32(Solicitud), nrofac, posfac, out coderror, out msgerror);
                }

            }

            cargasolicitudesdevolucion();
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Se ha generado un error favor contacte a administrador.";
            msjesError.Visible = true;
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_chRecibo_Recibido] Message: " + ex.Message + " Inner: " + ex.InnerException);

        }
    }

    protected void btnExportar_Click(object sender, EventArgs e)
    {
        int coderror = 0;
        string msgerror = "";
        DataSet dt = new DataSet();
        dt = _SQL.cargasolicitudesfiltradas(1, Session["rut"].ToString(), 0, txtAprobacion.Text, txtshipcode.Text, txtfecha.Text, out coderror, out msgerror);
        if (dt.Tables.Count != 0)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                //wb.Worksheets.Add(dt, "Cotizaciones");
                wb.Worksheets.Add(dt);
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=Reclamos.xlsx");
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
    protected void Buscar_Click(object sender, ImageClickEventArgs e)
    {
        try
        {
            int coderror = 0;
            string msgerror = "";

            //grSolicitudesDetalleDevolucion.DataSource = _SQL.cargasolicitudes(13, txtAprobacion.Text, 0, out coderror, out msgerror);
            grSolicitudesDetalleDevolucion.DataSource = _SQL.cargasolicitudesfiltradas(1, Session["rut"].ToString(), 0, txtAprobacion.Text, txtshipcode.Text, txtfecha.Text, out coderror, out msgerror);

            if (coderror != 0)
            {
                msjesError.InnerText = "Se a generado un error favor contactar a administrador.";
                msjesError.Visible = true;
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_Obtiene_SolicitudesDevBuscar] Message: " + msgerror + " Inner: " + coderror);
            }
            else
            {
                grSolicitudesDetalleDevolucion.DataBind();
            }
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Se a generado un error favor contactar a administrador.";
            msjesError.Visible = true;
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_Obtiene_Solicitudesbuscar_CATCH] Message: " + ex.Message + " Inner: " + ex.InnerException);
        }
    }
    protected void btnRefrescar_Click(object sender, ImageClickEventArgs e)
    {
        txtAprobacion.Text = "";
        cargasolicitudesdevolucion();
    }

}

