import { inject, Injectable } from '@angular/core';
import { of, forkJoin, tap } from 'rxjs';
import { CartService } from './cart.service';
import { AccountService } from './account.service';
import { SignalrService } from './signalr.service';

@Injectable({
  providedIn: 'root'
})
export class InitService {
  private cartService = inject(CartService);
  private accountService = inject(AccountService);
  private hubService = inject(SignalrService);


  init() {
    const cartId = localStorage.getItem('cart_id');
    const cart$ = cartId ? this.cartService.getCart(cartId) : of(null);

    return forkJoin({
      cart: cart$,
      user: this.accountService.getUserInfo().pipe(
        tap(user => {
          if(user) this.hubService.createHubConnection(); 
        })
      )
    })
  }
}
