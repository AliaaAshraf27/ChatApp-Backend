# ChatProject — Real-Time Chat Backend
A chat application backend built with **ASP.NET Core Web API**. The project supports private messaging, group conversations, media sharing, and real-time updates using SignalR.
The main goal of the project is to build a structured backend with clear separation of responsibilities, reliable data access, and real-time communication between connected users.

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

## Tech Stack
- C#
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- SignalR
- Redis
- JWT Authentication
- AutoMapper
- Swagger and Postman

## Project Structure
The solution is divided into four projects, following Clean Architecture principles.

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
- **Infrastructure:** Handles database access, repositories, and external integrations.

This structure separates application logic from infrastructure concerns and makes the codebase easier to maintain and extend.

## Real-Time Communication
SignalR is used to deliver events to connected clients as they occur.
The backend supports real-time notifications for new messages, message updates, read and delivery receipts, group membership changes, and status updates.
Connection management also supports multiple active connections for the same user, allowing users to connect from multiple devices or sessions.

## Database & Caching

- **SQL Server** stores application data.
- **Entity Framework Core** handles database operations.
- **Redis** supports caching and temporary data storage.
- **AutoMapper and DTOs** help separate database entities from API request and response models.

## Getting Started
### Prerequisites

- .NET SDK compatible with the solution.
- SQL Server.
- Redis.
- Visual Studio or another .NET-compatible IDE.

### Setup

1. Clone the repository:

   ```bash
   git clone <repository-url>
   ```

2. Open `ChatProject.sln` in Visual Studio.

3. Configure the database connection string, JWT settings, and Redis connection in your local configuration.

4. Apply the Entity Framework Core migrations using the correct startup and migrations projects:

   ```bash
   dotnet ef database update
   ```

5. Run the `Api` project.

6. Open Swagger to explore and test the available endpoints.

**Note:** The exact migration command depends on where the `DbContext` and migrations are defined. Keep credentials and secrets out of source control.

## API Testing

API endpoints can be explored using Swagger and tested with Postman. SignalR events can be tested using a compatible SignalR client.

## Future Improvements

- Expand unit and integration test coverage.
- Improve logging and exception handling.
- Add Docker support for easier deployment.
- Optimize database queries and caching where needed.
