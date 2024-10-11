using MySqlConnector;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using Xceed.Wpf.Toolkit;
using System.Reflection.PortableExecutable;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace szepsegek2._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static string connectionString = "Server=localhost; Database=szepsegek2; UserId=root; Password=; Allow User Variables=true";
        string dolgozoID;
        string szolgaltatasID;
        ObservableCollection<Foglalas> dtgSource = new();
        public MainWindow()
        {
            InitializeComponent();

            string queryDolgozo = "SELECT DISTINCT DolgozoKeresztNev, DolgozoID FROM dolgozok";

            MySqlConnection connectionDolgozo = new MySqlConnection(connectionString);
            connectionDolgozo.Open();
            MySqlCommand commandDolgozo = new MySqlCommand(queryDolgozo, connectionDolgozo);
            MySqlDataReader readerDolgozo = commandDolgozo.ExecuteReader();

            while (readerDolgozo.Read())
            {
                cbxDolgozok.Items.Add(readerDolgozo["DolgozoKeresztNev"].ToString());
            }
            readerDolgozo.Close();
            connectionDolgozo.Close();

            cbxDolgozok.SelectionChanged += cbxDolgozok_SelectionChanged;
            cbxSzolgaltatasok.SelectionChanged += cbxSzolgaltatasok_SelectionChanged;

            MySqlConnection connectionDatagrid = new MySqlConnection(connectionString);
            connectionDatagrid.Open();

            string queryDatagrid = "SELECT foglalasok.FoglalasID, foglalasok.Ido, foglalasok.OraPerc, dolgozok.DolgozoKeresztNev, szolgaltatasok.SzolgaltatasKategoria FROM foglalasok INNER JOIN dolgozok ON dolgozok.DolgozoID = foglalasok.DolgozoID INNER JOIN szolgaltatasok ON szolgaltatasok.SzolgaltatasID = foglalasok.SzolgaltatasID";
            MySqlCommand commandDatagrid = new MySqlCommand(queryDatagrid, connectionDatagrid);
            MySqlDataReader readerDatagrid = commandDatagrid.ExecuteReader();
            Foglalas ujFoglalas = new();

            while (readerDatagrid.Read())
            {
                ujFoglalas = new Foglalas()
                {
                    FoglalasID = readerDatagrid.GetInt32("FoglalasID"),
                    DolgozoID = readerDatagrid.GetString("DolgozoKeresztNev"),
                    SzolgaltatasID = readerDatagrid.GetString("SzolgaltatasKategoria"),
                    Ido = readerDatagrid.GetString("Ido"),
                    OraPerc = readerDatagrid.GetString("OraPerc")
                };
                dtgSource.Add(ujFoglalas);
            }
            readerDatagrid.Close();
            connectionDatagrid.Close();

            dtgFoglalasok.ItemsSource = dtgSource;
        }

        private void cbxDolgozok_SelectionChanged(object sender, EventArgs e)
        {
            string selectedValue = cbxDolgozok.SelectedItem.ToString();

            cbxSzolgaltatasok.Items.Clear();
            string querySzolgaltatas = "SELECT szolgaltatasok.SzolgaltatasID, szolgaltatasok.Szolgaltataskategoria, dolgozok.DolgozoID FROM szolgaltatasok INNER JOIN dolgozok ON dolgozok.SzolgaltatasID = szolgaltatasok.SzolgaltatasID WHERE dolgozok.DolgozoKeresztNev = @selectedValue";
            MySqlConnection connectionSzolgaltatas = new MySqlConnection(connectionString);
            connectionSzolgaltatas.Open();
            MySqlCommand commandSzolgaltatas = new MySqlCommand(querySzolgaltatas, connectionSzolgaltatas);
            commandSzolgaltatas.Parameters.AddWithValue("@selectedValue", selectedValue);
            MySqlDataReader readerSzolgaltatas = commandSzolgaltatas.ExecuteReader();

            while (readerSzolgaltatas.Read())
            {
                cbxSzolgaltatasok.Items.Add(readerSzolgaltatas["SzolgaltatasKategoria"].ToString());
                szolgaltatasID = readerSzolgaltatas["SzolgaltatasID"].ToString();
                dolgozoID = readerSzolgaltatas["DolgozoID"].ToString();
            }

            readerSzolgaltatas.Close();
            connectionSzolgaltatas.Close();
        }

        private void cbxSzolgaltatasok_SelectionChanged(object sender, EventArgs e)
        {
            using (MySqlConnection connectionIdotartam = new MySqlConnection(connectionString))
            {
                connectionIdotartam.Open();

                string selectedDolgozo = cbxDolgozok.SelectedItem.ToString();

                MySqlCommand commandIdotartam = new MySqlCommand("SELECT szolgaltatasok.SzolgaltatasIdotartam FROM dolgozok INNER JOIN szolgaltatasok ON dolgozok.SzolgaltatasID = szolgaltatasok.SzolgaltatasID WHERE dolgozok.DolgozoKeresztNev = @selectedDolgozo", connectionIdotartam);
                commandIdotartam.Parameters.AddWithValue("@selectedDolgozo", selectedDolgozo);
                MySqlDataReader readerIdotartam = commandIdotartam.ExecuteReader();
                string szolgaltatasIdotartamValue = "";
                while (readerIdotartam.Read())
                {
                    szolgaltatasIdotartamValue = readerIdotartam["SzolgaltatasIdotartam"].ToString();
                }

                lblIdotartam.Content = szolgaltatasIdotartamValue + " perc";
                readerIdotartam.Close();
                connectionIdotartam.Close();
            }
        }

        private void btnFoglal_Click(object sender, RoutedEventArgs e)
        {
            const int nyitas = 8;
            const int zaras = 15;
            const int zarasperc = 50;
            int selectedHour = dtudOra.Value.Value.Hour;
            int selectedMinute = dtudOra.Value.Value.Minute;
            string oraperc = selectedHour.ToString() + ":" + selectedMinute.ToString();

            if (selectedHour < nyitas || selectedHour >= zaras && selectedMinute >= zarasperc || selectedHour >= zaras)
            {
                System.Windows.MessageBox.Show("Figyeld a nyitvatartast!!!!!!");
                return;
            }
            else
            {
                DateTime? selectedDate = dtpIdopont.SelectedDate;

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    MySqlCommand command = new MySqlCommand("SELECT * FROM foglalasok WHERE DolgozoID = @dolgozoID AND Ido = @selectedDate AND OraPerc = @oraperc", connection);
                    command.Parameters.AddWithValue("@dolgozoID", int.Parse(dolgozoID));
                    command.Parameters.AddWithValue("@selectedDate", selectedDate.Value.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@oraperc", oraperc);

                    MySqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        System.Windows.MessageBox.Show("Már van foglalás erre az időpontra!");
                        return;
                    }
                }

                if (selectedDate.HasValue)
                {
                    if (cbxDolgozok.SelectedItem == null || cbxSzolgaltatasok.SelectedItem == null || selectedDate.Value.Date < DateTime.Today)
                    {
                        System.Windows.MessageBox.Show("A válaszott időpont nem lehet a múltban, válaszd ki a munkádosat/szolgáltatást");
                    }
                    else
                    {
                        DateTime selectedDateTime = dtpIdopont.SelectedDate.Value;

                        MySqlConnection connection = new MySqlConnection(connectionString);
                        connection.Open();

                        MySqlCommand command = new MySqlCommand("INSERT INTO foglalasok (SzolgaltatasID, DolgozoID, Ido, OraPerc) VALUES (@szolgaltatasID, @dolgozoID, @SelectedDateTime, @oraperc)", connection);

                        command.Parameters.AddWithValue("@szolgaltatasID", int.Parse(szolgaltatasID));

                        command.Parameters.AddWithValue("@dolgozoID", int.Parse(dolgozoID));
                        command.Parameters.AddWithValue("@SelectedDateTime", selectedDateTime.ToString("yyyy-MM-dd"));
                        command.Parameters.AddWithValue("@oraperc", oraperc);

                        command.ExecuteNonQuery();

                        connection.Close();

                        MySqlConnection connectionDatagrid = new MySqlConnection(connectionString);
                        connectionDatagrid.Open();

                        string queryDatagrid = "SELECT foglalasok.FoglalasID, foglalasok.Ido, foglalasok.OraPerc, dolgozok.DolgozoKeresztNev, szolgaltatasok.SzolgaltatasKategoria FROM foglalasok INNER JOIN dolgozok ON dolgozok.DolgozoID = foglalasok.DolgozoID INNER JOIN szolgaltatasok ON szolgaltatasok.SzolgaltatasID = foglalasok.SzolgaltatasID";
                        MySqlCommand commandDatagrid = new MySqlCommand(queryDatagrid, connectionDatagrid);
                        MySqlDataReader readerDatagrid = commandDatagrid.ExecuteReader();
                        Foglalas ujFoglalas = new();

                        while (readerDatagrid.Read())
                        {
                            ujFoglalas = new Foglalas()
                            {
                                FoglalasID = readerDatagrid.GetInt32("FoglalasID"),
                                DolgozoID = readerDatagrid.GetString("DolgozoKeresztNev"),
                                SzolgaltatasID = readerDatagrid.GetString("SzolgaltatasKategoria"),
                                Ido = readerDatagrid.GetString("Ido"),
                                OraPerc = readerDatagrid.GetString("OraPerc")
                            };
                        }
                        dtgSource.Add(ujFoglalas);
                        readerDatagrid.Close();
                        connectionDatagrid.Close();
                        System.Windows.MessageBox.Show("Foglalás rögzítve!");
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("Válaszd ki az időpontot.");
                }

                if (cbxDolgozok == null)
                {
                    System.Windows.MessageBox.Show("Válaszd ki a munkádosat!");
                }

                if (cbxSzolgaltatasok == null)
                {
                    System.Windows.MessageBox.Show("Válaszd ki a szolgáltatást!");
                }
            }
        }
        private void dtpIdopont_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            dtudOra.Visibility = Visibility.Visible;
        }
    }
}