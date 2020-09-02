import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';

import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { first } from 'rxjs/operators';
import { UserService } from '../../services/user.service';
import { AdminConfig } from './adminconfig.model';

@Component({
  selector: 'app-adminconfig',
  templateUrl: './adminconfig.component.html',
  styleUrls: ['./adminconfig.component.scss']
})

/**
* Ecomerce Customers component
*/
export class AdminConfigComponent implements OnInit {

  // bread crumb items
  breadCrumbItems: Array<{}>;
  statusMessage: string; // message to show after delete the record
  adminConfig: AdminConfig[] = [];
  term: any; // Variable to search the text in grid
  typeAdminIsSubmit: boolean;
  public isAdmin: boolean = false;
  currentUserID: number;

  typeAdminForm: FormGroup; // type validation form

  constructor(private userService: UserService, private modalService: NgbModal, public formBuilder: FormBuilder) {

    this.typeAdminIsSubmit = false;
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

    this.typeAdminForm = this.formBuilder.group({
      adminid: [-1],
      coinprice: ['', [Validators.required]],
      confirmseconds: ['', [Validators.required]]
    });
    this.loadConfiguration();
  }

  get configType() {
    return this.typeAdminForm.controls;
  }

  typeAdminConfigSubmit() {

    if (this.typeAdminForm.valid) {
      this.typeAdminIsSubmit = false;

      var objUserInfo = {
        'adminid': Number(this.typeAdminForm.get('adminid').value),
        'coinprice': this.typeAdminForm.get('coinprice').value,
        'confirmseconds': this.typeAdminForm.get('confirmseconds').value,
      };

      if (this.typeAdminForm.get('adminid').value === "-1") {

        this.userService.AddConfiguration(objUserInfo).subscribe(response => {
          this.statusMessage = 'Details added successfully'
          this.loadConfiguration();
        }, (error) => {
          console.log('error during post is ', error)
        });
      } else {
        this.userService.updateConfiguration(objUserInfo, Number(this.typeAdminForm.get('adminid').value)).subscribe(response => {
          this.statusMessage = 'Details updated successfully'
          this.loadConfiguration();
        }, (error) => {
          console.log('error during post is ', error)
        });
      }
      this.modalService.dismissAll();

    } else {
      // validate all form fields
      this.typeAdminIsSubmit = true;
    }
  }
  private loadConfiguration() {
    this.userService.getAllConfiguration().pipe(first()).subscribe(adminConfig => {
      this.adminConfig = adminConfig;
      this.statusMessage = '';
      if (this.adminConfig) {
        this.typeAdminForm.patchValue({

          adminid: adminConfig[0].adminid,
          coinprice: adminConfig[0].coinprice,
          confirmseconds: adminConfig[0].confirmseconds,
        });
      } else {

        this.typeAdminForm.patchValue({
          adminid: "-1",
          coinprice: "",
          confirmseconds: "",
        });
      }
    });
  }
}