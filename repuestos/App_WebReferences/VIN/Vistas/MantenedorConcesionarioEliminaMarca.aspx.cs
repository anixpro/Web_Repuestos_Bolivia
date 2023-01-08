using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
// Namespaces del log
using log4net;
using log4net.Config;

public partial class MantenedorConcesionarioEliminaMarca : System.Web.UI.Page
{
    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(MantenedorConcesionarioEliminaMarca));

    private ControlAuto controlAuto;

    private String[] _marcasSeleccionadas;
    protected String _concesionario;

    private int _iConcesionario;
    protected void Page_Init(object sender, EventArgs e)
    {

        if (int.Parse(Session["permisos"].ToString()) > 2)
        {
            Response.Redirect("../index.aspx?evento=ev2");
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;
        btnAgregar.OnClientClick = "javascript:return validaCheck();";
        
        controlAuto = new ControlAuto();

        try
        {
            _iConcesionario = ddlConcesionario.SelectedIndex;
            _concesionario = ddlConcesionario.SelectedItem.ToString();
        }
        catch (Exception ex)
        {
            _iConcesionario = 0;
            _concesionario = "";
            logger.Error("En Page_Load. Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
        }

        ddlConcesionario.Items.Clear();

        foreach (String dato in controlAuto.obtenerConcesionario())
        {
            ddlConcesionario.Items.Add(dato);
        }
        ddlConcesionario.SelectedIndex = _iConcesionario;


        String[] marcas = controlAuto.marcasPorConcesionario(ddlConcesionario.SelectedItem.ToString());

        try
        {
            List<String> marcaARescatar = new List<String>();
            for (int i = 0; i < cbxlMarcas.Items.Count; i++)
            {

                if (cbxlMarcas.Items[i].Selected)
                {
                    //_marca += cbxlMarcas.Items[i].Text + ",";
                    marcaARescatar.Add(cbxlMarcas.Items[i].Text);
                }

            }
            _marcasSeleccionadas = marcaARescatar.ToArray();
        }
        catch (NullReferenceException ex)
        {
            logger.Error("En Page_Load. Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
        }

        cbxlMarcas.Items.Clear();
        foreach (String marca in marcas)
        {
            cbxlMarcas.Items.Add(marca);
        }

    }

    protected void btnAgregar_Click(object sender, EventArgs e)
    {
        if (controlAuto.eliminaMarcasConcesionario(_marcasSeleccionadas, ddlConcesionario.SelectedItem.ToString()) > 0)
        {
            lblAviso.Text = "Marcas eliminadas exitosamente";
        }
        else
        {
            lblAviso.Text = "Error al eliminar marcas";
            lblConcesionario.ForeColor = System.Drawing.Color.Red;
        }
        String[] marcas = controlAuto.marcasPorConcesionario(ddlConcesionario.SelectedItem.ToString());
        cbxlMarcas.Items.Clear();
        foreach (String marca in marcas)
        {
            cbxlMarcas.Items.Add(marca);
        }
    }
}
