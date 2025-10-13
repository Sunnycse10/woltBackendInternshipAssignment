using Assignment.DTO;
using Microsoft.AspNetCore.Mvc.Testing;
//using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http.Json;

namespace Api.Tests
{
    public class IntegrationTest:IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient client;
        private readonly string urlTemplate = "api/v1/delivery-order-price?venue_slug={0}&cart_value={1}&user_lat={2}&user_lon={3}";
        public IntegrationTest(WebApplicationFactory<Program> factory)
        {
            client=factory.CreateClient();
        }
        [Fact]
        public async Task Test_With_Valid_Input()
        {
            string venue_slug = "home-assignment-venue-helsinki";
            int cart_value = 1000;
            double user_lat = 60.17094;
            double user_lon = 24.93087;
            string url = string.Format(urlTemplate,venue_slug,cart_value,user_lat,user_lon);
            var response = await client.GetFromJsonAsync<DeliveryInfoDTO>(url);
            Assert.NotNull(response);
            Assert.Equal(1190, response.total_price);
        }
        [Fact]
        public async Task Test_In_A_Different_Venue()
        {
            string venue_slug = "home-assignment-venue-stockholm";
            int cart_value = 1000;
            double user_lat = 59.35990918202689;
            double user_lon = 18.01320527555328;
            string url = string.Format(urlTemplate, venue_slug, cart_value, user_lat, user_lon);
            var response = await client.GetFromJsonAsync<DeliveryInfoDTO>(url);
            Assert.NotNull(response);
            Assert.Equal(14698, response.total_price);
        }
    }
}

