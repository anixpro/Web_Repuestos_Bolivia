using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using log4net.Config;

public partial class Vistas_Promociones : System.Web.UI.Page
{
    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_Promociones));

    ControlBD _controlBD;

    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;

        _controlBD = new ControlBD();

        rptrPromos.DataSource = obtenerPromociones();
        rptrPromos.DataBind();

        RepeaterPromociones2.DataSource = _controlBD.ObtenerDatos("promo2");
        RepeaterPromociones2.DataBind();

        RepeaterPromociones3.DataSource = _controlBD.ObtenerDatos("promo3");
        RepeaterPromociones3.DataBind();

        if (int.Parse(Session["permisos"].ToString()) != 1)
        {
            ImageButton2.Visible = false;
            ImageButton3.Visible = false;
            btnAdministrarPromociones.Visible = false;
        }

    }

    protected void limitar(object sender, RepeaterItemEventArgs e)
    {
        if ((sender as Repeater).Items.Count > 0)
        {
            (sender as Repeater).Controls.RemoveAt(0);
        }
    }
    private DataTable obtenerPromociones()
    {
        divConPromociones.Visible = false;
        divSinPromociones.Visible = false;
        DataTable resultado = null;
        try
        {
            DataTableCollection resultadoColeccion =
                _controlBD.ObtenerDatosFiltrados(@"select * from contenido where tipo='99' order by idContenido desc").Tables;
            if (resultadoColeccion[0].Rows.Count > 0)
            {
                divConPromociones.Visible = true;
                resultado = resultadoColeccion[0];
            }
            else
            {
                divSinPromociones.Visible = true;
                resultado = new DataTable();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error. " + ex.Message);
        }
        return resultado;
    }
}