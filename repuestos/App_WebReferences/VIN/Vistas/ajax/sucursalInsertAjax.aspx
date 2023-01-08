<%@ Page Language="C#"%>
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
    
       String q= Request.QueryString["q"];
       int op= Convert.ToInt32(Request.QueryString["op"]);

       con = new System.Data.SqlClient.SqlConnection();
       con.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;

       cmd = new System.Data.SqlClient.SqlCommand();


       con.Open();
       cmd.Connection = con;
       cmd.CommandType = System.Data.CommandType.Text;
       switch (op)
       {
           case 1:
               cmd.CommandText = @"select idProvincia,nombreProvincia 
					    from region,provincia where provincia.nombreRegion = region.NombreRegion 
					    and region.nombreRegion='" + q + "';";
               break;
           case 2: cmd.CommandText = @"select idComuna,nombreComuna
					    from provincia,comuna where provincia.nombreProvincia= comuna.NombreProvincia
					    and provincia.nombreProvincia='" + q + "';";
               break;
           default: break;
        }
       dr = cmd.ExecuteReader();
       if (dr.HasRows)
       {
           while (dr.Read())
           {
               Response.Write("<option>"+ dr[1].ToString()+"</option>");
           }
       }
       dr.Close();
       cmd.Connection.Close();
       con.Close();
    
    %>

