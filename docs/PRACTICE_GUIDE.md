# Practice Guide

Use this guide when you want to learn the architecture by rebuilding features inside the boilerplate.

The goal is not to memorize folder names. The goal is to practice moving from a business idea to domain model, application use case, persistence, and API endpoint without mixing responsibilities.

## Practice Loop

For every feature, repeat this loop:

1. Write the user action in one sentence.
2. Name the domain concept involved.
3. Add or update the domain model.
4. Define the application use case.
5. Add persistence only after the use case is clear.
6. Expose the use case through the API.
7. Test the endpoint.
8. Refactor names and boundaries while the feature is still small.

Example:

```text
User action: An author creates a blog post.
Domain concept: Blog
Use case: CreateBlog
Persistence: BlogRepository
API: POST /api/blog
```

## Seven-Day Practice Path

### Day 1: Understand The Layers

Trace the current `GET /api/blog` flow:

```text
BlogController -> IBlogService -> BlogService -> IBlogRepository -> BlogRepository -> AppDbContext
```

Practice tasks:

- Explain what each layer owns.
- Remove unused `using` statements.
- Build the solution.
- Run the API and open the OpenAPI UI.

Done when:

- You can explain why controllers should not query `AppDbContext` directly.
- `dotnet build .\DomainDrivenDesign.slnx` succeeds.

### Day 2: Finish Blog CRUD

The repository and service already support create, update, delete, and get-by-id. The API controller currently exposes only `GET /api/blog`.

Practice tasks:

- Add `GET /api/blog/{id}`.
- Add `POST /api/blog`.
- Add `PUT /api/blog/{id}`.
- Add `DELETE /api/blog/{id}`.
- Return useful status codes such as `200`, `201`, `204`, and `404`.

Done when:

- You can create, read, update, and delete a blog through HTTP requests.
- The controller still depends on `IBlogService`, not `BlogRepository`.

### Day 3: Add Business Rules

Move beyond plain CRUD.

Practice tasks:

- Require `Name`.
- Require `Author`.
- Set a maximum length for `Description`.
- Prevent empty image URLs if your feature requires images.
- Decide whether validation belongs in the API, Application, or Domain layer.

Done when:

- Invalid requests fail before writing to the database.
- Business rules are not hidden inside the controller.

### Day 4: Practice EF Core Migrations

Practice tasks:

- Add a new field to `Blog`, such as `CreatedAtUtc`.
- Add an EF Core migration.
- Inspect the generated migration before applying it.
- Update the database.

Commands:

```powershell
dotnet ef migrations add AddBlogCreatedAtUtc --project .\DomainDrivenDesign.Infrastructure --startup-project .\DomainDrivenDesign.API
dotnet ef database update --project .\DomainDrivenDesign.Infrastructure --startup-project .\DomainDrivenDesign.API
```

Done when:

- The database schema matches the entity.
- You understand which project contains migrations and which project starts the app.

### Day 5: Add Tests

Add a test project before the codebase grows.

Practice tasks:

- Create an application test project.
- Test `BlogService` using a fake `IBlogRepository`.
- Add at least one success case and one failure or edge case.

Suggested project:

```powershell
dotnet new xunit -n DomainDrivenDesign.Application.Tests
dotnet sln .\DomainDrivenDesign.slnx add .\DomainDrivenDesign.Application.Tests\DomainDrivenDesign.Application.Tests.csproj
dotnet add .\DomainDrivenDesign.Application.Tests reference .\DomainDrivenDesign.Application
```

Done when:

- You can test application behavior without a real database.

### Day 6: Add A Second Feature

Add a feature that relates to `Blog`, such as `Category`, `Post`, or `Comment`.

Practice tasks:

- Add the new entity.
- Add repository and service contracts.
- Implement persistence.
- Add controller endpoints.
- Add a migration.

Done when:

- The second feature follows the same flow as `Blog`.
- You can see repeated boilerplate and decide what should stay explicit.

### Day 7: Review And Refactor

Practice tasks:

- Rename vague folders such as `Interface` if you prefer `Repositories` or `Abstractions`.
- Move DTOs into the Application or API layer depending on how you use them.
- Check nullable warnings.
- Remove unused dependencies.
- Make sure the Domain project still has no framework dependencies.

Done when:

- The architecture is clearer than when you started.
- You can start another project from the template with fewer decisions.

## Feature Design Template

Copy this section into an issue or notes file when planning a feature.

```text
Feature:

User action:

Domain concepts:

Rules:

Application use cases:

API endpoints:

Persistence needs:

Migration needed:

Tests:
```

## Common Mistakes To Watch For

- Putting EF Core attributes or DbContext access into the Domain project too early.
- Letting controllers contain business rules.
- Returning EF entities directly forever instead of introducing DTOs when API shape matters.
- Creating repository methods before the use case is clear.
- Adding abstractions just because the folder structure exists.
- Forgetting to inspect generated migrations.

## Good Practice Standard

A feature is complete enough for practice when:

- It has one clear business use case.
- The controller is thin.
- The application service owns orchestration.
- The domain model expresses the business language.
- Infrastructure details stay in Infrastructure.
- The API can be exercised through HTTP.
- The solution builds cleanly.
