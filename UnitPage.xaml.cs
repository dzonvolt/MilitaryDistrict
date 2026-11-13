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
    /// Логика взаимодействия для UnitPage.xaml
    /// </summary>
    public partial class UnitPage : Page
    {

        public units unit { get; set; }

        public UnitPage(units unit)
        {
            InitializeComponent();

            this.unit = unit;
            this.DataContext = this;

            Reload();
        }


        public bool SafeUpdate = false;


        public void Reload()
        {

            if (unit == null) return;

            SafeUpdate = true;
            var positions = unit.personnel.Select(p => p.position).Distinct().ToList();
            positions.Insert(0, "Все должности");
            PersonnelPositionCB.ItemsSource = positions;
            if (PersonnelPositionCB.SelectedIndex == -1)
                PersonnelPositionCB.SelectedIndex = 0;

            var ranks = unit.personnel.Select(p => p.rank).Distinct().ToList();
            ranks.Insert(0, "Все звания");
            PersonnelRankCB.ItemsSource = ranks;
            if (PersonnelRankCB.SelectedIndex == -1)
                PersonnelRankCB.SelectedIndex = 0;

            var companies = unit.personnel.Select(p => p.companies).Distinct().ToList();
            companies.Insert(0, new companies { id = -1 });
            PersonnelCompanyCB.ItemsSource = companies;
            if (PersonnelCompanyCB.SelectedIndex == -1)
                PersonnelCompanyCB.SelectedIndex = 0;
            SafeUpdate = false;

            this.DataContext = null;
            this.DataContext = this;
        }




        public Visibility PersonnelVisibility
        {
            get
            {
                return ViewCB.SelectedIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility WeaponryVisibility
        {
            get
            {
                return ViewCB.SelectedIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility EquipmentVisibility
        {
            get
            {
                return ViewCB.SelectedIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility CompaniesVisibility
        {
            get
            {
                return ViewCB.SelectedIndex == 3 ? Visibility.Visible : Visibility.Collapsed;
            }
        }


        public Visibility DeletedComboBoxVisibility
        {
            get
            {
                return (ViewCB.SelectedIndex == 0 || ViewCB.SelectedIndex == 1 || ViewCB.SelectedIndex == 2) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public List<personnel> Personnel
        {
            get
            {
                var items = unit.personnel.ToList();
                if (DeletedCB.SelectedIndex == 1)
                    items = items.Where(p => p.deleted).ToList();
                if (PersonnelCompanyCB.SelectedIndex > 0)
                    items = items.Where(p => p.company_id == (PersonnelCompanyCB.SelectedItem as companies).id).ToList();
                if (!string.IsNullOrEmpty(SearchTB.Text))
                    items = items.Where(p => p.FullName.ToLower().Contains(SearchTB.Text.ToLower())).ToList();
                if (PersonnelPositionCB.SelectedIndex > 0)
                    items = items.Where(p => p.position == (PersonnelPositionCB.SelectedItem as string)).ToList();
                if (PersonnelRankCB.SelectedIndex > 0)
                    items = items.Where(p => p.rank == (PersonnelRankCB.SelectedItem as string)).ToList();
                return items;
            }
        }


        public List<weaponry> Weaponry
        {
            get
            {
                var items = unit.weaponry.ToList();
                if (DeletedCB.SelectedIndex == 1)
                    items = items.Where(w => w.deleted).ToList();
                if (!string.IsNullOrEmpty(SearchTB.Text))
                    items = items.Where(w =>
                        w.name.ToLower().Contains(SearchTB.Text.ToLower()) ||
                        w.specs.ToLower().Contains(SearchTB.Text.ToLower())
                    ).ToList();
                return items;
            }
        }

        public List<equipment> Equipment
        {
            get
            {
                var items = unit.equipment.ToList();
                if (DeletedCB.SelectedIndex == 1)
                    items = items.Where(e => e.deleted).ToList();
                if (!string.IsNullOrEmpty(SearchTB.Text))
                    items = items.Where(e =>
                        e.name.ToLower().Contains(SearchTB.Text.ToLower()) ||
                        e.specs.ToLower().Contains(SearchTB.Text.ToLower())
                    ).ToList();
                return items;
            }
        }


        public List<companies> Companies
        {
            get
            {
                var items = unit.companies.ToList();
                return items;
            }
        }








        public List<troop_types> TroopTypes
        {
            get
            {
                return MilitaryDistrictEntities.Instance.troop_types.ToList();
            }
        }



        private void ViewCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SearchTB.Text = "";
            Reload();
        }

        private void SearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            Reload();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MilitaryDistrictEntities.Instance.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении изменений: " + ex.Message);
                return;
            }
            MessageBox.Show("Изменения успешно сохранены");
            if (App.Current.MainWindow is MainWindow)
                (App.Current.MainWindow as MainWindow).MainFrame.GoBack();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите удалить воинскую часть?", "Подтверждение", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;
            try
            {
                MilitaryDistrictEntities.Instance.units.Remove(unit);
                MilitaryDistrictEntities.Instance.SaveChanges();
                if (App.Current.MainWindow is MainWindow)
                    (App.Current.MainWindow as MainWindow).MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при удалении воинской части: " + ex.Message);
                return;
            }
            MessageBox.Show("Воинская часть успешно удалена");
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Reload();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            MilitaryDistrictEntities.RecreateInstance();
        }

        private void AddPersonnelButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new forms.EditPersonnelWindow(new personnel() { companies = unit.companies.FirstOrDefault(), birth_date = DateTime.Now, enlistment_date = DateTime.Now });
            if (window.ShowDialog() != true) return;
            try
            {
                MilitaryDistrictEntities.Instance.personnel.Add(window._personnel);
                MilitaryDistrictEntities.Instance.SaveChanges();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении персонала: " + ex.Message);
                return;
            }

            Reload();
            MessageBox.Show("Персонал успешно добавлен");
        }

        private void EditPersonnelMenuItem_Click(object sender, RoutedEventArgs e)
        {
            personnel p = PersonnelListView.SelectedItem as personnel;
            var window = new forms.EditPersonnelWindow(p);
            if (window.ShowDialog() != true) return;
            try
            {
                MilitaryDistrictEntities.Instance.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при редактировании персонала: " + ex.Message);
                return;
            }

            Reload();
            MessageBox.Show("Персонал успешно отредактирован");
        }

        private void DeletePersonnelMenuItem_Click(object sender, RoutedEventArgs e)
        {
            personnel p = PersonnelListView.SelectedItem as personnel;
            if (MessageBox.Show("Вы уверены, что хотите удалить элемент?", "Подтверждение", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;
            try
            {
                //MilitaryDistrictEntities.Instance.personnel.Remove(p);
                p.deleted = true;
                MilitaryDistrictEntities.Instance.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при удалении персонала: " + ex.Message);
                return;
            }
            Reload();
            MessageBox.Show("Персонал успешно удален");
        }










        private void AddWeaponryButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new forms.EditWeaponryWindow(new weaponry() { units = unit });
            if (window.ShowDialog() != true) return;
            try
            {
                MilitaryDistrictEntities.Instance.weaponry.Add(window._weaponry);
                MilitaryDistrictEntities.Instance.SaveChanges();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении вооружения: " + ex.Message);
                return;
            }

            Reload();
            MessageBox.Show("Вооружение успешно добавлено");
        }

        private void EditWeaponryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            weaponry w = WeaponryListView.SelectedItem as weaponry;
            var window = new forms.EditWeaponryWindow(w);
            if (window.ShowDialog() != true) return;
            try
            {
                MilitaryDistrictEntities.Instance.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при редактировании вооружения: " + ex.Message);
                return;
            }

            Reload();
            MessageBox.Show("Вооружение успешно отредактировано");
        }

        private void DeleteWeaponryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            weaponry w = WeaponryListView.SelectedItem as weaponry;
            if (MessageBox.Show("Вы уверены, что хотите удалить элемент?", "Подтверждение", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;
            try
            {
                //MilitaryDistrictEntities.Instance.weaponry.Remove(w);
                w.deleted = true;
                MilitaryDistrictEntities.Instance.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при удалении вооружения: " + ex.Message);
                return;
            }
            Reload();
            MessageBox.Show("Вооружение успешно удалено");
        }








        private void AddEquipmentButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new forms.EditEquipmentWindow(new equipment() { units = unit });
            if (window.ShowDialog() != true) return;
            try
            {
                MilitaryDistrictEntities.Instance.equipment.Add(window._equipment);
                MilitaryDistrictEntities.Instance.SaveChanges();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении снаряжения: " + ex.Message);
                return;
            }

            Reload();
            MessageBox.Show("Снаряжение успешно добавлено");
        }


        private void EditEquipmentMenuItem_Click(object sender, RoutedEventArgs e)
        {
            equipment eq = EquipmentListView.SelectedItem as equipment;
            var window = new forms.EditEquipmentWindow(eq);
            if (window.ShowDialog() != true) return;
            try
            {
                MilitaryDistrictEntities.Instance.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при редактировании снаряжения: " + ex.Message);
                return;
            }

            Reload();
            MessageBox.Show("Снаряжение успешно отредактировано");
        }

        private void DeleteEquipmentMenuItem_Click(object sender, RoutedEventArgs e)
        {
            equipment eq = EquipmentListView.SelectedItem as equipment;
            if (MessageBox.Show("Вы уверены, что хотите удалить элемент?", "Подтверждение", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;
            try
            {
                //MilitaryDistrictEntities.Instance.equipment.Remove(eq);
                eq.deleted = true;
                MilitaryDistrictEntities.Instance.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при удалении снаряжения: " + ex.Message);
                return;
            }
            Reload();
            MessageBox.Show("Снаряжение успешно удалено");
        }

        private void AddCompanyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                unit.companies.Add(new companies() { units = unit });
                MilitaryDistrictEntities.Instance.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении роты: " + ex.Message);
                return;
            }

            Reload();
            MessageBox.Show("Рота успешно добавлена");
        }

        private void DeleteCompanyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            companies c = CompaniesListView.SelectedItem as companies;
            if (MessageBox.Show("Вы уверены, что хотите удалить роту? Весь персонал будет переведён в другую роту.", "Подтверждение", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;
            var dialog = new SelectCompanyWindow(unit, c);
            if (dialog.ShowDialog() == false) return;
            companies moveTo = dialog.CompanyCB.SelectedItem as companies;
            try
            {
                foreach (var p in c.personnel)
                    p.company_id = moveTo.id;
                MilitaryDistrictEntities.Instance.companies.Remove(c);
                MilitaryDistrictEntities.Instance.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при удалении роты: " + ex.Message);
                return;
            }
            Reload();
            MessageBox.Show($"Рота успешно удалена, весь состав переведён в роту №{moveTo.id}");
        }


        private void PrintPersonnelMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ListViewItem item = PersonnelListView.ItemContainerGenerator.ContainerFromItem(PersonnelListView.SelectedItem) as ListViewItem;
            ContentControl printWrapper = new ContentControl
            {
                Content = item.Content,
                ContentTemplate = PersonnelListView.ItemTemplate,
                Width = item.ActualWidth,
                Height = item.ActualHeight,
                Margin = new Thickness(10)
            };
            printWrapper.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            printWrapper.Arrange(new Rect(new Point(0, 0), printWrapper.DesiredSize));
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true) {
                printDialog.PrintVisual(printWrapper, "Печать персонала");
                MessageBox.Show("Данные готовы для печати");
            }
        }

        private void PersonnelPositionCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!SafeUpdate)
                Reload();
        }

        private void PersonnelRankCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!SafeUpdate)
                Reload();
        }

        private void DeletedCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!SafeUpdate)
                Reload();
        }

        private void PersonnelCompanyCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!SafeUpdate)
                Reload();
        }
    }
}
