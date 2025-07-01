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

---

## 🧪 Running Tests

```bash
dotnet test TaskFlow.Tests
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
