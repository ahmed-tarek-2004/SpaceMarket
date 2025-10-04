import { Position } from './position';

export interface DebrisAlertHistoryResponse {
  satelliteId: string;
  satelliteName: string;
  satellitePosition: Position;
  debrisId: string;
  debrisName: string;
  debrisPosition: Position;
  closestDistanceKm: number;
  timestamp: Date;
  status: string;
}
