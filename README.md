** ASP.NET Core MVC T-Shirt Order Entry System
A relational order processing and transactional e-commerce web application developed in C# using ASP.NET Core 8.0 MVC and Entity Framework Core. Built as part of the Management Information Systems (MIS 333K) curriculum at UT Austin.

** Project Overview
The T-Shirt Order Entry System provides a structured web-based workflow for customers to browse custom t-shirt products, build multi-item orders, and complete transactional checkouts. The system enforces relational data integrity across products, order line items, customer accounts, and real-time inventory management.

Tech Stack & Architecture
- Backend: C#, ASP.NET Core 8.0 MVC
- ORM & Database: Entity Framework Core (Code-First), Azure SQL Database / Microsoft SQL Server
- Frontend: Razor Views, HTML5, CSS3, Bootstrap 5
- Tools & Environment: Visual Studio, Git & GitHub, Azure App Service

Key Features & System Architecture
Relational Order Processing Engine
- Dynamic shopping cart and line-item aggregation supporting multi-item orders with size, color, and quantity tracking.
- Real-time order calculations including automated sales tax computations and total price summaries.
- Dynamic stock availability verification preventing order placement for out-of-stock items.

Entity Framework Core & Azure Integration
- Code-First database schema modeling one-to-many and many-to-many relationships across Customers, Orders, Products, and Order Details.
- Cloud database integration leveraging Azure SQL Server for secure data persistence and production deployment.

Transactional Integrity & Search
- Server-side and client-side model validation enforcing data consistency across order creation, modification, and submission.
- Multi-criteria product search and filtering across categories, price points, and inventory status.
