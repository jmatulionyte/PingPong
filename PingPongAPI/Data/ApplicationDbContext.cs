using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PingPongAPI.Models;

namespace PingPongAPI.Data
{
    //IdentityDbContext instead of DB context , for isentity service. uses applicationuser class for handling users
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>  options) : base(options)
        {
        }
        public DbSet<Player> Players { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        //protected override void OnModelUpdating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Match>()
        //        .AfterInsert(trigger => trigger.Action(triggerAction => triggerAction
        //            .Upsert(transaction => new { transaction.UserId },
        //                insertedTransaction => new UserBalance { UserId = transaction.UserId, Balance = insertedTransaction.Sum },
        //                    (insertedTransaction, oldBalance) => new UserBalance { Balance = oldBalance.Balance + insertedTransaction.Sum })));
        //}
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Match>(entry =>
        //    {
        //        entry.ToTable(tb => tb.HasTrigger("SetMatchWins"));
        //    });
        //}

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        }
    }
}

