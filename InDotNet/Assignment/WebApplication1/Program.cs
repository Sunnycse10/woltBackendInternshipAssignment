using Assignment;
using Assignment.DTO;
using System;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient<IDeliveryService, DeliveryService>();
var app = builder.Build();


app.MapGet("/api/v1/delivery-order-price", GetDeliverOrderPriceAsync );

async Task<IResult> GetDeliverOrderPriceAsync(string venue_slug, int cart_value , double user_lat , double user_lon , IDeliveryService service)
{
    //Console.WriteLine($"{venue_slug} {cart_value} {user_lat} {user_lon}");
    try
    {

        DeliveryInfoDTO delivery = await service.CalculateDeliveryFeeAsync(venue_slug, cart_value, user_lat, user_lon);

        return TypedResults.Ok(delivery);
    }
    catch (Exception ex)
    {
        return TypedResults.BadRequest(new { error = ex.Message });
    }
    //return TypedResults.Ok();
}

app.Run();
