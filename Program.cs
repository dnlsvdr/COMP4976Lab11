using AI4SchoolAgent.Components;
using Microsoft.EntityFrameworkCore;
using AI4SchoolAgent.Data;
using AI4SchoolAgent.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContextFactory<SchoolContext>(
    options => options.UseSqlite(
        builder.Configuration.GetConnectionString(
            "DefaultConnection")));

builder.Services.AddScoped<StudentsAgentTools>();
builder.Services.AddScoped<AgentService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var factory = scope.ServiceProvider
        .GetRequiredService<IDbContextFactory<SchoolContext>>();

    using var db = factory.CreateDbContext();

    db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
