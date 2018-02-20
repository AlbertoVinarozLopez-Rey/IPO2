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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WpfApp1;

namespace MiAppVenom
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Avatar miAvatar;
        DispatcherTimer temporizador;
        int intervalo;

        public MainWindow()
        {
            InitializeComponent();
            miAvatar = new Avatar(100, 100, 100);
            intervalo = 2000;
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
    }
}
