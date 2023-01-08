using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
using log4net;
using log4net.Config;

public partial class mantenedorModeloInsert : System.Web.UI.Page
{
    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(mantenedorModeloInsert));

    private ControlPersona controlPersona;
    private ControlAuto controlAuto;
    private int iMarca; //_iZona,
    private String _nombreMarca; //_marca,_sector,

    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;
        btnAgregar.OnClientClick = "javascript:return validaBlancos();";
        controlPersona = new ControlPersona();
        controlAuto = new ControlAuto();

        try
        {
            _nombreMarca = ddlMarca.SelectedItem.ToString();
            iMarca = ddlMarca.SelectedIndex;
        }
        catch (NullReferenceException ex)
        {
            _nombreMarca = "";
            iMarca = 0;
            Debug.Write(ex);
        }

        ddlMarca.Items.Clear();
        foreach (String marcas in controlAuto.marcas())
        {
            ddlMarca.Items.Add(marcas);
        }

        ddlMarca.SelectedIndex = iMarca;
                     
    }
    protected void btnAgregar_Click(object sender, EventArgs e)
    {
        lblNombreMarca.ForeColor = System.Drawing.Color.FromName("#696969");
        lblModelo.ForeColor = System.Drawing.Color.FromName("#696969");
        lblDescripcion.ForeColor = System.Drawing.Color.FromName("#696969");

        if (controlAuto.insertaModelo(ddlMarca.Text, txtModelo.Text, txtDescripcion.Text) > 0)
        {
            lblAviso.Text = "Modelo " + txtModelo.Text + " insertado exitosamente";
            ddlMarca.SelectedIndex = 0;
            txtModelo.Text = "";
            txtDescripcion.Text = "";
        }
        else
        {
            lblAviso.Text = "Error, puede que ya exista ese modelo para esa marca";
            txtModelo.Text = "";
            txtDescripcion.Text = "";
            lblNombreMarca.ForeColor = System.Drawing.Color.Red;
        }
    }
}