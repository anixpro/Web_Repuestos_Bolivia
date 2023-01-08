using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using log4net.Config;

public partial class cambiaContrasena : System.Web.UI.Page
{
    private ControlPersona controlPersona;

    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(cambiaContrasena));

    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;
        btnAgregar.OnClientClick = "javascript:return validaBlancos();";
        controlPersona = new ControlPersona();
    }

    protected void btnAgregar_Click(object sender, EventArgs e)
    {
        String mail = "";
        String nombre = "";
        String mensaje = "";
        try
        {
            if (controlPersona.modificaContrasena((Session["rut"].ToString()), txtNuevaContrasena.Text) > 0)
            {
                // Preparación de correo
                mail = controlPersona.mailPersona(Session["rut"].ToString(), txtNuevaContrasena.Text);
                nombre = controlPersona.nombrePersona(Session["rut"].ToString(), txtNuevaContrasena.Text);
                mensaje = "Estimado " + nombre + ", su contrasena fue cambiada con exito\n";

                // envío de correo
                controlPersona.mail(nombre, Session["rut"].ToString(), txtNuevaContrasena.Text, mail, mensaje);

                // Se reenvia el navegado al login nuevamente para que la persona se vuelva a autenticar
                Response.Redirect("index.aspx");
            }
            else
            {
                msjesError.InnerText = "No se pudo cambiar su contraseña. Inténtelo nuevamente. Si el problema persiste avisar a la administración del sitio";
            }
        }
        catch (FormatException fe)
        {
            logger.Warn("Error format exception al modificar la contraseña del usuario. ¿Esta bien escrito el correo del usuario? Usuario: " + nombre + ". RUT: "+ Session["rut"].ToString() + ". Nueva contraseña: " + txtNuevaContrasena.Text + ". correo: " + mail + ". InnerException: " + fe.InnerException + ". Stack: " + fe.StackTrace);
            
            //por si el usuario no tiene correcto su mail solo seguimos el curso de la aplicacion no hacemos nada mas
            Response.Redirect("index.aspx");
        }
        catch (Exception ex)
        {
            logger.Error("Error sin identificar al modificar la contraseña del usuario. Usuario: " + nombre + ". RUT: " + Session["rut"].ToString() + ". Nueva contraseña: " + txtNuevaContrasena.Text + ". correo: " + mail + ". InnerException: " + ex.InnerException + ". Stack: " + ex.StackTrace);
            msjesError.InnerText = "Error al cambiar la contraseña. Avise al administrador";
            msjesError.Visible = true;
        }
    }
}