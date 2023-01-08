/* Dieter Varas
*  dieter@intellicore.cl
*  Modulo que permite fijar una cantidad minima de adquisicion para un repuesto que sera validad en el modulo de cotizar.
*
*/
/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using log4net;
using log4net.Config;*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Diagnostics;
using System.IO;
using log4net;
using log4net.Config;

public partial class EditorAccesorioCantidad : System.Web.UI.Page
{
    private String _nombreMarca;
    private int _iMarca;
	SendMail_helper _mail = new SendMail_helper();
	
    // Clase que interactua con elementos de SAP
    SapAPI _sapApi = new SapAPI();

    // Objeto que se recibe parametros que se envian a SAP
    ConsultaRepuesto _consultaRep = new ConsultaRepuesto();
	
	 RealizarPedido _pedido = new RealizarPedido();

    private ControlRepuestos controlRepuestos;
    private ControlAuto controlAuto;

    protected void Page_Init(object sender, EventArgs e)
    {

        if (int.Parse(Session["permisos"].ToString()) != 1)
        {
            Response.Redirect("../index.aspx?evento=ev2");
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
		
        msjesError.Visible = false;
        controlRepuestos = new ControlRepuestos();
        controlAuto = new ControlAuto();
        String codigo = "";
        String marca = "";
		String accion = "";
		String indice = "Todas";

       /* btnBuscar.OnClientClick = "javascript:return validaBlancos();";*/
        try
        {
            if (!Page.IsPostBack){
            codigo = Request.QueryString["codigo"];
            marca = Request.QueryString["marca"];
			accion = Request.QueryString["accion"];
			indice = Request.QueryString["indice"];
			}
        }
        catch (ArgumentNullException)
        {
            codigo = "";
            marca = "";
        }
        catch (FormatException)
        {
            codigo = "";
            marca = "";
        }

   
        try
        {
            _nombreMarca = ddlMarca.SelectedItem.ToString();
            _iMarca = ddlMarca.SelectedIndex;
        }
        catch (NullReferenceException)
        {
            _nombreMarca = "";
            _iMarca = 0;
        }

        ddlMarca.Items.Clear();
        ddlMarca.Items.Add("Todas");

        foreach (String Marca in controlAuto.obtenerMarca())
        {
            ddlMarca.Items.Add(Marca);
        }

        ddlMarca.SelectedIndex = _iMarca;

        sqldRepuestos.SelectCommand = "select * from repuesto_cantidadmin";
        gridRepuestos.DataBind();

		if (marca != "" && codigo != "" && accion =="1") {
		
			if (controlRepuestos.detalleRepuestoPorMarcaCodigo(marca, codigo) == "none"){
					_consultaRep.GrupoMaterial = _sapApi.GetGrupoMaterialesByMarca(marca);
					 string prefijoMarca = _sapApi.GetPrefijoMarcaByGrupoMateriales(_consultaRep.GrupoMaterial,marca); //Se agrega marca
					_consultaRep.CodRepuesto = prefijoMarca.Trim().ToUpper() + codigo.ToUpper();
					string shipCode = "IBC05";
					_consultaRep.DestinaMercacia = shipCode;
					_consultaRep.DocVentas = _sapApi.GetVkorgByGrupoMaterial(_consultaRep.GrupoMaterial, marca);
					_consultaRep.CanalDistribucion = _sapApi.CanalDeDistribucionPedido;
					_consultaRep.TextoRep = "";
					_consultaRep.CantidadRep = 1;
					
					
					if (!_pedido.BuscarRepuestoCant(_consultaRep, Session["idSession"].ToString(), 1, "", marca) )
					{
						msjesError.InnerText = "El Repuesto no pudo ser encontrado. "; 
						msjesError.Visible = true;			
						return;
						
					} else {	
							sqldRepuestos.SelectCommand = controlRepuestos.detalleRepuestoPorMarcaCodigo(marca, codigo);
							
					}		
			}else  sqldRepuestos.SelectCommand = controlRepuestos.detalleRepuestoPorMarcaCodigo(marca, codigo);
		}else if (marca!="" && accion=="2")   {
				sqldRepuestos.SelectCommand = "select * from repuesto_cantidadmin where marca = '"+marca+"'";
				ddlMarca.SelectedIndex = int.Parse(indice);
		} 
		
    }

    /*
    * 
    * 
    */
    protected void btnBuscar_Click(object sender, EventArgs e)
    {
		if (txtCodigo.Text == "") {
				msjesError.InnerText = "Debe ingresar un código de repuesto";
				msjesError.Visible = true;
				return;
			}
		if (ddlMarca.Text == "Seleccione") {
				msjesError.InnerText = "Debe seleccionar una marca";
				msjesError.Visible = true;
				return;
			}
        Response.Redirect("EditorAccesorioCantidad.aspx?codigo=" + txtCodigo.Text + "&marca=" + ddlMarca.Text+"&accion=1");
    }


    /*
     * 
     * 
     */
    protected void ddlMarca_SelectedIndexChanged(object sender, EventArgs e)
    {
		Response.Redirect("EditorAccesorioCantidad.aspx?codigo=" + txtCodigo.Text + "&marca=" + ddlMarca.Text+"&accion=2&indice="+ddlMarca.SelectedIndex);
    }
   
  
}