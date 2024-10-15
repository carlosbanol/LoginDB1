using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace LoginDBLunes.BD
{
    public class Conexion
    {

        //Separo la conexión para seguir las buenas practicas
        public static SqlConnection getConexion()
        {
            //Cadena de conexión als ervidor y base de datos
            string sql = "server=CARLOS-BANOL; database=LOGINDB; Integrated Security=True; user=sa; password=pele2001";

            //Le paso la cadena a SqlServer
            SqlConnection conexion = new SqlConnection(sql);
            return conexion;

        }
    }
}
