using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ajax_detalleSolicitud : System.Web.UI.Page
{
    ControlVfc controlVFC = new ControlVfc();
    protected void Page_Load(object sender, EventArgs e)
    {
        string id = Request.QueryString["id"].ToString();

        gvDetalleSolicitud.Caption = "Detalle solicitud " + id;
        gvDetalleSolicitud.DataSource = controlVFC.getDetalleSolicitud(id);
        gvDetalleSolicitud.DataBind();
    }
}