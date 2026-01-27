import { Pipe, PipeTransform } from '@angular/core';
// Pipe δηλώνει μια μετατροπή που μπορεί να χρησιμοποιηθεί στις Angular templates
// για να κάνουμε μετατροπές δεδομένων απευθείας στην προβολή html
// θα δηλώσω στο Imports του component που θα το χρησιμοποιήσω Imports: [AgePipe]
// θα το χρησιμοποιήσω στο html ως εξής: {{ member.dateOfBirth | age }}
@Pipe({
  name: 'age'
})
export class AgePipe implements PipeTransform {

  // το value είναι μια ημερομηνία γέννησης σε μορφή string και επιστρέφουμε την ηλικία σε αριθμό
  transform(value:string): number {
    const today = new Date();
    const birthDate = new Date(value);
    // αρχικός υπολογισμός ηλικίας
    let age = today.getFullYear() - birthDate.getFullYear();
    // έλεγχος αν έχει περάσει ο μηνασ γενεθλίων φέτος
    const monthDifference = today.getMonth() - birthDate.getMonth();
    
    // αν δεν έχει περάσει η ημερομηνια γενεθλίων για φετος, μειώνουμε την ηλικία κατά 1
    if(monthDifference<0 || (monthDifference===0 && today.getDate()<birthDate.getDate())){
      age--;
    }
    return age;
  }

}
