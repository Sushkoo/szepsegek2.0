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
        int foglalsID = 1;
        ObservableCollection<Foglalas> dtgSource = new();
        public MainWindow()
        {
            InitializeComponent();
            LoadFromDB();
            cbxDolgozok.SelectionChanged += cbxDolgozok_SelectionChanged;
            dtgFoglalasok.ItemsSource = dtgSource;
        }

        public void LoadFromDB()
        {

            // Replace with your query
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

        private void btnFoglal_Click(object sender, RoutedEventArgs e)
        {
            const int nyitas = 8;
            const int zaras = 15;
            const int zarasperc = 50;
            int selectedHour = dtudOra.Value.Value.Hour;     
            int selectedMinute = dtudOra.Value.Value.Minute;

            string oraperc = selectedHour.ToString() + ":" + selectedMinute.ToString();

            if (selectedHour<=nyitas || selectedHour>=zaras && selectedMinute >= zarasperc || selectedHour>=zaras)
            {
                System.Windows.MessageBox.Show("Figyeld a nyitvatartast!!!!!!");
                return;
            }
            else
            {
                foreach (var item in dtgSource)
                {
                    if (item.DolgozoID.ToString() == dolgozoID && item.OraPerc == oraperc)
                    {
                        System.Windows.MessageBox.Show("Már van foglalás erre az időpontra!");
                        return;
                    } 
                }

                DateTime? selectedDate = dtpIdopont.SelectedDate;

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

                        System.Windows.MessageBox.Show("Foglalás rögzítve!");
                        Foglalas ujFoglalas = new Foglalas()
                        {
                            FoglalasID = foglalsID,
                            DolgozoID = int.Parse(dolgozoID),
                            SzolgaltatasID = int.Parse(szolgaltatasID),
                            Ido = selectedDateTime.ToString("yyyy-MM-dd"),
                            OraPerc = oraperc 
                        };
                        foglalsID++;
                        dtgSource.Add(ujFoglalas);
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