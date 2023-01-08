using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using log4net;
using log4net.Config;

public partial class mantenedorConcesionarioInsert : System.Web.UI.Page
{
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(mantenedorConcesionarioInsert));

    private ControlPersona controlPersona;
    private ControlAuto controlAuto;
    //private Persona persona; 

    private String[] _marcasSelecionadas;

    private int _iZona;
    private String _sector;

    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;

        controlPersona = new ControlPersona();
        controlAuto = new ControlAuto();
        
        if (!IsPostBack)
        {
            cargaddlAdministrador();
            cargaddlSucursal();
        }
             try
        {
            _iZona = ddlZona.SelectedIndex;
            _sector = ddlZona.SelectedItem.ToString();
        }
        catch (NullReferenceException)
        {
            _iZona = 0;
            _sector = "";
        }

        ddlZona.Items.Clear();

        foreach (String concesionario in controlAuto.obtenerZona())
        {
            ddlZona.Items.Add(concesionario);
        }

        ddlZona.SelectedIndex = _iZona;

        String[] marcas = controlAuto.marcas();

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
            _marcasSelecionadas = marcaARescatar.ToArray();
        }
        catch (NullReferenceException ex)
        {
            logger.Error("En [Page_Load] NullReferenceException. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
        }

        cbxlMarcas.Items.Clear();
        foreach (String marca in marcas)
        {
            cbxlMarcas.Items.Add(marca);
        }

    }

    public void cargaddlAdministrador()
    {
        ControlPersona contper = new ControlPersona();
        ddlAdministrador.DataSource = contper.obtienePersonasPorPerfil(5);
        ddlAdministrador.DataValueField = "rut";
        ddlAdministrador.DataTextField = "nombre";
        ddlAdministrador.DataBind();
        ddlAdministrador.Items.Insert(0, new ListItem("Elija una Opcion..", "0")); 

    }

    public void cargaddlSucursal()
    {
        ControlPersona contper = new ControlPersona();
        ddlSupervisor.DataSource = contper.obtienePersonasPorPerfil(5);
        ddlSupervisor.DataValueField = "rut";
        ddlSupervisor.DataTextField = "nombre";
        ddlSupervisor.DataBind();
        ddlSupervisor.Items.Insert(0, new ListItem("Elija una Opcion..", "0"));

    }
    protected void btnAgregar_Click(object sender, EventArgs e)
    {
        SapAPI _sapApi = new SapAPI();
        /*
        if (_sapApi.ValidarDealerExiste(txtCodigoCliente.Text))
        {
            MessageBox.Show("ERROR, el código cliente SAP no esta disponible");
            return;
        }
        */
        lblConcesionario.ForeColor = System.Drawing.Color.FromName("#696969");

        String rutaImagen = "";
        if (fldImagen.HasFile)
        {
            try
            {
                fldImagen.SaveAs(Server.MapPath("~/doc/imgConcesionarios/") + txtNombre.Text.Replace(".", "") + ".jpg");
                rutaImagen = "../doc/imgConcesionarios/" + txtNombre.Text.Replace(".", "") + ".jpg";
            } catch(Exception ex)
            {
                logger.Error("En [btnAgregar_Click] Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
                rutaImagen="";
            }
        }
        try 
	    {

            if (controlAuto.insertaConcesionario(_marcasSelecionadas, ddlZona.SelectedItem.ToString(), txtNombre.Text, rutaImagen, txtRut.Text, txtCodigoCliente.Text, ddlSupervisor.SelectedValue.ToString(), ddlAdministrador.SelectedValue.ToString(), txtCorreoVFC.Text) > 0)
            {
                msjesError.Visible = true;
                msjesError.InnerText = "Concesionario " + txtNombre.Text + " insertado exitosamente";
            }
            else
            {
                msjesError.Visible = true;
                msjesError.InnerText = "El concesionario ya existe";
                txtNombre.Text = "";
                lblConcesionario.ForeColor = System.Drawing.Color.Red;
            }
	    }
        catch (Exception ex)
        {
            logger.Error("En [btnAgregar_Click] Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
        }
    }
}
