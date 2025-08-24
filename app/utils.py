from math import radians, sin, cos, sqrt, asin

def calculate_distance(lat1, lon1, lat2, lon2):
    #using Haversine formula to find distance between two points
    R = 6371000 
    lat1, lon1, lat2, lon2 = map(radians, [lat1, lon1, lat2, lon2])
    
    dlat = lat1 -lat2
    dlon = lon1 - lon2
    
    a = sin(dlat / 2)**2 + cos(lat1) * cos(lat2) * sin(dlon / 2)**2
    c = 2 * asin(sqrt(a))
    
    return int(R*c)

def calculate_delivery_fee(distance, base_price, distance_ranges):
    for range in distance_ranges:
        if range["min"] <= distance < range["max"] or ( range["max"] == 0 and distance < range["min"]):
            return base_price + range["a"] + (range["b"] * distance // 10)
        return None
            
            

