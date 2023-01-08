using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using log4net.Config;

public partial class Vistas_VerMas : System.Web.UI.Page
{
    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_VerMas));

    string _idContenido;
    ControlBD _controlBD;
    string tipo;

    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;
        tipo = "";
        _controlBD = new ControlBD();
       _idContenido = Request.QueryString["valor"];
       btnEditar.Visible = false;
       btnEliminar.Visible = false;
       DataSet _dSet = _controlBD.MostrarContenido(_idContenido);
       
        // Se configura boton volver
       btnVolver.Click += new EventHandler(btnVolver_Click);

       if (Request.QueryString["tipo"] != null && Request.QueryString["tipo"] != "")
       {
           tipo = Request.QueryString["tipo"];
       }

       foreach (DataRow campos in _dSet.Tables[0].Rows)
       {
           
           this.Label1.Text = campos["html"].ToString();
       }
        //Si es administrador podrá editar el contenido
       int permiso = int.Parse(Session["permisos"].ToString());
       if (permiso == 1)
       {
           btnEditar.Visible = true;
           btnEliminar.Visible = true;
           _idContenido = _idContenido.Replace('\'', ' ');
           _idContenido = _idContenido.Trim();
           btnEditar.Click += new EventHandler(btnEditar_Click);
           btnEliminar.Click += new EventHandler(btnEliminar_Click);
       }
    }

    void btnVolver_Click(object sender, EventArgs e)
    {
        switch (tipo)
        {
            case "1":
                Response.Redirect("PaginaNoticia.aspx");
            break;
            case "3":
            case "4":
            case "5":
            case "9":
            case "11":
                Response.Redirect("CasaMatriz.aspx");
            break;
            case "6":
            case "7":
            case "8":
            case "10":
                Response.Redirect("Promociones.aspx");
            break;
            default:
                Response.Redirect("PaginaNoticia.aspx");
            break;
        }
    }

    void btnEditar_Click(object sender, EventArgs e)
    {
        Response.Redirect("EditorContenido.aspx?id=" + _idContenido);
    }

    void btnEliminar_Click(object sender, EventArgs e)
    {
        ControlContenido.eliminaContenido(_idContenido);
    }
}