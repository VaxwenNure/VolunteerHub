using VolunteerHub.Mvc.Models;

namespace VolunteerHub.Mvc.Services
{
    public interface IJsonStore
    {
        List<Volunteer> GetVolunteers();
        void SaveVolunteers(List<Volunteer> items);

        List<HelpType> GetHelpTypes();
        void SaveHelpTypes(List<HelpType> items);

        List<HelpRecord> GetHelpRecords();
        void SaveHelpRecords(List<HelpRecord> items);
    }
}
