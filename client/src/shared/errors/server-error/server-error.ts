import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { ApiError } from '../../../types/error';

@Component({
  selector: 'app-server-error',
  imports: [],
  templateUrl: './server-error.html',
  styleUrl: './server-error.css',
})
export class ServerError {
  protected error: ApiError;
  private router = inject(Router);
  protected showDetails = false;

  // Στον constructor διαβάζουμε το error object που περάστηκε μέσω του state κατά την πλοήγηση
  constructor() {
    // Λαμβάνουμε την τρέχουσα πλοήγηση
    const navigation = this.router.getCurrentNavigation();
    // Αν υπάρχει state με το error, το αναθέτουμε στην ιδιότητα error
    this.error = navigation?.extras?.state?.['error'];
  }

  detailToggle() {
    this.showDetails = !this.showDetails;
  }
}
