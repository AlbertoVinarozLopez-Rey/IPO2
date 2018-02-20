using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    class Avatar

    {
        private int energia;
        private int apetito;
        private int diversion;
          
        //hola 
        public Avatar(int energia, int apetito, int diversion)
        {
            this.Energia = energia;
            this.Apetito = apetito;
            this.Diversion = diversion;
        }

        public int Energia { get => energia; set => energia = value; }
        public int Apetito { get => apetito; set => apetito = value; }
        public int Diversion { get => diversion; set => diversion = value; }
    }

    
}
