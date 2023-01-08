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

public partial class Vistas_Accesorio : System.Web.UI.Page
{
    // Controlador de BD
    ControlBD _controlBD;
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_Accesorio));

    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;

        // Se inicia controlador de BD
        _controlBD = new ControlBD();

        // Se ocultan los divs al cargar la página
        divConPromos.Visible = false;
        divSinPromos.Visible = false;

        // Sección donde se muestran mensajes de error
        msjesError.InnerText = "";
        msjesError.Visible = false;

        if (!IsPostBack)
        {
            DataSet ds;

            try
            {
                // Se rescatan las marcas
                ds = _controlBD.ObtenerDatos("marca");
            }
            catch (Exception ex)
            {
                msjesError.InnerText = "Error en el servidor, favor contacte a soporte. Detalle del error: " + ex.Message + ". Inner Exception: " + ex.InnerException + ". Stack Trace: " + ex.StackTrace;
                msjesError.Visible = true;
                return;
            }

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
            Response.Redirect("../index.aspx?evento=ev2");
        }
        completarImagenes();
      
    }
    protected void EditarRegistro(object source, RepeaterCommandEventArgs e)
    {

    }
    protected void btnAgregarNuevaPromo_Click(object sender, EventArgs e)
    {
        string IdCont = "";
        try
        {
            // Se inserta nueva promocion en BD
            _controlBD.InsertarContenido("../doc/contenidosPromociones/" + subirImagen.FileName, txtTitulo.Text, int.Parse("99"));
            // Se sube imagen al servidor
            subirImagen.SaveAs(Server.MapPath("~/doc/contenidosPromociones/") + subirImagen.FileName);
            // Se rescata último ID
            DataSet ds = _controlBD.ObtenerDatosFiltrados("select top 1 idContenido from contenido where tipo='99' order by idContenido desc");
            foreach (DataRow campo in ds.Tables[0].Rows)
            {
                IdCont = campo["idContenido"].ToString();
            }
            for (int i = 0; i < CheckBoxListMarcas.Items.Count; i++)
            {
                if (CheckBoxListMarcas.Items[i].Selected)
                {
                    string _marca = CheckBoxListMarcas.Items[i].Text;
                    _controlBD.InsertarDatos("insert into contenido_marca values(" + IdCont + ",'" + _marca + "')");
                }
            }
            CheckBoxListMarcas.Items.Clear();
            msjesError.InnerText = "Promoción agregada satisfactoriamente";
            msjesError.Visible = true;
            completarImagenes();
        }
        catch (Exception ex)
        {
            // Se despliega el error
            msjesError.InnerText = "Error en el servidor, favor contacte a soporte. Detalle del error: " + ex.Message + ". Inner Exception: " + ex.InnerException + ". Stack Trace: " + ex.StackTrace;
            msjesError.Visible = true;
            logger.Error("En [btnAgregarNuevaPromo_Click] Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
        }
    }

    private DataTable obtenerUltimasPromociones()
    {
        DataTable resultado;
        DataTableCollection resultadoColeccion =
            _controlBD.ObtenerDatosFiltrados(@"select top 5 * from contenido where tipo='99'order by idContenido desc").Tables;
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

    protected void postbackGridview(Object sender, GridViewCommandEventArgs ev)
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
                GridView1.DataBind();
            }
            catch (Exception ex)
            {
                msjesError.InnerText = "Error al eliminar, contacte al administrador";
                logger.Error("en [postbackGridview] Mess: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
            }
        }
    }
    protected void completarImagenes()
    {
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
}
