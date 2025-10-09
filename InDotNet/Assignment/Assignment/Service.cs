using Assignment.DTO;
using System.Text.Json;

namespace Assignment
{
    public class Ranges
    {
        public int min { get; set; }
        public int max { get; set; }
        public int a {  get; set; }
        public float b { get; set; }
        public int? flag {  get; set; }
    }
   

    public interface IDeliveryService
    {
        Task<DeliveryInfoDTO> CalculateDeliveryFeeAsync(string venue_slug, int cart_value, double user_lat, double user_lon);

    }
    public class DeliveryService : IDeliveryService
    {
        private readonly HttpClient _client;
        private double lon, lat;
        private int minCartValue;
        private int basePrice;
        private List<Ranges> distance_Ranges= new();
        public DeliveryService(HttpClient client)
        {
            _client = client;

        }
        private async Task GetAsyncData(string venue_slug)
        {
            await GetCoordinate(venue_slug);
            await GetDynamicData(venue_slug);
        }
        public async Task<DeliveryInfoDTO> CalculateDeliveryFeeAsync(string venue_slug, int cart_value, double user_lat, double user_lon)
        {

            try
            {
                await GetAsyncData(venue_slug);        
                double distance = GetDistance(user_lon, user_lat, lon, lat);
                int a=0;
                int b=0;
                bool possible = false;
                foreach (var dict in distance_Ranges)
                {
                    if (distance >= dict.min && distance < dict.max)
                    {
                        a = dict.a;
                        b = (int)dict.b;
                        possible = true;
                    }

                }
                if (!possible) throw new Exception("Delivery distance too long!");
                int deliveryFee = basePrice + a + (int)(b * distance / 10);
                int smallOrderSurcharge = Math.Abs(cart_value - minCartValue);
                int total =  smallOrderSurcharge + cart_value + deliveryFee;
                return new DeliveryInfoDTO { total_price =total,small_order_surcharge = smallOrderSurcharge, cart_value= cart_value,delivery= new DeliveryDTO { fee=deliveryFee, distance=(int)distance} }; 


            }
            
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e.Message}");
                throw;
            }

        }


        public double GetDistance(double lon1, double lat1, double lon2, double lat2)
        {
            int R = 6371000;
            var dLat = deg2rad(Math.Abs(lat2 - lat1));  // deg2rad below
            var dLon = deg2rad(Math.Abs(lon2 - lon1));
            var a =
              Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
              Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) *
              Math.Sin(dLon / 2) * Math.Sin(dLon / 2)
              ;
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = R * c;
            return d;

        }

        public async Task GetDynamicData(string venue_slug)
        {
            HttpResponseMessage response = await _client.GetAsync($"{StaticFile.baseURL}{venue_slug}/dynamic");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseBody);
            var root = doc.RootElement;
            var deliverySpecs = root.GetProperty("venue_raw").GetProperty("delivery_specs");
            var rangesJson = deliverySpecs.GetProperty("delivery_pricing").GetProperty("distance_ranges");
            distance_Ranges = JsonSerializer.Deserialize<List<Ranges>>(rangesJson.GetRawText());
            basePrice = deliverySpecs.GetProperty("delivery_pricing").GetProperty("base_price").GetInt32();
            minCartValue = deliverySpecs.GetProperty("order_minimum_no_surcharge").GetInt32();
            Console.WriteLine($"{basePrice} {minCartValue}");

        
        }

        public async Task GetCoordinate(string venue_slug)
        {
            HttpResponseMessage response = await _client.GetAsync($"{StaticFile.baseURL}{venue_slug}/static");
            response.EnsureSuccessStatusCode();     
            string responseBody = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseBody);
            var root = doc.RootElement;
            var coOrdinates = root.GetProperty("venue_raw").GetProperty("location").GetProperty("coordinates");
            lon = coOrdinates[0].GetDouble();
            lat = coOrdinates[1].GetDouble();

        }


        public double deg2rad(double degree)
        {
            return Math.PI / 180 * degree;
        }

    }
}
