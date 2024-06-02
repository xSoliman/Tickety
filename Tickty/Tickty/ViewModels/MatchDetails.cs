using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Tickty.Models;

namespace Tickty.ViewModels;

public class MatchDetails
{
    public int? matchId { get; set; }
    public string? Team1 { get; set; }
    public string? Team2 { get; set; }
    public string? Referee { get; set; }
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan? StartTime { get; set; }
    public string StartTimeFormatted { get; set; } 
    public int? TicketCount { get; set; }
    public string? StadiumName { get; set; }
    public string? StadiumImage { get; set; }
    public string? StadiumLocation { get; set; }
    public string? GmapLocation { get; set; }
    public decimal? LowestTicket { get; set; }

}


