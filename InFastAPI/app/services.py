import requests
from app.utils import calculate_distance,calculate_delivery_fee

HOME_ASSIGNMENT_API_BASE = "https://consumer-api.development.dev.woltapi.com/home-assignment-api/v1/venues"


async def fetch_venue_static_data(venue_slug):
    #fetching static information about a venue
    url = f"{HOME_ASSIGNMENT_API_BASE}/{venue_slug}/static"
    response = requests.get(url)
    if response.status_code == 200:
        static_data= response.json()
        return static_data
    raise ValueError("Failed to fetch venue static data")


async def fetch_venue_dynamic_data(venue_slug):
    #fetching dynamic information about a venue
    url = f"{HOME_ASSIGNMENT_API_BASE}/{venue_slug}/dynamic"
    response = requests.get(url)

    if response.status_code == 200:
        dynamic_data = response.json()
        return dynamic_data
        
    raise ValueError(f"Failed to fetch venue dynamic data. Status code: {response.status_code}, Response: {response.text}")

    

async def calculate_delivery_order_price(venue_slug, cart_value, user_lat, user_lon):
    static_data = await fetch_venue_static_data(venue_slug)
    dynamic_data = await fetch_venue_dynamic_data(venue_slug)
    venue_coordinates = static_data["venue_raw"]["location"]["coordinates"]
    #setting longitude and latitude from venue coordinates
    venue_lon, venue_lat = venue_coordinates
    #calculating distance between user and venue
    distance = calculate_distance(venue_lat, venue_lon, user_lat, user_lon)

    min_cart_value = dynamic_data["venue_raw"]["delivery_specs"]["order_minimum_no_surcharge"]
    base_price = dynamic_data["venue_raw"]["delivery_specs"]["delivery_pricing"]["base_price"]
    distance_ranges = dynamic_data["venue_raw"]["delivery_specs"]["delivery_pricing"]["distance_ranges"]
    surcharge = max(0, min_cart_value - cart_value)
    #calculating delivery fee
    delivery_fee = calculate_delivery_fee(distance, base_price, distance_ranges)
    
    if delivery_fee is None:
        raise ValueError("Delivery not available for this distance")
    
    total_price = cart_value + surcharge + delivery_fee
    
    return {
        "total_price": total_price,
        "small_order_surcharge": surcharge,
        "cart_value": cart_value,
        "delivery": {
            "fee": delivery_fee,
            "distance": distance
        }
    }
        
        