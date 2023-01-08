using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Diagnostics;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.Security;
using System.Web.UI;
using log4net;
using log4net.Config;

public partial class Vistas_MantenedorCorreosVFC_Criticidad : System.Web.UI.Page
{
    SqlConnection con, con1;
    SqlCommand cmd, cmd1;
    ControlBD control = new ControlBD();
    SendMail_helper _mail =new SendMail_helper();
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_MantenedorCorreosVFC_Criticidad));

    protected void Page_Load(object sender, EventArgs e)
    {
        mjsError.Visible = false;
        mjsError.InnerText = "";
        mjsAlert.Visible = false;
        mjsAlert.InnerText = "";
        
        if(!IsPostBack)
        {
            LlenaLista();
            cargacombos();
        }  
    }

    public void LlenaLista()
    {
        gvEmails.DataSource = control.ObtenerListaEmail("SELECT * FROM Personas_Email_VFC");
        gvEmails.DataBind();
    }

    protected void cargacombos()
    {

        drMarcas.DataSource = control.ObtenerListaEmail("SELECT orgventas,nombremarca FROM marca UNION SELECT 'X','TODOS'"); ;
        drMarcas.DataValueField = "orgventas";
        drMarcas.DataTextField = "nombremarca";
        drMarcas.DataBind();    
    }

    public void EditaEmails(object sender, EventArgs e)
    {
        string query = "";
        int id = 0;
        for (int i = 0; i < gvEmails.Rows.Count; i++)
        {
            GridViewRow selecRow = gvEmails.Rows[i];
        }
    }

    protected void GridViewGMWeb_RowCommand(object sender, GridViewCommandEventArgs e)
    {

    }

    private void CorreoVFCUpdate(int id, string email, string analista)
    {
        int result = 0;
        string query = "";
        query = "UPDATE Personas_Email_VFC SET nomPersona_vfc = '" + analista + "', emaiilPersona_vfc = '" + email + "' WHERE idPersona_vfc = '" + id + "'";
        try
        {
            result = Convert.ToInt32(control.EjecutaQuery(query)); 
            if(result == 0)
            {
                mjsError.Visible = true;
                mjsError.InnerText = "No se pudo actualizar registro. Favor revisar datos y volver a intentar";
            }
            else
            {
                mjsAlert.Visible = true;
                mjsAlert.InnerText = "Registro Actualizado Correctamente.";
            }

            cargacombos();
        }
        catch(Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "Error al intentar Actualizar registro de correo VFC Criticidad Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
            //logger.Error("Error al intentar Actualizar registro de correo VFC Criticidad. Inner: "+ ex.InnerException + ". Stack: "+ex.StackTrace);
        }
    }

    protected void gvEmails_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gvEmails.EditIndex = e.NewEditIndex;
        LlenaLista();
    }

    protected void gvEmails_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {

        int indice = Convert.ToInt32(e.RowIndex);
        GridViewRow row = gvEmails.Rows[indice];

        TextBox txtEmailGrid = (TextBox)gvEmails.Rows[indice].FindControl("txtEmailGrid");
        TextBox txtAnaliGrid = (TextBox)gvEmails.Rows[indice].FindControl("txtAnalisGrid");
        Label idEmail = (Label) gvEmails.Rows[indice].FindControl("idEmail");
        string codigo = Convert.ToString(idEmail.Text);
        int dato = Convert.ToInt32(codigo);
        try
        {
            CorreoVFCUpdate(dato, txtEmailGrid.Text, txtAnaliGrid.Text);

            gvEmails.EditIndex = -1;
            LlenaLista();
        }
        catch (Exception ex)
        {
            logger.Error("Error al intentar Actualizar registro de correo VFC Criticidad. Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
        }
    }

    protected void gvEmails_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {

        int indice = Convert.ToInt32(e.RowIndex);
        GridViewRow row = gvEmails.Rows[indice];

        TextBox txtEmailGrid = (TextBox)gvEmails.Rows[indice].FindControl("txtEmailGrid");
        TextBox txtAnaliGrid = (TextBox)gvEmails.Rows[indice].FindControl("txtAnalisGrid");
        Label idEmail = (Label)gvEmails.Rows[indice].FindControl("idEmail");
        string codigo = Convert.ToString(idEmail.Text);
        int dato = Convert.ToInt32(codigo);
        try
        {
            string query = "";
            query = "DELETE Personas_Email_VFC WHERE idPersona_vfc = " + idEmail.Text;
            control.EjecutaQuery(query);
            gvEmails.EditIndex = -1;
            LlenaLista();
        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "Error al intentar Actualizar registro de correo VFC Criticidad Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
            //logger.Error("Error al intentar Actualizar registro de correo VFC Criticidad. Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
        }
    }

    protected void gvEmails_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gvEmails.EditIndex = -1;
        LlenaLista();
    }

    protected void gvEmails_PageIndexChanged(object sender, GridViewPageEventArgs e)
    {
        gvEmails.PageIndex = e.NewPageIndex;
        gvEmails.DataSource = control.ObtenerListaEmail("SELECT * FROM Personas_Email_VFC");
        gvEmails.DataBind();
    }

    protected void btnAgregar_Click(object sender, EventArgs e)
    {
        string query = "";
        query = "INSERT INTO Personas_Email_VFC(nomPersona_vfc,emaiilPersona_vfc,orgPersona_vfc,habilitado)VALUES('"+ txtnombre.Text +"','"+ txtcorreo.Text +"','" + drMarcas.SelectedValue + "',1)";
        control.EjecutaQuery(query);

        mjsAlert.InnerText = "Persona Insertada correctamente.";
        mjsAlert.Visible = true;

        txtcorreo.Text = "";
        txtnombre.Text = "";
        drMarcas.ClearSelection();
    

        LlenaLista();
        cargacombos();


    }
   

}