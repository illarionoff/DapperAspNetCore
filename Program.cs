using DapperAspNetCore.Context;
using DapperAspNetCore.Contracts;
using DapperAspNetCore.Repository;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<AppDbContext>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
