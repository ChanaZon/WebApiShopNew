
You can explore our API using the provided Swagger documentation [here](<link-to-swagger>).

## Project Structure

Our project is organized into several layers to promote separation of concerns and maintainability. Here's a brief overview of each layer and the reasoning behind this structure:

- **DTO (Data Transfer Objects) Layer**: This layer contains records that represent the data structures used for communication between the client and the server. It ensures that only required data is exposed and transferred.
  - **Why use records in DTO?**: Using records in DTOs offers several advantages. Records provide a concise and immutable way to define data structures, reducing boilerplate code and preventing accidental modifications. They also support value equality, which is useful for comparing DTOs.
- **Service Layer**: Contains business logic and handles the core functionality of the application. It processes data received from the DTO layer and interacts with the Repository layer.
- **Repository Layer**: Responsible for data access and persistence. It communicates with the database and performs CRUD operations.
- **Controller Layer**: The entry point for API requests. It receives HTTP requests, invokes the appropriate service methods, and returns HTTP responses.
- **Entities Layer**: Contains the entity classes that represent the data model of the application. These classes are used by the Repository layer to interact with the database.
  - **Why use an Entities Layer?**: The Entities layer encapsulates the database schema and provides a clear separation between the data model and the business logic. This separation improves maintainability and allows for easier updates to the data model without affecting the business logic.

### Why These Layers?

- **Separation of Concerns**: Each layer has a distinct responsibility, making the codebase easier to manage, understand, and test.
- **Maintainability**: Changes in one layer (e.g., changing the database) do not directly affect other layers.
- **Scalability**: The layered architecture supports scalability by allowing independent scaling of each layer.

## AutoMapper for Layer Conversion

We use AutoMapper to handle the conversion between different layers. This ensures efficient and error-free mapping of data structures, reducing boilerplate code and potential bugs.

## Dependency Injection (DI)

Layers interact with each other through Dependency Injection (DI). This promotes loose coupling between components, making the system easier to test and maintain.

## Scalability with Async/Await

To enhance scalability, we use `async` and `await` for asynchronous programming. This allows our API to handle a larger number of concurrent requests efficiently.

## Database

We use SQL as our database with a Code First approach. If you need to create the database, use the following commands:

Add-migration MyShop_214859456
Update-database

## Configuration Files

Our project leverages configuration files to manage settings and environment-specific configurations. This ensures flexibility and ease of deployment.

## Error Handling

All errors are handled by a custom middleware. Errors are logged using a logger and sent via email. This ensures robust error management and quick response to critical issues.

## Request Logging

Every request to the system is logged for rating and analysis purposes. This helps in monitoring usage patterns and improving the system based on real-world data.

## Clean Code

We adhere to clean code principles, ensuring that our codebase is readable, maintainable, and scalable.

---
