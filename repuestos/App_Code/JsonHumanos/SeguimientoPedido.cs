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
/// Descripción breve de SeguimientoPedido
/// </summary>
public class SeguimientoPedido
{
    public ModeloConsultaVFC SeguimientoPedidoVFC(string XMLData)
    {
        ModeloConsultaVFC RespuestaServicio = null;

        try
        {

            string url = ConfigurationManager.AppSettings["SeguimientoPedidoHumanos2"];
            string Token = ConfigurationManager.AppSettings["TokenSeguimientoPedido"];

            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

            JObject InfoApi;
            string respuesta = "";
            string estado = "";


            string tokenSAS = Token;
            string xmlData = XMLData;

            string Parametros = "tokenSAS=" + tokenSAS + "&xmlData=" + XMLData;


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
                estado = InfoApi["seguimientos"].ToString();
                RespuestaServicio = JsonConvert.DeserializeObject<ModeloConsultaVFC>(respuesta);
            }

            return RespuestaServicio;

        }
        catch (Exception ex)
        {
            RespuestaServicio.error = "An error has occurred.";
            return RespuestaServicio;
        }
    }
}