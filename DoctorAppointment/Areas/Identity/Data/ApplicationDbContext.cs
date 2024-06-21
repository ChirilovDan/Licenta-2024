#nullable disable
using DoctorAppointment.Areas.Identity.Data;
using DoctorAppointment.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DoctorAppointment.Areas.Identity.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<DoctorContactDetails> DoctorContactDetails { get; set; }   
    public DbSet<ScheduleTiming> ScheduleTimings {  get; set; } 
    public DbSet<Specialities> Specialities { get; set; }   
    public DbSet<PatientsOrder> PatientsOrders {  get; set; }   
    public DbSet<CardDetails> CardDetails {  get; set; }
    public DbSet<Prescription> Prescriptions {  get; set; }
    public DbSet<OfferServices> OfferServices {  get; set; }
    public DbSet<PatientsServicesDetails> PatientsServicesDetails {  get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
      
    }
}
