using System;
using System.Collections.Generic;
using System.Text;
using Volunteer_Hub.Data;
using Microsoft.EntityFrameworkCore;

namespace Volunteer_Hub.Services
{
    public static class ReportService
    {
        // Total amount by volunteer (original processing for lab)
        public static void ShowTotalsByVolunteer()
        {
            using var db = new VolunteerHubContext();

            var totals = db.VolunteerHelps
                .Include(x => x.Volunteer)
                .GroupBy(x => x.Volunteer!.FullName)
                .Select(g => new { Volunteer = g.Key, Total = g.Sum(x => x.Amount) })
                .OrderByDescending(x => x.Total)
                .ToList();

            Console.WriteLine("\n--- Total help by volunteer ---");
            if (totals.Count == 0)
            {
                Console.WriteLine("No help records yet.");
                return;
            }

            foreach (var t in totals)
                Console.WriteLine($"{t.Volunteer}: {t.Total}");
        }
    }
}
