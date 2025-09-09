# User Scenarios for Reservation System

## Project Overview
This reservation system allows users to book various items (like playgrounds, PlayStation areas, meeting rooms, etc.) 
with time slots. The system supports multiple user roles with different permissions and includes features 
for managing reservations, items, and user access.

## System Entities
- **Users**: Can register, login, and make reservations
- **Items**: Bookable resources (e.g., playgrounds, game consoles, meeting rooms)
- **ItemTypes**: Categories of items (e.g., "Playground Services", "Gaming", "Meeting Rooms")
- **Reservations**: Bookings with date, time slots, and status tracking
- **Roles & Permissions**: Role-based access control (SuperAdmin, Admin, User)

## User Roles

### 1. SuperAdmin
- Has full access to all system features
- Can manage users, roles, permissions, items, and reservations
- Can override any restrictions

### 2. Admin
- Limited administrative access
- Can manage items and reservations
- Cannot modify user roles or system permissions

### 3. Regular User
- Can view available items
- Can create, view, and manage their own reservations
- Limited access to system administration

---

## User Scenarios

### **Scenario 1: User Registration and Authentication**

#### 1.1 New User Registration
**Actor**: Guest User  
**Goal**: Register for a new account in the system

**Steps**:
1. User visits the registration page
2. User provides:
   - Full Name
   - Email address
   - Password
   - Role selection (defaults to "User")
3. System validates email uniqueness
4. System creates account with encrypted password
5. System assigns default "User" role
6. User receives confirmation of successful registration

**Success Criteria**: User can login with new credentials
**Error Handling**: System shows error if email already exists

#### 1.2 User Login
**Actor**: Registered User  
**Goal**: Authenticate and access the system

**Steps**:
1. User enters email and password
2. System validates credentials
3. System generates JWT access and refresh tokens
4. System returns user profile with role information
5. User gains access to role-appropriate features

**Success Criteria**: User is authenticated and redirected to dashboard
**Error Handling**: Invalid credentials show appropriate error message

---

### **Scenario 2: Item Management (Admin/SuperAdmin)**

#### 2.1 Create New Item Type
**Actor**: Admin/SuperAdmin  
**Goal**: Create a new category of bookable items

**Steps**:
1. Admin navigates to Item Types management
2. Admin clicks "Create New Item Type"
3. Admin enters type name (e.g., "Gaming Stations", "Meeting Rooms")
4. System validates permissions
5. System creates new item type
6. Success message confirms creation

**Success Criteria**: New item type is available for item creation
**Error Handling**: Insufficient permissions show unauthorized message

#### 2.2 Create New Bookable Item
**Actor**: Admin/SuperAdmin  
**Goal**: Add a new item that users can reserve

**Steps**:
1. Admin navigates to Items management
2. Admin clicks "Create New Item"
3. Admin fills out item details:
   - Name (e.g., "PlayStation 5 - Station 1")
   - Description
   - Price per hour
   - Item Type selection
   - Availability status
4. System validates permissions
5. System creates new item
6. Item becomes available for reservations

**Success Criteria**: Item appears in booking system for users
**Error Handling**: Missing required fields show validation errors

---

### **Scenario 3: Making Reservations (Regular Users)**

#### 3.1 Browse Available Items
**Actor**: Regular User  
**Goal**: Find items available for reservation

**Steps**:
1. User logs into the system
2. User browses items by category/type
3. System displays available items with:
   - Name and description
   - Price per hour
   - Availability status
4. User selects desired item for booking

**Success Criteria**: User can view all available items
**Error Handling**: No items show appropriate message

#### 3.2 Create New Reservation
**Actor**: Regular User  
**Goal**: Book an item for specific date and time

**Steps**:
1. User selects an item to book
2. User specifies reservation details:
   - Reservation date
   - Start time (HH:mm format)
   - End time (HH:mm format)
   - Confirms item and item type
3. System validates availability for requested time slot
4. System calculates total price (duration × hourly rate)
5. System checks for conflicting reservations
6. System creates reservation with "Pending" status
7. User receives confirmation

**Success Criteria**: Reservation is created and visible in user's bookings
**Error Handling**: 
- Time conflict shows "not available" message
- Invalid time range shows validation error

#### 3.3 View Personal Reservations
**Actor**: Regular User  
**Goal**: See all personal reservations and their status

**Steps**:
1. User navigates to "My Reservations"
2. System displays user's reservations with:
   - Reservation date and time
   - Item name
   - Status (Pending/Confirmed/Cancelled)
   - Total price
3. User can see reservation history

**Success Criteria**: All user reservations are displayed with current status
**Error Handling**: No reservations show appropriate empty state message

---

### **Scenario 4: Reservation Management (Admin)**

#### 4.1 View All Reservations
**Actor**: Admin/SuperAdmin  
**Goal**: Monitor all system reservations

**Steps**:
1. Admin navigates to reservation management
2. System displays all reservations with:
   - User name
   - Item name
   - Date and time
   - Status
   - Total price
3. Admin can filter and search reservations

**Success Criteria**: Admin can see comprehensive reservation overview
**Error Handling**: No reservations show empty state

#### 4.2 Confirm Pending Reservation
**Actor**: Admin/SuperAdmin  
**Goal**: Approve a pending reservation

**Steps**:
1. Admin finds pending reservation
2. Admin clicks "Confirm Reservation"
3. System validates current status is "Pending"
4. System updates status to "Confirmed"
5. System saves changes
6. Success message confirms action

**Success Criteria**: Reservation status changes to "Confirmed"
**Error Handling**: Cannot confirm non-pending reservations

#### 4.3 Cancel Reservation
**Actor**: Admin/SuperAdmin or Reservation Owner  
**Goal**: Cancel an existing reservation

**Steps**:
1. Actor finds the reservation to cancel
2. Actor clicks "Cancel Reservation"
3. System validates reservation is not already cancelled
4. System updates status to "Cancelled"
5. System makes time slot available again
6. Success message confirms cancellation

**Success Criteria**: Reservation is cancelled and time slot freed
**Error Handling**: Already cancelled reservations show warning

---

### **Scenario 5: Advanced Reservation Features**

#### 5.1 Filter Reservations by Date Range
**Actor**: Admin/User  
**Goal**: Find reservations within specific date range

**Steps**:
1. User navigates to reservation filtering
2. User specifies:
   - From date
   - To date
   - Specific item (optional)
3. System searches reservations in date range
4. System displays filtered results
5. User can analyze booking patterns

**Success Criteria**: Only reservations in specified range are shown
**Error Handling**: No results show "no reservations found" message

#### 5.2 Check Item Availability
**Actor**: Regular User  
**Goal**: Verify if specific time slot is available before booking

**Steps**:
1. User selects item and desired time
2. System checks for conflicting reservations:
   - Same date
   - Overlapping time slots
   - Same item
   - Active reservations (not cancelled)
3. System reports availability status
4. User proceeds with booking if available

**Success Criteria**: Accurate availability information prevents conflicts
**Error Handling**: Conflicts are clearly identified with suggestions

---

### **Scenario 6: Permission-Based Access Control**

#### 6.1 Role-Based Feature Access
**Actor**: Any User  
**Goal**: Access features appropriate to user role

**Steps**:
1. User attempts to access system feature
2. System identifies user's role and permissions
3. System checks if user has required permission:
   - Show (view data)
   - Add (create new records)
   - Edit (modify existing records)  
   - Delete (remove records)
4. System either grants access or shows unauthorized message

**Success Criteria**: Users only access features they're authorized for
**Error Handling**: Unauthorized access shows clear permission denied message

#### 6.2 SuperAdmin Override
**Actor**: SuperAdmin  
**Goal**: Access all system features regardless of specific permissions

**Steps**:
1. SuperAdmin logs into system
2. SuperAdmin attempts any system operation
3. System recognizes SuperAdmin role
4. System bypasses permission checks
5. SuperAdmin gains full system access

**Success Criteria**: SuperAdmin has unrestricted access to all features
**Error Handling**: N/A - SuperAdmin has complete access

---

### **Scenario 7: Error Handling and Edge Cases**

#### 7.1 Double Booking Prevention
**Actor**: Regular User  
**Goal**: Prevent conflicting reservations

**Steps**:
1. User A starts booking process for Item X, 2:00-4:00 PM
2. User B simultaneously tries to book same item, 3:00-5:00 PM
3. System detects time overlap during availability check
4. System prevents second booking
5. Second user receives "time slot not available" message
6. First successful booking proceeds

**Success Criteria**: No double bookings occur
**Error Handling**: Clear messaging about time conflicts

#### 7.2 System Maintenance Mode
**Actor**: Any User  
**Goal**: Handle system during maintenance periods

**Steps**:
1. Admin puts system in maintenance mode
2. New users cannot create reservations
3. Existing reservations remain intact
4. Users receive maintenance notification
5. System returns to normal operation after maintenance

**Success Criteria**: System integrity maintained during downtime
**Error Handling**: Clear maintenance messaging to users

---

## Technical Considerations

### Authentication & Security
- JWT token-based authentication
- Password hashing with BCrypt
- Role-based authorization
- Permission checking on all API endpoints

### Data Validation
- Email format validation
- Date/time format validation
- Business rule enforcement (end time after start time)
- Availability conflict prevention

### Internationalization
- Arabic and English language support
- Localized error messages
- Cultural date/time formatting

### Performance
- Database query optimization
- Efficient availability checking algorithms
- Proper indexing on frequently searched fields

### Scalability
- RESTful API design
- Stateless authentication
- Repository pattern for data access
- Unit of Work pattern for transaction management

---

## Success Metrics
- User registration and login success rates
- Reservation completion rates
- System availability and performance
- User satisfaction with booking process
- Admin efficiency in managing reservations
- Reduction in booking conflicts and errors