using CosmosDB.Simple.Store.Interfaces;
using dotnetcore.urlshortener.contracts;
using dotnetcore.urlshortener.contracts.Models;
using FluentAssertions;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTest_Shortener
{
    public class UnitTestSimpleCosmosDBStore : IClassFixture<MyTestServerFixture>
    {
        private MyTestServerFixture _fixture;

        public UnitTestSimpleCosmosDBStore(MyTestServerFixture fixture)
        {
            _fixture = fixture;
          
        }
        [Fact]
        public async Task TestMethod_SimpleItemDbContext()
        {
            var store = _fixture.GetService<ISimpleItemDbContext<ShortUrlCosmosDocument>>();
            store.Should().NotBeNull();
        }

        [Fact]
        public async Task TestMethod_SimpleItemDbContext_Insert()
        {
            var store = _fixture.GetService<ISimpleItemDbContext<ShortUrlCosmosDocument>>();
            store.Should().NotBeNull();
            var shortUrl = new ShortUrl
            {
                Expiration = DateTime.UtcNow.AddSeconds(2),
                Id = "1234",
                LongUrl = "https://www.google.com"
            };
            var document = new ShortUrlCosmosDocument(shortUrl.ToShortUrlCosmosDocument());
            var response = await store.UpsertItemAsync(document);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var read = await store.GetItemAsync(document.Id);

            read.Should().NotBeNull();
            read.Id.Should().Be(document.Id);

            // wait 2 seconds.
            Thread.Sleep(2000);
            read = await store.GetItemAsync(document.Id);

            read.Should().BeNull();

        }
    }
}
