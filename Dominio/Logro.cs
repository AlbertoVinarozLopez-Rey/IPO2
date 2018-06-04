using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiAppVenom.Dominio
{
    class Logro
    {
        private int id;
        public int Id
        {
            get
            {
                return this.id;
            }
        }

        private String texto;
        public String Texto
        {
            get
            {
                return this.texto;
            }
        }

        private Boolean conseguido;
        public Boolean Conseguido
        {
            get
            {
                return this.conseguido;
            }
            set
            {
                this.conseguido = value;
            }
        }


        /*********************************************************************
        *
        * Method name: Logro (Constructor)
        *
        * Description of the Method: Constructor del objeto Logro.
        *
        * Calling arguments: int : id, String : texto, Boolean : conseguido
        *
        * Return value: none
        *
        *********************************************************************/
        public Logro(int id, String texto, Boolean conseguido)
        {
            this.id = id;
            this.texto = texto;
            this.conseguido = conseguido;
        }


        /*********************************************************************
        *
        * Method name: Logro (Constructor)
        *
        * Description of the Method: Constructor del objeto Logro.
        *
        * Calling arguments: none
        *
        * Return value: none
        *
        *********************************************************************/
        public Logro()
        {
            this.id = -1;
            this.texto = null;
            this.conseguido = false;
        }
    }
}
