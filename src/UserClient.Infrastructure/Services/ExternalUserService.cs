using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Text.Json;
using UserClient.Core.Interfaces;
using UserClient.Core.Models;

namespace UserClient.Infrastructure.Services;

public class ExternalUserService : IExternalUserService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly IMemoryCache _cache;
    private readonly CacheSettings _cacheSettings;
    private readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web);

    public ExternalUserService(HttpClient httpClient, IOptions<ApiSettings> options, IMemoryCache cache, IOptions<CacheSettings> cacheOptions)
    {
        var settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _httpClient = httpClient;
        _baseUrl = settings.BaseUrl.TrimEnd('/');
        _cache = cache;
        _cacheSettings = cacheOptions.Value;
        if (!string.IsNullOrEmpty(settings.ApiKey))
        {
            _httpClient.DefaultRequestHeaders.Add(
                settings.ApiKeyHeaderName,
                settings.ApiKey
            );
        }
    }
    /// <summary>
    /// Get user details by id.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>User details if successful, or null if not found.</returns>
    public async Task<User?> GetUserByIdAsync(int userId)
    {
        string cacheKey = $"user_{userId}";
        if (_cache.TryGetValue(cacheKey, out var cachedObj) && cachedObj is User cachedUser)
        {
            return cachedUser;
        }

        var response = await _httpClient.GetAsync($"{_baseUrl}/users/{userId}").ConfigureAwait(false);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var wrapper = JsonSerializer.Deserialize<UserWrapper>(content, _serializerOptions);
        var user = wrapper?.Data;

        if (user != null)
        {
            _cache.Set(cacheKey, user, TimeSpan.FromMinutes(_cacheSettings.UserCacheDurationInMinutes));
        }

        return user;
    }

    /// <summary>
    /// Get all users.
    /// </summary>
    /// <returns>List of users if successful, or else an empty list.</returns>
    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        const string cacheKey = "all_users";

        if (_cache.TryGetValue(cacheKey, out var cachedUsersObj) && cachedUsersObj is List<User> cachedUsers && cachedUsers.Count != 0)
        {
            return cachedUsers;
        }

        var allUsers = new List<User>();
        int page = 1;

        while (true)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/users?page={page}").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PaginatedResponse>(content, _serializerOptions);

            if (result?.Data == null || !result.Data.Any())
                break;

            allUsers.AddRange(result.Data);

            if (page >= result.Total_Pages)
                break;

            page++;
        }
        _cache.Set(cacheKey, allUsers, TimeSpan.FromMinutes(_cacheSettings.UserCacheDurationInMinutes));
        return allUsers;
    }

    private class UserWrapper
    {
        public User Data { get; set; }
    }

    private class PaginatedResponse
    {
        public int Page { get; set; }
        public int Total_Pages { get; set; }
        public List<User> Data { get; set; }
    }
}
