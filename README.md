# Drone Management System: UAV Operations Monitoring System

A comprehensive solution for adding, managing, and dynamically filtering archived and upcoming flights.

## Key System Features
* **Real-Time Visualization:** Interactive map presenting drone positions and statuses.
* **Operation Reporting:** An intuitive panel allowing operators to quickly register new flights directly into the local database.
* **Dynamic Filtering:** Advanced time filters (sliders) and spatial filters (drawing a coverage area directly on the map).
---

## Management
<img width="1161" height="650" alt="obraz" src="https://github.com/user-attachments/assets/ea9639a4-00db-4ed3-bdb9-eb88d03f9a10" />

<details>
<summary>🎞️ Show the demonstration video</summary>
<br>
<video controls width="100%" src="https://github.com/user-attachments/assets/76c2443f-d54e-40af-8b5b-079febe6af00"></video>
</details>
<br>

## Edit mode
<details>
<summary>🎞️ Show the demonstration video</summary>
<br>
<video controls width="100%" src="https://github.com/user-attachments/assets/ae00cc04-b058-4bdb-b3b3-b876b7fd0384"></video>
</details>

## User Mode (Adding a new flight)
<img width="1158" height="653" alt="obraz" src="https://github.com/user-attachments/assets/fdc5329b-b0d4-4247-9358-4254948cafbd" />
<br>
<details>
<summary>🎞️ Show the demonstration video</summary>
<br>
<video controls width="100%" src="https://github.com/user-attachments/assets/3706cd03-7cd1-4ba5-b7f5-cf824e75a6a4"></video>
</details>

### Settings
<img width="1920" height="1080" alt="System monitorowania BSP" src="https://github.com/user-attachments/assets/a249e21f-b6a2-4dd9-9c73-af978c6c33a1" />

## Architecture
* **Desktop Frontend:** WPF (Windows Presentation Foundation) & XAML
* **Map Frontend:** HTML5, CSS3, Leaflet.js
* **Communication:** Microsoft.Web.WebView2 (`ExecuteScriptAsync` & `postMessage`)
* **Database & ORM:** SQLite & Microsoft.EntityFrameworkCore.Sqlite
* **MVVM Support:** CommunityToolkit.Mvvm (Source Generators)
<img width="1162" height="655" alt="obraz" src="https://github.com/user-attachments/assets/418810ac-321e-49b7-a577-9d84044993ba" />
<img width="1160" height="651" alt="obraz" src="https://github.com/user-attachments/assets/609206b7-4014-4b3e-8ec2-3107c829ca2a" />


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
