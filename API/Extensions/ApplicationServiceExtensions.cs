﻿using Application.Activities;
using Application.Core;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Extensions
{
	public static class ApplicationServiceExtensions
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services) {
			services.AddEndpointsApiExplorer();
			services.AddSwaggerGen();
			var dbpath = Path.Join(Directory.GetCurrentDirectory(), "reactivities.db");
			var conn = new SqliteConnection($"Data Source={dbpath}");
			services.AddDbContext<DataContext>(opt =>
			{
				opt.UseSqlite(conn);
			});
			services.AddCors(opt =>
			{
				opt.AddPolicy("CorsPolicy", policy =>
				{
					policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:3000");
				});
			});

			services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(List.Handler).Assembly));
			services.AddAutoMapper(typeof(MappingProfiles).Assembly);
			services.AddFluentValidationAutoValidation();
			services.AddValidatorsFromAssemblyContaining<Create>();

			return services;
		}
	}
}
