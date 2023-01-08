using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;
using System.Net;
using System.Collections;
using log4net;
using log4net.Config;

/// <summary>
/// Summary description for Pedido
/// </summary>
public class Pedido
{
    private static String connectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;

    // ID_PEDIDO 
    public String Id { get; set; }

    // idSession 
    public String IdSession { get; set; }

    // E_VBELN
    public String NumeroCotizacion { get; set; }

    // I_AUART (Clase Documento de Ventas (garantia o normal))
    public String ClaseDocumentosVentas { get; set; }

    // I_BNDDT Fecha validez
    public String FechaValidez { get; set; }

    // I_KUNNR Nº de cliente 
    public String CodigoClienteSAP { get; set; }

    // I_KUNNR2 dest de mercaincia
    public String DestinatarioMercancia { get; set; }

    // Canal de repuestos (siempre es el mismo BR) I_SPART
    public String CanalDeRepuestos { get; set; }

    // I_TEXTO, Descripción de cotización
    public String DescrpcionCotizacion { get; set; }

    // I_VKORG, Organizacion de ventas
    public String OrganizacionDeVentas { get; set; }

    // Canal de distribución, I_VTWEG;
    public String CanalDeDistribucion { get; set; }

    // SOLICITADO_POR
    public String SolicitadoPor { get; set; }

    // FECHA_SOLICITUD
    public String FechaSolicitado { get; set; }

    // estado = "";
    public String Estado { get; set; }

    // FECHA_EXPIRACION = "";
    public String FechaExpiracion { get; set; }

    // FECHA_EXPIRACION_SYS
    public String FechaExpiracionSistema { get; set; }

    // SUCURSAL
    public String Sucursal { get; set; }

    // TOTAL_NETO
    public String TotalNeto { get; set; }

    // COMENTARIOS
    public String Comentarios { get; set; }

    // E_VBELN_PEDIDO
    public String PedidoNumero { get; set; }

    // I_AUART_PEDIDO. Clase Documento de Ventas (garantia o normal)
    public String PedidoClaseDocumentoVentas { get; set; }

    // I_LPRIO_PEDIDO. Prioridad
    public String PedidoPrioridad { get; set; }

    private SqlConnection con;
    private SqlCommand cmd, cmd1,cmd2;
    private SqlDataReader dr, dr1,dr2;
    private DataTable dt;

    // Logger que dejará información de seguimiento en /doc/logWebReptos2.log
    private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(Pedido));

	public Pedido()
	{
        con = new SqlConnection();
        cmd = new SqlCommand();
        cmd1 = new SqlCommand();
        cmd2= new SqlCommand();
        dt = new DataTable();
	}

    public String detallePedidoPorCodigo(String codigo,int rutEnSesion,String marca,String desde,String hasta)
    {
        return @"SELECT MATERIALES_PEDIDO.codigo,MATERIALES_PEDIDO.descripcion,MATERIALES_PEDIDO.cantidad, MATERIALES_PEDIDO.valor AS precio, pedido.fecha_solicitud as fecha
					from MATERIALES_PEDIDO,PEDIDO,persona,sucursal,concesionario
					WHERE MATERIALES_PEDIDO.codigo='" +codigo+"' "
                    +"AND FECHA_SOLICITUD BETWEEN '"+desde+"' and '"+hasta+"'" 
                    +"AND MATERIALES_PEDIDO.marca='" + marca + "' "
					+"AND MATERIALES_PEDIDO.id_pedido=PEDIDO.ID_PEDIDO "
                    +"AND PEDIDO.E_VBELN_PEDIDO !=''"
					+"AND PEDIDO.SOLICITADO_POR=persona.rut "
					+"AND persona.shipCode=sucursal.shipCode "
					+"AND sucursal.nombreConcesionario=concesionario.nombreConcesionario "
					+"AND concesionario.nombreConcesionario=(SELECT concesionario.nombreConcesionario "
															+"FROM concesionario, persona,sucursal "
															+"WHERE persona.shipCode=sucursal.shipCode "
															+"AND sucursal.nombreConcesionario = concesionario.nombreConcesionario "
															+"AND persona.rut="+rutEnSesion+") "
                                                            +"order by fecha desc";
    }
    public String detallePedidoPorCodigo(String codigo, String concesionario, String marca, String desde, String hasta)
    {
        return @"SELECT MATERIALES_PEDIDO.codigo,MATERIALES_PEDIDO.descripcion,MATERIALES_PEDIDO.cantidad, MATERIALES_PEDIDO.valor AS precio, pedido.fecha_solicitud as fecha
					from MATERIALES_PEDIDO,PEDIDO,persona,sucursal,concesionario
					WHERE MATERIALES_PEDIDO.codigo='" + codigo + "' "
                    + "AND FECHA_SOLICITUD BETWEEN '" + desde + "' and '" + hasta + "'"
                    + "AND MATERIALES_PEDIDO.marca='" + marca + "' "
                    + "AND MATERIALES_PEDIDO.id_pedido=PEDIDO.ID_PEDIDO "
                    + "AND PEDIDO.E_VBELN_PEDIDO !=''"
                    + "AND PEDIDO.SOLICITADO_POR=persona.rut "
                    + "AND persona.shipCode=sucursal.shipCode "
                    + "AND sucursal.nombreConcesionario=concesionario.nombreConcesionario "
                    + "AND concesionario.nombreConcesionario='" + concesionario + "'";
    }
    //resumenPorCodigo
    public DataTable detallePedidoPorCodigoResumen(String codigo, int rutEnSesion, String marca, String desde, String hasta)
    {
        Hashtable cantidad = new Hashtable();
        con.ConnectionString = connectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = @"SELECT MATERIALES_PEDIDO.codigo,MATERIALES_PEDIDO.descripcion,MATERIALES_PEDIDO.cantidad, MATERIALES_PEDIDO.valor AS precio, pedido.fecha_solicitud as fecha
					        from MATERIALES_PEDIDO,PEDIDO,persona,sucursal,concesionario
					        WHERE MATERIALES_PEDIDO.codigo='" + codigo + "' "
                            + "AND FECHA_SOLICITUD BETWEEN '" + desde + "' and '" + hasta + "'"
                            + "AND MATERIALES_PEDIDO.marca='" + marca + "' "
                            + "AND MATERIALES_PEDIDO.id_pedido=PEDIDO.ID_PEDIDO "
                            + "AND PEDIDO.E_VBELN_PEDIDO !=''"
                            + "AND PEDIDO.SOLICITADO_POR=persona.rut "
                            + "AND persona.shipCode=sucursal.shipCode "
                            + "AND sucursal.nombreConcesionario=concesionario.nombreConcesionario "
                            + "AND concesionario.nombreConcesionario=(SELECT concesionario.nombreConcesionario "
                                                                    + "FROM concesionario, persona,sucursal "
                                                                    + "WHERE persona.shipCode=sucursal.shipCode "
                                                                    + "AND sucursal.nombreConcesionario = concesionario.nombreConcesionario "
                                                                    + "AND persona.rut=" + rutEnSesion + ") "
                                                                    + "order by fecha desc";

        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        String codigoRepuesto = "";
        String descripcion = "";
        int cantidadTotal = 0;
        int precio=0;
        int precioTotal = 0;
        while (dr.Read())
        {            
            codigoRepuesto = dr[0].ToString();
            descripcion = dr[1].ToString();
            cantidadTotal+=int.Parse(dr[2].ToString());
            precio = int.Parse(dr[3].ToString());
        }
        precioTotal = precio * cantidadTotal;
        cantidad.Add("codigo",codigoRepuesto);
        cantidad.Add("descripcion",descripcion);
        cantidad.Add("cantidad",cantidadTotal);
        cantidad.Add("precio", precio);
        cantidad.Add("precioTotal",precioTotal);
        dr.Close();

        dt.Columns.Add("codigo"); dt.Columns.Add("descripcion"); dt.Columns.Add("cantidad"); dt.Columns.Add("precio"); dt.Columns.Add("precioTotal");
        dt.Rows.Add(cantidad["codigo"], cantidad["descripcion"], cantidad["cantidad"], cantidad["precio"], cantidad["precioTotal"]);
        return dt;
    }
    //resumenPorCodigo Total
    public DataTable detallePedidoPorCodigoResumenTotal(String codigo, int rutEnSesion)
    {
        Hashtable cantidad = new Hashtable();
        con.ConnectionString = connectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = @"SELECT MATERIALES_PEDIDO.codigo,MATERIALES_PEDIDO.descripcion,MATERIALES_PEDIDO.cantidad, MATERIALES_PEDIDO.valor AS precio, pedido.fecha_solicitud as fecha
					        from MATERIALES_PEDIDO,PEDIDO,persona,sucursal,concesionario
					        WHERE MATERIALES_PEDIDO.codigo='" + codigo + "' "
                            + "AND MATERIALES_PEDIDO.id_pedido=PEDIDO.ID_PEDIDO "
                            + "AND PEDIDO.E_VBELN_PEDIDO !=''"
                            + "AND PEDIDO.SOLICITADO_POR=persona.rut "
                            + "AND persona.shipCode=sucursal.shipCode "
                            + "AND sucursal.nombreConcesionario=concesionario.nombreConcesionario "
                            + "AND concesionario.nombreConcesionario=(SELECT concesionario.nombreConcesionario "
                                                                    + "FROM concesionario, persona,sucursal "
                                                                    + "WHERE persona.shipCode=sucursal.shipCode "
                                                                    + "AND sucursal.nombreConcesionario = concesionario.nombreConcesionario "
                                                                    + "AND persona.rut=" + rutEnSesion + ") "
                                                                    + "order by fecha desc";

        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        String codigoRepuesto = "";
        String descripcion = "";
        int cantidadTotal = 0;
        int precio = 0;
        int precioTotal = 0;
        while (dr.Read())
        {
            codigoRepuesto = dr[0].ToString();
            descripcion = dr[1].ToString();
            cantidadTotal += int.Parse(dr[2].ToString());
            precio = int.Parse(dr[3].ToString());
        }
        precioTotal = precio * cantidadTotal;
        cantidad.Add("codigo", codigoRepuesto);
        cantidad.Add("descripcion", descripcion);
        cantidad.Add("cantidad", cantidadTotal);
        cantidad.Add("precio", precio);
        cantidad.Add("precioTotal", precioTotal);
        dr.Close();

        dt.Columns.Add("codigo"); dt.Columns.Add("descripcion"); dt.Columns.Add("cantidad"); dt.Columns.Add("precio"); dt.Columns.Add("precioTotal");
        dt.Rows.Add(cantidad["codigo"], cantidad["descripcion"], cantidad["cantidad"], cantidad["precio"], cantidad["precioTotal"]);
        return dt;
    }
    //sin fecha ni marca
    public String detallePedidoPorCodigo(String codigo, int rutEnSesion)
    {
        return @"SELECT MATERIALES_PEDIDO.codigo,MATERIALES_PEDIDO.descripcion,MATERIALES_PEDIDO.cantidad, MATERIALES_PEDIDO.valor AS precio, pedido.fecha_solicitud as fecha
					from MATERIALES_PEDIDO,PEDIDO,persona,sucursal,concesionario
					WHERE MATERIALES_PEDIDO.codigo='" + codigo + "' "
                    + "AND MATERIALES_PEDIDO.id_pedido=PEDIDO.ID_PEDIDO "
                    + "AND PEDIDO.E_VBELN_PEDIDO !=''"
                    + "AND PEDIDO.SOLICITADO_POR=persona.rut "
                    + "AND persona.shipCode=sucursal.shipCode "
                    + "AND sucursal.nombreConcesionario=concesionario.nombreConcesionario "
                    + "AND concesionario.nombreConcesionario=(SELECT concesionario.nombreConcesionario "
                                                            + "FROM concesionario, persona,sucursal "
                                                            + "WHERE persona.shipCode=sucursal.shipCode "
                                                            + "AND sucursal.nombreConcesionario = concesionario.nombreConcesionario "
                                                            + "AND persona.rut=" + rutEnSesion + ") "
                                                            + "order by fecha desc";
    }
    public String detallePedidoPorRut(int rut, int rutEnSesion, String marca, String desde, String hasta)
    {
        return @"SELECT persona.rut,persona.nombre,MATERIALES_PEDIDO.codigo,MATERIALES_PEDIDO.cantidad, MATERIALES_PEDIDO.valor AS precio, pedido.fecha_solicitud as fecha
					from MATERIALES_PEDIDO,PEDIDO,persona,sucursal,concesionario
					WHERE persona.rut=" + rut + " "
                    + "AND FECHA_SOLICITUD BETWEEN '" + desde + "' and '" + hasta + "'" 
                    + "AND MATERIALES_PEDIDO.marca='" + marca + "' "
                    + "AND MATERIALES_PEDIDO.id_pedido=PEDIDO.ID_PEDIDO "
                    + "AND PEDIDO.E_VBELN_PEDIDO !=''"
                    + "AND PEDIDO.SOLICITADO_POR=persona.rut "
                    + "AND persona.shipCode=sucursal.shipCode "
                    + "AND sucursal.nombreConcesionario=concesionario.nombreConcesionario "
                    + "AND concesionario.nombreConcesionario=(SELECT concesionario.nombreConcesionario "
                                                            + "FROM concesionario, persona,sucursal "
                                                            + "WHERE persona.shipCode=sucursal.shipCode "
                                                            + "AND sucursal.nombreConcesionario = concesionario.nombreConcesionario "
                                                            + "AND persona.rut=" + rutEnSesion + ") "
                                                            + "order by fecha desc";
    }
    //resumen por rut
    public DataTable detallePedidoPorRutResumen(int rut, int rutEnSesion, String marca, String desde, String hasta)
    {
        Hashtable cantidad = new Hashtable();
        con.ConnectionString = connectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = @"SELECT persona.rut,persona.nombre,MATERIALES_PEDIDO.codigo,MATERIALES_PEDIDO.cantidad, MATERIALES_PEDIDO.valor AS precio, pedido.fecha_solicitud as fecha
					        from MATERIALES_PEDIDO,PEDIDO,persona,sucursal,concesionario
					        WHERE persona.rut=" + rut + " "
                            + "AND FECHA_SOLICITUD BETWEEN '" + desde + "' and '" + hasta + "'"
                            + "AND MATERIALES_PEDIDO.marca='" + marca + "' "
                            + "AND MATERIALES_PEDIDO.id_pedido=PEDIDO.ID_PEDIDO "
                            + "AND PEDIDO.E_VBELN_PEDIDO !=''"
                            + "AND PEDIDO.SOLICITADO_POR=persona.rut "
                            + "AND persona.shipCode=sucursal.shipCode "
                            + "AND sucursal.nombreConcesionario=concesionario.nombreConcesionario "
                            + "AND concesionario.nombreConcesionario=(SELECT concesionario.nombreConcesionario "
                                                                    + "FROM concesionario, persona,sucursal "
                                                                    + "WHERE persona.shipCode=sucursal.shipCode "
                                                                    + "AND sucursal.nombreConcesionario = concesionario.nombreConcesionario "
                                                                    + "AND persona.rut=" + rutEnSesion + ") "
                                                                    + "order by fecha desc";

        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        String rutOperario = "";
        String nombre = "";
        int cantidadTotal = 0;
        int precio = 0;
        int precioTotal = 0;
        while (dr.Read())
        {
            rutOperario = dr[0].ToString();
            nombre = dr[1].ToString();
            cantidadTotal += int.Parse(dr[3].ToString());
            precio = int.Parse(dr[4].ToString());
        }
        precioTotal = precio * cantidadTotal;
        cantidad.Add("rut", rutOperario);
        cantidad.Add("nombre", nombre);
        cantidad.Add("cantidad", cantidadTotal);
        cantidad.Add("precioTotal", precioTotal);
        dr.Close();

        dt.Columns.Add("rut"); dt.Columns.Add("nombre"); dt.Columns.Add("cantidad");  dt.Columns.Add("precioTotal");
        dt.Rows.Add(cantidad["rut"], cantidad["nombre"], cantidad["cantidad"], cantidad["precioTotal"]);
        return dt;
    }
    //resumen por rut total
    public DataTable detallePedidoPorRutResumenTotal(int rut, int rutEnSesion)
    {
        Hashtable cantidad = new Hashtable();
        con.ConnectionString = connectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = @"SELECT persona.rut,persona.nombre,MATERIALES_PEDIDO.codigo,MATERIALES_PEDIDO.cantidad, MATERIALES_PEDIDO.valor AS precio, pedido.fecha_solicitud as fecha
					        from MATERIALES_PEDIDO,PEDIDO,persona,sucursal,concesionario
					        WHERE persona.rut=" + rut + " "
                            + "AND MATERIALES_PEDIDO.id_pedido=PEDIDO.ID_PEDIDO "
                            + "AND PEDIDO.E_VBELN_PEDIDO !=''"
                            + "AND PEDIDO.SOLICITADO_POR=persona.rut "
                            + "AND persona.shipCode=sucursal.shipCode "
                            + "AND sucursal.nombreConcesionario=concesionario.nombreConcesionario "
                            + "AND concesionario.nombreConcesionario=(SELECT concesionario.nombreConcesionario "
                                                                    + "FROM concesionario, persona,sucursal "
                                                                    + "WHERE persona.shipCode=sucursal.shipCode "
                                                                    + "AND sucursal.nombreConcesionario = concesionario.nombreConcesionario "
                                                                    + "AND persona.rut=" + rutEnSesion + ") "
                                                                    + "order by fecha desc";

        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        String rutOperario = "";
        String nombre = "";
        int cantidadTotal = 0;
        int precio = 0;
        int precioTotal = 0;
        while (dr.Read())
        {
            rutOperario = dr[0].ToString();
            nombre = dr[1].ToString();
            cantidadTotal += int.Parse(dr[3].ToString());
            precio = int.Parse(dr[4].ToString());
        }
        precioTotal = precio * cantidadTotal;
        cantidad.Add("rut", rutOperario);
        cantidad.Add("nombre", nombre);
        cantidad.Add("cantidad", cantidadTotal);
        cantidad.Add("precioTotal", precioTotal);
        dr.Close();

        dt.Columns.Add("rut"); dt.Columns.Add("nombre"); dt.Columns.Add("cantidad"); dt.Columns.Add("precioTotal");
        dt.Rows.Add(cantidad["rut"], cantidad["nombre"], cantidad["cantidad"], cantidad["precioTotal"]);
        return dt;
    }
    //sin fecha ni marca
    public String detallePedidoPorRut(int rut, int rutEnSesion)
    {
        return @"SELECT persona.rut,persona.nombre,MATERIALES_PEDIDO.codigo,MATERIALES_PEDIDO.cantidad, MATERIALES_PEDIDO.valor AS precio, pedido.fecha_solicitud as fecha
					from MATERIALES_PEDIDO,PEDIDO,persona,sucursal,concesionario
					WHERE persona.rut=" + rut + " "
                    + "AND MATERIALES_PEDIDO.id_pedido=PEDIDO.ID_PEDIDO "
                    + "AND PEDIDO.E_VBELN_PEDIDO !=''"
                    + "AND PEDIDO.SOLICITADO_POR=persona.rut "
                    + "AND persona.shipCode=sucursal.shipCode "
                    + "AND sucursal.nombreConcesionario=concesionario.nombreConcesionario "
                    + "AND concesionario.nombreConcesionario=(SELECT concesionario.nombreConcesionario "
                                                            + "FROM concesionario, persona,sucursal "
                                                            + "WHERE persona.shipCode=sucursal.shipCode "
                                                            + "AND sucursal.nombreConcesionario = concesionario.nombreConcesionario "
                                                            + "AND persona.rut=" + rutEnSesion + ") "
                                                            + "order by fecha desc";
    }
    public String detallePedidoPorTodos(int rutEnSesion, String marca, String desde, String hasta)
    {
        return @"SELECT persona.rut,persona.nombre,MATERIALES_PEDIDO.codigo,MATERIALES_PEDIDO.descripcion,MATERIALES_PEDIDO.cantidad, MATERIALES_PEDIDO.valor AS precio, pedido.fecha_solicitud as fecha
					from MATERIALES_PEDIDO,PEDIDO,persona,sucursal,concesionario
					WHERE  FECHA_SOLICITUD BETWEEN '" +desde+"' and '"+hasta+"'" 
                    + "AND MATERIALES_PEDIDO.marca='" + marca + "' "
                    + "AND MATERIALES_PEDIDO.id_pedido=PEDIDO.ID_PEDIDO "
                    + "AND PEDIDO.E_VBELN_PEDIDO !=''"
                    + "AND PEDIDO.SOLICITADO_POR=persona.rut "
                    + "AND persona.shipCode=sucursal.shipCode "
                    + "AND sucursal.nombreConcesionario=concesionario.nombreConcesionario "
                    + "AND concesionario.nombreConcesionario=(SELECT concesionario.nombreConcesionario "
                                                            + "FROM concesionario, persona,sucursal "
                                                            + "WHERE persona.shipCode=sucursal.shipCode "
                                                            + "AND sucursal.nombreConcesionario = concesionario.nombreConcesionario "
                                                            + "AND persona.rut=" + rutEnSesion + ") "
                                                            + "order by fecha desc";
    }
    public int[] detalleCotizacionVsCompraPorCodigo(String codigo, String marca, String desde, String hasta)
    {
        List<int> cotCompra = new List<int>();
        con.ConnectionString = connectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = @"SELECT count(*)
                            FROM MATERIALES_PEDIDO,PEDIDO
                            WHERE  MATERIALES_PEDIDO.marca='" + marca + "' "
                            + "AND MATERIALES_PEDIDO.id_pedido=PEDIDO.ID_PEDIDO "
                            + "AND FECHA_SOLICITUD BETWEEN '" + desde + "' and '" + hasta + "' "
                            + "AND MATERIALES_PEDIDO.codigo='" + codigo + "' "
                            + "AND PEDIDO.E_VBELN_PEDIDO IS NOT NULL" ;
        
        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        if (dr.Read())
        {
            cotCompra.Add(int.Parse(dr[0].ToString()));
        }

        dr.Close();
        cmd1.Connection = con;
        cmd1.CommandType = System.Data.CommandType.Text;
        cmd1.CommandText = @" SELECT count(*)
                            FROM MATERIALES_PEDIDO,PEDIDO
                            WHERE  MATERIALES_PEDIDO.marca='" + marca + "' "
                            + "AND MATERIALES_PEDIDO.id_pedido=PEDIDO.ID_PEDIDO "
                            + "AND FECHA_SOLICITUD BETWEEN '" + desde + "' and '" + hasta + "' "
                            + "AND MATERIALES_PEDIDO.codigo='" + codigo + "' "
                            + "AND PEDIDO.E_VBELN_PEDIDO IS  NULL " 
                            + "AND PEDIDO.E_VBELN IS NOT NULL";
        
        cmd1.CommandTimeout = 10;
        dr1 = cmd1.ExecuteReader();
        if (dr1.Read())
        {
            cotCompra.Add(int.Parse(dr1[0].ToString()));
        }
        
        dr1.Close();
        con.Close();

        return cotCompra.ToArray<int>();
    }
    public int[] detalleCotizacionVsCompraPorCodigoSucursal(String codigoSucursal, String marca, String desde, String hasta)
    {
        List<int> cotCompra = new List<int>();
        con.ConnectionString = connectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = @"SELECT COUNT(*)
                            FROM MATERIALES_PEDIDO,PEDIDO,sucursal,persona
                            WHERE  sucursal.shipCode='"+codigoSucursal+"' "
                            +"AND sucursal.shipCode=persona.shipCode "
                            +"AND PEDIDO.SOLICITADO_POR=persona.rut "
                            +"AND MATERIALES_PEDIDO.marca='"+marca+"' "
                            +"AND MATERIALES_PEDIDO.id_pedido=PEDIDO.ID_PEDIDO "
                            +"AND FECHA_SOLICITUD BETWEEN '"+desde+"' and '"+hasta+"' "
                            +"AND PEDIDO.E_VBELN_PEDIDO IS NOT NULL";

        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        if (dr.Read())
        {
            cotCompra.Add(int.Parse(dr[0].ToString()));
        }

        dr.Close();
        cmd1.Connection = con;
        cmd1.CommandType = System.Data.CommandType.Text;
        cmd1.CommandText = @"SELECT COUNT(*)
                            FROM MATERIALES_PEDIDO,PEDIDO,sucursal,persona
                            WHERE  sucursal.shipCode='" + codigoSucursal + "' "
                            + "AND sucursal.shipCode=persona.shipCode "
                            + "AND PEDIDO.SOLICITADO_POR=persona.rut "
                            + "AND MATERIALES_PEDIDO.marca='" + marca + "' "
                            + "AND MATERIALES_PEDIDO.id_pedido=PEDIDO.ID_PEDIDO "
                            + "AND FECHA_SOLICITUD BETWEEN '" + desde + "' and '" + hasta + "' "
                            + "AND PEDIDO.E_VBELN_PEDIDO IS  NULL "
                            + "AND PEDIDO.E_VBELN IS NOT NULL";

        cmd1.CommandTimeout = 10;
        dr1 = cmd1.ExecuteReader();
        if (dr1.Read())
        {
            cotCompra.Add(int.Parse(dr1[0].ToString()));
        }

        dr1.Close();
        con.Close();

        return cotCompra.ToArray<int>();
    }
    public int[] detalleCotizacionVsCompraPorCodigoOperario(int rut, String marca, String desde, String hasta)
    {
        List<int> cotCompra = new List<int>();
        con.ConnectionString = connectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = @"SELECT COUNT(*)
                            FROM MATERIALES_PEDIDO,PEDIDO
                            WHERE PEDIDO.SOLICITADO_POR=" +rut+" "
                            +"AND MATERIALES_PEDIDO.marca='"+marca+"' "
                            +"AND MATERIALES_PEDIDO.id_pedido=PEDIDO.ID_PEDIDO "
                            +"AND FECHA_SOLICITUD BETWEEN '"+desde+"' and '"+hasta+"' "
                            +"AND PEDIDO.E_VBELN_PEDIDO IS NOT NULL";

        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        if (dr.Read())
        {
            cotCompra.Add(int.Parse(dr[0].ToString()));
        }

        dr.Close();
        cmd1.Connection = con;
        cmd1.CommandType = System.Data.CommandType.Text;
        cmd1.CommandText = @" SELECT COUNT(*)
                            FROM MATERIALES_PEDIDO,PEDIDO
                            WHERE PEDIDO.SOLICITADO_POR=" + rut + " "
                            + "AND MATERIALES_PEDIDO.marca='" + marca + "' "
                            + "AND MATERIALES_PEDIDO.id_pedido=PEDIDO.ID_PEDIDO "
                            + "AND FECHA_SOLICITUD BETWEEN '" + desde + "' and '" + hasta + "' "
                            + "AND PEDIDO.E_VBELN_PEDIDO IS  NULL "
                            + "AND PEDIDO.E_VBELN IS NOT NULL";

        cmd1.CommandTimeout = 10;
        dr1 = cmd1.ExecuteReader();
        if (dr1.Read())
        {
            cotCompra.Add(int.Parse(dr1[0].ToString()));
        }

        dr1.Close();
        con.Close();

        return cotCompra.ToArray<int>();
    }
    public int[] detalleCotizacionVsCompraPorTodos(String marca, String desde, String hasta)
    {
        List<int> cotCompra = new List<int>();
        con.ConnectionString = connectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = @"SELECT count(*)
                            FROM MATERIALES_PEDIDO,PEDIDO
                            WHERE  MATERIALES_PEDIDO.marca='" + marca + "' "
                            + "AND MATERIALES_PEDIDO.id_pedido=PEDIDO.ID_PEDIDO "
                            + "AND FECHA_SOLICITUD BETWEEN '" + desde + "' and '" + hasta + "' "
                            + "AND PEDIDO.E_VBELN_PEDIDO IS NOT NULL";

        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        if (dr.Read())
        {
            cotCompra.Add(int.Parse(dr[0].ToString()));
        }

        dr.Close();
        cmd1.Connection = con;
        cmd1.CommandType = System.Data.CommandType.Text;
        cmd1.CommandText = @" SELECT count(*)
                            FROM MATERIALES_PEDIDO,PEDIDO
                            WHERE  MATERIALES_PEDIDO.marca='" + marca + "' "
                            + "AND MATERIALES_PEDIDO.id_pedido=PEDIDO.ID_PEDIDO "
                            + "AND FECHA_SOLICITUD BETWEEN '" + desde + "' and '" + hasta + "' "
                            + "AND PEDIDO.E_VBELN_PEDIDO IS  NULL "
                            + "AND PEDIDO.E_VBELN IS NOT NULL";

        cmd1.CommandTimeout = 10;
        dr1 = cmd1.ExecuteReader();
        if (dr1.Read())
        {
            cotCompra.Add(int.Parse(dr1[0].ToString()));
        }

        dr1.Close();
        con.Close();

        return cotCompra.ToArray<int>();
    }
    public int detalleFallidaPorCodigo(String codigo,String marca, String desde, String hasta)
    {
        int fallida = 0;
        con.ConnectionString = connectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = @"select count (num_BackOrder)from BACKORDER
                            where codigo_rep='"+codigo+"'AND marca='"+marca+"'AND fecha_creacion between '"+desde+"' and '"+hasta+"'";

        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        if (dr.Read())
        {
            fallida+=int.Parse(dr[0].ToString());
        }

        dr.Close();
        cmd1.Connection = con;
        cmd1.CommandType = System.Data.CommandType.Text;
        cmd1.CommandText = @"select count (num_VFC)from VFC
                             where codigo_rep='"+codigo+"' AND marca='"+marca+"'AND fecha_creacion between '"+desde+"' and '"+hasta+"'";

        cmd1.CommandTimeout = 10;
        dr1 = cmd1.ExecuteReader();
        if (dr1.Read())
        {
            fallida+=int.Parse(dr1[0].ToString());
        }

        dr1.Close();
        cmd2.Connection = con;
        cmd2.CommandType = System.Data.CommandType.Text;
        cmd2.CommandText = @"select count (idDescartado)from DESCARTADOS
                            where codigo_rep='" + codigo + "' AND marca='" + marca + "'AND fecha_creacion between '" + desde + "' and '" + hasta + "'";

        cmd.CommandTimeout = 10;
        dr2 = cmd.ExecuteReader();
        if (dr2.Read())
        {
            fallida+=int.Parse(dr2[0].ToString());
        }

        dr2.Close();
        con.Close();

        return fallida;
    }
    public int detalleFallidaPorCodigoOperario(int codigoOperario, String marca, String desde, String hasta)
    {
        int fallida = 0;
        con.ConnectionString = connectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = @"select count (num_BackOrder)from BACKORDER
                            where rut_user =" + codigoOperario + " AND marca='" + marca + "'AND fecha_creacion between '" + desde + "' and '" + hasta + "'";

        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        if (dr.Read())
        {
            fallida += int.Parse(dr[0].ToString());
        }

        dr.Close();
        cmd1.Connection = con;
        cmd1.CommandType = System.Data.CommandType.Text;
        cmd1.CommandText = @"select count (num_VFC)from VFC
                            where rut_user =" + codigoOperario + " AND marca='" + marca + "'AND fecha_creacion between '" + desde + "' and '" + hasta + "'";

        cmd1.CommandTimeout = 10;
        dr1 = cmd1.ExecuteReader();
        if (dr1.Read())
        {
            fallida += int.Parse(dr1[0].ToString());
        }

        dr1.Close();
        cmd2.Connection = con;
        cmd2.CommandType = System.Data.CommandType.Text;
        cmd2.CommandText = @"select count (idDescartado)from DESCARTADOS
                           where rut_user =" + codigoOperario + " AND marca='" + marca + "'AND fecha_creacion between '" + desde + "' and '" + hasta + "'";

        cmd.CommandTimeout = 10;
        dr2 = cmd.ExecuteReader();
        if (dr2.Read())
        {
            fallida += int.Parse(dr2[0].ToString());
        }

        dr2.Close();
        con.Close();

        return fallida;
    }
    public int detalleFallidaPorTodos(String marca, String desde, String hasta)
    {
        int fallida = 0;
        con.ConnectionString = connectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = @"select count (num_BackOrder)from BACKORDER
                            where  marca='" + marca + "'AND fecha_creacion between '" + desde + "' and '" + hasta + "'";

        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        if (dr.Read())
        {
            fallida += int.Parse(dr[0].ToString());
        }

        dr.Close();
        cmd1.Connection = con;
        cmd1.CommandType = System.Data.CommandType.Text;
        cmd1.CommandText = @"select count (num_VFC)from VFC
                            where marca='" + marca + "'AND fecha_creacion between '" + desde + "' and '" + hasta + "'";

        cmd1.CommandTimeout = 10;
        dr1 = cmd1.ExecuteReader();
        if (dr1.Read())
        {
            fallida += int.Parse(dr1[0].ToString());
        }

        dr1.Close();
        cmd2.Connection = con;
        cmd2.CommandType = System.Data.CommandType.Text;
        cmd2.CommandText = @"select count (idDescartado)from DESCARTADOS
                           where marca='" + marca + "'AND fecha_creacion between '" + desde + "' and '" + hasta + "'";

        cmd.CommandTimeout = 10;
        dr2 = cmd.ExecuteReader();
        if (dr2.Read())
        {
            fallida += int.Parse(dr2[0].ToString());
        }

        dr2.Close();
        con.Close();

        return fallida;
    }
    public String obtienePrefijoMarca(String marcaRecibida)
    {
        String marca = "";
        con.ConnectionString = connectionString;
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = @"SELECT prefijo FROM GRUPO_MATERIALES WHERE marca ='"+marcaRecibida+"'";

        cmd.CommandTimeout = 10;
        dr = cmd.ExecuteReader();
        if (dr.Read())
        {
           marca=dr[0].ToString();
        }
        dr.Close();
        con.Close();
        return marca;

    }

    public Boolean siConfirmoPedido(String numeroCotizacion, String idSession)
    {
        int cantidadFilas = 0;
        String query = "select count(*) from pedido where E_VBELN = " + numeroCotizacion + " and idSession = '" + idSession + "' and E_VBELN_PEDIDO is not NULL";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand(query, conn);
            try
            {
                conn.Open();
                cantidadFilas = (Int32)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                logger.Error("Error en [siConfirmoPedido]. Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
            }
        }
        return cantidadFilas > 0;
    }

    public Boolean siEsForaneo(String numeroCotizacion)
    {
        int cantidadFilas = 0;
        String query = "select count(*) from pedido where E_VBELN = " + numeroCotizacion + " and I_AUART='ZB04'";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand(query, conn);
            try
            {
                conn.Open();
                cantidadFilas = (Int32)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                logger.Error("Error en [siConfirmoPedido]. Message: " + ex.Message + ". Stack: " + ex.StackTrace + ". Inner: " + ex.InnerException);
            }
        }
        return cantidadFilas > 0;
    }

    /*
     *     Int32 newProdID = 0;
    string sql =
        "INSERT INTO Production.ProductCategory (Name) VALUES (@Name); "
        + "SELECT CAST(scope_identity() AS int)";
    using (SqlConnection conn = new SqlConnection(connString))
    {
        SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@Name", SqlDbType.VarChar);
        cmd.Parameters["@name"].Value = newName;
        try
        {
            conn.Open();
            newProdID = (Int32)cmd.ExecuteScalar();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    return (int)newProdID;
     */


    /* en desarrollo
    public Pedido obtenerPedidoPorCotizacion(String numCotizacion)
    {
        
        Pedido pedido = new Pedido();
        String query = "select * from pedido where E_VBELN = " + numCotizacion;
        DataSet _dSet = new DataSet();
        SqlConnection _conection = new SqlConnection();
        SqlCommand _comando = new SqlCommand();
        SqlDataAdapter _adapter = new SqlDataAdapter();

        _conection.ConnectionString = ConfigurationManager.ConnectionStrings["skbergeConnectionString"].ConnectionString;
        _conection.Open();

        try
        {
            if (_conection.State == ConnectionState.Open)
            {
                _comando.Connection = _conection;
                _comando.CommandType = CommandType.Text;
                _comando.CommandText = query;
                _adapter.SelectCommand = _comando;
                _adapter.Fill(_dSet);
            }
            foreach (DataRow dr in _dSet.Tables[0].Rows)
            {
                this.Id = dr["ID_PEDIDO"].ToString();
                this.IdSession = dr["idSession"].ToString();
                this.NumeroCotizacion = dr["E_VBELN"].ToString();
                this.ClaseDocumentosVentas = dr["I_AUART"].ToString();
                this.FechaValidez = dr["I_BNDDT"].ToString();
                this.CodigoClienteSAP = dr["I_KUNNR"].ToString();
                this.DestinatarioMercancia = dr["I_KUNNR2"].ToString();
                this.CanalDeRepuestos = dr["I_SPART"].ToString();
                this.DescrpcionCotizacion = dr["I_TEXTO"].ToString();
                dr["I_VKORG"].ToString();
                dr["I_VTWEG"].ToString();
                dr["SOLICITADO_POR"].ToString();
                dr["FECHA_SOLICITUD"].ToString();
                dr["estado"].ToString();
                dr["FECHA_EXPIRACION"].ToString();
                dr["FECHA_EXPIRACION_SYS"].ToString();
                dr["SUCURSAL"].ToString();
                dr["TOTAL_NETO"].ToString();
                dr["COMENTARIOS"].ToString();
                dr["E_VBELN_PEDIDO"].ToString();
                dr["I_AUART_PEDIDO"].ToString();
                dr["I_LPRIO_PEDIDO"].ToString();
            }
        }
        catch (Exception ex)
        {
            _dSet = null;
            logger.Error("Error en [obtenerPedidoPorCotizacion]. Message: " + ex.Message + ". Inner: " + ex.InnerException + ". Stack: " + ex.StackTrace);
        }
        finally
        {
            if (_conection.State == ConnectionState.Open)
                _conection.Close();
            _comando = null;
            _conection = null;
            _adapter = null;
        }
        return pedido;
         
    }
     * * */
}