import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'timeAgo'
})
export class TimeAgoPipe implements PipeTransform {

   transform(value: string): string {
        if (value) {
            const seconds = Math.floor((+new Date() - +new Date(value)) / 1000); //Με το + μετατρέπουμε την ημερομηνία σε milliseconds 
            // και διαιρώ με το 1000 για μετατροπή σε δευτερόλεπτα

            if (seconds < 29) // less than 30 seconds ago will show as 'Just now'
                return 'Just now';
            const intervals: { [key: string]: number } = {
                'year': 31536000,
                'month': 2592000,
                'week': 604800,
                'day': 86400,
                'hour': 3600,
                'minute': 60,
                'second': 1
            };
            let counter;
            for (const i in intervals) {
                counter = Math.floor(seconds / intervals[i]);
                if (counter > 0)
                    if (counter === 1) {
                      //Μόλις βρεθεί το πρώτο μεγαλύτερο διάστημα που χωράει:σταματά ουσιαστικά ο βρόχος με return
                        return counter + ' ' + i + ' ago'; // singular (1 day ago)  
                    } else {
                        return counter + ' ' + i + 's ago'; // plural (2 days ago)
                    }
            }
        }
        return value;
    }

}
