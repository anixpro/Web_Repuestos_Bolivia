using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;
using log4net;
using log4net.Config;

public partial class Vistas_editorBoletines : System.Web.UI.Page
{
    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_editorBoletines));

    ControlBD _controlBd;
    
    protected void Page_Load(object sender, EventArgs e)
    {
        _controlBd = new ControlBD();
        lblValidaDescrip.Visible = false;
        lblValidaTitulo.Visible = false;
        lblValidaDoc.Visible = false;
    }
    protected void AgregarBoletin_Click(object sender, EventArgs e)
    {
        if (txtTitulo.Text == "" || txtDescripcion.Text == "")
        {            
            lblValidaTitulo.Visible = true;
            lblValidaDescrip.Visible = true;
        }

        else
        {
            subirDoc.SaveAs(Server.MapPath("~/doc/boletines/") + subirDoc.FileName);
            string rutaDoc = "../doc/boletines/" + subirDoc.FileName.ToString();
            _controlBd.InsertarBoletin(txtTitulo.Text, txtDescripcion.Text, rutaDoc);
            Response.Redirect("Boletines.aspx");
        }
    }
}