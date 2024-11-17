# medsoft
This application is a **Medical Appointment Platform** built using **ASP.NET** and **MSSQL Server**. It allows users to schedule, view, and manage appointments with different access rights for **patients**, **doctors**, and **administrators**. The platform supports role-based access control, ensuring that each type of user (patient, doctor, or administrator) can access the features and data appropriate for their role.

### Features
- **Patient Access**: Patients can view available time slots, book appointments, and view their appointment history.
- **Doctor Access**: Doctors can manage their appointments, view patient information, and update the status of appointments.
- **Administrator Access**: Administrators can manage users, schedule appointments for doctors, and have full control over the application.

## Technologies Used
- **ASP.NET Core MVC** for web application structure
- **Microsoft SQL Server** for database management
- **Entity Framework Core** for ORM and data handling
- **Role-based authentication** for access control

## User Roles
### Patients can:

- View available time slots.
- Book appointments.
- View their past appointments.

### Doctors can:

- Manage appointments.
- View patient details.
- Update the status of appointments.

### Administrators can:

- Add, edit, and delete users.
- Assign roles to users.
- View and manage all appointments.

## Preview
![Medsoft Preview](utils/images/preview1.png)


License
This project is licensed under the MIT License - see the LICENSE file for details.
