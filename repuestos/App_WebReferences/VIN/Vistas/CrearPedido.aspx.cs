using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using log4net;
using log4net.Config;

public partial class Vistas_crearPedido : System.Web.UI.Page
{
    RealizarPedido _pedido = new RealizarPedido();
    ConfirmarPedido _confirPedido = new ConfirmarPedido();
    SapAPI _sapApi = new SapAPI();
    ControlBD _controlBD = new ControlBD();
    SendMail_helper _mail = new SendMail_helper();
    ControlPedido _controlPedido = new ControlPedido();

    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_crearPedido));

    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;
        GenerarPedidoSap();
        SapAPI _sapApi = new SapAPI();
    }

    /// <summary>
    /// Genera pedido en SAP y se envía correo al usuario con resumen
    /// de lo comprado (con stock)
    /// </summary>
    public void GenerarPedidoSap()
    {
        string numCoti = Request.QueryString["numeroCot"];
        string tipoPedido = Request.QueryString["tipoPedido"];
        string prioridad = Request.QueryString["prioridad"]; 

        _confirPedido.NumCotizacion = numCoti;
        _confirPedido.TipoPedido = tipoPedido;
        _confirPedido.Prioridad = prioridad;

        if (_controlPedido.obtieneSiCotizacionFueConfirmada(numCoti, Session["idSession"].ToString()))
        {
            msjesError.InnerText = "Estimado Operador. Esta cotización ya fue confirmada. Porfavor realice una nueva búsqueda haciendo clic en la opción del menú superior. Gracias";
            msjesError.Visible = true;
            PanelPedidoCreado.Visible = false;
            PanelResumen.Visible = false;
            return;
        }

        // En caso de ser foráneo se asigna el tipo de pedido que corresponde
        if (_controlPedido.siEsForaneo(numCoti))
        {
            _confirPedido.TipoPedido = "ZBP2";
        }

        // Se confirma cotizacion en SAP para transformarla a pedido
        string errorP;
        if (!_pedido.ConfirmarPedido(_confirPedido, out errorP))
        {
            msjesError.InnerText = "ERROR, pedido no se genero. Problemas con SAP: " + errorP;
            msjesError.Visible = true;
        }
        else
        {
            MessageBox.Show("Su pedido ha sido confirmado con el número: " + _pedido.ResultPedido.NumPedido + ". Se enviará el detalle del pedido a su E-Mail");

            PanelPedidoCreado.Visible = true;

            //Envío de correo
            string para = _sapApi.GetCorreoUsuario(Session["rut"].ToString());
            string asunto = "SKBERGE: Pedido de repuestos confirmado [NO RESPONDER]";
            string textoCorreo = "";
            string detallePedido = "";

            //Se actualiza la tabla pedido con el numero de pedido generado por SAP
            _controlBD.InsertarDatos(@"update pedido set E_VBELN_PEDIDO = '" + _pedido.ResultPedido.NumPedido + "' , " +
                                    " I_LPRIO_PEDIDO = " + _confirPedido.Prioridad.Trim() + " , " +
                                    " I_AUART_PEDIDO = '" + _confirPedido.TipoPedido.Trim() + "' , estado = 'CONFIRMADO' " +
                                    " where E_VBELN = '" + numCoti.Trim() + "'");

            int cont = 1;

            // Se envía mail con detalle
            DataSet dsDetallePed = _controlBD.ObtenerDatosFiltrados("select * from MATERIALES_PEDIDO where id_pedido = '" + _sapApi.getIdPedidoPorE_VBELN_PEDIDO(_pedido.ResultPedido.NumPedido) + "'");
            foreach (DataRow drPed in dsDetallePed.Tables[0].Rows)
            {
                detallePedido += cont + ". " + " | " + drPed["codigo"].ToString() + " | " + drPed["descripcion"].ToString() + " | $" + drPed["valor"].ToString() + " | " + drPed["cantidad"].ToString() + " | $" + drPed["total"].ToString() + " |\n";
                cont++;                    
            }

            textoCorreo = "Usted ha confirmado un pedido de repuestos a SKBergé con el código de seguimiento N°" + _pedido.ResultPedido.NumPedido;
            textoCorreo += "\nConcesionario: " + _sapApi.GetNombreDealer(Session["rut"].ToString());
            textoCorreo += "\nDireccion sucursal: " + _sapApi.GetDireccionSucursalByShipCode(_sapApi.GetShipCodeOfPedido(_pedido.ResultPedido.NumPedido));
            textoCorreo += "\nMonto total del pedido: $" + _sapApi.GetTotalNeto(numCoti);
            textoCorreo += "\n\nDetalle del pedido: (numero, código, descripción, valor, cantidad, total)";
            textoCorreo += "\n" + detallePedido;
            _mail.EnviarCorreo(para, asunto, textoCorreo);

            string numPedido = _pedido.ResultPedido.NumPedido;
            VerListasResumenes(_pedido.ResultPedido.NumPedido, numCoti); 

            lblNumPedido.Text = numPedido;
            lblConcesionario.Text = _sapApi.GetNombreDealer(Session["rut"].ToString());
            lblSucursal.Text = _sapApi.GetDireccionSucursalByShipCode(_sapApi.GetShipCodeOfPedido(_pedido.ResultPedido.NumPedido));
            lblTotalPedido.Text = "$" + _sapApi.GetTotalNeto(numCoti);

            //Se limpian los sin stock
            _controlBD.InsertarDatos("update vfc set id_pedido = '" + Session["rut"].ToString() + "' where id_pedido = '" + numCoti + "'");
            _controlBD.InsertarDatos("update backorder set id_pedido = '" + Session["rut"].ToString() + "' where id_pedido = '" + numCoti + "'");
            _controlBD.InsertarDatos("update descartados set id_pedido = '" + Session["rut"].ToString() + "' where id_pedido = '" + numCoti + "'");
        }
    }

    /// <summary>
    /// Se despliega el resumen de lo comprado, tanto con stock como sin stock (Reserva, VFC y Descartado)
    /// </summary>
    /// <param name="e_vbeln_pedido">Numero de pedido SAP</param>
    /// <param name="numeroCotizacion">Numero de cotización SAP</param>
    public void VerListasResumenes(string e_vbeln_pedido, string numeroCotizacion)
    {
        // Se muestra el resumen de los repuestos comprados
        DataSet cotizacion = _controlBD.ObtenerDatosFiltrados("select id_pedido, marca, codigo, descripcion, strGrupoMateriales,cantidad,stock,valor,total,cast(stock-cantidad as varchar) as diferencia from MATERIALES_PEDIDO where id_pedido = '" + _sapApi.getIdPedidoPorE_VBELN_PEDIDO(e_vbeln_pedido) + "'");

        GridViewResumen.DataSource = cotizacion;
        GridViewResumen.DataBind();

        DataSet dsBo = _controlBD.ObtenerDatosFiltrados("select * from BACKORDER where id_pedido = '" + numeroCotizacion + "'");
        DataSet dsVfc = _controlBD.ObtenerDatosFiltrados("select * from VFC where id_pedido = '" + numeroCotizacion + "'");
        DataSet dsDes = _controlBD.ObtenerDatosFiltrados("select * from DESCARTADOS where id_pedido = '" + numeroCotizacion + "'");

        if (dsBo.Tables[0].Rows.Count != 0)
        {
            GridViewReserva.DataSource = dsBo;
            GridViewReserva.DataBind();
            GridViewReserva.Visible = true;
        }
        else
        {
            GridViewReserva.DataSource = "";
            GridViewReserva.DataBind();
            GridViewReserva.Visible = true;
        }

        if (dsVfc.Tables[0].Rows.Count != 0)
        {
            GridViewVfc.DataSource = dsVfc;
            GridViewVfc.DataBind();
            GridViewVfc.Visible = true;
        }
        else
        {
            GridViewVfc.DataSource = "";
            GridViewVfc.DataBind();
            GridViewVfc.Visible = true;
        }

        if (dsDes.Tables[0].Rows.Count != 0)
        {
            GridViewDescartado.DataSource = dsDes;
            GridViewDescartado.DataBind();
            GridViewDescartado.Visible = true;
        }
        else
        {
            GridViewDescartado.DataSource = "";
            GridViewDescartado.DataBind();
            GridViewDescartado.Visible = true;
        }
    }

    #region Calcula el gran total de una columna
    public int TotalUnitPrice;
    public int GetUnitPrice(int Price)
    {
        TotalUnitPrice += Price;
        return Price;
    }
    public int GetTotal()
    {
        return TotalUnitPrice;
    }
    #endregion

}