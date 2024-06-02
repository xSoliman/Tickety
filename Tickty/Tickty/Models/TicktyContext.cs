using Microsoft.EntityFrameworkCore;
using Tickty.Models;

public class TicktyContext : DbContext
{
    public TicktyContext(DbContextOptions<TicktyContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Match> Matches { get; set; }
    public DbSet<Stadium> Stadiums { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Bill> Bills { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bill>()
        .HasOne(b => b.User)
        .WithMany(u => u.Bills)
        .HasForeignKey(b => b.UserId);

        modelBuilder.Entity<Bill>()
            .HasOne(b => b.Ticket)
            .WithMany(t => t.Bills)
            .HasForeignKey(b => b.TicketId);

        modelBuilder.Entity<Stadium>()
           .Property(s => s.Id)
           .ValueGeneratedOnAdd();


    }
}