using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Converters;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Tafels
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _tafelVan = 0;
        private List<int> _tafelVanList = new List<int> { };
        private int _random = 0;
        private int _randomNieuw = 0;
        private int _factor = 0;
        private int _uitkomst = 0;
        private int _maxAantalOefeningen = 0;
        private int _oefeningNrAct = 0;

        private int _aantalGoed =0;
        private int _aantalFout = 0;

        private string _stringGoed = "";

        private string _stringFout = "";

        private int _tijd = 0;
        private int _tijdMax = 10;

        private bool _bewerkingIsMaal = true;

        private bool _nieuweOefening = false;

        private readonly DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            // Timer
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;

            // comboboxes
            cboxAantalOefeningen.SelectedIndex = 0;
            cboxBewerking.SelectedIndex = 0;

            // max tijd
            tboxTijd.Text = _tijdMax.ToString();
            
            // Andere waarden
            InitWaardes();
        }

        private void cboxAantalOefeningen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _maxAantalOefeningen = (cboxAantalOefeningen.SelectedIndex + 1) * 5;
            InitStackPannel();
        }

        private void NieuweOefening()
        {
            _nieuweOefening = true;
            switch (cboxBewerking.SelectedIndex)
            {
                case 0:

                    _bewerkingIsMaal = true;

                    break;

                case 1:

                    _bewerkingIsMaal = false;

                    break;
                case 2:

                    if (DateTime.Now.Ticks % 2 == 1)
                    {
                        _bewerkingIsMaal = true;
                    }
                    else
                    {
                        _bewerkingIsMaal = false;
                    }

                    break;
                default:
                    break;
            }

            if (_oefeningNrAct < _maxAantalOefeningen)
            {

                int tafelselector = new Random().Next(0, _tafelVanList.Count);
                _tafelVan = _tafelVanList.ElementAt(tafelselector);
                
                do
                {
                    _randomNieuw = new Random().Next(0, 11);
                } while (_randomNieuw == _random);

                _random = _randomNieuw;
                
                if (_bewerkingIsMaal)
                {
                    _factor = _random;
                    lblOperator.Content = "x";
                }
                else
                {
                    _factor = _random * _tafelVan;
                    lblOperator.Content = ":";
                }
                
                _oefeningNrAct += 1;
                lblTafelMaal.Content = _tafelVan.ToString();
                lblRandomMaal.Content = _factor.ToString();
                tboxUitkomst.Text = "";
                tboxUitkomst.Background = new SolidColorBrush(Colors.White);
            }else
            {
                timer.Stop();
                //MessageBox.Show(_stringGoed + System.Environment.NewLine + _stringFout, "Score : " + _aantalGoed + "/" + _maxAantalOefeningen);
                tboxResultatenGoed.Text = _stringGoed;
                tboxResultatenFout.Text = _stringFout;
                Stop();
                

            }

        }
        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            Start();
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        private void tboxUitkomst_TextChanged(object sender, TextChangedEventArgs e)
        {
            //tboxUitkomst.Background = new SolidColorBrush(Colors.White);
        }

        private async void tboxUitkomst_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !string.IsNullOrEmpty(tboxUitkomst.Text) && _tafelVan > 0 && _nieuweOefening)
            { 

                if (int.TryParse(tboxUitkomst.Text, out _uitkomst))
                {
                    // Voorkom dat de Enter-toets andere rare dingen doet in de TextBox
                    e.Handled = true;
                    _nieuweOefening = false;

                    ControleerOefening();

                    await Task.Delay(500);

                    _tijd = 0;
                    NieuweOefening();
                }
                else
                {
                    tboxUitkomst.Text = "";
                }
            }
        }
        

        private void Timer_Tick(object? sender, EventArgs e)
        {
            _tijd += 1;
            pbarTijd.Value = _tijd;
            if (_tijd >= 100)
            {

                if (!int.TryParse(tboxUitkomst.Text, out _uitkomst))
                {
                    _uitkomst = -1;
                }
                    ControleerOefening();
                
                
                _tijd = 0;
                NieuweOefening();
            }
        }

        public void Start()
        {
            InitWaardes();

            if (_tafelVanList.Count == 0)
            {
                //MessageBox.Show("Geen tafel geselecteerd.", "Oeps!",MessageBoxButton.OK,MessageBoxImage.Warning);
                Meldingswindow melding = new Meldingswindow();
                melding.Owner = this; // Zorgt dat hij netjes in het midden van je hoofdvenster opduikt
                melding.ShowDialog();
                return;
            }

            VerbergKnoppenInstellingen();
            btnStart.IsEnabled = false;
            btnStop.IsEnabled = true;
            tboxUitkomst.Focus();

            if (!timer.IsEnabled)
            {
                NieuweOefening();
                timer.Interval = TimeSpan.FromMilliseconds(_tijdMax * 10);
                timer.Start();
            }
            tboxResultatenGoed.Text = _stringGoed;
            tboxResultatenFout.Text = _stringFout;

        }

        public void Stop()
        {
            KleineInitWaardes();
            timer.Stop();
            ToonKnoppenInstellingen();
            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;
        }

        private void VerbergKnoppenInstellingen()
        {
            cbox2.IsEnabled=false;
            cbox3.IsEnabled = false;
            cbox4.IsEnabled = false;
            cbox5.IsEnabled = false;
            cbox6.IsEnabled = false;
            cbox7.IsEnabled = false;
            cbox8.IsEnabled = false;
            cbox9.IsEnabled = false;
            cbox10.IsEnabled = false;
            
            cboxAantalOefeningen.IsEnabled = false;
            cboxBewerking.IsEnabled = false;
            tboxTijd.IsEnabled = false;
        }

        private void ToonKnoppenInstellingen()
        {
            cbox2.IsEnabled = true;
            cbox3.IsEnabled = true;
            cbox4.IsEnabled = true;
            cbox5.IsEnabled = true;
            cbox6.IsEnabled = true;
            cbox7.IsEnabled = true;
            cbox8.IsEnabled = true;
            cbox9.IsEnabled = true;
            cbox10.IsEnabled = true;

            cboxAantalOefeningen.IsEnabled = true;
            cboxBewerking.IsEnabled = true;
            tboxTijd.IsEnabled = true;
        }

        private void InitWaardes()
        {
            _aantalGoed = 0;
            _aantalFout = 0;
            _oefeningNrAct = 0;
            _tijd = 0;

            tboxAantalGoed.Text = _aantalGoed.ToString();
            tboxAantalFout.Text = _aantalFout.ToString();
            tboxUitkomst.Text = "";

            tboxUitkomst.Background = new SolidColorBrush(Colors.White);

            lblRandomMaal.Content = string.Empty;
            pbarTijd.Value = _tijd;

            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;

            _stringGoed = "";
            _stringFout = "";

            tboxResultatenGoed.Text = _stringGoed;
            tboxResultatenFout.Text = _stringFout;

            foreach (Image img in stackPanelVoortgang.Children)
            {
                img.Source = new BitmapImage(new Uri("pack://application:,,,/Afbeeldingen/Init.png", UriKind.Absolute));
            }
        }

        private void KleineInitWaardes()
        {
            _oefeningNrAct = 0;
            _tijd = 0;

            tboxUitkomst.Text = "";

            tboxUitkomst.Background = new SolidColorBrush(Colors.White);

            lblRandomMaal.Content = string.Empty;
            pbarTijd.Value = _tijd;

            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;

        }

        private void tboxTijd_TextChanged(object sender, TextChangedEventArgs e)
        {
            //_tijd = int.Parse(tboxTijd.Text);
        }

        private async void tboxTijd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !string.IsNullOrEmpty(tboxTijd.Text))
            {
                // Voorkom dat de Enter-toets andere rare dingen doet in de TextBox
                e.Handled = true;
                _tijd = 0;
                if (int.TryParse(tboxTijd.Text,out _tijdMax))
                {
                    _tijdMax = int.Parse(tboxTijd.Text);                    
                }
                else
                {
                    _tijdMax = 10;
                    tboxTijd.Text = "10";
                }
                timer.Stop();
                timer.Interval = TimeSpan.FromMilliseconds(_tijdMax * 10);
            }
        }

        private void MaakStringGoed ()
        {
            string oparator = (_bewerkingIsMaal) ?"x":":";
            string toeTeVoegenString = (_factor.ToString() + " " + oparator + " " + _tafelVan.ToString() + " = " + _uitkomst.ToString());
            _stringGoed = (_stringGoed + System.Environment.NewLine + toeTeVoegenString);

        }
        private void MaakStringFout()
        {
            string oparator = (_bewerkingIsMaal) ? "x" : ":";
            string toeTeVoegenString = (_factor.ToString() + " " + oparator + " " + _tafelVan.ToString() + " = " + _uitkomst.ToString());
            _stringFout = (_stringFout + System.Environment.NewLine + toeTeVoegenString);

        }

        private void InitStackPannel()
        {
            stackPanelVoortgang.Children.Clear();

            for (int i = 1; i <= _maxAantalOefeningen; i++)
            {
                Image newImage = new Image
                {
                    Source = new BitmapImage(new Uri("pack://application:,,,/Afbeeldingen/Init.png", UriKind.Absolute)),
                    Width = 25,
                    Height = 25,
                    Margin = new Thickness(0, 0, 5, 0)
                };

                stackPanelVoortgang.Children.Add(newImage);
            }
        }

        private void ControleerOefening()
        {

            Image img = GetImageAt(_oefeningNrAct - 1);

            if (((_uitkomst == _factor * _tafelVan)&&_bewerkingIsMaal)|| ((_uitkomst == _factor/_tafelVan) && !_bewerkingIsMaal))
            {
                tboxUitkomst.Background = new SolidColorBrush(Colors.Green);
                _aantalGoed++;
                tboxAantalGoed.Text = _aantalGoed.ToString();
                MaakStringGoed();
                img.Source = new BitmapImage(new Uri("pack://application:,,,/Afbeeldingen/Goed.png", UriKind.Absolute));
            }
            else
            {
                tboxUitkomst.Background = new SolidColorBrush(Colors.Red);
                _aantalFout++;
                tboxAantalFout.Text = _aantalFout.ToString();
                MaakStringFout();
                img.Source = new BitmapImage(new Uri("pack://application:,,,/Afbeeldingen/Fout.png", UriKind.Absolute));
            }
        }
        public Image GetImageAt(int index)
        {
            // 1. Safety check: Ensure the panel exists and the index is within range
            if (stackPanelVoortgang == null || index < 0 || index >= stackPanelVoortgang.Children.Count)
            {
                return null; // Index out of bounds or panel not initialized
            }

            // 2. Get the element and safely cast it to a Image
            if (stackPanelVoortgang.Children[index] is Image targetImage)
            {
                return targetImage;
            }

            return null; // Element at this index was not a Image
        }

        private void cboxBewerking_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            switch (cboxBewerking.SelectedIndex)
            {
                case 0:

                    lblOperator.Content = "x";

                    break;

                case 1:

                    lblOperator.Content = ":";

                    break;
                case 2:

                    lblOperator.Content = "?";

                    break;
                default:
                    break;
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cbox = (CheckBox)sender;
            int cboxWaarde = 0;
            bool result = int.TryParse(cbox.Content.ToString(), out cboxWaarde);

            if (result)
            {
                if (!_tafelVanList.Contains(cboxWaarde)) 
                {
                    _tafelVanList.Add(cboxWaarde);
                }
            }

        }

        private void CheckBox_UnChecked(object sender, RoutedEventArgs e)
        {
            CheckBox cbox = (CheckBox)sender;
            int cboxWaarde = 0;
            bool result = int.TryParse(cbox.Content.ToString(), out cboxWaarde);

            if (result)
            {
                if (_tafelVanList.Contains(cboxWaarde))
                {
                    _tafelVanList.Remove(cboxWaarde);
                }
            }

        }

        private void BtnAbout_Click(object sender, RoutedEventArgs e)
        {
            Info info = new Info();
            info.Owner = this; // Zorgt dat hij netjes in het midden van je hoofdvenster opduikt
            info.ShowDialog();
        }
    }
}