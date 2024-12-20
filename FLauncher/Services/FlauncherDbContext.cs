﻿using FLauncher.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.Services
{
        public class FlauncherDbContext : DbContext
        {

        private static DbContextOptions<FlauncherDbContext> _cachedOptions;

        public FlauncherDbContext()
        {
        }

        public FlauncherDbContext(DbContextOptions<FlauncherDbContext> options)
            : base(options)
        {
        }

        public static FlauncherDbContext Create(IMongoDatabase database)
        {
            if (_cachedOptions == null)
            {
                _cachedOptions = new DbContextOptionsBuilder<FlauncherDbContext>()
                    .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
                    .Options;
            }
            return new FlauncherDbContext(_cachedOptions);
        }



        public DbSet<Achivement> Achivements { get; init; }
            public DbSet<Admin> Admins { get; init; }
            public DbSet<Buy> Bills { get; init; }
            public DbSet<Category> Categories { get; init; }
            public DbSet<Download> Downloads { get; init; }
            public DbSet<Friend> Friends { get; init; }
            public DbSet<Game> Games { get; init; }
            public DbSet<Game_Has_Genre> GameHasGenres { get; init; }
            public DbSet<GamePublisher> GamePublishers { get; init; }
            public DbSet<Message> Messages { get; init; }
            public DbSet<Notification> Notifications { get; init; }
            public DbSet<Publish> Publishcations { get; init; }
            public DbSet<Review> Reviews { get; init; }
            public DbSet<UnlockAchivement> UnlockAchivements { get; init; }
            public DbSet<Genre> Genres { get; init; }
            public DbSet<Update> Updates { get; init; }
            public DbSet<User> Users { get; init; }
            public DbSet<Gamer> Gamers { get; init; }

       
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Achivement>();
                modelBuilder.Entity<Admin>();
                modelBuilder.Entity<Buy>();
                modelBuilder.Entity<Category>();
                modelBuilder.Entity<Download>();
                modelBuilder.Entity<Friend>();
                modelBuilder.Entity<Game>();
                modelBuilder.Entity<Game_Has_Genre>();
                modelBuilder.Entity<GamePublisher>();
                modelBuilder.Entity<Message>();
                modelBuilder.Entity<Notification>();
                modelBuilder.Entity<Publish>();
                modelBuilder.Entity<Review>();
                modelBuilder.Entity<UnlockAchivement>();
                modelBuilder.Entity<Genre>();
                modelBuilder.Entity<Update>();
                modelBuilder.Entity<User>().ToCollection("Users");
                modelBuilder.Entity<Gamer>();
            
            }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning));
        }
    }

}

