
using Microsoft.EntityFrameworkCore;
using TreatyAutomateSystem.Services;
using TreatyAutomateSystem.Helpers;

var builder = WebApplication.CreateBuilder(args);

var conf = builder.Configuration;
builder.Services.AddControllersWithViews();

builder.Services.AddTransient<ManyprofilesTreatyService>(s => 
    new ManyprofilesTreatyService(
        new ManyprofilesTreatyService.Options("Docs", "Docs/Многопрофильный.docx")
    )
);
builder.Services.AddTransient<StudentOneprofileTreatyService>(s => 
    new StudentOneprofileTreatyService(
        new StudentOneprofileTreatyService.Options("Docs", "Docs/Однопрофильный.docx")
    )
);
builder.Services.AddTransient<GroupWithStudentsExcelReader>();
builder.Services.AddScoped<DbService>();
builder.Services.AddScoped<TreateManager>();
builder.Services.AddTransient<PracticeDataExcelReader>();
builder.Services.AddTransient<OrganizationDataParser>();
builder.Services.AddDbContext<TasDbContext>(c => c.UseSqlite("Data source=TAS.db"));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
}

//app.UseHttpsRedirection();

app.Use(async(context, next) =>
{
    try{
        await next.Invoke(); 
    }catch(AppExceptionBase ex)
    {
        context.Response.Headers["Content-Type"] = "text/plain; charset=utf-8";
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync(ex.Message);
    }
    
});

app.UseStaticFiles();



app.UseRouting();

//app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
