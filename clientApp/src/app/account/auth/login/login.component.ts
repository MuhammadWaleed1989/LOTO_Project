import { Component, OnInit, AfterViewInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { first } from 'rxjs/operators';
import { AuthenticationService } from '../../../core/services/auth.service';
import { ActivatedRoute, Router } from '@angular/router';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';
import { environment } from '../../../../environments/environment';
import { Users } from '../../../pages/usermanagement/users.model';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})

/**
 * Login component
 */
export class LoginComponent implements OnInit, AfterViewInit {

  loginForm: FormGroup;
  submitted = false;
  error = '';

  // set the currenr year
  year: number = new Date().getFullYear();
  public _hubConnection: HubConnection;
  users: Users[] = [];
  // tslint:disable-next-line: max-line-length
  constructor(private formBuilder: FormBuilder, private route: ActivatedRoute, private router: Router, private authenticationService: AuthenticationService) { }

  ngOnInit() {
    this._hubConnection = new HubConnectionBuilder().withUrl(`${environment.apiUrl}/echo`).build();
    this._hubConnection.on('UserList', (data: any) => {
      this.users = data;
    });
    this._hubConnection.start()
      .then(() => {

        console.log('Hub connection started')
      })
      .catch(err => {
        console.log('Error while establishing connection')
      });

    console.log(this._hubConnection);
    this.loginForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
    });

    // reset login status
    this.authenticationService.logout();

  }

  ngAfterViewInit() {
  }

  // convenience getter for easy access to form fields
  get f() { return this.loginForm.controls; }

  /**
   * Form submit
   */
  onSubmit() {
    this.submitted = true;

    // stop here if form is invalid
    if (this.loginForm.invalid) {
      return;
    }
    this.authenticationService.login(this.f.email.value, this.f.password.value)
      .pipe(first())
      .subscribe(
        data => {
          this._hubConnection.invoke('Start');
          if (!localStorage.getItem('isAdmin')) { this.router.navigate(['/usermanagement']); }
          else {
            this.router.navigate(['/games']);
          }

        },
        error => {
          // this.alertService.error(error);
          // this.loading = false;
        });
    // this.authenticationService.login(this.f.email.value, this.f.password.value).then((res: any) => {
    //   this.router.navigate(['/dashboard']);
    // })
    //   .catch(error => {
    //     this.error = error ? error : '';
    //   });
  }
}
