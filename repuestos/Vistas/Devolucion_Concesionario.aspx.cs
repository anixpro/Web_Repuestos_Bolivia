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


public partial class Vistas_Devolucion_Concesionario : System.Web.UI.Page
{
    SQL_DevRec _SQL = new SQL_DevRec();
    SendMail_helper _mail = new SendMail_helper();

    protected void Page_Load(object sender, EventArgs e)
    {
        Button1.Style["visibility"] = "hidden";
        if (!IsPostBack)
        {
            cargasolicitudes();
        }
    }
    private void cargasolicitudes()
    {
        try
        {
            int coderror = 0;
            string msgerror = "";
            grSolicitudes.DataSource = null;
            grSolicitudes.DataBind();

            grSolicitudes.DataSource = _SQL.cargasolicitudes(1, Session["rut"].ToString(), 0, out coderror, out msgerror);

            if (coderror != 0)
            {
                msjesError.InnerText = "Se a generado un error favor contactar a administrador.";
                msjesError.Visible = true;
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_Obtiene_Solicitudes] Message: " + msgerror + " Inner: " + coderror);
            }
            else
            {
                grSolicitudes.DataBind();
            }
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Se a generado un error favor contactar a administrador.";
            msjesError.Visible = true;
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_Obtiene_Solicitudes_CATCH] Message: " + ex.Message + " Inner: " + ex.InnerException);
        }

    }


    protected void grSolicitudes_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grSolicitudes.PageIndex = e.NewPageIndex;
        cargasolicitudes();
    }


    protected void grSolicitudes_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            int coderror = 0;
            string msgerror = "";

            if (e.CommandName == "detalle")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = grSolicitudes.Rows[index];

                int id_solicitud = Convert.ToInt32(row.Cells[0].Text);

                grSolicitudesDetalle.DataSource = _SQL.cargasolicitudes(2, Session["rut"].ToString(), id_solicitud, out coderror, out msgerror);

                if (coderror != 0)
                {
                    msjesError.InnerText = "Se a generado un error favor contactar a administrador.";
                    msjesError.Visible = true;
                    _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_Obtiene_SolicitudesDET] Message: " + msgerror + " Inner: " + coderror);
                }
                else
                {
                    grSolicitudesDetalle.DataBind();
                    this.ModalPopupExtender1.Show();
                }
            }
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Se a generado un error favor contactar a administrador.";
            msjesError.Visible = true;
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_Obtiene_SolicitudesDET_CATCH] Message: " + ex.Message + " Inner: " + ex.InnerException);
        }

    }
    protected void rdAprobar_CheckedChanged(object sender, EventArgs e)
    {
        try
        {

            for (int i = 0; i < grSolicitudesDetalle.Rows.Count; i++)
            {
                GridViewRow row = grSolicitudesDetalle.Rows[i];
                string poscicion = row.Cells[1].Text;
                RadioButton rdAprobar = (RadioButton)grSolicitudesDetalle.Rows[i].FindControl("rdAprobar");
                TextBox txrNumeroNC = (TextBox)grSolicitudesDetalle.Rows[i].FindControl("txrNumeroNC");
                DropDownList drMoRechazo = (DropDownList)grSolicitudesDetalle.Rows[i].FindControl("drMoRechazo");
                CheckBox chAproComercial = (CheckBox)grSolicitudesDetalle.Rows[i].FindControl("chAproComercial");
                RequiredFieldValidator reqName = (RequiredFieldValidator)grSolicitudesDetalle.Rows[i].FindControl("reqName");

                if (rdAprobar.Checked)
                {
                    txrNumeroNC.Enabled = true;
                    chAproComercial.Enabled = true;
                    drMoRechazo.Items.Clear();
                    drMoRechazo.Enabled = false;
                    reqName.Enabled = true;
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
            motivo = _SQL.llenacombomotivo(4, out coderror, out msgerror, out flag);
            if (coderror != 0)
            {
                msjError.InnerText = "Error al cargar combo motivo, contancte al administrador.";
                msjError.Visible = true;
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_CargaCombo_rechazoComercial] Message: " + msgerror + " Inner: " + coderror);
            }
            else
            {
                for (int i = 0; i < grSolicitudesDetalle.Rows.Count; i++)
                {
                    GridViewRow row = grSolicitudesDetalle.Rows[i];
                    string poscicion = row.Cells[1].Text;
                    RadioButton rdRechazar = (RadioButton)grSolicitudesDetalle.Rows[i].FindControl("rdRechazar");
                    DropDownList drMoRechazo = (DropDownList)grSolicitudesDetalle.Rows[i].FindControl("drMoRechazo");
                    TextBox txrNumeroNC = (TextBox)grSolicitudesDetalle.Rows[i].FindControl("txrNumeroNC");
                    CheckBox chAproComercial = (CheckBox)grSolicitudesDetalle.Rows[i].FindControl("chAproComercial");
                    RequiredFieldValidator reqName = (RequiredFieldValidator)grSolicitudesDetalle.Rows[i].FindControl("reqName");

                    if (rdRechazar.Checked)
                    {
                        drMoRechazo.Enabled = true;
                        drMoRechazo.DataSource = motivo;
                        drMoRechazo.DataValueField = "id_motivo";
                        drMoRechazo.DataTextField = "desc_motivo";
                        drMoRechazo.DataBind();
                        txrNumeroNC.Enabled = false;
                        chAproComercial.Enabled = false;
                        reqName.Enabled = false;
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
    protected void btnCerrar_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < grSolicitudesDetalle.Rows.Count; i++)
        {
            GridViewRow row = grSolicitudesDetalle.Rows[i];
            RequiredFieldValidator reqName = (RequiredFieldValidator)grSolicitudesDetalle.Rows[i].FindControl("reqName");
            reqName.Enabled = false;
        }
        this.ModalPopupExtender1.Hide();
    }

    protected void btnGuardar_Click(object sender, EventArgs e)
    {
        try
        {
            bool ActivoCorreo = true;
            int coderror = 0;
            string msgerror = "";
            DateTime fechasol = DateTime.Now;
            String cuerpo = "";
            String cuerpo_cabecera = "";
            String cuerpo_pie = "";
            //Obtencion Correos            
            String correoCCR = "";
            String correoCCC = "";
            String correo = "";
            String correoS = "";

            for (int i = 0; i < grSolicitudesDetalle.Rows.Count; i++)
            {
                GridViewRow row = grSolicitudesDetalle.Rows[i];
                int id_solicitud = Convert.ToInt32(row.Cells[1].Text);
                string nrofac = row.Cells[2].Text;
                string posfac = row.Cells[3].Text;
                string repuesto = row.Cells[6].Text;

                string marca = row.Cells[10].Text;
                string desrepuesto = row.Cells[7].Text;
                string cantidadsol = row.Cells[8].Text;
                string neto = row.Cells[4].Text;
                string motivo = row.Cells[11].Text;


                string aprueba = "";
                int vbcomercial = 0;
                int Mrechazo = 0;
                string pedidoNC = "";

                CheckBox chkConfirmar = (CheckBox)grSolicitudesDetalle.Rows[i].FindControl("chkConfirmar");
                RadioButton rdAprobar = (RadioButton)grSolicitudesDetalle.Rows[i].FindControl("rdAprobar");
                RadioButton rdRechazar = (RadioButton)grSolicitudesDetalle.Rows[i].FindControl("rdRechazar");
                CheckBox chAproComercial = (CheckBox)grSolicitudesDetalle.Rows[i].FindControl("chAproComercial");
                DropDownList drMoRechazo = (DropDownList)grSolicitudesDetalle.Rows[i].FindControl("drMoRechazo");
                TextBox txrNumeroNC = (TextBox)grSolicitudesDetalle.Rows[i].FindControl("txrNumeroNC");
                RequiredFieldValidator reqName = (RequiredFieldValidator)grSolicitudesDetalle.Rows[i].FindControl("reqName");
                TextBox txtObservacionComercial = (TextBox)grSolicitudesDetalle.Rows[i].FindControl("txtObservacionComercial");

                reqName.Enabled = false;

                if (chkConfirmar.Checked)
                {
                    if (rdAprobar.Checked)
                    {
                        reqName.Enabled = true;
                        aprueba = "SI";
                        if (chAproComercial.Checked)
                        {
                            vbcomercial = 1;
                        }
                        pedidoNC = txrNumeroNC.Text;
                    }
                    else
                    {
                        if (rdRechazar.Checked)
                        {
                            aprueba = "NO";
                            Mrechazo = Convert.ToInt32(drMoRechazo.SelectedValue);
                        }
                    }
                    _SQL.CambiaEstadosSolicitud(1, Session["rut"].ToString(), id_solicitud, nrofac, posfac, fechasol, aprueba, vbcomercial, Mrechazo, pedidoNC, "", "","", txtObservacionComercial.Text, out coderror, out msgerror);


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

                    if (vbcomercial == 1)
                    {
                        ActivoCorreo = false;
                    }
                    else
                    {
                        ActivoCorreo = true;
                    }
                    

                    cuerpo_cabecera = "Se ha repondido su solicitud de devolución numero: " + Convert.ToString(id_solicitud) + "<br/><br/>";
                    
                    cuerpo += "Aprobada            : " + aprueba + "<br/>";
                    cuerpo += "Marca               : " + marca + "<br/>";
                    cuerpo += "Número Solicitud    : " + Convert.ToString(id_solicitud) + "<br/>";
                    cuerpo += "Número Factura      : " + nrofac + "<br/>";
                    cuerpo += "Posición Factura    : " + posfac + "<br/>";
                    cuerpo += "Código Repuesto     : " + repuesto + "<br/>";
                    cuerpo += "Descripción Repuesto: " + desrepuesto + "<br/>";
                    cuerpo += "Cantidad Solicitada : " + cantidadsol + "<br/>";
                    cuerpo += "Monto Neto          : $" + neto + "<br/>";
                    cuerpo += "Motivo              : " + motivo + "<br/>";
                    if (aprueba == "SI")
                    {
                        cuerpo += "Numero Aprobación   : " + pedidoNC + "<br/>";
                        cuerpo_pie = "<br/> Favor enviar el repuesto a centro de distribución, con la siguiente documentación: Guia de despacho - Correo de aprobación - Factura asociada <br/>";
                    }
                    else
                    {
                        cuerpo += "Motivo Rechazo      : " + drMoRechazo.SelectedItem.ToString() + "<br/>";
                    }
                    
                    cuerpo += "<br/><br/>";
                   
                }


                this.ModalPopupExtender1.Hide();


                if (ActivoCorreo)
                {
                    if (correo == "")
                    {
                        _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Envio correo Solicitud Comercial] Message: " + "La solicitud " + Convert.ToSByte(id_solicitud) + ", No contiene correo destinatario");
                    }
                    else
                    {
                        _mail.EnviarCorreoHTML(correo, "Resolución Solicitud de devolución ", cuerpo_cabecera + cuerpo + cuerpo_pie, correoCCR + "," + correoS);
                    }                    
                }   
            }
            //redirect para cargar de nuevo la grilla de solicitudes
            cargasolicitudes();
            //Response.Redirect("Devolucion_Concesionario.aspx", true);
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Se a generado un error favor contactar a administrador.";
            msjesError.Visible = true;
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [DEVREC_btnGuardar_CATCH] Message: " + ex.Message + " Inner: " + ex.InnerException);
        }

    }
}