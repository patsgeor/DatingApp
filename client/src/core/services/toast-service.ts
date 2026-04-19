import { inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class ToastService {
  private router=inject(Router)

  constructor() {
    // Μόλις δημιουργηθεί το service, φτιάχνουμε το toast container μία φορά
    this.createToastContainer();
  }

  // Δημιουργεί το container που θα κρατάει ΟΛΑ τα toast messages.
  // Εκτελείται μόνο μία φορά (αν δεν υπάρχει ήδη).
  private createToastContainer() {
    if (!document.getElementById('toast-container')) {
      const toast = document.createElement('div');
      toast.id = 'toast-container';
      toast.className = 'toast toast-bottom toast-end z-50';
      document.body.append(toast);
    }
  }

  // Δημιουργεί ΕΝΑ toast message.
  private createToastElement(message: string, alertClass: string, duration = 5000, avatar?: string, route?: string) {
    const toastContainer = document.getElementById('toast-container');
    if (!toastContainer) return;
    const toast = document.createElement('div');
    toast.classList.add('alert', alertClass, 'shadow-lg','items-center', 'gap-3', 'cursor-pointer',
      'opacity-100',
      'transition-opacity',
      'duration-500');

    // HTML περιεχόμενο: μήνυμα + κουμπί κλεισίματος
    toast.innerHTML = `
    ${avatar? `<img src=${avatar || '/user.png'} class= 'w-10 h-10 rounded' />` :''}
    <span>${message}</span>
    <button class="btn btn-sm btn-ghost ml-4 " type="button" > x </button>
    `;

    toast.querySelector('button')?.addEventListener('click', () => {
      toastContainer.removeChild(toast);// ή toast.remove();
    });

    if (route){
      toast.addEventListener('click',()=> {
        this.router.navigateByUrl(route);
      })
    }

    // Προσθήκη του toast στο container
    toastContainer.append(toast);

    setTimeout(() => {
      if (toastContainer.contains(toast)) {
        // toastContainer.removeChild(toast);
        toast.style.opacity = '0';
        setTimeout(() => toast.remove(), 500);
      }
    }, duration);
  }

  success(message: string, duration?: number,avatar?: string, route?: string) {
    this.createToastElement(message, 'alert-success', duration, avatar, route);
  }

  info(message: string, duration?: number,avatar?: string, route?: string) {
    this.createToastElement(message, 'alert-info', duration, avatar, route);
  }

  error(message: string, duration?: number, avatar?: string, route?: string) {
    this.createToastElement(message, 'alert-error', duration, avatar, route);
  }

  warning(message: string, duration?: number, avatar?: string, route?: string) {
    this.createToastElement(message, 'alert-warning', duration, avatar, route);
  }
}


