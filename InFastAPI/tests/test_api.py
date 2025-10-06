from fastapi.testclient import TestClient
from app.main import app

client = TestClient(app)

def test_delivery_order_price():
    response = client.get(
        "/api/v1/delivery-order-price",
        params={
            "venue_slug": "home-assignment-venue-helsinki",
            "cart_value": 1000,
            "user_lat": 60.17094,
            "user_lon": 24.93087
        }
    )
    assert response.status_code == 200
    data = response.json()
    assert "total_price" in data
    assert "delivery" in data

def test_missing_query_parameters():
    response = client.get("/api/v1/delivery-order-price")
    #should return 422 due to missing query parameters
    assert response.status_code == 422 

def test_invalid_cart_value():
    response = client.get(
        "/api/v1/delivery-order-price",
        params={
            "venue_slug": "home-assignment-venue-helsinki",
            "cart_value": 2000,
            "user_lat": 60.17194,
            "user_lon": 24.93087
        }
    )
    #should return 422 due to invalid cart value
    assert response.status_code == 422 