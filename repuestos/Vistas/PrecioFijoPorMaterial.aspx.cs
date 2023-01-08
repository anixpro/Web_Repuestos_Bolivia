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


public partial class Vistas_PrecioFijoPorMaterial : System.Web.UI.Page
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
                msjesError.InnerText = "Archvio debe ser extencion .xlsx";
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
            float mesAnno = 0;

            DataSet ds = _controlBD.ObtenerDatosFiltrados(_sql);

            foreach (DataRow campos in ds.Tables[0].Rows)
            {
                PrefijoMarca = campos["PREFIJO"].ToString();
            }

            //Busca periodo mas alto
            string _sql2 = "SELECT DISTINCT TOP(1) MESANIO FROM MAESTRO_FRECUENCIA ORDER BY MESANIO DESC";

            DataSet ds2 = _controlBD.ObtenerDatosFiltrados(_sql2);

            foreach (DataRow campos in ds2.Tables[0].Rows)
            {
                mesAnno = Convert.ToInt32(campos["MESANIO"]);
            }

            int nreg_in = 0;

            //Valida Frecuencia que no venga vacia
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string ValidaFrecuencia = dt.Rows[i]["CLASE_FRECUENCIA_DEMANDA"].ToString();

                if (ValidaFrecuencia == "")
                {
                    msjesError.Visible = true;
                    msjesError.InnerText = "Archvio Con frecuencias vacias, favor rellenar";
                    ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
                    return;
                }
            }

            for (int i=0; i < dt.Rows.Count; i++)
            {
                string  _codMaterial = PrefijoMarca + dt.Rows[i]["CODIGO"].ToString();
                string _GrupoTecnico = null;
                string _Frecuencia = dt.Rows[i]["CLASE_FRECUENCIA_DEMANDA"].ToString();

                if (dt.Rows[i]["GRUPO_TECNICO"].ToString()!= "")
                {
                    _GrupoTecnico = dt.Rows[i]["GRUPO_TECNICO"].ToString();
                }

                _query = ("  INSERT INTO MAESTRO_FRECUENCIA" +
                            "  (CODIGO_SKU, CLASE_FRECUENCIA_DEMANDA, MESANIO, GRUPO_TECNICO, TIPO)" +
                            "  VALUES" +
                            "  ('" + _codMaterial + "','"+_Frecuencia+"' , " + mesAnno + ",'"+_GrupoTecnico+"', 'M') ");
            nreg_in++;

                _ControlBD.InsertarDatos(_query);
            }

            msjesError.Visible = true;
            msjesError.InnerText = "carga finalizada. Se crearon " + nreg_in + " registros.";
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);

        }
        catch (Exception ex)
        {
            msjesError.Visible = true;
            msjesError.InnerText = ex.Message.ToString();
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), "Error en InsertarDatos", "Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace + ". Se muere con la query : [" + _query + "]", "");
        }

    }
}