namespace VolunteerHub.Mvc.Services
{
    public class ReportService
    {
        private readonly IJsonStore _store;
        public ReportService(IJsonStore store) => _store = store;

        public List<TotalByVolunteerRow> TotalsByVolunteer(DateTime? from = null, DateTime? to = null)
        {
            var records = _store.GetHelpRecords();

            if (from.HasValue) records = records.Where(r => r.Date.Date >= from.Value.Date).ToList();
            if (to.HasValue) records = records.Where(r => r.Date.Date <= to.Value.Date).ToList();

            var volunteers = _store.GetVolunteers().ToDictionary(v => v.Id, v => v.FullName);

            var result = records
                .GroupBy(r => r.VolunteerId)
                .Select(g => new TotalByVolunteerRow
                {
                    VolunteerId = g.Key,
                    VolunteerName = volunteers.TryGetValue(g.Key, out var name) ? name : $"#{g.Key}",
                    RecordsCount = g.Count(),
                    TotalAmount = g.Sum(x => x.Amount)
                })
                .OrderByDescending(x => x.TotalAmount)
                .ToList();

            return result;
        }

        public List<TotalByTypeRow> TotalsByHelpType(DateTime? from = null, DateTime? to = null)
        {
            var records = _store.GetHelpRecords();

            if (from.HasValue) records = records.Where(r => r.Date.Date >= from.Value.Date).ToList();
            if (to.HasValue) records = records.Where(r => r.Date.Date <= to.Value.Date).ToList();

            var types = _store.GetHelpTypes().ToDictionary(t => t.Id, t => t.Name);

            var result = records
                .GroupBy(r => r.HelpTypeId)
                .Select(g => new TotalByTypeRow
                {
                    HelpTypeId = g.Key,
                    HelpTypeName = types.TryGetValue(g.Key, out var name) ? name : $"#{g.Key}",
                    RecordsCount = g.Count(),
                    TotalAmount = g.Sum(x => x.Amount)
                })
                .OrderByDescending(x => x.TotalAmount)
                .ToList();

            return result;
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
