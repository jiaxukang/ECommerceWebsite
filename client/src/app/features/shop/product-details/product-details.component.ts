import { Component, inject, OnInit } from '@angular/core';
import { Product } from '../../../shared/model/product';
import { ShopService } from '../../../core/services/shop.service';
import { ActivatedRoute } from '@angular/router';
import { CurrencyPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { MatDivider } from '@angular/material/divider';
import { MatIcon } from '@angular/material/icon';
import { MatInput } from '@angular/material/input';
import { MatFormField, MatLabel } from '@angular/material/form-field'

@Component({
  selector: 'app-product-details',
  imports: [MatIcon,
    MatFormField,
    MatDivider,
    MatLabel,
    CurrencyPipe,
    MatButton,
    MatFormField,
    MatInput,
    FormsModule],
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.scss'
})
export class ProductDetailsComponent implements OnInit {
  product?: Product
  private shopService = inject(ShopService);
  private activatedRoute = inject(ActivatedRoute)

  ngOnInit() {
    this.loadProduct();
  }

  loadProduct() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    if (id) {
      this.shopService.getProduct(+id).subscribe({
        next: product => this.product = product,
        error: error => console.log(error)
      });
    }
  }
}
