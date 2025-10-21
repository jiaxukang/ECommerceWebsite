import { Component, inject, OnInit } from '@angular/core';
import { OrderService } from '../../core/services/order.service';
import { Order } from '../../shared/model/order';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-order',
  imports: [
    CurrencyPipe,
    DatePipe,
    RouterLink
  ],
  templateUrl: './order.component.html',
  styleUrl: './order.component.scss'
})
export class OrderComponent implements OnInit{
  private orderService = inject(OrderService);
  orders: Order[] = [];

  ngOnInit(): void {
      this.orderService.getOrdersForUser().subscribe({
        next: orders => this.orders = orders
      })
  }
}
