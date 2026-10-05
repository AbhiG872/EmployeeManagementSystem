using EMS.Entities.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.DataAccess.Data
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Designation> Designations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Employee ↔ Department
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Employee ↔ Designation
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Designation)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DesignationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Designation ↔ Department
            modelBuilder.Entity<Designation>()
                .HasOne(d => d.Department)
                .WithMany(dep => dep.Designations)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Employee ↔ ApplicationUser
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.ApplicationUser)
                .WithOne()
                .HasForeignKey<Employee>(e => e.ApplicationUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Employee ↔ Reporting Manager
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.ReportingManager)
                .WithMany(e => e.TeamMembers)
                .HasForeignKey(e => e.ReportingManagerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
