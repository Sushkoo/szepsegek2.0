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
            string query = "SELECT DISTINCT DolgozoKeresztNev FROM dolgozok";

            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                cbxDolgozo.Items.Add(reader["DolgozoKeresztNev"].ToString());
            }

            reader.Close();
            connection.Close();
        }

        private void btnFoglal_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}