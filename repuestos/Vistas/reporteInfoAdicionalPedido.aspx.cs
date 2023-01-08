using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using ClosedXML.Excel;
using System.IO;

public partial class Vistas_reporteInfoAdicionalPedido : System.Web.UI.Page
{
    ControlBD _controlBD = new ControlBD();
    SqlConnection con;
    SqlCommand cmd;
    SendMail_helper _mail = new SendMail_helper();

    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;

        if (!Page.IsPostBack)
        {
            llenaMarcas();
        }
    }

    protected void btnBuscarSoli_Click(object sender, EventArgs e)
    {
        llenaGrilla();
    }

    private void llenaMarcas() {
        ddlMarca.Items.Clear();
        ddlMarca.Items.Add(new ListItem("Seleccionar", "-1"));

        //Llenar el combo box con las marcas
        foreach (string marca in _controlBD.CrearMarcas(Session["rut"].ToString()))
        {
            ddlMarca.Items.Add(marca);
        }

    }

    private void llenaGrilla() {
        try
        {
            if (txtDesde.Text.Length > 0 && txtHasta.Text.Length == 0)
            {
                msjesError.InnerText = "Debe completar ambas fechas.";
                msjesError.Visible = true;
                return;
            }

            if (txtDesde.Text.Length == 0 && txtHasta.Text.Length > 0)
            {
                msjesError.InnerText = "Debe completar ambas fechas.";
                msjesError.Visible = true;
                return;
            }

            con = new SqlConnection();
            cmd = new SqlCommand();

            con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
            con.Open();
            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "sp_ia_reporte_pedidos";
            cmd.CommandTimeout = 10;
            cmd.Parameters.Add("@marca", SqlDbType.VarChar, 50).Value = ddlMarca.SelectedValue;
            cmd.Parameters.Add("@desde", SqlDbType.VarChar,10).Value = txtDesde.Text;
            cmd.Parameters.Add("@hasta", SqlDbType.VarChar, 10).Value = txtHasta.Text;
            cmd.Parameters.Add("@xls", SqlDbType.VarChar, 2).Value = "NO";
            cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgvPedidosSubClientes.DataSource = dt;
            dgvPedidosSubClientes.DataBind();

            con.Close();
        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Se presento un error al ingresar] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
            con.Close();
        }
    }

    protected void dgvPedidosSubClientes_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgvPedidosSubClientes.PageIndex = e.NewPageIndex;
        llenaGrilla();
    }

    protected void btnExcel_Click(object sender, EventArgs e)
    {
        string marca;
        string fechaIni;
        string fechaFin;


        marca = ddlMarca.SelectedValue; ;
        fechaIni = txtDesde.Text;
        fechaFin = txtHasta.Text;


        DataSet ds = new DataSet();

        ds = _controlBD.reportePedidos(marca, fechaIni, fechaFin);
        if (ds.Tables[0].Rows.Count != 0)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                //wb.Worksheets.Add(dt, "Cotizaciones");
                wb.Worksheets.Add(ds);
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=Tracking.xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
            msjesError.Visible = false;
            msjesError.InnerText = "";
        }
        else
        {

            msjesError.InnerText = "No existen datos para el criterio de busqueda realizado";
            msjesError.Visible = true;
        }
    }
}
