# Shopping
----
https://bugeto.net
----
[Microservice](#microservice)

[webFront](#webfront)

[IdentityServer4](#IdentityServer4)

[Database](#Database)

[RabbitMQ](#RabbitMQ)

[Serilog](#Serilog)

[docker](#docker)

[PayService](#PayService)

[Manitorting](#Manitorting)

-----
### Microservice
  - ProductService
  - BasketService
  - OrderService (smtp email service kay api elastic email)
  - DiscountService (Grpc)
  - PayService (DDD)
  - ApiGateway (Code New)
  - ApiGatewayAdmin (Code New)

### webFront
  - WebApp
  - WebAppAdmin 

### IdentityServer4 (Product and Order)
  - Clinet
  - Scope
  - Resources
 
### Database:
  - Product --> Postgresql
  - Basket --> Postgresql
  - Order --> Postgresql
  - Disocunt --> Mongo
  - Pay --> Postgresql
  - IdentityServer4 --> Postgresql

### RabbitMQ
  - BasketService (send to order and get update product)
  - OrderService (Get basket and get update product , send to pay ? and get pay)
  - ProductService (send Update Product)
  - PayService

### Serilog
  - Serilog seq (docker)
  - serilog metrics (productservice)
    - "/metrics-text" 

### docker
  - ElasticSearch (docker)
  - kibana (docker)
  - grafana (docker)
  - proteus (file yml and docker)
  - IdentityServer4 (docker)
  
#### PayService (DDD)

### Manitorting
- metrics
- kibana and ElasticSearch
- CheckHealther
  - productService (UI and API)
  - BasketService
  - OrderService
  - DiscountGrpc


  
