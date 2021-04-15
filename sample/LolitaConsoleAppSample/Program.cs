﻿using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using LolitaConsoleAppSample.Models;
using Microsoft.Extensions.Options;

namespace LolitaConsoleAppSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var collection = new ServiceCollection();
            collection.AddDbContext<LolitaContext>(x => 
            {
                var connectionString = "server=localhost;database=somedb;uid=someuser;pwd=somepwd;";
                x.UseMySql(connectionString, serverVersion: ServerVersion.AutoDetect(connectionString));
                x.UseMySqlLolita();
            });
            var services = collection.BuildServiceProvider();
            var db = services.GetRequiredService<LolitaContext>();
            db.Database.EnsureCreated();
            
            var row_updated = db.Articles
                .Where(x => x.Id <= 10)
                .Where(x => x.Title == "Hello World")
                .Where(x => DateTime.Now >= x.Time)
                .SetField(x => x.Title).Prepend("[old] ")
                .SetField(x => x.IsPinned).WithValue(true)
                .Update();
            
            var row_updated2 = db.Articles
                .Where(x => db.Users.Where(y => y.Id %2 == 0).Select(y=>y.Id).Contains(x.Id))
                .Delete();
            Console.WriteLine("Lolita finished...");
            Console.Read();
        }
    }
}
