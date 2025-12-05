import { ApplicationConfig, inject, provideAppInitializer, provideBrowserGlobalErrorListeners, provideZonelessChangeDetection } from '@angular/core';
import { provideRouter, withViewTransitions } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient } from '@angular/common/http';
import { InitService } from '../core/services/init-service';
import { last, lastValueFrom } from 'rxjs';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZonelessChangeDetection(),
    provideRouter(routes, withViewTransitions()),
    provideHttpClient(), //Παρέχει το HttpClient service του Angular, για να μπορείς να κάνεις HTTP requests (GET, POST, κλπ).

    // Παρέχει την υπηρεσία αρχικοποίησης της εφαρμογής πριν από την εκκίνηση.
    provideAppInitializer( () => {
      const initService = inject(InitService);

      console.log('App Initializer: Εκκίνηση αρχικοποίησης εφαρμογής...');
      return new Promise<void>(async (resolve) => {
        setTimeout(async () => {
          try {
            console.log('App Initializer: Εκτέλεση InitService...');
            return await lastValueFrom(initService.init()); // Περιμένει να ολοκληρωθεί η αρχικοποίηση       
          } finally {
            console.log('App Initializer: Αρχικοποίηση ολοκληρώθηκε.');
            // Αφαιρεί το αρχικό splash screen μετά την ολοκλήρωση της αρχικοποίησης
            const splash = document.getElementById('initial-splash');
            if (splash) {
              splash.remove();
            }
            resolve();//είναι το σήμα που λέει στο Angular:"Η αρχικοποίηση ολοκληρώθηκε".
          }
        }, 500); // Εξασφαλίζει ότι το splash screen θα είναι ορατό για τουλάχιστον 500ms
      });
    })
  ]
};
