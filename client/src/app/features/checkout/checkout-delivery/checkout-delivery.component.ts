import { Component, inject, OnInit, output } from '@angular/core';
import { CheckoutService } from '../../../core/services/checkout.service';
import { MatRadioModule } from "@angular/material/radio";
import { CurrencyPipe } from '@angular/common';
import { CartService } from '../../../core/services/cart.service';
import { DeliveryMethod } from '../../../shared/model/deliveryMethod';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-checkout-delivery',
  imports: [MatRadioModule,
    CurrencyPipe],
  templateUrl: './checkout-delivery.component.html',
  styleUrl: './checkout-delivery.component.scss'
})
export class CheckoutDeliveryComponent implements OnInit{
  checkoutService = inject(CheckoutService);
  cartService = inject(CartService);
  deliveryComplete = output<boolean>();

  ngOnInit() {
    this.checkoutService.getDeliveryMethods().subscribe({
      next: methods => {
        if (this.cartService.cart()?.deliveryMethodId){
          const method = methods.find(d => d.id === this.cartService.cart()?.deliveryMethodId);
          if (method){
            this.cartService.selectedDelivery.set(method);
            this.deliveryComplete.emit(true);
          }
        }
      }
    }
    );
  }

  async updateDeliveryMethod(deliveryMethod: DeliveryMethod){
    this.cartService.selectedDelivery.set(deliveryMethod);
    const cart = this.cartService.cart();

    if (cart){
      cart.deliveryMethodId = deliveryMethod.id;
      await firstValueFrom(this.cartService.setCart(cart));
      this.deliveryComplete.emit(true);
    }
  }


}
