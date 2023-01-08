using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

/// <summary>
/// Descripción breve de SeguimientoPedidoFiltro
/// </summary>
public class SeguimientoPedidoFiltro
{
    public ModeloConsultaRepuestos SeguimientoPedidoFiltroConsulta(string XMLData)
    {
        ModeloConsultaRepuestos ConsultaRpto = null;

        try
        {

            string url = ConfigurationManager.AppSettings["SeguimientoRepuestosHumanos2"];
            string Token = ConfigurationManager.AppSettings["TokenSolicitudRepuesto"];

            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

            JObject InfoApi;
            string respuesta = "";
            string estado = "";


            string tokenSAS = Token;
            string xmlData = XMLData;

            string Parametros = "tokenSAS=" + tokenSAS + "&xmlData=" + xmlData;


            WebRequest oRequest = WebRequest.Create(url) as HttpWebRequest;
            oRequest.Method = "POST";
            oRequest.ContentType = "application/x-www-form-urlencoded";




            using (var oSW = new StreamWriter(oRequest.GetRequestStream()))
            {
                oSW.Write(Parametros);
                oSW.Flush();
                oSW.Close();
            }

            HttpWebResponse oResponse = oRequest.GetResponse() as HttpWebResponse;

            using (var oSR = new StreamReader(oResponse.GetResponseStream()))
            {
                respuesta = oSR.ReadToEnd().Trim();
                InfoApi = JObject.Parse(respuesta);
                estado = InfoApi["solicitudes"].ToString();
                ConsultaRpto = JsonConvert.DeserializeObject<ModeloConsultaRepuestos>(respuesta);
            }

            return ConsultaRpto;

        }
        catch (Exception ex)
        {
            ConsultaRpto.error = "An error has occurred.";
            return ConsultaRpto;
        }

    }
}