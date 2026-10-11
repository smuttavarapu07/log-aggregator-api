# Log Aggregator API

A RESTful Web API built using ASP.NET Core to collect, store, and retrieve application logs. The API allows applications to submit log messages and retrieve logs filtered by severity level.

## Objective

To build a centralized logging system where applications can send error logs to an API, which stores them in a database for monitoring and troubleshooting.

## Features

- Accept application logs through a POST API endpoint.
- Store logs in a database.
- Retrieve logs filtered by severity level.
- Support log levels such as Information, Warning, Error, and Critical.
- Use structured API endpoints for logging operations.

## Technologies Used

- **Language:** C#
- **Framework:** ASP.NET Core Web API
- **Database:** MySQL (if configured)
- **Data access:** Dapper or Entity Framework Core
- **Tools:** Visual Studio Code, Git, GitHub

## Database Design

### SystemLogs

| Column | Description |
|---|---|
| Id | Unique identifier for each log |
| LogLevel | Severity level of the log |
| Message | Description of the event or error |
| Timestamp | Date and time when the log was recorded |

## API Endpoints

### 1. Create a log

**Endpoint:** `POST /api/logs`

Accepts a log payload and stores it in the database.

Example request:

```json
{
  "logLevel": "Error",
  "message": "An unexpected error occurred",
  "timestamp": "2026-10-11T10:30:00Z"
}
```

### 2. Filter logs by severity

**Endpoint:** `GET /api/logs/filter?level=Error`

Retrieves logs matching the specified severity level.

Example:

```http
GET /api/logs/filter?level=Error
```

## Getting Started

### Prerequisites

- .NET SDK compatible with the project
- MySQL or another configured database
- Visual Studio Code or a compatible IDE

### Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/smuttavarapu07/log-aggregator-api.git
   ```

2. Navigate to the project folder:

   ```bash
   cd log-aggregator-api
   ```

3. Configure the database connection locally using environment variables or local configuration files.

4. Restore dependencies:

   ```bash
   dotnet restore
   ```

5. Run the application:

   ```bash
   dotnet run
   ```

6. Test the endpoints using Postman or another API testing tool.

## Security

- Never commit database passwords, API keys, or other secrets.
- Validate incoming log data.
- Avoid storing sensitive personal information in logs.
- Consider authentication and rate limiting before using the API in production.

## Learning Outcomes

- Building RESTful APIs using ASP.NET Core.
- Connecting a .NET application to a database.
- Performing database insert and query operations.
- Implementing filtering using query parameters.
- Handling application logging data.

## Author

**Sravika Muttavarapu**

GitHub: [@smuttavarapu07](https://github.com/smuttavarapu07)
