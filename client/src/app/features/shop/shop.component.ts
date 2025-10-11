import { Component, inject, OnInit } from '@angular/core';
import { ShopService } from '../../core/services/shop.service';
import { Product } from '../../shared/model/product';
import { ProductItemComponent } from "./product-item/product-item.component";
import { MatDialog } from '@angular/material/dialog';
import { FiltersDialogComponent } from './filters-dialog/filters-dialog.component';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatMenu, MatMenuTrigger } from '@angular/material/menu';
import { MatListOption, MatSelectionList, MatSelectionListChange } from '@angular/material/list';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { FormsModule } from '@angular/forms';
import { ShopParams } from '../../shared/model/shopParams';
import { Pagination } from '../../shared/model/pagination';

@Component({
  selector: 'app-shop',
  imports: [ ProductItemComponent,
    MatButton,
    MatIcon,
    MatMenu,
    MatSelectionList,
    MatListOption,
    MatMenuTrigger,
    MatPaginator,
    FormsModule],
  templateUrl: './shop.component.html',
  styleUrl: './shop.component.scss'
})
export class ShopComponent implements OnInit {
  private shopService = inject(ShopService);
  private dialog = inject(MatDialog);
  products?: Pagination<Product>;
  shopParams = new ShopParams();
  sortOptions = [
    { name: 'Alphabetical', value: 'name' },
    { name: 'Price: Low-High', value: 'priceAsc' },
    { name: 'Price: High-Low', value: 'priceDesc' },
  ];
  pageSizeOptions = [5, 10, 15, 20];

  ngOnInit(): void {
    this.initService();
  }

  handlePageEvent(event: PageEvent) {
    this.shopParams.pageNumber = event.pageIndex + 1;
    this.shopParams.pageSize = event.pageSize;
    this.getProducts();
  }

  initService(){
    this.shopService.getBrands();
    this.shopService.getTypes();
    this.getProducts();
  }
  onSearchChange(){
    this.shopParams.pageNumber = 1;
    this.getProducts();
  }
  onSortChange(event: MatSelectionListChange) {
    const selectionOption = event.options[0];
    if (selectionOption){
      this.shopParams.sort = selectionOption.value;
      this.shopParams.pageNumber = 1;
      this.getProducts();
    }
    
  }
  getProducts() {
    this.shopService.getProducts(this.shopParams)
      .subscribe({
        next: response => this.products = response,
        error: err => console.log('Failed to fetch products:', err)
      });
  }

  openFiltersDialog() {
    const dialogRef = this.dialog.open(FiltersDialogComponent, {
      minWidth: '500px',
      data: {
        selectedBrands: this.shopParams.brands,
        selectedTypes: this.shopParams.types
      }
    });

    dialogRef.afterClosed().subscribe({
      next: result => {
        if (result) {
          this.shopParams.pageNumber = 1;
          this.shopParams.brands = result.selectedBrands;
          this.shopParams.types = result.selectedTypes;
          this.getProducts();
        }
      }
    });
  }
}
