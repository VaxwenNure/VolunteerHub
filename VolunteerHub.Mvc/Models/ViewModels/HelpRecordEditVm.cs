using Microsoft.AspNetCore.Mvc.Rendering;

namespace VolunteerHub.Mvc.Models.ViewModels
{
    public class HelpRecordEditVm
    {
        public int Id { get; set; }
        public int VolunteerId { get; set; }
        public int HelpTypeId { get; set; }
        public DateTime Date { get; set; } = DateTime.Today;
        public decimal Amount { get; set; }
        public string Description { get; set; } = "";

        public List<SelectListItem> Volunteers { get; set; } = new();
        public List<SelectListItem> HelpTypes { get; set; } = new();
    }
}
