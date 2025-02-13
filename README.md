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







