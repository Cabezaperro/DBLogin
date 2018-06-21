using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login_de_PabloJimenez
{
    class Usuario
    {
        private string user;
        private string password;
        private int activo;
        private int intentos = 3;

        public string User
        {
            get { return user; }
            set { user = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public int Activo
        {
            get { return activo; }
            set { activo = value; }
        }

        public int Intentos
        {
            get { return intentos; }
            set { intentos = value; }
        }
    }
}
