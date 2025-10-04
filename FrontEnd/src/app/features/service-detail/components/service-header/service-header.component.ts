import { Component, inject, Input, OnDestroy } from '@angular/core';
import { ServiceDetailsResponse } from '../../interfaces/service.interface';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { CartFacadeService } from '../../../cart/services/cart-facade.service';
import { Subscription } from 'rxjs';
import { ROUTES } from '../../../../shared/config/constants';
import { TokenService } from '../../../../core/services/token.service';

@Component({
  selector: 'app-service-header',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './service-header.component.html',
  styleUrls: ['./service-header.component.scss'],
})
export class ServiceHeaderComponent implements OnDestroy {
  readonly ROUTES = ROUTES;

  @Input() service!: ServiceDetailsResponse;

  private router = inject(Router);
  private cartFacade = inject(CartFacadeService);

  isAddingToCart = false;
  addToCartMessage = '';

  isProvider = false;

  private cartSubscription?: Subscription;

  constructor(private tokenService: TokenService) {}

  ngOnInit(): void {
    this.updateRoleFlag();

    this.tokenService.authState$.subscribe(() => {
      this.updateRoleFlag();
    });
  }

  private updateRoleFlag() {
    const role = this.tokenService.getRole();
    this.isProvider = !!role && role.toLowerCase().trim() === 'serviceprovider';
  }

  getImageUrl(service: ServiceDetailsResponse): string {
    return service.imagesUrl || 'assets/images/default-service.jpg';
  }

  getProviderInitial(service: ServiceDetailsResponse): string {
    return service.providerId?.charAt(0).toUpperCase() || 'P';
  }

  onAddToCart(serviceId: string) {
    if (this.isAddingToCart) return;

    if (this.isProvider) {
      this.addToCartMessage =
        'Providers cannot add services to the cart. Switch to a buyer account to purchase.';
      return;
    }

    this.isAddingToCart = true;
    this.addToCartMessage = '';

    const request = { serviceId, dataSetId: undefined };
    this.cartFacade.addToCart(request);

    this.cartSubscription = this.cartFacade.cartState$.subscribe((cart) => {
      if (!cart) return;

      this.isAddingToCart = false;

      const isAlreadyInCart = cart.items.some(
        (item) => item.itemType.toLowerCase() === 'service' && item.itemId === serviceId
      );

      this.addToCartMessage = isAlreadyInCart
        ? 'Service already in cart!'
        : 'Service added to cart successfully!';

      setTimeout(() => this.router.navigate(['/cart']), 1500);

      this.cartSubscription?.unsubscribe();
    });

    setTimeout(() => {
      if (this.isAddingToCart) {
        this.isAddingToCart = false;
        this.addToCartMessage = 'Added to cart!';
        this.router.navigate(['/cart']);
      }
    }, 3000);
  }

  onContactProvider() {
    console.log('Contact provider');
    // Implement contact provider logic
  }

  ngOnDestroy() {
    this.cartSubscription?.unsubscribe();
  }
}
