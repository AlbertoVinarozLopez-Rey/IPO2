using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiAppVenom.Dominio
{
    public class Comida
    {
        private String nombre;
        private int coste;
        private int energia;
        private int apetito;
        private int puntos;

        public int Coste { get => coste; set => coste = value; }
        public int Energia { get => energia; set => energia = value; }
        public int Apetito { get => apetito; set => apetito = value; }
        public int Puntos { get => puntos; set => puntos = value; }
        public string Nombre { get => nombre; set => nombre = value; }

        public Comida(string nombre, int coste, int energia, int apetito, int puntos)
        {
            this.nombre = nombre;
            this.coste = coste;
            this.energia = energia;
            this.apetito = apetito;
            this.puntos = puntos;
        }

    }
}
