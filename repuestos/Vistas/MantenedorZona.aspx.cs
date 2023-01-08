using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using log4net.Config;

public partial class mantenedorZona : System.Web.UI.Page
{
    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(mantenedorZona));

    private ControlAuto controlAuto;

    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;
        controlAuto = new ControlAuto();
    }

    protected void btnAgregar_Click(object senders,EventArgs e)
    {
        if (controlAuto.insertaZona(txtZona.Text) > 0)
        {
            lblAviso.Text = "Nueva zona agregada al sistema";
        }
        else
        {
            lblAviso.Text = "Error al agregar zona";
        }
    }
}