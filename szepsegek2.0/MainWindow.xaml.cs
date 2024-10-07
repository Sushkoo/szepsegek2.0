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
using MySqlConnector;

namespace szepsegek2._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static string connectionString = "Server=localhost; Database=szepsegek2; UserId=root; Password=; Allow User Variables=true";
        MySqlConnection connection = new MySqlConnection(connectionString);
        public MainWindow()
        {
            InitializeComponent();
            connection.Open();
            LoadFromDB();
        }

        public void LoadFromDB()
        {

            // Replace with your query
            string queryDolgozo = "SELECT DISTINCT DolgozoKeresztNev FROM dolgozok";

            MySqlCommand commandDolgozo = new MySqlCommand(queryDolgozo, connection);
            MySqlDataReader readerDolgozo = commandDolgozo.ExecuteReader();

            while (readerDolgozo.Read())
            {
                cbxDolgozo.Items.Add(readerDolgozo["DolgozoKeresztNev"].ToString());
            }

            string querySzolgaltatas = "SELECT szolgaltatasok.SzolgaltatasKategoria FROM szolgaltatasok INNER JOIN szolgaltatasok ON dolgozok.SzolgaltatasID = szolgaltatasok.SzolgaltatasID";

            MySqlCommand commandSzolgaltatas = new MySqlCommand(querySzolgaltatas, connection);
            MySqlDataReader readerSzolgaltatas = commandSzolgaltatas.ExecuteReader();

            while (readerSzolgaltatas.Read())
            {
                cbxSzolgaltatasok.Items.Add(readerSzolgaltatas["SzolgaltatasKategoria"].ToString());
            }

            readerDolgozo.Close();
            readerSzolgaltatas.Close();
            connection.Close();
        }

        private void btnFoglal_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}