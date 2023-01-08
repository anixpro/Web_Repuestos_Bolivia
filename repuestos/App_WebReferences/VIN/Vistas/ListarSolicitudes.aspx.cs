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

public partial class Vistas_ListarSolicitudes : System.Web.UI.Page
{
    private ControlVfc controlVfc;
    List<registroSolicitud> registros;
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_ListarSolicitudes));

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
            msjesError.InnerText = "Hubo problemas de conexión. Intente más tarde. ("+ex.Message+")";
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
        hiddenType.Value = "0";
        string id = txtIdSolicitud.Text;
        try
        {
            controlVfc = new ControlVfc();
            gvBusquedaExacta.Caption = "Detalle solicitud " + id;
            gvBusquedaExacta.DataSource = controlVfc.getDetalleSolicitud(id);
            gvBusquedaExacta.DataBind();
        } catch(Exception ex){
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
}
