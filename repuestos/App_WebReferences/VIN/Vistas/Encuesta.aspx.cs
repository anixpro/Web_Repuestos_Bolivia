using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using log4net;
using log4net.Config;

public partial class Vistas_encuesta : System.Web.UI.Page
{
    ControlBD _controlBD;

    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_encuesta));

    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;
        _controlBD = new ControlBD();

        ObtenerEncuestas();
        btnResponder.Click += BtnVotar_Click;

        if (Session["permisos"] != null)
        {
            if (int.Parse(Session["permisos"].ToString()) != 1)
            {
                btnAgregar.Visible = false;
            }
        }
        else
        {
            msjesError.Visible = true;
            msjesError.InnerText = "La sesión caducó";
            btnAgregar.Visible = false;
            btnResponder.Visible = false;
            btnVerResult.Visible = false;
        }
    }


    public void ObtenerEncuestas()
    {
        try { 
        DataSet _dSet = _controlBD.ObtenerDatos("encuesta");

        foreach (DataRow campos in _dSet.Tables[0].Rows)
        {
            lblIdEnc.Text = campos["idEncuesta"].ToString();
            lblPregunta.Text = campos["pregunta"].ToString();
            RbtnRespuesta1.Text = campos["texRes1"].ToString();
            RbtnRespuesta2.Text = campos["texRes2"].ToString();
            RbtnRespuesta3.Text = campos["texRes3"].ToString();
            RbtnRespuesta4.Text = campos["texRes4"].ToString();
            lblRes1.Text = campos["votosR1"].ToString();
            lblRes2.Text = campos["votosR2"].ToString();
            lblRes3.Text = campos["votosR3"].ToString();
            lblRes4.Text = campos["votosR4"].ToString();
        }

            if (RbtnRespuesta1.Text == "")
            { RbtnRespuesta1.Visible = false; }
            if (RbtnRespuesta2.Text == "")
            { RbtnRespuesta2.Visible = false; }
            if (RbtnRespuesta3.Text == "")
            { RbtnRespuesta3.Visible = false; }
            if (RbtnRespuesta4.Text == "")
            { RbtnRespuesta4.Visible = false; }

        }
        catch (NullReferenceException ex) { Debug.Write(ex); }
        
    }



    protected void btnResponder_Click(object sender, ImageClickEventArgs e)
    {
                
    }
    protected void btnVerResult_Click(object sender, ImageClickEventArgs e)
    {
        Response.Redirect("resultadosEncuesta.aspx");
    }
    protected void btnCreaNueva_Click(object sender, EventArgs e)
    {
        lblEncuesta.Visible = false;
        Response.Redirect("crearEncuesta.aspx");
    }
    public void BtnVotar_Click(object o, EventArgs e)
    {
        //Valido si el usuario ha votado en la encuesta
        string alter = "";
        int resp = 0;
        string user = Session["rut"].ToString();

        if (_controlBD.ValidarVotoEncuesta(lblIdEnc.Text, user))
        {
            MessageBox.Show("Usted ya votó");
            return;
        }


         if(RbtnRespuesta1.Checked)
         {
             alter = "votosR1";
             resp = int.Parse(lblRes1.Text) + 1 ;
         }
        if(RbtnRespuesta2.Checked)
         {
             alter = "votosR2";
             resp = int.Parse(lblRes2.Text) + 1;
         }
        if(RbtnRespuesta3.Checked)
         {
             alter = "votosR3";
             resp = int.Parse(lblRes3.Text) + 1;
         }
        if(RbtnRespuesta4.Checked)
         {
             alter = "votosR4";
             resp = int.Parse(lblRes4.Text) + 1;
         }

        _controlBD.VotarEncuesta(lblIdEnc.Text,alter,resp,user);
        Response.Redirect("resultadosEncuesta.aspx");
    }
}