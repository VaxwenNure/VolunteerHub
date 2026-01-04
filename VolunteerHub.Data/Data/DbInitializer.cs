using VolunteerHub.Data.Models;

namespace VolunteerHub.Data.Data;

public static class DbInitializer
{
    public static void Seed(VolunteerHubContext db)
    {
        if (!db.Volunteers.Any())
        {
            db.Volunteers.AddRange(
                new Volunteer { FullName = "Іван Петренко", Phone = "0990000001" },
                new Volunteer { FullName = "Олена Коваль", Phone = "0990000002" }
            );
        }

        if (!db.HelpTypes.Any())
        {
            db.HelpTypes.AddRange(
                new HelpType { Name = "Money" },
                new HelpType { Name = "Food" },
                new HelpType { Name = "Medicine" },
                new HelpType { Name = "Clothes" }
            );
        }

        db.SaveChanges();

        // Add a demo help record once
        if (!db.VolunteerHelps.Any())
        {
            var v1 = db.Volunteers.OrderBy(x => x.VolunteerId).First();
            var t1 = db.HelpTypes.OrderBy(x => x.HelpTypeId).First();

            db.VolunteerHelps.Add(new VolunteerHelp
            {
                VolunteerId = v1.VolunteerId,
                HelpTypeId = t1.HelpTypeId,
                HelpDate = DateTime.Today,
                Amount = 500,
                Description = "Initial demo record"
            });

            db.SaveChanges();
        }
    }
}
