using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Configuration;
using OfficeOpenXml;

/// <summary>
/// Summary description for Csv
/// </summary>
public class Csv
{

    public String rutaFinal { get; set; }

    public Csv()
	{
        rutaFinal = "";
	}

    public void crearCsv(List<registroSolicitud> detalleSol,String rutaArchivo)
    {
        String fichero = rutaArchivo + ".xlsx";

        if (File.Exists(fichero))
        {
            File.Delete(fichero);
        }
        this.rutaFinal = fichero;
        FileInfo newFile = new FileInfo(fichero);
        using (ExcelPackage xlPackage = new ExcelPackage(newFile))
        {
            ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add("Lista Solicitudes");
            // Titulos de cada
            //SOLICITUD;PRODUCTO;VIN;MARCA;FECHA;SUCURSAL;DIRECCION;
            //TIPO PEDIDO;ESTADO;CANTIDAD;CONCESIONARIO;CREADOR;DEALER;USUARIO;CREACION;TIPO SOLICITUD;OBSERVACION
            worksheet.Cell(1, 1).Value = "SOLICITUD";
            worksheet.Column(1).Width = 10;
            worksheet.Cell(1, 2).Value = "PRODUCTO";
            worksheet.Column(2).Width = 15;
            worksheet.Cell(1, 3).Value = "VIN";
            worksheet.Column(3).Width = 20;
            worksheet.Cell(1, 4).Value = "MARCA";
            worksheet.Column(4).Width = 15;
            worksheet.Cell(1, 5).Value = "FECHA";
            worksheet.Column(5).Width = 15;
            worksheet.Cell(1, 6).Value = "SUCURSAL";
            worksheet.Column(6).Width = 15;
            worksheet.Cell(1, 7).Value = "DIRECCION";
            worksheet.Column(7).Width = 30;
            worksheet.Cell(1, 8).Value = "TIPO PEDIDO";
            worksheet.Column(8).Width = 15;
            worksheet.Cell(1, 9).Value = "ESTADO";
            worksheet.Column(9).Width = 15;
            worksheet.Cell(1, 10).Value = "CANTIDAD";
            worksheet.Column(10).Width = 12;
            worksheet.Cell(1, 11).Value = "CONCESIONARIO";
            worksheet.Column(11).Width = 30;
            worksheet.Cell(1, 12).Value = "CREADOR";
            worksheet.Column(12).Width = 15;
            worksheet.Cell(1, 13).Value = "DEALER";
            worksheet.Column(13).Width = 20;
            worksheet.Cell(1, 14).Value = "USUARIO";
            worksheet.Column(14).Width = 25;
            worksheet.Cell(1, 15).Value = "CREACION";
            worksheet.Column(15).Width = 15;
            worksheet.Cell(1, 16).Value = "TIPO SOLICITUD";
            worksheet.Column(16).Width = 15;
            worksheet.Cell(1, 17).Value = "OBSERVACIÓN";
            worksheet.Column(17).Width = 50;

            int row = 2;

            foreach (registroSolicitud detalle in detalleSol)
            {
                worksheet.Cell(row, 1).Value = detalle.Id_solicitud;
                worksheet.Cell(row, 2).Value = detalle.Producto;
                worksheet.Cell(row, 3).Value = detalle.Vin;
                worksheet.Cell(row, 4).Value = detalle.Marca;
                worksheet.Cell(row, 5).Value = detalle.Fecha_eta;
                worksheet.Cell(row, 6).Value = detalle.Local;
                worksheet.Cell(row, 7).Value = detalle.Direccion;
                worksheet.Cell(row, 8).Value = detalle.TipoPedido;
                worksheet.Cell(row, 9).Value = detalle.Estado;
                worksheet.Cell(row, 10).Value = detalle.Cantidad;
                worksheet.Cell(row, 11).Value = detalle.Concesionario;
                worksheet.Cell(row, 12).Value = detalle.Creador;
                worksheet.Cell(row, 13).Value = detalle.Dealer;
                worksheet.Cell(row, 14).Value = detalle.Desc_usuario;
                worksheet.Cell(row, 15).Value = detalle.Fecha_creacion;
                worksheet.Cell(row, 16).Value = detalle.Tipovfc;
                worksheet.Cell(row, 15).Value = detalle.Observacion;
                row++;
            }

            xlPackage.Workbook.Properties.Title = "Lista Solicitudes";
            xlPackage.Workbook.Properties.Author = "SKBerge Centro Repuestos";
            xlPackage.Save();

            //worksheet.HeaderFooter.oddHeader.CenteredText = "Tinned Goods Sales";
            // add the page number to the footer plus the total number of pages
            //worksheet.HeaderFooter.oddFooter.RightAlignedText =
            //string.Format("Page {0} of {1}", ExcelHeaderFooter.PageNumber,ExcelHeaderFooter.NumberOfPages);
        }
    }

    public void crearCsv(List<IndicadorMetas> indicadores, String rutaArchivo)
    {
        String fichero = rutaArchivo + ".xlsx";

        if (File.Exists(fichero))
        {
            File.Delete(fichero);
        }
        this.rutaFinal = fichero;
        FileInfo newFile = new FileInfo(fichero);
        using (ExcelPackage xlPackage = new ExcelPackage(newFile))
        {
            ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add("Lista Solicitudes");
            // Titulos de cada
            worksheet.Cell(1, 1).Value = "CONCESIONARIO";
            worksheet.Column(1).Width = 10;
            worksheet.Cell(1, 2).Value = "MARCA";
            worksheet.Column(2).Width = 15;
            worksheet.Cell(1, 3).Value = "AÑO";
            worksheet.Column(3).Width = 20;
            worksheet.Cell(1, 4).Value = "MES";
            worksheet.Column(4).Width = 15;
            worksheet.Cell(1, 5).Value = "COMPRAS";
            worksheet.Column(5).Width = 15;
            worksheet.Cell(1, 6).Value = "METAS";
            worksheet.Column(6).Width = 15;

            int row = 2;

            foreach (IndicadorMetas indicador in indicadores)
            {
                worksheet.Cell(row, 1).Value = indicador.Concesionario.Nombre;
                worksheet.Cell(row, 2).Value = indicador.Marca.NomMarca;
                worksheet.Cell(row, 3).Value = indicador.Anio.ToString();
                worksheet.Cell(row, 4).Value = indicador.MesExtenso;
                worksheet.Cell(row, 5).Value = indicador.Compras.ToString();
                worksheet.Cell(row, 6).Value = indicador.Metas.ToString();
                row++;
            }

            xlPackage.Workbook.Properties.Title = "Indicadores ventas";
            xlPackage.Workbook.Properties.Author = "SKBerge Centro Repuestos";
            xlPackage.Save();

            //worksheet.HeaderFooter.oddHeader.CenteredText = "Tinned Goods Sales";
            // add the page number to the footer plus the total number of pages
            //worksheet.HeaderFooter.oddFooter.RightAlignedText =
            //string.Format("Page {0} of {1}", ExcelHeaderFooter.PageNumber,ExcelHeaderFooter.NumberOfPages);
        }
    }

    public void crearCsv(List<IndicadorEstadisticaDeCompra> indicadores, String rutaArchivo)
    {
        String fichero = rutaArchivo + ".xlsx";

        if (File.Exists(fichero))
        {
            File.Delete(fichero);
        }
        this.rutaFinal = fichero;
        FileInfo newFile = new FileInfo(fichero);
        using (ExcelPackage xlPackage = new ExcelPackage(newFile))
        {
            ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add("Lista Solicitudes");
            // Titulos de cada
            worksheet.Cell(1, 1).Value = "COTIZACION";
            worksheet.Column(1).Width = 15;
            worksheet.Cell(1, 2).Value = "CLIENTE";
            worksheet.Column(2).Width = 15;
            worksheet.Cell(1, 3).Value = "DEST MERCANCIA";
            worksheet.Column(3).Width = 20;
            worksheet.Cell(1, 4).Value = "RUT";
            worksheet.Column(4).Width = 15;
            worksheet.Cell(1, 5).Value = "CODIGO";
            worksheet.Column(5).Width = 15;
            worksheet.Cell(1, 6).Value = "DESCRIPCION";
            worksheet.Column(6).Width = 15;
            worksheet.Cell(1, 7).Value = "MARCA";
            worksheet.Column(7).Width = 15;
            worksheet.Cell(1, 8).Value = "CANTIDAD";
            worksheet.Column(8).Width = 15;
            worksheet.Cell(1, 9).Value = "VALOR";
            worksheet.Column(9).Width = 15;
            worksheet.Cell(1, 10).Value = "TOTAL";
            worksheet.Column(10).Width = 15;

            int row = 2;

            foreach (IndicadorEstadisticaDeCompra indicador in indicadores)
            {
                // "COTIZACION"
                worksheet.Cell(row, 1).Value = indicador.Cotizacion;
                    // "CLIENTE"
                worksheet.Cell(row, 2).Value = indicador.Cliente;
                    // "DEST MERCANCIA"
                worksheet.Cell(row, 3).Value = indicador.DestinatarioMercancia;
                    // "RUT"
                worksheet.Cell(row, 4).Value = indicador.RutSolicitante;
                    // "CODIGO"
                worksheet.Cell(row, 5).Value = indicador.Codigo;
                    // "DESCRIPCION"
                worksheet.Cell(row, 6).Value = indicador.Descipcion;
                    // "MARCA"
                worksheet.Cell(row, 7).Value = indicador.Marca;
                    // "CANTIDAD"
                worksheet.Cell(row, 8).Value = indicador.Cantidad;
                    // "VALOR"
                worksheet.Cell(row, 9).Value = indicador.Valor;
                    // "TOTAL"
                worksheet.Cell(row, 10).Value = indicador.Total;
                row++;
            }

            xlPackage.Workbook.Properties.Title = "Indicadores ventas";
            xlPackage.Workbook.Properties.Author = "SKBerge Centro Repuestos";
            xlPackage.Save();

            //worksheet.HeaderFooter.oddHeader.CenteredText = "Tinned Goods Sales";
            // add the page number to the footer plus the total number of pages
            //worksheet.HeaderFooter.oddFooter.RightAlignedText =
            //string.Format("Page {0} of {1}", ExcelHeaderFooter.PageNumber,ExcelHeaderFooter.NumberOfPages);
        }
    }

    public void crearCsv(List<IndicadorEvolucionCompra> indicadores, String rutaArchivo)
    {
        String fichero = rutaArchivo + ".xlsx";

        if (File.Exists(fichero))
        {
            File.Delete(fichero);
        }
        this.rutaFinal = fichero;
        FileInfo newFile = new FileInfo(fichero);
        using (ExcelPackage xlPackage = new ExcelPackage(newFile))
        {
            ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add("Lista Solicitudes");
            // Titulos de cada
            worksheet.Cell(1, 1).Value = "MARCA";
            worksheet.Column(1).Width = 15;
            worksheet.Cell(1, 2).Value = "CONCESIONARIO";
            worksheet.Column(2).Width = 15;
            worksheet.Cell(1, 3).Value = "COMPRAS";
            worksheet.Column(3).Width = 20;
            worksheet.Cell(1, 4).Value = "MES";
            worksheet.Column(4).Width = 15;
            worksheet.Cell(1, 5).Value = "ANIO";
            worksheet.Column(5).Width = 15;

            int row = 2;

            foreach (IndicadorEvolucionCompra indicador in indicadores)
            {
                // "MARCA"
                worksheet.Cell(row, 1).Value = indicador.Marca;
                // "CONCESIONARIO"
                worksheet.Cell(row, 2).Value = indicador.Concesionario;
                // "COMPRAS"
                worksheet.Cell(row, 3).Value = indicador.Compras.ToString();
                // "MES"
                worksheet.Cell(row, 4).Value = indicador.Mes;
                // "AÑO"
                worksheet.Cell(row, 5).Value = indicador.Anio;
                row++;
            }

            xlPackage.Workbook.Properties.Title = "Indicadores ventas";
            xlPackage.Workbook.Properties.Author = "SKBerge Centro Repuestos";
            xlPackage.Save();

            //worksheet.HeaderFooter.oddHeader.CenteredText = "Tinned Goods Sales";
            // add the page number to the footer plus the total number of pages
            //worksheet.HeaderFooter.oddFooter.RightAlignedText =
            //string.Format("Page {0} of {1}", ExcelHeaderFooter.PageNumber,ExcelHeaderFooter.NumberOfPages);
        }
    }

    public void crearCsv(List<indicadorFallidas> indicadores, String rutaArchivo)
    {
        String fichero = rutaArchivo + ".xlsx";

        if (File.Exists(fichero))
        {
            File.Delete(fichero);
        }
        this.rutaFinal = fichero;
        FileInfo newFile = new FileInfo(fichero);
        using (ExcelPackage xlPackage = new ExcelPackage(newFile))
        {
            ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add("Lista Solicitudes");
            // Titulos de cada
            worksheet.Cell(1, 1).Value = "TIPO";
            worksheet.Column(1).Width = 15;
            worksheet.Cell(1, 2).Value = "IDENTIFICADOR";
            worksheet.Column(2).Width = 15;
            worksheet.Cell(1, 3).Value = "RUT";
            worksheet.Column(3).Width = 20;
            worksheet.Cell(1, 4).Value = "FECHA SOLICITUD";
            worksheet.Column(4).Width = 15;
            worksheet.Cell(1, 5).Value = "MARCA";
            worksheet.Column(5).Width = 15;
            worksheet.Cell(1, 6).Value = "CANTIDAD";
            worksheet.Column(6).Width = 15;
            worksheet.Cell(1, 7).Value = "DESCRIPCION";
            worksheet.Column(7).Width = 15;
            worksheet.Cell(1, 8).Value = "COD REPUESTO";
            worksheet.Column(8).Width = 15;
            worksheet.Cell(1, 9).Value = "VIN";
            worksheet.Column(9).Width = 15;

            int row = 2;

            foreach (indicadorFallidas indicador in indicadores)
            {
                //"TIPO";
                worksheet.Cell(row, 1).Value = indicador.Tipo;
                //"IDENTIFICADOR";
                worksheet.Cell(row, 2).Value = indicador.Identificador;
                //"RUT";
                worksheet.Cell(row, 3).Value = indicador.RutSolicitante;
                //"FECHA SOLICITUD";
                worksheet.Cell(row, 4).Value = indicador.FechaSolicitud;
                //"MARCA";
                worksheet.Cell(row, 5).Value = indicador.Marca;
                //"CANTIDAD";
                worksheet.Cell(row, 6).Value = indicador.Cantidad;
                //"DESCRIPCION";
                worksheet.Cell(row, 7).Value = indicador.DetalleRepuesto;
                //"COD REPUESTO";
                worksheet.Cell(row, 8).Value = indicador.Repuesto;
                //"VIN";
                worksheet.Cell(row, 9).Value = indicador.VIN;

                row++;
            }

            xlPackage.Workbook.Properties.Title = "Indicadores ventas";
            xlPackage.Workbook.Properties.Author = "SKBerge Centro Repuestos";
            xlPackage.Save();

            //worksheet.HeaderFooter.oddHeader.CenteredText = "Tinned Goods Sales";
            // add the page number to the footer plus the total number of pages
            //worksheet.HeaderFooter.oddFooter.RightAlignedText =
            //string.Format("Page {0} of {1}", ExcelHeaderFooter.PageNumber,ExcelHeaderFooter.NumberOfPages);
        }
    }

    public void crearCsv(List<IndicadorCotizacionCompra> indicadores, String rutaArchivo)
    {
        String fichero = rutaArchivo + ".xlsx";

        if (File.Exists(fichero))
        {
            File.Delete(fichero);
        }
        this.rutaFinal = fichero;
        FileInfo newFile = new FileInfo(fichero);
        using (ExcelPackage xlPackage = new ExcelPackage(newFile))
        {
            ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add("Lista Solicitudes");

            // Titulos de cada
            worksheet.Cell(1, 1).Value = "IDENTIFICADOR";
            worksheet.Column(1).Width = 15;
            worksheet.Cell(1, 2).Value = "NUM COTIZACION";
            worksheet.Column(2).Width = 15;
            worksheet.Cell(1, 3).Value = "COD CLIENTE";
            worksheet.Column(3).Width = 20;
            worksheet.Cell(1, 4).Value = "DEST MERCANCIA";
            worksheet.Column(4).Width = 15;
            worksheet.Cell(1, 5).Value = "CLASE DOC VTAS";
            worksheet.Column(5).Width = 15;
            worksheet.Cell(1, 6).Value = "SOLICITANTE";
            worksheet.Column(6).Width = 15;
            worksheet.Cell(1, 7).Value = "FECHA SOLICITUD";
            worksheet.Column(7).Width = 15;
            worksheet.Cell(1, 8).Value = "TOTAL NETO";
            worksheet.Column(8).Width = 15;
            worksheet.Cell(1, 9).Value = "NUM PEDIDO";
            worksheet.Column(9).Width = 15;
            worksheet.Cell(1, 10).Value = "TIPO PEDIDO";
            worksheet.Column(10).Width = 15;
            worksheet.Cell(1, 11).Value = "PRIORIDAD";
            worksheet.Column(11).Width = 15;

            int row = 2;

            foreach (IndicadorCotizacionCompra indicador in indicadores)
            {
                //"IDENTIFICADOR";
                worksheet.Cell(row, 1).Value = indicador.Indentificador;
                //"NUM COTIZACION";
                worksheet.Cell(row, 2).Value = indicador.NumCotizacion;
                //"COD CLIENTE";
                worksheet.Cell(row, 3).Value = indicador.CodClienteSAP;
                //"DEST MERCANCIA";
                worksheet.Cell(row, 4).Value = indicador.CodSucursal;
                //"CLASE DOC VTAS";
                worksheet.Cell(row, 5).Value = indicador.ClaseDocumentoVentas;
                //"SOLICITANTE";
                worksheet.Cell(row, 6).Value = indicador.RutSolicitante;
                //"FECHA SOLICITUD";
                worksheet.Cell(row, 7).Value = indicador.FechaSolicitud;
                //"TOTAL NETO";
                worksheet.Cell(row, 8).Value = indicador.TotalNeto;
                //"NUM PEDIDO";
                worksheet.Cell(row, 9).Value = indicador.NumPedido;
                //"TIPO PEDIDO";
                worksheet.Cell(row, 10).Value = indicador.TipoPedido;
                // "PRIORIDAD";
                worksheet.Cell(row, 11).Value = indicador.PrioridadPedido;
                row++;
            }

            xlPackage.Workbook.Properties.Title = "Indicadores ventas";
            xlPackage.Workbook.Properties.Author = "SKBerge Centro Repuestos";
            xlPackage.Save();

            //worksheet.HeaderFooter.oddHeader.CenteredText = "Tinned Goods Sales";
            // add the page number to the footer plus the total number of pages
            //worksheet.HeaderFooter.oddFooter.RightAlignedText =
            //string.Format("Page {0} of {1}", ExcelHeaderFooter.PageNumber,ExcelHeaderFooter.NumberOfPages);
        }
    }
}