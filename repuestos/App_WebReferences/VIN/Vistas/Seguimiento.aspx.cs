using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Vistas_Seguimiento : System.Web.UI.Page
{
    protected void Page_Init(object sender, EventArgs e)
    {
        // Comprobación de permisos
        if (!(int.Parse(Session["permisos"].ToString()) == 1 ||
            int.Parse(Session["permisos"].ToString()) == 5
            ))
        {
            Response.Redirect("../index.aspx?evento=ev2");
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
		 String pedido ="";
		 String accion ="";
		 String tipoPedido = "";
		 
		 SqlDataSourcePedido.SelectCommand="SELECT * FROM [PEDIDO] order by id_pedido desc";
		 SqlDataSourceReservas.SelectCommand="SELECT * FROM [BACKORDER] order by num_backOrder desc";
		 SqlDataSourceVFC.SelectCommand="SELECT * FROM [VFC] order by fecha_creacion desc";
		 SqlDataSourceDescartados.SelectCommand="SELECT * FROM [DESCARTADOS] order by fechaDescarte desc";
		 
		if (!Page.IsPostBack){
            pedido = Request.QueryString["pedido"];
			accion = Request.QueryString["accion"];
			tipoPedido = Request.QueryString["tipoPedido"];
			
		}
			
			
		if ( pedido != "" && accion =="1" && tipoPedido!= "") {
			switch (tipoPedido)
			{
			  case "pedido":
                    SqlDataSourcePedido.SelectCommand = "SELECT * FROM [PEDIDO] WHERE  E_VBELN = '" + pedido.Trim() + "' order by id_pedido desc";
			  break;
			  case "reserva":
				SqlDataSourceReservas.SelectCommand="SELECT * FROM [BACKORDER] WHERE  num_backorder = '" + pedido.Trim() + "' order by num_backOrder desc";
			  break;
			  case "vfc":
				 SqlDataSourceVFC.SelectCommand="SELECT * FROM [VFC] WHERE  num_vfc = '" + pedido.Trim() + "' order by fecha_creacion desc";
			  break;
			  case "descartado":
				 SqlDataSourceDescartados.SelectCommand="SELECT * FROM [DESCARTADOS] WHERE id_pedido = '" + pedido.Trim() + "' order by fechaDescarte desc";
			  break;
			}
			
			
		
		}
    }
	
	 protected void btnBuscar_Click(object sender, EventArgs e)
    {
		if (txtPedido.Text == "") {
				msjesError.InnerText = "Debe ingresar pedido a buscar";
				msjesError.Visible = true;
				return;
			}
			
		if (tipoPedido.Value == "") {
				msjesError.InnerText = "La Busqueda no se puede realizar en indicadores  o Materiales";
				msjesError.Visible = true;
				return;
			}
		
        Response.Redirect("Seguimiento.aspx?pedido=" + txtPedido.Text + "&tipopedido=" + tipoPedido.Value + "&accion=1");
    }
}