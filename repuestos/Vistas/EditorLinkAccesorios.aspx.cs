using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Vistas_EditorLinkAccesorios : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void btnSubir_Click(object sender, EventArgs e)
    {
        String ruta = Server.MapPath("~") + "\\doc\\contenidoAccesorios\\LinkAccesorios\\Accesorios.rar";

        // Obtener el nombre del archivo a subir
        string fileName = fudEligeCatalogo.FileName;

        // Crear la ruta y el nombre para comprobar si hay duplicados.
        string pathToCheck = ruta + "Accesorios.rar";

        // Compruebe si ya existe un archivo con el
        // Mismo nombre que el archivo que desea cargar.       
        if (System.IO.File.Exists(ruta))
        {
            try
            {
                System.IO.File.Delete(ruta);
            }
            catch (System.IO.IOException ex)
            {
                
                msjesError.Visible = true;
                msjesError.InnerText = "Hubo un problema al cargar archivo. Disculpe las molestias " + ex.Message;

                return;
            }

           
        }
        

        // Llamar al método SaveAs para guardar la Subida

        try
        {
            fudEligeCatalogo.SaveAs(ruta);
            msjesError.Visible = true;
            msjesError.InnerText = "Carga correcta del archivo." ;
        }
        catch (System.IO.IOException ex)
        {

            msjesError.Visible = true;
            msjesError.InnerText = "Hubo un problema al cargar archivo. Disculpe las molestias " + ex.Message;

            return;
        }



    }
}