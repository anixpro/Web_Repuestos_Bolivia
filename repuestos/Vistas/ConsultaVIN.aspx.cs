using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using log4net.Config;

public partial class Vistas_ConsultaVIN : System.Web.UI.Page
{
    public string ano;
    public string vin;
    public string chasis;
    public string marca;
    public string modelo;
    public string version;

    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_ConsultaVIN));

    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;
        ControlVfc cVFC = new ControlVfc();
        cVFC.Vin = Request.QueryString["vin"];
        cVFC.ObtieneDatosVfc();
        ano = "";//cVFC.getAnio();
        vin = cVFC.getVin();
        chasis = Request.QueryString["vin"];
        marca = "";// cVFC.getMarca();
        modelo = "";// cVFC.getModelo();
      //  version = cVFC.getVersion();

        VIN.SI_Consulta_VehiculosService consulta = new VIN.SI_Consulta_VehiculosService();
        VIN.DT_Consulta_Vehiculos_RequestVehiculos consultaReq = new VIN.DT_Consulta_Vehiculos_RequestVehiculos();
        VIN.DT_Consulta_Vehiculos_RequestVehiculos[] consultaReq_2 = new VIN.DT_Consulta_Vehiculos_RequestVehiculos[1];
        VIN.DT_Consulta_Vehiculos_Response VinResponse = new VIN.DT_Consulta_Vehiculos_Response();
        VIN.DT_Consulta_Vehiculos_ResponseCaracteristicas_Vehiculos VinResponseCarc = new VIN.DT_Consulta_Vehiculos_ResponseCaracteristicas_Vehiculos();
        VIN.DT_Consulta_Vehiculos_ResponseT_RETURN VinResponseCarcRet = new VIN.DT_Consulta_Vehiculos_ResponseT_RETURN();



        consulta.Credentials = new System.Net.NetworkCredential("INT_SPI_SBCL", "5k82017PoPcl");
        consulta.PreAuthenticate = true;

        consultaReq.VHVIN = Request.QueryString["vin"];
        consultaReq_2[0] = consultaReq;

        VinResponse = consulta.Consulta_Vehiculos(consultaReq_2);

        foreach (var wsdl in VinResponse.Caracteristicas_Vehiculos)
        {
            string NomMarca = wsdl.NOMBRE.ToString();

            if (NomMarca == "SKB_MARCA")
            {
                marca = wsdl.VALOR.ToString();

                if (marca == "MITSUBISHI FUSO")
                {
                    marca = "";
                    marca = "FUSO";
                }
            }

            if (NomMarca == "SKB_ANO_MODELO")
            {
                ano = wsdl.VALOR.ToString();
            }

            if (NomMarca == "SKB_FAMILIA")
            {
                modelo = wsdl.VALOR.ToString();
            }
                      
        }

    }
}