using Microsoft.EntityFrameworkCore;
using AI4SchoolAgent.Models;

namespace AI4SchoolAgent.Data;

public class SchoolContext : DbContext
{
    public SchoolContext(
        DbContextOptions<SchoolContext> options)
        : base(options)
    {
    }

    public DbSet<Student> Students => Set<Student>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Student>().HasData(
            new Student
            {
                StudentId = 1,
                FirstName = "Ann",
                LastName = "Smith",
                School = "Computing",
                DateOfBirth = new DateTime(2002, 5, 10)
            },
            new Student
            {
                StudentId = 2,
                FirstName = "John",
                LastName = "Smith",
                School = "Medicine",
                DateOfBirth = new DateTime(2001, 3, 15)
            },
            new Student
            {
                StudentId = 3,
                FirstName = "Mary",
                LastName = "Lee",
                School = "Nursing",
                DateOfBirth = new DateTime(2003, 7, 20)
            },
            new Student
            {
                StudentId = 4,
                FirstName = "Tom",
                LastName = "Jones",
                School = "Computing",
                DateOfBirth = new DateTime(2000, 8, 12)
            },
            new Student
            {
                StudentId = 5,
                FirstName = "Ann",
                LastName = "Brown",
                School = "Medicine",
                DateOfBirth = new DateTime(2002, 9, 25)
            }
        );
    }
}