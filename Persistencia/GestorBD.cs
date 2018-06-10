using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MiAppVenom.Persistencia
{
    public class GestorBD
    {
        private String connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=bbddVenomGotchi.accdb";

        public GestorBD()
        {     }
        

        public void registrarUsuario(String usuario, String pass)
        {
            Avatar av = new Avatar(usuario, 1, 0, 0, 100, 100, 100,"", 0, 0, 0);
            try
            {
                using (OleDbConnection connection = new OleDbConnection(connString))
                {
                    connection.Open();

                    string query = @"INSERT INTO tb_usuario (usuario, pass) VALUES ('" + usuario + "', '" + pass + "')";

                    OleDbCommand command = new OleDbCommand(query, connection);

                    command.ExecuteNonQuery();
                    insertarNuevoAvatar(av);
                }
            }
            catch (Exception e1)
            {
                e1.ToString();
            }
           
           
        }

        public void insertarNuevoAvatar(Avatar av)
        {
            String logros = "";
            try
            {
                using (OleDbConnection connection = new OleDbConnection(connString))
                {
                    connection.Open();

                    string query = @"INSERT INTO tb_avatar (usuario, nivel, puntos, monedas, apetito, energia, diversion, logros, monedasConseguidas, partidas, puzzles) VALUES ('" + av.Usuario + "', " + av.Nivel + "," +
                    " " + av.PuntosNivel + ", " + av.Monedas + ", " + av.Apetito + ", " + av.Energia + ", " + av.Diversion + ", '" + logros + "', " + av.MonedasTotales + ", " + av.PartidasPuzzle + "," +
                    " " + av.PuzzlesGanados + ")";

                    OleDbCommand command = new OleDbCommand(query, connection);

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e1)
            {
                e1.ToString();
            }


        }

        public Avatar leerAvatar(String usuario)
        {
            Avatar av = new Avatar();
            try
            {
                using (OleDbConnection connection = new OleDbConnection(connString))
                {
                    connection.Open();

                    string query = @"SELECT * FROM tb_avatar WHERE usuario='" + usuario + "'";
                    OleDbCommand command = new OleDbCommand(query, connection);

                    OleDbDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        av = new Avatar(usuario, Convert.ToInt32(reader["nivel"].ToString()), Convert.ToInt32(reader["puntos"].ToString()),
                            Convert.ToInt32(reader["monedas"].ToString()), Convert.ToInt32(reader["apetito"].ToString()),
                            Convert.ToInt32(reader["energia"].ToString()), Convert.ToInt32(reader["diversion"].ToString()),
                            reader["logros"].ToString(), Convert.ToInt32(reader["monedasConseguidas"].ToString()), Convert.ToInt32(reader["partidas"].ToString()), Convert.ToInt32(reader["puzzles"].ToString()));
                    }
                }
            }
            catch (Exception e1)
            {
                e1.ToString();
            }

            return av;

        }

        public void actualizarAvatar(Avatar av, String logros)
        {
            
            try
            {
                using (OleDbConnection connection = new OleDbConnection(connString))
                {
                    connection.Open();

                    string query = @"UPDATE tb_avatar SET nivel=" + av.Nivel + ", puntos=" + av.PuntosNivel + ", monedas=" + av.Monedas + "," +
                    " apetito=" + av.Apetito + ", energia=" + av.Energia + ", diversion=" + av.Diversion + ", " +
                    "logros='" + logros + "', monedasConseguidas=" + av.MonedasTotales + ", partidas=" + av.PartidasPuzzle + ", puzzles=" + av.PuzzlesGanados +
                    " WHERE usuario='" + av.Usuario + "'";

                    OleDbCommand command = new OleDbCommand(query, connection);
                    command.ExecuteNonQuery();
                }
                
            }
            catch (Exception e1)
            {
                e1.ToString();
            }

        }

        public void eliminarUsuario(Avatar av)
        {
            try
            {
                using (OleDbConnection connection = new OleDbConnection(connString))
                {
                    connection.Open();
                    string query = @"DELETE FROM tb_usuario WHERE usuario='" + av.Usuario + "'";
                    OleDbCommand command = new OleDbCommand(query, connection); 
                    command.ExecuteNonQuery();

                }
            }
            catch (Exception e1)
            {
                e1.ToString();
            }


        }

        public int autenticar (String usuario, String pass)
        {
            int resultado = 1;

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connString))
                {
                    connection.Open();

                    string query = @"SELECT usuario FROM tb_usuario WHERE usuario='" + usuario + "' AND pass='" + pass + "'";
                    OleDbCommand command = new OleDbCommand(query, connection);
                    OleDbDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        if ((reader["usuario"].ToString()).Equals(usuario))
                        {
                            resultado = 0;
                        }

                    }
                }
            }

            catch (Exception e1)
            {
                e1.ToString();
            }

            return resultado;
      
                
        }

        public Boolean comprobarUsuarioExistente(String usuario)
        {
            Boolean usuarioExistente=false;

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connString))
                {
                    connection.Open();

                    string query = @"SELECT usuario FROM tb_usuario WHERE usuario='" + usuario + "'";
                    OleDbCommand command = new OleDbCommand(query, connection);
                    OleDbDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        if ((reader["usuario"].ToString()).Equals(usuario))
                        {
                            usuarioExistente = true;
                        }

                    }
                }
            }

            catch (Exception e1)
            {
                e1.ToString();
            }

            return usuarioExistente;
        }



    }
}
