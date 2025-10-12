import { inject, Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})
export class SnackbarService {
  private snackBar = inject(MatSnackBar);

  error (message: string, action: string = 'Close', duration: number = 5000) {
    this.snackBar.open(message, action, {
      duration,
      panelClass: ['snackbar-error']
    });
  }

  success (message: string, action: string = 'Close', duration: number = 5000) {
    this.snackBar.open(message, action, {
      duration,
      panelClass: ['snackbar-success']
    });
  }
}



