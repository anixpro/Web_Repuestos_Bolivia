using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using log4net.Config;

public partial class Vistas_SucursalesLista : System.Web.UI.Page
{
    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_SucursalesLista));

    protected void Page_Init(object sender, EventArgs e)
    {
        if (int.Parse(Session["permisos"].ToString()) > 2)
        {
            Response.Redirect("../index.aspx?evento=ev2");
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        msjesError.Visible = false;
        DropDownList1.DataBound += new EventHandler(DropDownList1_DataBound);
        GridView1.SelectedIndexChanged += OnEliminarSucursal;

        if (!IsPostBack)
        {
            if (Request.QueryString["evento"] != null && Request.QueryString["evento"] == "1")
            {
                //DropDownList1.se
                MessageBox.Show("Sucursal editada con éxito");
            }
            else if (Request.QueryString["evento"] != null && Request.QueryString["evento"] == "2")
            {
                MessageBox.Show("Hubo problemas con la sucursal. Contacte al administrador");
            }
        }
    }


    public void OnEliminarSucursal(object o, EventArgs e)
    {
        ControlBD _controlBD = new ControlBD();
        string id = GridView1.SelectedRow.Cells[3].Text.ToString();
        _controlBD.InsertarDatos("delete from sucursal where shipCode = '" + id + "'");
        GridView1.DataBind();

    }


    void DropDownList1_DataBound(object sender, EventArgs e)
    {
        if (Request.QueryString["numFac"] != null)
        {
            try
            {
                DropDownList1.SelectedIndex =
                    DropDownList1.Items.IndexOf(
                        DropDownList1.Items.FindByValue(Request.QueryString["numFac"])
                    );
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    protected void gridSucursales_RowCommand(Object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "editar")
        {
            int index = Convert.ToInt32(e.CommandArgument);
            GridViewRow selectedRow = GridView1.Rows[index];
            TableCell sucursalCell = selectedRow.Cells[0];
            string idSucursal = sucursalCell.Text;
            try
            {
                int a = int.Parse(idSucursal);
                Response.Redirect("SucursalesEditar.aspx?id=" + idSucursal);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Identificador de sucursal no válida. Contacte al administrador. " + ex.Message);
            }
        }
    }
}