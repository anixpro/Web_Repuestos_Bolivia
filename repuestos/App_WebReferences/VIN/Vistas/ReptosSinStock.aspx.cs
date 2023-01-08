using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using log4net.Config;

public partial class Vistas_ReptosSinStock : System.Web.UI.Page
{
    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_ReptosSinStock));

    SapAPI _sapApi = new SapAPI();
    ControlBD _controlBD = new ControlBD();

    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;

        GridViewNoStock.DataSource = _controlBD.ObtenerDatosFiltrados("select * from no_stock where idSession = '"+Session["idSession"].ToString()+"'");
        GridViewNoStock.DataBind();

        SinStock();
    }

    public void SinStock()
    {
        bool reserva = false; // Indicará si por lo menos 1 se fue a backorder
        RutHelper _rutHelper = new RutHelper();
        ControlVfc _controlVFC = new ControlVfc();
        for (int i = 0; i < GridViewNoStock.Rows.Count; i++)
        {
            //Para backOrder///////////////////////////////////////////////////////////////////////////////////////
            GridViewRow row = GridViewNoStock.Rows[i];

            if (row.Cells[7].Text.Trim() == "s")
            {
                //Si el repuesto fue checkeado para envio a BO se obtiene el detalle de dicho repuesto
                //y se envian los datos a la clase VFC para backoreder
                //Se declaran las variables de respuestas del WS
                List<String> identificadores = new List<String>();
                List<String> detalles = new List<String>();

                //Obtener los datos desde la grilla noStock
                //Obtener los datos seleccionados desde la grilla noStock
                string cantidad = GridViewNoStock.Rows[i].Cells[3].Text;
                string codigo = GridViewNoStock.Rows[i].Cells[1].Text;
                string detalle = GridViewNoStock.Rows[i].Cells[2].Text;
                string marca = _sapApi.GetAbreviadoMarca(GridViewNoStock.Rows[i].Cells[0].Text);

                string creador = _rutHelper.GetRutConDigito(Session["rut"].ToString());
                string nombreUser = _sapApi.GetNombreUsuario(Session["rut"].ToString());
                string dealer = _sapApi.GetNombreDealer(Session["rut"].ToString());
                string direccion = _sapApi.GetDireccionSucursal(Session["rut"].ToString());
                string tipoPed = "Backorder";
                string opcionvfc = "";// hiddenPrioridad.Value;
                string numBO = "";
                string numBO2 = "";
                string idPedido = Session["idSession"].ToString();
                string cc = _sapApi.GetCorreosVFCUsuario(Session["rut"].ToString());
                string codsap = _sapApi.GetCodigoShipCode(Session["rut"].ToString());
                _controlVFC.hacerBo(cantidad, codigo, detalle, marca, creador, nombreUser, dealer, direccion, tipoPed, opcionvfc,"","",cc,codsap);

                identificadores = _controlVFC.getIdentificadores();

                numBO = identificadores[1];
                numBO2 = identificadores[3];

                //se insertan los datos en la tabla BACKORDER
                _controlBD.InsertarDatos(@"insert into BACKORDER(num_backOrder,id_pedido,rut_user,fecha_creacion,
                                                                codigo_rep,marca,cantidad,detalle_rep)
                                        values('" + numBO.Substring(9, numBO.Length - 9) + "','"+idPedido+"','" + creador + "' , GETDATE(), " +
                                        " '" + codigo + "' , '" + marca + "' , " + cantidad + " , '" + detalle + "')");

            }
            //Para backOrder///////////////////////////////////////////////////////////////////////////////////////

            //Para VFC//////////////////////////////////////////////////////////////////////////////////////////////
            GridViewRow row2 = GridViewNoStock.Rows[i];

            if (row.Cells[6].Text.Trim() == "s")
            {
                //Si el repuesto fue checkeado para envio a VFC se obtiene el detalle de dicho repuesto
                //y se envian los datos a la clase VFC 

                //Obtener los datos seleccionados desde la grilla noStock
                string cantidad = GridViewNoStock.Rows[i].Cells[3].Text;
                string codigo = GridViewNoStock.Rows[i].Cells[1].Text;
                string detalle = GridViewNoStock.Rows[i].Cells[2].Text;
                string marca = _sapApi.GetAbreviadoMarca(GridViewNoStock.Rows[i].Cells[0].Text);

                string creador = _rutHelper.GetRutConDigito(Session["rut"].ToString());
                string nombreUser = _sapApi.GetNombreUsuario(Session["rut"].ToString());
                string dealer = _sapApi.GetNombreDealer(Session["rut"].ToString());
                string direccion = _sapApi.GetDireccionSucursal(Session["rut"].ToString());
                string tipoPed = "VFC";
                string opcionvfc = "";// hiddenPrioridad.Value;
                string cc = _sapApi.GetCorreosVFCUsuario(Session["rut"].ToString());
                string codsap = _sapApi.GetCodigoShipCode(Session["rut"].ToString());

                GridViewRow rowX = GridViewNoStock.Rows[i];
                TextBox txt = rowX.Cells[3].FindControl("txtVin") as TextBox;
                string vin = txt.Text;


                //Se declaran las variables de respuestas del WS
                List<String> identificadores = new List<String>();
                List<String> detalles = new List<String>();


                //Se llama al método que inserta el vfc
                _controlVFC.hacerVfc(cantidad, codigo, detalle, marca, vin, creador, nombreUser, dealer, direccion, tipoPed, opcionvfc,"","",cc, codsap);

                string numVFC = "";
                string numVFC2 = "";
                string idPedido = Session["idSession"].ToString();
                identificadores = _controlVFC.getIdentificadores();

                numVFC = identificadores[0];
                numVFC2 = identificadores[1];

                //se insertan los datos en la tabla BACKORDER
                _controlBD.InsertarDatos(@"insert into VFC(num_VFC,id_pedido,rut_user,fecha_creacion,
                                                                codigo_rep,marca,cantidad,detalle_rep,cod_vin)
                                        values('" + numVFC.Substring(9, numVFC.Length - 9) + "','"+idPedido+"','" + creador + "' , GETDATE(), " +
                                        " '" + codigo + "' , '" + marca + "' , " + cantidad + " , '" + detalle + "','" + vin + "')");


            }
            //Para VFC//////////////////////////////////////////////////////////////////////////////////////////////


            //Para Descarte ///////////////////////////////////////////////////////////////////////////////////////////
            GridViewRow row3 = GridViewNoStock.Rows[i];

            if (row.Cells[8].Text.Trim() == "s")
            {
                //Obtengo los datos para descartar
                string cantidad = GridViewNoStock.Rows[i].Cells[3].Text;
                string codigo = GridViewNoStock.Rows[i].Cells[1].Text;
                string detalle = GridViewNoStock.Rows[i].Cells[2].Text;
                string marca = _sapApi.GetAbreviadoMarca(GridViewNoStock.Rows[i].Cells[0].Text);


                string creador = _rutHelper.GetRutConDigito(Session["rut"].ToString());
                string idPedido = Session["idSession"].ToString();
                _controlBD.InsertarDatos(@"insert into DESCARTADOS(id_pedido,rut_user,fechaDescarte,codigo_rep,marca,cantidad,detalle_rep)
                                        values('" + idPedido + "','" + creador + "',GETDATE(),'" + codigo + "','" + marca + "'," + cantidad + ",'" + detalle + "')");


            }
            //Para Descarte ///////////////////////////////////////////////////////////////////////////////////////////
        }

        if (reserva)
        {
           // MessageBox.Show("El o los repuestos enviados a reserva se facturarán inmediatamente una vez arribado(s) a bodega");
        }

        GridViewReserva.DataSource = _controlBD.ObtenerDatosFiltrados("select * from BACKORDER where id_pedido = '" + Session["idSession"].ToString() + "'");
        GridViewReserva.DataBind();
        GridViewReserva.Visible = true;

        GridViewVfc.DataSource = _controlBD.ObtenerDatosFiltrados("select * from VFC where id_pedido = '" + Session["idSession"].ToString() + "'");
        GridViewVfc.DataBind();
        GridViewVfc.Visible = true;

        GridViewDescartado.DataSource = _controlBD.ObtenerDatosFiltrados("select * from DESCARTADOS where id_pedido = '" + Session["idSession"].ToString() + "'");
        GridViewDescartado.DataBind();
        GridViewDescartado.Visible = true;

        _controlBD.InsertarDatos("delete from no_stock where idSession = '"+Session["idSession"].ToString()+"'");

    }
}