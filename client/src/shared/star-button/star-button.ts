import { Component, input, output, signal } from '@angular/core';

@Component({
  selector: 'app-star-button',
  imports: [],
  templateUrl: './star-button.html',
  styleUrl: './star-button.css',
})
export class StarButton {
  disabled= input(true);
  selected=input(false)
  clickEvent=output<Event>();

  onClick(event:Event){
    this.clickEvent.emit(event);
  }

}
