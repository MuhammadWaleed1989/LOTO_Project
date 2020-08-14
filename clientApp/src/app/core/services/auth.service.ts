import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { getFirebaseBackend } from '../../authUtils';
import { environment } from '../../../environments/environment';
import { User } from '../models/auth.models';

@Injectable({ providedIn: 'root' })

export class AuthenticationService {

    private currentUserSubject: BehaviorSubject<User>;
    public currentUserDetail: Observable<User>;
    user: User;

    constructor(private http: HttpClient) {
        this.currentUserSubject = new BehaviorSubject<User>(JSON.parse(localStorage.getItem('currentUser')));
        this.currentUserDetail = this.currentUserSubject.asObservable();
    }


    /**
     * Returns the current user
     */
    public currentUser(): User {
        return getFirebaseBackend().getAuthenticatedUser();
    }

    /**
     * Performs the auth
     * @param email email of user
     * @param password password of user
     */
    login(username: string, password: string) {
        return this.http.post<any>(`${environment.apiUrl}/api/UserInfo/authenticate`, { username, password })
            .pipe(map(data => {
                // login successful if there's a jwt token in the response
                if (data && data.token) {
                    // store user details and jwt token in local storage to keep user logged in between page refreshes
                    localStorage.setItem('currentUser', JSON.stringify(data.firstName));
                    localStorage.setItem('access_token', data.token);
                    this.currentUserSubject.next(data.firstName);
                }

                return data;
            }));
    }

    /**
     * Performs the register
     * @param email email
     * @param password password
     */
    register(user: any) {
        return this.http.post(`${environment.apiUrl}/api/UserInfo`, user);
    }

    /**
     * Reset password
     * @param email email
     */
    resetPassword(email: string) {
        return getFirebaseBackend().forgetPassword(email).then((response: any) => {
            const message = response.data;
            return message;
        });
    }

    /**
     * Logout the user
     */
    logout() {
        // remove user from local storage to log user out
        localStorage.removeItem('currentUser');
        localStorage.removeItem('access_token');
        this.currentUserSubject.next(null);
    }
}

