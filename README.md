# 📋 TaskFlow

🎯 A clean-architecture based task management system built with ASP.NET Core.  
TaskFlow helps teams organize, prioritize, and track tasks efficiently.

---

## 🏗️ Architecture

This project follows the principles of **Clean Architecture**, separating concerns into distinct layers:

```
- 📦 TaskFlow.Domain
- 📦 TaskFlow.Application
- 📦 TaskFlow.Infrastructure
- 📦 TaskFlow.Persistence
- 📦 TaskFlow.API
- 🧪 TaskFlow.Tests
```

This structure ensures scalability, testability, and maintainability.

---

## 🚀 Features

- ✅ Create, update, and delete tasks
- ✅ Assign tasks to users
- ✅ Set priorities and deadlines
- ✅ Track task status (ToDo, InProgress, Done)
- ✅ Architecture based on Clean Architecture principles
- ✅ Entity Framework Core integration

---

## 🧠 Tech Stack

| Layer        | Technology            |
| ------------ | --------------------- |
| Backend      | ASP.NET Core          |
| Data Access  | Entity Framework Core |
| Architecture | Clean Architecture    |
| Database     | SQL Server            |
| Testing      | xUnit + Moq           |

---

## 🖥️ Project Structure

```text
TaskFlow/
├── TaskFlow.Domain
├── TaskFlow.Application
├── TaskFlow.Infrastructure
├── TaskFlow.Persistence
├── TaskFlow.WebAPI
└── TaskFlow.Tests
```

---

## ⚙️ Getting Started

### Prerequisites

- [.NET SDK 9.0+](https://dotnet.microsoft.com/en-us/download)
- SQL Server or LocalDB

### Setup Instructions

1. Clone the repo:

   ```bash
   git clone https://github.com/rralireza/TaskFlow-ASPNETCore-CleanArchitecture.git
   cd TaskFlow
   ```

2. Apply EF Core migrations:

   ```bash
   dotnet ef database update --project TaskFlow.Infrastructure
   ```

3. Run the API:

   ```bash
   dotnet run --project TaskFlow.WebAPI
   ```

4. Open in browser:
   ```
   https://localhost:7000/
   ```

### React web UI

The RTL React dashboard lives in `TaskFlow.Web` and uses the API endpoints for authentication, projects, and task creation.

```bash
cd TaskFlow.Web
npm install
npm run dev
```

> فایل `index.html` را مستقیماً با دوبارکلیک باز نکنید؛ React/Vite باید از طریق dev server اجرا شود. سپس آدرس `http://localhost:5173` را در مرورگر باز کنید.

By default the UI connects to `http://localhost:5017/api`. Set `VITE_API_URL` when the API is running at another address, for example `VITE_API_URL=https://localhost:7000/api npm run dev`.

---

## 🧪 Running Tests

```bash
dotnet test TaskFlow.Tests
```

---

## Docker

1. Create your local environment file:

   ```bash
   cp .env.example .env
   ```

   In PowerShell:

   ```powershell
   Copy-Item .env.example .env
   ```

2. Replace the example values in `.env`, particularly `MSSQL_SA_PASSWORD` and `JWT_KEY`.

3. Build and start the API and SQL Server:

   ```bash
   docker compose up --build
   ```

   The API is available at `http://localhost:8080`. Database migrations run automatically when the API starts. SQL Server data is retained in the `sqlserver-data` Docker volume.

4. Stop the stack with:

   ```bash
   docker compose down
   ```

---

## ✨ Future Improvements

- [ ] Add user authentication and roles
- [ ] Add support for notifications
- [ ] UI Frontend with React or Blazor
- [ ] Dockerize the application
- [ ] CI/CD pipeline

---

## 🙌 Contributing

I welcome contributions! If you'd like to fix a bug or suggest an enhancement:

1. Fork the repository
2. Create a new branch (`feature/my-feature`)
3. Commit your changes
4. Open a Pull Request

---

## 📫 Contact

Made with ❤️ by Alireza Nikandish  
🔗 [LinkedIn](https://www.linkedin.com/) | 🐙 [GitHub](https://github.com/rralireza)
