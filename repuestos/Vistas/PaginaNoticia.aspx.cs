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

public partial class Vistas_PaginaNoticia : System.Web.UI.Page
{
    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_PaginaNoticia));

    ControlBD _controlBD;    
   
    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;

        _controlBD = new ControlBD();

        Repeater1.DataSource = _controlBD.ObtenerDatosFiltrados(@"select top 1 contenido.idContenido,
                                  CONVERT(varchar, contenido.fecha, 102) as solfecha,contenido.titulo,contenido.tipo,contenido.importante,
								(select b.html from contenido b where b.idContenido = contenido.idContenido) as html   
                                from contenido
                                INNER JOIN (contenido_marca INNER JOIN (marca INNER JOIN (concesionarioMarca INNER JOIN (concesionario INNER JOIN (sucursal INNER JOIN persona ON sucursal.shipcode=persona.shipcode)
                                ON concesionario.nombreConcesionario=sucursal.nombreConcesionario)
                                ON concesionarioMArca.nombreConcesionario=concesionario.nombreConcesionario)
                                ON concesionarioMArca.nombreMArca= marca.nombreMarca) 
                                ON contenido_marca.nombreMarca=marca.nombreMarca)
                                ON contenido.idContenido=contenido_marca.idContenido
                                AND contenido.tipo=1 " +
                                " AND persona.rut=" + Session["rut"].ToString() +
                                " group by contenido.idContenido,contenido.fecha,contenido.titulo,contenido.tipo,contenido.importante " +
                                " order by idContenido desc ");
        Repeater1.DataBind();
/*
        Repeater2.DataSource = _controlBD.ObtenerDatosFiltrados(@"select contenido.idContenido,
                                CONVERT(varchar, contenido.fecha, 102) as solfecha,contenido.html,contenido.titulo,contenido.tipo,contenido.importante
                                from contenido
                                INNER JOIN (contenido_marca INNER JOIN (marca INNER JOIN (concesionarioMarca INNER JOIN (concesionario INNER JOIN (sucursal INNER JOIN persona ON sucursal.shipcode=persona.shipcode)
                                ON concesionario.nombreConcesionario=sucursal.nombreConcesionario)
                                ON concesionarioMArca.nombreConcesionario=concesionario.nombreConcesionario)
                                ON concesionarioMArca.nombreMArca= marca.nombreMarca) 
                                ON contenido_marca.nombreMarca=marca.nombreMarca)
                                ON contenido.idContenido=contenido_marca.idContenido
                                AND contenido.tipo=1 " +
                                " AND persona.rut=" + Session["rut"].ToString() +
                                " group by contenido.idContenido,contenido.fecha,contenido.html,contenido.titulo,contenido.tipo,contenido.importante " +
                                " order by idContenido desc ");
        Repeater2.DataBind();*/

        if (int.Parse(Session["permisos"].ToString()) != 1 && int.Parse(Session["permisos"].ToString()) != 6)
        {
            AgregarNoticia.Visible = false;
            
         } 
		
		/*sqldNoticias.SelectCommand  = "select contenido.idContenido,CONVERT(varchar, contenido.fecha, 102) as solfecha,contenido.html,contenido.titulo as titulo,contenido.tipo"
						+",contenido.importante from contenido INNER JOIN (contenido_marca INNER JOIN (marca INNER JOIN (concesionarioMarca INNER JOIN (concesionario INNER JOIN (sucursal INNER JOIN persona ON sucursal.shipcode=persona.shipcode)"
                             +  " ON concesionario.nombreConcesionario=sucursal.nombreConcesionario)"
                             +   "ON concesionarioMArca.nombreConcesionario=concesionario.nombreConcesionario)"
                             +   "ON concesionarioMArca.nombreMArca= marca.nombreMarca) "
                             +   "ON contenido_marca.nombreMarca=marca.nombreMarca)"
                             +   "ON contenido.idContenido=contenido_marca.idContenido"
                             +  " AND contenido.tipo=1 " 
                             +   " AND persona.rut=" + Session["rut"].ToString() +
                                " group by contenido.idContenido,contenido.fecha,contenido.html,contenido.titulo,contenido.tipo,contenido.importante " +
                                " order by idContenido desc ";		*/			
		sqldNoticias.SelectCommand  = "select contenido.idContenido,CONVERT(varchar, contenido.fecha, 102) as solfecha,contenido.titulo as titulo,contenido.tipo"
							 +",contenido.importante, (select b.html from contenido b where b.idContenido = contenido.idContenido) as html "
							 + "from contenido INNER JOIN (contenido_marca INNER JOIN (marca INNER JOIN (concesionarioMarca INNER JOIN (concesionario INNER JOIN (sucursal INNER JOIN persona ON sucursal.shipcode=persona.shipcode)"
                             +  " ON concesionario.nombreConcesionario=sucursal.nombreConcesionario)"
                             +   "ON concesionarioMArca.nombreConcesionario=concesionario.nombreConcesionario)"
                             +   "ON concesionarioMArca.nombreMArca= marca.nombreMarca) "
                             +   "ON contenido_marca.nombreMarca=marca.nombreMarca)"
                             +   "ON contenido.idContenido=contenido_marca.idContenido"
                             +  " AND contenido.tipo=1 " 
                             +   " AND persona.rut='" + Session["rut"].ToString() +
                                "' group by contenido.idContenido,contenido.fecha,contenido.titulo,contenido.tipo,contenido.importante " +
                                " order by idContenido desc ";	
		gridNoticias.DataBind();					
		
    }

    protected void Repeater2_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        string idCont = e.CommandArgument.ToString();
        _controlBD.MostrarContenido(idCont);
        Response.Redirect("VerMas.aspx?valor='" + idCont + "'");
    }

    protected void Repeater1_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        string idCont2 = e.CommandArgument.ToString();
        _controlBD.MostrarContenido(idCont2);
        Response.Redirect("VerMas.aspx?valor='" + idCont2 + "'");
    }
    protected void limitar(object sender, RepeaterItemEventArgs e)
    {
        if ((sender as Repeater).Items.Count > 3)
        {
            (sender as Repeater).Controls.RemoveAt(3);
        }
    }
}