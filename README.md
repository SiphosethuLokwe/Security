# Security

## Overview
The Security project is a .NET 8 application that provides authentication and authorization services using JWT tokens and refresh tokens. It leverages ASP.NET Core Identity for user management and Entity Framework Core for data access.

## Features
- User registration and login
- JWT token generation and validation
- Refresh token generation and management
- Role-based authorization
- Secure password storage using ASP.NET Core Identity

## Technologies Used
- .NET 8
- ASP.NET Core
- Entity Framework Core
- SQLite (or any other supported database)
- JWT (JSON Web Tokens)
- ASP.NET Core Identity

## Getting Started

### Prerequisites
- .NET 8 SDK
- Visual Studio 2022 (or any other preferred IDE)

### Installation
1. Clone the repository:


2. Install the required NuGet packages:


3. Update the database:
    


### Configuration
1. Update the `appsettings.json` file with your database connection string and JWT settings:
    
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=DB/Security.db"
  },
  "Jwt": {
    "Key": "YourSuperSecretKeyThatIsAtLeast32CharactersLong",
    "Issuer": "https://localhost:7054/",
    "Audience": "https://localhost:7054/"
  },
  "AllowedHosts": "*"
}


### Running the Application
1. Build and run the application:
    

2. The application will be available at `https://localhost:7054/`.

## Usage
### Register a New User
- Endpoint: `POST /api/auth/register`
- Request Body:

{
  "username": "newuser",
  "email": "newuser@example.com",
  "password": "Pass@word1"
}
 

### Login
- Endpoint: `POST /api/auth/login`
- Request Body:
{
  "username": "newuser",
  "password": "Pass@word1"
}

{
  "refreshToken": "your-refresh-token"
}

