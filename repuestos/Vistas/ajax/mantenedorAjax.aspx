<%@ Page Language="C#" %>
<%@ Import Namespace="System" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="System.Web" %>
<%@ Import Namespace="System.Web.UI" %>
<%@ Import Namespace="System.Web.UI.WebControls" %>
<%@ Import Namespace="System.Data.Sql" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Configuration" %>
<%@ Import Namespace="System.Data" %>

 <%

    SqlConnection con;
    SqlDataReader dr;
    SqlCommand cmd;

    
        String q = Request.QueryString["q"];
        int op = Convert.ToInt32(Request.QueryString["op"]);

        con = new System.Data.SqlClient.SqlConnection();
        con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;

        cmd = new System.Data.SqlClient.SqlCommand();


        con.Open();
        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        switch (op)
        {
            case 1:
                cmd.CommandText = @"select direccionSucursal,shipCode
                                from sucursal
                                where nombreConcesionario='" + q + "';";
                break;
            default: break;
        }
        dr = cmd.ExecuteReader();
        if (dr.HasRows)
        {
            Response.Write("<option SELECTED value=\"0\">Seleccionar</option>");
            while (dr.Read())
            {
                Response.Write("<option value=\"" + dr[1].ToString() + "\">" + dr[0].ToString() + "</option>");
            }
        }
        dr.Close();
        cmd.Connection.Close();
        con.Close();
    %>