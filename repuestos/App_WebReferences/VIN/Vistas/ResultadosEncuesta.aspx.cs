using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using log4net;
using log4net.Config;

public partial class Vistas_resultadosEncuesta : System.Web.UI.Page
{
    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_resultadosEncuesta));

    ControlBD _controlBD;
    public string pregunta1 = "Pregunta 1";
    public string pregunta2 = "Pregunta 2";
    public string pregunta3 = "Pregunta 3";
    public string pregunta4 = "Pregunta 4";
    public string respuesta1 = "0";
    public string respuesta2 = "0";
    public string respuesta3 = "0";
    public string respuesta4 = "0";
    public string pregunta = "Pregunta";

    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;
		btnVolver.Click += BtnVolver_Click;
        try {
            _controlBD = new ControlBD();
            DataSet resultadoEncuesta = _controlBD.ObtenerDatos("encuesta");
            respuesta1 = resultadoEncuesta.Tables[0].Rows[0]["votosR1"].ToString();
            respuesta2 = resultadoEncuesta.Tables[0].Rows[0]["votosR2"].ToString();
            respuesta3 = resultadoEncuesta.Tables[0].Rows[0]["votosR3"].ToString();
            respuesta4 = resultadoEncuesta.Tables[0].Rows[0]["votosR4"].ToString();
            pregunta1 = resultadoEncuesta.Tables[0].Rows[0]["texRes1"].ToString();
            pregunta2 = resultadoEncuesta.Tables[0].Rows[0]["texRes2"].ToString();
            pregunta3 = resultadoEncuesta.Tables[0].Rows[0]["texRes3"].ToString();
            pregunta4 = resultadoEncuesta.Tables[0].Rows[0]["texRes4"].ToString();
            pregunta = resultadoEncuesta.Tables[0].Rows[0]["pregunta"].ToString();
        
        }
        catch (IndexOutOfRangeException ex)
        {
            error.InnerHtml = "<p>Error, no existe encuesta.\n Detalle del error: " + ex.Message + "</p>";
            error.Visible = true;
        }
        
    }

    protected void limitar(object sender, RepeaterItemEventArgs e)
    {
        if ((sender as Repeater).Items.Count > 0)
        {
            (sender as Repeater).Controls.RemoveAt(0);
        }
    }
	
	 public void BtnVolver_Click(object o, EventArgs e)
    {
        Response.Redirect("encuesta.aspx");
    }

    
}