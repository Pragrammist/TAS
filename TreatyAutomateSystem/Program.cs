
using TreatyAutomateSystem.Services;

var builder = WebApplication.CreateBuilder(args);

var conf = builder.Configuration;
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<StudentTreateService.Options>(s => 
    new StudentTreateService.Options {
        FolderPathToSave = "Docs",
        TreatePlatePath = "Docs/treaty_template.docx"
    });
builder.Services.AddTransient<StudentTreateService>();
builder.Services.AddTransient<GroupesExcelParser>();
builder.Services.AddTransient<DbService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();



app.UseRouting();

//app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
