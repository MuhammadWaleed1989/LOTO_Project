import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';

import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { first } from 'rxjs/operators';

import { UserService } from '../../services/user.service';
import { Users } from './users.model';
import { environment } from '../../../environments/environment';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';
import { User } from 'src/app/core/models/auth.models';

@Component({
  selector: 'app-customers',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})

/**
* Ecomerce Customers component
*/
export class UsersComponent implements OnInit {

  // bread crumb items
  breadCrumbItems: Array<{}>;
  statusMessage: string; // message to show after delete the record
  users: Users[] = [];
  term: any; // Variable to search the text in grid
  typesubmit: boolean;
  public isAdmin: boolean = false;
  // page
  currentpage: number;
  totalPages: number;
  typeValidationForm: FormGroup; // type validation form
  public _hubConnection: HubConnection;
  constructor(private userService: UserService, private modalService: NgbModal, public formBuilder: FormBuilder) {
    this.typesubmit = false;
  }
  private getBoolean(value) {
    switch (value) {
      case true:
      case "true":
      case 1:
      case "1":
      case "on":
      case "yes":
        return true;
      default:
        return false;
    }
  }
  ngOnInit() {
    this.isAdmin = this.getBoolean(localStorage.getItem('isAdmin'));
    // setup the top lables
    this.breadCrumbItems = [{
      label: 'LOTO Game'
    }, {
      label: 'User Management',
      active: true
    }];

    this.currentpage = 1;
    /**
     * Fetches the user data
     */
    //this.loadAllUsers();
    this._hubConnection = new HubConnectionBuilder().withUrl(`${environment.apiUrl}/echo`).build();
    this._hubConnection.on('UserList', (data: any) => {
      this.users = data;
      this.totalPages = data.length;
    });
    this._hubConnection.start()
      .then(() => {
        this._hubConnection.invoke('Start');
        console.log('Hub connection started')
      })
      .catch(err => {
        console.log('Error while establishing connection')
      });
    this.typeValidationForm = this.formBuilder.group({
      userID: ['-1'],
      coinsCost: ['', [Validators.required]],
      remainingCoins: ['', [Validators.required]],
      usedCoins: ['', [Validators.required]],
    });
    //console.log(this._hubConnection);
  }
  openModal(largeDataModal: any, info: Users) {
    this.typesubmit = false;
    this.modalService.open(largeDataModal, {
      backdrop: 'static',
      size: 'lg',
      scrollable: true
    });
    if (info) {
      this.typeValidationForm.patchValue({
        userID: info.userID,
        coinsCost: "",
        remainingCoins: info.remainingCoins,
        usedCoins: info.usedCoins,


      });
    } else {

      this.typeValidationForm.patchValue({
        userID: "-1",
        coinsCost: "",
        remainingCoins: "",
        usedCoins: ""
      });
    }
  }
  get type() {
    return this.typeValidationForm.controls;
  }
  /**
  * Type validation form submit data
  */
  typeSubmit() {

    if (this.typeValidationForm.valid) {
      this.typesubmit = false;

      var objUserInfo = {
        'userID': Number(this.typeValidationForm.get('userID').value),
        'coinsCost': Number(this.typeValidationForm.get('remainingCoins').value) + Number(this.typeValidationForm.get('usedCoins').value) + Number(this.typeValidationForm.get('coinsCost').value),
      };

      if (this.typeValidationForm.get('userID').value === "-1") {

        this.userService.updateUser(objUserInfo).subscribe(response => {
          this.statusMessage = 'Coins added to user successfully.'
          this._hubConnection.invoke('Start');
        }, (error) => {
          console.log('error during post is ', error)
        });
      } else {
        this.userService.updateUser(objUserInfo).subscribe(response => {
          this.statusMessage = 'Coins added to user successfully.'
          this._hubConnection.invoke('Start');
        }, (error) => {
          console.log('error during post is ', error)
        });
      }
      this.modalService.dismissAll();

    } else {
      // validate all form fields
      this.typesubmit = true;
    }
  }

  private loadAllUsers() {
    this.userService.getAll().pipe(first()).subscribe(users => {
      this.users = users;
      this.totalPages = users.length;
    });
  }
}