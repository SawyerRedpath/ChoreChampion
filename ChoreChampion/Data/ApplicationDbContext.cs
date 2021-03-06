﻿using System;
using System.Collections.Generic;
using System.Text;
using ChoreChampion.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChoreChampion.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Category { get; set; }
        public DbSet<SubCategory> SubCategory { get; set; }

        public DbSet<Chore> Chore { get; set; }

        public DbSet<ApplicationUser> ApplicationUser { get; set; }
    }
}