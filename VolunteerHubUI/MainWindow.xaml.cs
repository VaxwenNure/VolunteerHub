using System.Globalization;
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
using VolunteerHub.Data.Data;
using VolunteerHub.Data.Models;
using Microsoft.EntityFrameworkCore;


namespace VolunteerHubUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow() => InitializeComponent();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using var db = new VolunteerHubContext();
            db.Database.EnsureCreated(); // OK for lab; migrations also possible
            RefreshAll();
            HelpDatePicker.SelectedDate = DateTime.Today;
        }

        private void RefreshAll()
        {
            using var db = new VolunteerHubContext();

            VolunteersGrid.ItemsSource = db.Volunteers.AsNoTracking().OrderBy(x => x.VolunteerId).ToList();
            HelpTypesGrid.ItemsSource = db.HelpTypes.AsNoTracking().OrderBy(x => x.HelpTypeId).ToList();

            HelpGrid.ItemsSource = db.VolunteerHelps
                .AsNoTracking()
                .Include(x => x.Volunteer)
                .Include(x => x.HelpType)
                .OrderByDescending(x => x.HelpDate)
                .ToList();

            VolunteerCombo.ItemsSource = db.Volunteers.AsNoTracking().OrderBy(x => x.FullName).ToList();
            HelpTypeCombo.ItemsSource = db.HelpTypes.AsNoTracking().OrderBy(x => x.Name).ToList();
        }

        // ---- Volunteers CRUD ----
        private void AddVolunteer_Click(object sender, RoutedEventArgs e)
        {
            var name = VolunteerNameBox.Text.Trim();
            var phone = VolunteerPhoneBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name)) { MessageBox.Show("Enter full name"); return; }

            using var db = new VolunteerHubContext();
            db.Volunteers.Add(new Volunteer { FullName = name, Phone = phone });
            db.SaveChanges();
            RefreshAll();
        }

        private void EditVolunteer_Click(object sender, RoutedEventArgs e)
        {
            if (VolunteersGrid.SelectedItem is not Volunteer selected) { MessageBox.Show("Select a volunteer"); return; }

            var name = VolunteerNameBox.Text.Trim();
            var phone = VolunteerPhoneBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name)) { MessageBox.Show("Enter full name"); return; }

            using var db = new VolunteerHubContext();
            var v = db.Volunteers.Find(selected.VolunteerId);
            if (v == null) return;

            v.FullName = name;
            v.Phone = phone;
            db.SaveChanges();
            RefreshAll();
        }

        private void DeleteVolunteer_Click(object sender, RoutedEventArgs e)
        {
            if (VolunteersGrid.SelectedItem is not Volunteer selected) { MessageBox.Show("Select a volunteer"); return; }

            using var db = new VolunteerHubContext();
            var v = db.Volunteers.Find(selected.VolunteerId);
            if (v == null) return;

            // optional: prevent delete if has help records
            var hasRecords = db.VolunteerHelps.Any(x => x.VolunteerId == selected.VolunteerId);
            if (hasRecords) { MessageBox.Show("Cannot delete: volunteer has help records."); return; }

            db.Volunteers.Remove(v);
            db.SaveChanges();
            RefreshAll();
        }

        // ---- HelpTypes CRUD ----
        private void AddHelpType_Click(object sender, RoutedEventArgs e)
        {
            var name = HelpTypeNameBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name)) { MessageBox.Show("Enter type name"); return; }

            using var db = new VolunteerHubContext();
            db.HelpTypes.Add(new HelpType { Name = name });
            db.SaveChanges();
            RefreshAll();
        }

        private void EditHelpType_Click(object sender, RoutedEventArgs e)
        {
            if (HelpTypesGrid.SelectedItem is not HelpType selected) { MessageBox.Show("Select a type"); return; }

            var name = HelpTypeNameBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name)) { MessageBox.Show("Enter type name"); return; }

            using var db = new VolunteerHubContext();
            var t = db.HelpTypes.Find(selected.HelpTypeId);
            if (t == null) return;

            t.Name = name;
            db.SaveChanges();
            RefreshAll();
        }

        private void DeleteHelpType_Click(object sender, RoutedEventArgs e)
        {
            if (HelpTypesGrid.SelectedItem is not HelpType selected) { MessageBox.Show("Select a type"); return; }

            using var db = new VolunteerHubContext();
            var t = db.HelpTypes.Find(selected.HelpTypeId);
            if (t == null) return;

            var hasRecords = db.VolunteerHelps.Any(x => x.HelpTypeId == selected.HelpTypeId);
            if (hasRecords) { MessageBox.Show("Cannot delete: type is used in help records."); return; }

            db.HelpTypes.Remove(t);
            db.SaveChanges();
            RefreshAll();
        }

        // ---- HelpRecords CRUD (MAIN) ----
        private void AddHelpRecord_Click(object sender, RoutedEventArgs e)
        {
            if (VolunteerCombo.SelectedValue is not int volunteerId) { MessageBox.Show("Select volunteer"); return; }
            if (HelpTypeCombo.SelectedValue is not int helpTypeId) { MessageBox.Show("Select help type"); return; }

            if (!decimal.TryParse(AmountBox.Text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var amount))
                if (!decimal.TryParse(AmountBox.Text.Trim(), out amount))
                { MessageBox.Show("Invalid amount"); return; }

            var date = HelpDatePicker.SelectedDate ?? DateTime.Today;
            var desc = DescriptionBox.Text.Trim();

            using var db = new VolunteerHubContext();
            db.VolunteerHelps.Add(new VolunteerHelp
            {
                VolunteerId = volunteerId,
                HelpTypeId = helpTypeId,
                Amount = amount,
                HelpDate = date,
                Description = desc
            });
            db.SaveChanges();
            RefreshAll();
        }

        private void EditHelpRecord_Click(object sender, RoutedEventArgs e)
        {
            if (HelpGrid.SelectedItem is not VolunteerHelp selected) { MessageBox.Show("Select a record"); return; }
            if (VolunteerCombo.SelectedValue is not int volunteerId) { MessageBox.Show("Select volunteer"); return; }
            if (HelpTypeCombo.SelectedValue is not int helpTypeId) { MessageBox.Show("Select help type"); return; }

            if (!decimal.TryParse(AmountBox.Text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var amount))
                if (!decimal.TryParse(AmountBox.Text.Trim(), out amount))
                { MessageBox.Show("Invalid amount"); return; }

            var date = HelpDatePicker.SelectedDate ?? DateTime.Today;
            var desc = DescriptionBox.Text.Trim();

            using var db = new VolunteerHubContext();
            var rec = db.VolunteerHelps.Find(selected.VolunteerHelpId);
            if (rec == null) return;

            rec.VolunteerId = volunteerId;
            rec.HelpTypeId = helpTypeId;
            rec.Amount = amount;
            rec.HelpDate = date;
            rec.Description = desc;

            db.SaveChanges();
            RefreshAll();
        }

        private void DeleteHelpRecord_Click(object sender, RoutedEventArgs e)
        {
            if (HelpGrid.SelectedItem is not VolunteerHelp selected) { MessageBox.Show("Select a record"); return; }

            using var db = new VolunteerHubContext();
            var rec = db.VolunteerHelps.Find(selected.VolunteerHelpId);
            if (rec == null) return;

            db.VolunteerHelps.Remove(rec);
            db.SaveChanges();
            RefreshAll();
        }

        // ---- Report (original LINQ processing) ----
        private void TotalsByVolunteer_Click(object sender, RoutedEventArgs e)
        {
            using var db = new VolunteerHubContext();

            var report = db.VolunteerHelps
                .Include(x => x.Volunteer)
                .GroupBy(x => x.Volunteer!.FullName)
                .Select(g => new { Volunteer = g.Key, TotalAmount = g.Sum(x => x.Amount) })
                .OrderByDescending(x => x.TotalAmount)
                .ToList();

            ReportGrid.ItemsSource = report;
        }
    }
}