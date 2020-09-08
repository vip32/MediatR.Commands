namespace IntegrationTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Application;
    using Application.Web;
    using Newtonsoft.Json;
    using Xunit;

    public class EndpointTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient client;

        public EndpointTests(CustomWebApplicationFactory<Startup> factory)
        {
            this.client = factory.CreateClient();
        }

        [Fact]
        public async Task UserFindAllQueryTest()
        {
            var response = await this.client.GetAsync("/users").ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<IEnumerable<User>>(stringResponse);

            Assert.True(response.Headers.Contains("X-QueryId"));
            Assert.NotEmpty(response.Headers.GetValues("X-QueryId"));
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains(result, u => u.Id == "aaa");
            Assert.Contains(result, u => u.Id == "bbb");
            //Assert.Equal(result.Id, lastAuthor.Id + 1);
            //Assert.Equal(result.Name, newAuthor.Name);
            //Assert.Equal(result.PluralsightUrl, newAuthor.PluralsightUrl);
            //Assert.Equal(result.TwitterAlias, newAuthor.TwitterAlias);
        }

        [Fact]
        public async Task UserFindByIdQueryTest()
        {
            var response = await this.client.GetAsync("/users/aaa").ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<User>(stringResponse);

            Assert.True(response.Headers.Contains("X-QueryId"));
            Assert.NotEmpty(response.Headers.GetValues("X-QueryId"));
            Assert.NotNull(result);
            Assert.Equal("aaa", result.Id);
        }

        [Fact]
        public async Task UserCreateCommandTest()
        {
            var command = new UserCreateCommand("John", "Doe03");
#pragma warning disable CA2000 // Dispose objects before losing scope
            var response = await this.client.PostAsync("/users", this.CreateJsonContent(command)).ConfigureAwait(false);
#pragma warning restore CA2000 // Dispose objects before losing scope

            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<UserCreateCommandResponse>(stringResponse);

            Assert.True(response.Headers.Contains("Location"));
            Assert.NotEmpty(response.Headers.GetValues("Location"));
            Assert.True(response.Headers.Contains("X-CommandId"));
            Assert.NotEmpty(response.Headers.GetValues("X-CommandId"));
            Assert.Null(result);
        }

        private HttpContent CreateJsonContent(object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

#pragma warning disable IDE0059 // Unnecessary assignment of a value
            // required due to https://github.com/dotnet/aspnetcore/issues/18463
            var contentLenth = content.Headers.ContentLength;
#pragma warning restore IDE0059 // Unnecessary assignment of a value

            return content;
        }
    }
}
