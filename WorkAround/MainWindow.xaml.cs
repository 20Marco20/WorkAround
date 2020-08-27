using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Linq;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Xml.Serialization;


namespace WorkAround
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        XmlDocument xmlDocument = new XmlDocument();

        public MainWindow()
        {
            InitializeComponent();

            // Anwendung startet oben, rechts
            this.Top = 0;
            this.Left = SystemParameters.PrimaryScreenWidth - this.Width;

            // Timer für Animation
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += TimerTick;
            timer.Start();


            btnCalender.Content= string.Format("{0:yyyy}", DateTime.Now);
        }

        public static int KW(DateTime Datum)
        {
            CultureInfo cultureInfo = new CultureInfo("de-DE");     // Ländereinstellung

            System.Globalization.Calendar calendar = cultureInfo.Calendar;      // Objekt Kalender aufrufen


            int currentWeek = calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstDay, DayOfWeek.Monday);      // Format festlegen

            return currentWeek;     // Kalenderwoche zurückgeben
        }

        void TimerTick(object sender, EventArgs e)
        {
            tbUhr.Text = DateTime.Now.ToLongTimeString() + " Uhr";
            tbDatum.Text = string.Format("{0:dddd}", DateTime.Now) + ", " + string.Format("{0:dd.MM.yyyy}", DateTime.Now);
            tbKW.Text = "Kalenderwoche: " + KW(DateTime.Now).ToString();    // Kalenderwoche aufrufen
        }

        private void CmdBeenden_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void CmdMinimieren_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void BtnCalender_Click(object sender, RoutedEventArgs e)
        {
            if (btnCalenderOff.Visibility == Visibility.Hidden)
            {
                btnCalenderOff.Visibility = Visibility.Visible;
            }
            else
            {
                btnCalenderOff.Visibility = Visibility.Hidden;  
            }
        }

        public void DatenEinlesen()
        {
            List<TodoItem> items = new List<TodoItem>();

            try
            {
                xmlDocument.Load("WorkAround.xml");
                XmlElement root = xmlDocument.DocumentElement;

                XmlNodeList xmlNode = root.SelectNodes("aufgaben/user");


                foreach (XmlNode node in xmlNode)
                {
                    if (node.Attributes["name"].Value == Environment.UserName)
                    {
                        foreach (XmlNode childnode in node.ChildNodes)
                        {
                            items.Add(new TodoItem() { Title = childnode.InnerText });
                        }

                        listboxAufgaben.ItemsSource = items;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Starteinstellungen()
        {
            listboxAufgaben.Height = 120;
            tbNew.Text = null;
            btnNeueAufgabe.IsEnabled = true;
        }
        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {          
            Starteinstellungen();
            DatenEinlesen();
        }

        private void BtnNeueAufgabe_Click(object sender, RoutedEventArgs e)
        {
            btnNeueAufgabe.IsEnabled = false;
            listboxAufgaben.Height = 85;
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            Starteinstellungen();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DatenEinlesen();
        }

        private void ListboxAufgaben_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            xmlDocument.Load("WorkAround.xml");



            //foreach (XmlNode xNode in xmlDocument.SelectNodes("aufgaben/user name='Marco Forbrig'"))
            //{
            //    if (xNode.SelectSingleNode("aufgabe").InnerText == "Aufgabe 1.2") xNode.ParentNode.RemoveChild(xNode);
            //}




            foreach (object o in listboxAufgaben.SelectedItems)
            {
                if (MessageBox.Show("Aufgabe " + (o as TodoItem).Title + " wirklich löschen?", "Aufgabe entfernen", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    
                }
            }


            xmlDocument.Save("WorkAround.xml");
            DatenEinlesen();
        }
    }

    public class TodoItem
    {
        public string Title { get; set; }
        public int Completition { get; set; }
    }
}