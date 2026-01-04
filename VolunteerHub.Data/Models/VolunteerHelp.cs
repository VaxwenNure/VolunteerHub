namespace VolunteerHub.Data.Models;

public class VolunteerHelp
{
    public int VolunteerHelpId { get; set; }

    public int VolunteerId { get; set; }
    public Volunteer? Volunteer { get; set; }

    public int HelpTypeId { get; set; }
    public HelpType? HelpType { get; set; }

    public DateTime HelpDate { get; set; } = DateTime.Today;
    public decimal Amount { get; set; }
    public string Description { get; set; } = "";
}
