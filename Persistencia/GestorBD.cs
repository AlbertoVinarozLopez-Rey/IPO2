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
            connection = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\bbdd_VenomGotchi.mdb");
            command = connection.CreateCommand();
        }

        public void registrarUsuario(String usuario, String pass)
        {
            try
            {
                command.CommandText = "INSERT INTO tb_usuario (usuario, pass) VALUES ('" + usuario + "', '" + pass + "')";
                command.CommandType = System.Data.CommandType.Text;
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
        }

        public Avatar LeerAvatar(String usuario)
        {
            Avatar av = new Avatar();
            try
            {
                command.CommandText = "SELECT * FROM tb_avatar WHERE usuario='" + usuario + "')";
                command.CommandType = System.Data.CommandType.Text;
                connection.Open();

                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    av = new Avatar(usuario, Convert.ToInt32(reader["nivel"].ToString()), Convert.ToInt32(reader["puntos"].ToString()), 
                        Convert.ToInt32(reader["monedas"].ToString()), Convert.ToInt32(reader["apetito"].ToString()), 
                        Convert.ToInt32(reader["energia"].ToString()), Convert.ToInt32(reader["diversion"].ToString()), 
                        reader["logros"].ToString(), Convert.ToInt32(reader["monedasConseguidas"].ToString()), 
                        Convert.ToInt32(reader["partidas"].ToString()), Convert.ToInt32(reader["puzzles"].ToString()));
                }

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
            return av;

        }


    }
}
