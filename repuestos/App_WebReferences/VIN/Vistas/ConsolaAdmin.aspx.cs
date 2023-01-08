using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using log4net.Config;

public partial class Vistas_ConsolaAdmin : System.Web.UI.Page
{
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_ConsolaAdmin));

    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;
    }
}