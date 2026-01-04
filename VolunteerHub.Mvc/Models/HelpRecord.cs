namespace VolunteerHub.Mvc.Models
{
    public class HelpRecord
    {
        public int Id { get; set; }

        public int VolunteerId { get; set; }
        public int HelpTypeId { get; set; }

        public DateTime Date { get; set; } = DateTime.Today;
        public decimal Amount { get; set; }
        public string Description { get; set; } = "";
    }
}
