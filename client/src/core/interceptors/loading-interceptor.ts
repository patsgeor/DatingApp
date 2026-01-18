import { HttpEvent, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { BusyService } from '../services/busy-service';
import { delay, finalize, of, tap } from 'rxjs';

// Απλο cache για τις GET αιτησεις
//το οριζουμε εκτος του interceptor για να διατηρειται αναμεσα σε πολλες κλησεις και να μην καθαριζεται καθε φορα
const cache = new Map<string,HttpEvent<unknown>>();

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const busyService=inject(BusyService);

  // αν υπαρχει η απαντηση στο cash, την επιστρεφουμε αμεσως χωρις να κανουμε το request
  if(req.method==='GET'){
    const cashResponse=cache.get(req.url);
    if(cashResponse){
       return of( cashResponse);// επιστρεφουμε την αποθηκευμενη απαντηση ως observable, για να ταιριαζει με τον τυπο επιστροφης
    }
  }

  // με καθε request που ξεκιναει, αυξανουμε τον μετρητη των busy requests
  busyService.busy();

  return next(req).pipe(
    delay(500), // προσθετουμε μια τεχνητη καθυστερηση 500ms για να φαινεται το loading spinner

    // αποθηκευουμε την απαντηση στο cash για μελλοντικη χρηση
    tap(response => {
      cache.set(req.url,response);
      // αν η αιτηση δεν ειναι GET, διαγραφουμε την αποθηκευμενη απαντηση απο το cache γιατι κατι αλλαξε
      if(req.method!=='GET'){
        cache.clear();
        }
    }),

    // οταν ολοκληρωνεται το request, μειωνουμε τον μετρητη των busy requests
    // ειτε η απαντηση ειναι επιτυχης ειτε οχι
    finalize(() => {
      busyService.idle();
    })
  );
};
