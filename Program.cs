var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRazorPages();

builder.Services.AddScoped<student_online_system.Data.Db>();

builder.Services.AddSession();
builder.Services.AddMemoryCache();

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();      

app.UseAuthorization();

app.MapRazorPages();

app.Run();
