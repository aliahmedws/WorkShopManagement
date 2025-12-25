export interface Car {
  id: string;

  color: string;
  vinNumber: string;
  ownerContact?: string;

  description?: string;

  ownerName?: string;
  ownerEmail?: string;

  buildYear: string;     // keep as string (e.g. 2022)
  carModelId: string;    // FK to Car Make

  createdAt: string;
  updatedAt?: string;
}
