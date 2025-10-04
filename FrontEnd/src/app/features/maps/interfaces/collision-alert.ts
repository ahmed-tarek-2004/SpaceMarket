// src/app/interfaces/collision-alert.ts

export interface PositionDto {
  latitude: number;
  longitude: number;
  altitudeKm: number;
}

export interface CollisionAlertResponse {
  satelliteId: string;        
  satelliteName: string;
  satellitePosition: PositionDto;

  debrisId: string;
  debrisName: string;
  debrisPosition: PositionDto;

  closestDistanceKm: number;
  timestamp: string;         
  status: string;
}