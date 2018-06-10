using MiAppVenom.Dominio;
using System;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;


namespace MiAppVenom
{

    public enum Juegos
    {
        YOYO,
        TELARAÑA,
        NINGUNO
    }

    public partial class MainWindow : Window
    {
      
        //ANIMACIONES
        private static Storyboard animParpadear;
        private static Storyboard animDormir;

        // ALIMENTOS        
        private static Comida comPlatano;
        private static Comida comPatatas;
        private static Comida comCafe;
        private static Comida comSandia;
        private static Comida comRefresco;
        private static Comida comSnacks;
        private static Comida comPizza;
        private static Comida comCocktail;
        private static Comida comGalleta;
        private static Comida comBurguer;

        //ESTADO INTERFACES
        private static Boolean barraActiva;
        private static Boolean finJuego;
        private static Boolean puzzleTerminado;
        private static Boolean lblFin;
        private static Boolean jugandoPuzzle;
        private static Boolean puzzleAbierto;
        private static Boolean botonesActivos;
        private static Boolean verLogrosActivo;

        //INTERACCION
        private Image piezaPuzzle;
        private Object piezaCogida;

        //ACCIONES
        private static Juegos jugar;
        private static Boolean despertar;

        //TIEMPOS
        private static DispatcherTimer temporizador;
        private static double intervalo = 1000.0;
        private static int contadorTiempo, prox, cierreBotonesAccion, proxLogro, proxHambre;
        private static int tiempoPuzzle;
        private static int tiempoComienzoPuzzle;

        //AVATAR Y NOMBRE DE USUARIO
        private Avatar avatar;
        public static String usuario;



        public MainWindow()
        {

            InitializeComponent();
            ((Storyboard)this.Resources["cerrarBarra"]).Begin();
            cvVenom.IsEnabled = false;
            btnPuzzle.IsEnabled = false;
            btnLogros.IsEnabled = false;
            
        }

        private void cargarAvatar()
        {
            Boolean avatarCargado = false;
            contadorTiempo = 0;
            cierreBotonesAccion = -1;
            tiempoComienzoPuzzle = 0;
            tiempoPuzzle = -1;
            prox = 0;
            proxLogro = 0;
            proxHambre = 0;

            try
            {
                avatar = (new Avatar()).leerUsuario(usuario);
                avatarCargado = true;
            }
            catch (Exception e1) {
                e1.ToString();
            }

            if (avatarCargado)
            {
                cargarLogros();
                cargarEstadisticas();
                cargarAlimentos();
                cargarEstados();

                lblUsuario.Content = avatar.Usuario;
                despertar = false;
                piezaCogida = null;
                piezaPuzzle = new Image();
                piezaPuzzle.Source = pieza9.Source;

                inicializarAnimaciones();
                inicializarTimers();
            }

        }

        private void cargarEstados()
        {
            barraActiva = false;
            verLogrosActivo = false;
            puzzleAbierto = false;
            lblFin = false;
            finJuego = false;
            botonesActivos = false;
            jugandoPuzzle = false;
            puzzleTerminado = false;
            lblFlechaDcha.IsEnabled = false;
            lblFlechaIzda.IsEnabled = true;
        }

        private void cargarAlimentos()
        {
            comSandia = new Comida("sandia", 5, 3, 5, 3);
            comPlatano = new Comida("banana", 6, 2, 6, 5);
            comRefresco = new Comida("lata", 7, 5, 2, 4);
            comSnacks = new Comida("coockie", 3, 1, 3, 7);
            comCafe = new Comida("cafe", 6, 20, 2, 8);
            comPatatas = new Comida("patatas", 7, 2, 20, 5);
            comCocktail = new Comida("cocktail", 10, 5, 7, 15);
            comGalleta = new Comida("galleta", 12, 3, 15, 18);
            comPizza = new Comida("pizza", 15, 6, 20, 20);
            comBurguer = new Comida("hamburguesa", 20, 4, 25, 20);

            for (int i = 2; i <= avatar.Nivel; i++)
            {
                desbloquearAlimentos(i);
            }
        }

        private void cargarEstadisticas()
        {
            lblNivel.Content = avatar.Nivel.ToString();
            lblPuntos.Content = avatar.PuntosNivel.ToString();
            lblMonedasRestantes.Content = avatar.Monedas.ToString();
            pbNivel.Maximum = 100 * avatar.Nivel;
            pbNivel.Value = avatar.PuntosNivel;
            pbMonedas.Value = avatar.Monedas;
            lblApetito.Content = avatar.Apetito.ToString() + " %";
            lblDiversion.Content = avatar.Diversion.ToString() + " %";
            lblEnergia.Content = avatar.Energia.ToString() + " %";
            pbApetito.Value = avatar.Apetito;
            pbDiversion.Value = avatar.Diversion;
            pbEnergia.Value = avatar.Energia;
        }

        private void inicializarTimers()
        {
            temporizador = new DispatcherTimer();
            temporizador.Interval = TimeSpan.FromMilliseconds(intervalo);
            temporizador.Tick += new EventHandler(tick);
            temporizador.Start();
        }

        private void inicializarAnimaciones()
        {
            animParpadear = (Storyboard)this.Resources["animParpadear"];
            animDormir = (Storyboard)this.Resources["animSoñar"];
        }

        private void tick(object sender, EventArgs e)
        {
            if (contadorTiempo >= proxLogro && avatar.logrosParaNotificar.Count > 0)
            {
                proxLogro = notificarProximoLogro();
            }
            if (contadorTiempo == cierreBotonesAccion + 4 && botonesActivos)
            {
                if (avatar.estadoActual == Estado.DORMIDO || avatar.estadoActual == Estado.SOÑANDO)
                {
                    ((Storyboard)this.Resources["esconderDormir"]).Begin();

                }
                else
                {
                    ((Storyboard)this.Resources["ocultarBotones"]).Begin();
                }
                botonesActivos = false;
            }
            if (contadorTiempo >= prox)
            {
                if (jugandoPuzzle)
                {
                    prox += mecanismoComportamientoPuzzle();
                }
                else
                {
                    if (avatar.NuevoNivel)
                    {
                        lblSubirNivel.Content = avatar.Nivel.ToString();
                        pbNivel.Maximum = 100 * avatar.Nivel;
                        avatar.NuevoNivel = false;
                        ((Storyboard)this.Resources["subirNivel"]).Begin();

                        prox += 3;
                        desbloquearAlimentos(avatar.Nivel);
                    }
                    else
                    {
                        prox += proximaAnimacion();
                    }
                }

            }

            consumirValoresPB();

            pbDiversion.Value = avatar.Diversion;
            pbApetito.Value = avatar.Apetito;
            pbEnergia.Value = avatar.Energia;

            lblApetito.Content = avatar.Apetito.ToString() + " %";
            lblDiversion.Content = avatar.Diversion.ToString() + " %";
            lblEnergia.Content = avatar.Energia.ToString() + " %";

            lblNivel.Content = avatar.Nivel.ToString();
            lblPuntos.Content = avatar.PuntosNivel.ToString();
            lblMonedasRestantes.Content = avatar.Monedas.ToString();

            pbMonedas.Value = avatar.Monedas;
            pbNivel.Maximum = avatar.Nivel * 100;
            pbNivel.Value = avatar.PuntosNivel;

            //ACTUALIZAMOS NUESTRO AVATAR CADA 2 MIN
            if (contadorTiempo % 120 == 0)
            {
                avatar.actualizar();
            }

            contadorTiempo++;
        }

        private void cargarLogros()
        {
            String logros = "";
            foreach (string key in avatar.Logros.Keys)
            {
                if (((Logro)avatar.Logros[key]).Desbloqueado)
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
                char[] separador = { ' ' };
                string[] idLogros = logros.Split(separador);
                for (int i = 0; i < idLogros.Length; i++)
                {
                    desbloquearLogro(int.Parse(idLogros[i]));
                }
            }
        }

        private void desbloquearLogro(int id)
        {
            switch (id)
            {
                case 1:

                    logroNivel3.Opacity = 100;
                    break;

                case 2:

                    logroNivel6.Opacity = 100;
                    break;

                case 3:

                    logroNivel10.Opacity = 100;
                    break;

                case 4:

                    logroJuega5Veces.Opacity = 100;
                    break;

                case 5:

                    logroJuega10Veces.Opacity = 100;
                    break;

                case 6:

                    logroJuega20Veces.Opacity = 100;
                    break;

                case 7:

                    logro200Monedas.Opacity = 100;
                    break;

                case 8:

                    logro500Monedas.Opacity = 100;
                    break;

                case 9:

                    logro1000Monedas.Opacity = 100;
                    break;

                case 10:

                    logro5PuzzlesGanados.Opacity = 100;
                    break;

                case 11:

                    logro10PuzzlesGanados.Opacity = 100;
                    break;

                case 12:

                    logroSecreto.Opacity = 100;
                    txtLogroSecreto.Text = "Desbloquea todos los logros";
                    break;
            }
        }

       

        private void MouseUp_label_right(object sender, MouseButtonEventArgs e)
        {
            ((Storyboard)this.Resources["cerrarBarra"]).Begin();
            lblFlechaDcha.IsEnabled = false;
            lblFlechaIzda.IsEnabled = true;
            barraActiva = false;
        }

        private void MouseUp_label_left(object sender, MouseButtonEventArgs e)
        {
            ((Storyboard)this.Resources["abrirBarra"]).Begin();
            lblFlechaIzda.IsEnabled = false;
            lblFlechaDcha.IsEnabled = true;
            barraActiva = true;
        }


        private void consumirValoresPB()
        {
            Estado x;
            if (avatar.estadoEstablecido())
            {
                x = avatar.estadoActual;
            }
            else
            {
                x = avatar.estadoAnterior;
            }
            switch (x)
            {
                case Estado.FELIZ:
                    if (contadorTiempo % 434 == 0)
                    {
                        avatar.Energia--;
                    }
                    if (contadorTiempo % 74 == 0)
                    {
                        avatar.Apetito--;
                    }
                    if (contadorTiempo % 38 == 0)
                    {
                        avatar.Diversion--;
                    }
                    break;
                case Estado.JUGANDO:
                    if (contadorTiempo % 6 == 0)
                    {
                        avatar.Energia--;
                    }
                    if (contadorTiempo % 5 == 0)
                    {
                        avatar.Apetito--;
                    }
                    if (contadorTiempo % 1 == 0)
                    {
                        avatar.Diversion++;
                    }
                    break;
                case Estado.ABURRIDO:
                    if (contadorTiempo % 450 == 0)
                    {
                        avatar.Energia--;
                    }
                    if (contadorTiempo % 60 == 0)
                    {
                        avatar.Apetito--;
                    }
                    if (contadorTiempo % 60 == 0)
                    {
                        avatar.Diversion--;
                    }
                    break;
                case Estado.DORMIDO:
                    if (contadorTiempo % 6 == 0)
                    {
                        avatar.Energia++;
                    }
                    if (contadorTiempo % 300 == 0)
                    {
                        avatar.Apetito--;
                    }
                    if (contadorTiempo % 360 == 0)
                    {
                        avatar.Diversion--;
                    }
                    break;
                case Estado.SOÑANDO:
                    if (contadorTiempo % 3 == 0)
                    {
                        avatar.Energia++;
                    }
                    if (contadorTiempo % 432 == 0)
                    {
                        avatar.Apetito--;
                    }
                    if (contadorTiempo % 360 == 0)
                    {
                        avatar.Diversion--;
                    }
                    break;
                case Estado.HAMBRIENTO:
                    if (contadorTiempo % 360 == 0)
                    {
                        avatar.Energia--;
                    }
                    if (contadorTiempo % 18 == 0)
                    {
                        avatar.Apetito--;
                    }
                    if (contadorTiempo % 36 == 0)
                    {
                        avatar.Diversion--;
                    }
                    break;
                case Estado.ENFADADO:
                    if (contadorTiempo % 288 == 0)
                    {
                        avatar.Energia--;
                    }
                    if (contadorTiempo % 72 == 0)
                    {
                        avatar.Apetito--;
                    }
                    if (contadorTiempo % 72 == 0)
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
                    alimento = comSandia;
                    break;
                case "banana":
                    alimento = comPlatano;
                    break;
                case "lata":
                    alimento = comRefresco;
                    break;
                case "coockie":
                    alimento = comSnacks;
                    break;
                case "patatas":
                    alimento = comPatatas;
                    break;
                case "cafe":
                    alimento = comCafe;
                    break;
                case "galletas":
                    alimento = comGalleta;
                    break;
                case "pizza":
                    alimento = comPizza;
                    break;
                case "cocktail":
                    alimento = comCocktail;
                    break;
                case "hamburguesa":
                    alimento = comBurguer;
                    break;
            }

            comprobarPrecio(alimento);
           

        }

        private void comprobarPrecio(Comida alimento)
        {
            if (avatar.Monedas >= alimento.Coste)
            {
                lbl_gridcomida_coste.Content = "-" + alimento.Coste.ToString();
                lbl_gridcomida_apetito.Content = "+" + alimento.Apetito.ToString();
                lbl_gridcomida_puntos.Content = "+" + alimento.Puntos.ToString();
                lbl_gridcomida_energia.Content = "+" + alimento.Energia.ToString();

                avatar.Apetito += alimento.Apetito;
                avatar.Energia += alimento.Energia;
                avatar.Monedas -= alimento.Coste;
                avatar.PuntosNivel += alimento.Puntos;

                ((Storyboard)this.Resources["animComer"]).Begin();
                ((Storyboard)this.Resources["valorComida"]).Begin();
            }
            else
            {
                MessageBox.Show("Ups... parece que aún no tienes suficientes monedas para comprar este alimento...\n¡Juega un poco o realiza algunos puzzles para conseguir monedas!", "DINERO INSUFICIENTE");

            }
        }

        private void desbloquearAlimentos(int nivel)
        {
            switch (nivel)
            {
                case 2:
                    lblNivel2_1.Visibility = Visibility.Hidden;
                    lblNivel2_2.Visibility = Visibility.Hidden;
                    lblNivel2_3.Visibility = Visibility.Hidden;
                    coockie.IsEnabled = true;
                    patatas.IsEnabled = true;
                    cafe.IsEnabled = true;
                    coockie.Opacity = 100;
                    patatas.Opacity = 100;
                    cafe.Opacity = 100;
                    lblMonedasSnacks.Opacity = 100;
                    lblMonedasPatatas.Opacity = 100;
                    lblPrecioPatatas.Opacity = 100;
                    lblPrecioSnacks.Opacity = 100;
                    lblMonedasCafe.Opacity = 100;
                    lblPrecioCafe.Opacity = 100;
                    
                    break;
                case 3:
                    lblNivel3.Visibility = Visibility.Hidden;                   
                    galletas.IsEnabled = true;
                    galletas.Opacity = 100;
                    lblMonedasGalleta.Opacity = 100;
                    lblPrecioGalleta.Opacity = 100;
                    break;
                case 6:
                    lblNivel6.Visibility = Visibility.Hidden;
                    pizza.IsEnabled = true;
                    pizza.Opacity = 100;
                    lblMonedasPizza.Opacity = 100;
                    lblPrecioPizza.Opacity = 100;
                    break;
                case 8:
                    lblNivel8.Visibility = Visibility.Hidden;
                    cocktail.IsEnabled = true;
                    cocktail.Opacity = 100;
                    lblMonedasCocktail.Opacity = 100;
                    lblPrecioCocktail.Opacity = 100;
                    break;
                case 10:
                    lblNivel10.Visibility = Visibility.Hidden;
                    hamburguesa.IsEnabled = true;
                    hamburguesa.Opacity = 100;
                    lblMonedasBurguer.Opacity = 100;
                    lblPrecioBurguer.Opacity = 100;
                    break;
                default:
                    break;
            }
        }


        private void moverPieza(object sender, MouseButtonEventArgs e)
        {
            DataObject dataO = new DataObject((Image)sender);
            DragDrop.DoDragDrop((Image)sender, dataO, DragDropEffects.Move);
        }

        private void soltarPieza(object sender, DragEventArgs e)
        {
            Image aux = (Image)e.Data.GetData(typeof(Image));

            if (((Image)piezaCogida).Name.Length == 6)
            {
                comparar(sender);

                ((Image)sender).Source = aux.Source;
                if (((Image)sender).Source.ToString() != piezaPuzzle.Source.ToString())
                {
                    ((Image)sender).Stretch = Stretch.Fill;
                }
                ((Image)piezaCogida).Stretch = Stretch.Uniform;
                ((Image)piezaCogida).Source = piezaPuzzle.Source;

            }
            else
            {
                comparar(sender);
                ((Image)sender).Stretch = Stretch.Fill;
                ((Image)sender).Source = aux.Source;
                ((Image)piezaCogida).Visibility = Visibility.Hidden;
            }


        }

  

        private void restablecerPuzzle(object sender, RoutedEventArgs e)
        {
            restablecerPuzzlePublic();

        }

        private void moviendoPieza(object sender, MouseEventArgs e)
        {
            piezaCogida = sender;
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

        private int proximaAnimacion()
        {
            int devolver = 1;


            switch (avatar.estadoActual)
            {
                case Estado.JUGANDO:
                    if (!avatar.estadoEstablecido())
                    {

                        if (avatar.estadoAnterior == Estado.ABURRIDO)
                        {
                            ((Storyboard)this.Resources["animJugarYoyo"]).Begin();
                            devolver = 2;
                            avatar.estadoAnterior = Estado.DORMIDO;
                        }
                        else
                        {
                            switch (jugar)
                            {
                                case Juegos.TELARAÑA:
                                    ((Storyboard)this.Resources["animTelaraña"]).Begin();
                                    devolver = 15;
                                    jugar = Juegos.NINGUNO;
                                    avatar.establecerEstado();

                                    break;
                                case Juegos.YOYO:
                                    ((Storyboard)this.Resources["animJugarYoyo"]).Begin();
                                    devolver = 13;
                                    jugar = Juegos.NINGUNO;
                                    avatar.establecerEstado();

                                    break;
                            }
                            boca.AllowDrop = false;
                            dientes.AllowDrop = false;
                            lengua.AllowDrop = false;
                        }

                    }
                    else
                    {
                        if ((avatar.Diversion < 40 && avatar.Apetito < 40) || avatar.Energia < 40)
                        {
                            avatar.proximoEstado(Estado.ENFADADO);
                        }
                        else
                        {
                            if (avatar.Diversion < 40 && avatar.Apetito > 40 && avatar.Energia > 40)
                            {
                                avatar.proximoEstado(Estado.ABURRIDO);
                            }
                            else
                            {
                                if (avatar.Apetito < 40 && avatar.Diversion > 40 && avatar.Energia > 40)
                                {
                                    avatar.proximoEstado(Estado.HAMBRIENTO);
                                }
                                else
                                {
                                    if (avatar.Apetito > 40 && avatar.Diversion > 40 && avatar.Energia > 40)
                                    {
                                        avatar.proximoEstado(Estado.FELIZ);
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
                case Estado.ENFADADO:
                    if (!avatar.estadoEstablecido())
                    {
                        animParpadear.Stop();
                        avatar.parpadeando = false;
                        ((Storyboard)this.Resources["animTriste"]).Begin();
                        devolver = 2;
                        avatar.establecerEstado();
                    }
                    else
                    {
                        if (avatar.Diversion < 40 && avatar.Apetito > 40 && avatar.Energia > 40)
                        {
                            avatar.proximoEstado(Estado.ABURRIDO);
                        }
                        else
                        {
                            if (avatar.Apetito < 40 && avatar.Diversion > 40 && avatar.Energia > 40)
                            {
                                avatar.proximoEstado(Estado.HAMBRIENTO);
                            }
                            else
                            {
                                if (avatar.Apetito > 40 && avatar.Diversion > 40 && avatar.Energia > 40)
                                {
                                    avatar.proximoEstado(Estado.FELIZ);
                                    ((Storyboard)this.Resources["animContento"]).Begin();
                                    devolver = 2;
                                }
                                else
                                {
                                    if (avatar.Energia < 25)
                                    {
                                        avatar.proximoEstado(Estado.DORMIDO);
                                    }
                                }
                            }
                        }
                    }
                    break;
                case Estado.FELIZ:
                    if (!avatar.estadoEstablecido())
                    {
                        if (!avatar.parpadeando)
                        {
                            animParpadear.Begin();
                            avatar.parpadear();
                        }
                        btnDormir.Content = "Dormir";
                        avatar.establecerEstado();
                    }
                    else
                    {
                        if ((avatar.Diversion < 40 && avatar.Apetito < 40) || avatar.Energia < 40)
                        {
                            avatar.proximoEstado(Estado.ENFADADO);
                        }
                        else
                        {
                            if (avatar.Diversion < 40 && avatar.Apetito > 40 && avatar.Energia > 40)
                            {
                                avatar.proximoEstado(Estado.ABURRIDO);
                            }
                            else
                            {
                                if (avatar.Apetito < 40 && avatar.Diversion > 40 && avatar.Energia > 40)
                                {
                                    avatar.proximoEstado(Estado.HAMBRIENTO);
                                }
                            }
                        }
                    }

                    break;
                
                
                case Estado.DORMIDO:
                    if (!avatar.estadoEstablecido())
                    {
                        if (avatar.estadoAnterior == Estado.ABURRIDO)
                        {
                            ((Storyboard)this.Resources["animJugarYoyo"]).Begin();
                            devolver = 2;
                            avatar.estadoAnterior = Estado.JUGANDO;
                        }
                        else
                        {
                            if (!avatar.parpadeando)
                            {
                                animParpadear.Stop();
                                avatar.parpadeando = false;
                            }
                            ((Storyboard)this.Resources["animDormir"]).Begin();
                            avatar.establecerEstado();
                            btnDormir.Content = "Despertar";
                            boca.AllowDrop = false;
                            dientes.AllowDrop = false;
                            lengua.AllowDrop = false;
                            devolver = 2;
                        }
                    }
                    else
                    {
                        avatar.proximoEstado(Estado.SOÑANDO);

                    }
                    break;
                case Estado.HAMBRIENTO:
                    if (!avatar.estadoEstablecido())
                    {
                        proxHambre = contadorTiempo + 12;
                        ((Storyboard)this.Resources["animSaciado"]).Begin();
                        devolver = 6;
                        avatar.establecerEstado();
                    }
                    else
                    {
                        if (avatar.Apetito > 40 && avatar.Diversion > 40 && avatar.Energia > 40)
                        {
                            avatar.proximoEstado(Estado.FELIZ);
                            ((Storyboard)this.Resources["animContento"]).Begin();
                            devolver = 2;

                        }
                        else
                        {
                            if (avatar.Apetito > 40 && avatar.Energia > 40 && avatar.Diversion < 40)
                            {
                                avatar.proximoEstado(Estado.ABURRIDO);
                            }
                            else
                            {
                                if (avatar.Energia < 40 || (avatar.Apetito < 40 && avatar.Diversion < 40))
                                {
                                    avatar.proximoEstado(Estado.ENFADADO);
                                }
                                else
                                {
                                    if (avatar.estadoAnterior == Estado.HAMBRIENTO && proxHambre <= contadorTiempo)
                                    {
                                        proxHambre = contadorTiempo + 12;
                                        ((Storyboard)this.Resources["animSaciado"]).Begin();
                                        devolver = 6;
                                    }
                                }
                            }
                        }
                    }
                    break;
                case Estado.SOÑANDO:
                    if (!avatar.estadoEstablecido())
                    {
                        animDormir.Begin();
                        avatar.establecerEstado();
                        devolver = 8;
                    }
                    else
                    {
                        if (avatar.Energia >= 100 || despertar)
                        {
                            despertar = false;
                            avatar.proximoEstado(Estado.FELIZ);
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
                case Estado.ABURRIDO:
                    if (!avatar.estadoEstablecido())
                    {
                        if (!avatar.parpadeando)
                        {
                            animParpadear.Stop();
                            avatar.parpadeando = false;
                        }

                        ((Storyboard)this.Resources["animTriste"]).Begin();
                        avatar.establecerEstado();
                        devolver = 2;
                    }
                    else
                    {
                        if (avatar.Apetito > 40 && avatar.Diversion > 40 && avatar.Energia > 40)
                        {
                            avatar.proximoEstado(Estado.FELIZ);
                            ((Storyboard)this.Resources["animTelaraña"]).Begin();
                            devolver = 2;
                        }
                        else
                        {
                            if (avatar.Diversion > 40 && avatar.Energia > 40 && avatar.Apetito < 40)
                            {
                                avatar.proximoEstado(Estado.HAMBRIENTO);
                                ((Storyboard)this.Resources["animJugarYoyo"]).Begin();
                                devolver = 2;
                            }
                            else
                            {
                                if (avatar.Energia < 40 || (avatar.Apetito < 40 && avatar.Diversion < 40))
                                {
                                    avatar.proximoEstado(Estado.ENFADADO);
                                    ((Storyboard)this.Resources["animTelaraña"]).Begin();
                                    devolver = 2;
                                }
                            }
                        }
                    }
                    break;
                

                
            }

            return devolver;
        }


        private int mecanismoComportamientoPuzzle()
        {
            int devolver = 1;

         
                if (!border_puzzle.IsEnabled)
                {
                    border_puzzle.IsEnabled = true;
                }
                if (tiempoPuzzle > 0 && !puzzleTerminado)
                {
                    tiempoPuzzle--;
                    comprobarPuzzle();
                    actualizarMinutero();
                }
                else
                {
                    grid_piezas.IsEnabled = false;
                    puzzle.IsEnabled = false;
                    if (puzzleTerminado)
                    {
                        if (!lblFin)
                        {
                            grid_piezas.Visibility = Visibility.Hidden;
                            puzzle.Visibility = Visibility.Hidden;
                            int monedas = 0;
                            if (rdb_dificil.IsChecked == true)
                            {
                                monedas = 3 * tiempoPuzzle;
                            }
                            else
                            {
                                if (rdb_medio.IsChecked == true)
                                {
                                    monedas = tiempoPuzzle;
                                }
                                else
                                {
                                    monedas = tiempoPuzzle / 3;
                                }
                            }
                            avatar.Monedas += monedas;
                            avatar.sumarMonedas(monedas);
                            String dificultad = null;
                            int tiempo = -1;
                            if (rdb_facil.IsChecked == true)
                            {
                                dificultad = "fácil";
                                tiempo = 90 - tiempoPuzzle;
                            }
                            else
                            {
                                if (rdb_medio.IsChecked == true)
                                {
                                    dificultad = "media";
                                    tiempo = 60 - tiempoPuzzle;
                                }
                                else
                                {
                                    dificultad = "difícil";
                                    tiempo = 30 - tiempoPuzzle;
                                }
                            }
                            lblPremio.Content = "PREMIO: " + Environment.NewLine + monedas.ToString() + " monedas";
                            lblResultado.Content = "¡GANASTE" + Environment.NewLine + "EN "+dificultad.ToUpper()+"!";
                            ((Storyboard)this.Resources["mostrarResultado"]).Begin();
                            avatar.puzzleGanado();
                            lblFin = true;
                            devolver = 5;
                        }
                        else
                        {
                            grid_piezas.Visibility = Visibility.Visible;
                            puzzle.Visibility = Visibility.Visible;
                            ((Storyboard)this.Resources["ocultarResultado"]).Begin();
                            lblFin = false;
                            devolver = 1;
                            finJuego = true;
                        }

                    }
                    else
                    {
                        if (!lblFin)
                        {
                            grid_piezas.Visibility = Visibility.Hidden;
                            puzzle.Visibility = Visibility.Hidden;
                            lblPremio.Content = "";
                            lblFin = true;
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
                            lblResultado.Content = "PERDISTE" + Environment.NewLine + "EN DIFICULTAD "+dificultad.ToUpper()+" :(";
                            ((Storyboard)this.Resources["mostrarResultado"]).Begin();
                            devolver = 5;
                        }
                        else
                        {
                            grid_piezas.Visibility = Visibility.Visible;
                            puzzle.Visibility = Visibility.Visible;
                            ((Storyboard)this.Resources["ocultarResultado"]).Begin();
                            lblFin = false;
                            devolver = 1;
                            finJuego = true;
                        }

                    }


                }
           

            if (finJuego && contadorTiempo >= prox)
            {
                finJuego = false;
                jugandoPuzzle = false;
                grid_piezas.IsEnabled = true;
                puzzle.IsEnabled = true;
                restablecerPuzzlePublic();
                puzzleTerminado = false;
                lblFin = false;

                btnLogros.IsEnabled = true;
                lblFlechaIzda.IsEnabled = true;
                cvVenom.IsEnabled = true;
                border_puzzle.IsEnabled = false;
                puzzleAbierto = false;

                devolver = 1;
            }


            return devolver;
        }

        private void mostrarBotones(object sender, MouseButtonEventArgs e)
        {
            if ((!botonesActivos && contadorTiempo >= prox) || (!botonesActivos && avatar.estadoActual == Estado.SOÑANDO))
            {
                if (avatar.estadoActual == Estado.DORMIDO || avatar.estadoActual == Estado.SOÑANDO)
                {
                    ((Storyboard)this.Resources["mostrarDormir"]).Begin();
                }
                else
                {
                    ((Storyboard)this.Resources["mostrarBotones"]).Begin();
                }
                botonesActivos = true;
                cierreBotonesAccion = contadorTiempo;
            }
        }


        private void mostrarLogros(object sender, MouseButtonEventArgs e)
        {
            if (!verLogrosActivo && contadorTiempo >= prox)
            {
                if (barraActiva)
                {
                    ((Storyboard)this.Resources["cerrarBarra"]).Begin();
                    lblFlechaDcha.IsEnabled = false;
                    barraActiva = false;
                }
                ((Storyboard)this.Resources["mostrarLogros"]).Begin();
                btnPuzzle.IsEnabled = false;
                lblFlechaIzda.IsEnabled = false;
                cvVenom.IsEnabled = false;
                verLogrosActivo = true;
                
                
            }
        }

        

        private void ocultarLogros(object sender, MouseButtonEventArgs e)
        {
            if (verLogrosActivo)
            {
                
                ((Storyboard)this.Resources["cerrarLogros"]).Begin();
                btnPuzzle.IsEnabled = true;
                lblFlechaIzda.IsEnabled = true;
                cvVenom.IsEnabled = true;
                verLogrosActivo = false;
            }
        }

       
        private int notificarProximoLogro()
        {
            Logro logro = avatar.logrosParaNotificar[0];
            avatar.logrosParaNotificar.RemoveAt(0);
            int dev = contadorTiempo + 6;
            lblTipoLogro.Content = logro.Descripcion;
        

           ((Storyboard)this.Resources["notificacion_logro"]).Begin();
            desbloquearLogro(logro.Id);
            return dev;
        }

    

        private void abrirPuzzle(object sender, RoutedEventArgs e)
        {
            if (!puzzleAbierto && contadorTiempo >= prox)
            {
                if (barraActiva)
                {
                    ((Storyboard)this.Resources["cerrarBarra"]).Begin();
                    lblFlechaDcha.IsEnabled = false;
                    barraActiva = false;
                }
                restablecerPuzzlePublic();
                border_puzzle.IsEnabled = false;
                cvVenom.IsEnabled = false;
                puzzleAbierto = true;
                btnLogros.IsEnabled = false;
                lblFlechaIzda.IsEnabled = false;
                ((Storyboard)this.Resources["elegirDificultadPuzzle"]).Begin();
                
            }
        }

        private void cerrarPuzzle(object sender, MouseButtonEventArgs e)
        {
            if (puzzleAbierto)
            {
                ((Storyboard)this.Resources["cerrar_puzzle"]).Begin();
                btnLogros.IsEnabled = true;
                lblFlechaIzda.IsEnabled = true;
                cvVenom.IsEnabled = true;
                puzzleAbierto = false;
                finJuego = false;
                lblFin = false;
                jugandoPuzzle = false;
                grid_piezas.IsEnabled = true;
                puzzle.IsEnabled = true;
                restablecerPuzzlePublic();
                border_puzzle.IsEnabled = false;
                puzzleTerminado = false;
            }

        }


      

        private void actualizarMinutero()
        {
            if ((tiempoPuzzle % 60).ToString().Length == 1)
            {
                lbl_reloj.Content = (tiempoPuzzle / 60).ToString() + " : 0" + (tiempoPuzzle % 60);
            }
            else
            {
                lbl_reloj.Content = (tiempoPuzzle / 60).ToString() + " : " + (tiempoPuzzle % 60);
            }
        }

        private void cerrarElegirDificultad(object sender, MouseButtonEventArgs e)
        {
            if (puzzleAbierto)
            {
                btnLogros.IsEnabled = true;
                lblFlechaIzda.IsEnabled = true;
                cvVenom.IsEnabled = true;
                ((Storyboard)this.Resources["ocultarElegirDificultad"]).Begin();
                border_puzzle.IsEnabled = true;
                puzzleAbierto = false;
            }
        }
        


        private void empezarJuego(object sender, RoutedEventArgs e)
        {
            ((Storyboard)this.Resources["ocultarElegirDificultad"]).Begin();
            ((Storyboard)this.Resources["abrir_puzzle"]).Begin();

            prox++;
            tiempoComienzoPuzzle = contadorTiempo + 4;
            if (rdb_dificil.IsChecked == true)
            {
                tiempoPuzzle = 30;
            }
            else
            {
                if (rdb_medio.IsChecked == true)
                {
                    tiempoPuzzle = 60;
                }
                else
                {
                    tiempoPuzzle = 90;
                }
            }
            avatar.nuevoPuzzle();
            jugandoPuzzle = true;
            actualizarMinutero();
        }

        private void restablecerPuzzlePublic()
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

            pieza1.Source = piezaPuzzle.Source;
            pieza1.Stretch = Stretch.Uniform;
            pieza2.Source = piezaPuzzle.Source;
            pieza2.Stretch = Stretch.Uniform;
            pieza3.Source = piezaPuzzle.Source;
            pieza3.Stretch = Stretch.Uniform;
            pieza4.Source = piezaPuzzle.Source;
            pieza4.Stretch = Stretch.Uniform;
            pieza5.Source = piezaPuzzle.Source;
            pieza5.Stretch = Stretch.Uniform;
            pieza6.Source = piezaPuzzle.Source;
            pieza6.Stretch = Stretch.Uniform;
            pieza7.Source = piezaPuzzle.Source;
            pieza7.Stretch = Stretch.Uniform;
            pieza8.Source = piezaPuzzle.Source;
            pieza8.Stretch = Stretch.Uniform;
            pieza9.Source = piezaPuzzle.Source;
            pieza9.Stretch = Stretch.Uniform;
        }

        private void comprobarPuzzle()
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
                puzzleTerminado = true;
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
                        lbl_msg_inicio.Foreground = Brushes.Green;
                        lbl_msg_inicio.Content = "USUARIO CORRECTO";
                        btnPuzzle.IsEnabled = true;
                        btnLogros.IsEnabled = true;
                        cvVenom.IsEnabled = true;
                        ((Storyboard)this.Resources["ocultarAutenticacion"]).Begin();
                        usuario = tb_inicio_usuario.Text.ToString();
                        cargarAvatar();
                        break;
                    case "1":
                        lbl_msg_inicio.Content = "CREDENCIALES INCORRECTOS";
                        break;
                }
            }
            catch (Exception)
            {
                lbl_msg_inicio.Content = "Error al conectar con la base de datos";
            }
        }

        private void animIrRegistro(object sender, RoutedEventArgs e)
        {
            tb_inicio_usuario.Text = "";
            tb_inicio_contrasena.Password = "";
            lbl_msg_inicio.Content = "";
            ((Storyboard)this.Resources["inicio_registro"]).Begin();
        }

        private void animIrAutenticar(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((new Avatar().usuarioExistente(tb_registro_usuario.Text.ToString()))){
                    lbl_msg_registro.Content="EL USUARIO INTRODUCIDO\nYA EXISTE";
                }
                else
                {
               
                new Avatar().registrar(tb_registro_usuario.Text.ToString(), tb_registro_contrasena.Password.ToString());


                ((Storyboard)this.Resources["ocultarRegistro"]).Begin();
                btnPuzzle.IsEnabled = true;
                btnLogros.IsEnabled = true;
                cvVenom.IsEnabled = true;
                MessageBox.Show("Si pulsas sobre el pecho de Venom reproducirá un espeluznante sonido...\n" +
                    "y en cualquier otra parte de su cuerpo, se mostrarán los botones de acción.\n\n¡PRUÉBALO!", "SABÍAS QUE...");
                usuario = tb_registro_usuario.Text.ToString();
                cargarAvatar();

            }
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

        private void volverAutenticar(object sender, RoutedEventArgs e)
        {
            tb_registro_usuario.Text = "";
            tb_registro_contrasena.Password = "";
            lbl_msg_registro.Content = "";
            ((Storyboard)this.Resources["volverAutenticacion"]).Begin();
        }


        private void jugarYoyo(object sender, RoutedEventArgs e)
        {
            if (botonesActivos)
            {
                ((Storyboard)this.Resources["ocultarBotones"]).Begin();
            }
            avatar.proximoEstado(Estado.JUGANDO);
            jugar = Juegos.YOYO;
            avatar.PuntosNivel += 15;
            avatar.Diversion += 19;
            avatar.Energia -= 3;
            botonesActivos = false;

        }


        private void lanzarTela(object sender, RoutedEventArgs e)
        {
            if (botonesActivos)
            {
                ((Storyboard)this.Resources["ocultarBotones"]).Begin();
            }
            avatar.proximoEstado(Estado.JUGANDO);
            jugar = Juegos.TELARAÑA;
            avatar.PuntosNivel += 20;
            avatar.Energia -= 7;
            avatar.Diversion += 25;
            botonesActivos = false;
        }

        private void dormir(object sender, RoutedEventArgs e)
        {
            if (botonesActivos)
            {
                if (avatar.estadoActual == Estado.DORMIDO || avatar.estadoActual == Estado.SOÑANDO)
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
                    avatar.PuntosNivel += 10;
                    logoVenom.IsEnabled = false;
                    avatar.proximoEstado(Estado.DORMIDO);
                    break;
                case "Despertar":
                    avatar.PuntosNivel += 5;
                    despertar = true;
                    logoVenom.IsEnabled = true;

                    break;
            }
            botonesActivos = false;
        }
    }

   
    

}
