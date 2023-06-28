using FluentValidation.AspNetCore;
using Route256.Week5.Homework.PriceCalculator.Api.GrpcServices;
using Route256.Week5.Homework.PriceCalculator.Api.GrpcServices.Interceptors;
using Route256.Week5.Homework.PriceCalculator.Api.NamingPolicies;
using Route256.Week5.Homework.PriceCalculator.Api.Options;
using Route256.Week5.Homework.PriceCalculator.BackgroundServices.Extensions;
using Route256.Week5.Homework.PriceCalculator.Bll.Extensions;
using Route256.Week5.Homework.PriceCalculator.Dal.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
    });

services.AddEndpointsApiExplorer();

// add swagger
services.AddSwaggerGen(o =>
{
    o.CustomSchemaIds(x => x.FullName);
});

//add validation
services.AddFluentValidation(conf =>
{
    conf.RegisterValidatorsFromAssembly(typeof(Program).Assembly);
    conf.AutomaticValidationEnabled = true;
});


//add dependencies
services
    .AddBll()
    .AddBackgroundServices(builder.Configuration)
    .AddDalInfrastructure(builder.Configuration)
    .AddDalRepositories()
    .Configure<GrpcDeliveryPriceCalculatorOptions>(
        builder.Configuration.GetSection(GrpcDeliveryPriceCalculatorOptions.SectionName))
    .AddGrpcReflection()
    .AddGrpc(options =>
    {
        options.Interceptors.Add<ExceptionInterceptor>();
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.MigrateUp();
app.MapGrpcService<DeliveryPriceCalculatorService>();
app.MapGrpcReflectionService();
app.Run();
