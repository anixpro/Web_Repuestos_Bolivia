using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using log4net;
using log4net.Config;

public partial class mantenedorConcesionario : System.Web.UI.Page
{
    private ControlPersona controlPersona;
    private ControlAuto controlAuto;

    protected String _concesionario;
    private int _iConcesionario;

    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(mantenedorConcesionario));

    protected void Page_Init(object sender, EventArgs e)
    {

        if (int.Parse(Session["permisos"].ToString()) > 2)
        {
            Response.Redirect("../index.aspx?evento=ev2");
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        btnAgregar.OnClientClick = "javascript:return obtenerExtencion();";
        btnEliminar.Click += OnEliminarConcesionario;
        msjesError.Visible = false;
        controlPersona = new ControlPersona();
        controlAuto = new ControlAuto();
        //Lleno el dropDownList con los datos de concesionarios en la bd, asi queda automatizado por si insertan
        //un nuevo usuario
        if (int.Parse(Session["permisos"].ToString()) == 1)
        {
            lblMiConcesionario.Visible = false;
            ddlConcesionario.Visible = true;
            HtmlGenericControl body = Master.FindControl("bodyMaster") as HtmlGenericControl;
            body.Attributes.Add("onLoad", "cambiaImagen();");
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

            if (!IsPostBack)
            {
                ddlConcesionario.Items.Clear();

                foreach (String dato in controlAuto.obtenerConcesionario())
                {
                    ddlConcesionario.Items.Add(dato);
                }
                ddlConcesionario.SelectedIndex = _iConcesionario;
            }
        }
        else if (int.Parse(Session["permisos"].ToString()) == 2)
        {
            HtmlGenericControl body = Master.FindControl("bodyMaster") as HtmlGenericControl;
            body.Attributes.Add("onLoad", "imagenConcesionario();");
            lblMiConcesionario.Visible = true;
            ddlConcesionario.Visible = false;
            _concesionario = controlPersona.miConcesionario(Session["rut"].ToString());
            lblMiConcesionario.Text = _concesionario;
            hlkSucursal.Visible = false;
            hlkNuevoConcesionario.Visible = false;
            hlkEliminaMarcas.Visible = false;
            hlkInsertaMarcas.Visible = false;
            btnModificaDatos.Visible = false;
        }
        else
        {

        }
    }
    protected void btnAgregar_Click(object sender, EventArgs e)
    {
        if (fldImagen.HasFile)
        {
            try
            {
                fldImagen.SaveAs(Server.MapPath("~/doc/imgConcesionarios/") + _concesionario.Replace(".", "") + ".jpg");
                if (controlAuto.modificaConcesionario(_concesionario, "../doc/imgConcesionarios/" + _concesionario.Replace(".", "") + ".jpg") > 0)
                {
                    lblAviso.Text = "Imagen de concesionario modificada";
                }
                else
                {
                    lblAviso.Text = "No se pudo cambiar la imagen";
                }

            }
            catch (Exception ex)
            {
                lblAviso.Text = "Hubo problemas. Contacte al administrador. Detalle técnico: " + ex.Message;
            }
        }
        else
        {
            lblAviso.Text = "La Ruta no es correcta";
        }
    }
    protected void btnModificaDatos_Click(object sender, EventArgs e)
    {
        controlAuto = new ControlAuto();
        string idConcesionario = "0";

        foreach (Concesionario cn in controlAuto.obtenerConcesionariosIDs())
        {
            if (ddlConcesionario.SelectedValue == cn.Nombre)
            {
                idConcesionario = cn.ID;
            }
        }
        Response.Redirect("ConcesionarioEditar.aspx?id=" + idConcesionario);
    }

    public void OnEliminarConcesionario(object o, EventArgs e)
    {
        ControlBD _controlBD = new ControlBD();
        string concesionario = ddlConcesionario.SelectedValue;
        _controlBD.InsertarDatos("delete from concesionario where nombreConcesionario = '"+concesionario+"'");
        Response.Redirect("mantenedorConcesionario.aspx");

    }
}
