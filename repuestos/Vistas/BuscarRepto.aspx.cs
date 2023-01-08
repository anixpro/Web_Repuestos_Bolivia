using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Web.UI.WebControls;

public partial class Vistas_buscarRepto : System.Web.UI.Page
{
    ControlBD _controlBD = new ControlBD();
    SqlConnection con;
    SqlCommand cmd;

    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;

        if (!IsPostBack) {
            lblSubCliente.Visible = false;
            ddlSubCliente.Visible = false;
        }

        string nombreConcesionario = Util.obtieneNombreConcesionarioPorRUTCualquierUsuario(Session["rut"].ToString());

        int _idConcesionario = 0;
        int _nreg = 0;

        DataSet dsIdConcesionario = _controlBD.ObtenerDatosFiltrados(" SELECT idConcesionario FROM concesionario WHERE RTRIM(LTRIM(nombreConcesionario)) = '" + nombreConcesionario + "' ");
        foreach (DataRow drIdConcesionario in dsIdConcesionario.Tables[0].Rows)
        {
            _idConcesionario = Convert.ToInt32(drIdConcesionario["idConcesionario"]);
        }

       // Se debe habilitar cuando se trebaje por canales 
        //DataSet dsCanalesVenta = _controlBD.ObtenerDatosFiltrados(" SELECT COUNT(*) AS nreg FROM canales_venta_usuario_canal WHERE rut_usuario = '" + Session["rut"].ToString() + "' AND habilitado = 1 ");
        //foreach (DataRow drCanalesVenta in dsCanalesVenta.Tables[0].Rows)
        //{
        //    _nreg = Convert.ToInt32(drCanalesVenta["nreg"]);
        //}

        //if (_nreg > 0){

        //    int _idCanalVenta = 0;
        //    string _CanalVenta = "";

        //    try
        //    {
        //        _idCanalVenta = ddlCanalVenta.SelectedIndex;
        //        _CanalVenta = ddlCanalVenta.SelectedItem.ToString();
        //    }
        //    catch (Exception)
        //    {
        //        _idCanalVenta = 0;
        //        _CanalVenta = "";
        //    }

        //    ddlCanalVenta.Items.Clear();
        //    //ddlCanalVenta.Items.Add(new ListItem("Seleccionar", "-1"));
        //    ddlCanalVenta.Items.Add(new ListItem("Web", "-2"));

        //    //foreach (String canal in _controlBD.CanalesPorConcesionario(_idConcesionario))
        //    foreach (String canal in _controlBD.CanalesPorUsuario(Session["rut"].ToString(), _idConcesionario.ToString()))
        //    {
        //        ddlCanalVenta.Items.Add(canal);
        //    }
        //    ddlCanalVenta.SelectedIndex = _idCanalVenta;

        //}
        //else
        //{
            Response.Redirect("BuscarRepto2.aspx", true);
        //}

    }

    protected void btnContinuar_Click(object sender, EventArgs e)
    {
        if (ddlCanalVenta.SelectedValue == "-1")
        {
            msjesError.InnerText = "Debe seleccionar un canal de venta";
            msjesError.Visible = true;
            return;
        }

        if (ddlSubCliente.SelectedValue == "-1" && ddlCanalVenta.SelectedValue != "-1")
        {
            msjesError.InnerText = "Debe seleccionar un sub cliente";
            msjesError.Visible = true;
            return;
        }

        int _idCanal = 0;

        DataSet dsCanalesVenta = _controlBD.ObtenerDatosFiltrados(" SELECT id_canal FROM canales_venta WHERE descripcion = '" + ddlCanalVenta.SelectedValue + "' ");
        foreach (DataRow drCanalesVenta in dsCanalesVenta.Tables[0].Rows)
        {
            _idCanal = Convert.ToInt32(drCanalesVenta["id_canal"]);
        }

        Session["idCanal"] = _idCanal;
        Session["Canal"] = ddlCanalVenta.SelectedValue;
        Session["subCliente"] = ddlSubCliente.SelectedValue;

        msjesError.Visible = false;
        switch (Convert.ToInt32(Session["idCanal"]))
        {
            case 1:     
                //1.- Mayorista
                Response.Redirect("BuscarReptoMayorista.aspx", true);
                break;
            case 2:
                //2.- Ecommerce
                Response.Redirect("BuscarReptoEcommerce.aspx", true);
               break;
            case 3:
                //3.- Police
                Response.Redirect("BuscarReptoPolice.aspx", true);
                break;
            case 4:
                //3.- Police
                Response.Redirect("BuscarMerma.aspx", true);
                break;
            default:
                Response.Redirect("BuscarRepto2.aspx", true);
                break;
        }
    }

    protected void ddlCanalVenta_TextChanged(object sender, EventArgs e)
    {
        ddlSubCliente.Items.Clear();

        if (ddlCanalVenta.SelectedValue == "-2" || ddlCanalVenta.SelectedValue == "Police" || ddlCanalVenta.SelectedValue == "Ecommerce" || ddlCanalVenta.SelectedValue == "Merma y daño")
        {
            lblSubCliente.Visible = false;
            ddlSubCliente.Visible = false;
        }
        else
        {
            lblSubCliente.Visible = true;
            ddlSubCliente.Visible = true;


            //Busca subClientes asociados al canal de venta
            int _idSubCliente = 0;

            string _nombreConcesionario = Util.obtieneNombreConcesionarioPorRUTCualquierUsuario(Session["rut"].ToString());
            string _subCliente = "";

            try
            {
                _idSubCliente = ddlSubCliente.SelectedIndex;
                _subCliente = ddlSubCliente.SelectedItem.ToString();
            }
            catch (Exception)
            {
                _idSubCliente = 0;
                _subCliente = "";
            }

            ddlSubCliente.Items.Clear();
            ddlSubCliente.Items.Add(new ListItem("Seleccionar", "-1"));

            foreach (String canal in _controlBD.SubclientesPorCanal(_nombreConcesionario, ddlCanalVenta.SelectedValue))
            {
                ddlSubCliente.Items.Add(canal);
            }
            ddlSubCliente.SelectedIndex = _idSubCliente;

        }
    }
}