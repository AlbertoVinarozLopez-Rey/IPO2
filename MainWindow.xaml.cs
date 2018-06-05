using MiAppVenom.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MiAppVenom
{

    public enum Animaciones_jugar
    {
        YOYO,
        TELARAÑA,
        NINGUNA
    }

    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer temporizador;
        int intervalo;

        //Alimentos
        private static Comida o_sandia;
        private static Comida o_banana;
        private static Comida o_lata;
        private static Comida o_coockie;
        private static Comida o_patatas;
        private static Comida o_cafe;
        private static Comida o_galleta;
        private static Comida o_pizza;
        private static Comida o_cocktail;
        private static Comida o_hamburguesa;

        //Animaciones
        private static Storyboard parpadear;
        private static Storyboard animacion_dormir;

        //Acciones
        private static Animaciones_jugar jugar;
        private static Boolean despertar;

        //Estado Intefaces
        private static Boolean estado_barra;
        private static Boolean botones;
        private static Boolean estado_logros;
        private static Boolean estado_puzzle;
        private static Boolean estado_ranking;
        private static Boolean jugando_puzzle;
        private static Boolean puzzle_completado;
        private static Boolean label_fin;
        private static Boolean fin_juego;

        //Timers
        private static DispatcherTimer timer_global;
        private static double intervalo_global = 1000.0;
        private static int cont, prox, cierre_auto_botones, prox_logro, prox_hambre;
        private static int tiempo_juego;
        private static int tiempo_comienzo;


        //Controlador de Twiiter
        //private TinyTwitter twitter;
        //private TwitterService twitter;

        //Motor de reconocimienot de voz
        //private static SpeechRecognizer reconocimiento_voz;

        //Imagenes
        private Object pieza_cogida;
        private Image pieza_puzzle;

        //Avatar
        private Avatar avatar;


        public MainWindow()
        {

            InitializeComponent();
            ((Storyboard)this.Resources["cerrarBarra"]).Begin();
            btn_puzzle.IsEnabled = false;
            btn_trofeo.IsEnabled = false;
            btn_ranking.IsEnabled = false;
            cvVenom.IsEnabled = false;

            /*InitializeComponent();
            miAvatar = new Avatar(100, 100, 100);
            intervalo = 9000;
            temporizador = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(intervalo)
            };
            temporizador.Tick += tickConsumoHandler;
            temporizador.Start();
            */
        }

        private void inicializar_avatar()
        {
            Boolean load = false;
            ((Storyboard)this.Resources["ocultarAutenticacion"]).Begin();
            cont = 0;
            prox = 0;
            prox_logro = 0;
            prox_hambre = 0;
            cierre_auto_botones = -1;
            tiempo_comienzo = 0;
            tiempo_juego = -1;
            try
            {
                avatar = (new Avatar()).LeerUsuario(tb_inicio_usuario.Text.ToString());
                load = true;
            }
            catch (Exception) { }

            if (load)
            {
                desbloquear_logros();
                lbl_Usuario.Content = avatar.Usuario;
                despertar = false;
                pieza_cogida = null;
                pieza_puzzle = new Image();
                pieza_puzzle.Source = pieza9.Source;

                lblNivel.Content = avatar.Nivel.ToString();
                lbl_puntos_nivel.Content = avatar.Puntos_nivel.ToString();
                lbl_monedas.Content = avatar.Monedas.ToString();
                pb_nivel.Maximum = 100 * avatar.Nivel;
                pb_nivel.Value = avatar.Puntos_nivel;
                pb_monedas.Value = avatar.Monedas;
                lblApetito.Content = avatar.Apetito.ToString() + " %";
                lblDiversion.Content = avatar.Diversion.ToString() + " %";
                lblEnergia.Content = avatar.Energia.ToString() + " %";

                pbApetito.Value = avatar.Apetito;
                pbDiversion.Value = avatar.Diversion;
                pbEnergia.Value = avatar.Energia;

                o_sandia = new Comida("sandia", 5, 3, 5, 3);
                o_banana = new Comida("banana", 6, 2, 6, 5);
                o_lata = new Comida("lata", 7, 5, 2, 4);
                o_coockie = new Comida("coockie", 3, 1, 3, 7);
                o_cafe = new Comida("cafe", 6, 20, 2, 8);
                o_patatas = new Comida("patatas", 7, 2, 20, 5);
                o_cocktail = new Comida("cocktail", 10, 5, 7, 15);
                o_galleta = new Comida("galleta", 12, 3, 15, 18);
                o_pizza = new Comida("pizza", 15, 6, 20, 20);
                o_hamburguesa = new Comida("hamburguesa", 20, 4, 25, 20);

                for (int i = 2; i <= avatar.Nivel; i++)
                {
                    desbloquear_objetos(i);
                }



                //Se inicializan las comidas
                estado_barra = false;
                estado_logros = false;
                estado_puzzle = false;
                label_fin = false;
                fin_juego = false;
                botones = false;
                jugando_puzzle = false;
                puzzle_completado = false;
                lbl_flecha_right.IsEnabled = false;
                lbl_flecha_left.IsEnabled = true;


                /*//Se crea el modulo de twitter 
                twitter = new TwitterService("Ldt80izwntbF0YeGeJqjRAvFH", "f56SsNbK99P9lIvWtrjxsoAaJh24T0SlF6SYjy4nGmyomJOaNb");
                twitter.AuthenticateWith("702046118222372864-NmQOGATRDJnQDaSdmYfAGNnqDDLM0XA", "3Lthr2I49OVqlJlHULvMRocig3W56OP3xAeOOojtvt8Do");
                */


               /* //Se crea el diccionario del reconocimiento de voz y se inicializa el mismo
                reconocimiento_voz = new SpeechRecognizer();
                Choices acciones = new Choices();
                acciones.Add(new string[] { "dormir", "despertar", "rapel", "yoyo", "logros", "puzzle", "ocultar", "ranking" });
                GrammarBuilder gb = new GrammarBuilder();
                gb.Append(acciones);
                Grammar g = new Grammar(gb);
                reconocimiento_voz.LoadGrammar(g);
                reconocimiento_voz.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(reconocimiento_voz_SpeechRecognized);
                */

                //Inicializacion animaciones
                parpadear = (Storyboard)this.Resources["parpadear"];
                animacion_dormir = (Storyboard)this.Resources["animacion_dormir"];



                //Empieza el Timer principal
                timer_global = new DispatcherTimer();
                timer_global.Interval = TimeSpan.FromMilliseconds(intervalo_global);
                timer_global.Tick += new EventHandler(reloj);
                timer_global.Start();

                /*try
                {
                    twitter.SendTweet(
                        new SendTweetOptions
                        {
                            Status = "@" + avatar.Usuario.ToString() + " está ahora mismo jugando a BatGotchi!!\n\nJuega tu también!"
                        }
                    );
                }
                catch (Exception) { };*/
            }

        }

        private void desbloquear_logros()
        {
            String logros = "";
            foreach (string key in avatar.Logros.Keys)
            {
                if (((Logro)avatar.Logros[key]).Conseguido)
                {
                    if (logros.Equals(""))
                    {
                        logros += ((Logro)avatar.Logros[key]).Id.ToString();
                    }
                    else
                    {
                        logros += " " + ((Logro)avatar.Logros[key]).Id.ToString();
                    }

                }
            }

            if (!logros.Equals(""))
            {
                char[] delimiterChars = { ' ' };
                string[] words = logros.Split(delimiterChars);
                for (int i = 0; i < words.Length; i++)
                {
                    desbloquear_logro(int.Parse(words[i]));
                }
            }
        }

        private void desbloquear_logro(int id)
        {
            switch (id)
            {
                case 1:
                    logro_lv3.Opacity = 100;
                    tb_logro_lv3.Opacity = 100;
                    break;
                case 2:
                    logro_lv6.Opacity = 100;
                    tb_logro_lv6.Opacity = 100;
                    break;
                case 3:
                    logro_lv10.Opacity = 100;
                    tb_logro_lv10.Opacity = 100;
                    break;
                case 4:
                    logro_pl5.Opacity = 100;
                    tb_logro_pl5.Opacity = 100;
                    break;
                case 5:
                    logro_pl10.Opacity = 100;
                    tb_logro_pl10.Opacity = 100;
                    break;
                case 6:
                    logro_pl20.Opacity = 100;
                    tb_logro_pl20.Opacity = 100;
                    break;
                case 7:
                    logro_mon200.Opacity = 100;
                    tb_logro_mon200.Opacity = 100;
                    break;
                case 8:
                    logro_mon500.Opacity = 100;
                    tb_logro_mon500.Opacity = 100;
                    break;
                case 9:
                    logro_mon1000.Opacity = 100;
                    tb_logro_mon1000.Opacity = 100;
                    break;
                case 10:
                    logro_pz5.Opacity = 100;
                    tb_logro_pz5.Opacity = 100;
                    break;
                case 11:
                    logro_pz10.Opacity = 100;
                    tb_logro_pz10.Opacity = 100;
                    break;
                case 12:
                    logro_todos.Opacity = 100;
                    tb_logro_todos.Opacity = 100;
                    break;
            }
        }


        private void desbloquear_objetos(int nivel)
        {
            switch (nivel)
            {
                case 2:
                    lbl_lv2_1.Visibility = Visibility.Hidden;
                    lbl_lv2_2.Visibility = Visibility.Hidden;
                    lbl_lv2_3.Visibility = Visibility.Hidden;
                    coockie.IsEnabled = true;
                    patatas.IsEnabled = true;
                    cafe.IsEnabled = true;
                    coockie.Opacity = 100;
                    patatas.Opacity = 100;
                    cafe.Opacity = 100;
                    break;
                case 3:
                    lbl_lv3_1.Visibility = Visibility.Hidden;
                    lbl_lv3_2.Visibility = Visibility.Hidden;
                    lbl_lv3_3.Visibility = Visibility.Hidden;
                    lbl_lv3_4.Visibility = Visibility.Hidden;
                    galletas.IsEnabled = true;
                    hamburguesa.IsEnabled = true;
                    pizza.IsEnabled = true;
                    cocktail.IsEnabled = true;
                    cocktail.Opacity = 100;
                    pizza.Opacity = 100;
                    hamburguesa.Opacity = 100;
                    galletas.Opacity = 100;
                    break;
                default:
                    break;
            }
        }




        private void tickConsumoHandler(object sender, EventArgs e)
        {

            int aux;
            avatar.Apetito = (aux = avatar.Apetito - aleatorizar(20)) < 0 ? 0 : aux;
            avatar.Diversion = (aux = avatar.Diversion - aleatorizar(15)) < 0 ? 0 : aux;
            avatar.Energia = (aux = avatar.Energia - aleatorizar(10)) < 0 ? 0 : aux;
            this.pbApetito.Value = avatar.Apetito;
            this.pbDiversion.Value = avatar.Diversion;
            this.pbEnergia.Value = avatar.Energia;
            this.lblEnergia.Content = this.pbEnergia.Value + "%";
            this.lblDiversion.Content = this.pbDiversion.Value + "%";
            this.lblApetito.Content = this.pbApetito.Value + "%";
            controlCansado();
            controlHambre();
        }

        private void controlHambre()
        {
            if (avatar.Apetito <= 25)
            {

                Storyboard sbHambre = (Storyboard)this.FindResource("animHambreKey");
                sbHambre.Begin();
            }

        }

        private void controlCansado()
        {
            if (avatar.Energia <= 25) { 

            Storyboard sbCansado = (Storyboard)this.FindResource("animCansadoKey");
            sbCansado.Begin();
        }
            
        }

        private int aleatorizar(int max)
        {
            Random generadorAleat = new Random();
            return 1 + generadorAleat.Next(max);
        }


        private void BEnergia_Click(object sender, RoutedEventArgs e)
        {
            if (avatar.Energia < 100)
            {
                intervalo -= 20;
                if (intervalo > 0)
                {
                    temporizador.Interval = TimeSpan.FromMilliseconds(intervalo);
                }
                else
                {
                    temporizador.Interval = TimeSpan.FromMilliseconds(5);
                }
                avatar.Energia += aleatorizar(10);
                this.pbEnergia.Value = avatar.Energia;
                this.lblEnergia.Content = this.pbEnergia.Value + "%";
            }
        }

        private void BApetito_Click(object sender, RoutedEventArgs e)
        {
            if (avatar.Apetito < 100)
            {
                intervalo -= 20;
                if (intervalo > 0)
                {
                    temporizador.Interval = TimeSpan.FromMilliseconds(intervalo);
                }
                else
                {
                    temporizador.Interval = TimeSpan.FromMilliseconds(5);
                }
                avatar.Apetito += aleatorizar(10);
                this.pbApetito.Value = avatar.Apetito;
                this.lblApetito.Content = this.pbApetito.Value + "%";
            }
        }

        private void BDiversion_Click(object sender, RoutedEventArgs e)
        {
            if (avatar.Diversion < 100)
            {
                intervalo -= 20;
                if (intervalo > 0)
                {
                    temporizador.Interval = TimeSpan.FromMilliseconds(intervalo);
                }
                else
                {
                    temporizador.Interval = TimeSpan.FromMilliseconds(5);
                }
                avatar.Diversion += aleatorizar(10);
                this.pbDiversion.Value = avatar.Diversion;
                this.lblDiversion.Content = this.pbDiversion.Value + "%";
            }
        }
        private void dormir(object sender, RoutedEventArgs e)
        {

        }

        private void abrirPuzzle(object sender, RoutedEventArgs e)
        {
            if (!estado_puzzle && cont >= prox)
            {
                if (estado_barra)
                {
                    ((Storyboard)this.Resources["cerrar_barra_no_mover"]).Begin();
                    lbl_flecha_right.IsEnabled = false;
                    estado_barra = false;
                }
                restablecer_puzzle_metodo();
                btn_trofeo.IsEnabled = false;
                btn_ranking.IsEnabled = false;
                lbl_flecha_left.IsEnabled = false;
                ((Storyboard)this.Resources["abrir_puzzle"]).Begin();
                ((Storyboard)this.Resources["empezar_juego"]).Begin();
                border_puzzle.IsEnabled = false;
                cvVenom.IsEnabled = false;
                estado_puzzle = true;
            }
        }

        private void restablecer_puzzle_metodo()
        {
            pz1_1.Visibility = Visibility.Visible;
            pz1_2.Visibility = Visibility.Visible;
            pz1_3.Visibility = Visibility.Visible;
            pz1_4.Visibility = Visibility.Visible;
            pz1_5.Visibility = Visibility.Visible;
            pz1_6.Visibility = Visibility.Visible;
            pz1_7.Visibility = Visibility.Visible;
            pz1_8.Visibility = Visibility.Visible;
            pz1_9.Visibility = Visibility.Visible;

            pieza1.Source = pieza_puzzle.Source;
            pieza1.Stretch = Stretch.Uniform;
            pieza2.Source = pieza_puzzle.Source;
            pieza2.Stretch = Stretch.Uniform;
            pieza3.Source = pieza_puzzle.Source;
            pieza3.Stretch = Stretch.Uniform;
            pieza4.Source = pieza_puzzle.Source;
            pieza4.Stretch = Stretch.Uniform;
            pieza5.Source = pieza_puzzle.Source;
            pieza5.Stretch = Stretch.Uniform;
            pieza6.Source = pieza_puzzle.Source;
            pieza6.Stretch = Stretch.Uniform;
            pieza7.Source = pieza_puzzle.Source;
            pieza7.Stretch = Stretch.Uniform;
            pieza8.Source = pieza_puzzle.Source;
            pieza8.Stretch = Stretch.Uniform;
            pieza9.Source = pieza_puzzle.Source;
            pieza9.Stretch = Stretch.Uniform;
        }

        private void autenticarse(object sender, RoutedEventArgs e)
        {
            try
            {
                String con = (new Avatar()).autenticar(tb_inicio_usuario.Text.ToString(), tb_inicio_contrasena.Password.ToString());
                switch (con)
                {
                    case "0":
                        lbl_msg_inicio.Content = "Usuario correcto";
                        btn_puzzle.IsEnabled = true;
                        btn_trofeo.IsEnabled = true;
                        btn_ranking.IsEnabled = true;
                        cvVenom.IsEnabled = true;
                        inicializar_avatar();
                        break;
                    case "1":
                        lbl_msg_inicio.Content = "Credenciales incorrectos";
                        break;
                }
            }
            catch (Exception)
            {
                lbl_msg_inicio.Content = "Error al conectar con la base de datos";
            }
        }
    }
}
