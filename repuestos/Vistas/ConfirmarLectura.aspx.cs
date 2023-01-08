using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using log4net.Config;

public partial class Vistas_ConfirmarLectura : System.Web.UI.Page
{
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_ConfirmarLectura));
    protected void Page_Load(object sender, EventArgs e)
    {
        String rut = Request.QueryString["rut"];
        idResultado.Text = Util.confirmaLectura(rut).ToString();
    }
}
