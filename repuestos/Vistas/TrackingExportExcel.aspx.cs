using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using ClosedXML.Excel;
using System.IO;

public partial class Vistas_TrackingExportExcel : System.Web.UI.Page
{

    SqlConnection con;
    SqlCommand cmd;
    // Clase que interactua con la base de datos
    ControlBD _controlBD = new ControlBD();

    protected void Page_Load(object sender, EventArgs e)
    {


        msjesError.Visible = false;
        msjesError.InnerText = "";

        if (!IsPostBack)
        {
            LlenarComboMarcas();

        }
       
    }


    protected void exportaExcel()
    {

        string marca;
        string fechaIni;
        string fechaFin;
        string evento;


        marca = ComboMarcas.SelectedValue; ;
        fechaIni = Convert.ToString(txtdesde.Text);
        fechaFin = Convert.ToString(txtHasta.Text);
        evento = ddlLasEvent.SelectedItem.Text;
        if (evento == "Seleccion Evento")
        {
            evento = "";
        }
        else
        {
            evento = ddlLasEvent.SelectedItem.Text;
        
        }







        DataSet ds = new DataSet();

        ds = _controlBD.SeguimientoPedidoBuscar(marca, fechaIni, fechaFin, evento);
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

    private void LlenarComboMarcas()
    {
        ComboMarcas.Items.Clear();
        ComboMarcas.Items.Add(new ListItem("Selección Marca", ""));

        //Llenar el combo box con las marcas
        foreach (string marca in _controlBD.CrearMarcas(Session["rut"].ToString()))
        {
            ComboMarcas.Items.Add(marca);
        }
    }

    protected void btnBuscarCodigo_Click(object sender, ImageClickEventArgs e)
    {
        exportaExcel();
    }
}