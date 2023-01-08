using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class doblePermiso : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void btnAgregar_Click(object sender, EventArgs e)
    {
        if (rbtnGerente.Checked)
        {
            Session["permisos"] = 2;
            Response.Redirect("vistas/verIndicador.aspx");
        }
        else if (rbtnOperario.Checked)
        {
            Session["permisos"] = 3;
            Response.Redirect("vistas/buscarRepto.aspx");
        }
    }

}