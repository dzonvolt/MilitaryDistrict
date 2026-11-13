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
    /// Логика взаимодействия для EditWeaponryWindow.xaml
    /// </summary>
    public partial class EditWeaponryWindow : Window
    {
        public weaponry _weaponry;

        public EditWeaponryWindow(weaponry _weaponry)
        {
            InitializeComponent();
            this.DataContext = _weaponry;
            this._weaponry = _weaponry;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            if (
                string.IsNullOrWhiteSpace(_weaponry.name) ||
                string.IsNullOrWhiteSpace(_weaponry.specs)
            )
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            this.DialogResult = true;
        }
    }
}
