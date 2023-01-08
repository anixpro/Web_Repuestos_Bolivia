using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using log4net;
using log4net.Config;

public partial class Vistas_SucursalesEditar : System.Web.UI.Page
{
    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_SucursalesEditar));

    ControlBD _controlBd;

    protected void Page_Init(object sender, EventArgs e)
    {
        if (int.Parse(Session["permisos"].ToString()) > 2)
        {
            Response.Redirect("../index.aspx?evento=ev2");
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;
        string idSucursal = Request.QueryString["id"];
        hdIdSucursal.Value = idSucursal;

        if (!IsPostBack)
        {
            _controlBd = new ControlBD();
            DataSet ds = _controlBd.ObtenerDatosFiltrados("Select * from Sucursal where idSucursal=" + idSucursal);

            if (ds.Tables.Count < 1)
            {
                if (ds.Tables[0].Rows.Count < 1)
                {
                    Response.Redirect("SucursalesLista.aspx?evento=2");
                }
            }

            String _shipCode = ds.Tables[0].Rows[0]["shipCode"].ToString();
            String _direccionSucursal = ds.Tables[0].Rows[0]["direccionSucursal"].ToString();
            String _numeroFactura = ds.Tables[0].Rows[0]["numeroFactura"].ToString();

            hdNumeroFactura.Value = _numeroFactura;
            shipCode.Text = _shipCode;
            hdShipCodeAntiguo.Value = _shipCode;
            direccionSucursal.Text = _direccionSucursal;
        }
    }
    protected void btnEditar_Click(object sender, EventArgs e)
    {
        _controlBd = new ControlBD();
        if(!_controlBd.InsertarDatos("update sucursal set shipCode='"+shipCode.Text+"', direccionSucursal='"+direccionSucursal.Text+"' where idSucursal = "+hdIdSucursal.Value))
        {
            msjesError.InnerText = "No se modificó. Compruebe que el destinatario de mercancía no esté siendo utilizado por otra sucursal";
            msjesError.Visible = true;
            return;
        } else {
            _controlBd.InsertarDatos("update persona set shipCode='" + shipCode.Text + "' where shipCode = '" + hdShipCodeAntiguo.Value + "'");
            Response.Redirect("SucursalesLista.aspx?evento=1&numFac=" + hdNumeroFactura.Value);
        }
    }
    protected void btnCancelar_Click(object sender, EventArgs e)
    {
        Response.Redirect("SucursalesLista.aspx?numFac=" + hdNumeroFactura.Value);
    }
}
