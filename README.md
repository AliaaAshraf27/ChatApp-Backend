# ChatProject — Real-Time Chat Backend
A chat application backend built with ASP.NET Core Web API. The project supports private messaging, group conversations, media sharing, and real-time updates using SignalR.

The main goal of the project is to build a structured backend with clear separation of responsibilities, reliable data access, and real-time communication between connected users.

## Tech Stack
- C# / ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- SignalR
- Redis
- JWT Authentication
- AutoMapper
- Swagger / OpenAPI
- Docker

## Features
- **Authentication:** User registration, login, OTP verification, profile completion, and JWT-based authentication.
- **Private Chats:** Create conversations and exchange messages between users.
- **Group Chats:** Manage groups, members, and administrative permissions.
- **Message Management:** Send, edit, delete, and forward messages.
- **Message Status:** Track message delivery and read receipts.
- **Media Sharing:** Upload and share images and files.
- **Status Updates:** Create text, image, and video statuses that expire after 24 hours.
- **Contact Management:** Support contact-specific display names.
- **User Blocking:** Prevent messaging between users who have blocked each other.
- **Online Presence:** Track online users and manage multiple active connections.
- **Caching:** Use Redis for caching and temporary data storage.

## Project Structure

```text
ChatProject/
├── Api/
├── Application/
├── Domain/
└── Infrastructure/
```

- **Api:** Handles HTTP requests, controllers, and API endpoints.
- **Application:** Contains application services, business workflows, and DTOs.
- **Domain:** Contains the core entities and business models.
- **Infrastructure:**Handles database access, repositories, and external integrations.

## Technical Highlights

- **Clean Architecture:** Separating the API, application logic, domain models, and infrastructure.
- **Repository Pattern:** Organizing data access through repositories.
- **Service Layer:** Keeping business logic separate from controllers.
- **Real-Time Communication:** Using SignalR for messaging and live notifications.
- **Authentication & Authorization:** Securing endpoints with JWT.
- **Data Mapping:** Using DTOs and AutoMapper to structure API requests and responses.
- **Caching:** Using Redis for caching and temporary data storage.
- **Database Management:** Using Entity Framework Core with SQL Server.

