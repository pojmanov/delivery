using DeliveryApp.Api;
using DeliveryApp.Core.Application.UseCases.Commands.AssignCouriersToOrders;
using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using DeliveryApp.Core.Application.UseCases.Commands.MoveCouriers;
using DeliveryApp.Core.Application.UseCases.Queries.GetBusyCouriers;
using DeliveryApp.Core.Application.UseCases.Queries.GetCreatedAndAssignedOrders;
using DeliveryApp.Core.Domain.Services;
using DeliveryApp.Core.Ports;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.QuerySelectors;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Primitives;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Health Checks
builder.Services.AddHealthChecks();

// Cors
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin(); // Не делайте так в проде!
        });
});

// Configuration
builder.Services.ConfigureOptions<SettingsSetup>();
var connectionString = builder.Configuration["CONNECTION_STRING"];

// БД, ORM 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseNpgsql(connectionString,
            sqlOptions => { sqlOptions.MigrationsAssembly("DeliveryApp.Infrastructure"); });
        options.EnableSensitiveDataLogging();
    }
);

// сервисы
builder.Services.AddScoped<IDispatchService, DispatchService>();

// репозитории
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICourierRepository, CourierRepository>();

// QurySelector
builder.Services.AddScoped<IGetBusyCouriersQuerySelector, GetBusyCouriersQuerySelector>();
builder.Services.AddScoped<IGetCreatedAndAssignedOrdersQuerySelector, GetCreatedAndAssignedOrdersQuerySelector>();

// UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Mediator
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

// Commands
builder.Services.AddScoped<IRequestHandler<CreateOrderCommand>, CreateOrderHandler>();
builder.Services.AddScoped<IRequestHandler<MoveCouriersCommand>, MoveCouriersHandler>();
builder.Services.AddScoped<IRequestHandler<AssignCouriersToOrdersCommand>, AssignCouriersToOrdersHandler>();

// Queries
builder.Services.AddScoped<IRequestHandler<GetCreatedAndAssignedOrdersQuery, GetCreatedAndAssignedOrdersResponse>, GetCreatedAndAssignedOrdersHandler>();
builder.Services.AddScoped<IRequestHandler<GetBusyCouriersQuery, GetCouriersResponse>, GetBusyCouriersHandler>();

var app = builder.Build();

// -----------------------------------
// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
    app.UseHsts();

app.UseHealthChecks("/health");
app.UseRouting();

// Apply Migrations
// using (var scope = app.Services.CreateScope())
// {
//     var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//     db.Database.Migrate();
// }

app.Run();