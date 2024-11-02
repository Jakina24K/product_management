import { Component, inject } from '@angular/core';
import {
    FormControl,
    FormGroup,
    ReactiveFormsModule,
    Validators,
} from '@angular/forms';
import { IUserInfo } from '../../model/userInfo';
import { LoginService } from '../../service/loginSrv/login.service';
import { Router } from '@angular/router';

@Component({
    selector: 'app-login',
    standalone: true,
    imports: [ReactiveFormsModule],
    templateUrl: './login.component.html',
    styleUrl: './login.component.css',
})
export class LoginComponent {
    loginSrv = inject(LoginService);

    LoginObj: IUserInfo = new IUserInfo();

    LoginForm: FormGroup = new FormGroup({});

    constructor(private router: Router) {
        this.initializeForm();
    }

    initializeForm() {
        this.LoginForm = new FormGroup({
            email: new FormControl(this.LoginObj.email),
            password: new FormControl(this.LoginObj.password),
        });
    }

    navigateToRoute() {
        this.router.navigate(['/products-component']);
    }

    LogUser() {
        this.loginSrv.UserLogIn(this.LoginForm.value).subscribe(
            (res: IUserInfo) => {
                alert('You are logged');
                this.navigateToRoute();
            },
            (error) => {
                alert('API error');
            },
        );
    }
}
