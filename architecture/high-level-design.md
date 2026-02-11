# High-Level Design (HLD) - eClaims System

## 1. Solution Overview
The eClaims System is designed as a **Service-Oriented Architecture (SOA)** or **Microservices** solution using the **Microsoft .NET Ecosystem**.
*   **Web Portals**: Built using **ASP.NET Core MVC** (Server-Side Rendering) for robust SEO and traditional web workflows.
*   **Mobile & Partner APIs**: Exposed via **ASP.NET Core Web APIs**.
*   **Data Layer**: **MongoDB** (NoSQL) for flexible schema handling of claims and metadata.

## 2. System Context Diagram (C4 Level 1)

```mermaid
C4Context
    title System Context Diagram for eClaims System

    Person(customer, "Customer", "Submits claims via Web/App")
    Person(partner, "3rd Party Provider", "Updates work estimates")
    Person(staff, "Internal Staff", "Adjudicates and reports")

    System_Boundary(eClaims, "eClaims System (.NET)") {
        System(web_mvc, "Web Portals (MVC)", "ASP.NET Core MVC Application")
        System(api_core, "Core API Services", "ASP.NET Core Web API")
        System(idp, "Identity Provider", "ASP.NET Identity / IdentityServer")
    }

    System_Ext(payment_gateway, "Payment Gateway", "Stripe / PayPal")
    System_Ext(email_system, "Notification Service", "SMTP / Twilio")

    Rel(customer, web_mvc, "Uses Portal", "HTTPS/HTML")
    Rel(customer, api_core, "Uses Mobile App", "HTTPS/JSON")
    Rel(partner, web_mvc, "Uses Partner Portal", "HTTPS/HTML")
    Rel(staff, web_mvc, "Uses Internal Portal", "HTTPS/HTML")
    
    Rel(web_mvc, api_core, "Delegates Logic/Data", "Internal HTTP / gRPC")
    Rel(api_core, idp, "Validates Auth", "OIDC/Tokens")
    Rel(api_core, payment_gateway, "Processes Payments", "API")
```

## 3. Container Diagram (C4 Level 2)

```mermaid
C4Container
    title Container Diagram - .NET & MongoDB Architecture

    Container_Boundary(frontend, "Frontend / Presentation Layer") {
        Container(customer_mvc, "Customer MVC Portal", "ASP.NET Core MVC", "Razor Views for Customers")
        Container(partner_mvc, "Partner MVC Portal", "ASP.NET Core MVC", "Razor Views for Providers")
        Container(internal_mvc, "Internal Admin Portal", "ASP.NET Core MVC", "Dashboard for Staff")
        Container(mobile_app, "Mobile App", "Xamarin / MAUI or React Native", "Consumes API directly")
    }

    Container(api_gateway, "API Gateway / YARP", "YARP (Yet Another Reverse Proxy)", "Routing & Load Balancing")

    Container_Boundary(backend, "Application Layer (C# APIs)") {
        Container(claims_api, "Claims API", "ASP.NET Core Web API", "Claims Logic & State Machine")
        Container(dms_api, "Document API", "ASP.NET Core Web API", "File handling")
        Container(reporting_api, "Reporting API", "ASP.NET Core Web API", "Data Aggregation")
        Container(notify_api, "Notification Service", "ASP.NET Core Worker", "Background Jobs")
    }

    ContainerDb(mongo_db, "Primary Database", "MongoDB", "Collections: Claims, Users, Logs")
    ContainerDb(obj_store, "File Storage", "Disk / S3 / GridFS", "Physical File Storage")
    ContainerQueue(msg_queue, "Message Queue", "RabbitMQ / Azure Service Bus", "Async Events")

    Rel(customer_mvc, api_gateway, "Calls", "HTTPS")
    Rel(partner_mvc, api_gateway, "Calls", "HTTPS")
    Rel(internal_mvc, api_gateway, "Calls", "HTTPS")
    Rel(mobile_app, api_gateway, "Calls", "HTTPS")

    Rel(api_gateway, claims_api, "Routes Request")
    Rel(api_gateway, dms_api, "Routes Request")

    Rel(claims_api, mongo_db, "Read/Write Documents", "MongoDB Driver")
    Rel(dms_api, mongo_db, "Stores Metadata")
    Rel(dms_api, obj_store, "Stores Blobs")
    
    Rel(claims_api, msg_queue, "Publishes Events")
    Rel(notify_api, msg_queue, "Consumes Events")
```

## 4. Key Components

### A. ASP.NET Core MVC (Portals)
Each portal (Customer, Partner, Internal) can be a separate Area within a single modular Monolith OR separate deployable MVC apps.
*   **Controller**: Handles UI logic and View returning.
*   **Service Agent**: Calls the backend APIs (separation of concerns).

### Sequence Diagrams

A sequence diagram for the primary Claim submission flow has been added: architecture/sequence/claim_submission.puml

### B. MongoDB Data Model
Leverage the schema-less nature for **Claims**.
*   A "Claim" document can embed "Estimates" and "Notes" directly, avoiding complex joins.
*   Polymorphism is easy: `CarAccidentClaim` vs `HomeClaim` can coexist in the same collection.

### C. Authentication
*   **ASP.NET Core Identity** storing users in MongoDB.
*   **JWT Bearer Tokens** for API communication.

## 5. Technology Stack
*   **Frontend Web**: ASP.NET Core 8 MVC (Razor).
*   **Mobile**: (Choice of User) consuming C# API.
*   **Backend API**: C# / ASP.NET Core Web API.
*   **Database**: MongoDB (v6+).
*   **Documentation**: Swagger/OpenAPI.
