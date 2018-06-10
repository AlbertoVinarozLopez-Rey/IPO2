using MiAppVenom.Dominio;
using MiAppVenom.Persistencia;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MiAppVenom
{
    public enum Estado
    {
       
        SOÑANDO,
        HAMBRIENTO,
        ABURRIDO,
        JUGANDO,
        FELIZ,
        ENFADADO,
        DORMIDO
        
    }

    public class Avatar
    {
        public List<Logro> logrosParaNotificar;
        public Boolean parpadeando;
        public Estado estadoActual;
        public Estado estadoAnterior;

        private GestorBD gestor;

        private Hashtable logros;
        public Hashtable Logros
        {
            get
            {
                return this.logros;
            }
      
        }

        private int puntos;
        public int PuntosNivel
        {
            get
            {
                return puntos;
            }
            set
            {
                if (value < 0)
                {
                    puntos = 0;
                }
                else
                {
                    if (value > (nivel * 100))
                    {
                        puntos = value - (nivel * 100);
                        nivel++;

                        switch (nivel)
                        {
                            case 3:
                                if (!((Logro)logros["1"]).Desbloqueado)
                                {
                                    desbloquearNuevoLogro("1");

                                }
                                break;

                            case 6:
                                if (!((Logro)logros["2"]).Desbloqueado)
                                {
                                    desbloquearNuevoLogro("2");

                                }
                                break;

                            case 10:
                                if (!((Logro)logros["3"]).Desbloqueado)
                                {
                                    desbloquearNuevoLogro("3");

                                    comprobarLogroOculto();
                                }
                                break;
                        }
                        nuevoNivel = true;
                    }
                    else
                    {
                        puntos = value;
                    }
                }
            }
        }


        private int monedas;
        public int Monedas
        {
            get
            {
                return monedas;
            }
            set
            {
                if (value > 1000)
                {
                    monedas = 1000;
                }
                else
                {
                    if (value < 0)
                    {
                        monedas = 0;
                    }
                    else
                    {
                        monedas = value;
                    }
                }
            }
        }

        private int nivel;
        public int Nivel
        {
            get
            {
                return nivel;
            }
            set
            {
                nivel = value;
            }
        }

       


        private int apetito;
        public int Apetito
        {
            get
            {
                return apetito;
            }
            set
            {
                if (value > 100)
                {
                    apetito = 100;
                }
                else
                {
                    if (value < 0)
                    {
                        apetito = 0;
                    }
                    else
                    {
                        apetito = value;
                    }
                }
            }
        }

        private int energia;
        public int Energia
        {
            get
            {
                return energia;
            }
            set
            {
                if (value > 100)
                {
                    energia = 100;
                }
                else
                {
                    if (value < 0)
                    {
                        energia = 0;
                    }
                    else
                    {
                        energia = value;
                    }
                }
            }
        }
        private int diversion;
        public int Diversion
        {
            get
            {
                return diversion;
            }
            set
            {
                if (value > 100)
                {
                    diversion = 100;
                }
                else
                {
                    if (value < 0)
                    {
                        diversion = 0;
                    }
                    else
                    {
                        diversion = value;
                    }
                }
            }
        }

        private Boolean nuevoNivel;
        public Boolean NuevoNivel
        {
            get
            {
                return nuevoNivel;
            }
            set
            {
                nuevoNivel = value;
            }
        }

        

        private int monedasTotales;
        public int MonedasTotales
        {
            get
            {
                return this.monedasTotales;
            }
            set
            {
                monedasTotales = value;
            }
        }
        private int partidasPuzzle;
        public int PartidasPuzzle
        {
            get
            {
                return this.partidasPuzzle;
            }
            set
            {
                partidasPuzzle = value;
            }
        }
        private int puzzlesGanados;
        public int PuzzlesGanados
        {
            get
            {
                return this.puzzlesGanados;
            }
            set
            {
               puzzlesGanados = value;
            }
        }
        
        private String usuario;
        public String Usuario
        {
            get
            {
                return this.usuario;
            }
            set
            {
                usuario = value;
            }
        }

        public Avatar(String usuario, int nivel, int puntos, int monedas, int apetito, int energia, int diversion, String logrosConseguidos, int monedasConseguidas, int partidas, int puzzles)
        {
            gestor = new GestorBD();
            logrosParaNotificar = new List<Logro>();
            this.logros = crearLogros();

            this.usuario = usuario;
            this.nivel = nivel;
            this.puntos = puntos;
            this.monedas = monedas;
            this.energia = energia;
            this.apetito = apetito;
            this.diversion = diversion;
            desbloquearLogros(logrosConseguidos);
            this.monedasTotales = monedasConseguidas;
            this.partidasPuzzle = partidas;
            this.puzzlesGanados = puzzles;

            this.parpadeando = false;
            this.estadoActual = Estado.FELIZ;
            this.estadoAnterior = Estado.DORMIDO;
            this.nuevoNivel = false;
        }

        public Avatar()
        {
            gestor = new GestorBD();
        }

        private Hashtable crearLogros()
        {

            logros = new Hashtable();
            logros.Add("1", new Logro(1, "Alcanza el nivel 3", false));
            logros.Add("2", new Logro(2, "Alcanza el nivel 6", false));
            logros.Add("3", new Logro(3, "Alcanza el nivel 10", false));
            logros.Add("4", new Logro(4, "Juega 5 veces al puzzle", false));
            logros.Add("5", new Logro(5, "Juega 10 veces al puzzle", false));
            logros.Add("6", new Logro(6, "Juega 20 veces al puzzle", false));
            logros.Add("7", new Logro(7, "Consigue 200 monedas totales", false));
            logros.Add("8", new Logro(8, "Consigue 500 monedas totales", false));
            logros.Add("9", new Logro(9, "Consigue 1000 monedas totales", false));
            logros.Add("10", new Logro(10, "Resuelve 5 puzzles", false));
            logros.Add("11", new Logro(11, "Resuelve 10 puzzles", false));
            logros.Add("12", new Logro(12, "Obtén todos los logros", false));

            return logros;
        }

        private void desbloquearLogros(String logrosConseguidos)
        {
            if (!logrosConseguidos.Equals(""))
            {
                char[] separador = { ' ' };
                string[] cadenasLogros = logrosConseguidos.Split(separador);
                for (int i = 0; i < cadenasLogros.Length; i++)
                {
                    Logro nuevoLogro = (Logro)logros[cadenasLogros[i]];
                    nuevoLogro.Desbloqueado = true;
                    logros[cadenasLogros[i]] = nuevoLogro;
                }
            }
        }

        public void sumarMonedas(int monedasGanadas)
        {

            monedasTotales += monedasGanadas;
            if (monedasTotales >= 200 && monedasTotales < 500)
            {

                if (!((Logro)logros["7"]).Desbloqueado)
                {
                    desbloquearNuevoLogro("7");

                }
            }
            else
            {
                if (monedasTotales >= 500 && monedasTotales < 1000)
                {
                    if (!((Logro)logros["8"]).Desbloqueado)
                    {
                        desbloquearNuevoLogro("8");

                    }
                }
                else
                {
                    if (!((Logro)logros["9"]).Desbloqueado && monedasTotales > 1000)
                    {
                        desbloquearNuevoLogro("9");

                        comprobarLogroOculto();
                    }
                }
            }
        }


       


        public void nuevoPuzzle()
        {
            partidasPuzzle++;
            switch (partidasPuzzle)
            {
                case 5:
                    if (!((Logro)logros["4"]).Desbloqueado)
                    {
                        desbloquearNuevoLogro("4");

                    }
                    break;
                case 10:
                    if (!((Logro)logros["5"]).Desbloqueado)
                    {
                        desbloquearNuevoLogro("5");

                    }
                    break;
                case 20:
                    if (!((Logro)logros["6"]).Desbloqueado)
                    {
                        desbloquearNuevoLogro("6");

                        comprobarLogroOculto();
                    }
                    break;
            }
        }

        private void desbloquearNuevoLogro(String id)
        {
            Logro nuevoLogro = (Logro)logros[id];
            nuevoLogro.Desbloqueado = true;
            logrosParaNotificar.Add(nuevoLogro);
            logros[id] = nuevoLogro;
        }


        public void puzzleGanado()
        {
            puzzlesGanados++;
            switch (puzzlesGanados)
            {
                case 5:
                    if (!((Logro)logros["10"]).Desbloqueado)
                    {
                        Logro nuevoLogro = (Logro)logros["10"];
                        nuevoLogro.Desbloqueado = true;
                        logrosParaNotificar.Add(nuevoLogro);
                        logros["10"] = nuevoLogro;
                    }
                    break;
                case 10:
                    if (!((Logro)logros["11"]).Desbloqueado)
                    {
                        Logro nuevoLogro = (Logro)logros["11"];
                        nuevoLogro.Desbloqueado = true;
                        logrosParaNotificar.Add(nuevoLogro);
                        logros["11"] = nuevoLogro;
                        comprobarLogroOculto();
                    }
                    break;
            }

        }


        private void comprobarLogroOculto()
        {
            int cont = 0;
            for (int i = 1; i < 13; i++)
            {
                if (((Logro)logros[i.ToString()]).Desbloqueado)
                {
                    cont++;
                }

            }
            if (cont == 11 && !((Logro)logros["12"]).Desbloqueado)
            {
                desbloquearNuevoLogro("12");
            }
        }





        public void proximoEstado(Estado nuevoEstado)
        {
            this.estadoAnterior = this.estadoActual;
            this.estadoActual = nuevoEstado;
        }


        public void establecerEstado()
        {
            this.estadoAnterior = this.estadoActual;
        }


        public Boolean estadoEstablecido()
        {
            Boolean establecido = false;

            if (this.estadoActual == this.estadoAnterior)
            {
                establecido= true;
            }          
                return establecido;
            
        }

        public int parpadear()
        {
            this.parpadeando = true;
            return 1;
        }


        //METODOS QUE SE COMUNICAN CON EL GESTORBD

        public Avatar leerUsuario(String usuario)
        {
            return this.gestor.leerAvatar(usuario);
        }


       
        public String autenticar(String usuario, String pass)
        {
            String msg = null;
            int resultado = this.gestor.autenticar(usuario, pass);
            if (resultado == 0)
            {
                msg = "0";
            }
            else
            {
                msg = "1";
            }

            return msg;
        }


    
        public void registrar(String usuario, String pass)
        {
            this.gestor.registrarUsuario(usuario, pass);
        }


      
        public void actualizar()
        {
            String logros = establecerLogros();
            this.gestor.actualizarAvatar(this,logros);
        }

        private String establecerLogros()
        {
            String logros = "";
            foreach (string key in this.Logros.Keys)
            {
                if (((Logro)this.Logros[key]).Desbloqueado)
                {
                    if (logros.Equals(""))
                    {
                        logros += ((Logro)this.Logros[key]).Id.ToString();
                    }
                    else
                    {
                        logros += " " + ((Logro)this.Logros[key]).Id.ToString();
                    }

                }
            }
            return logros;
        }

        public Boolean usuarioExistente(String usuario)
        {
            Boolean existente = this.gestor.comprobarUsuarioExistente(usuario);
            return existente;
        }

    }
}
