using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;

namespace Login_de_PabloJimenez
{
    class DAOUsuario
    {
        SQLiteConnection conexion;

        public bool Conectar(string db)
        {
            string cadenaConexion = String.Format("Data Source={0};Version=3;Foreign Keys=on;FailIfMissing=true;", db);

            try
            {
                conexion = new SQLiteConnection(cadenaConexion);
                conexion.Open();
                return true;
            }
            catch ( SQLiteException e )
            {
                throw new Exception("Error de conexión: " + e.Message);
            }
        }

        public void Desconectar()
        {
            try
            {
                if ( conexion != null )
                    conexion.Close();
            }
            catch ( SQLiteException e )
            {
                throw new Exception("Error de desconexión: " + e.Message);
            }
        }

        public bool Conectado()
        {
            if ( conexion != null )
                return conexion.State == ConnectionState.Open;

            return false;
        }

        public bool ComprobarExistenciaUsuario(string uid)
        {
            string orden = String.Format("select * from usuario where user = '{0}'", uid);
            SQLiteCommand cmd = new SQLiteCommand(orden, conexion);
            SQLiteDataReader lector;

            try
            {
                lector = cmd.ExecuteReader();

                if ( lector.HasRows )
                    return true;
            }
            catch ( SQLiteException e )
            {
                throw new Exception("ERROR: " + e.Message);
            }

            return false;
        }

        public bool Activo(string uid)
        {
            string orden = String.Format("select * from usuario where user = '{0}'", uid);
            SQLiteCommand cmd = new SQLiteCommand(orden, conexion);
            SQLiteDataReader lector;
            Usuario usuario = new Usuario();

            try
            {
                lector = cmd.ExecuteReader();

                if ( lector.HasRows )
                {
                    while ( lector.Read() )
                    {
                        usuario.User = lector["user"].ToString();
                        usuario.Password = lector["password"].ToString();
                        usuario.Activo = int.Parse(lector["activo"].ToString());
                    }

                    if ( usuario.Activo == 1 )
                        return true;
                }
            }
            catch ( SQLiteException e )
            {
                throw new Exception("ERROR: " + e.Message);
            }

            return false;
        }

        public bool? SeleccionarUsuario(string uid, string pwd)
        {
            string orden = String.Format("select * from usuario where user = '{0}'", uid);
            SQLiteCommand cmd = new SQLiteCommand(orden, conexion);
            SQLiteDataReader lector;
            Usuario usuario = new Usuario();

            try
            {
                lector = cmd.ExecuteReader();

                if ( lector.HasRows )
                {
                    while ( lector.Read() )
                    {
                        usuario.User = lector["user"].ToString();
                        usuario.Password = lector["password"].ToString();
                        usuario.Activo = int.Parse(lector["activo"].ToString());
                    }

                    if ( usuario.Password == pwd )
                    {
                        if ( usuario.Activo == 1 )
                            return true;
                        else
                            return false;
                    }
                    else
                        return false;
                }
            }
            catch (SQLiteException e)
            {
                throw new Exception("ERROR: " + e.Message);
            }

            return null;
        }

        public void BloquearUsuario(string uid)
        {
            string orden = String.Format("update usuario set activo = 0 where user = '{0}'", uid);
            SQLiteCommand cmd = new SQLiteCommand(orden, conexion);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException e)
            {
                throw new Exception("ERROR: " + e.Message);
            }
        }
    }
}
