# eClaims System

## Overview
A modern claims processing system built with **ASP.NET Core** and **MongoDB**.

## Architecture
The solution follows a Clean Architecture / Onion Architecture approach:

*   **eClaims.Core**: Domain Entities, Interfaces, and Business Logic. (No dependencies on Infrastructure/Web).
*   **eClaims.Infrastructure**: Implementation of Interfaces (e.g., MongoDB Repositories), External Service Adapters.
*   **eClaims.API**: ASP.NET Core Web API controllers exposing endpoints for Mobile/SPA clients.
*   **eClaims.Web**: ASP.NET Core MVC application for Customer, Partner, and Admin portals.

## Prerequisites
*   .NET 9.0 SDK
*   MongoDB (running locally or Atlas connection string)

## Setup
1.  Navigate to root.
2.  Restore packages: `dotnet restore`
3.  Run API: `dotnet run --project eClaims.API`
4.  Run Web: `dotnet run --project eClaims.Web`

## Configuration
Update `appsettings.json` in `eClaims.API` and `eClaims.Web` with your MongoDB connection string.
