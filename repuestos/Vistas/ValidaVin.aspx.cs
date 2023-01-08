using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using log4net.Config;

public partial class ValidaVin : System.Web.UI.Page
{
    public string ano;
    public string vin;
    public string chasis;
    public string marca;
    public string modelo;
    public string version;


    protected void Page_Load(object sender, EventArgs e)
    {
        ControlVfc cVFC = new ControlVfc();
        cVFC.Vin = Request.QueryString["vin"];
		marca = Request.QueryString["marca"];
		/*Vin[] VinArr = cVFC.ObtieneVINS();
		if (VinArr.Count() > 0){
			Response.Write("true");
		}else	Response.Write("false");*/
		cVFC.ObtieneDatosVfc();
        //ano = cVFC.getAnio();
        //vin = cVFC.getVin();
        //chasis = cVFC.getChasis();
        string marcavalidar = "";//cVFC.getMarca();

        VIN.SI_Consulta_VehiculosService consulta = new VIN.SI_Consulta_VehiculosService();
        VIN.DT_Consulta_Vehiculos_RequestVehiculos consultaReq = new VIN.DT_Consulta_Vehiculos_RequestVehiculos();
        VIN.DT_Consulta_Vehiculos_RequestVehiculos[] consultaReq_2 = new VIN.DT_Consulta_Vehiculos_RequestVehiculos[1];
        VIN.DT_Consulta_Vehiculos_Response VinResponse = new VIN.DT_Consulta_Vehiculos_Response();
        VIN.DT_Consulta_Vehiculos_ResponseCaracteristicas_Vehiculos VinResponseCarc = new VIN.DT_Consulta_Vehiculos_ResponseCaracteristicas_Vehiculos();
        VIN.DT_Consulta_Vehiculos_ResponseT_RETURN VinResponseCarcRet =  new VIN.DT_Consulta_Vehiculos_ResponseT_RETURN();


        consulta.Credentials = new System.Net.NetworkCredential("INT_SPI_SBCL", "5k82017PoPcl");
        consulta.PreAuthenticate = true;

        consultaReq.VHVIN = Request.QueryString["vin"];
        consultaReq_2[0] = consultaReq;

        VinResponse = consulta.Consulta_Vehiculos(consultaReq_2);

        if (VinResponse.T_RETURN != null)
	    {
           //hacer nada
	    }
        else
        {
            foreach (var wsdl in VinResponse.Caracteristicas_Vehiculos)
            {
                string NomMarca = wsdl.NOMBRE.ToString();

                if (NomMarca == "SKB_MARCA")
                {
                    marcavalidar = wsdl.VALOR.ToString();

                    if (marcavalidar == "MITSUBISHI FUSO")
                    {
                        marcavalidar = "";
                        marcavalidar = "FUSO";
                    }


                }
            }

        }

        
        			
		if (marcavalidar == "") {
			Response.Write("false");
			return;
		}
			
		if (marcavalidar == marca){
			Response.Write("true");
		}else{
			if (cVFC.VinGrupoporMarca(marca) == cVFC.VinGrupoporMarca(marcavalidar) ){
				Response.Write("true");
			}else  Response.Write("false");
		}
		/*if (chasis != "") {
			Response.Write("true");
		}else	Response.Write("false");*/
    }
}