using System;


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

        private String descripcion;
        public String Descripcion
        {
            get
            {
                return this.descripcion;
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
            this.descripcion = texto;
            this.desbloqueado = conseguido;
        }


        public Logro()
        {
            this.id = -1;
            this.descripcion = null;
            this.desbloqueado = false;
        }
    }
}
