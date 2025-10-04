import { Component, Input, AfterViewInit, OnDestroy, ViewChild, ElementRef, OnChanges, SimpleChanges, ChangeDetectorRef } from '@angular/core';
import * as Cesium from 'cesium';
import { CollisionAlertResponse } from '../../interfaces/collision-alert';

@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.scss'],
  standalone: true
})
export class MapComponent implements AfterViewInit, OnDestroy, OnChanges {
  @Input() alerts: CollisionAlertResponse[] = [];
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
    }, 0);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.isViewerReady && changes['alerts']) {
      this.drawAlerts();
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
      shouldAnimate: false
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

    this.alerts.forEach(alert => {
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
          pixelSize: 8,
          color: Cesium.Color.BLUE.withAlpha(0.8),
          outlineColor: Cesium.Color.WHITE,
          outlineWidth: 2
        }
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
          pixelSize: 8,
          color: Cesium.Color.RED.withAlpha(0.8),
          outlineColor: Cesium.Color.WHITE,
          outlineWidth: 2
        }
      });

      dataSource.entities.add({
        polyline: {
          positions: [satPos, debPos],
          width: 2,
          material: Cesium.Color.YELLOW.withAlpha(0.6),
          clampToGround: false,
        }
      });
    });

    if (allPositions.length > 0) {
      const boundingSphere = Cesium.BoundingSphere.fromPoints(allPositions);
      this.viewer.camera.flyToBoundingSphere(boundingSphere, { duration: 1.5 });
    }

    this.addHoverTooltip(dataSource);
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
}