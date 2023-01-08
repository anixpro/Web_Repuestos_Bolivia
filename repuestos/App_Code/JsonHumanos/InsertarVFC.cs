using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
/// Descripción breve de InsertarVFC
/// </summary>
public class InsertarVFC
{
    public  ModeloRespuestaInsertaVFC InsertarVFCHuanos(string XMLData)
    {
        ModeloRespuestaInsertaVFC RespuestaServicio = null;

        try
        {

            string url = ConfigurationManager.AppSettings["InsertaPedidoVFCHumanos2"];
            string Token = ConfigurationManager.AppSettings["TokenInsertaVFC"];

            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

            JObject InfoApi;
            string respuesta = "";
            string estado = "";


            string tokenSAS = Token;
            string xmlData = XMLData;

            string Parametros= "tokenSAS="+tokenSAS+"&xmlData="+ XMLData;
            

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
                estado = InfoApi["idEntityH2"].ToString();
                RespuestaServicio = JsonConvert.DeserializeObject<ModeloRespuestaInsertaVFC>(respuesta);
            }

            return RespuestaServicio;

        }
        catch (Exception ex)
        {
            RespuestaServicio.Message = "An error has occurred.";
            return RespuestaServicio;
        }

    }
}