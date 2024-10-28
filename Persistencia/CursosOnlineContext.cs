using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dominio;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistencia
{
    public class CursosOnlineContext : IdentityDbContext<Usuario>
    {
        //Constructor para realizar la migracion de entidades 
        public CursosOnlineContext(DbContextOptions options) : base(options){
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder){
            //Indicar que tiene una clave primaria compuesta 
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<CursoInstructor>().HasKey(ci => new {ci.InstructorId, ci.CursoId});
        }

        public DbSet<Comentario> Comentario{get;set;}
        public DbSet<Curso> Curso{get;set;}
        public DbSet<CursoInstructor> CursoInstructor{get;set;}
        public DbSet<Instructor> Instructor{get;set;}
        public DbSet<Precio> Precio{get;set;}
    }
}