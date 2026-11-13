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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MilitaryDistrict
{
    /// <summary>
    /// Логика взаимодействия для UnitsPage.xaml
    /// </summary>
    public partial class UnitsPage : Page
    {
        public UnitsPage()
        {
            InitializeComponent();

            var types = MilitaryDistrictEntities.Instance.troop_types.ToList();
            types.Insert(0, new troop_types { name = "Все типы" });
            TypeCB.ItemsSource = types;
            TypeCB.SelectedIndex = 0;

            this.DataContext = this;
        }


        public void Reload()
        {
            this.DataContext = null;
            this.DataContext = this;
        }


        public List<units> Units
        {
            get
            {
                var currentUnits = MilitaryDistrictEntities.Instance.units.ToList();
                if (TypeCB.SelectedIndex > 0)
                    currentUnits = currentUnits.Where(p => p.troop_type_id == (TypeCB.SelectedItem as troop_types).id).ToList();
                if (!string.IsNullOrWhiteSpace(SearchTB.Text))
                    currentUnits = currentUnits.Where(p =>
                        p.locations.FullAddress.ToLower().Contains(SearchTB.Text.ToLower()) ||
                        p.troop_types.name.ToLower().Contains(SearchTB.Text.ToLower())
                    ).ToList();
                switch (SortCB.SelectedIndex)
                {
                    case 1:
                        currentUnits = currentUnits.OrderBy(p => p.companies_count).ToList();
                        break;
                    case 2:
                        currentUnits = currentUnits.OrderByDescending(p => p.companies_count).ToList();
                        break;
                    case 3:
                        currentUnits = currentUnits.OrderBy(p => p.equipment_count).ToList();
                        break;
                    case 4:
                        currentUnits = currentUnits.OrderByDescending(p => p.equipment_count).ToList();
                        break;
                    case 5:
                        currentUnits = currentUnits.OrderBy(p => p.weapons_count).ToList();
                        break;
                    case 6:
                        currentUnits = currentUnits.OrderByDescending(p => p.weapons_count).ToList();
                        break;
                }
                return currentUnits;
            }
        }


        private void TypeCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Reload();

        }

        private void SortCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Reload();
        }

        private void SearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            Reload();
        }


        private void UnitsLV_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var unit = (sender as ListView).SelectedItem as units;
            if (unit == null) return;
            NavigationService.Navigate(new UnitPage(unit));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.UnitsLV.SelectedIndex = -1;
            Reload();
        }
    }
}   
