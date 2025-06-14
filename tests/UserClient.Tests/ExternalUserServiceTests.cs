using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using UserClient.Core.Models;
using UserClient.Infrastructure;
using UserClient.Infrastructure.Services;
using Xunit;

namespace UserClient.Tests
{
    public class ExternalUserServiceTests
    {
        private ExternalUserService CreateService(HttpResponseMessage response, ApiSettings apiSettings, CacheSettings cacheSettings)
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var client = new HttpClient(handlerMock.Object);
            var memoryCache = new MemoryCache(new MemoryCacheOptions());

            return new ExternalUserService(
                client,
                Options.Create(apiSettings),
                memoryCache,
                Options.Create(cacheSettings));
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsUser_WhenFound()
        {
            // Arrange
            var json = "{ 'data': { 'id': 1, 'email': 'a@b.com', 'first_name': 'A', 'last_name': 'B', 'avatar': '' } }".Replace("'", "\"");
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            { Content = new StringContent(json) };
            var service = CreateService(
                response,
                new ApiSettings { BaseUrl = "https://test" },
                new CacheSettings { UserCacheDurationInMinutes = 1 });

            // Act
            var user = await service.GetUserByIdAsync(1);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(1, user!.Id);
            Assert.Equal("A", user.First_Name);
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsNull_When404()
        {
            var response = new HttpResponseMessage(HttpStatusCode.NotFound);
            var service = CreateService(
                response,
                new ApiSettings { BaseUrl = "https://test" },
                new CacheSettings { UserCacheDurationInMinutes = 1 });

            var user = await service.GetUserByIdAsync(99);
            Assert.Null(user);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsList_WhenDataPresent()
        {
            var page1 = "{ 'total_pages': 1, 'data': [ { 'id': 1, 'email': 'a@b.com', 'first_name': 'A', 'last_name': 'B', 'avatar': '' } ] }".Replace("'", "\"");
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            { Content = new StringContent(page1) };
            var service = CreateService(
                response,
                new ApiSettings { BaseUrl = "https://test" },
                new CacheSettings { UserCacheDurationInMinutes = 1 });

            var users = await service.GetAllUsersAsync();
            Assert.Single(users);
        }
    }
}
