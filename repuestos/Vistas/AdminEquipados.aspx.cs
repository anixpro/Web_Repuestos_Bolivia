using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using log4net;
using log4net.Config;

public partial class Vistas_AdminEquipados : System.Web.UI.Page
{
    ControlBD _controlBD;
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_AdminEquipados));

    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;
        _controlBD = new ControlBD();
        divConPromos.Visible = false;
        divSinPromos.Visible = false;

        if (!IsPostBack)
        {
            DataSet ds = _controlBD.ObtenerDatos("marca");
            foreach (DataRow campo in ds.Tables[0].Rows)
            {
                ListItem ls = new ListItem();
                ls.Text = campo["nombreMarca"].ToString();
                CheckBoxListMarcas.Items.Add(ls);
            }
        }

        // Se inician imagenes del carrusel ocultas. Se activan solo si hay accesorios ingresados
        // (recordar que se deben mostrar los ulitmos 5... si es que los hay!)
        imgPromo1.Visible = false;
        imgPromo2.Visible = false;
        imgPromo3.Visible = false;
        imgPromo4.Visible = false;

        _controlBD = new ControlBD();

        if (int.Parse(Session["permisos"].ToString()) != 1)
        {
            // en el caso que usuario no sea administrador
        }

        // Se completan las imágenes de los últimos repuestos subidos
        DataTable dtImagenes = obtenerUltimasPromociones();

        String[] imgSrcs = new String[5];
        int x = 0;

        if (dtImagenes.Rows.Count > 0)
        {
            divConPromos.Visible = true;
                
            foreach (DataRow fila in dtImagenes.Rows)
            {
                imgSrcs[x] = fila["html"].ToString();
                x++;
            }
            // Se activa la primera imagen del carrusel, si es que hay
            if (imgSrcs[0] != null)
            {
                imgPromo1.Src = imgSrcs[0];
                imgPromo1.Visible = true;
            }

            if (imgSrcs[1] != null)
            {
                imgPromo2.Src = imgSrcs[1];
                imgPromo2.Visible = true;
            }

            if (imgSrcs[2] != null)
            {
                imgPromo3.Src = imgSrcs[2];
                imgPromo3.Visible = true;
            }

            if (imgSrcs[3] != null)
            {
                imgPromo4.Src = imgSrcs[3];
                imgPromo4.Visible = true;
            }
        }
        else
        {
            divSinPromos.Visible = true;
        }
    }
    protected void EditarRegistro(object source, RepeaterCommandEventArgs e)
    {

    }
    protected void btnAgregarNuevaPromo_Click(object sender, EventArgs e)
    {
        //string IdCont = "";
        // Se inserta nueva promocion en BD
        _controlBD.InsertarContenido("../doc/contenidosPromociones/" + subirImagen.FileName, txtTitulo.Text, int.Parse("199"));
        // Se sube imagen al servidor
        subirImagen.SaveAs(Server.MapPath("~/doc/contenidosPromociones/") + subirImagen.FileName);
        Response.Redirect("AdminEquipados.aspx");
    }

    private DataTable obtenerUltimasPromociones()
    {
        DataTable resultado;
        DataTableCollection resultadoColeccion =
            _controlBD.ObtenerDatosFiltrados(@"select top 5 * from contenido where tipo='199'order by idContenido desc").Tables;
        if (resultadoColeccion.Count > 0)
        {
            resultado = resultadoColeccion[0];
        }
        else
        {
            resultado = new DataTable();
        }
        return resultado;
    }

    public void postbackGridview(Object sender, GridViewCommandEventArgs ev)
    {
        if (ev.CommandName == "elimina")
        {
            int index = Convert.ToInt32(ev.CommandArgument);
            GridViewRow selectedRow = GridView1.Rows[index];
            TableCell promocion = selectedRow.Cells[1];
            string idPromocion = promocion.Text;
            try
            {
                _controlBD.EliminarRegistro("contenido", idPromocion, "idContenido");
                Response.Redirect("AdminEquipados.aspx");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error. " + ex.Message);
            }
        }
    }
}
