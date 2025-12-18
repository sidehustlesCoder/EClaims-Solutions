dotnet new sln -n eClaims
dotnet new classlib -n eClaims.Core
dotnet new classlib -n eClaims.Infrastructure
dotnet new webapi -n eClaims.API
dotnet new mvc -n eClaims.Web
dotnet sln add eClaims.Core/eClaims.Core.csproj
dotnet sln add eClaims.Infrastructure/eClaims.Infrastructure.csproj
dotnet sln add eClaims.API/eClaims.API.csproj
dotnet sln add eClaims.Web/eClaims.Web.csproj

dotnet add eClaims.Infrastructure/eClaims.Infrastructure.csproj reference eClaims.Core/eClaims.Core.csproj
dotnet add eClaims.API/eClaims.API.csproj reference eClaims.Core/eClaims.Core.csproj
dotnet add eClaims.API/eClaims.API.csproj reference eClaims.Infrastructure/eClaims.Infrastructure.csproj
dotnet add eClaims.Web/eClaims.Web.csproj reference eClaims.Core/eClaims.Core.csproj
