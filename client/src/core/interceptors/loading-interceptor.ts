import { HttpEvent, HttpInterceptorFn, HttpParams } from '@angular/common/http';
import { inject } from '@angular/core';
import { BusyService } from '../services/busy-service';
import { delay, finalize, of, tap } from 'rxjs';
import { HttpResponse } from '@microsoft/signalr';

// Απλο cache για τις GET αιτησεις
//το οριζουμε εκτος του interceptor για να διατηρειται αναμεσα σε πολλες κλησεις και να μην καθαριζεται καθε φορα
const cache = new Map<string,HttpEvent<unknown>>();

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const busyService=inject(BusyService);

  const generateCacheKey=(url:string,params:HttpParams) :string =>{
    const paramString = params.keys().map(key => `${key}=${params.get(key)}`).join('&');
    return paramString ? `${url}?${paramString}` : `${url}`;
  }

  const cacheKey = generateCacheKey(req.url,req.params);

  const invalidateCache=(urlPattern : string)=>{
    for( const key of cache.keys()){
      if(key.includes(urlPattern)){
        cache.delete(key);
        console.log( `Cache invalidated for: ${key}`)
      }
    }
  }

  if(req.method==='POST' && req.url.includes(`/likes`)){
    invalidateCache('/likes')
  }

  if(req.method==='POST' && req.url.includes(`/messages`)){
    invalidateCache('/messages')
  }

  if(req.method==='POST' && req.url.includes(`/logout`)){
    cache.clear();
  }

  // αν υπαρχει η απαντηση στο cash, την επιστρεφουμε αμεσως χωρις να κανουμε το request
  if(req.method==='GET'){
    const cacheResponse=cache.get(cacheKey);
    if(cacheResponse){
       return of( cacheResponse);// επιστρεφουμε την αποθηκευμενη απαντηση ως observable, για να ταιριαζει με τον τυπο επιστροφης
    }
  }

  // με καθε request που ξεκιναει, αυξανουμε τον μετρητη των busy requests
  busyService.busy();

  return next(req).pipe(
    delay(500), // προσθετουμε μια τεχνητη καθυστερηση 500ms για να φαινεται το loading spinner

    // αποθηκευουμε την απαντηση στο cash για μελλοντικη χρηση
    tap(event => { if (event instanceof HttpResponse) 
      { cache.set(cacheKey, event); } 
    }),

    // οταν ολοκληρωνεται το request, μειωνουμε τον μετρητη των busy requests
    // ειτε η απαντηση ειναι επιτυχης ειτε οχι
    finalize(() => {
      busyService.idle();
    })
  );
};
