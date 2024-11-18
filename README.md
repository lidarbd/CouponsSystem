# Coupon Management System

This project is a Coupon Management System designed for businesses to manage and apply coupons, offering customers discounts on their purchases. The system supports the creation, application, and management of coupon codes, with a focus on ensuring secure and efficient usage.

## Key Features:

* Enable users (anonymous customers) to input coupon codes and receive discounts.
* Provide an admin panel for business administrators to manage coupons and monitor their usage.
* Ensure seamless integration with a MySQL database, using Entity Framework Core as the ORM for database interactions.
* Implement a logical backend system in C# with .NET, with API endpoints for integration with a front-end.

## System Features:

* Admin Panel: Allows business administrators to manage coupons, including:
  * Creating, updating, and deleting coupons.
  * Tracking coupon details, such as discount type, expiration date, and usage limits.
  * Viewing reports on coupon usage and creation.

* Customer Features:
  * Apply one or more valid coupon codes to receive discounts.
  * Coupons can provide percentage-based or flat discounts, with optional expiration dates, usage limits, and stacking rules..

* Database Integration: The system uses MySQL with Entity Framework Core for data management.

## System Features:

1. Create a Database: First, create a MySQL database named coupons_system to store the necessary data.
2. Migrations: Run Update-Database to apply the migrations and connect the project to the MySQL database.
3. Run the Server: After setting up the database, run the server.
4. API Requests: Use the provided Postman collection to test the API endpoints.
