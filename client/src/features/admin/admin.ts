import { Component, inject } from '@angular/core';
import { AdminService } from '../../core/services/admin-service';
import { UserManagement } from './user-management/user-management';
import { PhotoManagement } from './photo-management/photo-management';
import { AccountService } from '../../core/services/account-service';

@Component({
  selector: 'app-admin',
  imports: [UserManagement,PhotoManagement],
  templateUrl: './admin.html',
  styleUrl: './admin.css',
})
export class Admin {
  private adminService=inject(AdminService);
  protected accountService=inject(AccountService);
  protected activeTab="photos";
  tabs=[
    {label:"Photo Moderation",value:"photos"},
    {label:"Roles Management",value:"roles"}
  ]

  setTab(value:string){
    this.activeTab=value;
  }

}
