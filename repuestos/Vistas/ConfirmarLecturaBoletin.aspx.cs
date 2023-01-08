using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using log4net.Config;

public partial class Vistas_ConfirmarLecturaBoletin : System.Web.UI.Page
{
    ControlBD _controlBd;
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_ConfirmarLecturaBoletin));

    protected void Page_Load(object sender, EventArgs e)
    {
        _controlBd = new ControlBD();
        Boletin modeloBoletin = new Boletin();

        string idBol = Request.QueryString["id"];

        //El id del boletín habria que sacarlo por el lado del cliente
        if (!modeloBoletin.leyoBoletin(Session["rut"].ToString(), idBol))
        {
            // Se inserta registro que permitirá saber si persona ha leido ese boletin
            _controlBd.InsertarDatos("insert into boletines_persona values('" + idBol + "','" + Session["rut"].ToString() + "')");
        }
        lblSalida.Text = modeloBoletin.leyoBoletin(Session["rut"].ToString(), idBol) + "," +idBol + "," + Session["rut"].ToString();
    }
}