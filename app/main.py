from fastapi import FastAPI, HTTPException, Query
from app.services import calculate_delivery_order_price

app = FastAPI()

@app.get("/api/v1/delivery-order-price")
async def get_delivery_order_price(venue_slug: str = Query(..., description="Unique identifier for the venue"),
    cart_value: int = Query(..., description="Total cart value in cents"),
    user_lat: float = Query(..., description="User's latitude"),
    user_lon: float = Query(..., description="User's longitude")
    ):
    try:
        result = await calculate_delivery_order_price(venue_slug, cart_value, user_lat, user_lon)
        return result
    except ValueError as e:
        raise HTTPException(status_code=400, detail=str(e))
    

