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

namespace MilitaryDistrict.forms
{
    /// <summary>
    /// Логика взаимодействия для EditPersonnelWindow.xaml
    /// </summary>
    public partial class EditPersonnelWindow : Window
    {
        public personnel _personnel;

        public EditPersonnelWindow(personnel _personnel)
        {
            InitializeComponent();
            this.DataContext = _personnel;
            this._personnel = _personnel;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            if (
                string.IsNullOrWhiteSpace(_personnel.first_name) ||
                string.IsNullOrWhiteSpace(_personnel.last_name) ||
                string.IsNullOrWhiteSpace(_personnel.patronymic) ||
                string.IsNullOrWhiteSpace(_personnel.rank) ||
                string.IsNullOrWhiteSpace(_personnel.position)
            ) {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_personnel.birth_date > DateTime.Now ||
                _personnel.enlistment_date > DateTime.Now
            )
            {
                MessageBox.Show("Дата рождения и дата призыва не могут быть в будущем.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            this.DialogResult = true;
        }
    }
}
