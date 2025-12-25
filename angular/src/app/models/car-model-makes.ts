export interface CarModelMakeImage {
  id: string;
  fileName: string;
  contentType: string;
  dataUrl: string;
}

export interface CarModelMake {
  id: string;
  name: string;
  createdAt: string;   // ISO
  updatedAt?: string;  // ISO
  images: CarModelMakeImage[];
}
