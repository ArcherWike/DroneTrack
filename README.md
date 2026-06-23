# Drone Management System: UAV Operations Monitoring System

A comprehensive solution for adding, managing, and dynamically filtering archived and upcoming flights.

## Key System Features
* **Real-Time Visualization:** Interactive map presenting drone positions and statuses.
* **Operation Reporting:** An intuitive panel allowing operators to quickly register new flights directly into the local database.
* **Dynamic Filtering:** Advanced time filters (sliders) and spatial filters (drawing a coverage area directly on the map).
---

### Management

https://github.com/user-attachments/assets/76c2443f-d54e-40af-8b5b-079febe6af00

### Edit mode

https://github.com/user-attachments/assets/ae00cc04-b058-4bdb-b3b3-b876b7fd0384

### User Mode (Adding a new flight)

https://github.com/user-attachments/assets/3706cd03-7cd1-4ba5-b7f5-cf824e75a6a4

## Architecture
* **Desktop Frontend:** WPF (Windows Presentation Foundation) & XAML
* **Map Frontend:** HTML5, CSS3, Leaflet.js
* **Communication:** Microsoft.Web.WebView2 (`ExecuteScriptAsync` & `postMessage`)
* **Database & ORM:** SQLite & Microsoft.EntityFrameworkCore.Sqlite
* **MVVM Support:** CommunityToolkit.Mvvm (Source Generators)

---

## 🏁 How to Run

### 1. For Users (Running the pre-built application)

#### System Requirements:
Before running the application, ensure your Windows system has the appropriate runtime environment installed:
* [**Microsoft .NET Desktop Runtime 8.0**](https://dotnet.microsoft.com/download/dotnet/8.0) (or newer)

#### STEPS:
1. Download the project archive and extract the `.zip` file anywhere on your drive.
2. Go to the main folder and run the executable file **`DroneTrack.exe`**.

---

### 2. For Developers (Compiling and running in Visual Studio)

#### Environment Requirements:
* **Visual Studio 2022** (or newer)
* Installed Workload: **.NET Desktop Development**

#### NuGet Packages Installation:
The application requires external libraries to function correctly. If your environment doesn't automatically restore the packages upon the first launch, install them manually via **NuGet Package Manager** (GUI):

Menu path: `Tools` -> `NuGet Package Manager` -> `Manage NuGet Packages for Solution`

Search for and install the following packages:
- `Microsoft.Web.WebView2` (Embedded browser and C# <-> JS two-way communication support)
- `Microsoft.EntityFrameworkCore.Sqlite` (SQLite database integration in EF Core architecture)
- `CommunityToolkit.Mvvm` (Modern toolkit automating binding)

**Alternative: Package Manager Console**
You can also quickly install the required packages using the built-in console.
Menu path: `Tools` -> `NuGet Package Manager` -> `Package Manager Console`

Run the following commands:
```powershell
Install-Package Microsoft.Web.WebView2
Install-Package Microsoft.EntityFrameworkCore.Sqlite
Install-Package CommunityToolkit.Mvvm
