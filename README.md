# 📚 Book-Ecommerce

![Book-Ecommerce](https://www.servcorp.co.uk/media/34561/e-commerce-img.jpeg?format=webp&quality=80&width=688)

A scalable e-commerce platform for buying books online, developed using ASP.NET MVC. This platform includes user authentication, product management, order management, and integration with Stripe for online transactions. It supports four types of users: Admin, Company, and Customer.

---

## 🚀 Features

- **User Authentication:** Login and register functionalities for all users.
- **Product Management:** Browse and manage books.
- **Order Management:** Track orders and transactions.
- **Stripe Integration:** Secure online payments.
- **User Roles:**
  - **Admin:** Full CRUD operations for all models, can create companies.
  - **Company:** Can buy books on credit (payment deferred).
  - **Customer:** Can buy books and track orders.

---

## 🛠️ Technologies Used

- **Framework:** ASP.NET MVC
- **Database:** SQL Server (using Entity Framework Code First approach)
- **Payment Gateway:** Stripe
- **Design Patterns:** Repository and Unit of Work
- **Languages:** C#, HTML, CSS, JavaScript
- **Tools:** Visual Studio

---

## 👥 User Roles

### Admin
- Manage all entities in the system.
- Create, update, delete companies and books.
- Oversee and update all transactions and order statuses.

### Company
- Browse and buy books with a deferred payment option.
- Track order statuses and manage purchased books.

### Customer
- Browse and buy books.
- Complete transactions using Stripe.
- Track order statuses and manage purchased books.

---

## 🔒 Authentication

- **Register:** Create a new account for Admin, Company, or Customer roles.
- **Login:** Secure login for all user roles.
- **Authorization:** Role-based access control to ensure proper permissions.

---

## 💳 Payments

- **Stripe Integration:** Secure and reliable payment processing.
- **Order Tracking:** Monitor the status of each order from placement to delivery.
- **Deferred Payment for Companies:** Companies can purchase books on credit, with payment due at a later date.

---

## 🛠️ Setup and Installation

### Prerequisites

- Visual Studio 2019 or later
- SQL Server 2019 or later
- .NET Core SDK
- Stripe Account for payment integration

## 🤝 Contributing
Contributions are welcome! Please fork the repository and create a pull request with your changes.

