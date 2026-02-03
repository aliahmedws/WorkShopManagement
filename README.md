
# Workshop Management User Guide

## Introduction
Welcome to the **Workshop Management** Web Application! This guide will help you navigate through the application, manage vehicles, monitor workflows, and perform various tasks efficiently.

### Features of the Application:
- **RBAC & Permissions**: Detailed role-based access control (RBAC) and permissions management.
- **Vehicle Workflow Management**: Manage vehicles through different stages of production.
- **Issue Tracking & Logistics**: Track and manage issues, logistics details, and various stages of vehicle processing.
- **External Integrations**: Fetch car details using external APIs like CarXe, NHTSA, and Vpic.
- **File Management**: Upload and manage documents related to vehicles and stages.

---

## Getting Started

### Logging In

1. **Navigate to the Login Page**: Enter your registered email and password.
2. **Multi-Factor Authentication (MFA)**: If enabled, enter the verification code sent via SMS to your mobile number using Twilio.

### User Roles and Permissions
The application supports a robust role and permission system. Admin users can create roles, assign them to users, and define permissions for different actions.

---

## Application Modules & Features

### 1. **Vehicle Models Management**
- **View & Add Car Models**: Admins can seed vehicle models and variants into the system.
- **Add/Modify Checklists and Items**: Checklists and checklist items can be added or updated for each car model.
- **Assign Checklist Options**: Radio button options for checklist items can be configured.

### 2. **Vehicle Stock Management**
- **Add Vehicle to Stock**: Enter the **VIN number** and fetch details from external APIs like CarXe and NHTSA.
  - If no data is returned, a message will notify you that the VIN was not found.
  - If available, data like **Exterior Color** and **Model Year** will auto-populate in the form.
  - **Manual Data Entry**: Add color, owner, and related notes.
- **Attach Documents**: Users can attach documents related to the car during registration.

### 3. **Vehicle Workflow & Stage Transitions**
The application supports the following stages in the vehicle workflow:
- **Incoming**
- **Dealer Warehouse**
- **SCD Warehouse**
- **Production**
- **Post Production**
- **Awaiting Transport**
- **Dispatched**

Each stage has specific actions and options available.

---

### 4. **Stage Management**

#### **Incoming Stage**:
- **Check-In Report**: Fetches and populates data based on VIN. If available, external details from the **AusEv website** can be viewed.
- **Move Car**: Cars can be moved to the next stage (Dealer Warehouse or SCD Warehouse).
- **Action Options**: Includes viewing **recalls**, managing **logistics**, and marking **issues**.

#### **Dealer Warehouse**:
- **Move Car**: Move the car from Dealer Warehouse to SCD Warehouse.

#### **SCD Warehouse**:
- **Bay Assignment**: Cars can be assigned to an available bay.
- **Move to Production**: Once assigned, cars move to production.

#### **Production**:
- **Car Bay Details**: View car details, checklists, and quality gates.
- **Check-In Report**: View and modify details of the check-in report.
- **Clock In/Out**: Clock in the car for production, automatically setting the manufacture date and release date.
- **Actions**: Move to the next stage, print production stickers, and handle quality gates.

#### **Post Production**:
- **Finalize Production**: Once production is completed, cars are moved to the Post Production stage.
- **AVV Status**: The AVV status can be updated, indicating that the car is ready for transport.

#### **Awaiting Transport**:
- **Prepare for Dispatch**: Cars in this stage await transport once all required actions (e.g., AVV status) are set.

#### **Dispatched**:
- **Final Stage**: Once dispatched, cars are marked as completed, and the workflow ends.

---

### 5. **Logistics Management**

- **Logistics Form**: Add and modify CRE details, arrival estimates, and clearance information.
- **Arrival Estimates**: Users can add multiple arrival estimates and view the history of estimates.
- **Document Attachments**: Attach documents related to logistics and CRE.

---

### 6. **Issue Management**

- **Mark Issues**: Issues can be marked on cars during the production process.
- **Issue Tracking**: Track issue status, rectification actions, and quality control steps.
- **Audit Logs**: View logs for each marked issue.

---

### 7. **External API Integrations**

The application integrates with external APIs to fetch car details and recall information.

- **CarXe API**: Used to fetch car details based on VIN and model year.
- **NHTSA Vpic API**: Fetches data when CarXe API does not provide results.
- **Recalls**: Fetches recall information for vehicles.

---

## Admin Settings

### **User & Role Management**
- **Add/Edit Users**: Admins can create, modify, and assign roles to users.
- **Set Permissions**: Permissions can be set for various roles and actions within the system.
- **Enable/Disable 2FA**: Admins can enable or disable Two-Factor Authentication for users.

---

## System Workflow Documentation

### Overview

The **Workshop Management** application follows a structured workflow for managing vehicles in the production process. The workflow consists of multiple stages and actions that ensure seamless vehicle management and issue tracking.

---

### **Vehicle Workflow Stages**

1. **Incoming Stage**
   - **Add Car to Incoming**: Once a vehicle enters the system, it’s automatically added to the Incoming stage.
   - **Stage Transition**: Cars can be moved to the next stage (Dealer Warehouse or SCD Warehouse) once **CRE details** are added.

2. **Dealer Warehouse**
   - **Transition**: Vehicles can be moved to SCD Warehouse from Dealer Warehouse.

3. **SCD Warehouse**
   - **Bay Assignment**: Cars can be assigned to an available bay.
   - **Move to Production**: Once assigned, cars move to the Production stage.

4. **Production**
   - **Clock In**: The car is clocked into the production stage, which sets its manufacturing and estimated release dates.
   - **Car Bay Actions**: Users can view car details, add or modify checklists, and track quality gates.
   - **Move Between Bays**: Cars can be reassigned to other bays as needed.

5. **Post Production**
   - **Finalize Production**: Once production is completed, cars are moved to the Post Production stage.
   - **AVV Status**: The AVV status can be updated, indicating that the car is ready for transport.

6. **Awaiting Transport**
   - **Prepare for Dispatch**: Cars in this stage await transport once all required actions (e.g., AVV status) are set.

7. **Dispatched**
   - **Final Stage**: Once dispatched, cars are marked as completed, and the workflow ends.

---

### **External API Workflow**

- **VIN Lookup**: When a user adds a new car, the VIN is sent to the **CarXe API** to fetch vehicle data. If no data is returned, a fallback API (**NHTSA Vpic**) is used.
- **Recall Information**: Recall data for the car is fetched from **CarXe** and displayed in the Recall section for each vehicle.
- **Car Images**: Car images are fetched from **CarXe** based on model year and exterior color.

---

### **File Management Workflow**

1. **Temporary File Storage**: When users upload files, they are temporarily stored in **Google Cloud Storage** buckets.
2. **Retention Job**: A worker job runs periodically to delete files beyond a certain retention period.
3. **Persistent File Storage**: Once the form is saved, the uploaded files are permanently stored.

---

## Conclusion
This user guide and system workflow documentation provide a comprehensive overview of the **Workshop Management** application. Whether you're managing vehicles, tracking progress, or overseeing administrative tasks, this documentation will help you effectively use the system. For more detailed instructions or if you encounter any issues, please refer to the in-app help section or contact the support team.
