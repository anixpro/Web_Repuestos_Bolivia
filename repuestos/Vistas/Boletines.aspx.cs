using System;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using log4net.Config;

public partial class Vistas_Boletines : System.Web.UI.Page
{
    ControlBD _controlBd;
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_Boletines));

    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;
        
        _controlBd = new ControlBD();
        CargarBoletines(Session["rut"].ToString());

        grillaBoletines.SelectedIndexChanged += OnEliminarBoletin;

        if (int.Parse(Session["permisos"].ToString()) != 1)
        {
            btnSeccionAgregarBoletin.Visible = false;
            grillaBoletines.Columns[6].Visible = false;
            divAgregaBoletin.Visible = false;
        }

        if (!IsPostBack)
        {

        }
    }

    public void CargarBoletines(string rut)
    {
        DataSet ds = _controlBd.ObtenerDatos("boletines");
        // se rescata la cantidad de boletines leidos
        ControlBoletin ctrlBoletin = new ControlBoletin();
        DataSet dsLeidos = ctrlBoletin.obtenerBoletinesLeidosPorRut(rut);
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            if (dsLeidos.Tables[0].Select("idBoletin=" + dr["idBoletin"]).Length > 0)
            {
                dr["leido"] = "Sí";
            }
        }
        grillaBoletines.DataSource = ds;
        grillaBoletines.DataBind();
    }

    public void CargarBoletinesPorFechas(string rut,String fecha)
    {
        ControlBoletin ctrlBoletin = new ControlBoletin();
        DataSet ds = ctrlBoletin.obtenerBoletinesPorFecha(fecha);
        // se rescata la cantidad de boletines leidos
        DataSet dsLeidos = ctrlBoletin.obtenerBoletinesLeidosPorRut(rut);
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            if (dsLeidos.Tables[0].Select("idBoletin=" + dr["idBoletin"]).Length > 0)
            {
                dr["leido"] = "Sí";
            }
        }
        grillaBoletines.DataSource = ds;
        grillaBoletines.DataBind();
    }
    
    protected void btnBuscar_CLick(object sender, ImageClickEventArgs e)
    {
        if (txtFechaBusqueda.Text == "")
        {
            // NTDH
        }
        else
        {
            string fecha = txtFechaBusqueda.Text;
            CargarBoletinesPorFechas(Session["rut"].ToString(),fecha);
        }

        if (grillaBoletines.Rows.Count == 0)
        {
            return;
        }
    }

    protected void OnEliminarBoletin(object o, EventArgs e)
    {
        //Eliminar el boletin seleccionado
        string idBol = grillaBoletines.SelectedRow.Cells[0].Text;
        _controlBd.InsertarDatos("delete from boletines where idBoletin = '"+idBol+"'");
        _controlBd.InsertarDatos("delete from boletines_persona where idBoletin = '" + idBol + "'");
        msjesError.InnerText = "Boletín eliminado";
        msjesError.Visible = true;
        CargarBoletines(Session["rut"].ToString());
    }
    protected void btnAgregaBoletin_Click(object sender, EventArgs e)
    {
        if (txtTitulo.Text == "" || txtDescripcion.Text == "")
        {
            lblValidaTitulo.Visible = true;
            lblValidaDescrip.Visible = true;
        }

        else
        {
            String fileName = subirDoc.FileName;

            // Get the extension of the uploaded file.
            string extension = System.IO.Path.GetExtension(fileName);
            extension = extension.ToLower();

            // se restringen los formatos
            if (extension == ".pdf")
            {
                String pathDocu = Server.MapPath("~/doc/boletines/");
                subirDoc.SaveAs(pathDocu + fileName);
                //subirDoc.SaveAs(Server.MapPath("~/doc/boletines/") + fileName);
                string rutaDoc = "../doc/boletines/" + subirDoc.FileName.ToString();
                _controlBd.InsertarBoletin(txtTitulo.Text, txtDescripcion.Text, rutaDoc);
                lblValidaDoc.Visible = false;
                lblValidaDescrip.Visible = false;
                lblValidaTitulo.Visible = false;
                msjesError.InnerText = "Boletin agregado con éxito";
                msjesError.Visible = true;
                CargarBoletines(Session["rut"].ToString());
            }
            else
            {
                lblValidaDoc.Visible = true;
                msjesError.InnerText = "Error. Porfavor chequee la extensión del documento";
            }
        }
    }
}