using CosmosDB.Simple.Store.Interfaces;
using dotnetcore.urlshortener.contracts;
using dotnetcore.urlshortener.contracts.Models;
using dotnetcore.urlshortener.generator;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTest_Shortener
{

    public class MyHandler
    {
        public ShortenerEventArgs Evt { get; set; }
        public MyHandler()
        {
        }

        public void OnEvent(object sender, ShortenerEventArgs e)
        {
            Evt = e;
        }
    }
    public class UnitTestInMemoryStore : IClassFixture<MyTestServerFixture>
    {
        private MyTestServerFixture _fixture;

        public UnitTestInMemoryStore(MyTestServerFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task TestMethod_CosmosDB_Short()
        {
            var store = _fixture.GetService<ISimpleItemDbContext<ShortUrlCosmosDocument>>();
            store.Should().NotBeNull();

        }
        [Fact]
        public async Task TestMethod_StoreUrl_AddRemoveEventHandler()
        {
            var store = _fixture.GetService<IUrlShortenerService>();
            store.Should().NotBeNull();
            var url = "https://github.com/P7CoreOrg/dotnetcore.urlshortener/tree/dev";

            ShortenerEventArgs evt = null;
            var myHandler = new MyHandler();
            store.AddListenter(myHandler.OnEvent);
            var (code,shortUrl) = await store.UpsertShortUrlAsync("1",new ShortUrl()
            {
                LongUrl = url,
                Expiration = DateTime.UtcNow.AddSeconds(2)

            });
            code.Should().Be(HttpStatusCode.OK);

            myHandler.Evt.Should().NotBeNull();
            myHandler.Evt.EventType.Should().Be(ShortenerEventType.Upsert);
            myHandler.Evt.ShortUrl.Should().NotBeNull();
            myHandler.Evt.ShortUrl.LongUrl.Should().Match(url);
            myHandler.Evt.ShortUrl.Id.Should().NotBeNullOrEmpty();

            shortUrl.LongUrl.Should().Match(url);
            shortUrl.Id.Should().NotBeNullOrEmpty();

            myHandler.Evt = null;
            store.RemoveListenter(myHandler.OnEvent);

            var lookup = await store.GetShortUrlAsync(shortUrl.Id);
            myHandler.Evt.Should().BeNull();

 
        }
    }
}
