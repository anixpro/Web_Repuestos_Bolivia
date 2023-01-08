using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using log4net.Config;

public partial class Vistas_PedidoSujerido : System.Web.UI.Page
{
    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_PedidoSujerido));

    SapAPI _sapApi;
    string nn = "";
    
    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;

        _sapApi = new SapAPI();

        nn = Request.QueryString["nn"];
        GridViewSugerido.DataSource = _sapApi.GetSugeridoPorUser(nn);
        GridViewSugerido.DataBind();
        
    }
}