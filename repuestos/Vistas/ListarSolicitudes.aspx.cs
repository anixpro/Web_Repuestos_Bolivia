using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Diagnostics;
using System.IO;
using log4net;
using log4net.Config;
using System.Xml.Linq;
using SaveVFC = pe.com.skberge.crm.SaveVFC; //cl.humano2.skbergecl.SaveVFC;
using SaveVFC2 = QAS.PERU.SaveVFC;// pe.com.skberge.crm.SaveVFC;
using ConsultaVFC = pe.humano2.skberge.ConsultaVFC;
using System.Configuration;
using System.Data;//com.skbergechile.crm.ConsultaVFC;


public partial class Vistas_ListarSolicitudes : System.Web.UI.Page
{
    private ControlVfc controlVfc;
    List<registroSolicitud> registros;
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_ListarSolicitudes));
    private SaveVFC saveVfc;
    private SaveVFC2 saveVFC2;
    private ConsultaVFC consultaVFC;
    private RutHelper _rutHelper;
    ControlBDSolicitud _ControlBD = new ControlBDSolicitud();
    SendMail_helper _mail = new SendMail_helper();

    protected void Page_PreInit(object sender, EventArgs e)
    {
        if (Session["permisos"] == null)
        {
            Response.Redirect("../index.aspx?evento=ev1");
        }
    }

    protected void Page_Init(object sender, EventArgs e)
    {

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;
        trRut.Visible = false;
        lblTexto.Visible = false;
        msjesError.Visible = false;
        gvSolicitudes.Visible = true;
        btnExcel.Visible = false;
        pnlCriticidad.Visible = false;
        try
        {
            gvSolicitudes.PageIndexChanging += new GridViewPageEventHandler(gvSolicitudes_PageIndexChanging);
            gvSolicitudes.PagerSettings.Mode = PagerButtons.NextPreviousFirstLast;
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Problemas al paginar la grilla. Contacte al administrador";
            msjesError.Visible = true;
            logger.Error("Problemas al paginar la grilla en Page Load. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
        }

        if (Session["permisos"].ToString() == "1")
        {
            trRut.Visible = true;
            trRutColegas.Visible = false;
        }

        DataSet DsCrit = _ControlBD.ObtenerDatosFiltrados("SELECT nombre, criticidad FROM persona WHERE rut = '" + Session["rut"].ToString() + "'");
        //string criticidad = "";
        var nombre = "";
        foreach (DataRow campo in DsCrit.Tables[0].Rows)
        {
            Session["Criticidad"] = campo["criticidad"].ToString();
            nombre = campo["nombre"].ToString();
        }
    }

    void gvSolicitudes_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvSolicitudes.PageIndex = e.NewPageIndex;
        busqueda();
    }
    protected void btnBuscar_Click(object sender, EventArgs e)
    {
        gvSolicitudes.PageIndex = 0;
        busqueda();
    }

    private void busqueda()
    {
        btnExcel.Visible = true;
        hiddenType.Value = "1";
        controlVfc = new ControlVfc();

        String fechaInicioBruta = txtFechaBusquedaInicio.Text;
        String fechaFinBruta = txtFechaBusquedaFin.Text;
        String fechaInicio = fechaInicioBruta.Replace("-", "");
        String fechaFin = fechaFinBruta.Replace("-", "");
        String tipoVenta = ddTipoVenta.SelectedValue;
        String estado = ddEstado.SelectedValue;
        String tipoPedido = ddTipoPedido.SelectedValue;

        if (fechaInicio == "" || fechaFin == "")
        {
            msjesError.Visible = true;
            msjesError.InnerText = "Fecha inválida";
            return;
        }
        char[] separador = new char[1];
        separador[0] = '-';
        DateTime dtFechaInicio = new DateTime(
                int.Parse(fechaInicioBruta.Split(separador)[0]),
                int.Parse(fechaInicioBruta.Split(separador)[1]),
                int.Parse(fechaInicioBruta.Split(separador)[2])
            );
        DateTime dtFechaFin = new DateTime(
                int.Parse(fechaFinBruta.Split(separador)[0]),
                int.Parse(fechaFinBruta.Split(separador)[1]),
                int.Parse(fechaFinBruta.Split(separador)[2])
            );
        if (dtFechaFin < dtFechaInicio)
        {
            msjesError.Visible = true;
            msjesError.InnerText = "La fecha final debe ser mayor que la fecha incial";
            return;
        }
        try
        {
            // Si usuario es administrador puede hacer busquedas exactas
            if (trRut.Visible)
            {
                registros = controlVfc.getSolicitudesRepuestos(txtRut.Text, fechaInicio, fechaFin, "", "", estado, tipoVenta, tipoPedido);
            }
            // Usuario es operador/cotizador/gerente
            else
            {
                // Usuario debe seleccionar un operador de ddl
                if (ddlOperarios.SelectedIndex < 0)
                {
                    msjesError.InnerText = "Debe seleccionar un operario, por favor";
                    return;
                }
                RutHelper rh = new RutHelper();
                String rEnetero = rh.GetRutConDigito(ddlOperarios.SelectedValue);
                msjesError.InnerText = "Resultados de VFCs y reservas para " + rEnetero;
                msjesError.Visible = true;
                registros = controlVfc.getSolicitudesRepuestos(rEnetero, fechaInicio, fechaFin, "", "", estado, tipoVenta, tipoPedido);
            }

            //Donde el "tipo" sea Normal, se debe mostar VFC
            foreach (registroSolicitud registro in registros)
            {
                if (registro.TipoPedido == "NORMAL")
                {
                    registros[registros.IndexOf(registro)].TipoPedido = "VFC";
                }
            }

            // Se filtra segun marca seleccionada
            if (ddMarcas.SelectedValue != "")
            {
                registros.RemoveAll(filtroPorMarca);
            }
            if (registros.Count == 0)
            {
                msjesError.Visible = true;
                msjesError.InnerText = "No hubo resultados de la consulta, modifique filtros";
                lblTexto.Visible = false;
                btnExcel.Visible = false;
            }
        }
        catch (System.Web.Services.Protocols.SoapException ex)
        {
            msjesError.Visible = true;
            msjesError.InnerText = "Hubo problemas de conexión. Intente más tarde. (" + ex.Message + ")";
            gvSolicitudes.Visible = false;
            btnExcel.Visible = false;
            logger.Error("Problemas de conexion. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
            return;
        }
        catch (Exception ex)
        {
            msjesError.Visible = true;
            msjesError.InnerText = "No hubo resultados de la consulta para este usuario";
            gvSolicitudes.Visible = false;
            btnExcel.Visible = false;
            logger.Error("Error al buscar resultados. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
            return;
        }
        try
        {
            gvSolicitudes.DataSource = registros;
            gvSolicitudes.DataBind();
        }
        catch (Exception ex)
        {
            msjesError.Visible = true;
            msjesError.InnerText = "Hubo un problema al enlazar los datos. Disculpe las molestias";
            logger.Error("Problema al enlazar los datos. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
        }
        lblTexto.Visible = true;
        lblTexto.Text = "Mostrando página " + (gvSolicitudes.PageIndex + 1).ToString() + " de " + gvSolicitudes.PageCount;
    }
    protected void btnBuscarExacto_Click(object sender, EventArgs e)
    {
        ModeloConsultaRepuestos Repuestos = new ModeloConsultaRepuestos();
        hiddenType.Value = "0";
        string id = txtIdSolicitud.Text;

        try
        {
            controlVfc = new ControlVfc();
            gvBusquedaExacta.Caption = "Detalle solicitud " + id;
            gvBusquedaExacta.DataSource = controlVfc.getDetalleSolicitud(id);
            gvBusquedaExacta.DataBind();

            bool Flagcritico = Convert.ToBoolean(Session["Criticidad"].ToString());
            if (Flagcritico)
            {
                pnlCriticidad.Visible = true;
            }

            //Nuevo BLoque Criticidad VFC
            DateTime fecha = new DateTime();

            string feha = DateTime.Now.ToString("yyyy-MM-dd");

            Repuestos.solicitudes = getSolicitudesRepuestos(crearXmlSolicitudRepuestos("", "2015-01-01", feha, "", "", "", "", "", id)).solicitudes;

            string critico = "";
            string obs = "";

            foreach (var nodo in Repuestos.solicitudes)
            {
                    //<critico>1</critico>
                    critico = nodo.critico.ToString();

                    //<obsvfc>PRUEBA VFC2</obsvfc>
                    obs = nodo.obsVFC.ToString();

            }

            if (critico == "")
            {
                critico = "0";
            }
            drCriticidad.SelectedValue = critico;
            txtObservacion.Text = obs;
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "No se encontró resultados" + ex.Message;
        }
    }

    private bool filtroPorMarca(registroSolicitud registro)
    {
        if (registro.Marca != ddMarcas.SelectedValue)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected void btnExcel_Click(object sender, EventArgs e)
    {
        busqueda();
        try
        {
            int num = 1;
            String nombreArchivo = Server.MapPath("..\\doc\\" + Session["idSession"].ToString());
            String nombreArchivoOriginal = nombreArchivo;
            String nombreArchivoConExtension = Server.MapPath("..\\doc\\" + Session["idSession"].ToString()) + ".xlsx";
            while (File.Exists(nombreArchivoConExtension))
            {
                nombreArchivo = nombreArchivoOriginal + "[" + num.ToString() + "]";
                nombreArchivoConExtension = nombreArchivo + ".xlsx";
                num++;
            }
            Csv csv = new Csv();
            csv.crearCsv(registros, nombreArchivo);
            Response.ContentType = "application/vnd.ms-excel.12application/x-font";
            Response.AppendHeader("Content-Disposition", "attachment; filename=resumenVFCyReservas.xlsx");
            Response.TransmitFile(nombreArchivoConExtension);
            Response.End();
        }
        catch (Exception ex)
        {
            msjesError.InnerText = "Error al escribir archivo Excel. Detalle técnico:" + ex.Message;
            logger.Error("Error al escribir archivo Excel. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
        }
    }

    private ModeloConsultaRepuestos getSolicitudesRepuestos(string xmlDatosEntrada)
    {
        ModeloConsultaRepuestos Consulta = null;

        try
        {//no existe ws para consultar vfc en qa, solo productivo
            SeguimientoPedidoFiltro SeguminetoPerdido = new SeguimientoPedidoFiltro();
            Consulta = SeguminetoPerdido.SeguimientoPedidoFiltroConsulta(xmlDatosEntrada);

            return Consulta;
        }
        catch (Exception ex)
        {
            return Consulta;
        }
    }

    private String crearXmlSolicitudRepuestos(String usuario, String fechaDesde, String fechaHasta,
                                                String concesionario, String local, String estado,
                                                String opcionvfc, String tipoPedido, String id)
    {
        XDocument miXML = new XDocument(
        new XDeclaration("1.0", "utf-8", "yes"),
        //new XComment("Lista de Registros"),
        new XElement("registros",
                            new XElement("registro",
                                //new XElement("usuario", usuario),
                                new XElement("fechaDesde", fechaDesde),
                                new XElement("fechaHasta", fechaHasta)
                                //new XElement("concesionario", concesionario),
                                //new XElement("local", local),
                                //new XElement("estado", estado),
                                //new XElement("opcionvfc", opcionvfc),
                                //new XElement("tipoPedido", tipoPedido),
                                //new XElement("id_solicitud", id)
                            )
                    )
               );
        return miXML.ToString();

    }

    protected void btnCriticidad_Click(object sender, EventArgs e)
    {
        SendMail_helper _mail = new SendMail_helper();

        String datosXml = "";
        datosXml = CambiaCriticidad(crearXml(txtIdSolicitud.Text, drCriticidad.SelectedValue, txtObservacion.Text), "userVFC", "us3rVF").ToString();
        XmlDocument xDoc = new XmlDocument();
        if (datosXml == "")
        {
            return;
        }
        //leo el xml en formato String con loadXml
        xDoc.LoadXml(datosXml);

        try
        {

            string query5, query6, query1;
            String textoCorreo = "";
            textoCorreo = "Estimados, Se han modificado el siguiente VFC críticos: ";
            textoCorreo += "<br/><br/>Detalle :";



            string sociedad = "";
            string codigo = "";

            for (int j = 0; j < gvBusquedaExacta.Rows.Count; j++)
            {
                GridViewRow row = gvBusquedaExacta.Rows[j];
                sociedad = row.Cells[1].Text;
                codigo = row.Cells[2].Text;
                break;
            }

            string corMar = "";

            query1 = "SELECT marca FROM VFC WHERE num_vfc = '" + txtIdSolicitud.Text + "'";
            string marca = _ControlBD.marcaVFC(query1);

            query5 = "SELECT STUFF((SELECT CAST(';' AS VARCHAR(MAX)) + emaiilPersona_vfc FROM dbo.Personas_Email_VFC where orgpersona_vfc = 'X' ORDER BY idPersona_vfc FOR XML PATH('')), 1, 1, '') AS Correos";
            string corAnal = _ControlBD.EmailCriticidad(query5);

            string msjCriticoMail = "";
            if (drCriticidad.SelectedValue == "1")
            {
                msjCriticoMail = "CRITICO";
            }

            textoCorreo += "<br/> 1. | VFC | " + txtIdSolicitud.Text + " | " + sociedad + " | " + codigo + " | " + msjCriticoMail + "|<br/>";

            //Si VFC no fue creado via web de repuestos
            if (marca != "")
            {
                query6 = "SELECT STUFF((SELECT CAST(',' AS VARCHAR(MAX)) + b.emaiilPersona_vfc FROM marca A INNER JOIN Personas_Email_VFC B ON a.orgventas = b.orgPersona_vfc WHERE substring(a.abreviado,1,2) = '" + marca + "' FOR XML PATH('')), 1, 1, '') AS Correos";
                corMar = _ControlBD.EmailCriticidad(query6);
            }
            else
            {
                query6 = "SELECT STUFF((SELECT CAST(',' AS VARCHAR(MAX)) + b.emaiilPersona_vfc FROM marca A INNER JOIN Personas_Email_VFC B ON a.orgventas = b.orgPersona_vfc WHERE a.orgventas = '" + sociedad + "' FOR XML PATH('')), 1, 1, '') AS Correos";
                corMar = _ControlBD.EmailCriticidad(query6);
            }


            query1 = "SELECT email FROM persona WHERE rut = '" + Session["rut"].ToString() + "'";
            string para = _ControlBD.corrreoVFC(query1);


            _mail.EnviarCorreo2VFc(para, "VFC Críticos modificados: ", textoCorreo, "");

            _mail.EnviarCorreo2VFc(corAnal, "VFC Críticos modificados: " + txtIdSolicitud.Text, textoCorreo, corMar);


        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), ConfigurationManager.AppSettings["asunto_error"].ToString(), "En [Envia_Correo_Criticidad_Modificacion] Message: " + ex.Message + " Inner: " + ex.InnerException + " Stack: " + ex.StackTrace);
        }

    }

    private string CambiaCriticidad(String xmlDatosAEnviar, String userVFC, String us3rVF)
    {
        string ambiente = "";
        ambiente = ConfAmbiente.ambiente;
        if (ambiente == "ERP")
        {
            saveVfc = new SaveVFC();

            return saveVfc.updVFC(xmlDatosAEnviar, userVFC, us3rVF);
        }
        else if (ambiente == "ERQ")
        {
            saveVFC2 = new SaveVFC2();

            return saveVFC2.updVFC(xmlDatosAEnviar, userVFC, us3rVF);
        }
        else
        {
            return "";
        }
        
        
    }

    private String crearXml(string id, string criticidad, string observacion)
    {

        XDocument miXML = new XDocument(
        new XDeclaration("1.0", "utf-8", "yes"),
        new XComment("Lista de Registros"),
        new XElement("registros",
                            new XElement("registro",
                                 new XElement("idvfc", id),
                                new XElement("critico", criticidad),
                                new XElement("obsvfc", observacion)

                                //new XElement("nrocotiza",)
                            )
                    )
               );
        return miXML.ToString();

    }
}
