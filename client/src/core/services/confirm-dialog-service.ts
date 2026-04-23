import { Injectable } from '@angular/core';
import { ConfirmDialog } from '../../shared/confirm-dialog/confirm-dialog';

@Injectable({
  providedIn: 'root'
})
export class ConfirmDialogService {
  protected dialogComponent?: ConfirmDialog;

  register(component :ConfirmDialog){
    this.dialogComponent=component;
  }

  needConfirm(message:string):Promise<boolean>{
    if(!this.dialogComponent ) throw new Error('Confirm dialog component is not registered');
    return  this.dialogComponent.open(message);
  }
  
  
}
