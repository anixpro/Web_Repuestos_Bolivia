using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using log4net;
using log4net.Config;

public partial class Vistas_crearEncuesta : System.Web.UI.Page
{
    ControlBD _controlBD;
    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Vistas_crearEncuesta));

    protected void Page_Load(object sender, EventArgs e)
    {
        _controlBD = new ControlBD();

        comboNumAlternativas.SelectedIndexChanged += AgregarNumeroAlternativas;
        btnCrear.Click += BtnCrear_click;
        btnVolver.Click += BtnVolver_Click;

        txtPregunta.TextChanged += ErrorInvi;
        txtAlter1.TextChanged += ErrorInvi;
        txtAlter2.TextChanged += ErrorInvi;
        txtAlter3.TextChanged += ErrorInvi;
        txtAlter4.TextChanged += ErrorInvi;
        txtFechaTermino.TextChanged += ErrorInvi;
    }

    public void AgregarNumeroAlternativas(object sender, EventArgs e)
    {
        try
        {

            if (int.Parse(comboNumAlternativas.SelectedItem.ToString()) == 2)
            {
                lblAlter1.Visible = true;
                txtAlter1.Visible = true;
                lblAlter2.Visible = true;
                txtAlter2.Visible = true;
            }
            if (int.Parse(comboNumAlternativas.SelectedItem.ToString()) == 3)
            {
                lblAlter1.Visible = true;
                txtAlter1.Visible = true;
                lblAlter2.Visible = true;
                txtAlter2.Visible = true;
                lblAlter3.Visible = true;
                txtAlter3.Visible = true;
            }
            if (int.Parse(comboNumAlternativas.SelectedItem.ToString()) == 4)
            {
                lblAlter1.Visible = true;
                txtAlter1.Visible = true;
                lblAlter2.Visible = true;
                txtAlter2.Visible = true;
                lblAlter3.Visible = true;
                txtAlter3.Visible = true;
                lblAlter4.Visible = true;
                txtAlter4.Visible = true;
            }
        }
        catch(Exception ex)
        {
            Response.Write("");
            Debug.Write(ex);
        }
    }

    public void BtnCrear_click(object o, EventArgs e)
    {
        string pre = txtPregunta.Text;
        string alter1 = txtAlter1.Text;
        string alter2 = txtAlter2.Text;
        string alter3 = txtAlter3.Text;
        string alter4 = txtAlter4.Text;
        //string fecFin = txtFechaTermino.Text;
        int votR1 = 0;
        int votR2 = 0;
        int votR3 = 0;
        int votR4 = 0;
        int numOp = 0;

        if (comboNumAlternativas.SelectedItem.ToString() == "")
        {
            numOp = 0;
        }
        else
        {
            numOp = int.Parse(comboNumAlternativas.SelectedItem.ToString());
        }
        if (comboNumAlternativas.SelectedItem.ToString() == "2")
        {
            if (pre == "" || alter1 == "" || alter2 == "" )//|| fecFin == "")
            {
                lblError.Visible = true;
            }
            else {
                _controlBD.PublicarEncuesta(pre, alter1, alter2, alter3, alter4, votR1, votR2, votR3, votR4, numOp);// fecFin);
                Response.Redirect("encuesta.aspx");            
            }
        }

        if (comboNumAlternativas.SelectedItem.ToString() == "3")
        {
            if (pre == "" || alter1 == "" || alter2 == "" || alter3 == "")// || fecFin == "")
            {
                lblError.Visible = true;
            }
            else
            {
                _controlBD.PublicarEncuesta(pre, alter1, alter2, alter3, alter4, votR1, votR2, votR3, votR4, numOp);
                Response.Redirect("encuesta.aspx");
            }
        }

        if (comboNumAlternativas.SelectedItem.ToString() == "4")
        {
            if (pre == "" || alter1 == "" || alter2 == "" || alter3 == "" || alter4 == "")// || fecFin == "")
            {
                lblError.Visible = true;
            }
            else
            {
                _controlBD.PublicarEncuesta(pre, alter1, alter2, alter3, alter4, votR1, votR2, votR3, votR4, numOp);
                Response.Redirect("encuesta.aspx");
            }
        }       

        
        
    }

    public void ErrorInvi(object o, EventArgs e)
    {
        lblError.Visible = false;
    }

    public void BtnVolver_Click(object o, EventArgs e)
    {
        Response.Redirect("encuesta.aspx");
    }
}