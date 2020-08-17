import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';

import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { first } from 'rxjs/operators';

import { UserService } from '../../services/user.service';
import { Users } from './gamestart.model';
import { environment } from '../../../environments/environment';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';

@Component({
  selector: 'app-customers',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})

/**
* Ecomerce Customers component
*/
export class GameStartComponent implements OnInit {

  // bread crumb items
  breadCrumbItems: Array<{}>;
  statusMessage: string; // message to show after delete the record
  users: Users[] = [];
  term: any; // Variable to search the text in grid
  typesubmit: boolean;
  // page
  currentpage: number;
  totalPages: number;
  typeValidationForm: FormGroup; // type validation form
  public _hubConnection: HubConnection;
  constructor(private userService: UserService, private modalService: NgbModal, public formBuilder: FormBuilder) {
    this.typesubmit = false;
  }

  ngOnInit() {
    // setup the top lables
    this.breadCrumbItems = [{
      label: 'Skote'
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

    console.log(this._hubConnection);
  }


  private loadAllUsers() {
    this.userService.getAll().pipe(first()).subscribe(users => {
      this.users = users;
      this.totalPages = users.length;
    });
  }
}