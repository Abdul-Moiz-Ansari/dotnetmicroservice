using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
if(builder.Environment.IsProduction()){
    Console.WriteLine("--> Using SQL Server db");
    builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn")));
}
else{
    Console.WriteLine("--> Using InMem db");
    builder.Services
        .AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
}
builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();

builder.Services.AddHttpClient<ICommandDataClient,HttpCommandDataClient>();

builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMvc(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

PrepDb.PrepPopulation(app);

app.Run();

//using (var httpClient = new HttpClient())
//{
//    var organizationService = new OrganizationService(httpClient);

//    try
//    {
//        var organizationSummaries = await organizationService.GetOrganizationsSummary();

//        // Output or return organization summaries as needed
//        foreach (var summary in organizationSummaries)
//        {
//            Console.WriteLine($"Organization Id: {summary.Id}");
//            Console.WriteLine($"Organization Name: {summary.Name}");
//            Console.WriteLine($"blacklistTotal: {summary.BlacklistTotal}");
//            Console.WriteLine($"Total Phones: {summary.TotalCount}");

//            foreach (var userSummary in summary.Users)
//            {
//                Console.WriteLine($"-- User Id: {userSummary.Id}");
//                Console.WriteLine($"-- User Email: {userSummary.Email}");
//                Console.WriteLine($"-- Phone Count: {userSummary.PhoneCount}");
//            }
//            Console.WriteLine();
//        }
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine($"An error occurred: {ex.Message}");
//    }
//}