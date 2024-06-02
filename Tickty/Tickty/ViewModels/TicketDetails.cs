using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Tickty.Models;

namespace Tickty.ViewModels;

public class TicketDetails
{
    public int? BillId { get; set; }

    public string? Team1 { get; set; }
    public string? Team2 { get; set; }
    public string? Referee { get; set; }
    public string? MatchDescription { get; set; }
    public DateTime MatchDate { get; set; }
    public int MatchId { get; set; }

    public DateTime? BillDate { get; set; }

    public TimeSpan StartTime { get; set; }
    public string? StadiumName { get; set; }
    public string? StadiumLocation { get; set; }
    public string? GmapLocation { get; set; }
    public string? QrCode { get; set; }
    public int? TicketId { get; set; }
    public string TicketType { get; set; }
    public string TicketDescription { get; set; }
    public int? UserId { get; set; }
    public string? UserEmail { get; set; }






}


