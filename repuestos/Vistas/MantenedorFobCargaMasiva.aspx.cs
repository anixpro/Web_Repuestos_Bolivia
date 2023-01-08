using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;


public partial class Vistas_MantenedorFobCargaMasiva : System.Web.UI.Page
{
    ControlBD _controlBD = new ControlBD();
    ControlBDSolicitud _ControlBD = new ControlBDSolicitud();
    SendMail_helper _mail = new SendMail_helper();
    SQL_DevRec _SQL = new SQL_DevRec();
    SapAPI _sapApi = new SapAPI();
    ConsultaRepuesto _consultaRep = new ConsultaRepuesto();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            LlenarComboMarcas();
        }
    }

    private void LlenarComboMarcas()
    {
        ddlMarca.Items.Clear();
        ddlMarca.Items.Add(new ListItem("Seleccionar", "-1"));

        //Llenar el combo box con las marcas
        foreach (string marca in _controlBD.CrearMarcas(Session["rut"].ToString()))
        {
            ddlMarca.Items.Add(marca);
        }
    }
    
    protected void btnCargar_Click(object sender, EventArgs e)
    {
        Server.ScriptTimeout = 420;

        string _query = "";
        try
        {

            if (ddlMarca.SelectedValue == "-1")
            {
                msjesError.Visible = true;
                msjesError.InnerText = "Debe seleccionar una marca";
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                return;
            }

            if (!fupArchivo.HasFile)
            {
                msjesError.Visible = true;
                msjesError.InnerText = "Debe seleccionar un archivo";
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                return;
            }

            //Vacio tabla temporal
            _query = ("  TRUNCATE TABLE carga_fob ");
            _ControlBD.InsertarDatos(_query);

            String pathDocu = Server.MapPath("~/Documentacion/");
            fupArchivo.SaveAs(pathDocu + fupArchivo.FileName);

            string connectString = "";
            if (System.IO.Path.GetExtension(fupArchivo.FileName).ToString().ToLower() == ".xlsx")
            { 
                    connectString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + pathDocu + fupArchivo.FileName + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1;\"";
            }
            else
            {
                msjesError.Visible = true;
                msjesError.InnerText = "El archivo debe ser extencion .xlsx";
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                return;
            }


            OleDbConnection conn = new OleDbConnection(connectString);
            OleDbDataAdapter da = new OleDbDataAdapter("Select * From [Hoja1$]", conn);
            DataTable dt = new DataTable();
            da.Fill(dt);

            //Busca prefijo de la marca
            string _sql = "SELECT PREFIJO FROM GRUPO_MATERIALES WHERE MARCA = '" + ddlMarca.SelectedItem + "'";
            string PrefijoMarca = "";
            string unidadMedida = "";
            string moneda = "";
            string marca = "";
            int nReg = 0;

            DataSet ds = _controlBD.ObtenerDatosFiltrados(_sql);

            foreach (DataRow campos in ds.Tables[0].Rows)
            {
                PrefijoMarca = campos["PREFIJO"].ToString();
            }

            unidadMedida= dt.Rows[0][1].ToString();
            moneda = dt.Rows[1][1].ToString();
            marca = dt.Rows[2][1].ToString();

            if (marca != ddlMarca.SelectedValue)
            {
                msjesError.Visible = true;
                msjesError.InnerText = "La marca que seleccionada no coincide con la indicada en la planilla. Carga detenida.";
                ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                return;
            }

            int nreg_in = 0;
            int nreg_ac = 0;
            string descripcion = "";

            for (int i=3; i < dt.Rows.Count; i++)
            {
                descripcion = dt.Rows[i]["DENOMINACION"].ToString();
                descripcion = descripcion.Trim();

                
                _query = ("  INSERT INTO carga_fob" +
                            "  (campo0, campo1, campo2, campo3, campo4, campo5, campo6, campo7, fecha_carga, actualizado)" +
                            "  VALUES" +
                            "  ('" + PrefijoMarca + "', '" + dt.Rows[i]["PN"].ToString() + "', '" + dt.Rows[i]["DENOMINACION"].ToString() + "', '" + dt.Rows[i]["FOB"].ToString() + "', '" + dt.Rows[i]["VOLUMEN"].ToString() + "', '" + unidadMedida + "', '" + moneda + "', '" + dt.Rows[i]["GRUPO_TECNICO"].ToString() + "', GETDATE(), '0') ");
                nreg_in++;

                _ControlBD.InsertarDatos(_query);
            }

            _query = "UPDATE carga_fob_tmp set campo3 = REPLACE(campo3, '.', ',')";
            _ControlBD.InsertarDatos(_query);

            _query = "UPDATE carga_fob_tmp set campo4 = REPLACE(campo4, '.', ',')";
            _ControlBD.InsertarDatos(_query);

            msjesError.Visible = true;
            msjesError.InnerText = "Pre carga finalizada. Se contabilizaron " + nreg_in + " registros.";
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);

            Response.Redirect("MantenedorFobCargaMasiva2.aspx", true);
        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), "Error en InsertarDatos", "Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace + ". Se muere con la query : [" + _query + "]", "");
        }

    }
}