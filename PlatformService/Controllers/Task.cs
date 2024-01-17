using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

public class OrganizationSummary
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int BlacklistTotal { get; set; }
    public int TotalCount { get; set; }
    public List<UserSummary> Users { get; set; }
}

public class UserSummary
{
    public string Id { get; set; }
    public string Email { get; set; }
    public int PhoneCount { get; set; }
    public bool IsBlacklist { get; set; }
}
public class PhoneSummary
{
    public string Id { get; set; }
    public string PhoneNo { get; set; }
    public bool IsBlacklist { get; set; }
}
public class OrganizationService
{
    private readonly HttpClient _httpClient;

    public OrganizationService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<List<OrganizationSummary>> GetOrganizationsSummary()
    {
        var organizations = await GetOrganizations();
        var organizationSummaries = new List<OrganizationSummary>();
        int count = 0;
        foreach (var organization in organizations)
        {
            count++;
            if (count > 5) break;
            try
            {
                var summary = await GetOrganizationSummary(organization.Id);
                organizationSummaries.Add(summary);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        return organizationSummaries;
    }

    private async Task<List<Organization>> GetOrganizations()
    {
        var response = await _httpClient.GetStringAsync("https://607a0575bd56a60017ba2618.mockapi.io/organization");
        return JsonConvert.DeserializeObject<List<Organization>>(response);
    }

    private async Task<OrganizationSummary> GetOrganizationSummary(string organizationId)
    {
        var organization = await GetOrganization(organizationId);
        var users = await GetUsers(organizationId);

        var summary = new OrganizationSummary
        {
            Id = organization.Id,
            Name = organization.Name,
            BlacklistTotal = 0,
            TotalCount = 0,
            Users = new List<UserSummary>()
        };
        int count = 0;
        foreach (var user in users)
        {
            count++;
            try
            {
                var phones = await GetPhones(organizationId, user.Id);

                //har user ki phoen summary
                var userSummary = new UserSummary
                {
                    Id = user.Id,
                    Email = user.Email == null ? user.Name :user.Email,
                    PhoneCount = phones.Count
                };

                summary.Users.Add(userSummary);
                summary.BlacklistTotal += phones.Count(p=> p.blacklisted);
                summary.TotalCount += phones.Count;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        return summary;
    }
    private async Task<Organization> GetOrganization(string organizationId)
    {
        var response = await _httpClient.GetStringAsync($"https://607a0575bd56a60017ba2618.mockapi.io/organization/{organizationId}");
        return JsonConvert.DeserializeObject<Organization>(response);
    }
    private async Task<List<User>> GetUsers(string organizationId)
    {
        var response = await _httpClient.GetStringAsync($"https://607a0575bd56a60017ba2618.mockapi.io/organization/{organizationId}/users");
        return JsonConvert.DeserializeObject<List<User>>(response);
    }

    private async Task<List<Phone>> GetPhones(string organizationId, string userId)
    {
        var response = await _httpClient.GetStringAsync($"https://607a0575bd56a60017ba2618.mockapi.io/organization/{organizationId}/users/{userId}/phones");
        return JsonConvert.DeserializeObject<List<Phone>>(response);
    }
}

public class Organization
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string CreatedAt { get; set; }
}

public class User
{
    public string Id { get; set; }
    public string OrganizationId { get; set; }
    public string CreatedAt { get; set; }
    public string Name { get; set; }
    public string Avatar { get; set; }
    public string Email { get; set; }
}

public class Phone
{
    public string Id { get; set; }
    public string userId { get; set; }
    public string createdAt { get; set; }
    public string name { get; set; }
    public bool blacklisted { get; set; }
}

//class Program
//{
//    static async Task Main()
//    {
//        using (var httpClient = new HttpClient())
//        {
//            var organizationService = new OrganizationService(httpClient);

//            try
//            {
//                var organizationSummaries = await organizationService.GetOrganizationsSummary();

//                // Output or return organization summaries as needed
//                foreach (var summary in organizationSummaries)
//                {
//                    Console.WriteLine($"Organization Id: {summary.Id}");
//                    Console.WriteLine($"Organization Name: {summary.Name}");
//                    Console.WriteLine($"blacklistTotal: {summary.BlacklistTotal}");
//                    Console.WriteLine($"Total Phones: {summary.TotalCount}");

//                    foreach (var userSummary in summary.Users)
//                    {
//                        Console.WriteLine($"-- User Id: {userSummary.Id}");
//                        Console.WriteLine($"-- User Email: {userSummary.Email}");
//                        Console.WriteLine($"-- Phone Count: {userSummary.PhoneCount}");
//                    }
//                    Console.WriteLine();
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"An error occurred: {ex.Message}");
//            }
//        }
//    }
//}
