import { Injectable, OnDestroy } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class SpaceThemeService implements OnDestroy {
  private starFieldElement: HTMLElement | null = null;

  initializeStarField(containerId: string = 'star-field'): void {
    this.starFieldElement = document.getElementById(containerId);

    if (this.starFieldElement) {
      this.createStarField();
      this.createShootingStars();
    }
  }

  private createStarField(): void {
    if (!this.starFieldElement) return;

    const starCount = 150;

    for (let i = 0; i < starCount; i++) {
      const star = document.createElement('div');
      const size = Math.random();

      // Set star size
      if (size < 0.7) {
        star.classList.add('star', 'small');
      } else if (size < 0.9) {
        star.classList.add('star', 'medium');
      } else {
        star.classList.add('star', 'large');
      }

      // Random twinkle speed
      const twinkleSpeed = Math.random();
      if (twinkleSpeed < 0.3) {
        star.classList.add('star-twinkle-slow');
      } else if (twinkleSpeed < 0.7) {
        star.classList.add('star-twinkle');
      } else {
        star.classList.add('star-twinkle-fast');
      }

      star.style.left = `${Math.random() * 100}%`;
      star.style.top = `${Math.random() * 100}%`;

      this.starFieldElement.appendChild(star);
    }
  }

  private createShootingStars(): void {
    if (!this.starFieldElement) return;

    const shootingStarCount = 3;

    for (let i = 0; i < shootingStarCount; i++) {
      const shootingStar = document.createElement('div');
      shootingStar.classList.add('shooting-star');

      // Random starting position
      const startX = Math.random() * 20;
      const startY = Math.random() * 20;

      shootingStar.style.left = `${startX}%`;
      shootingStar.style.top = `${startY}%`;
      shootingStar.style.width = `${Math.random() * 50 + 30}px`;
      shootingStar.style.animationDelay = `${Math.random() * 20}s`;

      this.starFieldElement.appendChild(shootingStar);
    }
  }

  ngOnDestroy(): void {
    // Clean up if needed
    this.starFieldElement = null;
  }
}
