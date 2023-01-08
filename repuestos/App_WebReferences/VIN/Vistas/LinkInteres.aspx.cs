using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using log4net.Config;

public partial class Vistas_LinkInteres : System.Web.UI.Page
{
    ControlBD _contrlBD;
    
    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_LinkInteres));

    protected void Page_Load(object sender, EventArgs e)
    {
        lblError.Visible = false;
        msjesError.Visible = false;

        if (int.Parse(Session["permisos"].ToString()) != 1)
        {
            agregarLink.Visible = false;
        }

        _contrlBD = new ControlBD();
        CargarLinks();

        if (!IsPostBack)
        {
            panelAgregaLink.Visible = false;
        }
    }

    private void CargarLinks()
    {
        RepeaterLink.DataSource = _contrlBD.ObtenerDatos("linkInteres");
        RepeaterLink.DataBind();
    }

    protected void BtnAgregarLink_CLick(object sender, ImageClickEventArgs e)
    {
        
    }
    protected void RepeaterLink_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        string id = e.CommandArgument.ToString();
        string idDelete = "idLinks";
        string tabla = "linksInteres";

        _contrlBD.EliminarRegistro(tabla, id, idDelete);
        CargarLinks();
        msjesError.InnerText = "Link eliminado";
        msjesError.Visible = true;
    }
    
    protected void btnAgregarLink_Click(object sender, EventArgs e)
    {
        if (txtLinkInteres.Text == "" || txtNomLinkInteres.Text == "")
        {
            lblError.Visible = true;
        }
        else
        {
            _contrlBD.InsertarLinksDeInteres(txtLinkInteres.Text, txtNomLinkInteres.Text);
            panelAgregaLink.Visible = false;
            msjesError.InnerText = "Link agregado";
            msjesError.Visible = true;
            CargarLinks();
        }
    }
    protected void agregarLink_Click1(object sender, EventArgs e)
    {
        panelAgregaLink.Visible = true;
    }
}