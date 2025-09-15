using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Services;
using System.Web.UI.HtmlControls;
using System.Web.Services;
using System.Configuration; 


public partial class MasterPage : System.Web.UI.MasterPage
{
    public string _urlEncuesta;
    ControlBD _controlBD;

    protected void Page_Init(object sender, EventArgs e)
    {
        // Se utiliza el siguiente truco para evitar el caché

        HttpContext.Current.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
        HttpContext.Current.Response.Cache.SetValidUntilExpires(false);
        HttpContext.Current.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
        HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        HttpContext.Current.Response.Cache.SetNoStore();

        if (Session["nombre"] == null)
        {
            Response.Redirect("../index.aspx?evento=ev1");
        }
    }
    //{
    //    // Se utiliza el siguiente truco para evitar el caché
        
    //    HttpContext.Current.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
    //    HttpContext.Current.Response.Cache.SetValidUntilExpires(false);
    //    HttpContext.Current.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
    //    HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
    //    HttpContext.Current.Response.Cache.SetNoStore();

    //    if (Session["nombre"] == null)
    //    {
    //        Response.Redirect("../index.aspx?evento=ev1");
    //    }
    //    else
    //    {
    //        System.Web.UI.Page formulario = (System.Web.UI.Page)HttpContext.Current.Handler;

    //        //Creamos script que redireccione desde el padre a otra ventana, (cierra repuestos.aspx).
    //        string script = "<script type=\"text/javascript\">";
    //        script += " redirecciona('" + ConfigurationManager.AppSettings["urlprincipal"].ToString() + "');";
    //        script += "</script>";

    //        //Registramos el Script.
    //        ScriptManager.RegisterStartupScript(formulario.Page, typeof(string), "redirecc", script, false);
    //    }
    //}

    protected void Page_Load(object sender, EventArgs e)
    {
        error.Visible = false;
        _urlEncuesta = "../vistas/encuesta.aspx";
        _controlBD = new ControlBD();
        idNoticiaImportante.Visible = false;

        ConfAmbiente.ConfCredenciales();
        lblversion.Text = "Versión : " + ConfAmbiente.version;
        



        if (Session["nombre"] != null)
        {
            hlkUsuario.Text = "Cerrar sesión: " + Session["nombre"].ToString();
            String nombreConcesionario = Util.obtieneNombreConcesionarioPorRUTCualquierUsuario(Session["rut"].ToString());
            if (nombreConcesionario != null)
            {
                lblConcesionario.Text = nombreConcesionario;
            }
        }
        try
        {
            int permiso = int.Parse(Session["permisos"].ToString());
            // Si usuario es administrador, no se muestra noticia importante
            //if (permiso != 1 || permiso != 3)
            //{
            //    ImageButton4.Visible = false;
            //    // Se comprueba si hay noticias importantes (por lo menos 1)
            //    if (Util.obtenerUltimaNoticiaImportante() != "")
            //    {
            //        // Se comprueba si es que el usuario leyó noticias urgentes
            //        if (Util.noLeyoNoticiaImportante(Session["rut"].ToString()))
            //        {
            //            idNoticiaImportante.Visible = true;
            //            idNoticiaImportante.InnerHtml = Util.obtenerUltimaNoticiaImportante() +
            //                @"
            //            <div id='divLeiNoticia' class='leiNoticia'>
            //                <center>
            //                    <input id='btnLeiNoticia' class='button' type='button' value='Leí la noticia' />
            //                    <input type='hidden' id='rutSession' value='" + Session["rut"].ToString() + @"'/>
            //                </center>
            //            </div>
            //        ";
            //        }
            //    }
            //}

            switch (permiso)
            {
                case 1:
                    lblTipoUsuario.Text = "Administrador";
                    break;
                case 2:
                    lblTipoUsuario.Text = "Gerente";
                    break;
                case 3:
                    lblTipoUsuario.Text = "Operario";
                    break;
                case 4:
                    lblTipoUsuario.Text = "Cotizador";
                    break;
                case 5:
                    lblTipoUsuario.Text = "Supervisor";
                    break;
                case 6:
                    lblTipoUsuario.Text = "Administrador Noticias";
                    break;
                default:
                    break;
            }
        }
        catch (Exception ex)
        {
            error.Visible = true;
            error.InnerHtml = "<p><center><b>Error. Contacte al administrador. El detalle técnico del error es: " + ex.Message + "</b></center></p>";
        }
        //repInfoGeneral.DataSource = _controlBD.ObtenerDatos("infoGeneral");
        //repInfoGeneral.DataBind();
    }


    //Limitar a uno el contenido del repeater de informacion general
    protected void limitar(object sender, RepeaterItemEventArgs e)
    {
        if ((sender as Repeater).Items.Count > 1)
        {
            (sender as Repeater).Controls.RemoveAt(1);
        }
    }
}
