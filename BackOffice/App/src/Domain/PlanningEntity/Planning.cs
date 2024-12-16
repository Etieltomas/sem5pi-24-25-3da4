using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class Planning
{
    public int Room { get; set; }
    public int Day { get; set; }
    public int FinOp { get; set; }

    [JsonPropertyName("agenda_room")]
    public List<List<int>> AgendaRoom { get; set; }

    [JsonPropertyName("agenda_doctors")]
    public List<AgendaDoctor> AgendaDoctors { get; set; }
}

public class AgendaDoctor
{
    public string Id { get; set; }

    [JsonPropertyName("slots")]
    public List<List<int>> Slots { get; set; }
}
