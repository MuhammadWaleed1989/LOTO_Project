import { Component, OnInit, AfterViewInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

import { AuthenticationService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.scss']
})
export class SignupComponent implements OnInit, AfterViewInit {

  signupForm: FormGroup;
  submitted = false;
  error = '';
  successmsg = false;

  // set the currenr year
  year: number = new Date().getFullYear();

  // tslint:disable-next-line: max-line-length
  constructor(private formBuilder: FormBuilder, private route: ActivatedRoute, private router: Router, private authenticationService: AuthenticationService) { }

  ngOnInit() {

    this.signupForm = this.formBuilder.group({
      Email: ['', [Validators.required, Validators.email]],
      FirstName: ['', Validators.required],
      LastName: ['', Validators.required],
      Password: ['', Validators.required],
    });
  }

  ngAfterViewInit() {
  }

  // convenience getter for easy access to form fields
  get f() { return this.signupForm.controls; }

  /**
   * On submit form
   */
  onSubmit() {
    this.submitted = true;

    // stop here if form is invalid
    if (this.signupForm.invalid) {
      return;
    }

    var datetimehigh = new Date();
    var sDate = datetimehigh.getFullYear() + '' + datetimehigh.getMonth() + '' +
      datetimehigh.getHours() + '' + datetimehigh.getMinutes() + '' + datetimehigh.getSeconds();

    var object = {
      'UserID': -1, 'FirstName': this.signupForm.get('FirstName').value, 'LastName': this.signupForm.get('LastName').value,
      'UserName': this.signupForm.get('FirstName').value + sDate, 'Email': this.signupForm.get('Email').value,
      'IsAdmin': false, 'IsDeleted': false, 'Password': this.signupForm.get('Password').value
    };

    this.authenticationService.register(object).subscribe(response => {
      // this.statusMessage = 'User is created Successfully.'
      // this.loadAllUsers();
      this.successmsg = true;
      if (this.successmsg) {
        this.router.navigate(['/dashboard']);
      }
    }, (error) => {
      console.log('error during post is ', error)
    });
    // this.authenticationService.register(object).then((res: any) => {
    //   this.successmsg = true;
    //   if (this.successmsg) {
    //     this.router.navigate(['/dashboard']);
    //   }
    // })
    //   .catch (error => {
    //   this.error = error ? error : '';
    // });
  }
}
