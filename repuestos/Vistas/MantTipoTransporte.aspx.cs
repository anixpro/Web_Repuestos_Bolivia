using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using log4net;
using log4net.Config;
using System.Configuration;
using System.Data;

public partial class Vistas_MantTipoTransporte : System.Web.UI.Page
{
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_MantTipoTransporte));
    SendMail_helper _mail = new SendMail_helper();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            cargarGrilla();
        }
    }

    private void cargarGrilla()
    {
        try
        {
            RepuestosModelDataContext ctx = new RepuestosModelDataContext();
            int? nro_error = null;
            string msg_error = null;
            var listaResultado = (from i in ctx.webr_obtiene_tipo_transporte( 1,ref nro_error, ref msg_error)
                                   select i).ToList();

            if (listaResultado != null)
            {
                grTipos.DataSource = listaResultado;
                grTipos.DataBind();
            }
        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Se presento un error al ingresar] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
        }

    }

    protected void grTipos_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "elimina")
        {
            try
            {
                int indice = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = grTipos.Rows[indice];
                HiddenField idHd = (HiddenField)row.FindControl("hdId");

                if (idHd != null)
                {
                    int? nro_error = null;
                    string msg_error = null;
                    RepuestosModelDataContext ctx = new RepuestosModelDataContext();
                    var resultado = ctx.webr_elimina_tipo_transporte(int.Parse(idHd.Value), ref nro_error, ref msg_error);
                    lblAviso.Visible = false;
                    msjesError.Visible = false; 
                    cargarGrilla();
                }
            }
            catch (Exception ex)
            {
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Se presento un error al ingresar] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
            }        
        }
    }

    protected void chkHabilitado_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            GridViewRow row = ((GridViewRow)((CheckBox)sender).NamingContainer);
            int index = row.RowIndex;
            CheckBox cb1 = (CheckBox)grTipos.Rows[index].FindControl("chkHabilitado");
            HiddenField idHd = (HiddenField)row.FindControl("hdId");
            bool checkValue = cb1.Checked;

            int? nro_error = null;
            string msg_error = null;
            RepuestosModelDataContext ctx = new RepuestosModelDataContext();
            var resultado = ctx.webr_actualiza_tipo_transporte(checkValue, idHd.Value, ref nro_error, ref msg_error);
        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Se presento un error al ingresar] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
        }
    }

    protected void grTipos_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        int index = e.Row.RowIndex;
        HiddenField idHd = (HiddenField)e.Row.FindControl("hdId"); 
        if (idHd != null && int.Parse(idHd.Value) < 5){
            var lnkElimina = e.Row.Cells[2];
            lnkElimina.Visible = false;
        }
    }

    protected void nuevo_Click(object sender, EventArgs e)
    {
        try
        {
            string nuevoNombre = txtNuevoNombre.Text;
            if (nuevoNombre == "")
            {
                lblAviso.Visible = false;
                msjesError.InnerText = "Debe completar el campo nombre";
                msjesError.Visible = true;
                return;
            }
            else
            {
                int? nro_error = null;
                string msg_error = null;
                RepuestosModelDataContext ctx = new RepuestosModelDataContext();
                var resultado = ctx.webr_inserta_tipo_transporte(nuevoNombre, ref nro_error, ref msg_error);

                txtNuevoNombre.Text = "";
                msjesError.Visible = false;
                lblAviso.Visible = true;
                lblAviso.Text = "Datos Actualizados";
                cargarGrilla();
            }

        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Se presento un error al ingresar] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);

        }
    }
}