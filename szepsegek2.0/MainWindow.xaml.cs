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
        public MainWindow()
        {
            InitializeComponent();
            LoadFromDB();
            cbxDolgozok.SelectionChanged += cbxDolgozok_SelectionChanged;
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
            string querySzolgaltatas = "SELECT szolgaltatasok.SzolgaltatasID, dolgozok.DolgozoID FROM szolgaltatasok INNER JOIN dolgozok ON dolgozok.SzolgaltatasID = szolgaltatasok.SzolgaltatasID WHERE dolgozok.DolgozoKeresztNev = @selectedValue";
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

            /*string queryDolgozoID = "SElECT dolgozok.DolgozoID from dolgozok WHERE DolgozoKeresztNev = @selectedValue";
            MySqlConnection connectionDolgozoID = new MySqlConnection(connectionString);
            connectionDolgozoID.Open();
            MySqlCommand commandDolgozoID = new MySqlCommand(queryDolgozoID, connectionDolgozoID);
            MySqlDataReader readerDolgozoID = commandDolgozoID.ExecuteReader();
            while (readerDolgozoID.Read())
            {
                dolgozoID = readerDolgozoID["DolgozoID"].ToString();
            }
            readerDolgozoID.Close();
            connectionDolgozoID.Close();*/
        }

        private void btnFoglal_Click(object sender, RoutedEventArgs e)
        {
            DateTime? selectedDate = dtpIdopont.SelectedDate;
  
            if (selectedDate.HasValue)
            {
                if (cbxDolgozok.SelectedItem == null || cbxSzolgaltatasok.SelectedItem == null || selectedDate.Value.Date < DateTime.Today)
                {
                    MessageBox.Show("A válaszott időpont nem lehet a múltban, válaszd ki a munkádosat/szolgáltatást");
                }
                else
                {
                    DateTime selectedDateTime = dtpIdopont.SelectedDate.Value;

                    MySqlConnection connection = new MySqlConnection(connectionString);
                    connection.Open();

                    MySqlCommand command = new MySqlCommand("INSERT INTO foglalas (SzolgaltatasID, DolgozoID, Ido) VALUES (@szolgaltatasID, @dolgozoID, @SelectedDateTime)", connection);

                    command.Parameters.AddWithValue("@szolgaltatasID", int.Parse(szolgaltatasID));
                    command.Parameters.AddWithValue("@dolgozoID", int.Parse(dolgozoID));
                    command.Parameters.AddWithValue("@SelectedDateTime", selectedDateTime);

                    command.ExecuteNonQuery();

                    connection.Close();

                    MessageBox.Show("Foglalás rögzítve!");
                }
            }
            else
            {
                MessageBox.Show("Válaszd ki az időpontot.");
            }

            if (cbxDolgozok == null)
            {
                MessageBox.Show("Válaszd ki a munkádosat!");
            }

            if (cbxSzolgaltatasok == null)
            {
                MessageBox.Show("Válaszd ki a szolgáltatást!");
            }
        }
    }
}