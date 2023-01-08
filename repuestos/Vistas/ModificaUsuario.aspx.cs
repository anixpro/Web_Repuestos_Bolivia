using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using log4net;
using log4net.Config;

public partial class Vistas_modificaUsuario : System.Web.UI.Page
{
    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_modificaUsuario));

    private ControlPersona controlPersona;
    private ControlAuto controlAuto;
    private String _nombreConcesionario;
    private int _iConcesionario;
    private Persona _persona = new Persona();

    protected void Page_Init(object sender, EventArgs e)
    {
        if (int.Parse(Session["permisos"].ToString()) != 1)
        {
            Response.Redirect("../index.aspx?evento=ev2");
        }

        controlAuto = new ControlAuto();

        if (!IsPostBack)
        {
            ddlZona.Items.Add("Todas");
            foreach (String zona in controlAuto.obtenerZona())
            {
                ddlZona.Items.Add(zona);
            }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;

        controlPersona = new ControlPersona();
        
        String evento = "";
        int rut = 0;
        string cargo = "";

        btnBuscar.OnClientClick = "javascript:return validaBlancos();";
        try
        {
            evento=  Request.QueryString["ev"];
            cargo = Request.QueryString["cargo"];
            rut = int.Parse(Request.QueryString["rut"]);
        }
        catch (ArgumentNullException)
        {
            evento = "";
            rut = 0;
        }
        catch (FormatException)
        {
            evento = "";
            rut = 0;
        }

        try
        {
            _nombreConcesionario = ddlConcesionario.SelectedItem.ToString();
            _iConcesionario = ddlConcesionario.SelectedIndex;
        }

        catch (NullReferenceException)
        {
            _nombreConcesionario = "";
            _iConcesionario = 0;
        }

        if (!IsPostBack)
        {
            ddlConcesionario.Items.Clear();
            ddlConcesionario.Items.Add("Todos");
            foreach (String concesionario in controlAuto.obtenerConcesionario())
            {
                ddlConcesionario.Items.Add(concesionario);
            }

            ddlConcesionario.SelectedIndex = 0;
        }
        completaGrillaUsuarios();
    }

    protected void ddlConcesionario_SelectedIndexChanged(object sender, EventArgs e)
    {
        
    }

    private void completaGrillaUsuarios()
    {
        String nombreConcesionario = ddlConcesionario.SelectedValue;
        if (nombreConcesionario == "Todos")
        {
            sqldPersona.SelectCommand = controlPersona.datosPersona();
        }
        else
        {
            sqldPersona.SelectCommand = controlPersona.datosConcesionario(nombreConcesionario);
        }
        gridUsuarios.DataBind();
    }

    protected void ddlZona_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlZona.SelectedIndex == 0)
        {
            sqldPersona.SelectCommand = controlPersona.usuariosPorZona();
        }
        else
        {
            sqldPersona.SelectCommand = controlPersona.usuariosPorZona(ddlZona.SelectedItem.Text);
        }
    }

    protected void btnBuscar_Click(object sender, EventArgs e)
    {
        Response.Redirect("mantenedorUsuarioUpdate.aspx?rut=" + txtRut.Text + "");
    }
    protected void gridUsuarios_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "elimina")
        {
            int indice = Convert.ToInt32(e.CommandArgument);
            GridViewRow row = gridUsuarios.Rows[indice];
            string rut = row.Cells[1].Text;
            String cargo = row.Cells[6].Text;

            // Primero comprobar si usuario tiene doble rol
            // en el caso que lo tenga eliminar doble rol primero
            if (_persona.GetDoblePermiso(rut.ToString()))
            {
                switch (cargo)
                {
                    case "Operario":
                        if (_persona.DropPermiso(rut.ToString(), "3"))
                        {
                            msjesError.InnerText="El permiso " + cargo + " ha sido eliminado";
                            msjesError.Visible = true;
                        }
                        break;
                    case "Gerente":
                        if (_persona.DropPermiso(rut.ToString(), "2"))
                        {
                            msjesError.InnerText="El permiso " + cargo + " ha sido eliminado";
                            msjesError.Visible = true;
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (controlPersona.eliminaPersona(rut) > 0)
                {
                    msjesError.InnerText = "Usuario eliminado";
                    msjesError.Visible = true;
                    gridUsuarios.Rows[indice].Visible = false;
                }
            }
        }
    }
}