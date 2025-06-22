📌 Project Summary – TaskProTracker Minimal API

TaskProTracker is a clean, modular, and production-ready .NET 8/9 Minimal API built for managing user tasks efficiently. This API is designed with best practices in mind, supporting core enterprise features like authentication, validation, logging, and structured documentation.

⚙️ Current Tech Stack

* .NET 8 / .NET 9 – Backend framework using Minimal API pattern
* Entity Framework Core – ORM for database interactions
* Microsoft Identity / PasswordHasher – For secure password handling
* JWT (JSON Web Tokens) – Authentication mechanism
* Serilog – Structured logging
* Swagger / Swashbuckle – API documentation and testing
* SQL Server – Relational database

🚀 Included Features


| Feature                  | Description                                                               |
| -------------------------| ------------------------------------------------------------------------- |
| ✅ Authentication        | Login endpoint issues JWT tokens after verifying hashed passwords         |
| ✅ Authorization         | Role-based access using `[Authorize]` attribute or endpoint configuration |
| ✅ Validation            | Model and DTO validations using `System.ComponentModel.DataAnnotations`   |
| ✅ Logging               | Application logging with **Serilog**, configurable for console or file    |
| ✅ Swagger UI            | Interactive API documentation and request testing using **SwaggerGen**    |
| ✅ Modular Endpoints     | Grouped route mappings for better organization and readability            |

Swagger UI Screenshot:

![image](https://github.com/user-attachments/assets/9a1bb1a2-7127-464b-83ef-fa44317e58e4)

Test Explorer for this project:

![image](https://github.com/user-attachments/assets/fdc7e264-3950-4eab-81f0-6142753878fa)

