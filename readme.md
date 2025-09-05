
i.

- Name: Sunny Chowdhury
- used technology: fastapi, docker

ii. To run the project, follow the instructions below.

#### The instructions are based on linux (ubuntu) machine.

- Go to project directory and build docker image and then run the container

```sh
docker build -t fastapi-docker-app .
docker run -p 8000:8000 fastapi-docker-app

```

- Access the input parameters at

```sh
http://localhost:8000/docs#/default/get_delivery_order_price_api_v1_delivery_order_price_get
```

- run the testcases

```sh
docker exec -it container_id  env PYTHONPATH=/app pytest tests/
```
