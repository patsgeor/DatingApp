import { Component, inject, input, output } from '@angular/core';
import { RegisterCreds, User } from '../../../types/user';
import { createLinkedSignal } from '@angular/core/primitives/signals';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../../../core/services/account-service';

@Component({
  selector: 'app-register',
  imports: [FormsModule],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  private accountService=inject(AccountService);
  protected creds ={} as RegisterCreds;
  protected cancelRegister=output<boolean>();

  register(){
    console.log('register...');
    this.accountService.register(this.creds).subscribe({
      next: res => {
        console.log(res);
        this.cancel();
      },
      error: error => console.log(error)
    });
  }

  cancel(){
    console.log('cancel...');
    this.cancelRegister.emit(false);
  }

}
