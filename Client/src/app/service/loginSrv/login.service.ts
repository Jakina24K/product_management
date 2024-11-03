import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IUserInfo } from '../../model/userInfo';

@Injectable({
    providedIn: 'root',
})
export class LoginService {
    constructor(private http: HttpClient) {}

    UserLogIn(obj: IUserInfo) {
        return this.http.post<IUserInfo>(
            `http://localhost:5283/api/products/login`,
            obj,
            {
                withCredentials: true,
            },
        );
    }

    UserLogOut() {
        return this.http.delete('http://localhost:5283/api/products/logout', {
            withCredentials: true,
        });
    }
}
