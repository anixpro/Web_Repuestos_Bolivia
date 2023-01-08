using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using log4net.Config;

public partial class Vistas_CasaMatriz : System.Web.UI.Page
{
    ControlBD _controlBD;

    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_CasaMatriz));

    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;

        // Instanciacion de objetos
        _controlBD = new ControlBD();

        try
        {
            RepeaterCazaMatriz.DataSource = _controlBD.ObtenerDatos("casaMatriz1");
            RepeaterCazaMatriz.DataBind();

            RepeaterCazaMatriz0.DataSource = _controlBD.ObtenerDatos("casaMatriz0");
            RepeaterCazaMatriz0.DataBind();

            RepeaterCasaMatriz2.DataSource = _controlBD.ObtenerDatos("casaMatriz2");
            RepeaterCasaMatriz2.DataBind();
        }
        catch(ArgumentException argEx)
        {
            msjesError.InnerText = "Error (de argumentos) al enlazar con la Base de datos. Detalle técnico: " + argEx.Message;
            msjesError.Visible = true;
            logger.Error("Error ArgumentException en Page Load InnerEx: " + argEx.InnerException + ". Stack: " + argEx.StackTrace);
        }
        catch (HttpException httpEx)
        {
            msjesError.InnerText = "Error (de http) al enlazar con la Base de datos. Detalle técnico: " + httpEx.Message;
            msjesError.Visible = true;
            logger.Error("Error HttpException en Page Load InnerEx: " + httpEx.InnerException + ". Stack: " + httpEx.StackTrace);
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Error (general) al enlazar con la Base de datos. Detalle técnico: " + ex.Message;
            msjesError.Visible = true;
            logger.Error("Error Exception en Page Load InnerEx: " + ex.InnerException + ". Stack: " + ex.StackTrace);
        }

        if (int.Parse(Session["permisos"].ToString()) != 1)
        {
            ImageButton1.Visible = false;
            ImageButton2.Visible = false;
            ImageButton4.Visible = false;
        }
    }

    protected void limitar(object sender, RepeaterItemEventArgs e)
    {
        if ((sender as Repeater).Items.Count > 0)
        {
            (sender as Repeater).Controls.RemoveAt(0);
        }
    }
}