using MiAppVenom.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
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

    public enum Juegos
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
        
        //Alimentos
        
        private static Comida com_platano;
        private static Comida com_chips;
        private static Comida com_cafe;
        private static Comida com_coockies;
        private static Comida com_sandia;
        private static Comida com_pizza;
        private static Comida com_vino;
        private static Comida com_refresco;
        private static Comida com_galleta;
        private static Comida com_burguer;
        

        //Animaciones
        private static Storyboard animParpadear;
        private static Storyboard animDormir;

        //Acciones
        private static Juegos jugar;
        private static Boolean despertar;

        //Estado Intefaces
        private static Boolean estado_barra;
        private static Boolean botones;
        private static Boolean estado_logros;
        private static Boolean estado_puzzle;
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
        public static String usuario;



        public MainWindow()
        {

            InitializeComponent();
            ((Storyboard)this.Resources["cerrarBarra"]).Begin();
            btn_puzzle.IsEnabled = false;
            btn_trofeo.IsEnabled = false;
            cvVenom.IsEnabled = false;

          
        }

        private void inicializar_avatar()
        {
            Boolean load = false;
            cont = 0;
            prox = 0;
            prox_logro = 0;
            prox_hambre = 0;
            cierre_auto_botones = -1;
            tiempo_comienzo = 0;
            tiempo_juego = -1;
            try
            {
                avatar = (new Avatar()).LeerUsuario(usuario);
                load = true;
            }
            catch (Exception e1) {
                e1.ToString();
            }

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

                com_sandia = new Comida("sandia", 5, 3, 5, 3);
                com_platano = new Comida("banana", 6, 2, 6, 5);
                com_refresco = new Comida("lata", 7, 5, 2, 4);
                com_galleta = new Comida("coockie", 3, 1, 3, 7);
                com_cafe = new Comida("cafe", 6, 20, 2, 8);
                com_chips = new Comida("patatas", 7, 2, 20, 5);
                com_vino = new Comida("cocktail", 10, 5, 7, 15);
                com_coockies = new Comida("galleta", 12, 3, 15, 18);
                com_pizza = new Comida("pizza", 15, 6, 20, 20);
                com_burguer = new Comida("hamburguesa", 20, 4, 25, 20);
                
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
                animParpadear = (Storyboard)this.Resources["animParpadear"];
                animDormir = (Storyboard)this.Resources["animSoñar"];



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
                    break;
                case 2:
                    logro_lv6.Opacity = 100;
                    break;
                case 3:
                    logro_lv10.Opacity = 100;
                    break;
                case 4:
                    logro_pl5.Opacity = 100;
                    break;
                case 5:
                    logro_pl10.Opacity = 100;
                    break;
                case 6:
                    logro_pl20.Opacity = 100;
                    break;
                case 7:
                    logro_mon200.Opacity = 100;
                    break;
                case 8:
                    logro_mon500.Opacity = 100;
                    break;
                case 9:
                    logro_mon1000.Opacity = 100;
                    break;
                case 10:
                    logro_pz5.Opacity = 100;
                    break;
                case 11:
                    logro_pz10.Opacity = 100;
                    break;
                case 12:
                    logro_todos.Opacity = 100;
                    break;
            }
        }

        private void reloj(object sender, EventArgs e)
        {
            if (cont >= prox_logro && avatar.logros_notificar.Count > 0)
            {
                prox_logro = proximo_logro();
            }
            if (cont == cierre_auto_botones + 4 && botones)
            {
                if (avatar.estado_actual == Estado.DORMIDO || avatar.estado_actual == Estado.DORMIDO_PROFUNDAMENTE)
                {
                    ((Storyboard)this.Resources["esconderDormir"]).Begin();

                }
                else
                {
                    ((Storyboard)this.Resources["ocultarBotones"]).Begin();
                }
                botones = false;
            }
            if (cont >= prox)
            {
                if (jugando_puzzle)
                {
                    prox += comportamiento_juego_puzzle();
                }
                else
                {
                    if (avatar.Nuevo_nivel)
                    {
                        lbl_subir_nivel.Content = avatar.Nivel.ToString();
                        pb_nivel.Maximum = 100 * avatar.Nivel;
                        avatar.Nuevo_nivel = false;
                        ((Storyboard)this.Resources["subirNivel"]).Begin();
                       /* try
                        {
                            twitter.SendTweet(
                                new SendTweetOptions
                                {
                                    Status = "¡Enhorabuena @" + avatar.Usuario.ToString() + " ! Has alcanzado el nivel " + avatar.Nivel.ToString() + ".\nSigue jugando para subir de nivel!!"
                                }
                            );
                        }
                        catch (Exception) { }*/
                        prox += 3;
                        desbloquear_objetos(avatar.Nivel);
                    }
                    else
                    {
                        prox += proxima_animacion();
                    }
                }

            }

            consumir_valores();

            pbDiversion.Value = avatar.Diversion;
            pbApetito.Value = avatar.Apetito;
            pbEnergia.Value = avatar.Energia;

            lblNivel.Content = avatar.Nivel.ToString();
            lbl_puntos_nivel.Content = avatar.Puntos_nivel.ToString();
            lbl_monedas.Content = avatar.Monedas.ToString();

            pb_monedas.Value = avatar.Monedas;
            pb_nivel.Maximum = avatar.Nivel * 100;
            pb_nivel.Value = avatar.Puntos_nivel;

            lblApetito.Content = avatar.Apetito.ToString() + " %";
            lblDiversion.Content = avatar.Diversion.ToString() + " %";
            lblEnergia.Content = avatar.Energia.ToString() + " %";

            if (cont % 120 == 0)
            {
                avatar.actualizar();
            }

            cont++;
        }

        private void MouseUp_label_right(object sender, MouseButtonEventArgs e)
        {
            ((Storyboard)this.Resources["cerrarBarra"]).Begin();
            lbl_flecha_right.IsEnabled = false;
            lbl_flecha_left.IsEnabled = true;
            estado_barra = false;
        }

        private void MouseUp_label_left(object sender, MouseButtonEventArgs e)
        {
            ((Storyboard)this.Resources["abrirBarra"]).Begin();
            lbl_flecha_left.IsEnabled = false;
            lbl_flecha_right.IsEnabled = true;
            estado_barra = true;
        }


        private void consumir_valores()
        {
            Estado x;
            if (avatar.estado_establecido())
            {
                x = avatar.estado_actual;
            }
            else
            {
                x = avatar.estado_anterior;
            }
            switch (x)
            {
                case Estado.FELIZ:
                    if (cont % 432 == 0)
                    {
                        avatar.Energia--;
                    }
                    if (cont % 72 == 0)
                    {
                        avatar.Apetito--;
                    }
                    if (cont % 36 == 0)
                    {
                        avatar.Diversion--;
                    }
                    break;
                case Estado.JUGANDO:
                    if (cont % 5 == 0)
                    {
                        avatar.Energia--;
                    }
                    if (cont % 7 == 0)
                    {
                        avatar.Apetito--;
                    }
                    if (cont % 2 == 0)
                    {
                        avatar.Diversion++;
                    }
                    break;
                case Estado.ABURRIDO:
                    if (cont % 450 == 0)
                    {
                        avatar.Energia--;
                    }
                    if (cont % 60 == 0)
                    {
                        avatar.Apetito--;
                    }
                    if (cont % 60 == 0)
                    {
                        avatar.Diversion--;
                    }
                    break;
                case Estado.DORMIDO:
                    if (cont % 72 == 0)
                    {
                        avatar.Energia++;
                    }
                    if (cont % 432 == 0)
                    {
                        avatar.Apetito--;
                    }
                    if (cont % 360 == 0)
                    {
                        avatar.Diversion--;
                    }
                    break;
                case Estado.DORMIDO_PROFUNDAMENTE:
                    if (cont % 36 == 0)
                    {
                        avatar.Energia++;
                    }
                    if (cont % 432 == 0)
                    {
                        avatar.Apetito--;
                    }
                    if (cont % 360 == 0)
                    {
                        avatar.Diversion--;
                    }
                    break;
                case Estado.HAMBRIENTO:
                    if (cont % 360 == 0)
                    {
                        avatar.Energia--;
                    }
                    if (cont % 36 == 0)
                    {
                        avatar.Apetito--;
                    }
                    if (cont % 36 == 0)
                    {
                        avatar.Diversion--;
                    }
                    break;
                case Estado.ENFADADO:
                    if (cont % 288 == 0)
                    {
                        avatar.Energia--;
                    }
                    if (cont % 72 == 0)
                    {
                        avatar.Apetito--;
                    }
                    if (cont % 72 == 0)
                    {
                        avatar.Diversion--;
                    }
                    break;
            }
        }


        private void iniciarArrastre(object sender, MouseButtonEventArgs e)
        {
            DataObject dataO = new DataObject((Image)sender);
            DragDrop.DoDragDrop((Image)sender, dataO, DragDropEffects.Move);
        }


        private void soltar(object sender, DragEventArgs e)
        {
            Image aux = (Image)e.Data.GetData(typeof(Image));
            Comida alimento = null;
            switch (aux.Name)
            {
                case "sandia":
                    alimento = com_sandia;
                    break;
                case "banana":
                    alimento = com_platano;
                    break;
                case "lata":
                    alimento = com_refresco;
                    break;
                case "coockie":
                    alimento = com_galleta;
                    break;
                case "patatas":
                    alimento = com_chips;
                    break;
                case "cafe":
                    alimento = com_cafe;
                    break;
                case "galletas":
                    alimento = com_coockies;
                    break;
                case "pizza":
                    alimento = com_pizza;
                    break;
                case "cocktail":
                    alimento = com_vino;
                    break;
                case "hamburguesa":
                    alimento = com_burguer;
                    break;
            }

            if (avatar.Monedas >= alimento.Coste)
            {
                lbl_gridcomida_coste.Content = "-" + alimento.Coste.ToString();
                lbl_gridcomida_apetito.Content = "+" + alimento.Apetito.ToString();
                lbl_gridcomida_puntos.Content = "+" + alimento.Puntos.ToString();
                lbl_gridcomida_energia.Content = "+" + alimento.Energia.ToString();

                ((Storyboard)this.Resources["animComer"]).Begin();
                ((Storyboard)this.Resources["valorComida"]).Begin();

                avatar.Apetito += alimento.Apetito;
                avatar.Energia += alimento.Energia;
                avatar.Monedas -= alimento.Coste;
                avatar.Puntos_nivel += alimento.Puntos;
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


        private void mover_pieza(object sender, MouseButtonEventArgs e)
        {
            DataObject dataO = new DataObject((Image)sender);
            DragDrop.DoDragDrop((Image)sender, dataO, DragDropEffects.Move);
        }

        private void soltar_pieza(object sender, DragEventArgs e)
        {
            Image aux = (Image)e.Data.GetData(typeof(Image));

            if (((Image)pieza_cogida).Name.Length == 6)
            {
                comparar(sender);

                ((Image)sender).Source = aux.Source;
                if (((Image)sender).Source.ToString() != pieza_puzzle.Source.ToString())
                {
                    ((Image)sender).Stretch = Stretch.Fill;
                }
                ((Image)pieza_cogida).Stretch = Stretch.Uniform;
                ((Image)pieza_cogida).Source = pieza_puzzle.Source;

            }
            else
            {
                comparar(sender);
                ((Image)sender).Stretch = Stretch.Fill;
                ((Image)sender).Source = aux.Source;
                ((Image)pieza_cogida).Visibility = Visibility.Hidden;
            }


        }

       /* private void restablecer_puzzle(object sender, MouseButtonEventArgs e)
        {
            restablecer_puzzle_metodo();
        }*/

        private void restablecer_puzzle(object sender, RoutedEventArgs e)
        {
            restablecer_puzzle_metodo();

        }

        private void moviendo_pieza(object sender, MouseEventArgs e)
        {
            pieza_cogida = sender;
        }

        private void comparar(object sender)
        {
            if (((Image)sender).Source.ToString().Equals(pz1_1.Source.ToString()))
            {
                pz1_1.Visibility = Visibility.Visible;
            }
            else
            {
                if (((Image)sender).Source.ToString().Equals(pz1_2.Source.ToString()))
                {
                    pz1_2.Visibility = Visibility.Visible;
                }
                else
                {
                    if (((Image)sender).Source.ToString().Equals(pz1_3.Source.ToString()))
                    {
                        pz1_3.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        if (((Image)sender).Source.ToString().Equals(pz1_4.Source.ToString()))
                        {
                            pz1_4.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            if (((Image)sender).Source.ToString().Equals(pz1_5.Source.ToString()))
                            {
                                pz1_5.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                if (((Image)sender).Source.ToString().Equals(pz1_6.Source.ToString()))
                                {
                                    pz1_6.Visibility = Visibility.Visible;
                                }
                                else
                                {
                                    if (((Image)sender).Source.ToString().Equals(pz1_7.Source.ToString()))
                                    {
                                        pz1_7.Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        if (((Image)sender).Source.ToString().Equals(pz1_8.Source.ToString()))
                                        {
                                            pz1_8.Visibility = Visibility.Visible;
                                        }
                                        else
                                        {
                                            if (((Image)sender).Source.ToString().Equals(pz1_9.Source.ToString()))
                                            {
                                                pz1_9.Visibility = Visibility.Visible;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private int proxima_animacion()
        {
            int devolver = 1;


            switch (avatar.estado_actual)
            {
                case Estado.FELIZ:
                    if (!avatar.estado_establecido())
                    {
                        if (!avatar.parpadeando)
                        {
                            animParpadear.Begin();
                            avatar.parpadear();
                        }
                        btnDormir.Content = "Dormir";
                        avatar.establecer_estado();
                    }
                    else
                    {
                        if ((avatar.Diversion < 35 && avatar.Apetito < 35) || avatar.Energia < 35)
                        {
                            avatar.proximo_estado(Estado.ENFADADO);
                        }
                        else
                        {
                            if (avatar.Diversion < 35 && avatar.Apetito > 35 && avatar.Energia > 35)
                            {
                                avatar.proximo_estado(Estado.ABURRIDO);
                            }
                            else
                            {
                                if (avatar.Apetito < 35 && avatar.Diversion > 35 && avatar.Energia > 35)
                                {
                                    avatar.proximo_estado(Estado.HAMBRIENTO);
                                }
                            }
                        }
                    }

                    break;
                case Estado.ENFADADO:
                    if (!avatar.estado_establecido())
                    {
                        animParpadear.Stop();
                        avatar.parpadeando = false;
                        ((Storyboard)this.Resources["animTriste"]).Begin();
                        devolver = 2;
                        avatar.establecer_estado();
                    }
                    else
                    {
                        if (avatar.Diversion < 35 && avatar.Apetito > 35 && avatar.Energia > 35)
                        {
                            avatar.proximo_estado(Estado.ABURRIDO);
                        }
                        else
                        {
                            if (avatar.Apetito < 35 && avatar.Diversion > 35 && avatar.Energia > 35)
                            {
                                avatar.proximo_estado(Estado.HAMBRIENTO);
                            }
                            else
                            {
                                if (avatar.Apetito > 35 && avatar.Diversion > 35 && avatar.Energia > 35)
                                {
                                    avatar.proximo_estado(Estado.FELIZ);
                                    ((Storyboard)this.Resources["animContento"]).Begin();
                                    devolver = 2;
                                }
                                else
                                {
                                    if (avatar.Energia < 25)
                                    {
                                        avatar.proximo_estado(Estado.DORMIDO);
                                    }
                                }
                            }
                        }
                    }
                    break;
                case Estado.HAMBRIENTO:
                    if (!avatar.estado_establecido())
                    {
                        prox_hambre = cont + 12;
                        ((Storyboard)this.Resources["animSaciado"]).Begin();
                        devolver = 6;
                        avatar.establecer_estado();
                    }
                    else
                    {
                        if (avatar.Apetito > 35 && avatar.Diversion > 35 && avatar.Energia > 35)
                        {
                            avatar.proximo_estado(Estado.FELIZ);
                            ((Storyboard)this.Resources["animContento"]).Begin();
                            devolver = 2;

                        }
                        else
                        {
                            if (avatar.Apetito > 35 && avatar.Energia > 35 && avatar.Diversion < 35)
                            {
                                avatar.proximo_estado(Estado.ABURRIDO);
                            }
                            else
                            {
                                if (avatar.Energia < 35 || (avatar.Apetito < 35 && avatar.Diversion < 35))
                                {
                                    avatar.proximo_estado(Estado.ENFADADO);
                                }
                                else
                                {
                                    if (avatar.estado_anterior == Estado.HAMBRIENTO && prox_hambre <= cont)
                                    {
                                        prox_hambre = cont + 12;
                                        ((Storyboard)this.Resources["animSaciado"]).Begin();
                                        devolver = 6;
                                    }
                                }
                            }
                        }
                    }
                    break;
                case Estado.DORMIDO:
                    if (!avatar.estado_establecido())
                    {
                        if (avatar.estado_anterior == Estado.ABURRIDO)
                        {
                            ((Storyboard)this.Resources["animJugarYoyo"]).Begin();
                            devolver = 2;
                            avatar.estado_anterior = Estado.JUGANDO;
                        }
                        else
                        {
                            if (!avatar.parpadeando)
                            {
                                animParpadear.Stop();
                                avatar.parpadeando = false;
                            }
                            ((Storyboard)this.Resources["animDormir"]).Begin();
                            avatar.establecer_estado();
                            btnDormir.Content = "Despertar";
                            boca.AllowDrop = false;
                            dientes.AllowDrop = false;
                            lengua.AllowDrop = false;
                            devolver = 2;
                        }
                    }
                    else
                    {
                        avatar.proximo_estado(Estado.DORMIDO_PROFUNDAMENTE);

                    }
                    break;
                case Estado.ABURRIDO:
                    if (!avatar.estado_establecido())
                    {
                        if (!avatar.parpadeando)
                        {
                            animParpadear.Stop();
                            avatar.parpadeando = false;
                        }

                        ((Storyboard)this.Resources["animTriste"]).Begin();
                        avatar.establecer_estado();
                        devolver = 2;
                    }
                    else
                    {
                        if (avatar.Apetito > 35 && avatar.Diversion > 35 && avatar.Energia > 35)
                        {
                            avatar.proximo_estado(Estado.FELIZ);
                            ((Storyboard)this.Resources["animTelaraña"]).Begin();
                            devolver = 2;
                        }
                        else
                        {
                            if (avatar.Diversion > 35 && avatar.Energia > 35 && avatar.Apetito < 35)
                            {
                                avatar.proximo_estado(Estado.HAMBRIENTO);
                                ((Storyboard)this.Resources["animJugarYoyo"]).Begin();
                                devolver = 2;
                            }
                            else
                            {
                                if (avatar.Energia < 35 || (avatar.Apetito < 35 && avatar.Diversion < 35))
                                {
                                    avatar.proximo_estado(Estado.ENFADADO);
                                    ((Storyboard)this.Resources["animTelaraña"]).Begin();
                                    devolver = 2;
                                }
                            }
                        }
                    }
                    break;
                case Estado.DORMIDO_PROFUNDAMENTE:
                    if (!avatar.estado_establecido())
                    {
                        animDormir.Begin();
                        avatar.establecer_estado();
                        devolver = 8;
                    }
                    else
                    {
                        if (avatar.Energia >= 100 || despertar)
                        {
                            despertar = false;
                            avatar.proximo_estado(Estado.FELIZ);
                            animDormir.Stop();
                            ((Storyboard)this.Resources["animDespertar"]).Begin();
                            devolver = 2;
                            boca.AllowDrop = true;
                            dientes.AllowDrop = true;
                            lengua.AllowDrop = true;
                            boca.AllowDrop = true;
                        }
                    }

                    break;

                case Estado.JUGANDO:
                    if (!avatar.estado_establecido())
                    {

                        if (avatar.estado_anterior == Estado.ABURRIDO)
                        {
                            ((Storyboard)this.Resources["animJugarYoyo"]).Begin();
                            devolver = 2;
                            avatar.estado_anterior = Estado.DORMIDO;
                        }
                        else
                        {
                            switch (jugar)
                            {
                                case Juegos.TELARAÑA:
                                    ((Storyboard)this.Resources["animTelaraña"]).Begin();
                                    devolver = 15;
                                    jugar = Juegos.NINGUNA;
                                    avatar.establecer_estado();

                                    break;
                                case Juegos.YOYO:
                                    ((Storyboard)this.Resources["animJugarYoyo"]).Begin();
                                    devolver = 13;
                                    jugar = Juegos.NINGUNA;
                                    avatar.establecer_estado();

                                    break;
                            }
                            boca.AllowDrop = false;
                            dientes.AllowDrop = false;
                            lengua.AllowDrop = false;
                        }

                    }
                    else
                    {
                        if ((avatar.Diversion < 35 && avatar.Apetito < 35) || avatar.Energia < 35)
                        {
                            avatar.proximo_estado(Estado.ENFADADO);
                        }
                        else
                        {
                            if (avatar.Diversion < 35 && avatar.Apetito > 35 && avatar.Energia > 35)
                            {
                                avatar.proximo_estado(Estado.ABURRIDO);
                            }
                            else
                            {
                                if (avatar.Apetito < 35 && avatar.Diversion > 35 && avatar.Energia > 35)
                                {
                                    avatar.proximo_estado(Estado.HAMBRIENTO);
                                }
                                else
                                {
                                    if (avatar.Apetito > 35 && avatar.Diversion > 35 && avatar.Energia > 35)
                                    {
                                        avatar.proximo_estado(Estado.FELIZ);
                                        ((Storyboard)this.Resources["animContento"]).Begin();
                                        devolver = 2;
                                    }
                                }
                            }
                        }
                        boca.AllowDrop = true;
                        dientes.AllowDrop = true;
                        lengua.AllowDrop = true;
                    }
                    break;
            }

            return devolver;
        }


        private int comportamiento_juego_puzzle()
        {
            int devolver = 1;

            if (cont >= tiempo_comienzo)
            {
                if (!border_puzzle.IsEnabled)
                {
                    border_puzzle.IsEnabled = true;
                }
                if (tiempo_juego > 0 && !puzzle_completado)
                {
                    tiempo_juego--;
                    comprobar_puzzle();
                    actualizar_reloj();
                }
                else
                {
                    grid_piezas.IsEnabled = false;
                    puzzle.IsEnabled = false;
                    if (puzzle_completado)
                    {
                        if (!label_fin)
                        {
                            grid_piezas.Visibility = Visibility.Hidden;
                            puzzle.Visibility = Visibility.Hidden;
                            int monedas = 0;
                            if (rdb_dificil.IsChecked == true)
                            {
                                monedas = 3 * tiempo_juego;
                            }
                            else
                            {
                                if (rdb_medio.IsChecked == true)
                                {
                                    monedas = tiempo_juego;
                                }
                                else
                                {
                                    monedas = tiempo_juego / 3;
                                }
                            }
                            lbl_monedas_ganadas.Content = "Has ganado :" + Environment.NewLine + monedas.ToString() + " monedas";
                            lbl_derrota_victoria.Content = "Enhorabuena," + Environment.NewLine + "¡ has ganado !";
                            ((Storyboard)this.Resources["mostrarResultado"]).Begin();
                            avatar.Monedas += monedas;
                            avatar.anadir_monedas(monedas);
                            String dificultad = null;
                            int tiempo = -1;
                            if (rdb_facil.IsChecked == true)
                            {
                                dificultad = "fácil";
                                tiempo = 90 - tiempo_juego;
                            }
                            else
                            {
                                if (rdb_medio.IsChecked == true)
                                {
                                    dificultad = "media";
                                    tiempo = 60 - tiempo_juego;
                                }
                                else
                                {
                                    dificultad = "difícil";
                                    tiempo = 30 - tiempo_juego;
                                }
                            }
                           /* try
                            {
                                twitter.SendTweet(
                                    new SendTweetOptions
                                    {
                                        Status = "¡Enhorabuena @" + avatar.Usuario.ToString() + " !\nHas conseguido " + monedas + " monedas resolviendo un puzzle en dificultad " + dificultad + ", en " + tiempo.ToString() + " segundos.\nSigue jugando para conseguir más monedas!"
                                    }
                                );
                            }
                            catch (Exception) { }*/
                            avatar.puzzle_resuelto();
                            label_fin = true;

                            devolver = 5;
                        }
                        else
                        {
                            grid_piezas.Visibility = Visibility.Visible;
                            puzzle.Visibility = Visibility.Visible;
                            ((Storyboard)this.Resources["ocultarResultado"]).Begin();
                            label_fin = false;
                            devolver = 1;
                            fin_juego = true;
                        }

                    }
                    else
                    {
                        if (!label_fin)
                        {
                            grid_piezas.Visibility = Visibility.Hidden;
                            puzzle.Visibility = Visibility.Hidden;
                            lbl_monedas_ganadas.Content = "";
                            lbl_derrota_victoria.Content = "Lo siento," + Environment.NewLine + "¡ has perdido !";
                            ((Storyboard)this.Resources["mostrarResultado"]).Begin();
                            label_fin = true;
                            String dificultad = null;
                            if (rdb_facil.IsChecked == true)
                            {
                                dificultad = "fácil";
                            }
                            else
                            {
                                if (rdb_medio.IsChecked == true)
                                {
                                    dificultad = "media";
                                }
                                else
                                {
                                    dificultad = "difícil";
                                }
                            }
                           /* try
                            {
                                twitter.SendTweet(
                                    new SendTweetOptions
                                    {
                                        Status = "¡Lo siento @" + avatar.Usuario.ToString() + " ! No has conseguido resolver el puzze en dificultad " + dificultad + ".\nInténtalo de nuevo para conseguir monedas!!."
                                    }
                                );
                            }
                            catch (Exception) { }*/
                            devolver = 5;
                        }
                        else
                        {
                            grid_piezas.Visibility = Visibility.Visible;
                            puzzle.Visibility = Visibility.Visible;
                            ((Storyboard)this.Resources["ocultarResultado"]).Begin();
                            label_fin = false;
                            devolver = 1;
                            fin_juego = true;
                        }

                    }


                }
            }
            else
            {
                if ((tiempo_comienzo - cont) < 4)
                {
                    lbl_cuenta_atras.Content = tiempo_comienzo - cont;
                    devolver = 1;
                }

            }

            if (fin_juego && cont >= prox)
            {
                fin_juego = false;
                jugando_puzzle = false;
                grid_piezas.IsEnabled = true;
                puzzle.IsEnabled = true;
                restablecer_puzzle_metodo();
                border_puzzle.IsEnabled = false;
                puzzle_completado = false;
                label_fin = false;
                ((Storyboard)this.Resources["elegirDificultadPuzzle"]).Begin();
                devolver = 1;
            }


            return devolver;
        }

        private void mostrar_botones(object sender, MouseButtonEventArgs e)
        {
            if ((!botones && cont >= prox) || (!botones && avatar.estado_actual == Estado.DORMIDO_PROFUNDAMENTE))
            {
                if (avatar.estado_actual == Estado.DORMIDO || avatar.estado_actual == Estado.DORMIDO_PROFUNDAMENTE)
                {
                    ((Storyboard)this.Resources["mostrarDormir"]).Begin();
                }
                else
                {
                    ((Storyboard)this.Resources["mostrarBotones"]).Begin();
                }
                botones = true;
                cierre_auto_botones = cont;
            }
        }


        private void mostrar_logros(object sender, MouseButtonEventArgs e)
        {
            if (!estado_logros && cont >= prox)
            {
                if (estado_barra)
                {
                    ((Storyboard)this.Resources["cerrarBarra"]).Begin();
                    lbl_flecha_right.IsEnabled = false;
                    estado_barra = false;
                }
                btn_puzzle.IsEnabled = false;
                lbl_flecha_left.IsEnabled = false;
                ((Storyboard)this.Resources["mostrarLogros"]).Begin();
                cvVenom.IsEnabled = false;
                estado_logros = true;
            }
        }

        

        private void ocultar_logros(object sender, MouseButtonEventArgs e)
        {
            if (estado_logros)
            {
                
                ((Storyboard)this.Resources["cerrarLogros"]).Begin();
                btn_puzzle.IsEnabled = true;
                lbl_flecha_left.IsEnabled = true;
                cvVenom.IsEnabled = true;
                estado_logros = false;
            }
        }

        /*private void tickConsumoHandler(object sender, EventArgs e)
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
        */
        private int proximo_logro()
        {
            Logro logro = avatar.logros_notificar[0];
            avatar.logros_notificar.RemoveAt(0);
            int dev = cont + 4;
            lbl_logro_desbloqueado.Content = logro.Texto;
           /* try
            {
                twitter.SendTweet(
                    new SendTweetOptions
                    {
                        Status = "¡Enhorabuena @" + avatar.Usuario.ToString() + " ! Has desbloqueado el siguiente logro: " + logro.Texto.ToString() + ".\nSigue jugando para desbloquear más logros y subir de nivel."
                    }
                );
            }
            catch (Exception) { }*/

           ((Storyboard)this.Resources["notificacion_logro"]).Begin();
            desbloquear_logro(logro.Id);
            return dev;
        }

     /*   private void controlCansado()
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

        }*/

        private void abrirPuzzle(object sender, RoutedEventArgs e)
        {
            if (!estado_puzzle && cont >= prox)
            {
                if (estado_barra)
                {
                    ((Storyboard)this.Resources["cerrarBarra"]).Begin();
                    lbl_flecha_right.IsEnabled = false;
                    estado_barra = false;
                }
                restablecer_puzzle_metodo();
                btn_trofeo.IsEnabled = false;
                lbl_flecha_left.IsEnabled = false;
                ((Storyboard)this.Resources["elegirDificultadPuzzle"]).Begin();
                border_puzzle.IsEnabled = false;
                cvVenom.IsEnabled = false;
                estado_puzzle = true;
            }
        }

        private void cerrar_puzzle(object sender, MouseButtonEventArgs e)
        {
            if (estado_puzzle)
            {
                btn_trofeo.IsEnabled = true;
                lbl_flecha_left.IsEnabled = true;
                cvVenom.IsEnabled = true;
                ((Storyboard)this.Resources["cerrar_puzzle"]).Begin();
                estado_puzzle = false;

                fin_juego = false;
                label_fin = false;
                jugando_puzzle = false;
                grid_piezas.IsEnabled = true;
                puzzle.IsEnabled = true;
                restablecer_puzzle_metodo();
                border_puzzle.IsEnabled = false;
                puzzle_completado = false;
            }

        }


       /* private void empezar_juego(object sender, MouseButtonEventArgs e)
        {
            ((Storyboard)this.Resources["ocultarElegirDificultad"]).Begin();
            ((Storyboard)this.Resources["abrir_puzzle"]).Begin();

            prox++;
            tiempo_comienzo = cont + 4;
            if (rdb_dificil.IsChecked == true)
            {
                tiempo_juego = 30;
            }
            else
            {
                if (rdb_medio.IsChecked == true)
                {
                    tiempo_juego = 60;
                }
                else
                {
                    tiempo_juego = 90;
                }
            }
            avatar.nueva_partida();
            jugando_puzzle = true;
            actualizar_reloj();
        }*/

        private void actualizar_reloj()
        {
            if ((tiempo_juego % 60).ToString().Length == 1)
            {
                lbl_reloj.Content = (tiempo_juego / 60).ToString() + " : 0" + (tiempo_juego % 60);
            }
            else
            {
                lbl_reloj.Content = (tiempo_juego / 60).ToString() + " : " + (tiempo_juego % 60);
            }
        }

        private void cerrar_empezar_juego(object sender, MouseButtonEventArgs e)
        {
            if (estado_puzzle)
            {
                btn_trofeo.IsEnabled = true;
                lbl_flecha_left.IsEnabled = true;
                cvVenom.IsEnabled = true;
                ((Storyboard)this.Resources["ocultarElegirDificultad"]).Begin();
                border_puzzle.IsEnabled = true;
                estado_puzzle = false;
            }
        }
        


        private void empezarJuego(object sender, RoutedEventArgs e)
        {
            ((Storyboard)this.Resources["ocultarElegirDificultad"]).Begin();
            ((Storyboard)this.Resources["abrir_puzzle"]).Begin();

            prox++;
            tiempo_comienzo = cont + 4;
            if (rdb_dificil.IsChecked == true)
            {
                tiempo_juego = 30;
            }
            else
            {
                if (rdb_medio.IsChecked == true)
                {
                    tiempo_juego = 60;
                }
                else
                {
                    tiempo_juego = 90;
                }
            }
            avatar.nueva_partida();
            jugando_puzzle = true;
            actualizar_reloj();
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

        private void comprobar_puzzle()
        {
            Boolean p1 = pieza1.Source.ToString().Equals(pz1_1.Source.ToString());
            Boolean p2 = pieza2.Source.ToString().Equals(pz1_2.Source.ToString());
            Boolean p3 = pieza3.Source.ToString().Equals(pz1_3.Source.ToString());
            Boolean p4 = pieza4.Source.ToString().Equals(pz1_4.Source.ToString());
            Boolean p5 = pieza5.Source.ToString().Equals(pz1_5.Source.ToString());
            Boolean p6 = pieza6.Source.ToString().Equals(pz1_6.Source.ToString());
            Boolean p7 = pieza7.Source.ToString().Equals(pz1_7.Source.ToString());
            Boolean p8 = pieza8.Source.ToString().Equals(pz1_8.Source.ToString());
            Boolean p9 = pieza9.Source.ToString().Equals(pz1_9.Source.ToString());

            if (p1 && p2 && p3 && p4 && p5 && p6 && p7 && p8 && p9)
            {
                puzzle_completado = true;
            }
        }

        private void autenticarse(object sender, RoutedEventArgs e)
        {
            try
            {


                String con = (new Avatar()).autenticar(tb_inicio_usuario.Text.ToString(), tb_inicio_contrasena.Password.ToString());
                switch (con)
                {
                    case "0":
                        lbl_msg_inicio.Content = "USUARIO CORRECTO";
                        btn_puzzle.IsEnabled = true;
                        btn_trofeo.IsEnabled = true;
                        cvVenom.IsEnabled = true;
                        ((Storyboard)this.Resources["ocultarAutenticacion"]).Begin();
                        usuario = tb_inicio_usuario.Text.ToString();
                        inicializar_avatar();
                        break;
                    case "1":
                        lbl_msg_inicio.Foreground = Brushes.DarkRed;
                        lbl_msg_inicio.Content = "CREDENCIALES INCORRECTOS";
                        break;
                }
            }
            catch (Exception)
            {
                lbl_msg_inicio.Content = "Error al conectar con la base de datos";
            }
        }

        private void animacion_registro(object sender, RoutedEventArgs e)
        {
            tb_inicio_usuario.Text = "";
            tb_inicio_contrasena.Password = "";
            lbl_msg_inicio.Content = "";
            ((Storyboard)this.Resources["inicio_registro"]).Begin();
        }

        private void animacion_ir_inicio(object sender, RoutedEventArgs e)
        {
            try {
                new Avatar().registrar(tb_registro_usuario.Text.ToString(), tb_registro_contrasena.Password.ToString());
                
                            
                            ((Storyboard)this.Resources["ocultarRegistro"]).Begin();
                            btn_puzzle.IsEnabled = true;
                            btn_trofeo.IsEnabled = true;
                            cvVenom.IsEnabled = true;
                             usuario = tb_registro_usuario.Text.ToString();
                            inicializar_avatar();
                
                       
                }
                catch (Exception)
                {
                    lbl_msg_registro.Content = "Error al conectar con la base de datos";
                }

            
        }


        private void ventana_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                avatar.actualizar();
            }
            catch(Exception exc)
            {
                exc.ToString();
            }
        }

        private void logoVenom_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Boolean reproducido = false;
            if (reproducido == false)
            {
                ((Storyboard)this.Resources["ocultarBotones"]).Begin();

                ((Storyboard)this.Resources["hablar"]).Begin();
                SoundPlayer simpleSound = new SoundPlayer(Properties.Resources.we_are_venom);
                simpleSound.Play();
            }
            
        }

        private void volver_atras_registro(object sender, RoutedEventArgs e)
        {
            tb_registro_usuario.Text = "";
            tb_registro_contrasena.Password = "";
            lbl_msg_registro.Content = "";
            ((Storyboard)this.Resources["volverAutenticacion"]).Begin();
        }


        private void jugar_yoyo(object sender, RoutedEventArgs e)
        {
            if (botones)
            {
                ((Storyboard)this.Resources["ocultarBotones"]).Begin();
            }
            avatar.proximo_estado(Estado.JUGANDO);
            jugar = Juegos.YOYO;
            avatar.Puntos_nivel += 15;
            avatar.Diversion += 19;
            avatar.Energia -= 3;
            botones = false;

        }


        private void lanzar_tela(object sender, RoutedEventArgs e)
        {
            if (botones)
            {
                ((Storyboard)this.Resources["ocultarBotones"]).Begin();
            }
            avatar.proximo_estado(Estado.JUGANDO);
            jugar = Juegos.TELARAÑA;
            avatar.Puntos_nivel += 20;
            avatar.Energia -= 7;
            avatar.Diversion += 25;
            botones = false;
        }

        private void dormir(object sender, RoutedEventArgs e)
        {
            if (botones)
            {
                if (avatar.estado_actual == Estado.DORMIDO || avatar.estado_actual == Estado.DORMIDO_PROFUNDAMENTE)
                {
                    ((Storyboard)this.Resources["esconderDormir"]).Begin();

                }
                else
                {
                    ((Storyboard)this.Resources["ocultarBotones"]).Begin();
                }
            }


            switch (btnDormir.Content.ToString())
            {
                case "Dormir":
                    avatar.Puntos_nivel += 10;
                    logoVenom.IsEnabled = false;
                    avatar.proximo_estado(Estado.DORMIDO);
                    break;
                case "Despertar":
                    avatar.Puntos_nivel += 5;
                    despertar = true;
                    logoVenom.IsEnabled = true;

                    break;
            }
            botones = false;
        }
    }

   
    

}
