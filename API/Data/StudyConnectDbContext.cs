using Microsoft.EntityFrameworkCore;
using StudyConnect.API.Models;

namespace StudyConnect.API.Data;

public class StudyConnectDbContext : DbContext
{
    public StudyConnectDbContext(DbContextOptions<StudyConnectDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<StudyGroup> StudyGroups { get; set; } = null!;
    public DbSet<GroupMember> GroupMembers { get; set; } = null!;
    public DbSet<SharedResource> SharedResources { get; set; } = null!;
    public DbSet<StudySession> StudySessions { get; set; } = null!;
    public DbSet<GroupTask> GroupTasks { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<GroupMember>()
            .HasIndex(gm => new { gm.GroupId, gm.UserId })
            .IsUnique();

        modelBuilder.Entity<StudyGroup>()
            .HasIndex(sg => sg.InviteCode)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<StudyGroup>()
            .HasOne(sg => sg.CreatedBy)
            .WithMany(u => u.CreatedGroups)
            .HasForeignKey(sg => sg.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<GroupMember>()
            .HasOne(gm => gm.Group)
            .WithMany(g => g.Members)
            .HasForeignKey(gm => gm.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GroupMember>()
            .HasOne(gm => gm.User)
            .WithMany(u => u.GroupMemberships)
            .HasForeignKey(gm => gm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Explicitly map UploadedBy so EF doesn't create a shadow UploadedById column
        modelBuilder.Entity<SharedResource>()
            .HasOne(sr => sr.UploadedBy)
            .WithMany()
            .HasForeignKey(sr => sr.UploadedByUserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SharedResource>()
            .HasOne(sr => sr.Group)
            .WithMany(g => g.Resources)
            .HasForeignKey(sr => sr.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        // Explicitly map CreatedBy on StudySession
        modelBuilder.Entity<StudySession>()
            .HasOne(ss => ss.CreatedBy)
            .WithMany()
            .HasForeignKey(ss => ss.CreatedByUserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<StudySession>()
            .HasOne(ss => ss.Group)
            .WithMany(g => g.Sessions)
            .HasForeignKey(ss => ss.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GroupTask>()
            .HasOne(gt => gt.Group)
            .WithMany(g => g.Tasks)
            .HasForeignKey(gt => gt.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GroupTask>()
            .HasOne(gt => gt.CreatedBy)
            .WithMany()
            .HasForeignKey(gt => gt.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<GroupTask>()
            .HasOne(gt => gt.AssignedTo)
            .WithMany()
            .HasForeignKey(gt => gt.AssignedToUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}