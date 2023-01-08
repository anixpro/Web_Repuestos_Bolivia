using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Vistas_AccesorioDetalleAjax : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        String idRepuesto = Request["id"].ToString();
        String marca = Request["marca"].ToString();
        Response.Write("-->" + idRepuesto + "-->" + marca);
    }
}