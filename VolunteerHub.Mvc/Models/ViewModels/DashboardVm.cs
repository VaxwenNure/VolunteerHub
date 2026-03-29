using VolunteerHub.Data.Models;

namespace VolunteerHub.Mvc.Models.ViewModels
{
    public class DashboardVm
    {
        public int TotalVolunteers { get; set; }
        public int TotalHelpTypes { get; set; }
        public int TotalRecords { get; set; }
        public decimal TotalAmount { get; set; }
        public List<VolunteerHelp> RecentRecords { get; set; } = new();
    }
}
