using Microsoft.EntityFrameworkCore;
using VolunteerHub.Data.Data;

namespace VolunteerHub.Mvc.Services
{
    public class ReportService
    {
        private readonly VolunteerHubContext _db;
        public ReportService(VolunteerHubContext db) => _db = db;

        public List<TotalByVolunteerRow> TotalsByVolunteer(DateTime? from = null, DateTime? to = null)
        {
            // Load into memory first, then filter — avoids SQLite decimal/date translation errors
            var records = _db.VolunteerHelps
                             .Include(h => h.Volunteer)
                             .AsEnumerable(); // switch to client-side here

            if (from.HasValue) records = records.Where(r => r.HelpDate.Date >= from.Value.Date);
            if (to.HasValue) records = records.Where(r => r.HelpDate.Date <= to.Value.Date);

            return records
                .GroupBy(r => new { r.VolunteerId, r.Volunteer!.FullName })
                .Select(g => new TotalByVolunteerRow
                {
                    VolunteerId = g.Key.VolunteerId,
                    VolunteerName = g.Key.FullName,
                    RecordsCount = g.Count(),
                    TotalAmount = g.Sum(x => x.Amount)
                })
                .OrderByDescending(x => x.TotalAmount)
                .ToList();
        }

        public List<TotalByTypeRow> TotalsByHelpType(DateTime? from = null, DateTime? to = null)
        {
            var records = _db.VolunteerHelps
                             .Include(h => h.HelpType)
                             .AsEnumerable();

            if (from.HasValue) records = records.Where(r => r.HelpDate.Date >= from.Value.Date);
            if (to.HasValue) records = records.Where(r => r.HelpDate.Date <= to.Value.Date);

            return records
                .GroupBy(r => new { r.HelpTypeId, r.HelpType!.Name })
                .Select(g => new TotalByTypeRow
                {
                    HelpTypeId = g.Key.HelpTypeId,
                    HelpTypeName = g.Key.Name,
                    RecordsCount = g.Count(),
                    TotalAmount = g.Sum(x => x.Amount)
                })
                .OrderByDescending(x => x.TotalAmount)
                .ToList();
        }
    }

    public class TotalByVolunteerRow
    {
        public int VolunteerId { get; set; }
        public string VolunteerName { get; set; } = "";
        public int RecordsCount { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class TotalByTypeRow
    {
        public int HelpTypeId { get; set; }
        public string HelpTypeName { get; set; } = "";
        public int RecordsCount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}