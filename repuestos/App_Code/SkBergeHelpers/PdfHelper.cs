using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Collections;
using System.Web.UI.WebControls;
using System;
using System.Globalization;
using System.Threading;
using System.Security.Cryptography;

using iTextSharp.text.html.simpleparser;
using System.Text;
using System.Data.SqlClient;



/// <summary>
/// Descripción breve de PdfHelper
/// </summary>
public class PdfHelper
{
    ControlBD _controlBD;
    ControlBDSolicitud _ControlBD = new ControlBDSolicitud();
    string _path = "";
    SqlConnection con, con1;
    SqlCommand cmd, cmd1;

	public PdfHelper()
	{
        _controlBD = new ControlBD();
	}

    //Constructor con un parametro
    public PdfHelper(string path)
    {
        //
        // TODO: Agregar aquí la lógica del constructor
        //
        _path = path;
        _controlBD = new ControlBD();
    }
    /// <summary>
    /// Crea un archivo PDF con el detalle de la cotización para usuario operador.
    /// recive como parametro el numero de cotización
    /// </summary>
    /// <param name="rut"></param>
    public string CrearArchivoPdfCotizacion(string numCotizacion,string rut)
    {
        //Datos cotizador
        string nombreUser = "";
        string sucursal = "";
        string dealer = "";

        //Se obtiene la fecha con formato dd.mm.aaaa
        DateTime dt = DateTime.Now;
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        Console.WriteLine(dt.ToString("d"));
        CultureInfo ci = new CultureInfo("de-DE");


        //Se declara el documento para crear el pdf
        Document document = new Document(iTextSharp.text.PageSize.LETTER, 50, 50, 25, 25);//tamaño carta

        BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
        string Cotizacion = Guid.NewGuid().ToString();//se le da el nombre al archivo

        //Se declaran tipos de fuentes y sus atributos para que quede sexy ;)
        Font times = new Font(bfTimes, 20, Font.BOLD);
        Font pie = new Font(bfTimes, 9, Font.NORMAL);



        //Se declara la ruta para obtener el archivo pdf creado
        String ruta = _path + "/" + "../img";
        try
        {

            //Se crea un nuevo archivo PDF
            PdfWriter.GetInstance(document, new FileStream(_path + "\\" + Cotizacion.Trim() + ".pdf", FileMode.Create));

        }
        catch (System.IO.IOException ex)
        {
            string errorEx = ex.ToString();
            return errorEx;
        }


        //Se abre el documento para su procesamiento
        document.Open();

        //Se agregan los parametros al documento mediante "document.Add()"      

        //Titulo
        document.Add(new Paragraph("Cotización - "+ numCotizacion +" \n\n", times));
        //se obtienen los datos del usuario y del dealer
        DataSet ds2 = _controlBD.ObtenerDatosFiltrados(@"select concesionario.nombreConcesionario,sucursal.direccionSucursal,concesionario.imagen,persona.nombre
                                                from persona,sucursal,concesionario
                                                where persona.rut='" + rut + "' " +
                                                " AND persona.shipCode=sucursal.shipCode " +
                                                " AND sucursal.nombreConcesionario=concesionario.nombreConcesionario " +
                                                " ");
        foreach (DataRow campo in ds2.Tables[0].Rows)
        {


            try
            {
                //Se agrega el logo del concesionario obteniendo la ruta de la imagen desde la BD
                iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(_path + "/" + "../doc/imgConcesionarios/" + campo["nombreConcesionario"].ToString() + ".jpg");
                
                gif.ScalePercent(26f);
                gif.SetAbsolutePosition(document.PageSize.Width - 90f,
                    document.PageSize.Height - 50f);
                gif.ScaleToFit(90f, 50f);
                document.Add(gif);
            }
            catch (System.Net.WebException ex)
            {
                iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(_path + "/" + "../img/noImgExist.png");
                gif.ScalePercent(15f);
                gif.SetAbsolutePosition(document.PageSize.Width - 90f,
                    document.PageSize.Height - 50f);
                //gif.ScaleToFit(250f, 250f);
                document.Add(gif);
            }
            catch (System.IO.DirectoryNotFoundException ex)
            {
                iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(_path + "/" + "../img/noImgExist.png");
                gif.ScalePercent(15f);
                gif.SetAbsolutePosition(document.PageSize.Width - 90f,
                    document.PageSize.Height - 50f);
                //gif.ScaleToFit(250f, 250f);
                document.Add(gif);
            }

            nombreUser = campo["nombre"].ToString();
            sucursal = campo["direccionSucursal"].ToString();
            dealer = campo["nombreConcesionario"].ToString();


            document.Add(new Paragraph("Concesionario: " + dealer));
            document.Add(new Paragraph("Cotizador: " + nombreUser));
            //document.Add(new Paragraph("Rut Cotizador: " + rut));
            document.Add(new Paragraph("Dirección: " + sucursal));

        }

        //Se ingresa la fecha de creacion
        document.Add(new Paragraph("Fecha Creación: " + dt.ToString("d", ci)));
        string validez = "Esta cotización tiene una validez de 5 días hábiles a partir de la fecha de creación";
        document.Add(new Paragraph(validez, pie));
        document.Add(new Paragraph("\n\n"));


        //Se crea la tabla que contendra los datos de los repuestos consultados
        PdfPTable table = new PdfPTable(6);
        PdfPCell titulo = new PdfPCell(new Phrase("Detalle Cotización", new Font(Font.NORMAL, 12f, Font.NORMAL)));
        //PdfPCell cod = new PdfPCell(new Phrase("Código", new Font(Font.FontFamily.TIMES_ROMAN, 10f, Font.NORMAL)));
        PdfPCell marc = new PdfPCell(new Phrase("Marca", new Font(Font.FontFamily.TIMES_ROMAN, 10f, Font.NORMAL)));
        PdfPCell cant = new PdfPCell(new Phrase("Cantidad", new Font(Font.FontFamily.TIMES_ROMAN, 10f, Font.NORMAL)));
        PdfPCell codigo = new PdfPCell(new Phrase("Codigo", new Font(Font.FontFamily.TIMES_ROMAN, 10f, Font.NORMAL)));
        PdfPCell descrip = new PdfPCell(new Phrase("Descripcion", new Font(Font.FontFamily.TIMES_ROMAN, 10f, Font.NORMAL)));
        PdfPCell precioUnit = new PdfPCell(new Phrase("Precio Lista(PLS)", new Font(Font.FontFamily.TIMES_ROMAN, 10f, Font.NORMAL)));
        PdfPCell precioTot = new PdfPCell(new Phrase("Total", new Font(Font.FontFamily.TIMES_ROMAN, 10f, Font.NORMAL)));

        titulo.Colspan = 6;
        titulo.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right

        //Agrego los titulos de las columnas en celdas
        table.AddCell(titulo);
        //table.AddCell(cod);
        table.AddCell(marc);
        table.AddCell(cant);
        table.AddCell(codigo);
        table.AddCell(descrip);
        table.AddCell(precioUnit);
        table.AddCell(precioTot);

        //Se obtiene el idPedido con E_VBELN
        SapAPI _sapApi = new SapAPI();
        string idPedido = _sapApi.getIdPedidoPorE_VBELN(numCotizacion);

        //Se obtienen los datos de los repuestos de la cotización
        DataSet dsL = _controlBD.ObtenerDatosFiltrados("select * from MATERIALES_PEDIDO where id_pedido = '" + idPedido + "'");
        foreach (DataRow campo in dsL.Tables[0].Rows)
        {
            //PdfPCell _cod = new PdfPCell(new Phrase(codigo, new Font(Font.FontFamily.COURIER, 9f, Font.NORMAL)));
            PdfPCell _marca = new PdfPCell(new Phrase(campo["marca"].ToString(), new Font(Font.FontFamily.COURIER, 9f, Font.NORMAL)));
            PdfPCell _cant = new PdfPCell(new Phrase(campo["cantidad"].ToString(), new Font(Font.FontFamily.COURIER, 9f, Font.NORMAL)));
            PdfPCell _codigo = new PdfPCell(new Phrase(campo["codigo"].ToString(), new Font(Font.FontFamily.COURIER, 8f, Font.NORMAL)));
            PdfPCell _descrip = new PdfPCell(new Phrase(campo["descripcion"].ToString(), new Font(Font.FontFamily.COURIER, 8f, Font.NORMAL)));
            PdfPCell _precio = new PdfPCell(new Phrase("$" + campo["valor"].ToString(), new Font(Font.FontFamily.COURIER, 9f, Font.NORMAL)));
            PdfPCell _total = new PdfPCell(new Phrase("$" + campo["total"].ToString(), new Font(Font.FontFamily.COURIER, 9f, Font.NORMAL)));


            //table.AddCell(_cod);
            table.AddCell(_marca);
            table.AddCell(_cant);
            table.AddCell(_codigo);
            table.AddCell(_descrip);
            table.AddCell(_precio);
            table.AddCell(_total);

        }

        //Calculo el total de la cotización
        int suma = 0;
        DataSet dsT = _controlBD.ObtenerDatosFiltrados("select * from MATERIALES_PEDIDO where id_pedido = '" + idPedido + "'");
        foreach (DataRow row in dsT.Tables[0].Rows)
        {
            suma += int.Parse(row["total"].ToString());
        }

        PdfPCell granTotal = new PdfPCell(new Phrase("Total Neto: $" + suma, new Font(Font.NORMAL, 10f, Font.NORMAL)));
        granTotal.Colspan = 6;
        granTotal.HorizontalAlignment = 2;
        table.AddCell(granTotal);

        document.Add(table);
        document.Close();

        string pdf_ruta = _path + "\\" + Cotizacion.Trim() + ".pdf";

        return pdf_ruta;

    }

    public string CrearSolicitudPdfComunFinal(string ssesionId, string solicitud, string rut)
    {
        string querydel = "DELETE t_SolicitudComunPDFSP";
        _ControlBD.EjecutaQuery(querydel);

        //Se crea la cotizacion en la base de datos, se envia el rut y el id de sesión al metodo
        LlenarTablaSolicitudComunFinal(ssesionId, solicitud, rut);

        //Datos cotizador
        string nombreUser = "";
        string sucursal = "";
        string dealer = "";


        //Se obtiene la fecha con formato dd.mm.aaaa
        DateTime dt = DateTime.Now;
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        Console.WriteLine(dt.ToString("d"));
        CultureInfo ci = new CultureInfo("de-DE");


        //Se declara el documento para crear el pdf
        Document document = new Document(iTextSharp.text.PageSize.LETTER, 50, 50, 25, 25);//tamaño carta

        BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
        string Cotizacion = Guid.NewGuid().ToString();//se le da el nombre al archivo

        //Se declaran tipos de fuentes y sus atributos para que quede sexy ;)
        Font times = new Font(bfTimes, 20, Font.BOLD);
        Font pie = new Font(bfTimes, 9, Font.NORMAL);

        //Detalle COtizacion correo
        DataSet dsDet = _controlBD.ObtenerDatosFiltrados("select top 1 detalle, rut_cotiza from t_Cotiza where id_cotiza = " + solicitud + " ");
        string detalleNombre = "";
        string rutCotizador = "";
        foreach (DataRow campo in dsDet.Tables[0].Rows)
        {
            detalleNombre = campo["detalle"].ToString();
            rutCotizador = campo["rut_cotiza"].ToString();

        }

        //Se declara la ruta para obtener el archivo pdf creado
        String ruta = _path + "/" + "../img";
        try
        {

            //Se crea un nuevo archivo PDF
            PdfWriter.GetInstance(document, new FileStream(_path + "\\" + detalleNombre.Trim() + "_" + rut + "_Final.pdf", FileMode.Create));

        }
        catch (System.IO.IOException ex)
        {
            string errorEx = ex.ToString();
            return errorEx;
        }

        //Se abre el documento para su procesamiento
        document.Open();


        //Se agregan los parametros al documento medinte "document.Add()"      

        //Titulo
        document.Add(new Paragraph("Solicitud de Cotización\n\n", times));


        //Se obtiene el correlativo del documento
        //Se obtiene el id de la cotización y se actualiza la lista de repuestos con el id de la cotizacion para relacionar
        DataSet dsId2 = _controlBD.ObtenerDatosFiltrados("select top 1 * from t_CotizaPDF WHERE idSolicitud ='" + Convert.ToInt32(solicitud) + "' order by idSolicitud desc");
        string correlativo = "";
        foreach (DataRow campo in dsId2.Tables[0].Rows)
        {
            correlativo = campo["idSolicitud"].ToString();
            document.Add(new Paragraph("Folio: " + correlativo));
        }

        //Sp para recuperar Datos de Usuarios cotizador de acuerdo a cotizacion P.Orostegui
        DataSet ds2 = new DataSet();
        ds2 = _controlBD.obtienedealercotiza(1, correlativo);

        //Fin Modificacion P.Orostegui

        //se obtienen los datos del usuario y del dealer
        /*
        DataSet ds2 = _controlBD.ObtenerDatosFiltrados(@"select concesionario.nombreConcesionario,sucursal.direccionSucursal,concesionario.imagen,persona.nombre
                                                from persona,sucursal,concesionario
                                                where persona.rut='" + rut + "' " +
                                                " AND persona.shipCode=sucursal.shipCode " +
                                                " AND sucursal.nombreConcesionario=concesionario.nombreConcesionario " +
                                                " ");*/
        foreach (DataRow campo in ds2.Tables[0].Rows)
        {


            try
            {
                //Se agrega el logo del concesionario obteniendo la ruta de la imagen desde la BD
                iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(_path + "/" + "../doc/imgConcesionarios/" + campo["nombreConcesionario"].ToString() + ".jpg");
                gif.ScalePercent(26f);
                gif.SetAbsolutePosition(document.PageSize.Width - 90f,
                    document.PageSize.Height - 50f);
                gif.ScaleToFit(90f, 50f);
                document.Add(gif);
            }
            catch (System.Net.WebException ex)
            {
                iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(_path + "/" + "../img/noImgExist.png");
                gif.ScalePercent(15f);
                gif.SetAbsolutePosition(document.PageSize.Width - 90f,
                    document.PageSize.Height - 50f);
                //gif.ScaleToFit(250f, 250f);
                document.Add(gif);
            }
            catch (System.IO.DirectoryNotFoundException ex)
            {
                iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(_path + "/" + "../img/noImgExist.png");
                gif.ScalePercent(15f);
                gif.SetAbsolutePosition(document.PageSize.Width - 90f,
                    document.PageSize.Height - 50f);
                //gif.ScaleToFit(250f, 250f);
                document.Add(gif);
            }

            //Sp para recuperar Datos de Usuarios cotizador de acuerdo a cotizacion P.Orostegui


            nombreUser = campo["nombre"].ToString();
            sucursal = campo["direccionSucursal"].ToString();
            dealer = campo["nombreConcesionario"].ToString();


            string digv = Dv(rutCotizador);
            string rutf = rutCotizador + digv;

            document.Add(new Paragraph("Concesionario: " + dealer));
            document.Add(new Paragraph("Cotizador: " + nombreUser));
            document.Add(new Paragraph("Rut Cotizador: " + formatearRut(rutf)));
            document.Add(new Paragraph("Dirección: " + sucursal));


        }


        string validez = "";

        DataSet dsseg = _controlBD.ObtenerDatosFiltrados("select top 1 * from t_CotizaPDF where idsolicitud = " + solicitud + " order by idSolicitud desc");
        string idCots = "";
        foreach (DataRow campo in dsseg.Tables[0].Rows)
        {
            idCots = campo["idSolicitud"].ToString();
        }


        DataSet dsLs = _controlBD.ObtenerDatosFiltrados("select * from t_SolicitudComunPDFSP where numeroSolicitud = '" + idCots + "'");
        foreach (DataRow campo in dsLs.Tables[0].Rows)
        {
            if (campo["tipo"].ToString() == "seguro")
            {

                DataSet dataSeguro = _controlBD.ObtenerDatosFiltrados("SELECT dias FROM validezCotizacion WHERE nombre = 'seguro'");
                string diasSeguro = "";
                foreach (DataRow campoSeguro in dataSeguro.Tables[0].Rows)
                {
                    diasSeguro = campoSeguro["dias"].ToString();
                }

                validez = "Esta Cotización tiene una validez de " + diasSeguro + " días a partir de la fecha de creación";
                break;
            }
            else if (campo["tipo"].ToString() == "normal")
            {

                DataSet dataNormal = _controlBD.ObtenerDatosFiltrados("SELECT dias FROM validezCotizacion WHERE nombre = 'normal'");
                string diasNormal = "";
                foreach (DataRow campoNormal in dataNormal.Tables[0].Rows)
                {
                    diasNormal = campoNormal["dias"].ToString();
                }

                validez = "Esta Cotización tiene una validez de " + diasNormal + " días a partir de la fecha de creación";
                break;
            }
            else if (campo["tipo"].ToString() == "garantia")
            {

                DataSet datagarantia = _controlBD.ObtenerDatosFiltrados("SELECT dias FROM validezCotizacion WHERE nombre = 'garantia'");
                string diasgarantia = "";
                foreach (DataRow campogarantia in datagarantia.Tables[0].Rows)
                {
                    diasgarantia = campogarantia["dias"].ToString();
                }

                validez = "Esta Cotización tiene una validez de " + diasgarantia + " días a partir de la fecha de creación";
                break;
            }

        }



        //Obtiene nuevo campo detalle
        string detalle = "";
        DataSet dsdet = _controlBD.ObtenerDatosFiltrados("select detalle from t_Cotiza where id_cotiza = " + solicitud);
        foreach (DataRow campo in dsdet.Tables[0].Rows)
        {
            detalle = campo["detalle"].ToString();
        }


        //Se ingresa la fecha de creacion
        document.Add(new Paragraph("Fecha Creación: " + dt.ToString()));
        document.Add(new Paragraph("Detalle: " + detalle));//Nuevo campo Detalle
        //string validez = "Esta Cotización tiene una validez de 10 días a partir de la fecha de creación";
        document.Add(new Paragraph(validez, pie));
        document.Add(new Paragraph("\n\n"));


        //Se crea la tabla que contendra los datos de los repuestos consultados
        PdfPTable table = new PdfPTable(13);
        table.WidthPercentage = 100;
        PdfPCell titulo = new PdfPCell(new Phrase("Detalle Solicitud", new Font(Font.NORMAL, 12f, Font.NORMAL)));
        PdfPCell numero = new PdfPCell(new Phrase("Numero Solicitud", new Font(Font.FontFamily.TIMES_ROMAN, 07f, Font.NORMAL)));
        PdfPCell marc = new PdfPCell(new Phrase("Marca", new Font(Font.FontFamily.TIMES_ROMAN, 07f, Font.NORMAL)));
        PdfPCell cant = new PdfPCell(new Phrase("Cantidad", new Font(Font.FontFamily.TIMES_ROMAN, 07f, Font.NORMAL)));
        PdfPCell cod = new PdfPCell(new Phrase("Código", new Font(Font.FontFamily.TIMES_ROMAN, 07f, Font.NORMAL)));
        PdfPCell descrip = new PdfPCell(new Phrase("Descripción", new Font(Font.FontFamily.TIMES_ROMAN, 07f, Font.NORMAL)));
        PdfPCell tipo = new PdfPCell(new Phrase("Tipo de Solicitud", new Font(Font.FontFamily.TIMES_ROMAN, 07f, Font.NORMAL)));
        PdfPCell vin = new PdfPCell(new Phrase("Vin", new Font(Font.FontFamily.TIMES_ROMAN, 07f, Font.NORMAL)));
        PdfPCell envio = new PdfPCell(new Phrase("Envio Solicitado", new Font(Font.FontFamily.TIMES_ROMAN, 07f, Font.NORMAL)));
        PdfPCell dias = new PdfPCell(new Phrase("Plazo Importación (Días)", new Font(Font.FontFamily.TIMES_ROMAN, 07f, Font.NORMAL)));
        PdfPCell precio = new PdfPCell(new Phrase("Precio lista Sugerido Unitario", new Font(Font.FontFamily.TIMES_ROMAN, 07f, Font.NORMAL)));
        PdfPCell anulado = new PdfPCell(new Phrase("Rechazado", new Font(Font.FontFamily.TIMES_ROMAN, 7f, Font.NORMAL)));
        PdfPCell comentario = new PdfPCell(new Phrase("Comentario", new Font(Font.FontFamily.TIMES_ROMAN, 7f, Font.NORMAL)));
        PdfPCell PrecioTotal = new PdfPCell(new Phrase("Valor Total", new Font(Font.FontFamily.TIMES_ROMAN, 7f, Font.NORMAL)));

        titulo.Colspan = 13;
        titulo.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
        numero.HorizontalAlignment = 1;
        marc.HorizontalAlignment = 1;
        cant.HorizontalAlignment = 1;
        cod.HorizontalAlignment = 1;
        descrip.HorizontalAlignment = 1;
        tipo.HorizontalAlignment = 1;
        vin.HorizontalAlignment = 1;
        envio.HorizontalAlignment = 1;
        dias.HorizontalAlignment = 1;
        precio.HorizontalAlignment = 1;
        anulado.HorizontalAlignment = 1;
        comentario.HorizontalAlignment = 1;
        PrecioTotal.HorizontalAlignment = 1;

        //Agrego los titulos de las columnas en celdas
        table.AddCell(titulo);
        table.AddCell(numero);
        table.AddCell(marc);
        table.AddCell(cant);
        table.AddCell(cod);
        table.AddCell(descrip);
        table.AddCell(tipo);
        table.AddCell(vin);
        table.AddCell(envio);
        table.AddCell(dias);
        table.AddCell(precio);
        table.AddCell(anulado);
        table.AddCell(comentario);
        table.AddCell(PrecioTotal);



        //Se obtiene el id de la cotización y se actualiza la lista de repuestos con el id de la cotizacion para relacionar
        DataSet dsId = _controlBD.ObtenerDatosFiltrados("select top 1 * from t_CotizaPDF WHERE idSolicitud = '" + Convert.ToInt32(solicitud) + "' order by idSolicitud desc");
        string idCot = "";
        foreach (DataRow campo in dsId.Tables[0].Rows)
        {
            idCot = campo["idSolicitud"].ToString();
        }

        string EstaAnulada = "";

        //INICIO  : Bloque de codigo dnd agrego un a columna de dias habiles para mostrar en PDF  04Oct2021
        string diasPDF = " ,isnull((Select m.diasHabiles from dbo.marca m where m.[nombreMarca]= marca),'0') as diasHabiles"; //maikol
        // DataSet dsL = _controlBD.ObtenerDatosFiltrados("select * from t_SolicitudCotizacion where numeroSolicitud = '" + idCot + "'");
        DataSet dsL = _controlBD.ObtenerDatosFiltrados("select * " + diasPDF + " from t_SolicitudCotizacion where numeroSolicitud = '" + idCot + "'");
        //agrego un a columna de dias habiles para mostrar en PDF  
        //FIN  : Bloque de codigo dnd agrego un a columna de dias habiles para mostrar en PDF 04Oct2021
        foreach (DataRow campo in dsL.Tables[0].Rows)
        {

            if (campo["anulada"].ToString() == "True")
            {
                EstaAnulada = "SI";
            }
            else
            {
                EstaAnulada = "NO";
            }

            //PdfPCell _cod = new PdfPCell(new Phrase(codigo, new Font(Font.FontFamily.COURIER, 9f, Font.NORMAL)));
            PdfPCell _numero = new PdfPCell(new Phrase(campo["numeroSolicitud"].ToString(), new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));
            PdfPCell _marca = new PdfPCell(new Phrase(campo["marca"].ToString(), new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));
            PdfPCell _cant = new PdfPCell(new Phrase(campo["cantidad"].ToString(), new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));
            PdfPCell _cod = new PdfPCell(new Phrase(campo["codRepto"].ToString(), new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));
            PdfPCell _descrip = new PdfPCell(new Phrase(campo["descripcion"].ToString(), new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));
            PdfPCell _tipo = new PdfPCell(new Phrase(campo["tipo"].ToString(), new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));
            PdfPCell _vin = new PdfPCell(new Phrase(campo["vin"].ToString(), new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));
            PdfPCell _envio = new PdfPCell(new Phrase(campo["envio"].ToString(), new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));
            PdfPCell _dias = new PdfPCell(new Phrase(campo["dias"].ToString(), new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));
            PdfPCell _precio = new PdfPCell(new Phrase(campo["preciounitario"].ToString(), new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));
            PdfPCell _anulado = new PdfPCell(new Phrase(EstaAnulada, new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));
            PdfPCell _comentario = new PdfPCell(new Phrase(campo["comentario"].ToString(), new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));
            PdfPCell _preciototal = new PdfPCell(new Phrase(campo["precio_solicitud"].ToString(), new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));

            _numero.HorizontalAlignment = 1;
            _marca.HorizontalAlignment = 1;
            _cant.HorizontalAlignment = 1;
            _cant.HorizontalAlignment = 1;
            _descrip.HorizontalAlignment = 1;
            _tipo.HorizontalAlignment = 1;
            _vin.HorizontalAlignment = 1;
            _envio.HorizontalAlignment = 1;
            _dias.HorizontalAlignment = 1;
            _precio.HorizontalAlignment = 1;
            _anulado.HorizontalAlignment = 1;
            _comentario.HorizontalAlignment = 1;
            _preciototal.HorizontalAlignment = 1;

            table.AddCell(_numero);
            table.AddCell(_marca);
            table.AddCell(_cant);
            table.AddCell(_cod);
            table.AddCell(_descrip);
            table.AddCell(_tipo);
            table.AddCell(_vin);
            table.AddCell(_envio);
            table.AddCell(_dias);
            table.AddCell(_precio);
            table.AddCell(_anulado);
            table.AddCell(_comentario);
            table.AddCell(_preciototal);
        }
        int diasH = 0;
        int suma1 = 0;
        string val = "";
        //comentado por maikol para dias habiles 
        //DataSet dsT = _controlBD.ObtenerDatosFiltrados("select * from t_SolicitudComunPDFSP where numeroSolicitud = '" + idCot + "'");
        DataSet dsT = _controlBD.ObtenerDatosFiltrados("select * " + diasPDF + " from t_SolicitudComunPDFSP where numeroSolicitud = '" + idCot + "'");
        foreach (DataRow row in dsT.Tables[0].Rows)
        {
            val = (row["precio"].ToString());
            val = val.Replace(".", "");
            suma1 += int.Parse(val);
            diasH = int.Parse(row["diasHabiles"].ToString()); //Dias habiles para PDF -- Cambiado 01-08-2021 (Maikol Queupumil  Chile)
        }
        string suma = FormatoValor(Convert.ToString(suma1));
        PdfPCell granTotal = new PdfPCell(new Phrase("Total Neto: $" + suma, new Font(Font.NORMAL, 10f, Font.NORMAL)));
        granTotal.Colspan = 10;
        granTotal.HorizontalAlignment = 2;
        table.AddCell(granTotal);

        document.Add(table);
        document.Add(new Paragraph("\n\n"));
        //antes'corresponden a días hábiles' despues 'corresponden a 15 días hábiles' -- Cambiado 01-08-2021 (Maikol Queupumil  Chile)
        //antes'corresponden a días hábiles' despues 'corresponden a 15 días hábiles' -- Cambiado 01-08-2021 (Maikol Queupumil  Chile) 04Oct2021
        if (diasH == 0) diasH = 15;
        // string nuevo = "Nota 1: Los días estipulados en el plazo de importación tienen una validez que corresponden a " + diasH + " días hábiles y se consideran a partir de la fecha de generación del VFC.";
        string nuevo = "Nota 1: El periodo de validez de los precios cotizados corresponde a " + diasH + " días hábiles y se consideran a partir de la fecha de confirmación por parte del ADV.";
        document.Add(new Paragraph(nuevo, pie));  
        document.Add(new Paragraph("\n"));
        string linea = "Nota 2: El valor neto cotizado corresponde al precio lista sugerido, al cual hay que aplicar el descuento por marca + IGV.";
        document.Add(new Paragraph(linea, pie));
        document.Close();

        string pdf_ruta = _path + "\\" + detalleNombre.Trim() + "_" + rut + "_Final.pdf";
        return pdf_ruta;

    }

    public static string FormatoValor(string valor)
    {
        double value = double.Parse(valor);
        CultureInfo elGR = CultureInfo.CreateSpecificCulture("el-GR");
        string valorFinal = value.ToString("0,0", elGR);
        return valorFinal;
    }

    public void LlenarTablaSolicitudComunFinal(string idSession, string solicitud, string rut)
    {

        //Datos Repuestos
        string numero = ""; // numero de solicitud
        string fecha = ""; //fecha solicitud
        string marca = ""; //marca
        string cod = ""; // codigo del repuesto
        string descripcion = ""; //descripcion del repuesto
        string cantidad = ""; //cantidad
        string usuario = ""; //usuario
        string conce = ""; //concesionario
        string tipo = ""; // tipo garantia - normal
        string vin = ""; //vin
        string envio = ""; //tipo envio
        string sesion = ""; //sesion
        string precio; //precio
        string dia = "";//dias de envio

        //Inserto los datos de la cotización en la tabla COTIZACION_COMUN para su posterior consulta
        _controlBD.InsertarDatos(@"insert into t_CotizaPDF(idSolicitud, sesion, fecha_cotiza, rut_cotiza) values('" + Convert.ToInt32(solicitud) + "', '" + idSession + "', GETDATE(), '" + rut + "' )");


        //Se obtienen los datos desde la tabla temporal LISTA_PRODUCTOS_TEMP
        //y se insertan en la tabla COTIZACION_COMUN_REPUESTO
        DataSet ds = _controlBD.ObtenerDatosFiltrados("select * from t_SolicitudCotizacion where numeroSolicitud = '" + solicitud + "'");
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            numero = campo["numeroSolicitud"].ToString();
            fecha = campo["fecha"].ToString();
            marca = campo["marca"].ToString();
            cod = campo["codRepto"].ToString();
            descripcion = campo["descripcion"].ToString();
            cantidad = campo["cantidad"].ToString();
            usuario = campo["usuario"].ToString();
            conce = campo["concesionario"].ToString();
            tipo = campo["tipo"].ToString();
            vin = campo["vin"].ToString();
            envio = campo["envio"].ToString();
            sesion = campo["sesion"].ToString();
            precio = FormatoValor(campo["precio_Solicitud"].ToString());
            dia = campo["dias"].ToString();

            _controlBD.InsertarDatos(@"insert into t_SolicitudComunPDFSP(numeroSolicitud, fecha, marca, codRepto, descripcion, cantidad, usuario, concesionario, tipo, vin, envio, sesion, precio, dias)
                values('" + numero + "','" + fecha + "','" + marca + "','" + cod + "','" + descripcion + "','" + cantidad + "','" + usuario + "','" + conce + "', '" + tipo + "' , '" + vin + "','" + envio + "','" + sesion + "','" + precio + "','" + dia + "')");
        }
    }

    public string CrearArchivoPdfComun(string rut,string ssesionId)
    {
        //Se crea la cotizacion en la base de datos, se envia el rut y el id de sesión al metodo
        LlenarTablaCotizacionComun(rut,ssesionId);

        //Datos cotizador
        string nombreUser = "";
        string sucursal = "";
        string dealer = "";
        
        
        //Se obtiene la fecha con formato dd.mm.aaaa
        DateTime dt = DateTime.Now;
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        Console.WriteLine(dt.ToString("d"));
        CultureInfo ci = new CultureInfo("de-DE");


        //Se declara el documento para crear el pdf
        Document document = new Document(iTextSharp.text.PageSize.LETTER, 50, 50, 25, 25);//tamaño carta
    
        BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
        string Cotizacion = Guid.NewGuid().ToString();//se le da el nombre al archivo
        
        //Se declaran tipos de fuentes y sus atributos para que quede sexy ;)
        Font times = new Font(bfTimes, 20, Font.BOLD);
        Font pie = new Font(bfTimes, 9, Font.NORMAL);


        
        //Se declara la ruta para obtener el archivo pdf creado
        String ruta = _path + "/" + "../img";
        try {

            //Se crea un nuevo archivo PDF
            PdfWriter.GetInstance(document, new FileStream(_path + "\\" + Cotizacion.Trim() + ".pdf", FileMode.Create));

        }
        catch (System.IO.IOException ex)
        {
            string errorEx = ex.ToString();
            return errorEx;
        }
        

        //Se abre el documento para su procesamiento
        document.Open();


        //Se agregan los parametros al documento medinte "document.Add()"      
       
        //Titulo
        document.Add(new Paragraph("Cotización\n\n", times));


        //Se obtiene el correlativo del documento
        //Se obtiene el id de la cotización y se actualiza la lista de repuestos con el id de la cotizacion para relacionar
        DataSet dsId2 = _controlBD.ObtenerDatosFiltrados("select top 1 * from COTIZACION_COMUN order by idCotizacionComun desc");
        string correlativo = "";
        foreach (DataRow campo in dsId2.Tables[0].Rows)
        {
            correlativo = campo["idCotizacionComun"].ToString();
            document.Add(new Paragraph("Folio: " + correlativo));
        }

        //se obtienen los datos del usuario y del dealer
        DataSet ds2 = _controlBD.ObtenerDatosFiltrados(@"select top 1 concesionario.nombreConcesionario,sucursal.direccionSucursal,concesionario.imagen,persona.nombre
                                                from persona,sucursal,concesionario
                                                where persona.rut='" + rut + "' " +
                                                " AND persona.shipCode=sucursal.shipCode " +
                                                " AND sucursal.nombreConcesionario=concesionario.nombreConcesionario " +
                                                " ");
        foreach (DataRow campo in ds2.Tables[0].Rows)
        {
            

            try
            {
                //Se agrega el logo del concesionario obteniendo la ruta de la imagen desde la BD
                iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(_path + "/" + "../doc/imgConcesionarios/" + campo["nombreConcesionario"].ToString() + ".jpg");
                gif.ScalePercent(26f);
                gif.SetAbsolutePosition(document.PageSize.Width - 90f,
                    document.PageSize.Height - 50f );
                gif.ScaleToFit(90f, 50f);
                document.Add(gif);
            }
            catch (System.Net.WebException ex)
            {
                iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(_path + "/" + "../img/noImgExist.png");
                gif.ScalePercent(15f);
                gif.SetAbsolutePosition(document.PageSize.Width - 90f,
                    document.PageSize.Height - 50f);
                //gif.ScaleToFit(250f, 250f);
                document.Add(gif);
            }
            catch (System.IO.DirectoryNotFoundException ex)
            {
                iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(_path + "/" + "../img/noImgExist.png");
                gif.ScalePercent(15f);
                gif.SetAbsolutePosition(document.PageSize.Width - 90f,
                    document.PageSize.Height - 50f);
                //gif.ScaleToFit(250f, 250f);
                document.Add(gif);
            }

            nombreUser = campo["nombre"].ToString();
            sucursal = campo["direccionSucursal"].ToString();
            dealer = campo["nombreConcesionario"].ToString();
                

            document.Add(new Paragraph("Concesionario: " + dealer));
            document.Add(new Paragraph("Cotizador: " + nombreUser));
            document.Add(new Paragraph("Rut Cotizador: " + rut));
            document.Add(new Paragraph("Dirección: " + sucursal));

        }

        //Se ingresa la fecha de creacion
        document.Add(new Paragraph("Fecha Creación: " + dt.ToString("d", ci)));
        string validez = "Esta cotización tiene una validez de 5 días a partir de la fecha de creación";
        document.Add(new Paragraph(validez, pie));
        document.Add(new Paragraph("\n\n"));


        //Se crea la tabla que contendra los datos de los repuestos consultados
        PdfPTable table = new PdfPTable(6);
        PdfPCell titulo = new PdfPCell(new Phrase("Detalle Cotización",new Font(Font.NORMAL, 12f, Font.NORMAL)));
        PdfPCell cod = new PdfPCell(new Phrase("Código", new Font(Font.FontFamily.TIMES_ROMAN, 10f, Font.NORMAL)));
        PdfPCell marc = new PdfPCell(new Phrase("Marca", new Font(Font.FontFamily.TIMES_ROMAN, 10f, Font.NORMAL)));
        PdfPCell cant = new PdfPCell(new Phrase("Cantidad", new Font(Font.FontFamily.TIMES_ROMAN, 10f, Font.NORMAL)));
        PdfPCell descrip = new PdfPCell(new Phrase("Descripcion", new Font(Font.FontFamily.TIMES_ROMAN, 10f, Font.NORMAL)));
        PdfPCell precioUnit = new PdfPCell(new Phrase("Precio Lista(PLS)", new Font(Font.FontFamily.TIMES_ROMAN, 10f, Font.NORMAL)));
        PdfPCell precioTot = new PdfPCell(new Phrase("Total", new Font(Font.FontFamily.TIMES_ROMAN, 10f, Font.NORMAL)));

        titulo.Colspan = 6;
        titulo.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right

        //Agrego los titulos de las columnas en celdas
        table.AddCell(titulo);
        table.AddCell(cod);
        table.AddCell(marc);
        table.AddCell(cant);
        table.AddCell(descrip);
        table.AddCell(precioUnit);
        table.AddCell(precioTot);



        //Se obtiene el id de la cotización y se actualiza la lista de repuestos con el id de la cotizacion para relacionar
        DataSet dsId = _controlBD.ObtenerDatosFiltrados("select top 1 * from COTIZACION_COMUN order by idCotizacionComun desc");
        string idCot = "";
        foreach (DataRow campo in dsId.Tables[0].Rows)
        {
            idCot = campo["idCotizacionComun"].ToString();
        }



        DataSet dsL = _controlBD.ObtenerDatosFiltrados("select * from COTIZACION_COMUN_REPUESTO where idCotizacionComun = '" + idCot + "'");
        foreach (DataRow campo in dsL.Tables[0].Rows)
        {
            PdfPCell _cod = new PdfPCell(new Phrase(campo["codigo"].ToString(), new Font(Font.FontFamily.COURIER, 9f, Font.NORMAL)));
            PdfPCell _marca = new PdfPCell(new Phrase(campo["marca"].ToString(), new Font(Font.FontFamily.COURIER, 9f, Font.NORMAL)));
            PdfPCell _cant = new PdfPCell(new Phrase(campo["cantidad"].ToString(), new Font(Font.FontFamily.COURIER, 9f, Font.NORMAL)));
            PdfPCell _descrip = new PdfPCell(new Phrase(campo["descripcion"].ToString(), new Font(Font.FontFamily.COURIER, 8f, Font.NORMAL)));
            PdfPCell _precio = new PdfPCell(new Phrase("$" + campo["precioConce"].ToString(), new Font(Font.FontFamily.COURIER, 9f, Font.NORMAL)));
            PdfPCell _total = new PdfPCell(new Phrase("$" + campo["total"].ToString(), new Font(Font.FontFamily.COURIER, 9f, Font.NORMAL)));


            table.AddCell(_cod);
            table.AddCell(_marca);
            table.AddCell(_cant);
            table.AddCell(_descrip);
            table.AddCell(_precio);
            table.AddCell(_total);
 
        }
       


        double suma = 0;
        DataSet dsT = _controlBD.ObtenerDatosFiltrados("select * from COTIZACION_COMUN_REPUESTO where idCotizacionComun = '" + idCot + "'");
        foreach (DataRow row in dsT.Tables[0].Rows)
        {
            suma += double.Parse(row["total"].ToString());
        }

        PdfPCell granTotal = new PdfPCell(new Phrase("Total Neto: $" + suma, new Font(Font.NORMAL, 10f, Font.NORMAL)));
        granTotal.Colspan = 6;
        granTotal.HorizontalAlignment = 2;
        table.AddCell(granTotal);
                
        document.Add(table);
        document.Close();

        string pdf_ruta = _path + "\\" + Cotizacion.Trim() + ".pdf";
        //System.Diagnostics.Process.Start(pdf_ruta);
        
       // _controlBD.InsertarDatos("delete from carro where idSession = '"+ssesionId+"'");
        //_controlBD.InsertarDatos("delete from LISTA_BUSQUEDA_TMP where ID_SESSION = '" + ssesionId + "'");
        //_controlBD.InsertarDatos("delete from LISTA_REEMPLAZO_TMP where ID_SESSION = '" + ssesionId + "'");

        return pdf_ruta;
 
    }


    public void LlenarTablaCotizacionComun(string rut,string idSession)
    {

         //Datos Repuestos
        string codigo = "";
        string marca = "";
        string descripcion = "";
        string valor = "";
        string valorLista = "";
        string cantidad = "";
        string total = "";
        string totalLista = "";
        string grupMat = "";
        string stock = "";


        //Inserto los datos de la cotización en la tabla COTIZACION_COMUN para su posterior consulta
        _controlBD.InsertarDatos(@"insert into COTIZACION_COMUN(idSession,fechaCreacion,rutCotizador) values('" + idSession + "' ,GETDATE(),'" + rut + "')");


        //Se obtienen los datos desde la tabla temporal LISTA_PRODUCTOS_TEMP
        //y se insertan en la tabla COTIZACION_COMUN_REPUESTO
        DataSet ds = _controlBD.ObtenerDatosFiltrados("select * from carro where idSession = '" + idSession + "'");
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            codigo = campo["codigo"].ToString();
            marca = campo["marca"].ToString();
            cantidad = campo["cantidad"].ToString();
            descripcion = campo["descripcion"].ToString();
            valor = campo["precioC"].ToString();
            valorLista = campo["precioL"].ToString();
            total = campo["totalC"].ToString();
            totalLista = campo["totalL"].ToString();
            grupMat = campo["grupoMat"].ToString();
            stock = campo["stock"].ToString();

            _controlBD.InsertarDatos(@"insert into COTIZACION_COMUN_REPUESTO(idCotizacionComun,marca,codigo,descripcion,stock,precioConce,precioLista,cantidad,total,totalLista)
                values('" + idSession + "','" + marca + "','" + codigo + "','" + descripcion + "','" + stock + "'," + "convert(float,replace('" + valor + "',',','.'))" + "," + "convert(float,replace('" + valorLista + "',',','.'))" + ",'" + cantidad + "', " + "convert(float,replace('" + total + "',',','.'))" + " , " + "convert(float,replace('" + totalLista + "',',','.'))" + ")");
        }

        //Se obtiene el id de la cotización y se actualiza la lista de repuestos con el id de la cotizacion para relacionar
        DataSet dsId = _controlBD.ObtenerDatosFiltrados("select top 1 * from COTIZACION_COMUN order by idCotizacionComun desc");
        string idCot = "";
        foreach (DataRow campo in dsId.Tables[0].Rows)
        {
            idCot = campo["idCotizacionComun"].ToString();
            _controlBD.InsertarDatos(@"update COTIZACION_COMUN_REPUESTO set idCotizacionComun = '" + idCot + "' " +
                                      " where idCotizacionComun = '" + idSession + "' ");
        } 
 
    }

    //pdf de Solicitud Cotizacion
    public string CrearSolicitudPdfComun(string ssesionId, string solicitud, string rut)
    {
        //Se crea la cotizacion en la base de datos, se envia el rut y el id de sesión al metodo
        LlenarTablaSolicitudComun(ssesionId, solicitud, rut);

        //Datos cotizador
        string nombreUser = "";
        string sucursal = "";
        string dealer = "";


        //Se obtiene la fecha con formato dd.mm.aaaa
        DateTime dt = DateTime.Now;
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        Console.WriteLine(dt.ToString("d"));
        CultureInfo ci = new CultureInfo("de-DE");


        //Se declara el documento para crear el pdf
        Document document = new Document(iTextSharp.text.PageSize.LETTER, 50, 50, 25, 25);//tamaño carta

        BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
        string Cotizacion = Guid.NewGuid().ToString();//se le da el nombre al archivo

        //Se declaran tipos de fuentes y sus atributos para que quede sexy ;)
        Font times = new Font(bfTimes, 20, Font.BOLD);
        Font pie = new Font(bfTimes, 9, Font.NORMAL);

        //Detalle COtizacion correo
        DataSet dsDet = _controlBD.ObtenerDatosFiltrados("select top 1 detalle from t_Cotiza where id_cotiza = " + solicitud + " ");
        string detalleNombre = "";
        foreach (DataRow campo in dsDet.Tables[0].Rows)
        {
            detalleNombre = campo["detalle"].ToString();
        }


        //Se declara la ruta para obtener el archivo pdf creado
        String ruta = _path + "/" + "../img";
        try
        {

            //Se crea un nuevo archivo PDF
            //PdfWriter.GetInstance(document, new FileStream(_path + "\\" + Cotizacion.Trim() + ".pdf", FileMode.Create));
            PdfWriter.GetInstance(document, new FileStream(_path + "\\" + detalleNombre.Trim() + "_" + rut + ".pdf", FileMode.Create));

        }
        catch (System.IO.IOException ex)
        {
            string errorEx = ex.ToString();
            return errorEx;
        }


        //Se abre el documento para su procesamiento
        document.Open();


        //Se agregan los parametros al documento medinte "document.Add()"      

        //Titulo
        document.Add(new Paragraph("Solicitud de Cotización\n\n", times));


        //Se obtiene el correlativo del documento
        //Se obtiene el id de la cotización y se actualiza la lista de repuestos con el id de la cotizacion para relacionar
        DataSet dsId2 = _controlBD.ObtenerDatosFiltrados("select top 1 * from t_CotizaPDF where idsolicitud = " + solicitud + " order by idSolicitud desc");
        string correlativo = "";
        foreach (DataRow campo in dsId2.Tables[0].Rows)
        {
            correlativo = campo["idSolicitud"].ToString();
            document.Add(new Paragraph("Folio: " + correlativo));
        }

        //se obtienen los datos del usuario y del dealer
        DataSet ds2 = _controlBD.ObtenerDatosFiltrados(@"select concesionario.nombreConcesionario,sucursal.direccionSucursal,concesionario.imagen,persona.nombre
                                                from persona,sucursal,concesionario
                                                where persona.rut='" + rut + "' " +
                                                " AND persona.shipCode=sucursal.shipCode " +
                                                " AND sucursal.nombreConcesionario=concesionario.nombreConcesionario " +
                                                " ");
        foreach (DataRow campo in ds2.Tables[0].Rows)
        {


            try
            {
                //Se agrega el logo del concesionario obteniendo la ruta de la imagen desde la BD
                iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(_path + "/" + "../doc/imgConcesionarios/" + campo["nombreConcesionario"].ToString() + ".jpg");
                gif.ScalePercent(26f);
                gif.SetAbsolutePosition(document.PageSize.Width - 90f,
                    document.PageSize.Height - 50f);
                gif.ScaleToFit(90f, 50f);
                document.Add(gif);
            }
            catch (System.Net.WebException ex)
            {
                iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(_path + "/" + "../img/noImgExist.png");
                gif.ScalePercent(15f);
                gif.SetAbsolutePosition(document.PageSize.Width - 90f,
                    document.PageSize.Height - 50f);
                //gif.ScaleToFit(250f, 250f);
                document.Add(gif);
            }
            catch (System.IO.DirectoryNotFoundException ex)
            {
                iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(_path + "/" + "../img/noImgExist.png");
                gif.ScalePercent(15f);
                gif.SetAbsolutePosition(document.PageSize.Width - 90f,
                    document.PageSize.Height - 50f);
                //gif.ScaleToFit(250f, 250f);
                document.Add(gif);
            }

            nombreUser = campo["nombre"].ToString();
            sucursal = campo["direccionSucursal"].ToString();
            dealer = campo["nombreConcesionario"].ToString();

            string digv = Dv(rut);
            string rutf = rut + digv;

            document.Add(new Paragraph("Concesionario: " + dealer));
            document.Add(new Paragraph("Cotizador: " + nombreUser));
            document.Add(new Paragraph("Rut Cotizador: " + formatearRut(rutf)));
            document.Add(new Paragraph("Dirección: " + sucursal));

        }
        string validez = "";

        DataSet dsseg = _controlBD.ObtenerDatosFiltrados("select top 1 * from t_CotizaPDF where idsolicitud = " + solicitud + " order by idSolicitud desc");
        string idCots = "";
        foreach (DataRow campo in dsseg.Tables[0].Rows)
        {
            idCots = campo["idSolicitud"].ToString();
        }


        DataSet dsLs = _controlBD.ObtenerDatosFiltrados("select * from t_SolicitudComunPDFSP where numeroSolicitud = '" + idCots + "'");
        foreach (DataRow campo in dsLs.Tables[0].Rows)
        {
            if (campo["tipo"].ToString() == "seguro")
            {

                DataSet dataSeguro = _controlBD.ObtenerDatosFiltrados("SELECT dias FROM validezCotizacion WHERE nombre = 'seguro'");
                string diasSeguro = "";
                foreach (DataRow campoSeguro in dataSeguro.Tables[0].Rows)
                {
                    diasSeguro = campoSeguro["dias"].ToString();
                }

                validez = "Esta Cotización tiene una validez de " + diasSeguro + " días a partir de la fecha de creación";
                break;
            }
            else if (campo["tipo"].ToString() == "normal")
            {

                DataSet dataNormal = _controlBD.ObtenerDatosFiltrados("SELECT dias FROM validezCotizacion WHERE nombre = 'normal'");
                string diasNormal = "";
                foreach (DataRow campoNormal in dataNormal.Tables[0].Rows)
                {
                    diasNormal = campoNormal["dias"].ToString();
                }

                validez = "Esta Cotización tiene una validez de " + diasNormal + " días a partir de la fecha de creación";
                break;
            }
            else if (campo["tipo"].ToString() == "garantia")
            {

                DataSet datagarantia = _controlBD.ObtenerDatosFiltrados("SELECT dias FROM validezCotizacion WHERE nombre = 'garantia'");
                string diasgarantia = "";
                foreach (DataRow campogarantia in datagarantia.Tables[0].Rows)
                {
                    diasgarantia = campogarantia["dias"].ToString();
                }

                validez = "Esta Cotización tiene una validez de " + diasgarantia + " días a partir de la fecha de creación";
                break;
            }

        }



        //Obtiene nuevo campo detalle
        string detalle = "";
        DataSet dsdet = _controlBD.ObtenerDatosFiltrados("select detalle from t_Cotiza where id_cotiza = " + solicitud);
        foreach (DataRow campo in dsdet.Tables[0].Rows)
        {
            detalle = campo["detalle"].ToString();
        }

        //Se ingresa la fecha de creacion
        document.Add(new Paragraph("Fecha Creación: " + dt.ToString()));
        document.Add(new Paragraph("Detalle: " + detalle));//Nuevo campo Detalle
        document.Add(new Paragraph(validez, pie));
        document.Add(new Paragraph("\n\n"));


        //Se crea la tabla que contendra los datos de los repuestos consultados
        PdfPTable table = new PdfPTable(9);
        table.WidthPercentage = 100;
        PdfPCell titulo = new PdfPCell(new Phrase("Detalle Solicitud", new Font(Font.NORMAL, 12f, Font.NORMAL)));
        PdfPCell numero = new PdfPCell(new Phrase("N° Solicitud", new Font(Font.FontFamily.TIMES_ROMAN, 9f, Font.NORMAL)));
        PdfPCell marc = new PdfPCell(new Phrase("Marca", new Font(Font.FontFamily.TIMES_ROMAN, 9f, Font.NORMAL)));
        PdfPCell cant = new PdfPCell(new Phrase("Cantidad", new Font(Font.FontFamily.TIMES_ROMAN, 9f, Font.NORMAL)));
        PdfPCell cod = new PdfPCell(new Phrase("Código", new Font(Font.FontFamily.TIMES_ROMAN, 9f, Font.NORMAL)));
        PdfPCell descrip = new PdfPCell(new Phrase("Descripción", new Font(Font.FontFamily.TIMES_ROMAN, 9f, Font.NORMAL)));
        PdfPCell tipo = new PdfPCell(new Phrase("Tipo", new Font(Font.FontFamily.TIMES_ROMAN, 9f, Font.NORMAL)));
        PdfPCell vin = new PdfPCell(new Phrase("Vin", new Font(Font.FontFamily.TIMES_ROMAN, 9f, Font.NORMAL)));
        PdfPCell envio = new PdfPCell(new Phrase("Envio Solicitado", new Font(Font.FontFamily.TIMES_ROMAN, 9f, Font.NORMAL)));
        PdfPCell dias = new PdfPCell(new Phrase("Plazo Importación (Días)", new Font(Font.FontFamily.TIMES_ROMAN, 9f, Font.NORMAL)));


        //table.SetWidths(values);

        titulo.Colspan = 9;
        titulo.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
        numero.HorizontalAlignment = 1;
        marc.HorizontalAlignment = 1;
        cant.HorizontalAlignment = 1;
        cod.HorizontalAlignment = 1;
        descrip.HorizontalAlignment = 1;
        tipo.HorizontalAlignment = 1;
        vin.HorizontalAlignment = 1;
        envio.HorizontalAlignment = 1;
        dias.HorizontalAlignment = 1;


        //Agrego los titulos de las columnas en celdas
        table.AddCell(titulo);
        table.AddCell(numero);
        table.AddCell(marc);
        table.AddCell(cant);
        table.AddCell(cod);
        table.AddCell(descrip);
        table.AddCell(tipo);
        table.AddCell(vin);
        table.AddCell(envio);
        table.AddCell(dias);



        //Se obtiene el id de la cotización y se actualiza la lista de repuestos con el id de la cotizacion para relacionar
        DataSet dsId = _controlBD.ObtenerDatosFiltrados("select top 1 * from t_CotizaPDF where idsolicitud = " + solicitud + " order by idSolicitud desc");
        string idCot = "";
        foreach (DataRow campo in dsId.Tables[0].Rows)
        {
            idCot = campo["idSolicitud"].ToString();
        }
        //INICIO  : Bloque de codigo dnd agrego un a columna de dias habiles para mostrar en PDF  04Oct2021
        //agrego un a columna de dias habiles para mostrar en PDF  
        string diasPDF = " ,isnull((Select m.diasHabiles from dbo.marca m where m.[nombreMarca]= marca),'0') as diasHabiles";
        //Linea original  DataSet dsL = _controlBD.ObtenerDatosFiltrados("select *  from t_SolicitudCotizacion where numeroSolicitud = '" + idCot + "'");//Linea antigua 
        DataSet dsL = _controlBD.ObtenerDatosFiltrados("select * " + diasPDF + " from t_SolicitudCotizacion where numeroSolicitud = '" + idCot + "'");  //Linea nueva 1170
                                                                                                                                                        //FIN  : Bloque de codigo dnd agrego un a columna de dias habiles para mostrar en PDF 04Oct2021

        foreach (DataRow campo in dsL.Tables[0].Rows)
        {
            //PdfPCell _cod = new PdfPCell(new Phrase(codigo, new Font(Font.FontFamily.COURIER, 9f, Font.NORMAL)));
            PdfPCell _numero = new PdfPCell(new Phrase(campo["numeroSolicitud"].ToString(), new Font(Font.FontFamily.COURIER, 8f, Font.NORMAL)));
            PdfPCell _marca = new PdfPCell(new Phrase(campo["marca"].ToString(), new Font(Font.FontFamily.COURIER, 8f, Font.NORMAL)));
            PdfPCell _cant = new PdfPCell(new Phrase(campo["cantidad"].ToString(), new Font(Font.FontFamily.COURIER, 8f, Font.NORMAL)));
            PdfPCell _cod = new PdfPCell(new Phrase(campo["codRepto"].ToString(), new Font(Font.FontFamily.COURIER, 8f, Font.NORMAL)));
            PdfPCell _descrip = new PdfPCell(new Phrase(campo["descripcion"].ToString(), new Font(Font.FontFamily.COURIER, 8f, Font.NORMAL)));
            PdfPCell _tipo = new PdfPCell(new Phrase(campo["tipo"].ToString(), new Font(Font.FontFamily.COURIER, 8f, Font.NORMAL)));
            PdfPCell _vin = new PdfPCell(new Phrase(campo["vin"].ToString(), new Font(Font.FontFamily.COURIER, 8f, Font.NORMAL)));
            PdfPCell _envio = new PdfPCell(new Phrase(campo["envio"].ToString(), new Font(Font.FontFamily.COURIER, 8f, Font.NORMAL)));
            PdfPCell _dias = new PdfPCell(new Phrase(campo["dias"].ToString(), new Font(Font.FontFamily.COURIER, 8f, Font.NORMAL)));


            _numero.HorizontalAlignment = 1;
            _marca.HorizontalAlignment = 1;
            _cant.HorizontalAlignment = 1;
            _cod.HorizontalAlignment = 1;
            _descrip.HorizontalAlignment = 1;
            _tipo.HorizontalAlignment = 1;
            _vin.HorizontalAlignment = 1;
            _envio.HorizontalAlignment = 1;
            _dias.HorizontalAlignment = 1;


            //table.AddCell(_cod);
            table.AddCell(_numero);
            table.AddCell(_marca);
            table.AddCell(_cant);
            table.AddCell(_cod);
            table.AddCell(_descrip);
            table.AddCell(_tipo);
            table.AddCell(_vin);
            table.AddCell(_envio);
            table.AddCell(_dias);

        }
        int suma = 0;
        int diasH = 0;
        // DataSet dsT = _controlBD.ObtenerDatosFiltrados("select * from t_SolicitudCotizacion where numeroSolicitud = '" + idCot + "'");   //Linea antigua 04Oct2021
        DataSet dsT = _controlBD.ObtenerDatosFiltrados("select * " + diasPDF + " from t_SolicitudCotizacion where numeroSolicitud = '" + idCot + "'");  //Linea nueva 1212 04Oct2021

        foreach (DataRow row in dsT.Tables[0].Rows)
        {
            suma += int.Parse(row["precio_Solicitud"].ToString());
            diasH = int.Parse(row["diasHabiles"].ToString()); //Dias habiles para PDF -- Cambiado 01-08-2021 (Maikol Queupumil  Chile)
        }


        PdfPCell granTotal = new PdfPCell(new Phrase("Total Neto: $" + suma, new Font(Font.NORMAL, 10f, Font.NORMAL)));
        granTotal.Colspan = 5;
        granTotal.HorizontalAlignment = 2;
        table.AddCell(granTotal);

        document.Add(table);
        document.Add(new Paragraph("\n\n"));
        //antes'corresponden a días hábiles' despues 'corresponden a 15 días hábiles' -- Cambiado 01-08-2021 (Maikol Queupumil  Chile) 04Oct2021
        if (diasH == 0) diasH = 15;
        // string nuevo = "Nota 1: Los días estipulados en el plazo de importación tienen una validez que corresponden a " + diasH + " días hábiles y se consideran a partir de la fecha de generación del VFC.";
        string nuevo = "Nota 1: El periodo de validez de los precios cotizados corresponde a " + diasH + " días hábiles y se consideran a partir de la fecha de confirmación por parte del ADV.";
        document.Add(new Paragraph(nuevo, pie));
        document.Add(new Paragraph("\n"));
        string linea = "Nota 2: El valor neto cotizado corresponde al precio lista sugerido, al cual hay que aplicar el descuento por marca + IGV.";
        document.Add(new Paragraph(linea, pie));
        document.Close();

        string pdf_ruta = _path + "\\" + detalleNombre.Trim() + "_" + rut + ".pdf";
        return pdf_ruta;
    }
    public int ObtieneDiasHabiles(string marca)
    {
        int n = 0;
        //datos de tabla diaHabiles


        return n;
    }
    public void LlenarTablaSolicitudComun(string idSession, string solicitud, string rut)
    {

        //Datos Repuestos
        string numero = ""; // numero de solicitud
        string fecha = ""; //fecha solicitud
        string marca = ""; //marca
        string cod = ""; // codigo del repuesto
        string descripcion = ""; //descripcion del repuesto
        string cantidad = ""; //cantidad
        string usuario = ""; //usuario
        string conce = ""; //concesionario
        string tipo = ""; // tipo garantia - normal
        string vin = ""; //vin
        string envio = ""; //tipo envio
        string sesion = ""; //sesion
        string precio = ""; //precio 
        string dia = "";//dias

        //Inserto los datos de la cotización en la tabla COTIZACION_COMUN para su posterior consulta
        _controlBD.InsertarDatos(@"insert into t_CotizaPDF(idSolicitud, sesion, fecha_cotiza, rut_cotiza) values('" + Convert.ToInt32(solicitud) + "', '" + idSession + "', GETDATE(), '" + rut + "' )");


        //Se obtienen los datos desde la tabla temporal LISTA_PRODUCTOS_TEMP
        //y se insertan en la tabla COTIZACION_COMUN_REPUESTO
        DataSet ds = _controlBD.ObtenerDatosFiltrados("select * from t_SolicitudCotizacion_TMP where numeroSolicitud = '" + solicitud + "'");
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            numero = campo["numeroSolicitud"].ToString();
            fecha = campo["fecha"].ToString();
            marca = campo["marca"].ToString();
            cod = campo["codRepto"].ToString();
            descripcion = campo["descripcion"].ToString();
            cantidad = campo["cantidad"].ToString();
            usuario = campo["usuario"].ToString();
            conce = campo["concesionario"].ToString();
            tipo = campo["tipo"].ToString();
            vin = campo["vin"].ToString();
            envio = campo["envio"].ToString();
            sesion = campo["sesion"].ToString();
            dia = campo["dias"].ToString();
            precio = "0";

            _controlBD.InsertarDatos(@"insert into t_SolicitudComunPDFSP(numeroSolicitud, fecha, marca, codRepto, descripcion, cantidad, usuario, concesionario, tipo, vin, envio, sesion, precio, dias)
                values('" + numero + "','" + fecha + "','" + marca + "','" + cod + "','" + descripcion + "','" + cantidad + "','" + usuario + "','" + conce + "', '" + tipo + "' , '" + vin + "','" + envio + "','" + sesion + "','" + precio + "','" + dia + "')");
        }

        ////Se obtiene el id de la cotización y se actualiza la lista de repuestos con el id de la cotizacion para relacionar
        //DataSet dsId = _controlBD.ObtenerDatosFiltrados("select top 1 * from t_CotizaPDF order by idSolicitud desc");
        //string idCot = "";
        //foreach (DataRow campo in dsId.Tables[0].Rows)
        //{
        //    idCot = campo["idSolicitud"].ToString();
        //    _controlBD.InsertarDatos(@"update t_SolicitudComunPDFSP set numeroSolicitud = '" + idCot + "' " +
        //                              " where sesion = '" + idSession + "' ");
        //}

    }

    public string formatearRut(string rut)
    {
        int cont = 0;
        string format;
        if (rut.Length == 0)
        {
            return "";
        }
        else
        {
            rut = rut.Replace(".", "");
            rut = rut.Replace("-", "");
            format = "-" + rut.Substring(rut.Length - 1);
            for (int i = rut.Length - 2; i >= 0; i--)
            {
                format = rut.Substring(i, 1) + format;
                cont++;
                if (cont == 3 && i != 0)
                {
                    format = "." + format;
                    cont = 0;
                }
            }
            return format;
        }
    }

    public static string Dv(string r)
    {
        int suma = 0;
        for (int x = r.Length - 1; x >= 0; x--)
            suma += int.Parse(char.IsDigit(r[x]) ? r[x].ToString() : "0") * (((r.Length - (x + 1)) % 6) + 2);
        int numericDigito = (11 - suma % 11);
        string digito = numericDigito == 11 ? "0" : numericDigito == 10 ? "K" : numericDigito.ToString();
        return digito;
    }

    public string CrearSolicitudPdfMultiSucursal(string ssesionId, string solicitud, string rut, string direccion)
    {
        string querydel = "DELETE t_SolicitudComunPDFSP";
        _ControlBD.EjecutaQuery(querydel);

        //Se crea la cotizacion en la base de datos, se envia el rut y el id de sesión al metodo
        LlenarTablaSolicitudComunFinal2(ssesionId, solicitud, rut);

        //Datos cotizador
        string nombreUser = "";
        string sucursal = "";
        string dealer = "";

        //Se obtiene la fecha con formato dd.mm.aaaa
        DateTime dt = DateTime.Now;
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        Console.WriteLine(dt.ToString("d"));
        CultureInfo ci = new CultureInfo("de-DE");


        //Se declara el documento para crear el pdf
        Document document = new Document(iTextSharp.text.PageSize.LETTER, 50, 50, 25, 25);//tamaño carta

        BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
        string Cotizacion = Guid.NewGuid().ToString();//se le da el nombre al archivo

        //Se declaran tipos de fuentes y sus atributos para que quede sexy ;)
        Font times = new Font(bfTimes, 20, Font.BOLD);
        Font pie = new Font(bfTimes, 9, Font.NORMAL);

        //Detalle COtizacion correo
        DataSet dsDet = _controlBD.ObtenerDatosFiltrados("select top 1 detalle,rut_cotiza  from t_Cotiza where id_cotiza = " + solicitud + " ");
        string detalleNombre = "";
        string rutCotiza2 = "";
        foreach (DataRow campo in dsDet.Tables[0].Rows)
        {
            detalleNombre = campo["detalle"].ToString();
            rutCotiza2 = campo["rut_cotiza"].ToString();

        }

        //Se declara la ruta para obtener el archivo pdf creado
        String ruta = _path + "/" + "../img";
        try
        {

            //Se crea un nuevo archivo PDF
            PdfWriter.GetInstance(document, new FileStream(_path + "\\" + detalleNombre.Trim() + "_" + rut + "_Final2.pdf", FileMode.Create));

        }
        catch (System.IO.IOException ex)
        {
            string errorEx = ex.ToString();
            return errorEx;
        }

        //Se abre el documento para su procesamiento
        document.Open();


        //Se agregan los parametros al documento medinte "document.Add()"      

        //Titulo
        document.Add(new Paragraph("Solicitud de Cotización\n\n", times));


        //Se obtiene el correlativo del documento
        //Se obtiene el id de la cotización y se actualiza la lista de repuestos con el id de la cotizacion para relacionar
        DataSet dsId2 = _controlBD.ObtenerDatosFiltrados("select top 1 * from t_CotizaPDF WHERE idSolicitud ='" + Convert.ToInt32(solicitud) + "' order by idSolicitud desc");
        string correlativo = "";
        foreach (DataRow campo in dsId2.Tables[0].Rows)
        {
            correlativo = campo["idSolicitud"].ToString();
            document.Add(new Paragraph("Folio: " + correlativo));
        }

        //Sp para recuperar Datos de Usuarios cotizador de acuerdo a cotizacion P.Orostegui
        DataSet ds2 = new DataSet();
        ds2 = _controlBD.obtienedealercotiza(1, correlativo);

        //Fin Modificacion P.Orostegui

        //se obtienen los datos del usuario y del dealer
        /*
        DataSet ds2 = _controlBD.ObtenerDatosFiltrados(@"select concesionario.nombreConcesionario,sucursal.direccionSucursal,concesionario.imagen,persona.nombre
                                                from persona,sucursal,concesionario
                                                where persona.rut='" + rut + "' " +
                                                " AND persona.shipCode=sucursal.shipCode " +
                                                " AND sucursal.nombreConcesionario=concesionario.nombreConcesionario " +
                                                " ");*/




        foreach (DataRow campo in ds2.Tables[0].Rows)
        {


            try
            {
                //Se agrega el logo del concesionario obteniendo la ruta de la imagen desde la BD
                iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(_path + "/" + "../doc/imgConcesionarios/" + campo["nombreConcesionario"].ToString() + ".jpg");
                gif.ScalePercent(26f);
                gif.SetAbsolutePosition(document.PageSize.Width - 90f,
                    document.PageSize.Height - 50f);
                gif.ScaleToFit(90f, 50f);
                document.Add(gif);
            }
            catch (System.Net.WebException ex)
            {
                iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(_path + "/" + "../img/noImgExist.png");
                gif.ScalePercent(15f);
                gif.SetAbsolutePosition(document.PageSize.Width - 90f,
                    document.PageSize.Height - 50f);
                //gif.ScaleToFit(250f, 250f);
                document.Add(gif);
            }
            catch (System.IO.DirectoryNotFoundException ex)
            {
                iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(_path + "/" + "../img/noImgExist.png");
                gif.ScalePercent(15f);
                gif.SetAbsolutePosition(document.PageSize.Width - 90f,
                    document.PageSize.Height - 50f);
                //gif.ScaleToFit(250f, 250f);
                document.Add(gif);
            }

            //Sp para recuperar Datos de Usuarios cotizador de acuerdo a cotizacion P.Orostegui


            nombreUser = campo["nombre"].ToString();
            sucursal = campo["direccionSucursal"].ToString();
            dealer = campo["nombreConcesionario"].ToString();


            string digv = Dv(rutCotiza2);
            string rutf = rutCotiza2 + digv;

            document.Add(new Paragraph("Concesionario: " + dealer));
            document.Add(new Paragraph("Cotizador: " + nombreUser));
            document.Add(new Paragraph("Rut Cotizador: " + formatearRut(rutf)));
            document.Add(new Paragraph("Dirección: " + direccion));

        }


        string validez = "";

        DataSet dsseg = _controlBD.ObtenerDatosFiltrados("select top 1 * from t_CotizaPDF where idsolicitud = " + solicitud + " order by idSolicitud desc");
        string idCots = "";
        foreach (DataRow campo in dsseg.Tables[0].Rows)
        {
            idCots = campo["idSolicitud"].ToString();
        }


        DataSet dsLs = _controlBD.ObtenerDatosFiltrados("select * from t_SolicitudComunPDFSP where numeroSolicitud = '" + idCots + "'");
        foreach (DataRow campo in dsLs.Tables[0].Rows)
        {
            if (campo["tipo"].ToString() == "seguro")
            {

                DataSet dataSeguro = _controlBD.ObtenerDatosFiltrados("SELECT dias FROM validezCotizacion WHERE nombre = 'seguro'");
                string diasSeguro = "";
                foreach (DataRow campoSeguro in dataSeguro.Tables[0].Rows)
                {
                    diasSeguro = campoSeguro["dias"].ToString();
                }

                validez = "Esta Cotización tiene una validez de " + diasSeguro + " días a partir de la fecha de creación";
                break;
            }
            else if (campo["tipo"].ToString() == "normal")
            {

                DataSet dataNormal = _controlBD.ObtenerDatosFiltrados("SELECT dias FROM validezCotizacion WHERE nombre = 'normal'");
                string diasNormal = "";
                foreach (DataRow campoNormal in dataNormal.Tables[0].Rows)
                {
                    diasNormal = campoNormal["dias"].ToString();
                }

                validez = "Esta Cotización tiene una validez de " + diasNormal + " días a partir de la fecha de creación";
                break;
            }
            else if (campo["tipo"].ToString() == "garantia")
            {

                DataSet datagarantia = _controlBD.ObtenerDatosFiltrados("SELECT dias FROM validezCotizacion WHERE nombre = 'garantia'");
                string diasgarantia = "";
                foreach (DataRow campogarantia in datagarantia.Tables[0].Rows)
                {
                    diasgarantia = campogarantia["dias"].ToString();
                }

                validez = "Esta Cotización tiene una validez de " + diasgarantia + " días a partir de la fecha de creación";
                break;
            }

        }



        //Obtiene nuevo campo detalle
        string detalle = "";
        DataSet dsdet = _controlBD.ObtenerDatosFiltrados("select detalle from t_Cotiza where id_cotiza = " + solicitud);
        foreach (DataRow campo in dsdet.Tables[0].Rows)
        {
            detalle = campo["detalle"].ToString();
        }


        //Se ingresa la fecha de creacion
        document.Add(new Paragraph("Fecha Creación: " + dt.ToString()));
        document.Add(new Paragraph("Detalle: " + detalle));//Nuevo campo Detalle
        //string validez = "Esta Cotización tiene una validez de 10 días a partir de la fecha de creación";
        //document.Add(new Paragraph(validez, pie));
        document.Add(new Paragraph("\n\n"));


        //Se crea la tabla que contendra los datos de los repuestos consultados
        PdfPTable table = new PdfPTable(14);
        table.WidthPercentage = 100;
        PdfPCell titulo = new PdfPCell(new Phrase("Detalle Solicitud", new Font(Font.NORMAL, 12f, Font.NORMAL)));
        PdfPCell numero = new PdfPCell(new Phrase("Numero Solicitud", new Font(Font.FontFamily.TIMES_ROMAN, 07f, Font.NORMAL)));
        PdfPCell marc = new PdfPCell(new Phrase("Marca", new Font(Font.FontFamily.TIMES_ROMAN, 07f, Font.NORMAL)));
        PdfPCell cant = new PdfPCell(new Phrase("Cantidad", new Font(Font.FontFamily.TIMES_ROMAN, 07f, Font.NORMAL)));
        PdfPCell cod = new PdfPCell(new Phrase("Código", new Font(Font.FontFamily.TIMES_ROMAN, 07f, Font.NORMAL)));
        PdfPCell descrip = new PdfPCell(new Phrase("Descripción", new Font(Font.FontFamily.TIMES_ROMAN, 07f, Font.NORMAL)));
        PdfPCell tipo = new PdfPCell(new Phrase("Tipo de Solicitud", new Font(Font.FontFamily.TIMES_ROMAN, 07f, Font.NORMAL)));
        PdfPCell vin = new PdfPCell(new Phrase("Vin", new Font(Font.FontFamily.TIMES_ROMAN, 07f, Font.NORMAL)));
        PdfPCell envio = new PdfPCell(new Phrase("Envio Solicitado", new Font(Font.FontFamily.TIMES_ROMAN, 07f, Font.NORMAL)));
        PdfPCell dias = new PdfPCell(new Phrase("Plazo Importación (Días)", new Font(Font.FontFamily.TIMES_ROMAN, 07f, Font.NORMAL)));
        PdfPCell precio = new PdfPCell(new Phrase("Precio lista Sugerido Unitario", new Font(Font.FontFamily.TIMES_ROMAN, 07f, Font.NORMAL)));
        PdfPCell anulado = new PdfPCell(new Phrase("Rechazado", new Font(Font.FontFamily.TIMES_ROMAN, 7f, Font.NORMAL)));
        PdfPCell comentario = new PdfPCell(new Phrase("Comentario", new Font(Font.FontFamily.TIMES_ROMAN, 7f, Font.NORMAL)));
        PdfPCell PrecioTotal = new PdfPCell(new Phrase("Valor Total", new Font(Font.FontFamily.TIMES_ROMAN, 7f, Font.NORMAL)));
        //NUEVA MODIFICACION CORREOS VFC
        PdfPCell numVFC = new PdfPCell(new Phrase("Numero VFC", new Font(Font.FontFamily.TIMES_ROMAN, 7f, Font.NORMAL)));

        titulo.Colspan = 14;
        titulo.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
        numero.HorizontalAlignment = 1;
        marc.HorizontalAlignment = 1;
        cant.HorizontalAlignment = 1;
        cod.HorizontalAlignment = 1;
        descrip.HorizontalAlignment = 1;
        tipo.HorizontalAlignment = 1;
        vin.HorizontalAlignment = 1;
        envio.HorizontalAlignment = 1;
        dias.HorizontalAlignment = 1;
        precio.HorizontalAlignment = 1;
        anulado.HorizontalAlignment = 1;
        comentario.HorizontalAlignment = 1;
        PrecioTotal.HorizontalAlignment = 1;
        numVFC.HorizontalAlignment = 1;

        //Agrego los titulos de las columnas en celdas
        table.AddCell(titulo);
        table.AddCell(numero);
        table.AddCell(marc);
        table.AddCell(cant);
        table.AddCell(cod);
        table.AddCell(descrip);
        table.AddCell(tipo);
        table.AddCell(vin);
        table.AddCell(envio);
        table.AddCell(dias);
        table.AddCell(precio);
        table.AddCell(anulado);
        table.AddCell(comentario);
        table.AddCell(PrecioTotal);
        table.AddCell(numVFC);



        //Se obtiene el id de la cotización y se actualiza la lista de repuestos con el id de la cotizacion para relacionar
        DataSet dsId = _controlBD.ObtenerDatosFiltrados("select top 1 * from t_CotizaPDF WHERE idSolicitud = '" + Convert.ToInt32(solicitud) + "' order by idSolicitud desc");
        string idCot = "";
        foreach (DataRow campo in dsId.Tables[0].Rows)
        {
            idCot = campo["idSolicitud"].ToString();
        }

        string EstaAnulada = "";
        //INICIO  : Bloque de codigo dnd agrego un a columna de dias habiles para mostrar en PDF  04Oct2021
        string diasPDF = " ,isnull((Select m.diasHabiles from dbo.marca m where m.[nombreMarca]= marca),'0') as diasHabiles"; //maikol
        //DataSet dsL = _controlBD.ObtenerDatosFiltrados("select a.*,b.num_vfc from t_SolicitudCotizacion a inner join cotizacion_final b on a.numerosolicitud = b.id_cotiza and a.codRepto = b.cod_repuesto where a.numeroSolicitud = '" + idCot + "' AND b.id_session = '" + ssesionId + "'");
        DataSet dsL = _controlBD.ObtenerDatosFiltrados("select a.*,b.num_vfc " + diasPDF + " from t_SolicitudCotizacion a inner join cotizacion_final b on a.numerosolicitud = b.id_cotiza and a.codRepto = b.cod_repuesto where a.numeroSolicitud = '" + idCot + "' AND b.id_session = '" + ssesionId + "'");
        //agrego un a columna de dias habiles para mostrar en PDF  
        //FIN  : Bloque de codigo dnd agrego un a columna de dias habiles para mostrar en PDF 04Oct2021









        foreach (DataRow campo in dsL.Tables[0].Rows)
        {

            if (campo["anulada"].ToString() == "True")
            {
                EstaAnulada = "SI";
            }
            else
            {
                EstaAnulada = "NO";
            }

            //PdfPCell _cod = new PdfPCell(new Phrase(codigo, new Font(Font.FontFamily.COURIER, 9f, Font.NORMAL)));
            PdfPCell _numero = new PdfPCell(new Phrase(campo["numeroSolicitud"].ToString(), new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));
            PdfPCell _marca = new PdfPCell(new Phrase(campo["marca"].ToString(), new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));
            PdfPCell _cant = new PdfPCell(new Phrase(campo["cantidad"].ToString(), new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));
            PdfPCell _cod = new PdfPCell(new Phrase(campo["codRepto"].ToString(), new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));
            PdfPCell _descrip = new PdfPCell(new Phrase(campo["descripcion"].ToString(), new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));
            PdfPCell _tipo = new PdfPCell(new Phrase(campo["tipo"].ToString(), new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));
            PdfPCell _vin = new PdfPCell(new Phrase(campo["vin"].ToString(), new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));
            PdfPCell _envio = new PdfPCell(new Phrase(campo["envio"].ToString(), new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));
            PdfPCell _dias = new PdfPCell(new Phrase(campo["dias"].ToString(), new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));
            PdfPCell _precio = new PdfPCell(new Phrase(campo["preciounitario"].ToString(), new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));
            PdfPCell _anulado = new PdfPCell(new Phrase(EstaAnulada, new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));
            PdfPCell _comentario = new PdfPCell(new Phrase(campo["comentario"].ToString(), new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));
            PdfPCell _preciototal = new PdfPCell(new Phrase(campo["precio_solicitud"].ToString(), new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));
            PdfPCell _num_VFC = new PdfPCell(new Phrase(campo["num_VFC"].ToString(), new Font(Font.FontFamily.COURIER, 6f, Font.NORMAL)));

            _numero.HorizontalAlignment = 1;
            _marca.HorizontalAlignment = 1;
            _cant.HorizontalAlignment = 1;
            _cant.HorizontalAlignment = 1;
            _descrip.HorizontalAlignment = 1;
            _tipo.HorizontalAlignment = 1;
            _vin.HorizontalAlignment = 1;
            _envio.HorizontalAlignment = 1;
            _dias.HorizontalAlignment = 1;
            _precio.HorizontalAlignment = 1;
            _anulado.HorizontalAlignment = 1;
            _comentario.HorizontalAlignment = 1;
            _preciototal.HorizontalAlignment = 1;
            _num_VFC.HorizontalAlignment = 1;

            table.AddCell(_numero);
            table.AddCell(_marca);
            table.AddCell(_cant);
            table.AddCell(_cod);
            table.AddCell(_descrip);
            table.AddCell(_tipo);
            table.AddCell(_vin);
            table.AddCell(_envio);
            table.AddCell(_dias);
            table.AddCell(_precio);
            table.AddCell(_anulado);
            table.AddCell(_comentario);
            table.AddCell(_preciototal);
            table.AddCell(_num_VFC);
        }

        int diasH = 0;
        int suma1 = 0;
        string val = "";
        // DataSet dsT = _controlBD.ObtenerDatosFiltrados("select *   from t_SolicitudComunPDFSP where numeroSolicitud = '" + idCot + "'");

        DataSet dsT = _controlBD.ObtenerDatosFiltrados("select * " + diasPDF + " from t_SolicitudComunPDFSP where numeroSolicitud = '" + idCot + "'");



        foreach (DataRow row in dsT.Tables[0].Rows)
        {
            val = (row["precio"].ToString());
            val = val.Replace(".", "");
            suma1 += int.Parse(val);
            diasH = int.Parse(row["diasHabiles"].ToString()); //Dias habiles para PDF -- Cambiado 01-08-2021 (Maikol Queupumil  Chile)
        }
        string suma = FormatoValor(Convert.ToString(suma1));
        PdfPCell granTotal = new PdfPCell(new Phrase("Total Neto: $" + suma, new Font(Font.NORMAL, 10f, Font.NORMAL)));
        granTotal.Colspan = 10;
        granTotal.HorizontalAlignment = 2;
        table.AddCell(granTotal);

        document.Add(table);
        document.Add(new Paragraph("\n\n"));
        //antes'corresponden a días hábiles' despues 'corresponden a 15 días hábiles' -- Cambiado 01-08-2021 (Maikol Queupumil  Chile)
        if (diasH == 0) diasH = 15;
        // string nuevo = "Nota 1: Los días estipulados en el plazo de importación tienen una validez que corresponden a " + diasH + " días hábiles y se consideran a partir de la fecha de generación del VFC.";
        string nuevo = "Nota 1: El periodo de validez de los precios cotizados corresponde a " + diasH + " días hábiles y se consideran a partir de la fecha de confirmación por parte del ADV.";
        document.Add(new Paragraph(nuevo, pie));
        document.Add(new Paragraph("\n"));
        string linea = "Nota 2: El valor neto cotizado corresponde al precio lista sugerido, al cual hay que aplicar el descuento por marca + IGV.";
        document.Add(new Paragraph(linea, pie));
        document.Close();

        string pdf_ruta = _path + "\\" + detalleNombre.Trim() + "_" + rut + "_Final2.pdf";
        return pdf_ruta;

    }

    public void LlenarTablaSolicitudComunFinal2(string idSession, string solicitud, string rut)
    {

        //Datos Repuestos
        string numero = ""; // numero de solicitud
        string fecha = ""; //fecha solicitud
        string marca = ""; //marca
        string cod = ""; // codigo del repuesto
        string descripcion = ""; //descripcion del repuesto
        string cantidad = ""; //cantidad
        string usuario = ""; //usuario
        string conce = ""; //concesionario
        string tipo = ""; // tipo garantia - normal
        string vin = ""; //vin
        string envio = ""; //tipo envio
        string sesion = ""; //sesion
        string precio; //precio
        string dia = "";//dias de envio

        //Inserto los datos de la cotización en la tabla COTIZACION_COMUN para su posterior consulta
        _controlBD.InsertarDatos(@"insert into t_CotizaPDF(idSolicitud, sesion, fecha_cotiza, rut_cotiza) values('" + Convert.ToInt32(solicitud) + "', '" + idSession + "', GETDATE(), '" + rut + "' )");


        //Se obtienen los datos desde la tabla temporal LISTA_PRODUCTOS_TEMP
        //y se insertan en la tabla COTIZACION_COMUN_REPUESTO

        DataSet ds = _controlBD.ObtenerDatosFiltrados("select a.* from t_SolicitudCotizacion a inner join cotizacion_final b on a.numerosolicitud = b.id_cotiza and a.codRepto = b.cod_repuesto where a.numeroSolicitud = '" + solicitud + "' AND b.id_session = '" + idSession + "'");
        foreach (DataRow campo in ds.Tables[0].Rows)
        {
            numero = campo["numeroSolicitud"].ToString();
            fecha = campo["fecha"].ToString();
            marca = campo["marca"].ToString();
            cod = campo["codRepto"].ToString();
            descripcion = campo["descripcion"].ToString();
            cantidad = campo["cantidad"].ToString();
            usuario = campo["usuario"].ToString();
            conce = campo["concesionario"].ToString();
            tipo = campo["tipo"].ToString();
            vin = campo["vin"].ToString();
            envio = campo["envio"].ToString();
            sesion = campo["sesion"].ToString();
            precio = FormatoValor(campo["precio_Solicitud"].ToString());
            dia = campo["dias"].ToString();

            _controlBD.InsertarDatos(@"insert into t_SolicitudComunPDFSP(numeroSolicitud, fecha, marca, codRepto, descripcion, cantidad, usuario, concesionario, tipo, vin, envio, sesion, precio, dias)
                values('" + numero + "','" + fecha + "','" + marca + "','" + cod + "','" + descripcion + "','" + cantidad + "','" + usuario + "','" + conce + "', '" + tipo + "' , '" + vin + "','" + envio + "','" + sesion + "','" + precio + "','" + dia + "')");
        }
    }

    public string CrearSolicitudPdfComunFinal2(string ssesionId, string solicitud, string rut)
    {
        string querydel = "DELETE t_SolicitudComunPDFSP";
        _ControlBD.EjecutaQuery(querydel);

        //Se crea la cotizacion en la base de datos, se envia el rut y el id de sesión al metodo
        LlenarTablaSolicitudComunFinal2(ssesionId, solicitud, rut);

        //Datos cotizador
        string nombreUser = "";
        string sucursal = "";
        string dealer = "";
        string direccion = "";

        //Se obtiene la fecha con formato dd.mm.aaaa
        DateTime dt = DateTime.Now;
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        Console.WriteLine(dt.ToString("d"));
        CultureInfo ci = new CultureInfo("de-DE");


        //Se declara el documento para crear el pdf
        Document document = new Document(iTextSharp.text.PageSize.LETTER, 50, 50, 25, 25);//tamaño carta

        BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
        string Cotizacion = Guid.NewGuid().ToString();//se le da el nombre al archivo

        //Se declaran tipos de fuentes y sus atributos para que quede sexy ;)
        Font times = new Font(bfTimes, 20, Font.BOLD);
        Font pie = new Font(bfTimes, 9, Font.NORMAL);

        //Detalle COtizacion correo
        DataSet dsDet = _controlBD.ObtenerDatosFiltrados("select top 1 detalle,rut_cotiza  from t_Cotiza where id_cotiza = " + solicitud + " ");
        string detalleNombre = "";
        string rutCotiza2 = "";
        foreach (DataRow campo in dsDet.Tables[0].Rows)
        {
            detalleNombre = campo["detalle"].ToString();
            rutCotiza2 = campo["rut_cotiza"].ToString();

        }

        //Se declara la ruta para obtener el archivo pdf creado
        String ruta = _path + "/" + "../img";
        try
        {

            //Se crea un nuevo archivo PDF
            PdfWriter.GetInstance(document, new FileStream(_path + "\\" + detalleNombre.Trim() + "_" + rut + "_Final2.pdf", FileMode.Create));

        }
        catch (System.IO.IOException ex)
        {
            string errorEx = ex.ToString();
            return errorEx;
        }

        //Se abre el documento para su procesamiento
        document.Open();


        //Se agregan los parametros al documento medinte "document.Add()"      

        //Titulo
        document.Add(new Paragraph("Solicitud de Cotización\n\n", times));


        //Se obtiene el correlativo del documento
        //Se obtiene el id de la cotización y se actualiza la lista de repuestos con el id de la cotizacion para relacionar
        DataSet dsId2 = _controlBD.ObtenerDatosFiltrados("select top 1 * from t_CotizaPDF WHERE idSolicitud ='" + Convert.ToInt32(solicitud) + "' order by idSolicitud desc");
        string correlativo = "";
        foreach (DataRow campo in dsId2.Tables[0].Rows)
        {
            correlativo = campo["idSolicitud"].ToString();
            document.Add(new Paragraph("Folio: " + correlativo));
        }

        //Sp para recuperar Datos de Usuarios cotizador de acuerdo a cotizacion P.Orostegui
        DataSet ds2 = new DataSet();
        ds2 = _controlBD.obtienedealercotiza(1, correlativo);

        //Fin Modificacion P.Orostegui

        //se obtienen los datos del usuario y del dealer
        /*
        DataSet ds2 = _controlBD.ObtenerDatosFiltrados(@"select concesionario.nombreConcesionario,sucursal.direccionSucursal,concesionario.imagen,persona.nombre
                                                from persona,sucursal,concesionario
                                                where persona.rut='" + rut + "' " +
                                                " AND persona.shipCode=sucursal.shipCode " +
                                                " AND sucursal.nombreConcesionario=concesionario.nombreConcesionario " +
                                                " ");*/




        foreach (DataRow campo in ds2.Tables[0].Rows)
        {


            try
            {
                //Se agrega el logo del concesionario obteniendo la ruta de la imagen desde la BD
                iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(_path + "/" + "../doc/imgConcesionarios/" + campo["nombreConcesionario"].ToString() + ".jpg");
                gif.ScalePercent(26f);
                gif.SetAbsolutePosition(document.PageSize.Width - 90f,
                    document.PageSize.Height - 50f);
                gif.ScaleToFit(90f, 50f);
                document.Add(gif);
            }
            catch (System.Net.WebException ex)
            {
                iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(_path + "/" + "../img/noImgExist.png");
                gif.ScalePercent(15f);
                gif.SetAbsolutePosition(document.PageSize.Width - 90f,
                    document.PageSize.Height - 50f);
                //gif.ScaleToFit(250f, 250f);
                document.Add(gif);
            }
            catch (System.IO.DirectoryNotFoundException ex)
            {
                iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(_path + "/" + "../img/noImgExist.png");
                gif.ScalePercent(15f);
                gif.SetAbsolutePosition(document.PageSize.Width - 90f,
                    document.PageSize.Height - 50f);
                //gif.ScaleToFit(250f, 250f);
                document.Add(gif);
            }

            //Sp para recuperar Datos de Usuarios cotizador de acuerdo a cotizacion P.Orostegui


            nombreUser = campo["nombre"].ToString();
            sucursal = campo["direccionSucursal"].ToString();
            dealer = campo["nombreConcesionario"].ToString();


            string digv = Dv(rutCotiza2);
            string rutf = rutCotiza2 + digv;

            document.Add(new Paragraph("Concesionario: " + dealer));
            document.Add(new Paragraph("Cotizador: " + nombreUser));
            document.Add(new Paragraph("Rut Cotizador: " + formatearRut(rutf)));
            document.Add(new Paragraph("Dirección: " + sucursal));

        }


        string validez = "";

        DataSet dsseg = _controlBD.ObtenerDatosFiltrados("select top 1 * from t_CotizaPDF where idsolicitud = " + solicitud + " order by idSolicitud desc");
        string idCots = "";
        foreach (DataRow campo in dsseg.Tables[0].Rows)
        {
            idCots = campo["idSolicitud"].ToString();
        }


        DataSet dsLs = _controlBD.ObtenerDatosFiltrados("select * from t_SolicitudComunPDFSP where numeroSolicitud = '" + idCots + "'");
        foreach (DataRow campo in dsLs.Tables[0].Rows)
        {
            if (campo["tipo"].ToString() == "seguro")
            {

                DataSet dataSeguro = _controlBD.ObtenerDatosFiltrados("SELECT dias FROM validezCotizacion WHERE nombre = 'seguro'");
                string diasSeguro = "";
                foreach (DataRow campoSeguro in dataSeguro.Tables[0].Rows)
                {
                    diasSeguro = campoSeguro["dias"].ToString();
                }

                validez = "Esta Cotización tiene una validez de " + diasSeguro + " días a partir de la fecha de creación";
                break;
            }
            else if (campo["tipo"].ToString() == "normal")
            {

                DataSet dataNormal = _controlBD.ObtenerDatosFiltrados("SELECT dias FROM validezCotizacion WHERE nombre = 'normal'");
                string diasNormal = "";
                foreach (DataRow campoNormal in dataNormal.Tables[0].Rows)
                {
                    diasNormal = campoNormal["dias"].ToString();
                }

                validez = "Esta Cotización tiene una validez de " + diasNormal + " días a partir de la fecha de creación";
                break;
            }
            else if (campo["tipo"].ToString() == "garantia")
            {

                DataSet datagarantia = _controlBD.ObtenerDatosFiltrados("SELECT dias FROM validezCotizacion WHERE nombre = 'garantia'");
                string diasgarantia = "";
                foreach (DataRow campogarantia in datagarantia.Tables[0].Rows)
                {
                    diasgarantia = campogarantia["dias"].ToString();
                }

                validez = "Esta Cotización tiene una validez de " + diasgarantia + " días a partir de la fecha de creación";
                break;
            }

        }



        //Obtiene nuevo campo detalle
        string detalle = "";
        DataSet dsdet = _controlBD.ObtenerDatosFiltrados("select detalle from t_Cotiza where id_cotiza = " + solicitud);
        foreach (DataRow campo in dsdet.Tables[0].Rows)
        {
            detalle = campo["detalle"].ToString();
        }


        //Se ingresa la fecha de creacion
        document.Add(new Paragraph("Fecha Creación: " + dt.ToString()));
        document.Add(new Paragraph("Detalle: " + detalle));//Nuevo campo Detalle
        //string validez = "Esta Cotización tiene una validez de 10 días a partir de la fecha de creación";
        //document.Add(new Paragraph(validez, pie));
        document.Add(new Paragraph("\n\n"));


        //Se crea la tabla que contendra los datos de los repuestos consultados
        PdfPTable table = new PdfPTable(14);
        table.WidthPercentage = 100;
        PdfPCell titulo = new PdfPCell(new Phrase("Detalle Solicitud", new Font(Font.NORMAL, 12f, Font.NORMAL)));
        PdfPCell numero = new PdfPCell(new Phrase("Numero Solicitud", new Font(Font.FontFamily.TIMES_ROMAN, 07f, Font.NORMAL)));
        PdfPCell marc = new PdfPCell(new Phrase("Marca", new Font(Font.FontFamily.TIMES_ROMAN, 07f, Font.NORMAL)));
        PdfPCell cant = new PdfPCell(new Phrase("Cantidad", new Font(Font.FontFamily.TIMES_ROMAN, 07f, Font.NORMAL)));
        PdfPCell cod = new PdfPCell(new Phrase("Código", new Font(Font.FontFamily.TIMES_ROMAN, 07f, Font.NORMAL)));
        PdfPCell descrip = new PdfPCell(new Phrase("Descripción", new Font(Font.FontFamily.TIMES_ROMAN, 07f, Font.NORMAL)));
        PdfPCell tipo = new PdfPCell(new Phrase("Tipo de Solicitud", new Font(Font.FontFamily.TIMES_ROMAN, 07f, Font.NORMAL)));
        PdfPCell vin = new PdfPCell(new Phrase("Vin", new Font(Font.FontFamily.TIMES_ROMAN, 07f, Font.NORMAL)));
        PdfPCell envio = new PdfPCell(new Phrase("Envio Solicitado", new Font(Font.FontFamily.TIMES_ROMAN, 07f, Font.NORMAL)));
        PdfPCell dias = new PdfPCell(new Phrase("Plazo Importación (Días)", new Font(Font.FontFamily.TIMES_ROMAN, 07f, Font.NORMAL)));
        PdfPCell precio = new PdfPCell(new Phrase("Precio lista Sugerido Unitario", new Font(Font.FontFamily.TIMES_ROMAN, 07f, Font.NORMAL)));
        PdfPCell anulado = new PdfPCell(new Phrase("Rechazado", new Font(Font.FontFamily.TIMES_ROMAN, 7f, Font.NORMAL)));
        PdfPCell comentario = new PdfPCell(new Phrase("Comentario", new Font(Font.FontFamily.TIMES_ROMAN, 7f, Font.NORMAL)));
        PdfPCell PrecioTotal = new PdfPCell(new Phrase("Valor Total", new Font(Font.FontFamily.TIMES_ROMAN, 7f, Font.NORMAL)));
        //NUEVA MODIFICACION CORREOS VFC
        PdfPCell numVFC = new PdfPCell(new Phrase("Numero VFC", new Font(Font.FontFamily.TIMES_ROMAN, 7f, Font.NORMAL)));

        titulo.Colspan = 14;
        titulo.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
        numero.HorizontalAlignment = 1;
        marc.HorizontalAlignment = 1;
        cant.HorizontalAlignment = 1;
        cod.HorizontalAlignment = 1;
        descrip.HorizontalAlignment = 1;
        tipo.HorizontalAlignment = 1;
        vin.HorizontalAlignment = 1;
        envio.HorizontalAlignment = 1;
        dias.HorizontalAlignment = 1;
        precio.HorizontalAlignment = 1;
        anulado.HorizontalAlignment = 1;
        comentario.HorizontalAlignment = 1;
        PrecioTotal.HorizontalAlignment = 1;
        numVFC.HorizontalAlignment = 1;

        //Agrego los titulos de las columnas en celdas
        table.AddCell(titulo);
        table.AddCell(numero);
        table.AddCell(marc);
        table.AddCell(cant);
        table.AddCell(cod);
        table.AddCell(descrip);
        table.AddCell(tipo);
        table.AddCell(vin);
        table.AddCell(envio);
        table.AddCell(dias);
        table.AddCell(precio);
        table.AddCell(anulado);
        table.AddCell(comentario);
        table.AddCell(PrecioTotal);
        table.AddCell(numVFC);



        //Se obtiene el id de la cotización y se actualiza la lista de repuestos con el id de la cotizacion para relacionar
        DataSet dsId = _controlBD.ObtenerDatosFiltrados("select top 1 * from t_CotizaPDF WHERE idSolicitud = '" + Convert.ToInt32(solicitud) + "' order by idSolicitud desc");
        string idCot = "";
        foreach (DataRow campo in dsId.Tables[0].Rows)
        {
            idCot = campo["idSolicitud"].ToString();
        }

        string EstaAnulada = "";


        //INICIO  : Bloque de codigo dnd agrego un a columna de dias habiles para mostrar en PDF  04Oct2021
        string diasPDF = " ,isnull((Select m.diasHabiles from dbo.marca m where m.[nombreMarca]= marca),'0') as diasHabiles"; //maikol
        //DataSet dsL = _controlBD.ObtenerDatosFiltrados("select a.*,b.num_vfc from t_SolicitudCotizacion a inner join cotizacion_final b on a.numerosolicitud = b.id_cotiza and a.codRepto = b.cod_repuesto where a.numeroSolicitud = '" + idCot + "' AND b.id_session = '" + ssesionId + "'");
        DataSet dsL = _controlBD.ObtenerDatosFiltrados("select a.*,b.num_vfc " + diasPDF + " from t_SolicitudCotizacion a inner join cotizacion_final b on a.numerosolicitud = b.id_cotiza and a.codRepto = b.cod_repuesto where a.numeroSolicitud = '" + idCot + "' AND b.id_session = '" + ssesionId + "'");
        //agrego un a columna de dias habiles para mostrar en PDF  
        //FIN  : Bloque de codigo dnd agrego un a columna de dias habiles para mostrar en PDF 04Oct2021




        foreach (DataRow campo in dsL.Tables[0].Rows)
        {

            if (campo["anulada"].ToString() == "True")
            {
                EstaAnulada = "SI";
            }
            else
            {
                EstaAnulada = "NO";
            }

            //PdfPCell _cod = new PdfPCell(new Phrase(codigo, new Font(Font.FontFamily.COURIER, 9f, Font.NORMAL)));
            PdfPCell _numero = new PdfPCell(new Phrase(campo["numeroSolicitud"].ToString(), new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));
            PdfPCell _marca = new PdfPCell(new Phrase(campo["marca"].ToString(), new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));
            PdfPCell _cant = new PdfPCell(new Phrase(campo["cantidad"].ToString(), new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));
            PdfPCell _cod = new PdfPCell(new Phrase(campo["codRepto"].ToString(), new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));
            PdfPCell _descrip = new PdfPCell(new Phrase(campo["descripcion"].ToString(), new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));
            PdfPCell _tipo = new PdfPCell(new Phrase(campo["tipo"].ToString(), new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));
            PdfPCell _vin = new PdfPCell(new Phrase(campo["vin"].ToString(), new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));
            PdfPCell _envio = new PdfPCell(new Phrase(campo["envio"].ToString(), new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));
            PdfPCell _dias = new PdfPCell(new Phrase(campo["dias"].ToString(), new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));
            PdfPCell _precio = new PdfPCell(new Phrase(campo["preciounitario"].ToString(), new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));
            PdfPCell _anulado = new PdfPCell(new Phrase(EstaAnulada, new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));
            PdfPCell _comentario = new PdfPCell(new Phrase(campo["comentario"].ToString(), new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));
            PdfPCell _preciototal = new PdfPCell(new Phrase(campo["precio_solicitud"].ToString(), new Font(Font.FontFamily.COURIER, 7f, Font.NORMAL)));
            PdfPCell _num_VFC = new PdfPCell(new Phrase(campo["num_VFC"].ToString(), new Font(Font.FontFamily.COURIER, 6f, Font.NORMAL)));

            _numero.HorizontalAlignment = 1;
            _marca.HorizontalAlignment = 1;
            _cant.HorizontalAlignment = 1;
            _cant.HorizontalAlignment = 1;
            _descrip.HorizontalAlignment = 1;
            _tipo.HorizontalAlignment = 1;
            _vin.HorizontalAlignment = 1;
            _envio.HorizontalAlignment = 1;
            _dias.HorizontalAlignment = 1;
            _precio.HorizontalAlignment = 1;
            _anulado.HorizontalAlignment = 1;
            _comentario.HorizontalAlignment = 1;
            _preciototal.HorizontalAlignment = 1;
            _num_VFC.HorizontalAlignment = 1;

            table.AddCell(_numero);
            table.AddCell(_marca);
            table.AddCell(_cant);
            table.AddCell(_cod);
            table.AddCell(_descrip);
            table.AddCell(_tipo);
            table.AddCell(_vin);
            table.AddCell(_envio);
            table.AddCell(_dias);
            table.AddCell(_precio);
            table.AddCell(_anulado);
            table.AddCell(_comentario);
            table.AddCell(_preciototal);
            table.AddCell(_num_VFC);
        }
        int diasH = 0;
        int suma1 = 0;
        string val = "";
        //DataSet dsT = _controlBD.ObtenerDatosFiltrados("select * from t_SolicitudComunPDFSP where numeroSolicitud = '" + idCot + "'");
        DataSet dsT = _controlBD.ObtenerDatosFiltrados("select * " + diasPDF + " from t_SolicitudComunPDFSP where numeroSolicitud = '" + idCot + "'");


        foreach (DataRow row in dsT.Tables[0].Rows)
        {
            val = (row["precio"].ToString());
            val = val.Replace(".", "");
            suma1 += int.Parse(val);
            diasH = int.Parse(row["diasHabiles"].ToString()); //Dias habiles para PDF -- Cambiado 01-08-2021 (Maikol Queupumil  Chile)
        }
        string suma = FormatoValor(Convert.ToString(suma1));
        PdfPCell granTotal = new PdfPCell(new Phrase("Total Neto: $" + suma, new Font(Font.NORMAL, 10f, Font.NORMAL)));
        granTotal.Colspan = 10;
        granTotal.HorizontalAlignment = 2;
        table.AddCell(granTotal);

        document.Add(table);
        document.Add(new Paragraph("\n\n"));
        //antes'corresponden a días hábiles' despues 'corresponden a 15 días hábiles' -- Cambiado 01-08-2021 (Maikol Queupumil  Chile)
        //antes'corresponden a días hábiles' despues 'corresponden a 15 días hábiles' -- Cambiado 01-08-2021 (Maikol Queupumil  Chile) 04Oct2021
        if (diasH == 0) diasH = 15;
        // string nuevo = "Nota 1: Los días estipulados en el plazo de importación tienen una validez que corresponden a " + diasH + " días hábiles y se consideran a partir de la fecha de generación del VFC.";
        string nuevo = "Nota 1: El periodo de validez de los precios cotizados corresponde a " + diasH + " días hábiles y se consideran a partir de la fecha de confirmación por parte del ADV.";
        document.Add(new Paragraph(nuevo, pie));
        document.Add(new Paragraph("\n"));
        string linea = "Nota 2: El valor neto cotizado corresponde al precio lista sugerido, al cual hay que aplicar el descuento por marca + IGV.";
        document.Add(new Paragraph(linea, pie));
        document.Close();

        string pdf_ruta = _path + "\\" + detalleNombre.Trim() + "_" + rut + "_Final2.pdf";
        return pdf_ruta;

    }
}