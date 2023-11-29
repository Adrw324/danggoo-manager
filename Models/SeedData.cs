using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using danggoo_manager.Data;
using System;
using System.Linq;

namespace danggoo_manager.Models;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new dmContext(
            serviceProvider.GetRequiredService<
                DbContextOptions<dmContext>>()))
        {
            // Look for any movies.
            if (context.Record.Any())
            {
                return;   // DB has been seeded
            }
            context.Record.AddRange(
                new Record
                {
                    Table_Num = 1,
                    Date = DateTime.Parse("2023/8/1"),
                    Start = DateTime.Parse("2023/8/1 13:45:00"),
                    End = DateTime.Parse("2023/8/1 14:45:00"),
                    Playtime = 60,
                    Fee = 20,
                },
                new Record
                {
                    Table_Num = 1,
                    Date = DateTime.Parse("2023/8/2"),
                    Start = DateTime.Parse("2023/8/2 13:45:00"),
                    End = DateTime.Parse("2023/8/2 15:00:00"),
                    Playtime = 75,
                    Fee = 30,
                },
                new Record
                {
                    Table_Num = 1,
                    Date = DateTime.Parse("2023/8/3"),
                    Start = DateTime.Parse("2023/8/3 13:45:00"),
                    End = DateTime.Parse("2023/8/3 14:45:00"),
                    Playtime = 60,
                    Fee = 20,
                },
                new Record
                {
                    Table_Num = 2,
                    Date = DateTime.Parse("2023/8/1"),
                    Start = DateTime.Parse("2023/8/1 23:00:00"),
                    End = DateTime.Parse("2023/8/2 00:00:00"),
                    Playtime = 60,
                    Fee = 20,
                },
                new Record
                {
                    Table_Num = 3,
                    Date = DateTime.Parse("2023/8/1"),
                    Start = DateTime.Parse("2023/8/1 21:00:00"),
                    End = DateTime.Parse("2023/8/1 22:00:00"),
                    Playtime = 60,
                    Fee = 20,
                }
                
               
            );
            context.SaveChanges();
        }
    }
}