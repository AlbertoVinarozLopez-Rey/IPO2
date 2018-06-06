using MiAppVenom.Dominio;
using MiAppVenom.Persistencia;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiAppVenom
{
    public enum Estado
    {
        FELIZ,
        ENFADADO,
        DORMIDO,
        DORMIDO_PROFUNDAMENTE,
        HAMBRIENTO,
        ABURRIDO,
        JUGANDO
    }

    public class Avatar
    {
        private Hashtable logros;
        public Hashtable Logros
        {
            get
            {
                return this.logros;
            }
      
        }
        public List<Logro> logros_notificar;

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

        private int puntos;
        public int Puntos_nivel
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
                                if (!((Logro)logros["1"]).Conseguido)
                                {
                                    Logro logro1 = (Logro)logros["1"];
                                    logro1.Conseguido = true;
                                    logros_notificar.Add(logro1);
                                    logros["1"] = logro1;
                                }
                                break;

                            case 6:
                                if (!((Logro)logros["2"]).Conseguido)
                                {
                                    Logro logro2 = (Logro)logros["2"];
                                    logro2.Conseguido = true;
                                    logros_notificar.Add(logro2);
                                    logros["2"] = logro2;
                                }
                                break;

                            case 10:
                                if (!((Logro)logros["3"]).Conseguido)
                                {
                                    Logro logro3 = (Logro)logros["3"];
                                    logro3.Conseguido = true;
                                    logros_notificar.Add(logro3);
                                    logros["3"] = logro3;
                                    comprobar_logros();
                                }
                                break;
                        }
                        nuevo_nivel = true;
                    }
                    else
                    {
                        puntos = value;
                    }
                }
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

        private Boolean nuevo_nivel;
        public Boolean Nuevo_nivel
        {
            get
            {
                return nuevo_nivel;
            }
            set
            {
                nuevo_nivel = value;
            }
        }

        public Boolean parpadeando;
        public Estado estado_actual;
        public Estado estado_anterior;

        private int monedas_conseguidas;
        public int Monedas_conseguidas
        {
            get
            {
                return this.monedas_conseguidas;
            }
            set
            {
                monedas_conseguidas = value;
            }
        }
        private int partidas_jugadas;
        public int Partidas_jugadas
        {
            get
            {
                return this.partidas_jugadas;
            }
            set
            {
                partidas_jugadas = value;
            }
        }
        private int puzzles_resueltos;
        public int Puzzles_resueltos
        {
            get
            {
                return this.puzzles_resueltos;
            }
            set
            {
               puzzles_resueltos = value;
            }
        }
        private GestorBD gestor;
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

        public Avatar(String usuario, int nivel, int puntos, int monedas, int apetito, int energia, int diversion, String logros, int monedasConseguidas, int partidas, int puzzles)
        {
            gestor = new GestorBD();
            this.usuario = usuario;
            logros_notificar = new List<Logro>();
            crear_logros();
            procesar_logros_conseguidos(logros);
            this.nivel = nivel;
            this.puntos = puntos;
            this.monedas = monedas;
            this.energia = energia;
            this.apetito = apetito;
            this.diversion = diversion;
            this.parpadeando = false;
            this.monedas_conseguidas = monedasConseguidas;
            this.partidas_jugadas = partidas;
            this.puzzles_resueltos = puzzles;
            this.estado_actual = Estado.FELIZ;
            this.estado_anterior = Estado.DORMIDO;
            this.nuevo_nivel = false;
        }


        public Avatar(String usuario, int nivel)
        {
            this.usuario = usuario;
            this.nivel = nivel;
            gestor = new GestorBD();
        }

        public Avatar()
        {
            gestor = new GestorBD();
        }

        private void crear_logros()
        {
            logros = new Hashtable();
            logros.Add("1", new Logro(1, "Alcanza el nivel 3", false));
            logros.Add("2", new Logro(2, "Alcanza el nivel 6", false));
            logros.Add("3", new Logro(3, "Alcanza el nivel 10", false));
            logros.Add("4", new Logro(4, "Juega 5 veces", false));
            logros.Add("5", new Logro(5, "Juega 10 veces", false));
            logros.Add("6", new Logro(6, "Juega 20 veces", false));
            logros.Add("7", new Logro(7, "Consigue 200 monedas", false));
            logros.Add("8", new Logro(8, "Consigue 500 monedas", false));
            logros.Add("9", new Logro(9, "Consigue 1000 monedas", false));
            logros.Add("10", new Logro(10, "Resuelve 5 puzzles", false));
            logros.Add("11", new Logro(11, "Resuelve 10 puzzles", false));
            logros.Add("12", new Logro(12, "Obtén todos los logros", false));
        }

        public void anadir_monedas(int x)
        {

            monedas_conseguidas += x;
            if (monedas_conseguidas >= 200 && monedas_conseguidas < 500)
            {

                if (!((Logro)logros["7"]).Conseguido)
                {
                    Logro logro7 = (Logro)logros["7"];
                    logro7.Conseguido = true;
                    logros_notificar.Add(logro7);
                    logros["7"] = logro7;
                }
            }
            else
            {
                if (monedas_conseguidas >= 500 && monedas_conseguidas < 1000)
                {
                    if (!((Logro)logros["8"]).Conseguido)
                    {
                        Logro logro8 = (Logro)logros["8"];
                        logro8.Conseguido = true;
                        logros_notificar.Add(logro8);
                        logros["8"] = logro8;
                    }
                }
                else
                {
                    if (!((Logro)logros["9"]).Conseguido && monedas_conseguidas > 1000)
                    {
                        Logro logro9 = (Logro)logros["9"];
                        logro9.Conseguido = true;
                        logros_notificar.Add(logro9);
                        logros["9"] = logro9;
                        comprobar_logros();
                    }
                }
            }
        }


        private void procesar_logros_conseguidos(String log_obtenidos)
        {
            if (!log_obtenidos.Equals(""))
            {
                char[] delimiterChars = { ' ' };
                string[] words = log_obtenidos.Split(delimiterChars);
                for (int i = 0; i < words.Length; i++)
                {
                    Logro achievement = (Logro)logros[words[i]];
                    achievement.Conseguido = true;
                    logros[words[i]] = achievement;
                }
            }
        }


        public void nueva_partida()
        {
            partidas_jugadas++;
            switch (partidas_jugadas)
            {
                case 5:
                    if (!((Logro)logros["4"]).Conseguido)
                    {
                        Logro logro4 = (Logro)logros["4"];
                        logro4.Conseguido = true;
                        logros_notificar.Add(logro4);
                        logros["4"] = logro4;
                    }
                    break;
                case 10:
                    if (!((Logro)logros["5"]).Conseguido)
                    {
                        Logro logro5 = (Logro)logros["5"];
                        logro5.Conseguido = true;
                        logros_notificar.Add(logro5);
                        logros["5"] = logro5;
                    }
                    break;
                case 20:
                    if (!((Logro)logros["6"]).Conseguido)
                    {
                        Logro logro6 = (Logro)logros["6"];
                        logro6.Conseguido = true;
                        logros_notificar.Add(logro6);
                        logros["6"] = logro6;
                        comprobar_logros();
                    }
                    break;
            }
        }

        public void puzzle_resuelto()
        {
            puzzles_resueltos++;
            switch (puzzles_resueltos)
            {
                case 5:
                    if (!((Logro)logros["10"]).Conseguido)
                    {
                        Logro logro10 = (Logro)logros["10"];
                        logro10.Conseguido = true;
                        logros_notificar.Add(logro10);
                        logros["10"] = logro10;
                    }
                    break;
                case 10:
                    if (!((Logro)logros["11"]).Conseguido)
                    {
                        Logro logro11 = (Logro)logros["11"];
                        logro11.Conseguido = true;
                        logros_notificar.Add(logro11);
                        logros["11"] = logro11;
                        comprobar_logros();
                    }
                    break;
            }
        }


        private void comprobar_logros()
        {
            int cont = 0;
            for (int i = 1; i < 13; i++)
            {
                if (((Logro)logros[i.ToString()]).Conseguido)
                {
                    cont++;
                }
                if (cont == 11 && !((Logro)logros["12"]).Conseguido)
                {
                    Logro logro12 = (Logro)logros["12"];
                    logro12.Conseguido = true;
                    logros_notificar.Add(logro12);
                    logros["12"] = logro12;
                }
            }
        }


        /*********************************************************************
        *
        * Method name: proximo_estado
        *
        * Description of the Method: Establece el estado actual del avatar y modifica el estado anterior del avatar.
        *
        * Calling arguments: Estado : e
        *
        * Return value: none
        *
        *********************************************************************/
        public void proximo_estado(Estado e)
        {
            this.estado_anterior = this.estado_actual;
            this.estado_actual = e;
        }


        /*********************************************************************
        *
        * Method name: estado_establecido
        *
        * Description of the Method: comprueba si se ha establecido el estado (que el estado anterior y actual sea el mismo)
        *
        * Calling arguments: none
        *
        * Return value: Boolean 
        *
        *********************************************************************/
        public Boolean estado_establecido()
        {
            if (this.estado_actual == this.estado_anterior)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /*********************************************************************
        *
        * Method name: establecer_estado
        *
        * Description of the Method: Pone el estado anterior igual que el actual, para asi establecer un estado
        *
        * Calling arguments: none
        *
        * Return value: none
        *
        *********************************************************************/
        public void establecer_estado()
        {
            this.estado_anterior = this.estado_actual;
        }


        /*********************************************************************
        *
        * Method name: parpadear
        *
        * Description of the Method: registra que el avatar esta parpadeando y devuelve 1 (el minimo), que es el tiempo que se ha de esperar hasta
        *                            la proxima animacion
        *
        * Calling arguments: none
        *
        * Return value: int : 1
        *
        *********************************************************************/
        public int parpadear()
        {
            this.parpadeando = true;
            return 1;
        }


        /*********************************************************************
        *
        * Method name: LeerUsuario
        *
        * Description of the Method: Lee de la base de datos el usuario que se le pasa como argumento a dicho metodo y devuelve el avatar correspondiente a dicho usuario
        *
        * Calling arguments: String : user
        *
        * Return value: Avatar
        *
        *********************************************************************/
        public Avatar LeerUsuario(String user)
        {
            return this.gestor.leerAvatar(user);
        }


        /*********************************************************************
        *
        * Method name: autenticar
        *
        * Description of the Method: Comprueba con la base de datos la contrasena del usuario y devuelve un mensaje dependiendo si es 
        *                            correcta, incorrecta o el usuario es erroneo.
        *
        * Calling arguments: String : user, String : contra
        *
        * Return value: String : msg
        *
        *********************************************************************/
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


        /*********************************************************************
        *
        * Method name: registrar
        *
        * Description of the Method: Guarda en la base de datos el usuario y contrasena del nuevo usuario. Solo se han de indicar estos
        *                            parametros ya que los demas son los mismos, por defecto, para todos los usuarios.
        *
        * Calling arguments: String : user, String : contra
        *
        * Return value: String 
        *
        *********************************************************************/
        public void registrar(String user, String contra)
        {
            this.gestor.registrarUsuario(user, contra);
        }


        /*********************************************************************
        *
        * Method name: actualizar
        *
        * Description of the Method: Actualiza todos los datos de la base de datos correspondientes a este avatar con los datos actuales.
        *
        * Calling arguments: none
        *
        * Return value: none
        *
        *********************************************************************/
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
                if (((Logro)this.Logros[key]).Conseguido)
                {
                    if (logros.Equals(""))
                    {
                        logros += ((Logro)this.Logros[key]).Id.ToString();
                    }
                    else
                    {
                        logros += "%20" + ((Logro)this.Logros[key]).Id.ToString();
                    }

                }
            }
            return logros;
        }

        /*********************************************************************
        *
        * Method name: get_mejores_jugadores
        *
        * Description of the Method: Devuelve una lista con los mejores jugadores del juego
        *
        * Calling arguments: none
        *
        * Return value: List<Avatar>
        *
        *********************************************************************/
        //     public List<Avatar> get_mejores_jugadores()
        //   {
        //     return this.gestor.get_mejores_jugadores();
        // }


    }
}
