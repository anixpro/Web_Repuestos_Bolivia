using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;


public partial class Vistas_MantenedorFobParidad : System.Web.UI.Page
{
    ControlBDSolicitud _ControlBD = new ControlBDSolicitud();
    ControlBD _controlBD = new ControlBD();
    SendMail_helper _mail = new SendMail_helper();
    SQL_DevRec _SQL = new SQL_DevRec();

    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Form.Attributes.Add("enctype", "multipart/form-data");
        mjsError.Visible = false;

        cargarGrilla();

        if (!Page.IsPostBack)
        {
            LlenarComboMonedas();

        }
    }

    private void LlenarComboMonedas()
    {
        ddlMoneda.Items.Clear();
        ddlMoneda.Items.Add(new ListItem("Seleccionar", "-1"));

        foreach (string moneda in _ControlBD.obtieneMonedas())
        {
            ddlMoneda.Items.Add(moneda);
        }
    }

    protected void btnGrabar_Click(object sender, EventArgs e)
    {
        mjsError.Visible = false;
        /*
        if (txtfecha.Text == "")
        {
            mjsError.InnerText = "Debe ingresar un fecha.";
            mjsError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }
        */
        if (ddlMoneda.SelectedValue == "-1")
        {
            mjsError.InnerText = "Debe seleccionar una moneda.";
            mjsError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }

        if (txtSigla.Text == "")
        {
            mjsError.InnerText = "Debe ingresar una sigla para la moneda seleccionada.";
            mjsError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }

        if (txtValor.Text == "")
        {
            mjsError.InnerText = "Debe ingresar un valor para la moneda seleccionada.";
            mjsError.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "RewindScroll", "window.scrollTo(0,0)", true);
            return;
        }

        //Valida si la moneda ya se ingreso
        string _nreg = "";

        DataSet dsExisteMoneda = new DataSet();
        dsExisteMoneda = _ControlBD.ObtenerDatosFiltrados(" SELECT COUNT(*) AS n_reg FROM monedas WHERE descripcion = '"+ ddlMoneda.SelectedValue + "' ");
        foreach (DataRow datos in dsExisteMoneda.Tables[0].Rows)
        {
            _nreg = datos["n_reg"].ToString();
        }
        //Fin valida si la moneda ya se ingreso

        if (_nreg == "0")
        {
            string _query = ("  INSERT INTO monedas" +
                            "  (descripcion, valor, sigle, habilitado)" +
                            "  VALUES" +
                            "  ('" + ddlMoneda.SelectedValue + "', '" + txtValor.Text + "', '" + txtSigla.Text + "', 1) ");

            try
            {
                txtValor.Text = txtValor.Text.Replace(",", ".");

                _ControlBD.InsertarDatos(_query);

                cargarGrilla();

            }
            catch (Exception ex)
            {
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), "Error en InsertarDatos", "Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace + ". Se muere con la query : [" + _query + "]", "");
            }
        }
        else
        {
            txtValor.Text = txtValor.Text.Replace(",", ".");

            string _query = (" UPDATE monedas SET" +
                            "  valor = " + txtValor.Text + ",  " +
                            "  sigla = '" + txtSigla.Text + "'  " +
                            "  WHERE " +
                            "  descripcion = '" + ddlMoneda.SelectedValue + "' ");

            try
            {

                _ControlBD.InsertarDatos(_query);

                //LOG CAMBIO DE VALORES

                string hora = DateTime.Now.ToString("hh:mm:ss");
                lblValor.Text = lblValor.Text.Replace(",", ".");

                _query = "";
                _query = (" INSERT INTO monedas_log " +
                           "  (usuario, fecha, hora, moneda, valor_inicial, sigla_inicial, valor_cambio, sigla_cambio) " +
                           "  VALUES " +
                           "  ('" + Session["rut"].ToString() + "', GETDATE(), '"+ hora + "', '" + ddlMoneda.SelectedValue +"', " + lblValor.Text + ", '" + lblSigla.Text + "', " + txtValor.Text + ", '" + txtSigla.Text + "') ");

                _ControlBD.InsertarDatos(_query);

                //FIN LOG CAMBIO DE VALORES

                cargarGrilla();

            }
            catch (Exception ex)
            {
                _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), "Error en InsertarDatos", "Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace + ". Se muere con la query : [" + _query + "]", "");
            }
        }

    }

    protected void btnVer_Click(object sender, EventArgs e)
    {
        mjsError.Visible = false;
        decimal _valor = 0;
        string _sigla = "";

        try
        {
            DataSet dsFactorGrupoTecnico2 = _controlBD.ObtenerDatosFiltrados(" SELECT * FROM monedas WHERE descripcion = '" + ddlMoneda.SelectedValue + "' ");
            foreach (DataRow dr2 in dsFactorGrupoTecnico2.Tables[0].Rows)
            {
                _valor = Convert.ToDecimal(dr2["valor"]);
                _sigla = dr2["sigla"].ToString();
            }

            txtValor.Text = _valor.ToString();
            txtSigla.Text = _sigla;

            lblValor.Text = _valor.ToString();
            lblSigla.Text = _sigla;

            cargarGrilla();
        }
        catch (Exception ex)
        {
            _mail.EnviarCorreo(ConfigurationManager.AppSettings["correo_error"].ToString(), "Error en InsertarDatos", "Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace +"");
        }

    }

    private void cargarGrilla()
    {
        int coderror = 0;
        string msgerror = "";

        dgvMonedas.DataSource = _SQL.obtieneValoresMonedas(out coderror, out msgerror);
        dgvMonedas.DataBind();
    }


    protected void btnVerLog_Click(object sender, EventArgs e)
    {
        int coderror = 0;
        string msgerror = "";

        grvLog.DataSource = _SQL.obtieneLogParidad(out coderror, out msgerror);
        grvLog.DataBind();
    }
}