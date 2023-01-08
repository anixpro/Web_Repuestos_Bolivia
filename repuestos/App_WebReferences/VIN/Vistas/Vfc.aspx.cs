using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using log4net;
using log4net.Config;

public partial class Vistas_vfc : System.Web.UI.Page
{
    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_vfc));

    private ControlVfc controlVfc;

    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;
    }
    protected void checkVFC_Click(object sender, EventArgs e)
    {
        
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        controlVfc = new ControlVfc();
        controlVfc.Vin = ttxVin.Text;
        controlVfc.ObtieneDatosVfc();
        Response.Write("Año: "+controlVfc.getAnio() + "\nVin: " + controlVfc.getVin() + "\nChasis: " + controlVfc.getChasis() + "\nMarca: " + controlVfc.getMarca() + "\nModelo: " + controlVfc.getModelo() + "\nVersión: " + controlVfc.getVersion());
    }//FIN DEL BOTON

    protected void btnVfc_Click(object sender, EventArgs e)
    {
        List<String> identificadores = new List<String>();
        List<String> detalles= new List<String>();
        //LLAMO AL METODO QUE DEVUELVE EL XML
        controlVfc.hacerVfc("1", "278950100147", "NADA", "CH", "MAT464051ASL01037", "11111111-1", "LUIS ARROYO VERA", "BAUER Y CIA LTDA", "DIRECCION 01", "VFC", "normal", "0", "0", "");
        
        //LLAMO AL METODO QUE DEVUELVE LOS DATOS ANTEROIORES DEL XML RETORNADO, EN LISTAS PARSEADAS EN LA MISMA CLASE
        identificadores = controlVfc.getIdentificadores();
        detalles = controlVfc.getDetalles();
        Response.Write("Identificador        Detalle<br/><br/>");
        int x = 0; 
        foreach (String identificador in identificadores)
        {
            Response.Write(identificador+": "+detalles[x]+"<br/>");
            x++;
        }
        
    }

    protected void hacerBo_Click(object sender, EventArgs e)
    {
        List<String> identificadores = new List<String>();
        List<String> detalles = new List<String>();
        //LLAMO AL METODO QUE DEVUELVE EL XML
        controlVfc.hacerBo("1", "278950100147", "NADA", "CH", "11111111-1", "LUIS ARROYO VERA", "BAUER Y CIA LTDA", "DIRECCION 01", "Backorder","normal","0","0","");

        //LLAMO AL METODO QUE DEVUELVE LOS DATOS ANTEROIORES DEL XML RETORNADO, EN LISTAS PARSEADAS EN LA MISMA CLASE
        identificadores = controlVfc.getIdentificadores();
        detalles = controlVfc.getDetalles();
        Response.Write("Identificador        Detalle<br/><br/>");
        int x = 0;
        foreach (String identificador in identificadores)
        {
            Response.Write(identificador + ": " + detalles[x] + "<br/>");
            x++;
        }
       
    }
}