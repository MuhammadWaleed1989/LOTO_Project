import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';

import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { first } from 'rxjs/operators';
import { MustMatch } from './validation.mustmatch';
import { UserService } from '../../services/user.service';
import { Users } from '../usermanagement/users.model';

@Component({
  selector: 'app-updatepassword',
  templateUrl: './updatepassword.component.html',
  styleUrls: ['./updatepassword.component.scss']
})

/**
* Ecomerce Customers component
*/
export class UpdatePasswordComponent implements OnInit {

  // bread crumb items
  breadCrumbItems: Array<{}>;
  statusMessage: string; // message to show after delete the record
  users: Users[] = [];
  term: any; // Variable to search the text in grid
  typeResetPasswordsubmit: boolean;
  public isAdmin: boolean = false;
  currentUserID: number;

  typeResetPasswordForm: FormGroup; // type validation form

  constructor(private userService: UserService, private modalService: NgbModal, public formBuilder: FormBuilder) {

    this.typeResetPasswordsubmit = false;
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
    this.currentUserID = Number(localStorage.getItem('id'));
    this.isAdmin = this.getBoolean(localStorage.getItem('isAdmin'));
    // setup the top lables
    this.breadCrumbItems = [{
      label: 'LOTO Game'
    }, {
      label: 'Reset Password',
      active: true
    }];

    this.typeResetPasswordForm = this.formBuilder.group({
      userID: [this.currentUserID],
      oldPassword: ['', [Validators.required]],
      newPassword: ['', [Validators.required]],
      confimPassword: ['', [Validators.required]],
    }, {
      validator: MustMatch('newPassword', 'confimPassword'),
    });

  }

  get resetType() {
    return this.typeResetPasswordForm.controls;
  }

  typeResetPasswordSubmit() {

    if (this.typeResetPasswordForm.valid) {
      this.typeResetPasswordsubmit = false;

      var objUserInfo = {
        'userID': Number(this.typeResetPasswordForm.get('userID').value),
        'password': this.typeResetPasswordForm.get('newPassword').value,
      };

      if (this.typeResetPasswordForm.get('userID').value === "-1") {

        this.userService.updateUserPassword(objUserInfo).subscribe(response => {
          this.statusMessage = 'User password updated successfully, please logout and re-login again'
        }, (error) => {
          console.log('error during post is ', error)
        });
      } else {
        this.userService.updateUserPassword(objUserInfo).subscribe(response => {
          this.statusMessage = 'User password updated successfully, please logout and re-login again'
        }, (error) => {
          console.log('error during post is ', error)
        });
      }
      this.modalService.dismissAll();

    } else {
      // validate all form fields
      this.typeResetPasswordsubmit = true;
    }
  }
}