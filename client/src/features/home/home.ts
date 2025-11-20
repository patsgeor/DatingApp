import { Component, input, Input, signal } from '@angular/core';
import { Register } from "../account/register/register";
import { User } from '../../types/user';

@Component({
  selector: 'app-home',
  imports: [Register],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home {
 

  registerMode = signal(false);

  showRegiser(value :boolean) {
    this.registerMode.set(value)
  }

}
