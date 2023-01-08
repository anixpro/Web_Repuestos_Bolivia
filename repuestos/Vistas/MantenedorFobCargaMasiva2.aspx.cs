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


public partial class Vistas_MantenedorFobCargaMasiva2 : System.Web.UI.Page
{
    ControlBD _controlBD = new ControlBD();
    ControlBDSolicitud _ControlBD = new ControlBDSolicitud();
    SendMail_helper _mail = new SendMail_helper();
    SQL_DevRec _SQL = new SQL_DevRec();
    SapAPI _sapApi = new SapAPI();
    ConsultaRepuesto _consultaRep = new ConsultaRepuesto();

    protected void Page_Load(object sender, EventArgs e)
    {
        //int n_tot = 0;
        //int nreg_in = 0;
        //int nreg_ac = 0;
        string _descripcion = "";
        string _fob = "";
        string _volumen = "";
        string _unidadMedida = "";
        string _moneda = "";
        string _grupoTecnico = "";
        string _query = "";
        SqlConnection con = new SqlConnection();
        SqlCommand cmd = new SqlCommand();

        try
        {
            /*
            _query = "SELECT TOP(5000) * FROM carga_fob WHERE actualizado = '0'";
            string _prefijo = "";
            string _codigo = "";
            int nReg = 0;
            DataSet ds = _controlBD.ObtenerDatosFiltrados(_query);

            foreach (DataRow campos in ds.Tables[0].Rows)
            {
                _prefijo        = Convert.ToString(campos["campo0"]);
                _codigo         = Convert.ToString(campos["campo1"]);
                _descripcion    = Convert.ToString(campos["campo2"]);
                _descripcion    = _descripcion.Trim();
                _fob            = Convert.ToString(campos["campo3"]);
                _volumen        = Convert.ToString(campos["campo4"]);
                _unidadMedida   = Convert.ToString(campos["campo5"]);
                _moneda         = Convert.ToString(campos["campo6"]);
                _grupoTecnico   = Convert.ToString(campos["campo7"]);

                _query = "SELECT COUNT(*) AS n_reg FROM carga_fob_tmp WHERE campo0 = '" + _prefijo + "' AND campo1  = '" + _codigo + "'";

                DataSet ds2 = _controlBD.ObtenerDatosFiltrados(_query);

                foreach (DataRow campos2 in ds2.Tables[0].Rows)
                {
                   nReg = Convert.ToInt32(campos2["n_reg"]);
                }

                if (nReg == 0)
                {
                    _query = ("  INSERT INTO carga_fob_tmp" +
                                "  (campo0, campo1, campo2, campo3, campo4, campo5, campo6, campo7, fecha_carga)" +
                                "  VALUES" +
                                "  ('" + _prefijo + "', '" + _codigo + "', '" + _descripcion + "', '" + _fob + "', '" + _volumen + "', '" + _unidadMedida + "', '" + _moneda + "', '" + _grupoTecnico + "', GETDATE()) ");
                    //nreg_in++;
                }
                else
                {
                    _query = (" UPDATE carga_fob_tmp SET" +
                              " campo2 = '" + _descripcion + "', " +
                              " campo3 = '" + _fob + "', " +
                              " campo4 = '" + _volumen + "', " +
                              " campo5 = '" + _unidadMedida + "', " +
                              " campo6 = '" + _moneda + "', " +
                              " campo7 = '" + _grupoTecnico + "', " +
                              " fecha_carga = GETDATE() " +
                              " WHERE campo0 = '" + _prefijo + "' AND campo1  = '" + _codigo + "'");
                    //nreg_ac++;
                }

                _ControlBD.InsertarDatos(_query);

                _query = (" UPDATE carga_fob SET" +
                            "   actualizado = '1' " +
                            "   WHERE campo0 = '"+ _prefijo + "' " +
                            "   AND campo1 = '"+ _codigo + "' ");

                _ControlBD.InsertarDatos(_query);
                //n_tot++;
            }


            _query = "SELECT COUNT(*) AS n_reg FROM carga_fob WHERE actualizado = '0'";
            DataSet ds3 = _controlBD.ObtenerDatosFiltrados(_query);
            foreach (DataRow campos3 in ds3.Tables[0].Rows) {
                nReg = Convert.ToInt32(campos3["n_reg"]);
            }

            if (nReg == 0)
            {
                _query = "UPDATE carga_fob_tmp set campo3 = REPLACE(campo3, '.', ',')";
                _ControlBD.InsertarDatos(_query);

                _query = "UPDATE carga_fob_tmp set campo4 = REPLACE(campo4, '.', ',')";
                _ControlBD.InsertarDatos(_query);

            }
            else
            {
                Response.Redirect("MantenedorFobCargaMasiva2.aspx", true);
            }

            _query = "SELECT COUNT(*) AS n_reg FROM carga_fob WHERE actualizado = '1'";
            DataSet ds4 = _controlBD.ObtenerDatosFiltrados(_query);
            foreach (DataRow campos3 in ds4.Tables[0].Rows)
            {
                nReg = Convert.ToInt32(campos3["n_reg"]);
            }

            msjesError.Visible = true;
            //msjesError.InnerText = "Carga finalizada. Se ingresaron " + nreg_in + " . Se actualizaron " + nreg_ac + " registros.";
            msjesError.InnerText = "Carga finalizada. Se procesaron " + nReg + " registros.";
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            */

            con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
            con.Open();
            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "webr_fob_cm_actualizacion";
            cmd.CommandTimeout = 10;
            //cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
            //cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
            cmd.ExecuteReader();

            con.Close();


            con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
            con.Open();
            cmd.Connection = con;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "webr_fob_cm_inserta";
            cmd.CommandTimeout = 10;
            //cmd.Parameters.Add("@o_nro_error", SqlDbType.Int, 2).Direction = ParameterDirection.Output;
            //cmd.Parameters.Add("@o_msg_error", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
            cmd.ExecuteReader();

            con.Close();

            msjesError.InnerText = "Carga finalizada.";
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);

            Server.ScriptTimeout = 90;

        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), "Error en InsertarDatos", "Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace + ". Se muere con la query : [" + _query + "]", "");
        }

    }
}