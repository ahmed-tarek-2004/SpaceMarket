export interface SatelliteCard {
  pageNumber: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
  items: SatelliteCardData[];
}

export interface SatelliteCardData {
  id: string;
  noradId: string;
  name: string;
  tleLine1: string;
  tleLine2: string;
}
