import { CommonModule } from '@angular/common';
import { Component, inject, Input } from '@angular/core';
import { DatasetDetailsResponse } from '../../interfaces/dataset-details-response';
import { ROUTES } from '../../../../shared/config/constants';
import { Router, RouterLink } from '@angular/router';
import { Subscription } from 'rxjs';
import { CartFacadeService } from '../../../cart/services/cart-facade.service';
import { TokenService } from '../../../../core/services/token.service';

@Component({
  selector: 'app-dataset-header',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './dataset-header.component.html',
  styleUrls: ['./dataset-header.component.scss'],
})
export class DatasetHeaderComponent {
  readonly ROUTES = ROUTES;

  @Input() dataset!: DatasetDetailsResponse;

  private cartFacade = inject(CartFacadeService);

  isAddingToCart = false;
  addToCartMessage = '';

  isProvider = false;

  private cartSubscription?: Subscription;

  getImageUrl(dataset: DatasetDetailsResponse): string {
    return dataset.thumbnailUrl || 'assets/images/default-dataset.jpg';
  }

  constructor(private tokenService: TokenService, private router: Router) {}

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

  getProviderInitial(dataset: DatasetDetailsResponse): string {
    return dataset.providerName?.charAt(0).toUpperCase() || 'P';
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

    const request = { serviceId: undefined, dataSetId: this.dataset.id };
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
}
