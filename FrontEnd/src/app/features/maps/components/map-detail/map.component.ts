import {
  Component,
  Input,
  AfterViewInit,
  OnDestroy,
  ViewChild,
  ElementRef,
  OnChanges,
  SimpleChanges,
  ChangeDetectorRef,
} from '@angular/core';
import * as Cesium from 'cesium';
import { CollisionAlertResponse, PositionDto } from '../../interfaces/collision-alert';

@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.scss'],
  standalone: true,
})
export class MapComponent implements AfterViewInit, OnDestroy, OnChanges {
  @Input() alerts: CollisionAlertResponse[] = [];
  @Input() satellitePosition: PositionDto | null = null;
  @Input() satelliteId: string | null = null;
  @Input() satelliteName: string | null = null;
  @ViewChild('cesiumContainer', { static: true }) cesiumContainer!: ElementRef;

  tooltipVisible = false;
  tooltipX = 0;
  tooltipY = 0;
  tooltipContent = '';

  private viewer!: Cesium.Viewer;
  private isViewerReady = false;

  constructor() {}

  ngAfterViewInit(): void {
    this.initCesiumViewer();
    this.isViewerReady = true;

    setTimeout(() => {
      this.drawAlerts();
      // Also draw satellite position if available
      if (this.satellitePosition) {
        console.log('Drawing satellite position in ngAfterViewInit');
        this.drawSatellitePosition();
      }
    }, 1000); // Increased timeout to ensure viewer is fully ready
  }

  ngOnChanges(changes: SimpleChanges): void {
    console.log('Map component changes detected:', changes);
    console.log('Current satellite position:', this.satellitePosition);
    console.log('Current satellite ID:', this.satelliteId);
    console.log('Current satellite name:', this.satelliteName);

    if (this.isViewerReady) {
      if (changes['alerts']) {
        console.log('Alerts changed, redrawing alerts');
        this.drawAlerts();
      }
      if (changes['satellitePosition'] && this.satellitePosition) {
        console.log('Satellite position changed, drawing satellite position');
        // Add a small delay to ensure the viewer is fully ready
        setTimeout(() => {
          this.drawSatellitePosition();
        }, 100);
      }
    } else {
      console.log('Viewer not ready yet, changes will be handled when viewer is ready');
      // If viewer is not ready but we have satellite position, set a timeout to try again
      if (this.satellitePosition && changes['satellitePosition']) {
        setTimeout(() => {
          if (this.isViewerReady) {
            console.log('Viewer is now ready, drawing satellite position');
            this.drawSatellitePosition();
          }
        }, 2000);
      }
    }
  }

  private initCesiumViewer(): void {
    this.viewer = new Cesium.Viewer(this.cesiumContainer.nativeElement, {
      sceneMode: Cesium.SceneMode.SCENE2D,
      baseLayerPicker: false,
      fullscreenButton: false,
      vrButton: false,
      geocoder: false,
      homeButton: false,
      infoBox: false,
      sceneModePicker: false,
      selectionIndicator: false,
      timeline: false,
      navigationHelpButton: false,
      animation: false,
      shouldAnimate: false,
    });

    const controller = this.viewer.scene.screenSpaceCameraController;
    controller.enableRotate = false;
    controller.enableTilt = false;
    controller.enableLook = false;
  }

  private drawAlerts(): void {
    if (!this.isViewerReady) return;

    this.viewer.dataSources.removeAll();

    const dataSource = new Cesium.CustomDataSource('alerts');
    this.viewer.dataSources.add(dataSource);

    const allPositions: Cesium.Cartesian3[] = [];

    this.alerts.forEach((alert) => {
      const satPos = Cesium.Cartesian3.fromDegrees(
        alert.satellitePosition.longitude,
        alert.satellitePosition.latitude,
        alert.satellitePosition.altitudeKm * 1000
      );
      allPositions.push(satPos);

      dataSource.entities.add({
        id: `sat-${alert.satelliteId}`,
        name: alert.satelliteName,
        position: satPos,
        point: {
          pixelSize: 10,
          color: Cesium.Color.BLUE.withAlpha(0.9),
          outlineColor: Cesium.Color.WHITE,
          outlineWidth: 3,
          heightReference: Cesium.HeightReference.RELATIVE_TO_GROUND,
        },
      });

      const debPos = Cesium.Cartesian3.fromDegrees(
        alert.debrisPosition.longitude,
        alert.debrisPosition.latitude,
        alert.debrisPosition.altitudeKm * 1000
      );
      allPositions.push(debPos);

      dataSource.entities.add({
        id: `deb-${alert.debrisId}`,
        name: alert.debrisName,
        position: debPos,
        point: {
          pixelSize: 10,
          color: Cesium.Color.RED.withAlpha(0.9),
          outlineColor: Cesium.Color.WHITE,
          outlineWidth: 3,
          heightReference: Cesium.HeightReference.RELATIVE_TO_GROUND,
        },
      });

      dataSource.entities.add({
        polyline: {
          positions: [satPos, debPos],
          width: 3,
          material: Cesium.Color.YELLOW.withAlpha(0.8),
          clampToGround: false,
        },
      });
    });

    if (allPositions.length > 0) {
      const boundingSphere = Cesium.BoundingSphere.fromPoints(allPositions);
      this.viewer.camera.flyToBoundingSphere(boundingSphere, { duration: 1.5 });
    }

    this.addHoverTooltip(dataSource);
  }

  private drawSatellitePosition(): void {
    console.log('drawSatellitePosition called');
    console.log('isViewerReady:', this.isViewerReady);
    console.log('satellitePosition:', this.satellitePosition);
    console.log('satelliteId:', this.satelliteId);
    console.log('satelliteName:', this.satelliteName);

    if (!this.isViewerReady) {
      console.log('Cannot draw satellite position - viewer not ready');
      return;
    }

    if (!this.satellitePosition) {
      console.log('Cannot draw satellite position - no position data');
      return;
    }

    // Validate position data
    if (
      typeof this.satellitePosition.longitude !== 'number' ||
      typeof this.satellitePosition.latitude !== 'number' ||
      typeof this.satellitePosition.altitudeKm !== 'number'
    ) {
      console.error('Invalid satellite position data:', this.satellitePosition);
      return;
    }

    console.log('Drawing satellite position:', this.satellitePosition);

    try {
      // Remove any existing satellite position data source
      const existingDataSource = this.viewer.dataSources.getByName('satellite-position');
      if (existingDataSource.length > 0) {
        console.log('Removing existing satellite position data source');
        this.viewer.dataSources.remove(existingDataSource[0]);
      }

      // Create a new data source for the satellite position
      const satelliteDataSource = new Cesium.CustomDataSource('satellite-position');
      this.viewer.dataSources.add(satelliteDataSource);

      // Convert altitude from km to meters
      const altitudeInMeters = this.satellitePosition.altitudeKm * 1000;
      console.log('Satellite position coordinates:', {
        longitude: this.satellitePosition.longitude,
        latitude: this.satellitePosition.latitude,
        altitudeKm: this.satellitePosition.altitudeKm,
        altitudeMeters: altitudeInMeters,
      });

      // Add the satellite position as a colored point using PositionDto structure
      const entity = satelliteDataSource.entities.add({
        id: `satellite-${this.satelliteId}`,
        name: this.satelliteName || 'Satellite',
        position: Cesium.Cartesian3.fromDegrees(
          this.satellitePosition.longitude,
          this.satellitePosition.latitude,
          altitudeInMeters
        ),
        point: {
          pixelSize: 16,
          color: Cesium.Color.CYAN.withAlpha(0.9),
          outlineColor: Cesium.Color.YELLOW,
          outlineWidth: 4,
          heightReference: Cesium.HeightReference.RELATIVE_TO_GROUND,
          scaleByDistance: new Cesium.NearFarScalar(1.5e2, 2.0, 1.5e7, 0.5),
        },
        label: {
          text: this.satelliteName || 'Satellite',
          font: '14pt sans-serif',
          fillColor: Cesium.Color.CYAN,
          outlineColor: Cesium.Color.BLACK,
          outlineWidth: 3,
          style: Cesium.LabelStyle.FILL_AND_OUTLINE,
          pixelOffset: new Cesium.Cartesian2(0, -50),
          scaleByDistance: new Cesium.NearFarScalar(1.5e2, 1.0, 1.5e7, 0.5),
        },
      });

      console.log('Satellite entity added successfully:', entity);

      // Fly to the satellite position
      const position = Cesium.Cartesian3.fromDegrees(
        this.satellitePosition.longitude,
        this.satellitePosition.latitude,
        altitudeInMeters
      );

      console.log('Flying to satellite position');
      this.viewer.camera.flyTo({
        destination: position,
        duration: 2.0,
      });

      console.log('Satellite position drawing completed successfully');
    } catch (error) {
      console.error('Error drawing satellite position:', error);
    }
  }

  private addHoverTooltip(dataSource: Cesium.CustomDataSource): void {
    const handler = new Cesium.ScreenSpaceEventHandler(this.viewer.scene.canvas);
    let lastHovered: Cesium.Entity | null = null;

    handler.setInputAction((movement: { endPosition: { x: number; y: number } }) => {
      const picked = this.viewer.scene.pick(
        new Cesium.Cartesian2(movement.endPosition.x, movement.endPosition.y)
      );

      if (Cesium.defined(picked) && picked.id) {
        const entity = picked.id;

        if (lastHovered && lastHovered !== entity && lastHovered.point) {
          const lastPoint = lastHovered.point as any;
          if (lastPoint.originalSize !== undefined) {
            lastPoint.pixelSize = lastPoint.originalSize;
            lastPoint.color = lastPoint.originalColor;
          }
        }

        const point = entity.point as any;
        if (!point.originalSize) point.originalSize = point.pixelSize;
        if (!point.originalColor) point.originalColor = point.color;

        point.pixelSize = point.originalSize * 1.5;
        point.color = Cesium.Color.YELLOW.withAlpha(0.9);

        lastHovered = entity;

        this.tooltipContent = entity.name;
        this.tooltipX = movement.endPosition.x + 10;
        this.tooltipY = movement.endPosition.y + 10;
        this.tooltipVisible = true;
      } else {
        if (lastHovered && lastHovered.point) {
          const lastPoint = lastHovered.point as any;
          if (lastPoint.originalSize !== undefined) {
            lastPoint.pixelSize = lastPoint.originalSize;
            lastPoint.color = lastPoint.originalColor;
          }
        }
        lastHovered = null;
        this.tooltipVisible = false;
      }
    }, Cesium.ScreenSpaceEventType.MOUSE_MOVE);
  }

  ngOnDestroy(): void {
    if (this.viewer && !this.viewer.isDestroyed) {
      this.viewer.destroy();
    }
  }

  // Public method to manually trigger satellite position drawing
  public drawSatellitePositionManually(): void {
    console.log('Manually triggering satellite position drawing');
    if (this.satellitePosition) {
      this.drawSatellitePosition();
    } else {
      console.log('No satellite position data available for manual drawing');
    }
  }

  // Method to force redraw with delay
  public forceRedrawSatellitePosition(): void {
    console.log('Force redrawing satellite position with delay');
    setTimeout(() => {
      if (this.satellitePosition && this.isViewerReady) {
        console.log('Force redraw - drawing satellite position');
        this.drawSatellitePosition();
      } else {
        console.log('Force redraw - conditions not met', {
          hasPosition: !!this.satellitePosition,
          viewerReady: this.isViewerReady,
        });
      }
    }, 2000);
  }

  // Method to check if satellite position is currently displayed
  public isSatellitePositionDisplayed(): boolean {
    if (!this.isViewerReady) return false;

    const satelliteDataSource = this.viewer.dataSources.getByName('satellite-position');
    return satelliteDataSource.length > 0 && satelliteDataSource[0].entities.values.length > 0;
  }

  // Method to get debug information about the current state
  public getDebugInfo(): any {
    return {
      isViewerReady: this.isViewerReady,
      hasSatellitePosition: !!this.satellitePosition,
      satellitePosition: this.satellitePosition,
      satelliteId: this.satelliteId,
      satelliteName: this.satelliteName,
      isPositionDisplayed: this.isSatellitePositionDisplayed(),
      dataSourceCount: this.isViewerReady ? this.viewer.dataSources.length : 0,
    };
  }
}
