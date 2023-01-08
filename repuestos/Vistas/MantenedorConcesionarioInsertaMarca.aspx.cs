using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using log4net;
using log4net.Config;

public partial class mantenedorConcesionarioInsertaMarca : System.Web.UI.Page
{
    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(mantenedorConcesionarioInsertaMarca));

    private ControlPersona controlPersona;
    private ControlAuto controlAuto;

    private String[] _marcasSeleccionadas;
    private String _marca;

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
        btnAgregar.Enabled = true;

        controlAuto = new ControlAuto();

        try
        {
            _iConcesionario = ddlConcesionario.SelectedIndex;
            _concesionario = ddlConcesionario.SelectedItem.ToString();
        }
        catch (Exception)
        {
            _iConcesionario = 0;
            _concesionario = "";
        }

        ddlConcesionario.Items.Clear();

        foreach (String dato in controlAuto.obtenerConcesionario())
        {
            ddlConcesionario.Items.Add(dato);
        }
        ddlConcesionario.SelectedIndex = _iConcesionario;


        String[] marcas = controlAuto.obtenerMarcasQueNoTengo(ddlConcesionario.SelectedItem.ToString());

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
        catch (NullReferenceException)
        {
            _marca = "";
        }

        cbxlMarcas.Items.Clear();
        foreach (String marca in marcas)
        {
            cbxlMarcas.Items.Add(marca);
        }

        if (cbxlMarcas.Items.Count == 0)
        {
            btnAgregar.Enabled = false;
        }

        String[] misMarcas = controlAuto.marcasPorConcesionario(ddlConcesionario.SelectedItem.ToString());
        cbxlMisMarcas.Items.Clear();
        int indiceMarca = 0;
        foreach (String marca in misMarcas)
        {
            cbxlMisMarcas.Items.Add(marca);
            cbxlMisMarcas.Items[indiceMarca].Selected = true;
            indiceMarca++;
        }
           
    }

    protected void btnAgregar_Click(object sender, EventArgs e)
    {
        if (_marcasSeleccionadas.Length < 1)
        {
            lblAviso.Text = "Debe escoger marcas";
            return;
        }
        if (controlAuto.insertaNuevasMarcasConcesionario(_marcasSeleccionadas, ddlConcesionario.SelectedItem.ToString()) > 0)
        {
            lblAviso.Text = "Marcas agregadas exitosamente";
        }
        else
        {
            lblAviso.Text = "Error al insertar marcas";
            lblConcesionario.ForeColor = System.Drawing.Color.Red;
        }
        String[] marcas = controlAuto.obtenerMarcasQueNoTengo(ddlConcesionario.SelectedItem.ToString());
        cbxlMarcas.Items.Clear();
        foreach (String marca in marcas)
        {
            cbxlMarcas.Items.Add(marca);
        }
    }
}