using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using log4net.Config;
using System.Data;
using System.IO;

public partial class Vistas_MerchandisingAdmin : System.Web.UI.Page
{
    ControlBD _controlBD;
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_MerchandisingAdmin));

    protected void Page_Load(object sender, EventArgs e)
    {
        // Instanciación de objetos
        _controlBD = new ControlBD();

        // Iniciación de mensajes de error
        msjesError.Visible = false;
        lblValidaCodigo.Visible = false;
        lblValidaDescrip.Visible = false;

        // En caso que se entre por 1a
        if (!IsPostBack)
        {
            DataSet _dSet = _controlBD.ObtenerDatos("marca");
            foreach (DataRow campos in _dSet.Tables[0].Rows)
            {
                this.cbxMarcas.Items.Add(campos["nombreMarca"].ToString());
            }
        }
    }
    protected void btnAgregaAccesorio_Click(object sender, EventArgs e)
    {
        if (txtCodigo.Text == "" || txtDescripcion.Text == "")
        {
            lblValidaCodigo.Visible = true;
            lblValidaDescrip.Visible = true;
        }
        else
        {
            if (subirImagen.HasFile)
            {
                string nombreArchivo = "";

                    // Get the name of the file to upload.
                string fileName = Server.HtmlEncode(subirImagen.FileName);

                // Get the extension of the uploaded file.
                string extension = System.IO.Path.GetExtension(fileName);
                extension = extension.ToLower();

                // se restringen los formatos
                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif" || extension == ".bmp")
                {
                    nombreArchivo = cbxMarcas.SelectedItem.ToString().Trim() + txtCodigo.Text + extension;
                    if (File.Exists(Server.MapPath("~/doc/contenidoMerchandising/") + nombreArchivo))
                    {
                        msjesError.InnerText = "El Merchandising ya existe";
                        msjesError.Visible = true;
                        return;
                    }
                    try
                    {
                        subirImagen.SaveAs(Server.MapPath("~/doc/contenidoMerchandising/") + nombreArchivo);
                        string codigo = txtCodigo.Text;
                        string descripcion = txtDescripcion.Text;
                        string marca = cbxMarcas.SelectedItem.ToString();
                        string queryInsertar = @"INSERT INTO merchandising(nombreMarca,fecha,codigo,descripcion,imagen)
                                VALUES('" + marca + "' , GETDATE() , '" + codigo + "' , '" + descripcion + "' , '" + "../doc/contenidoMerchandising/" + nombreArchivo + "')";
                        _controlBD.InsertarDatos(queryInsertar);
                        msjesError.InnerText = "El merchandising de codigo " + codigo + " de la marca " + marca + " fue ingresado correctamente";
                        msjesError.Visible = true;
                    }

                    catch (Exception ex)
                    {
                        msjesError.InnerText = "Error al subir la imágen, porfavor contacte al administrador del sitio";
                        msjesError.Visible = true;
                        logger.Error("En [btnAgregaAccesorio_Click] Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
                    }
                // Fin if restriccion de formatos
                }
                else
                {
                    msjesError.InnerText = "La imágen debe ser de extensión jpg, jpeg, png, gif o bmp";
                    msjesError.Visible = true;
                }
            }
            else
            {
                msjesError.InnerText = "No hay imágen para cargar, o es inválida";
                msjesError.Visible = true;
                logger.Warn("En [btnAgregaAccesorio_Click] imagen inexistente o inválida");
            }
        }
    }
    protected void btnRestaurar_Click(object sender, EventArgs e)
    {
        Response.Redirect("MerchandisingAdmin.aspx", false);
    }
}