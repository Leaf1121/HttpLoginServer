using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.DateBase
{
    public class UsersContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseMySql("Server=127.0.0.1;User=root;Database=httpserver;Password=root;", ServerVersion.Create(new Version("12.3.0"), Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MariaDb));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("ytf8mb4_general_ci")
                .HasCharSet("utf8mb4");
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
                entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
                entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            });
        }
    }
    public class User
    {
        [Key]
        public int? Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
