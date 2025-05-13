# Security

## Overview
The Security project is a .NET 8 application that provides authentication and authorization services using JWT tokens and refresh tokens. It leverages ASP.NET Core Identity for user management and Entity Framework Core for data access.

## Features
- User registration and login
- JWT token generation and validation
- Refresh token generation and management
- Role-based authorization
- Secure password storage using ASP.NET Core Identity
- **OAuth2 SSO login with Google**

## Technologies Used
- .NET 8
- ASP.NET Core
- Entity Framework Core
- SQLite (or any other supported database)
- JWT (JSON Web Tokens)
- ASP.NET Core Identity
- Google OAuth2

## Getting Started

### Prerequisites
- .NET 8 SDK
- Visual Studio 2022 (or any other preferred IDE)
- Google Cloud account (for OAuth2 setup)

### Installation
1. Clone the repository:
git clone https://github.com/yourusername/Security.git
cd Security


2. Install the required NuGet packages:
 dotnet restore

 
3. Update the database:
 dotnet ef database update


### Configuration
4. Update the `appsettings.json` file with your database connection string, JWT settings, and Google OAuth2 credentials:
    
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
  "Authentication": {
    "Google": {
      "ClientId": "your-google-client-id",
      "ClientSecret": "your-google-client-secret"
    }
  },
  "AllowedHosts": "*"
}

5. In the [Google Cloud Console](https://console.cloud.google.com/apis/credentials), register your OAuth2 client and add the following **Authorized redirect URI**:
        
https://localhost:7054/signin-google    
   
(Add `http://localhost:7054/signin-google` if you use HTTP in development.)
### Running the Application
1. Build and run the application:
  dotnet run


6. The application will be available at `https://localhost:7054/`.

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

### Refresh Token
- Endpoint: `POST /api/auth/refresh`
- Request Body:
{
  "refreshToken": "your-refresh-token"
}
    

### Google OAuth2 Login

#### How it Works

1. **Initiate Login:**  
   Redirect the user's browser to the following endpoint:
https://localhost:7054/api/auth/external-login?provider=Google

   This will redirect the user to Google's login page.

2. **Google Authentication:**  
   The user logs in with their Google account.

3. **Callback:**  
   After successful authentication, Google redirects the user to:
 https://localhost:7054/signin-google

    The backend processes the response and then calls your `/api/auth/external-login-callback` endpoint.

   
   