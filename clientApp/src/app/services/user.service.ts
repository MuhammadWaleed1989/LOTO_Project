import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Users } from '../pages/usermanagement/users.model';
import { AdminConfig } from '../pages/adminconfiguration/adminconfig.model';

@Injectable({ providedIn: 'root' })
export class UserService {
    constructor(private http: HttpClient) { }

    getAll() {
        return this.http.get<Users[]>(`${environment.apiUrl}/api/UserInfo`);
    }

    // postUser(user: any) {
    //     return this.http.post(`${environment.apiUrl}/user/`, user);
    // }

    updateUser(user: any) {
        return this.http.post(`${environment.apiUrl}/api/UserInfo/UpdatedCoinsDetail`, user);
    }
    updateUserPassword(user: any) {
        return this.http.post(`${environment.apiUrl}/api/UserInfo/UpdateUserPassword`, user);
    }
    getAllConfiguration() {
        return this.http.get<AdminConfig[]>(`${environment.apiUrl}/api/AdminConfig`);
    }
    updateConfiguration(config: any) {
        return this.http.post<AdminConfig[]>(`${environment.apiUrl}/api/AdminConfig`, config);
    }

    // deleteUser(id: number) {
    //     return this.http.delete(`${environment.apiUrl}/user/${id}`);
    // }
}