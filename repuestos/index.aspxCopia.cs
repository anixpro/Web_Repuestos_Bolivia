using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using log4net.Config;

/// <summary>
///  Página incial del proyecto. Contiene autenticación
/// </summary>
public partial class index : System.Web.UI.Page
{
    // Permite hacer seguimiento de las personas
    private ControlPersona controlPersona;

    // Objeto persona
    Persona _persona;

    // Funcionalidades de BD
    ControlBD _controlBD;

    // Clase que permite crear identificadores unicos
    Guid _idSession;

    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(index));

    /// <summary>
    /// Se ejecuta cuando se carga la página.
    ///  - Valida el navegador al exigir IE > 6
    ///  - Muestra mensajes de error en caso que se haya provocado un problema de permisos
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Instanciación de objetos
        controlPersona = new ControlPersona();
        _persona = new Persona();
        _idSession = Guid.NewGuid();
        _controlBD = new ControlBD();

        // Informacion del navegador
        lblNav.Text = Request.Browser.Browser;
        lblVersion.Text = Request.Browser.Version;

        // Se valida el navegador
        if (Request.Browser.Browser == "IE")
        {
            try
            {
                if (Request.Browser.MajorVersion < 7)
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                errorNavegador.Visible = true;
                loginUser.Visible = false;
                divRecuperaPass.Visible = false;
                logger.Info("Alguien trató de usar un navegador IE de versión menor a 7");
            }
        }

        try
        {
            if (Request.QueryString["evento"] != null)
            {
                if (Request.QueryString["evento"].Equals("ev1"))
                {
                    lblError.Text = "Por favor ingrese su usuario y contraseña";
                }
                else if (Request.QueryString["evento"].Equals("ev2"))
                {
                    lblError.Text = "No tiene permisos para acceder a esa funcionalidad";
                }
                else if (Request.QueryString["evento"].Equals("logout"))
                {
                    Session.RemoveAll();
                    //Capturamos el contexto actual.
                    System.Web.UI.Page formulario = (System.Web.UI.Page)HttpContext.Current.Handler;

                    //Creamos script que redireccione desde el padre a otra ventana, (cierra repuestos.aspx).
                    string script = "<script type=\"text/javascript\">";
                    script += " parent.document.getElementById (\"IframePrincipal\"). style.display = \"none\";"; 
                    script += "window.parent.location.href=\"index.aspx\";";
                    script += "</script>";

                    //Registramos el Script.
                    ScriptManager.RegisterStartupScript(formulario.Page, typeof(string), "AbrirVentana", script, false);
                    

                }
                else if (Request.QueryString["evento"].Equals("recupera"))
                {
                    int rut = int.Parse(Request.QueryString["rut"]);
                    if (controlPersona.resetaContrasena(rut) > 0)
                    {
                        String[] datosPersona = controlPersona.buscaPersonaPorRut(rut);
                        String mensaje = "Estimado :" + datosPersona[0] + ". Su contraseña ha sido reestablecida:\n";
                        controlPersona.mail(datosPersona[0], datosPersona[1], datosPersona[2], datosPersona[3], mensaje);
                        Response.Write("<script language='javascript' type='text/javascript'>alert('Su contraseña ha sido reestablecida y enviada a su correo')</script>");
                    }
                }
            }
        }
        catch (NullReferenceException ex1)
        {
            logger.Warn("NullPointerException en Page Load. InnerException: " + ex1.InnerException + ". StackTrace: " + ex1.StackTrace);
        }
        catch (Exception ex)
        {
            logger.Warn("Exception en Page Load. InnerException: " + ex.InnerException + ". StackTrace: " + ex.StackTrace);
        }
    }

    protected void loginUser_Authenticate(object sender, AuthenticateEventArgs e)
    {
        bool IsValidLogin = false;

        if (this.ViewState["LoginAttempts"] == null)
            this.ViewState["LoginAttempts"] = 0;

        if (IsValidLogin == false)
        {
            this.ViewState["LoginAttempts"] = Convert.ToInt32(this.ViewState["LoginAttempts"]) + 1;
            ShowCaptcha();
        }
        else
        {
            this.ViewState["LoginAttempts"] = null;
        }
    }

    private void ShowCaptcha()
    {
        if (Convert.ToInt32(this.ViewState["LoginAttempts"]) > 3)
        {
            ccJoin.Visible = true;
            txtCap.Visible = true;
            lblMsjCaptcha.Visible = true;

            if (!ccJoin.UserValidated)
            {
                return;
            }
            else if (txtCap.Text != "")
            {
                ValidarUsuario();
            }
        }
        else
        {
            ValidarUsuario();
        }
    }

    public void ValidarUsuario()
    {
        try
        {
            switch (controlPersona.loguearse(loginUser.UserName, loginUser.Password))
            {
                case 0:
                    lblError.Text = "El servidor está ocupado. Favor intente en 1 minuto. Si el problema continúa notificar al administrador";
                    break;
                case 1:
                    Session["rut"] = controlPersona.rutPersona();
                     Session["url"] = "cambiaContrasena.aspx";
                     //Response.Redirect("Repuestos.aspx", false);
                    break;
                case 2:
                    Session.Timeout = 480;
                    Session["idSession"] = _idSession;
                    Session["nombre"] = controlPersona.nombrePersona();
                    Session["permisos"] = controlPersona.permisosPersona();
                    Session["rut"] = controlPersona.rutPersona();

                    switch (int.Parse(Session["permisos"].ToString()))
                    {
                        // Administrador
                        case 1:
                            Session["url"] = "vistas/CasaMatriz.aspx";
                            //Response.Redirect("Repuestos.aspx", false);
                            break;
                        // Gerente
                        case 2:
                            Session["url"] = "vistas/Indicadores2.aspx";
                            //Response.Redirect("Repuestos.aspx", false);
                            break;
                        // Operario
                        case 3:
                            Session["url"] = "vistas/buscarrepto2.aspx";
                            //Response.Redirect("Repuestos.aspx", false);
                            break;
                        // Cotizador
                        case 4:
                            Session["url"] = "vistas/buscarrepto2.aspx";
                            //Response.Redirect("Repuestos.aspx", false);
                           break;
                        // Supervisor
                        case 5:
                            Session["url"] = "vistas/CasaMatriz.aspx";
                            //Response.Redirect("Repuestos.aspx", false);
                            break;
                        // Administrador de Noticias
                        case 6:
                            Session["url"] = "vistas/CasaMatriz.aspx";
                            //Response.Redirect("Repuestos.aspx", false);
                            break;
                        default: break;
                    }
                    break;
                case 3:
                    Session.Timeout = 480;
                    Session["idSession"] = _idSession;
                    Session["nombre"] = controlPersona.nombrePersona();
                    Session["permisos"] = controlPersona.permisosPersona();
                    Session["rut"] = controlPersona.rutPersona();
                    Session["url"] = "doblePermiso.aspx";
                    //Response.Redirect("Repuestos.aspx", false);
                    break;
                default: break;
            }
        }
        catch (Exception ex)
        {
            lblError.Text = "El servidor está ocupado. Favor intente en 1 minuto. Si el problema continúa notificar al administrador";
            logger.Error("Exception en Page Load. InnerException: " + ex.InnerException + ". StackTrace: " + ex.StackTrace);
        }
    }
}
