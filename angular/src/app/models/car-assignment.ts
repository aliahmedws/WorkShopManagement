import { Bay, Priority } from './car-assignment.enums';

export interface CarAssignment {
  id: string;
  vinNumber: string;

  bay: Bay;
  priority: Priority;

  dueDate?: string; // ISO date (yyyy-MM-dd)
  createdAt: string;
  updatedAt?: string;
}
