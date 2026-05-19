<div align="center">

<h1>Parking Management System</h1>



![C#](https://img.shields.io/badge/C%23-.NET%20Framework-239120?style=for-the-badge&logo=csharp&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-Microsoft-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)
![Windows Forms](https://img.shields.io/badge/Windows%20Forms-WinForms-0078D6?style=for-the-badge&logo=windows&logoColor=white)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
![Status](https://img.shields.io/badge/Status-Active-brightgreen?style=for-the-badge)

<p align="center">
  A full-featured <strong>desktop-based Parking Management System</strong> built with C# Windows Forms and Microsoft SQL Server — supporting multi-role access, real-time slot tracking, vehicle booking, and automated billing.
</p>

---

</div>

## 📌 Table of Contents

- [Overview](#-overview)
- [Features](#-features)
- [Tech Stack](#-tech-stack)
- [Screenshots](#-screenshots)
- [Database Schema](#-database-schema)
- [Getting Started](#-getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Database Setup](#database-setup)
- [Usage](#-usage)
- [Project Structure](#-project-structure)
- [Role-Based Access](#-role-based-access)
- [Known Issues & Limitations](#-known-issues--limitations)
- [Contributing](#-contributing)
- [License](#-license)
- [Author](#-author)

---

## 🧭 Overview

The **Parking Management System** is a Windows desktop application designed to streamline vehicle parking operations. It supports two types of users — **Admin** and **Regular User** — each with their own dashboard and access control. The system handles the full lifecycle of a parking session: from vehicle entry and slot assignment to payment processing and record deletion.

> Built as a practical real-world project using **C# WinForms** with a **Microsoft SQL Server** backend.

---

## ✨ Features

### 🔐 Authentication
- Secure user **Login** and **Registration**
- Role-based redirection (Admin vs Regular User)
- Password visibility toggle
- Duplicate username prevention

### 🅿️ Parking Management
- **Add Booking** — Register a vehicle with owner name, vehicle number, entry time, and slot
- **Update Booking** — Modify owner name, vehicle number, and entry time
- **Delete Booking** — Remove records (only after payment is confirmed)
- **Search** — Search by vehicle number or owner name with live autocomplete (`ListBox` suggestions)

### 💳 Payment & Billing
- Automated **cost calculation** based on parking duration
- Configurable **parking rate (BDT/hour)** — set by Admin and persisted via Application Settings
- Payment status tracking (`not paid` / `paid`)

### 🏢 Slot Management
- **5 Parking Slots** (A, B, C, D, E) — each with a **20-vehicle capacity**
- Real-time slot occupancy counter via `ParkingSlotCounter` table
- Slot overflow protection (blocks booking when full)

### 📊 Dashboard
- Total, Occupied, and Available parking space display
- User-specific booking view for regular users
- Admin sees all records across all users
- Current parking rate display

---

## 🛠 Tech Stack

| Technology | Purpose |
|---|---|
| **C# (.NET Framework)** | Application logic & UI |
| **Windows Forms (WinForms)** | Desktop GUI |
| **Microsoft SQL Server (Express)** | Relational database |
| **ADO.NET** (`SqlConnection`, `SqlCommand`, `SqlDataAdapter`) | Database connectivity |
| **Application Settings** (`Properties.Settings`) | Persistent rate configuration |

---
## 🎬 Demo Video

[![Watch the Demo](https://img.shields.io/badge/Google%20Drive-Watch%20Demo-4285F4?style=for-the-badge&logo=googledrive&logoColor=white)]([https://drive.google.com/YOUR_SHARE_LINK](https://drive.google.com/file/d/1jFJO1rJvjwID4dzcyyenaBFn62-sgyr8/view?usp=sharing
)

> Click the badge above to watch a full walkthrough of the Parking Management System.

## 📸 Screenshots

| Login Screen | Registration | Admin Dashboard |
|---|---|---|
| `Screenshots/login.png` | `Screenshots/register.png` | `Screenshots/admin_dashboard.png` |

| Add Booking | Delete Booking | Payment |
|---|---|---|
| `Screenshots/add_booking.png` | `Screenshots/delete.png` | `Screenshots/payment.png` |

| User Dashboard |
|---|
| `screenshots/user_dashboard.png` |
---

## 🗄 Database Schema

The project uses two primary tables and one counter table.

### `usersInfo` — User Accounts
| Column | Type | Description |
|---|---|---|
| `username` | `VARCHAR` | Unique username (Primary Key) |
| `password` | `VARCHAR` | User password (plain text) |
| `email` | `VARCHAR` | User email address |

### `Parking` — Parking Records
| Column | Type | Description |
|---|---|---|
| `id` | `INT` | Auto-increment primary key |
| `username` | `VARCHAR` | Linked user account |
| `ownername` | `VARCHAR` | Vehicle owner's name |
| `VehicleNumber` | `VARCHAR` | Unique vehicle plate number |
| `EntryTime` | `DATETIME` | Vehicle entry timestamp |
| `ExitTime` | `DATETIME` | Vehicle exit timestamp (nullable) |
| `TotalCost` | `DECIMAL` | Calculated parking cost (nullable) |
| `slot` | `VARCHAR` | Assigned slot (A–E) |
| `status` | `VARCHAR` | `not paid` or `paid` |

### `ParkingSlotCounter` — Slot Capacity Tracker
| Column | Type | Description |
|---|---|---|
| `slot` | `VARCHAR` | Slot identifier (A–E) |
| `VehicleCount` | `INT` | Current number of vehicles in slot |

> 📁 A **database backup file (`.bak`)** is included in this repository for easy restoration.

---

## 🚀 Getting Started

### Prerequisites

Before running this project, make sure you have the following installed:

- [Visual Studio 2019 or later](https://visualstudio.microsoft.com/) (with .NET Desktop Development workload)
- [Microsoft SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or full SQL Server)
- [SQL Server Management Studio (SSMS)](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)
- .NET Framework 4.7.2+

---

### Installation

**1. Clone the repository**
```bash
git clone https://github.com/YOUR_USERNAME/parking-management-system.git
cd parking-management-system
```

**2. Open in Visual Studio**

Open `parking_management.sln` in Visual Studio.

**3. Restore NuGet Packages** *(if needed)*
```
Tools → NuGet Package Manager → Restore Packages
```

**4. Update the connection string**

The connection string is defined in each `.cs` file as:
```csharp
string cs = @"Data Source=localhost\SQLEXPRESS01;Initial Catalog=userDB;Integrated Security=True;TrustServerCertificate=True";
```

> ⚠️ Update `localhost\SQLEXPRESS01` to match your SQL Server instance name if it differs.

**5. Build and Run**

Press `F5` or click **Start** in Visual Studio.

---

### Database Setup

**Option A — Restore from Backup (Recommended)**

1. Open **SQL Server Management Studio (SSMS)**
2. Right-click **Databases** → **Restore Database...**
3. Select **Device** → Browse to the `.bak` file included in this repo
4. Set the database name to `userDB`
5. Click **OK** to restore

**Option B — Manual Setup**

Run the following SQL scripts in SSMS:

```sql
CREATE DATABASE userDB;
USE userDB;

-- Users table
CREATE TABLE usersInfo (
    username VARCHAR(50) PRIMARY KEY,
    password VARCHAR(50) NOT NULL,
    email    VARCHAR(100)
);

-- Parking records table
CREATE TABLE Parking (
    id            INT IDENTITY(1,1) PRIMARY KEY,
    username      VARCHAR(50),
    ownername     VARCHAR(100),
    VehicleNumber VARCHAR(20),
    EntryTime     DATETIME,
    ExitTime      DATETIME NULL,
    TotalCost     DECIMAL(10,2) NULL,
    slot          VARCHAR(5),
    status        VARCHAR(20)
);

-- Slot counter table
CREATE TABLE ParkingSlotCounter (
    slot         VARCHAR(5) PRIMARY KEY,
    VehicleCount INT DEFAULT 0
);

-- Initialize slots
INSERT INTO ParkingSlotCounter (slot, VehicleCount) VALUES
('A', 0), ('B', 0), ('C', 0), ('D', 0), ('E', 0);

-- Create default admin account
INSERT INTO usersInfo (username, password, email)
VALUES ('admin', 'admin123', 'admin@parking.com');
```

---

## 📖 Usage

### Default Admin Credentials
```
Username : admin
Password : admin123
```

### Workflow

```
1. Launch the application → Login screen appears
2. Register a new user OR login with existing credentials
3. Admin → Full access to all records and settings
   User  → Access limited to their own bookings
4. Add a vehicle booking (Owner Name, Vehicle Number, Slot)
5. When the vehicle exits → Go to Payment to generate cost
6. After payment → Delete the record to free the slot
```

---

## 📁 Project Structure

```
parking-management-system/
│
├── 📄 Form1.cs               # Payment / billing form
├── 📄 login.cs               # Login screen
├── 📄 register.cs            # New user registration
├── 📄 dashboard.cs           # Regular user dashboard
├── 📄 admin.cs               # Admin dashboard
├── 📄 add.cs                 # Add new parking booking
├── 📄 update.cs              # Update existing booking
├── 📄 delete.cs              # Delete a booking record
├── 📄 Program.cs             # Application entry point
│
├── 📁 Properties/
│   └── Settings.settings     # Persistent parking rate config
│
├── 📁 database/
│   └── userDB.bak            # SQL Server database backup
│
└── 📄 README.md
```

---

## 👥 Role-Based Access

| Feature | Admin | Regular User |
|---|:---:|:---:|
| View all bookings | ✅ | ❌ |
| View own bookings | ✅ | ✅ |
| Add booking | ✅ | ✅ |
| Update booking | ✅ | ✅ |
| Delete booking | ✅ | ✅ |
| Search (all users) | ✅ | ❌ |
| Search (own only) | ✅ | ✅ |
| Update parking rate | ✅ | ❌ |
| Process payment | ✅ | ✅ |

---

## 📜 License

This project is licensed under the **MIT License** — see the [LICENSE](LICENSE) file for details.

---

<div align="center">

⭐ **If you found this project useful, please give it a star!** ⭐

<img src="https://komarev.com/ghpvc/?username=YOUR_USERNAME&label=Profile+Views&color=3B82F6&style=flat" alt="Profile Views" />

*Made with ❤️ using C# and SQL Server*

</div>
