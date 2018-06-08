using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiAppVenom.Dominio
{
    public class Logro
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

        private Boolean desbloqueado;
        public Boolean Desbloqueado
        {
            get
            {
                return this.desbloqueado;
            }
            set
            {
                this.desbloqueado = value;
            }
        }

        public Logro(int id, String texto, Boolean conseguido)
        {
            this.id = id;
            this.texto = texto;
            this.desbloqueado = conseguido;
        }


        public Logro()
        {
            this.id = -1;
            this.texto = null;
            this.desbloqueado = false;
        }
    }
}
