Academic Student Record System (WPF & .NET)
A desktop application designed for comprehensive management of student data and grades. The project demonstrates a robust architecture combining modern ORM techniques with advanced data handling.

 Key Features
Full Student & Grade Management: Complete CRUD operations with data validation using Regex.

Multi-Format Data Persistence: Advanced export and import system supporting JSON, XML, Binary, and Plain Text formats.

Advanced Data Presentation: Custom Value Converters for formatting complex relationships in DataGrids.

Relational Database: Integrated with SQL Server using Entity Framework Core (Migrations, Code-First approach).

Asynchronous Operations: Usage of Task.Run and async/await for non-blocking UI during data heavy import/export tasks.

Technical Stack
Language: C# (.NET 9)

Framework: WPF (Windows Presentation Foundation)

ORM: Entity Framework Core

Database: MS SQL Server

Tools: LINQ (Advanced queries, Joins, Grouping), Serializers (JsonSerializer, XmlSerializer).

Architectural Highlights
Separation of Concerns: Distinct logic for UI management, Data Models, and File Persistence Managers.

Data Integrity: Implemented checks for duplicate index numbers and cascading deletes for related grades.

Reflection: Usage of reflection for dynamic object retrieval from UI controls.
