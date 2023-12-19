dotnet ef migrations add --project DAL --startup-project WebApp InitialCreate

~~~bash
dotnet tool update --global dotnet-aspnet-codegenerator
cd WebApp

dotnet aspnet-codegenerator razorpage -m Domain.Database.Game -dc AppDbContext -udl -outDir Pages/Games --referenceScriptLibraries
dotnet aspnet-codegenerator razorpage -m Domain.Database.Player -dc AppDbContext -udl -outDir Pages/Players --referenceScriptLibraries
~~~