﻿# EF Core Migrations & Rollback Guide

This document provides a comprehensive reference for managing Entity Framework Core migrations. Whether you are using the Package Manager Console (PMC) in Visual Studio or the .NET CLI, these commands will help you evolve your database schema safely and efficiently.

## Table of Contents

- [Introduction](#introduction)
- [Package Manager Console Commands](#package-manager-console-commands)
- [.NET CLI Commands](#net-cli-commands)
- [Common Migration Scenarios](#common-migration-scenarios)
- [Troubleshooting & Best Practices](#troubleshooting--best-practices)
- [Additional Resources](#additional-resources)
- [General](#general)

## Introduction

EF Core migrations allow you to incrementally update your database schema as your application's data model changes. Migrations help keep your database in sync with your model by generating code to transform the database schema. This guide outlines commands for:
- Enabling migrations (if needed)
- Creating new migrations
- Applying migrations to update the database
- Removing migrations when necessary
- Rolling back applied migrations

> **Note:** In EF Core, the "Enable-Migrations" command is not required as migrations are enabled implicitly when you add your first migration. This command is retained for developers familiar with EF6.

## Package Manager Console Commands

If you work within Visual Studio using the Package Manager Console, here are the key commands:

### 1. Enable Migrations (EF6 Style)
For EF Core, enabling migrations is implicit. However, for EF6 you would run:
```powershell
Enable-Migrations
```

### 2. Add a New Migration
After modifying your model, create a new migration by running:
```powershell
Add-Migration "MigrationName"
```
Replace `"MigrationName"` with a descriptive name (e.g., `AddCustomerTable`).

### 3. Update the Database
Apply all pending migrations to your database:
```powershell
Update-Database
```

### 4. Remove the Last Migration
If the last migration has not yet been applied to the database, you can remove it with:
```powershell
Remove-Migration
```

### 5. Rollback Applied Migrations
If a migration has already been applied and you need to roll it back, update the database to a previous state. For example, to rollback all migrations (i.e., revert to the initial state):
```powershell
Update-Database -Migration:0
```
Alternatively, to rollback only to a specific migration, use:
```powershell
Update-Database -Migration "PreviousMigrationName"
```

### 6. List Available Migrations
To view all migrations in your project:
```powershell
Get-Migrations
```

## .NET CLI Commands

For developers who prefer using the command line, EF Core supports migration commands via the .NET CLI:

### 1. Add a New Migration
```bash
dotnet ef migrations add MigrationName
```

### 2. Update the Database
```bash
dotnet ef database update
```

### 3. Remove the Last Migration
```bash
dotnet ef migrations remove
```

### 4. Generate a SQL Script for Migrations
To review or manually apply the SQL changes:
```bash
dotnet ef migrations script
```

## Common Migration Scenarios

- **Creating a New Migration:**  
  After updating your data model, run the migration add command to scaffold the necessary changes.
  - **PMC:** `Add-Migration "DescriptiveMigrationName"`
  - **CLI:** `dotnet ef migrations add DescriptiveMigrationName`

- **Updating the Database:**  
  Apply the migration to your database.
  - **PMC:** `Update-Database`
  - **CLI:** `dotnet ef database update`

- **Rolling Back a Migration:**  
  To undo changes:
  - If the migration hasn't been applied, simply remove it:
    - **PMC:** `Remove-Migration`
    - **CLI:** `dotnet ef migrations remove`
  - If already applied, rollback to a previous migration:
    - **PMC:** `Update-Database -Migration "TargetMigrationName"` or use `-Migration:0` to revert all changes
    - **CLI:** `dotnet ef database update TargetMigrationName`

- **Reviewing Migration History:**  
  Check which migrations have been applied:
  - **PMC:** `Get-Migrations`

## Troubleshooting & Best Practices

- **Backup Your Database:** Always back up your database before applying migrations in a production environment.
- **Review Generated SQL:** Use `dotnet ef migrations script` to review the SQL generated by your migrations.
- **Incremental Changes:** Apply migrations incrementally rather than batching many changes together.
- **Consistent Naming:** Use clear, descriptive names for migrations to ease tracking changes.
- **Version Control:** Keep migration files under version control with your code to ensure team consistency.


---

## General 

**How to override the table name**

- using data-anotations

```csharp
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCore.Data
{
    [Table("Blogs")]
    public class Blog
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public List<Post> Posts { get; set; }
    }
}
```

- using FluentApi

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCore.Data.Configurations
{
    public class BlogEntityTypeConfiguration : IEntityTypeConfiguration<Blog>
    {
        public void Configure(EntityTypeBuilder<Blog> builder)
        {
           builder.ToTable("Blooooooogs");
           builder.Property(b=>b.Url).IsRequired().HasDefaultValue("https://example.com");
        }
    }
}
```
----
**How to override defualt schema "dbo"**

- using data-annotations

```csharp
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCore.Data
{
    [Table("Posts", Schema = "Core")]
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public Blog Blog { get; set; }
    }
}
```

- **Using FluentApi**

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCore.Data.Configurations
{
    public class BlogEntityTypeConfiguration : IEntityTypeConfiguration<Blog>
    {
        public void Configure(EntityTypeBuilder<Blog> builder)
        {
            builder.ToTable("Blooooooogs", schema: "Core");
            builder.Property(b => b.Url).IsRequired().HasDefaultValue("https://example.com");
        }
    }
}
```

- change the defualt schema

```csharp
namespace EFCore.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions) : base(dbContextOptions) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            new BlogEntityTypeConfiguration().Configure(modelBuilder.Entity<Blog>());
            modelBuilder.Ignore<AuditEntry>();
            modelBuilder.HasDefaultSchema("Core");
        }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
    }
}
```

---
**How to execlude property**

- Using Data-Anotations

```csharp
[NotMapped]
public string AddedOn { get; set; }
```

- Using FluentApi 

```csharp
 modelBuilder.Entity<Post>()
                .Ignore(p => p.AddedOn);
```
---

**How to change column name**

- Using data-anotations

```csharp
[Column("BlogUrl")]
public string Url { get; set; }
```

- Using fluentApi

```csharp
modelBuilder.Entity<Blog>()
                .Property(p => p.Url)
                .HasColumnName("url");
```

---

**How to change column data type**

- Using data-anotations

```csharp
[Column(TypeName = "varchar(200)")]
public string Title { get; set; }
```

- Using fluentApi

```csharp
modelBuilder.Entity<Blog>(eb =>
        {
            eb.Property(b=>b.Url).HasColumnType("varchar(250)");
        });
```

---

**How to apply MaxLength**

- using data-anotations.
```csharp
[MaxLength(200)]
public string Url { get; set; }
```

- using fluentApi

```csharp
modelBuilder.Entity<Blog>(eb =>
        {
            eb.Property(b=>b.Url).HasMaxLength(3000);
        });
```

---

**How to set primary key**

- Using Data-Anotations

```csharp
[Key]
public int Id { get; set; }
```

- Using FluentApi

```csharp
modelBuilder.Entity<Blog>()
            .HasKey(p => p.Id);
```

----

**How to add composite-key**

using only fluentApi

```csharp
modelBuilder.Entity<Blog>()
.HasKey(p => new { p.Id, p.Url });
```

---

**How to set Defualt Value**

- using fluentApi

```csharp
modelBuilder.Entity<Blog>(eb =>
        {
            eb.Property(b => b.Url).HasMaxLength(3000).HasDefaultValue("https://example.com");
        });
```

```csharp
 modelBuilder.Entity<Blog>()
     .Property<DateTime>(b => b.CreatedAt)
     .HasDefaultValueSql("GETDATE()");
```
---

**How To Configure Computed Columns**

```csharp
modelBuilder.Entity<Author>()
    .Property<string>(p => p.Name)
    .HasComputedColumnSql("[LastName] + ',' + [FirstName]");
```

---

**One-To-One Relationship**

EF core will recognize the following relationship and identify the owner of the relation 
```csharp
public class Author
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Account Account { get; set; }
}

public class Account
{
    public int Id { get; set; }
    public string Type { get; set; }
    public int AuthorId { get; set; }
    public Author Author { get; set; }
}
```

but on the following may not recognize it

```csharp
public class Author
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Account Account { get; set; }
}

public class Account
{
    public int Id { get; set; }
    public string Type { get; set; }
    public int ForienKeyAuthor { get; set; }
    public Author Author { get; set; }
}
```

we can solve it using **FluentApi**

```csharp
modelBuilder.Entity<Author>()
            .HasOne(a => a.Account)
            .WithOne(a => a.Author)
            .HasForeignKey<Account>(a => a.ForienKeyAuthor );
```

---

**One-To-Many Relationship**


example code :

```csharp
public class Blog
{
    public int Id { get; set; }
    public string Url { get; set; }  

    public List<Post> Posts { get; set; }
}

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public int BlogId { get; set; }
    public Blog Blog { get; set; }
}
```

EF will generate all relations about above but let's try using **FluentApi**

```csharp
modelBuilder.Entity<Blog>()
    .HasMany(b => b.Posts)
    .WithOne(p => p.Blog);

// or

modelBuilder.Entity<Post>()
    .HasOne(p => p.Blog)
    .WithMany(b => b.Posts);
```

---

**Many-To-Many Relationship**

example code :

```csharp
public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public int BlogId { get; set; }
    public Blog Blog { get; set; }

    public ICollection<Tag> Tags { get; set; }
}

public class Tag
{
    public int Id { get; set; }

    public ICollection<Post> Posts { get; set; }
}
```

using **FluentApi**

```csharp
modelBuilder.Entity<Post>()
    .HasMany(p => p.Tags)
    .WithMany(t => t.Posts)
    .UsingEntity(j => j.ToTable("PostTagsTest"));
```


---

**Indexing**

- using data-anotations

```csharp
[Index(nameof(Title))]
public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public int BlogId { get; set; }
    public Blog Blog { get; set; }

    public ICollection<Tag> Tags { get; set; }
} 
```

- using FluentApi

```csharp
modelBuilder.Entity<Post>()
            .HasIndex(p => p.Tags);
```

---

**Composite Index**

- using data-anotations

```csharp
[Index(nameof(Title), nameof(Content))]
public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public int BlogId { get; set; }
    public Blog Blog { get; set; }

    public ICollection<Tag> Tags { get; set; }
} 
```

- using FluentApi

```csharp
modelBuilder.Entity<Post>()
                .HasIndex(p => new {p.Tags, p.Title});
```
---

**Index Uniqness**

- using data-anotations

```csharp
[Index(nameof(Title), IsUnique = true)]
```

- using fluentApi

```csharp
modelBuilder.Entity<Post>()
            .HasIndex(p => new { p.Tags, p.Title })
            .IsUnique();
```

---

**Index Name**

- using data-anotations

```csharp
[Index(nameof(Title), Name = "Title_Index")]
```

- using fluentApi

```csharp
modelBuilder.Entity<Post>()
            .HasIndex(p => new { p.Tags, p.Title })
            .HasDatabaseName("Tags_Title_IDX");
```

---

**Sequence**

```csharp
modelBuilder.HasSequence<int>("OrderNumber");

modelBuilder.Entity<Post>()
    .Property(p => p.Id)
    .HasDefaultValueSql("NEXT VALUE FOR OrderNumber");
```

---

**Data-Seeding**    

```csharp
modelBuilder.Entity<Post>()
    .HasData(new Post { Id = 1, Title = "Test" });
```

---

## Migrations: .NET CLI vs. Visual Studio

This document compares working with database migrations using the .NET CLI and Visual Studio's Package Manager Console.

### Comparison Table

| Feature                 | .NET CLI Commands                             | Visual Studio (PMC) Commands                 |
|-------------------------|---------------------------------------------|---------------------------------------------|
| **Add Migration**       | `dotnet ef migrations add <MigrationName>`  | `Add-Migration <MigrationName>`            |
| **Remove Migration**    | `dotnet ef migrations remove`               | `Remove-Migration`                         |
| **Update Database**     | `dotnet ef database update`                 | `Update-Database`                          |
| **Revert to Migration** | `dotnet ef database update <MigrationName>` | `Update-Database -Migration <MigrationName>` |
| **List Migrations**     | `dotnet ef migrations list`                 | `Get-Migrations`                           |
| **Generate SQL Script** | `dotnet ef migrations script`               | `Script-Migration`                         |
| **Specify Connection**  | `--connection "<ConnectionString>"`         | `-Connection "<ConnectionString>"`         |
| **Apply to Specific Context** | `--context <DbContextName>`        | `-Context <DbContextName>`                 |


---

## Working With Database Scaffolding

Database Scaffolding in Entity Framework Core
Database scaffolding in Entity Framework Core allows you to generate entity classes and a DbContext based on an existing database. This is useful when working with legacy databases or when you want to generate models quickly from a pre-existing schema.


### Using Package Manager Console (PMC)
Run the following command inside Visual Studio's **Package Manager Console**:

```powershell
Scaffold-DbContext "Server=YOUR_SERVER;Database=YOUR_DATABASE;User Id=YOUR_USER;Password=YOUR_PASSWORD" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models
```

### Using .NET CLI
Run the following command in the terminal:

```bash
dotnet ef dbcontext scaffold "Server=YOUR_SERVER;Database=YOUR_DATABASE;User Id=YOUR_USER;Password=YOUR_PASSWORD" Microsoft.EntityFrameworkCore.SqlServer --output-dir Models
```

### Additional Options
You can customize the scaffolding process with the following parameters:

| Option | Description |
|--------|-------------|
| `-Tables Table1,Table2` | Scaffold only specific tables. |
| `-Schema dbo` | Specify a schema to scaffold from. |
| `-Context MyDbContext` | Rename the DbContext. |
| `-DataAnnotations` | Use data annotations instead of Fluent API. |
| `-Force` | Overwrite existing models if they already exist. |

#### Example:
```powershell
Scaffold-DbContext "Server=YOUR_SERVER;Database=YOUR_DATABASE;User Id=YOUR_USER;Password=YOUR_PASSWORD" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -Tables Customers,Orders -Context MyAppDbContext -DataAnnotations
```

After running the command, the **Models** folder will contain:
- Entity classes for each table.
- A `DbContext` class to interact with the database.

If you modify the database schema later, re-run the command with `-Force` to regenerate models. 🚀

---

# General In Code


### to get all data with in a table

- using `ToList`

```csharp
var resultSync = _dbContext.Blogs.ToList();
var resultAsync = await _dbContext.Blogs.ToListAsync();
```

# Entity Framework: Single(), SingleOrDefault(), Find(), First(), and FirstOrDefault()

In Entity Framework, `Single()`, `SingleOrDefault()`, `Find()`, `First()`, and `FirstOrDefault()` are methods used to retrieve a single entity from a database query. They operate on `IQueryable<T>` or `IEnumerable<T>` collections and are commonly used when querying records.

## Differences Between Single(), SingleOrDefault(), Find(), First(), and FirstOrDefault()

| Method               | If No Record Found | If One Record Found | If Multiple Records Found |
|----------------------|-------------------|---------------------|---------------------------|
| `Single()`          | Throws Exception  | Returns the record  | Throws Exception          |
| `SingleOrDefault()` | Returns `null`    | Returns the record  | Throws Exception          |
| `Find()`            | Returns `null`    | Returns the record  | Returns the first match   |
| `First()`           | Throws Exception  | Returns the record  | Returns the first match   |
| `FirstOrDefault()`  | Returns `null`    | Returns the record  | Returns the first match   |

## When to Use Which?
- Use `Single()` when you are **certain** there will be exactly **one record** (e.g., searching by a unique identifier).
- Use `SingleOrDefault()` when a record **may or may not exist**, but you still expect at most **one** record.
- Use `Find()` when searching by **primary key** as it provides efficient lookups using the context's cache before querying the database.
- Use `First()` when you want the **first matching record** and expect **at least one record** to exist.
- Use `FirstOrDefault()` when you want the **first matching record**, but it is possible that no records exist.

## Example Usage

### Using `Single()`
```csharp
var user = dbContext.Users.Single(u => u.Email == "test@example.com");
```
- If **one user** exists with that email → returns the user.
- If **no user** exists → throws `InvalidOperationException`.
- If **multiple users** exist → throws `InvalidOperationException`.

### Using `SingleOrDefault()`
```csharp
var user = dbContext.Users.SingleOrDefault(u => u.Email == "test@example.com");
```
- If **one user** exists → returns the user.
- If **no user** exists → returns `null`.
- If **multiple users** exist → throws `InvalidOperationException`.

### Using `Find()`
```csharp
var user = dbContext.Users.Find(1);
```
- If the user with ID `1` exists → returns the user.
- If no user exists → returns `null`.
- If multiple users exist (which shouldn't happen for a primary key search) → returns the first match.

### Using `First()`
```csharp
var user = dbContext.Users.First(u => u.Age > 18);
```
- If **at least one user** exists → returns the first matching user.
- If **no users** exist → throws `InvalidOperationException`.
- If **multiple users** exist → returns the first match.

### Using `FirstOrDefault()`
```csharp
var user = dbContext.Users.FirstOrDefault(u => u.Age > 18);
```
- If **at least one user** exists → returns the first matching user.
- If **no users** exist → returns `null`.
- If **multiple users** exist → returns the first match.

---

# Entity Framework: Where() Method

In Entity Framework, the `Where()` method is used to filter records based on a specified condition. It returns an `IQueryable<T>` or `IEnumerable<T>`, allowing further operations like sorting, grouping, or additional filtering.

## When to Use `Where()`?
- When filtering a collection based on one or more conditions.
- When retrieving multiple records that match a given criteria.
- When chaining with other LINQ methods for complex queries.

## Example Usage

### Basic Usage
```csharp
var users = dbContext.Users.Where(u => u.Age > 18);
```
- Returns all users where `Age` is greater than 18.

### Using Multiple Conditions
```csharp
var users = dbContext.Users.Where(u => u.Age > 18 && u.IsActive);
```
- Returns all active users older than 18.

### Chaining with Other Methods
```csharp
var users = dbContext.Users.Where(u => u.Age > 18).OrderBy(u => u.Name);
```

- Returns users older than 18, ordered by name.

---

# Entity Framework: Any() & All() Methods

In Entity Framework, `Any()` and `All()` are used to evaluate conditions on a collection of records. They return a `bool` value indicating whether any or all records satisfy the given condition.

## Differences Between `Any()` and `All()`

| Method   | Returns `true` If... | Returns `false` If... |
|----------|----------------------|-----------------------|
| `Any()`  | At least one record matches the condition | No records match the condition or the collection is empty |
| `All()`  | All records match the condition | At least one record does not match the condition |

## Example Usage

### Using `Any()`
```csharp
bool hasAdults = dbContext.Users.Any(u => u.Age > 18);
```
- Returns `true` if at least one user is older than 18.
- Returns `false` if no users match the condition.

### Using `All()`
```csharp
bool allAdults = dbContext.Users.All(u => u.Age > 18);
```
- Returns `true` if **all** users are older than 18.
- Returns `false` if **at least one** user is 18 or younger.

## When to Use `Any()` and `All()`?
- Use `Any()` when you need to check if **at least one** record meets a condition.
- Use `All()` when you need to check if **every record** meets a condition.

---

## Entity Framework: Append() & Prepend() Methods

In Entity Framework, `Append()` and `Prepend()` are used to add elements to an `IEnumerable<T>` collection without modifying the original collection.

### Differences Between `Append()` and `Prepend()`

| Method    | Adds Element To... |
|-----------|------------------|
| `Append()`  | End of the collection |
| `Prepend()` | Beginning of the collection |

## Example Usage

### Using `Append()`
```csharp
var users = dbContext.Users.ToList().Append(newUser);
```
- Adds `newUser` to the **end** of the collection.

### Using `Prepend()`
```csharp
var users = dbContext.Users.ToList().Prepend(newUser);
```
- Adds `newUser` to the **beginning** of the collection.

### Key Points
- These methods do **not** modify the original collection but return a **new collection**.
- Useful for adding elements without modifying the database directly.

These methods provide a simple way to extend collections efficiently in LINQ queries.

---

# Entity Framework: Average(), Count(), Sum(), Min(), & Max() Methods

In Entity Framework, `Average()`, `Count()`, `Sum()`, `Min()`, and `Max()` are used for aggregation operations on numeric collections.

## Differences Between `Average()`, `Count()`, `Sum()`, `Min()`, and `Max()`

| Method    | Purpose |
|-----------|---------|
| `Average()` | Computes the average of numeric values in a collection. |
| `Count()`   | Returns the total number of elements in a collection. |
| `Sum()`     | Calculates the sum of numeric values in a collection. |
| `Min()`     | Finds the smallest numeric value in a collection. |
| `Max()`     | Finds the largest numeric value in a collection. |

### Example Usage
    
### Using `Average()`
```csharp
var averageAge = dbContext.Users.Average(u => u.Age);
```
- Returns the **average age** of all users.

### Using `Count()`
```csharp
var userCount = dbContext.Users.Count();
```
- Returns the **total number of users** in the database.

### Using `Sum()`
```csharp
var totalSalary = dbContext.Users.Sum(u => u.Salary);
```
- Returns the **total salary** of all users.

### Using `Min()`
```csharp
var minSalary = dbContext.Users.Min(u => u.Salary);
```
- Returns the **minimum salary** among all users.

### Using `Max()`
```csharp
var maxSalary = dbContext.Users.Max(u => u.Salary);
```
- Returns the **maximum salary** among all users.

## Key Points
- `Average()`, `Sum()`, `Min()`, and `Max()` work only on numeric values.
- `Count()` can be used with or without a condition.
- These methods are efficient for performing calculations directly in the database.

---

# Entity Framework: OrderBy() Method

In Entity Framework, the `OrderBy()` method is used to sort a collection in ascending order based on a specified key.

### Syntax
```csharp
var sortedUsers = dbContext.Users.OrderBy(u => u.Name).ToList();
```
- Sorts users by their **Name** in ascending order.

### Using `OrderByDescending()`
To sort in descending order, use `OrderByDescending()`:
```csharp
var sortedUsersDesc = dbContext.Users.OrderByDescending(u => u.Name).ToList();
```
- Sorts users by **Name** in descending order.

### Sorting by Multiple Columns
Use `ThenBy()` and `ThenByDescending()` for multi-level sorting:
```csharp
var sortedUsers = dbContext.Users
    .OrderBy(u => u.Age)
    .ThenBy(u => u.Name)
    .ToList();
```
- Sorts by **Age** first, then by **Name** within the same age group.

### Key Points
- `OrderBy()` sorts in **ascending order**.
- `OrderByDescending()` sorts in **descending order**.
- Use `ThenBy()` and `ThenByDescending()` for multi-level sorting.

This method is useful for organizing query results efficiently in Entity Framework.


---

# Entity Framework: Select() Method

In Entity Framework, the `Select()` method is used to project and transform data from a collection.

### Syntax
```csharp
var userNames = dbContext.Users.Select(u => u.Name).ToList();
```
- Retrieves only the **Name** field from the `Users` table.

### Selecting Multiple Fields
You can select multiple fields using an anonymous type:
```csharp
var userDetails = dbContext.Users.Select(u => new { u.Name, u.Age }).ToList();
```
- Returns a list of objects containing **Name** and **Age**.

### Transforming Data
You can modify the data while selecting:
```csharp
var userInfo = dbContext.Users.Select(u => new { FullName = u.Name, BirthYear = DateTime.Now.Year - u.Age }).ToList();
```
- Renames **Name** to `FullName` and calculates the **BirthYear**.

### Key Points
- `Select()` is used for **projection** (choosing specific fields).
- Can return **single or multiple** fields.
- Allows **data transformation** during selection.

This method is useful for optimizing queries and fetching only the required data in Entity Framework.

---

# Entity Framework: Distinct() Method

In Entity Framework, the `Distinct()` method is used to remove duplicate values from a collection.

### Syntax
```csharp
var uniqueNames = dbContext.Users.Select(u => u.Name).Distinct().ToList();
```
- Retrieves a **list of unique names** from the `Users` table.

### Using `Distinct()` on Multiple Fields
To apply `Distinct()` on multiple fields, use an anonymous type:
```csharp
var uniqueUsers = dbContext.Users.Select(u => new { u.Name, u.Age }).Distinct().ToList();
```
- Returns a **list of unique user records** based on `Name` and `Age`.

### Key Points
- `Distinct()` removes **duplicate values** from a collection.
- Works efficiently with **single fields**.
- When using **multiple fields**, it requires **anonymous types**.

This method is useful for filtering out duplicate data efficiently in Entity Framework.

---

# Entity Framework: Skip() & Take() Methods

In Entity Framework, `Skip()` and `Take()` are used for **pagination** and retrieving a specific subset of records.

## Syntax
### Using `Skip()`
```csharp
var skippedUsers = dbContext.Users.Skip(5).ToList();
```
- Skips the **first 5 users** and retrieves the remaining records.

### Using `Take()`
```csharp
var firstFiveUsers = dbContext.Users.Take(5).ToList();
```
- Retrieves **only the first 5 users**.

### Using `Skip()` and `Take()` Together
```csharp
var paginatedUsers = dbContext.Users.Skip(10).Take(5).ToList();
```
- Skips the **first 10 users** and retrieves the **next 5 users** (useful for pagination).

## Key Points
- `Skip(n)`: Ignores the first **n** records.
- `Take(n)`: Retrieves the first **n** records.
- Used together for **pagination** and limiting query results.

This method is useful for handling large datasets efficiently in Entity Framework.

---

# Entity Framework: GroupBy() Method

In Entity Framework, the `GroupBy()` method is used to group records based on a specific key.

## Syntax
```csharp
var usersGroupedByAge = dbContext.Users
    .GroupBy(u => u.Age)
    .Select(g => new { Age = g.Key, Users = g.ToList() })
    .ToList();
```
- Groups users by **Age** and returns a list of users for each age group.

## Grouping with Aggregation
```csharp
var userCountByAge = dbContext.Users
    .GroupBy(u => u.Age)
    .Select(g => new { Age = g.Key, Count = g.Count() })
    .ToList();
```
- Groups users by **Age** and calculates the **count** of users in each age group.

## Key Points
- `GroupBy()` organizes data into **groups** based on a key.
- Can be combined with **aggregation functions** like `Count()`, `Sum()`, `Min()`, and `Max()`.
- Useful for **categorizing and summarizing data** in Entity Framework.

This method is helpful for analyzing grouped data efficiently in Entity Framework.


---

# Entity Framework: Inner Join using Join()

In Entity Framework, the `Join()` method is used to perform an **inner join** between two tables.

## Syntax
```csharp
var userOrders = dbContext.Users
    .Join(dbContext.Orders,
          user => user.Id,
          order => order.UserId,
          (user, order) => new { user.Name, order.OrderDate })
    .ToList();
```
- Joins `Users` and `Orders` tables based on the **UserId**.
- Selects the **User's Name** and **Order Date**.

## Key Points
- `Join()` requires **two tables** and a **common key**.
- Uses **lambda expressions** for defining join conditions.
- Returns a **new anonymous object** containing selected fields.

This method is useful for efficiently retrieving related data in Entity Framework.

---

# Entity Framework: Left Join using GroupJoin()

In Entity Framework, the `GroupJoin()` method is used to perform a **left join** between two tables.

## Syntax
```csharp
var usersWithOrders = dbContext.Users
    .GroupJoin(dbContext.Orders,
               user => user.Id,
               order => order.UserId,
               (user, orders) => new { user.Name, Orders = orders.DefaultIfEmpty() })
    .ToList();
```
- Groups `Orders` with `Users` based on the **UserId**.
- Uses `DefaultIfEmpty()` to ensure users without orders are included.

## Key Points
- `GroupJoin()` performs a **left join** by keeping all records from the **left table** (`Users`).
- `DefaultIfEmpty()` ensures that users without orders appear with **null** orders.
- Returns a **new anonymous object** containing the **User's Name** and **Orders**.

This method is useful for retrieving users **with or without** related records in Entity Framework.


---

# Entity Framework: Tracing vs. NoTracking

In Entity Framework, **tracking** determines whether retrieved entities are monitored for changes.

## 1. Tracing (Default Behavior)
By default, EF Core tracks changes to entities.

### Example
```csharp
var user = dbContext.Users.FirstOrDefault(u => u.Id == 1);
user.Name = "Updated Name";
dbContext.SaveChanges();
```
- The **context tracks changes**, so calling `SaveChanges()` updates the database.

## 2. NoTracking (Improves Performance)
Use `AsNoTracking()` to disable tracking when read-only queries are needed.

### Example
```csharp
var users = dbContext.Users.AsNoTracking().ToList();
```
- **No tracking** means EF does not monitor changes, improving performance.
- Ideal for **read-heavy operations** where no updates are needed.

## Key Differences
| Feature       | Tracing (Default) | NoTracking |
|--------------|-----------------|-----------|
| Tracks Changes | ✅ Yes | ❌ No |
| Performance  | 🔽 Slower | 🔼 Faster |
| Suitable For | Read & Write | Read-Only Queries |

Use `AsNoTracking()` when retrieving large datasets **without modifications** to optimize performance.

---

# Entity Framework: Eager Loading

Eager loading is a technique in Entity Framework that loads related entities **along with the main entity** to reduce database queries.

## Syntax
```csharp
var usersWithOrders = dbContext.Users
    .Include(u => u.Orders)
    .ToList();
```
- Uses `.Include()` to load related `Orders` when retrieving `Users`.

## Multiple Levels of Eager Loading
```csharp
var usersWithOrdersAndItems = dbContext.Users
    .Include(u => u.Orders)
        .ThenInclude(o => o.OrderItems)
    .ToList();
```
- `.ThenInclude()` loads related data at **deeper levels**.

## Key Points
- **Reduces** the number of database queries by fetching related data in **one query**.
- Uses `.Include()` for **direct relationships**.
- Uses `.ThenInclude()` for **nested relationships**.
- Best for scenarios where related data is **always required**.

This method improves efficiency but should be used **judiciously** to avoid **loading too much data** unnecessarily.

---

# Entity Framework: Explicit Loading

Explicit loading is a technique in Entity Framework where related entities are **manually loaded** after querying the main entity.

## Syntax
```csharp
var user = dbContext.Users.FirstOrDefault(u => u.Id == 1);
if (user != null)
{
    dbContext.Entry(user).Collection(u => u.Orders).Load();
}
```
- Loads `Orders` **only when needed**, reducing initial query size.

## Loading a Single Related Entity
```csharp
var user = dbContext.Users.FirstOrDefault(u => u.Id == 1);
if (user != null)
{
    dbContext.Entry(user).Reference(u => u.Profile).Load();
}
```
- `.Reference()` loads a **single related entity**.

## Key Points
- **More control** over when and what related data is loaded.
- Reduces **unnecessary data retrieval** in initial queries.
- Requires **separate database queries** for related entities.
- Best for scenarios where related data **might not always be needed**.

Explicit loading helps optimize performance while avoiding unnecessary data retrieval.

---

# Entity Framework: Lazy Loading

Lazy loading is a technique in Entity Framework where related entities are loaded **only when accessed**.

## Enabling Lazy Loading
Lazy loading requires **virtual navigation properties** and a **lazy-loading proxy**.

### Example
```csharp
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public virtual ICollection<Order> Orders { get; set; }
}
```
- Declaring `Orders` as `virtual` enables lazy loading.

## Using Lazy-Loading Proxies
Ensure the `Microsoft.EntityFrameworkCore.Proxies` package is installed, then enable proxies:
```csharp
services.AddDbContext<AppDbContext>(options =>
    options.UseLazyLoadingProxies()
           .UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
```

## Key Points
- **Loads related entities only when accessed**, reducing initial query size.
- Requires **virtual properties** and **lazy-loading proxies**.
- May cause **multiple database queries**, leading to the **N+1 problem**.
- Best for scenarios where related data is **not always needed**.

Lazy loading simplifies navigation but should be used carefully to avoid performance issues.







































