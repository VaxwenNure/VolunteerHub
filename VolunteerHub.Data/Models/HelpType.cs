using System;
using System.Collections.Generic;
using System.Text;
namespace VolunteerHub.Data.Models;

public class HelpType
{
    public int HelpTypeId { get; set; }
    public string Name { get; set; } = "";

    public List<VolunteerHelp> VolunteerHelps { get; set; } = new();
}