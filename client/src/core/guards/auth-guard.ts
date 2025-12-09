import { CanActivateFn } from '@angular/router';
import { AccountService } from '../services/account-service';
import { inject } from '@angular/core';
import { ToastService } from '../services/toast-service';


export const authGuard: CanActivateFn = () => {
  const accountService = inject(AccountService);
  const toast = inject(ToastService);

  // 👇 Ελέγχουμε αν υπάρχει τρέχων user (logged-in)
  if (accountService.currentUser()) {
    //  Ο χρήστης είναι logged-in → επιτρέπουμε την πλοήγηση
    return true;
  } else {
    //  Ο χρήστης ΔΕΝ είναι logged-in
    toast.error('You shall not pass!');
    // Απορρίπτουμε την πλοήγηση
    return false;
  }
};