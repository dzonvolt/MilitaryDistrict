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
using System.Windows.Shapes;

namespace MilitaryDistrict
{
    /// <summary>
    /// Логика взаимодействия для SelectCompanyWindow.xaml
    /// </summary>
    public partial class SelectCompanyWindow : Window
    {
        units unit { get; set; }

        public SelectCompanyWindow(units unit, companies company)
        {
            InitializeComponent();
            this.unit = unit;
            CompanyCB.ItemsSource = unit.companies.Where(c => c.id != company.id).ToList();
            CompanyCB.SelectedIndex = unit.companies.Count != 0 ? 0 : -1;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (CompanyCB.SelectedItem == null)
            {
                MessageBox.Show("Нет подходящей роты");
                return;
            }
            this.DialogResult = true;
        }
    }
}
