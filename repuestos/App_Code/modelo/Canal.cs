using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using log4net.Config;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Descripción breve de Marca
/// </summary>
public class Canal
{
    public String idCanal { get; set; }
    public String descripcion { get; set; }
    public String codigoSap { get; set; }
    public String habilitado { get; set; }

    // Atributos privados
    private SqlConnection con;
    private SqlCommand cmd, cmd1;
    private SqlDataReader dr, dr1;

	public Canal()
	{
        inicio();
	}

    private void inicio()
    {
        con = new SqlConnection();
        cmd = new SqlCommand();
        cmd1 = new SqlCommand();
    }
             
}