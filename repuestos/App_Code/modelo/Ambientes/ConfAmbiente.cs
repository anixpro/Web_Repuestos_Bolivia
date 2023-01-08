using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

/// <summary>
/// Descripción breve de ConfAmbiente
/// </summary>
public class ConfAmbiente
{
    
    
    public static string user = "";
    public static string pass = "";
    public static string ambiente = "";
    public static string version = "";
    public static string ambienteWebConf = ConfigurationManager.AppSettings["ambiente"].ToString();

	public static void ConfCredenciales()
	{
        version = "1.0.1.2";

        if (ambienteWebConf == "p")
        {
            //pass = "InSoa2015_P";
            //user = "INTSOPI_SKBP";
            pass = "5k82017PoPpe";
            user = "INT_REP_SKBP";
            ambiente = "ERP";
        }
        else if (ambienteWebConf == "q")
        {
            //pass = "SKB2015pi";
            //user = "INTSOAPI_SKB";

            //pass = "InSoa2015_P";
            //user = "INTSOPI_SKBP";

            //pass = "skb2017poq";
            //user = "INT_RPTOS_SKB";

            pass = "skb2017poq";
            user = "INT_WTY_SKB";

            ambiente = "ERQ";

        }
        else if (ambienteWebConf == "d")
        {
            //pass = "InSoa2015";
            //user = "insoapi_sk";
            pass = "InSoa2015_P";
            user = "INSOAPI_SK";
            ambiente = "ERD";
        }
	}
}