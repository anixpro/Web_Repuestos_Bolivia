using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Diagnostics;
using System.IO;
using log4net;
using log4net.Config;

public partial class Vistas_Descartados : System.Web.UI.Page
{
    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_Descartados));

    protected void Page_Load(object sender, EventArgs e)
    {
            SqlDataSource1.SelectCommand = "select * from DESCARTADOS where id_pedido = '"+Session["rut"].ToString()+"'";
            GridViewDescartados.PagerSettings.Mode = PagerButtons.NumericFirstLast;
    }
}