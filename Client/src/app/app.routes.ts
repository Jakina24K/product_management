import { Routes } from '@angular/router';
import { ProductsComponent } from './machine_test/products/products.component';
import { LoginComponent } from './machine_test/login/login.component';

export const routes: Routes = [
    {
      path: '',
      component: LoginComponent,
    },
    {
        path: 'products-component',
        component: ProductsComponent,
    },
];
