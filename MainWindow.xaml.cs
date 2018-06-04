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
using WpfApp1;

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
        private Avatar miAvatar;


        private void inicializar_avatar()
        {
            Boolean load = false;
            //((Storyboard)this.Resources["autenticacion_correcta"]).Begin();
            cont = 0;
            prox = 0;
            prox_logro = 0;
            prox_hambre = 0;
            cierre_auto_botones = -1;
            tiempo_comienzo = 0;
            tiempo_juego = -1;
            try
            {
                //Se lee de la base de datos los datos relativos al avatar que se ha autenticado
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

                lbl_nivel.Content = avatar.Nivel.ToString();
                lbl_puntos_nivel.Content = avatar.Puntos_nivel.ToString();
                lbl_monedas.Content = avatar.Monedas.ToString();
                pb_nivel.Maximum = 100 * avatar.Nivel;
                pb_nivel.Value = avatar.Puntos_nivel;
                pb_monedas.Value = avatar.Monedas;
                lbl_pb_apetito.Content = avatar.Apetito.ToString() + " %";
                lbl_pb_diversion.Content = avatar.Diversion.ToString() + " %";
                lbl_pb_energia.Content = avatar.Energia.ToString() + " %";

                //Se establecen los valores de las progress_bar en funcion de los valores del avatar 
                pb_alimentacion.Value = avatar.Apetito;
                pb_diversion.Value = avatar.Diversion;
                pb_energia.Value = avatar.Energia;

                //Inicilizamos las comidas
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


                //Se crea el modulo de twitter 
                twitter = new TwitterService("Ldt80izwntbF0YeGeJqjRAvFH", "f56SsNbK99P9lIvWtrjxsoAaJh24T0SlF6SYjy4nGmyomJOaNb");
                twitter.AuthenticateWith("702046118222372864-NmQOGATRDJnQDaSdmYfAGNnqDDLM0XA", "3Lthr2I49OVqlJlHULvMRocig3W56OP3xAeOOojtvt8Do");



                //Se crea el diccionario del reconocimiento de voz y se inicializa el mismo
                reconocimiento_voz = new SpeechRecognizer();
                Choices acciones = new Choices();
                acciones.Add(new string[] { "dormir", "despertar", "rapel", "yoyo", "logros", "puzzle", "ocultar", "ranking" });
                GrammarBuilder gb = new GrammarBuilder();
                gb.Append(acciones);
                Grammar g = new Grammar(gb);
                reconocimiento_voz.LoadGrammar(g);
                reconocimiento_voz.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(reconocimiento_voz_SpeechRecognized);

                //Inicializacion animaciones
                parpadear = (Storyboard)this.Resources["parpadear"];
                animacion_dormir = (Storyboard)this.Resources["animacion_dormir"];



                //Empieza el Timer principal
                timer_global = new DispatcherTimer();
                timer_global.Interval = TimeSpan.FromMilliseconds(intervalo_global);
                timer_global.Tick += new EventHandler(reloj);
                timer_global.Start();

                try
                {
                    twitter.SendTweet(
                        new SendTweetOptions
                        {
                            Status = "@" + avatar.Usuario.ToString() + " está ahora mismo jugando a BatGotchi!!\n\nJuega tu también!"
                        }
                    );
                }
                catch (Exception) { };
            }

        }


        public MainWindow()
        {
            InitializeComponent();
            miAvatar = new Avatar(100, 100, 100);
            intervalo = 9000;
            temporizador = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(intervalo)
            };
            temporizador.Tick += tickConsumoHandler;
            temporizador.Start();

        }


        private void tickConsumoHandler(object sender, EventArgs e)
        {

            int aux;
            miAvatar.Apetito = (aux = miAvatar.Apetito - aleatorizar(20)) < 0 ? 0 : aux;
            miAvatar.Diversion = (aux = miAvatar.Diversion - aleatorizar(15)) < 0 ? 0 : aux;
            miAvatar.Energia = (aux = miAvatar.Energia - aleatorizar(10)) < 0 ? 0 : aux;
            this.PBapetito.Value = miAvatar.Apetito;
            this.PBdiversion.Value = miAvatar.Diversion;
            this.PBenergia.Value = miAvatar.Energia;
            this.energyLevel.Content = this.PBenergia.Value + "%";
            this.funnyLevel.Content = this.PBdiversion.Value + "%";
            this.hungryLevel.Content = this.PBapetito.Value + "%";
            controlCansado();
            controlHambre();
        }

        private void controlHambre()
        {
            if (miAvatar.Apetito <= 25)
            {

                Storyboard sbHambre = (Storyboard)this.FindResource("animHambreKey");
                sbHambre.Begin();
            }

        }

        private void controlCansado()
        {
            if (miAvatar.Energia <= 25) { 

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
            if (miAvatar.Energia < 100)
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
                miAvatar.Energia += aleatorizar(10);
                this.PBenergia.Value = miAvatar.Energia;
                this.energyLevel.Content = this.PBenergia.Value + "%";
            }
        }

        private void BApetito_Click(object sender, RoutedEventArgs e)
        {
            if (miAvatar.Apetito < 100)
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
                miAvatar.Apetito += aleatorizar(10);
                this.PBapetito.Value = miAvatar.Apetito;
                this.hungryLevel.Content = this.PBapetito.Value + "%";
            }
        }

        private void BDiversion_Click(object sender, RoutedEventArgs e)
        {
            if (miAvatar.Diversion < 100)
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
                miAvatar.Diversion += aleatorizar(10);
                this.PBdiversion.Value = miAvatar.Diversion;
                this.funnyLevel.Content = this.PBdiversion.Value + "%";
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
                batman.IsEnabled = false;
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
    }
}
