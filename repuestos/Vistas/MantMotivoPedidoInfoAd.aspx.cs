using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using log4net;
using log4net.Config;
using System.Configuration;
using System.Data;

public partial class Vistas_MantMotivoPedidoInfoAd : System.Web.UI.Page
{
    ControlBDSolicitud _ControlBD = new ControlBDSolicitud();

    protected void Page_Load(object sender, EventArgs e)
    {
        int _idMotivo = Convert.ToInt32(Request.QueryString["idMotivo"]);
        string _marca = "";
        string _motivo = "";

        DataSet ds = _ControlBD.ObtenerDatosFiltrados(" SELECT MP_Descripcion, mp_Marca FROM Motivo_Pedido WHERE ID_Motivo_pedido = '"+ _idMotivo + "' ");
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            _marca = dr["mp_Marca"].ToString();
            _motivo = dr["MP_Descripcion"].ToString();
        }

        txtMarca.Text = _marca;
        txtMotivo.Text = _motivo;

        if (!IsPostBack)
        {
             grwInfoAdicional.DataSource = _ControlBD.ObtenerDatosFiltrados(" SELECT " +
                                                                            "   IAD.id, IAD.descripcion, IADD.habilitado " +
                                                                            " FROM " +
                                                                            "    dbo.info_ad_datos_detalle IADD, " +
                                                                            "    dbo.info_ad_datos IAD " +
                                                                            " WHERE " +
                                                                            "    IAD.id = IADD.id_info_ad " +
                                                                            " AND " +
                                                                            "    IADD.id_motivo = '"+ _idMotivo + "' " +
                                                                            " AND " +
                                                                            "    IADD.marca = '"+ _marca  +"' ");
            grwInfoAdicional.DataBind();

            //MessageBox.Show(grwInfoAdicional.Rows.Count.ToString());
            if (grwInfoAdicional.Rows.Count == 0)
            {
                _ControlBD.InsertarDatos("  INSERT INTO info_ad_datos_detalle " +
                                         "  (id_info_ad, id_motivo, marca, habilitado)" +
                                         "  SELECT id, '" + _idMotivo + "', '" + _marca + "', 0 FROM dbo.info_ad_datos ");

                grwInfoAdicional.DataSource = _ControlBD.ObtenerDatosFiltrados(" SELECT " +
                                                                               "   IAD.id, IAD.descripcion, IADD.habilitado " +
                                                                               " FROM " +
                                                                               "    dbo.info_ad_datos_detalle IADD, " +
                                                                               "    dbo.info_ad_datos IAD " +
                                                                               " WHERE " +
                                                                               "    IAD.id = IADD.id_info_ad " +
                                                                               " AND " +
                                                                               "    IADD.id_motivo = '" + _idMotivo + "' " +
                                                                               " AND " +
                                                                               "    IADD.marca = '" + _marca + "' ");
                grwInfoAdicional.DataBind();
            }
        }

    }

    protected void grwInfoAdicional_RowCommand(object sender, GridViewCommandEventArgs e)
    {

    }

    protected void chkHabilitado_CheckedChanged(object sender, EventArgs e)
    {

    }

    protected void btnGuardar_Click(object sender, EventArgs e)
    {
        int _idMotivo = Convert.ToInt32(Request.QueryString["idMotivo"]);
        int _nReg = 0;

        for (int i = 0; i < grwInfoAdicional.Rows.Count; i++)
        {
            GridViewRow idRow = grwInfoAdicional.Rows[i];
            Label lblId = idRow.Cells[0].FindControl("lblId") as Label;
            CheckBox chkHabilitado = idRow.Cells[2].FindControl("chkHabilitado") as CheckBox;

            if (chkHabilitado.Checked == true)
            {
                DataSet ds = _ControlBD.ObtenerDatosFiltrados(" SELECT COUNT(*) AS nReg FROM info_ad_datos_detalle WHERE id_info_ad = '" + lblId.Text + "' AND id_motivo = '"+ _idMotivo + "' AND marca = '" + txtMarca.Text+"' ");
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    _nReg = Convert.ToInt32(dr["nReg"]);
                }

                if (_nReg == 0)
                {
                    _ControlBD.InsertarDatos("  INSERT INTO info_ad_datos_detalle " +
                                             "  (id_info_ad, id_motivo, marca, habilitado)" +
                                             "  VALUES" +
                                             "  ('"+ lblId.Text + "', '"+ _idMotivo+"', '"+ txtMarca.Text+"', 1) ");
                }
                else
                {
                    _ControlBD.InsertarDatos("  UPDATE info_ad_datos_detalle " +
                                             "  SET " +
                                             "  habilitado = 1 " +
                                             "  WHERE " +
                                             "  id_info_ad = '" + lblId.Text + "' AND id_motivo = '" + _idMotivo + "' AND marca = '" + txtMarca.Text + "' ");
                }
            }
            else
            {
                _ControlBD.InsertarDatos("  UPDATE info_ad_datos_detalle " +
                                         "  SET " +
                                         "  habilitado = 0 " +
                                         "  WHERE " +
                                         "  id_info_ad = '" + lblId.Text + "' AND id_motivo = '" + _idMotivo + "' AND marca = '" + txtMarca.Text + "' ");
            }
        }
    }
}