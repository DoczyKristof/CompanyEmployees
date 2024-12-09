# 🚧 WORK IN PROGRESS 🚧
> **Disclaimer:** This project is still a work in progress. Yet to be finished.


# Learning ASP.NET Core Web API

This repository contains the projects and code I developed while learning ASP.NET Core Web API concepts from the **"Ultimate ASP.NET Core Web API Premium"** guide. The journey included foundational to advanced topics, such as middleware, routing, data handling, and API security.

## Table of Contents

- [Overview](#overview)
- [Features Implemented](#features-implemented)
- [Topics Covered](#topics-covered)
- [Setup](#setup)
- [Acknowledgments](#acknowledgments)

## Overview

This repository is a hands-on demonstration of concepts covered in the guide, focusing on building robust, scalable, and well-architected APIs using ASP.NET Core. It includes several projects, each showcasing specific aspects of API development.

## Features Implemented

- Middleware pipelines for request handling
- Onion architecture for maintainability
- Dependency injection for decoupled code
- Entity Framework Core for data access
- CRUD operations with generic repositories
- API versioning and content negotiation
- JWT-based authentication and authorization
- Custom logging services using NLog

## Topics Covered

1. **Project Setup and Configuration**
   - Configuring `Program.cs` and `launchSettings.json`
   - Environment-specific settings and CORS
2. **Data Handling**
   - Entity Framework Core models and migrations
   - Repository and service layers
3. **Advanced API Features**
   - Global error handling
   - Filtering, paging, and sorting
   - HATEOAS and CQRS with MediatR
4. **API Security**
   - JWT Authentication and Refresh Tokens
   - Role-based authorization
5. **Deployment and Optimization**
   - Hosting with IIS
   - API caching and rate limiting

## Setup

To explore the projects:
1. Clone the repository.
2. Restore dependencies using `dotnet restore`.
3. Update connection strings in `appsettings.json` to your database setup.
4. Run the project with `dotnet run`.

## Acknowledgments

This repository is inspired by the **"Ultimate ASP.NET Core Web API Premium"** guide, a comprehensive resource for learning ASP.NET Core API development.
