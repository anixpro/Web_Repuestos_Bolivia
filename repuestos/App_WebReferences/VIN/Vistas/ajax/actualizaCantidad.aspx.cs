using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using log4net;
using log4net.Config;



public partial class actualizaCantidad : System.Web.UI.Page
{
    private ControlRepuestos controlRepuestos;
	 
    protected void Page_Load(object sender, EventArgs e)
    {
		controlRepuestos = new ControlRepuestos();
		String id = Request["id"].ToString();
        String cant = Request["cant"].ToString();
		
		if (controlRepuestos.actualizaCantidadMin(cant,id)){
			Response.Write("true");
		}else 	Response.Write("false");
    }
}