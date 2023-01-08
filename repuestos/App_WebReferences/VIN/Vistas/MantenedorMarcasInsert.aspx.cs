using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using log4net;
using log4net.Config;

public partial class mantenedorMarcasInsert : System.Web.UI.Page
{
    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(mantenedorMarcasInsert));

    private ControlPersona controlPersona;
    private ControlAuto controlAuto;

    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;
        controlPersona = new ControlPersona();
        controlAuto = new ControlAuto();
    }
    protected void btnAgregar_Click(object sender, EventArgs e)
    {
        if (controlAuto.insertaMarca(
                txtNombreMarca.Text.ToUpper(),
                txtAbreviado.Text.ToUpper(),
                txtDescripcion.Text.ToUpper(),
                txtOrgVenta.Text.ToUpper(),
                txtGrupoMaterial.Text.ToUpper(),
                txtPrefijo.Text.ToUpper(),
                txtCodigoCompania.Text.ToUpper(),
                chkEsForaneo.Checked
            ) > 0)
        {
            //lblAviso.Text = "Marca " + txtNombreMarca.Text + " insertada exitosamente";
            msjesError.InnerText = "Marca " + txtNombreMarca.Text + " insertada exitosamente";
            msjesError.Visible = true;

            txtNombreMarca.Text = "";
            txtAbreviado.Text = "";
            txtDescripcion.Text = "";
            txtOrgVenta.Text = "";
            txtCodigoCompania.Text = "";
            txtGrupoMaterial.Text = "";
            txtPrefijo.Text = "";
            chkEsForaneo.Checked = false;
        }
        else
        {
            msjesError.InnerText = "Marca existe";
            msjesError.Visible = true;
        }
    }
}