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
using ClosedXML.Excel;
using System.Linq;
using System.Drawing;
using System.IO;

public partial class Vistas_Devolucion_CDR_Reclamos : System.Web.UI.Page
{
    SQL_DevRec _SQL = new SQL_DevRec();
    SendMail_helper _mail = new SendMail_helper();
    ControlBDSolicitud _ControlBD = new ControlBDSolicitud();
    protected void Page_Load(object sender, EventArgs e)
    {
        //Button1.Style["visibility"] = "hidden";
        //Button4.Style["visibility"] = "hidden";
        if (!IsPostBack)
        {
            cargasolicitudesreclamos();
        }

    }

    private void cargasolicitudesreclamos()
    {
        try
        {
            int coderror = 0;
            string msgerror = "";

            grSolicitudesDetalleReclamos.DataSource = _SQL.cargasolicitudes(8, Session["rut"].ToString(), 0, out coderror, out msgerror);

            if (coderror != 0)
            {
                msjesError.InnerText = "Se a generado un error favor contactar a administrador.";
                msjesError.Visible = true;
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_Obtiene_SolicitudesRecl] Message: " + msgerror + " Inner: " + coderror);
            }
            else
            {
                grSolicitudesDetalleReclamos.DataBind();
            }
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Se a generado un error favor contactar a administrador.";
            msjesError.Visible = true;
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_Obtiene_SolicitudesRecl_CATCH] Message: " + ex.Message + " Inner: " + ex.InnerException);
        }

    }


    protected void rdAprobar_CheckedChanged(object sender, EventArgs e)
    {
        try
        {

            for (int i = 0; i < grSolicitudesDetalleReclamos.Rows.Count; i++)
            {
                GridViewRow row = grSolicitudesDetalleReclamos.Rows[i];
                string poscicion = row.Cells[1].Text;
                RadioButton rdAprobar = (RadioButton)grSolicitudesDetalleReclamos.Rows[i].FindControl("rdAprobar");
                DropDownList drMoRechazo = (DropDownList)grSolicitudesDetalleReclamos.Rows[i].FindControl("drMoRechazo");

                if (rdAprobar.Checked)
                {
                    drMoRechazo.Items.Clear();
                    drMoRechazo.Enabled = false;
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

    protected void rdRechazar_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            int coderror = 0;
            string msgerror = "";
            int flag = 0;

            DataSet motivo = new DataSet();
            motivo = _SQL.llenacombomotivo(6, out coderror, out msgerror, out flag);
            if (coderror != 0)
            {
                msjError.InnerText = "Error al cargar combo motivo, contancte al administrador.";
                msjError.Visible = true;
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_CargaCombo_rechazoComercial] Message: " + msgerror + " Inner: " + coderror);
            }
            else
            {
                for (int i = 0; i < grSolicitudesDetalleReclamos.Rows.Count; i++)
                {
                    GridViewRow row = grSolicitudesDetalleReclamos.Rows[i];
                    string poscicion = row.Cells[1].Text;
                    RadioButton rdRechazar = (RadioButton)grSolicitudesDetalleReclamos.Rows[i].FindControl("rdRechazar");
                    DropDownList drMoRechazo = (DropDownList)grSolicitudesDetalleReclamos.Rows[i].FindControl("drMoRechazo");


                    if (rdRechazar.Checked)
                    {
                        drMoRechazo.Enabled = true;
                        drMoRechazo.DataSource = motivo;
                        drMoRechazo.DataValueField = "id_motivo";
                        drMoRechazo.DataTextField = "desc_motivo";
                        drMoRechazo.DataBind();
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

    protected void btnGuardarReclamo_Click(object sender, EventArgs e)
    {
        try
        {
           
            int coderror = 0;
            string msgerror = "";
            DateTime fechasol = DateTime.Now;
            
            for (int i = 0; i < grSolicitudesDetalleReclamos.Rows.Count; i++)
            {
                GridViewRow row = grSolicitudesDetalleReclamos.Rows[i];
                int id_solicitud = Convert.ToInt32(row.Cells[1].Text);
                string nrofac = row.Cells[6].Text;
                string posfac = row.Cells[7].Text;
                string aprueba = "";
                int Mrechazo = 0;

                string marca = row.Cells[8].Text;
                string desrepuesto = row.Cells[10].Text;
                string codrepuesto = row.Cells[9].Text;

                CheckBox chkConfirmar = (CheckBox)grSolicitudesDetalleReclamos.Rows[i].FindControl("chkConfirmar");
                RadioButton rdAprobar = (RadioButton)grSolicitudesDetalleReclamos.Rows[i].FindControl("rdAprobar");
                RadioButton rdRechazar = (RadioButton)grSolicitudesDetalleReclamos.Rows[i].FindControl("rdRechazar");
                DropDownList drMoRechazo = (DropDownList)grSolicitudesDetalleReclamos.Rows[i].FindControl("drMoRechazo");
                TextBox txtComentario = (TextBox)grSolicitudesDetalleReclamos.Rows[i].FindControl("txtComentario");
                DropDownList drResponsable = (DropDownList)grSolicitudesDetalleReclamos.Rows[i].FindControl("drResponsable");

                if (chkConfirmar.Checked)
                {
                    if (rdAprobar.Checked)
                    {
                        aprueba = "SI";
                    }
                    else
                    {
                        if (rdRechazar.Checked)
                        {
                            aprueba = "NO";
                            Mrechazo = Convert.ToInt32(drMoRechazo.SelectedValue);
                        }
                    }
                    
                    if (aprueba != "")
                    {
                        _SQL.CambiaEstadosSolicitud(2, Session["rut"].ToString(), id_solicitud, nrofac, posfac, fechasol, aprueba, 0, Mrechazo, "", txtComentario.Text, drResponsable.SelectedItem.ToString(),"","", out coderror, out msgerror);

                        //Obtencion Correos            
                        String correoCCR = "";
                        String correoCCC = "";
                        String correo = "";
                        String correoS = "";
                        //Encargado Reclamos
                        DataSet dsr = new DataSet();
                        dsr = _SQL.cargacombos(3, "", out coderror, out msgerror);

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


                        String cuerpo = "";
                        String cuerpo_cabecera = "";

                        cuerpo_cabecera = "Se ha repondido su solicitud de devolución numero: " + Convert.ToString(id_solicitud) + "<br/><br/>";

                        cuerpo += "Aprobada            : " + aprueba + "<br/>";
                        cuerpo += "Marca               : " + marca + "<br/>";
                        cuerpo += "Número Solicitud    : " + Convert.ToString(id_solicitud) + "<br/>";
                        cuerpo += "Número Factura      : " + nrofac + "<br/>";
                        cuerpo += "Posición Factura    : " + posfac + "<br/>";
                        cuerpo += "Código Repuesto     : " + codrepuesto + "<br/>";
                        cuerpo += "Descripción Repuesto: " + desrepuesto + "<br/>";
                        if (aprueba == "SI")
                        {
                            //cuerpo += "Motivo              : " + motivo + "<br/>";
                        }
                        else
                        {
                            cuerpo += "Motivo Rechazo      : " + drMoRechazo.SelectedItem.ToString() + "<br/>";
                        }

                        cuerpo += "<br/><br/>";

                        cuerpo += "Se ha respondido su solicitud de devolución por: " + aprueba;
                        //_mail.EnviarCorreo(correo, "Resolución Devolución: " + aprueba, cuerpo, correoCCR + "," + correoS);
                        //_mail.EnviarCorreoHTML(correo, "Resolución Solicitud de Reclamo ", cuerpo_cabecera + cuerpo , correoCCR + "," + correoS);
                       

                        if (aprueba == "SI")
                        {
                            if (correoS == "")
                            {
                                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Envio correo Solicitud Reclamo CDR] Message: " + "La solicitud " + Convert.ToSByte(id_solicitud) + ", No contiene correo destinatario");
                            }
                            else
                            {
                                _mail.EnviarCorreoHTML(correoS, "Resolución Solicitud de Reclamo ", cuerpo_cabecera + cuerpo, correoCCR);
                            }                            
                        }
                        else
                        {
                            if (correo == "")
                            {
                                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Envio correo Solicitud Reclamo CDR] Message: " + "La solicitud " + Convert.ToSByte(id_solicitud) + ", No contiene correo destinatario");
                            }
                            else
                            {
                                _mail.EnviarCorreoHTML(correo, "Resolución Solicitud de Reclamo ", cuerpo_cabecera + cuerpo, correoCCR + "," + correoS);
                            }                            
                        }
                    }                    

                }


            }


            String query2 = "SELECT email FROM persona WHERE rut = '" + Session["rut"].ToString() + "'";

            //this.ModalPopupExtender1.Hide();
            cargasolicitudesreclamos();
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Se a generado un error favor contactar a administrador.";
            msjesError.Visible = true;
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_btnGuardar_CATCH] Message: " + ex.Message + " Inner: " + ex.InnerException);
        }

    }


    protected void rdAnalista_CheckedChanged1(object sender, EventArgs e)
    {
        try
        {
            int coderror = 0;
            string msgerror = "";
            int flag = 0;

            for (int i = 0; i < grSolicitudesDetalleReclamos.Rows.Count; i++)
            {
                GridViewRow row = grSolicitudesDetalleReclamos.Rows[i];
                string Solicitud = row.Cells[1].Text;
                string nrofac = row.Cells[6].Text;
                string posfac = row.Cells[7].Text;
                CheckBox rdAnalista = (CheckBox)grSolicitudesDetalleReclamos.Rows[i].FindControl("rdAnalista");
                if (rdAnalista.Checked)
                {
                    //Ejecutar SQL
                    _SQL.EstadosCDRreclamos(1, Session["rut"].ToString(), Convert.ToInt32(Solicitud), nrofac, posfac, out coderror, out msgerror);
                }

            }

            cargasolicitudesreclamos();
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Se ha generado un error favor contacte a administrador.";
            msjesError.Visible = true;
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_chkchange_Analista] Message: " + ex.Message + " Inner: " + ex.InnerException);

        }
    }

    protected void btnExportar_Click(object sender, EventArgs e)
    {
        int coderror = 0;
        string msgerror = "";
        DataSet dt = new DataSet();
        dt = _SQL.cargasolicitudes(15, Session["rut"].ToString(), 0, out coderror, out msgerror);
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
}

