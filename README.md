# Tickety 
### Football Match Ticket Booking System
Tickety is a web application developed using ASP.NET MVC (.NET 7) that allows users to book tickets for football matches. It provides two types of users: normal users and administrators. Normal users can register, explore matches, purchase tickets, manage their tickets and profile, and update their information. Administrators have additional privileges such as managing tickets, stadiums, matches, and users.

### Features

- **User Registration and Authentication**: Users can register and sign up to access the platform securely.
- **Match Exploration**: Users can explore upcoming matches and view match details.
- **Ticket Booking**: Users can select the type of ticket they want and purchase it.
- **QR Code Generation**: Upon successful booking, users receive a QR code which they can use to enter the stadium.
- **Ticket Management**: Users can manage their booked tickets, view details, and cancel if necessary.
- **Profile Management**: Users can update their profile information and change passwords securely.
- **Admin Dashboard**: Administrators have access to a dashboard where they can manage tickets, stadiums, matches, and users.
- **Ticket Management by Admin**: Administrators can view all tickets purchased by users and perform CRUD operations on them.
- **Stadium and Match Management**: Administrators can manage stadiums and matches, including creating, updating, and deleting them.
- **User Management by Admin**: Administrators can manage other administrators, including creating and assigning roles.

### Installation

1. **Clone the Repository**: Clone the Tickety repository to your local machine.
   ```
   git clone https://github.com/yourusername/tickety.git
   ```

2. **Set up the Database**: Create a new SQL Server database and execute the SQL scripts provided in the `database_scripts` folder to set up the necessary tables and data.

3. **Configure Connection String**: Update the connection string in the `appsettings.json` file with your database credentials.

4. **Build and Run**: Build the solution in Visual Studio and run the application. Ensure that all dependencies are installed correctly.

### Usage

1. **User Registration and Login**: Users can register for an account and log in using their credentials.

2. **Explore Matches**: Users can browse through the list of upcoming matches and view match details.

3. **Book Tickets**: Users can select a match, choose the type of ticket they want, and proceed with the booking process.

4. **Manage Tickets**: Users can view their booked tickets, cancel bookings if necessary, and access their QR codes for stadium entry.

5. **Update Profile**: Users can update their profile information and change passwords from the profile settings.

6. **Admin Dashboard**: Administrators can access the admin dashboard after logging in.

7. **Manage Tickets, Stadiums, Matches, and Users**: Administrators can perform CRUD operations on tickets, stadiums, matches, and users from the admin dashboard.

