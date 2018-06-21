using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Login_de_PabloJimenez
{
    class UsuarioVM : INotifyPropertyChanged
    {
        DAOUsuario dao = new DAOUsuario();
        List<Usuario> listaUsuarios = new List<Usuario>();
        string uid;
        string pwd;
        string mensaje;
        char[] prohibidos = { ' ', 'ñ', 'á', 'é', 'í', 'ó', 'ú', 'à', 'è', 'ì', 'ò', 'ù' }; 

        public string Uid
        {
            get { return uid; }
            set
            {
                if ( uid != value )
                {
                    uid = value;
                    Notificar("Uid");
                    Notificar("ColorUsuario");
                }
            }
        }

        public string Pwd
        {
            get { return pwd; }
            set
            {
                if ( pwd != value )
                {
                    pwd = value;
                    Notificar("Pwd");
                }
            }
        }

        public string Mensaje
        {
            get { return mensaje; }
            set
            {
                if ( mensaje != value )
                {
                    mensaje = value;
                    Notificar("Mensaje");
                }
            }
        }

        public string ColorUsuario
        {
            get
            {
                if ( ComprobarUsuario() )
                    return "Green";
                if ( !ComprobarUsuario() )
                    return "Red";

                return "Black";
            }
        }

        public bool ComprobarUsuario()
        {
            try
            {
                dao.Conectar("examen.db");

                if ( dao.Conectado() && Uid != null && dao.ComprobarExistenciaUsuario(Uid) )
                    return true;
            }
            catch
            {
                Mensaje = "Error de comprobación";
            }

            return false;
        }

        public RelayCommand Login_Click
        {
            get { return new RelayCommand(o => Login(), o => true); }
        }

        private bool ComprobarOrtografia()
        {
            foreach ( char c in Uid )
                if ( prohibidos.Contains(c) )
                {
                    Mensaje = "El usuario contiene caracteres no permitidos";
                    return false;
                }

            foreach ( char c in Pwd )
                if ( prohibidos.Contains(c) )
                {
                    Mensaje = "La contraseña contiene caracteres no permitidos";
                    return false;
                }

            return true;
        }

        private bool BuscarUsuario(Usuario usuario)
        {
            foreach ( Usuario u in listaUsuarios )
                if ( u.User == usuario.User )
                    return true;

            return false;
        }

        private void Login()
        {
            Usuario usuario = new Usuario();

            try
            {
                if ( !ComprobarOrtografia() )
                    return;

                dao.Conectar("examen.db");

                usuario.User = Uid;
                usuario.Password = Pwd;

                if ( !BuscarUsuario(usuario) )
                    listaUsuarios.Add(usuario);

                for ( int i = 0; i < listaUsuarios.Count; i++ )
                    if ( listaUsuarios[i].User == usuario.User )
                    {
                        usuario.Intentos = listaUsuarios[i].Intentos;

                        if ( dao.SeleccionarUsuario(usuario.User, usuario.Password) == true )
                            Mensaje = "Login realizado con éxito";
                        else if ( dao.SeleccionarUsuario(usuario.User, usuario.Password) == null )
                            Mensaje = "Ese usuario no existe en la base de datos";
                        else
                        {
                            if ( dao.Activo(usuario.User) )
                            {
                                if ( listaUsuarios[i].Intentos > 0 )
                                    listaUsuarios[i].Intentos--;

                                if ( listaUsuarios[i].Intentos == 0 )
                                {
                                    dao.BloquearUsuario(listaUsuarios[i].User);
                                    Mensaje = "El usuario ha sido bloqueado. Contacte con el administrador";
                                }
                                else
                                    Mensaje = String.Format("Contraseña incorrecta. Intentos restantes: {0}", listaUsuarios[i].Intentos);
                            }
                            else
                                Mensaje = "Ese usuario está bloqueado. Contacte con el administrador";
                        }
                    }
            }
            catch
            {
                Mensaje = "Error en el login";
            }
        }

        private void Notificar(string propiedad)
        {
            if ( PropertyChanged != null )
                PropertyChanged(this, new PropertyChangedEventArgs(propiedad));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
