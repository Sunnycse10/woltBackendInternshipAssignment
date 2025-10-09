using Assignment.DTO;
using System.Net.Http.Json;

namespace Api.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test_With_Valid_Input()
        {
            HttpClient client = new HttpClient();
            var response = await client.GetFromJsonAsync<DeliveryInfoDTO>("http://localhost:5176/api/v1/delivery-order-price?venue_slug=home-assignment-venue-helsinki&cart_value=1000&user_lat=60.17094&user_lon=24.93087");
            Assert.NotNull(response);
            Assert.Equal(1190, response.total_price);
        }
    }
}

