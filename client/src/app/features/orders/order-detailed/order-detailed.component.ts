import { Component, inject, OnInit } from '@angular/core';
import { OrderService } from '../../../core/services/order.service';
import { Order } from '../../../shared/model/order';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { DatePipe, CurrencyPipe } from '@angular/common';
import { MatButton } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { AddressPipe } from '../../../shared/pipes/address-pipe';
import { PaymentCardPipe } from '../../../shared/pipes/payment-card-pipe';

@Component({
  selector: 'app-order-detailed',
  imports: [MatCardModule,
    DatePipe,
    MatButton,
    AddressPipe,
    PaymentCardPipe,
    CurrencyPipe],
  templateUrl: './order-detailed.component.html',
  styleUrl: './order-detailed.component.scss'
})
export class OrderDetailedComponent implements OnInit{
  private orderService = inject(OrderService);
  order? : Order;
  private activatedRoute = inject(ActivatedRoute);
  private router = inject(Router);
  buttonText = 'Return to orders'

  ngOnInit(): void {
    this.loadOrder();
  }
  loadOrder(){
    const id = this.activatedRoute.snapshot.paramMap.get("id");
    if (!id) return;

    this.orderService.getOrderDetailed(+id).subscribe({
      next: order => this.order = order
    });
  }
  onReturnClick() {
    this.router.navigateByUrl('/orders')
  }
}
