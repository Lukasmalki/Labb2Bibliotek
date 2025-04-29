
using Labb2Bibliotek.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json.Serialization;

namespace Labb2Bibliotek
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

			//Read con string from select appsettings.json
			//builder.Configuration.AddUserSecrets();
			var connectionString = builder.Configuration.GetConnectionString("BooksDb");
			if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Azure")
			{
				//add user secrets when Env is not Development
				builder.Configuration.AddUserSecrets("8733e4b8-c1b7-4799-98b9-0e4e2c7037ad");
				//set the password in the connectionstring from user-secrets
				var connBuilder = new SqlConnectionStringBuilder(connectionString)
				{
					Password = builder.Configuration["DbPassword"]
				};
				connectionString = connBuilder.ConnectionString;
			}
			builder.Services.AddDbContext<ApplicationDbContext>(opt =>
			{
				opt.UseSqlServer(connectionString, sqlOptions =>
					sqlOptions.EnableRetryOnFailure(
						maxRetryCount: 5,
						maxRetryDelay: TimeSpan.FromSeconds(10),
						errorNumbersToAdd: null)
					);
			});



			////builder.Services.AddDbContext<ApplicationDbContext>(opt =>
			////	opt.UseSqlServer(builder.Configuration.GetConnectionString("BooksDb")));

			// Add services to the container.

			builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

			using (var scope = app.Services.CreateScope())
			{
				var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
				db.Database.EnsureDeleted();
				db.Database.EnsureCreated();
			}


			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
