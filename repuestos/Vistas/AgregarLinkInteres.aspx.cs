using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using log4net.Config;

public partial class Vistas_AgregarLinkInteres : System.Web.UI.Page
{
    ControlBD _controlBD;
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_AgregarLinkInteres));

    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;
        _controlBD = new ControlBD();
        txtNomLinkInteres.Focus();
        lblError.Visible = false;
    }

    protected void btnAgregar_Click(object sender, EventArgs e)
    {
        if (txtLinkInteres.Text == "" || txtNomLinkInteres.Text == "")
        {
            lblError.Visible = true;
        }
        else
        {
            _controlBD.InsertarLinksDeInteres(txtLinkInteres.Text, txtNomLinkInteres.Text);
            Response.Redirect("LinkInteres.aspx");
        }
    }
}