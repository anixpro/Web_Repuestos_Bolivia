using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using log4net.Config;

public partial class Vistas_Contacto : System.Web.UI.Page
{
    SendMail_helper _mail = new SendMail_helper();
    ControlBD _controlBD = new ControlBD();
    SapAPI _sapApi = new SapAPI();

    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_Contacto));

    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;
        string error = Request.QueryString["error"];
        if (error == "errorVfc")
        {
            MessageBox.Show("Se ha producido un error al crear la solicitud VFC, contacte al administrador");
        }
    }
    protected void btnEnviar_Click(object sender, EventArgs e)
    {
        //Obtener nombre de usuario y mail de usuario
        string correoUser = _sapApi.GetCorreoUsuario(Session["rut"].ToString());
        string nombreUsuario = _sapApi.GetNombreUsuario(Session["rut"].ToString());
        string comentario = txtMensaje.Text;

        string para = "jalcoholado@skberge.cl";
        string asunto = "Comentario Web Repuestos";
        string mensaje = nombreUsuario + " (" + correoUser + ") " + "a enviado el siguiente comentario: \n" + comentario;

        try
        {
            _mail.EnviarCorreo(para, asunto, mensaje);
            MessageBox.Show("Mensaje enviado con exito.");
            txtMensaje.Text = "";
        }
        catch (Exception)
        {
            MessageBox.Show("Hubo un problema al enviar el correo electrónico. Inténtelo más tarde");
        }
    }
    protected void btnLimpiar_Click(object sender, EventArgs e)
    {
        txtMensaje.Text = "";
    }
}