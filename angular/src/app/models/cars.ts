export interface Car {
  id: string;

  color: string;
  vinNumber: string;
  ownerContact?: string;

  description?: string;

  ownerName?: string;
  ownerEmail?: string;

  buildYear: string;
  carModelId: string;

  // ✅ NEW: waiting list flag (default false if missing)
  isInWaitingList?: boolean;

  createdAt: string;
  updatedAt?: string;
}
