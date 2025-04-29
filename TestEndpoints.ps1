# This script is used to test the endpoints
param(
    [string]$environment = "Development",
    [string]$launchProfile = "https-Development",
    [string]$connectionStringKey = "BooksDb",
    [bool]$dropDatabase = $true,
    [bool]$createDatabase = $false
)

# Get the project name
$projectName = Get-ChildItem -Recurse -Filter "*.csproj" | Select-Object -First 1 | ForEach-Object { $_.Directory.Name } # Projectname can also be set manually

# Get the base URL of the project
$launchSettings = Get-Content -LiteralPath ".\Properties\launchSettings.json" | ConvertFrom-Json
$baseUrl = ($launchSettings.profiles.$launchProfile.applicationUrl -split ";")[0] # Can also set manually -> $baseUrl = "https://localhost:7253"

#Install module SqlServer
if (-not (Get-Module -ErrorAction Ignore -ListAvailable SqlServer)) {
    Write-Verbose "Installing SqlServer module for the current user..."
    Install-Module -Scope CurrentUser SqlServer -ErrorAction Stop
}
Import-Module SqlServer

# Set the environment variable
$env:ASPNETCORE_ENVIRONMENT = $environment



# Read the connection string from appsettings.Development.json
$appSettings = Get-Content ".\appsettings.$environment.json" | ConvertFrom-Json
$connectionString = $appSettings.ConnectionStrings.$connectionStringKey
Write-Host "Database Connection String: $connectionString" -ForegroundColor Blue


# Get the database name from the connection string
if ($connectionString -match "Database=(?<BooksDb>[^;]+)") {
    $databaseName = $matches['BooksDb']
    Write-Host "Database Name: $databaseName" -ForegroundColor Blue
}
else {
    Write-Host "Database Name not found in connection string" -ForegroundColor Red
    exit
}


# Check if the database exists
$conStringDbExcluded = $connectionString -replace "Database=[^;]+;", ""
$queryDbExists = Invoke-Sqlcmd -ConnectionString $conStringDbExcluded -Query "Select name FROM sys.databases WHERE name='$databaseName'"
if ($queryDbExists) {
    if ($dropDatabase -or (Read-Host "Do you want to drop the database? (y/n)").ToLower() -eq "y") { 

        # Drop the database
        Invoke-Sqlcmd -ConnectionString $connectionString -Query  "USE master;ALTER DATABASE $databaseName SET SINGLE_USER WITH ROLLBACK IMMEDIATE;DROP DATABASE $databaseName;"
        Write-Host "Database $databaseName dropped." -ForegroundColor Green
    }
}

# Create the database from the model
if (Select-String -LiteralPath ".\Program.cs" -Pattern "EnsureCreated()") {
    Write-Host "The project uses EnsureCreated() to create the database from the model." -ForegroundColor Yellow
}
else {
    if ($createDatabase -or (Read-Host "Should dotnet ef migrate and update the database? (y/n)").ToLower() -eq "y") { 

        dotnet ef migrations add "UpdateModelFromScript_$(Get-Date -Format "yyyyMMdd_HHmmss")" --project ".\$projectName.csproj"
        dotnet ef database update --project ".\$projectName.csproj"
    }
}

# Run the application
if ((Read-Host "Start the server from Visual studio? (y/n)").ToLower() -ne "y") { 
    Start-Process -FilePath "dotnet" -ArgumentList "run --launch-profile $launchProfile --project .\$projectName.csproj" -WindowStyle Normal    
    Write-Host "Wait for the server to start..." -ForegroundColor Yellow 
}

# Continue with the rest of the script
Read-Host "Press Enter to continue when the server is started..."



### =============================================================
### =============================================================
### =============================================================

# Send requests to the API endpoint




### Copy below code to test the endpoints




###

### ------------Post an Author


Write-Host "`nCreate an Author"


$httpMethod = "Post"   ### "Get", "Post", "Put", "Delete"

$endPoint = "$baseUrl/api/Authors"

$json = '{ 
    "FirstName": "Lukas", 
    "LastName": "Malki" 
}'

$response = Invoke-RestMethod -Uri $endPoint -Method $httpMethod -Body $json -ContentType "application/json"
$response | Format-Table


### ------------ Query Movies from the database
$sqlResult = Invoke-Sqlcmd -ConnectionString $connectionString -Query "Select * FROM Authors"
$sqlResult | Format-Table



###

### ------------Post an Author


Write-Host "`nCreate an Author"


$httpMethod = "Post"   ### "Get", "Post", "Put", "Delete"

$endPoint = "$baseUrl/api/Authors"

$json = '{ 
    "FirstName": "Luke", 
    "LastName": "Skywalker" 
}'

$response = Invoke-RestMethod -Uri $endPoint -Method $httpMethod -Body $json -ContentType "application/json"
$response | Format-Table


### ------------ Query Movies from the database
$sqlResult = Invoke-Sqlcmd -ConnectionString $connectionString -Query "Select * FROM Authors"
$sqlResult | Format-Table



###

### ------------Post a Book


Write-Host "`nCreate a Book"


$httpMethod = "Post"   ### "Get", "Post", "Put", "Delete"

$endPoint = "$baseUrl/api/Books"

$json = '{ 
    "title": "harry potter",
    "isbn": 123,
    "publicationYear": 2024,
    "rating": 8,
    "CopiesTotal": 6,
    "authorIds": [
        1, 2
    ]
}'

$response = Invoke-RestMethod -Uri $endPoint -Method $httpMethod -Body $json -ContentType "application/json"
$response | Format-Table


### ------------ Query Movies from the database
$sqlResult = Invoke-Sqlcmd -ConnectionString $connectionString -Query "Select * FROM Books"
$sqlResult | Format-Table




###

### ------------Post a Book


Write-Host "`nCreate a Borrower"


$httpMethod = "Post"   ### "Get", "Post", "Put", "Delete"

$endPoint = "$baseUrl/api/Borrowers"

$json = '{
    "firstName": "dibora",
    "lastName": "afewerki"
  }'

$response = Invoke-RestMethod -Uri $endPoint -Method $httpMethod -Body $json -ContentType "application/json"
$response | Format-Table


### ------------ Query Movies from the database
$sqlResult = Invoke-Sqlcmd -ConnectionString $connectionString -Query "Select * FROM Borrowers"
$sqlResult | Format-Table



###

### ------------Post a Book


Write-Host "`nCreate a Loan"


$httpMethod = "Post"   ### "Get", "Post", "Put", "Delete"

$endPoint = "$baseUrl/api/Loans"

$json = '{
    "borrowerId": 1,
    "bookId": 1
  }'

$response = Invoke-RestMethod -Uri $endPoint -Method $httpMethod -Body $json -ContentType "application/json"
$response | Format-Table


### ------------ Query Movies from the database
$sqlResult = Invoke-Sqlcmd -ConnectionString $connectionString -Query "Select * FROM Loans"
$sqlResult | Format-Table

