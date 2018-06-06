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
        private OleDbConnection connection;
        private OleDbCommand command;
        private OleDbDataReader reader;

        public GestorBD()
        {
            connectTo();
        }
        private void connectTo()
        {
            connection = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\alber\Source\Repos\IPO2\bbddVenomGotchi.accdb");
            command = connection.CreateCommand();
        }

        public void registrarUsuario(String usuario, String pass)
        {
            Avatar av = new Avatar(usuario, 0, 0, 0, 100, 100, 100,"", 0, 0, 0);
            try
            {
                command.CommandText = "INSERT INTO tb_usuario (usuario, pass) VALUES ('" + usuario + "', '" + pass + "')";
                command.CommandType = System.Data.CommandType.Text;
                connection.Open();
                command.ExecuteNonQuery();

                insertarNuevoAvatar(av);
            }
            catch (Exception)
            {
                throw;
            }
           
        }

        public void insertarNuevoAvatar(Avatar av)
        {
            String logros = "";
            try
            {
                command.CommandText = "INSERT INTO tb_avatar (usuario, nivel, puntos, monedas, apetito, energia, diversion, logros, monedasConseguidas, partidas, puzzles) VALUES ('" + av.Usuario + "', " + av.Nivel + "," +
                    " " + av.Puntos_nivel + ", " + av.Monedas +", " + av.Apetito + ", " + av.Energia + ", " + av.Diversion + ", '" + logros + "', " + av.Monedas_conseguidas + ", " + av.Partidas_jugadas + "," +
                    " " + av.Puzzles_resueltos+ ")";
                command.CommandType = System.Data.CommandType.Text;
                command.ExecuteNonQuery();

            }
            catch (Exception)
            {
                throw;
            }

        }

        public Avatar leerAvatar(String usuario)
        {
            Avatar av = new Avatar();
            try
            {
                command.CommandText = "SELECT * FROM tb_avatar WHERE usuario='" + usuario + "'";
                command.CommandType = System.Data.CommandType.Text;
                connection.Open();

                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    av = new Avatar(usuario, Convert.ToInt32(reader["nivel"].ToString()), Convert.ToInt32(reader["puntos"].ToString()), 
                        Convert.ToInt32(reader["monedas"].ToString()), Convert.ToInt32(reader["apetito"].ToString()), 
                        Convert.ToInt32(reader["energia"].ToString()), Convert.ToInt32(reader["diversion"].ToString()), 
                        "", Convert.ToInt32(reader["monedasConseguidas"].ToString()), Convert.ToInt32(reader["partidas"].ToString()), Convert.ToInt32(reader["puzzles"].ToString()));
                }

            }
            catch (Exception)
            {
                throw;
            }
           
            return av;

        }

        public void actualizarAvatar(Avatar av, String logros)
        {
            
            try
            {
                command.CommandText = "UPDATE tb_avatar SET nivel="+av.Nivel+ ", puntos=" + av.Puntos_nivel + ", monedas=" + av.Monedas + "," +
                    " apetito=" + av.Apetito + ", energia=" + av.Energia + ", diversion=" + av.Diversion + ", " +
                    "logros='"+logros+"', monedasConseguidas=" + av.Monedas_conseguidas + ", partidas=" + av.Partidas_jugadas + ", puzzles=" + av.Puzzles_resueltos +
                    " WHERE usuario='" + av.Usuario + "'";
                command.CommandType = System.Data.CommandType.Text;
                connection.Open();

                command.ExecuteNonQuery();

                
            }
            catch (Exception)
            {
                throw;
            }
           
        }

        public void eliminarUsuario(Avatar av)
        {
            try
            {
                command.CommandText = "DELETE FROM tb_usuario WHERE usuario='"+av.Usuario+"'";
                command.CommandType = System.Data.CommandType.Text;
                connection.Open();

                command.ExecuteNonQuery();


            }
            catch (Exception)
            {
                throw;
            }
          
        }

        public int autenticar (String usuario, String pass)
        {
            int resultado=1;
            try
            {
                command.CommandText = "SELECT usuario FROM tb_usuario WHERE usuario='" + usuario + "' AND pass='" + pass + "'";
                command.CommandType = System.Data.CommandType.Text;
                connection.Open();

                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if ((reader["usuario"].ToString()).Equals(usuario))
                    {
                        resultado = 0;
                    }
               
                }
                }
            catch (Exception e)
            {
                e.ToString();
            }
          
            return resultado;
      
                
        }

       

}
}
