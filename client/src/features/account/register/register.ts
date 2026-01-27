import { Component, inject, output, signal } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { AccountService } from '../../../core/services/account-service';
import { TextInput } from "../../../shared/text-input/text-input";
import { RegisterCreds } from '../../../types/user';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, TextInput],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private accountService = inject(AccountService);
  protected cancelRegister = output<boolean>();
  protected credentialForm: FormGroup;
  protected profileForm: FormGroup;
  currentStep = signal<number>(1);
  protected validationErrors = signal<string[]>([]);

  constructor() {
    this.initializeForm();
  }


  initializeForm() {
    this.credentialForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      displayName: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(20)]],
      confirmPassword: ['', [Validators.required, this.matchValues('password')]],
    });
    // Ενημέρωση της εγκυρότητας του πεδίου "confirmPassword" όταν αλλάζει το "password"
    this.credentialForm.controls['password']?.valueChanges.subscribe({
      next: () => this.credentialForm.controls['confirmPassword']?.updateValueAndValidity()// Ενημέρωση της εγκυρότητας
    });

    //δημιουργία 2ης φόρμας προφίλ
    this.profileForm = this.fb.group({
      gender: ['male', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
    });
  }

  // 1. Το "Εργοστάσιο": Δέχεται παραμέτρους και επιστρέφει μια συνάρτηση τύπου ValidatorFn
  matchValues(matchTo: string): ValidatorFn {
    // 2. Η "Συνάρτηση Ελέγχου": Αυτή εκτελείται από την Angular κάθε φορά που αλλάζει η τιμή
    return (control: AbstractControl): ValidationErrors | null => {
      const parent = control.parent; // Πρόσβαση σε όλη τη φόρμα (FormGroup)

      if (!parent) return null;      // Αμυντικός προγραμματισμός

      // Λήψη τιμής από το άλλο πεδίο (π.χ. 'password')
      const matchValue = parent.get(matchTo)?.value;

      // 3. Το Αποτέλεσμα: null = Έγκυρο, Object = Λάθος
      return control.value === matchValue ? null : { misMatching: true };
    }
  }

  getMaxDate() {
    const today = new Date();
    today.setFullYear(today.getFullYear() - 18);
    return today.toISOString().split('T')[0];//επιστρέφει την ημερομηνία σε μορφή 'YYYY-MM-DD' δεν θέλω την ώρα
  }

  register() {
    if (this.credentialForm.valid && this.profileForm.valid) {
      const formData = { ...this.credentialForm.value, ...this.profileForm.value };
      console.log("formData:", formData);

      this.accountService.register(formData).subscribe({
        next: () => {
          this.router.navigateByUrl('/members');
        },
        error: error => {
          console.log(error);
          this.validationErrors.set(error);
        }
      });
    }
  }

  nextStep() {
    if (this.credentialForm.valid) {
      this.currentStep.update(n => n + 1);
    }
  }

  prevStep() {
    this.currentStep.update(n => n - 1);
  }

  cancel() {
    console.log('cancel...');
    this.cancelRegister.emit(false);
  }

}
