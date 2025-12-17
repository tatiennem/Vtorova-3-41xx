using System.Windows;
using Auto.Views;

namespace Auto
{
    public partial class MainWindow : Window
    {
        private readonly CatalogView catalogView;
        private readonly TestDriveView testDriveView;
        private readonly ReportsView reportsView;

        public MainWindow()
        {
            InitializeComponent();

            catalogView = new CatalogView();
            testDriveView = new TestDriveView();
            reportsView = new ReportsView();
            

            MainContent.Content = catalogView;
        }

        private void BtnCatalog_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = catalogView;
        }

        private void BtnTestDrives_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = testDriveView;
        }

        private void BtnReports_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = reportsView;
        }
    }
}