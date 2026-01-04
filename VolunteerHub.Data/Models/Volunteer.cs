using System;
using System.Collections.Generic;
using System.Text;

namespace VolunteerHub.Data.Models;

public class Volunteer
{
    public int VolunteerId { get; set; }
    public string FullName { get; set; } = "";
    public string Phone { get; set; } = "";

    public List<VolunteerHelp> VolunteerHelps { get; set; } = new();
}

