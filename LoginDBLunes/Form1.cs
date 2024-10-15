using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using LoginDBLunes.BD;

namespace LoginDBLunes
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //Utilizo dictionary para almacenar los intentos fallidos por usuario
        private static Dictionary<string, int> intentosFalllidos = new Dictionary<string, int>();
        private const int max_intentos = 3;

        private void btn_enviar_Click(object sender, EventArgs e)
        {
            //Asigno a las nuevas variables lo que el usuario ingresa en el textBox
            string user = txtUser.Text;
            string pass = txtPass.Text;

            //Método para verificar si el usuario y clave es correcto, de lo contrario le informa error
            if (ValidarUsuario(user, pass))
            {
                MessageBox.Show("¡Inicio de sesión exitoso!");
            }
            else
            {
                MessageBox.Show("Nombre de usuario y contraseñas inválidos.");
            }
            
        }

        //Validación de parametros
        private bool ValidarUsuario(string usuario, string password)
        {
            bool esValido = false;

            //Using se utiliza por buenas practicas y para cerrar la conexión automaticamente, sin tener que hacer Close() o Dispose()
            using(SqlConnection conexion = Conexion.getConexion()) //Genero mi conexión
            {
                try //Try para controlar los errores
                {
                    
                    conexion.Open(); //Abro la conexión

                    if (EstaBloqueado(usuario)) //Validación para el bloqueo de usuario
                    {
                        MessageBox.Show("Su usuario esta bloqueado por intentos fallidos. Intente más tarde!");
                        return false;
                    }

                    //Consulta a la bd donde selecciono el usuario y clave para validar si es correcto
                    string sql = "SELECT * FROM dbo.tbl_user WHERE usuario = @usuario AND contrasena = @password";
                    
                    //SqlCommand es el que ejecuta la consulta a la base de datos con la conexión
                    using(SqlCommand cmd =  new SqlCommand(sql, conexion))
                    {

                        //Le agrego los valores a la consulta y luego los ejecuto con reader
                        cmd.Parameters.AddWithValue("@usuario", usuario);
                        cmd.Parameters.AddWithValue("@password", password);

                        using (SqlDataReader reader =  cmd.ExecuteReader())
                        {
                            //Devuelve una fila en caso de que los valores consultados sean correctos
                            esValido = reader.HasRows;
                        }
                    }


                } catch (SqlException ex)
                {
                    MessageBox.Show("Error al conectar a la base de datos: " + ex.Message);
                }
            }
            return esValido;
        }

        //Private indica que la función solo será usada en esta clase
        private static bool EstaBloqueado(string usuario)
        {
            //Verifica si el diccionario intentosFallidos tiene una entrada para usuario
            if (!intentosFalllidos.ContainsKey(usuario))
            {
                //En caso de que el usuario no existe o sea incorrecto los intentos se establecen en 1
                intentosFalllidos.Add(usuario, 1);
            } 
            else
            {
                //Se aumenta hasta que llegue a 3
                intentosFalllidos[usuario]++;
            }
            //Devuelve true si los intentos son => a 3
            return intentosFalllidos[usuario] >= max_intentos;
        }
    }
}
