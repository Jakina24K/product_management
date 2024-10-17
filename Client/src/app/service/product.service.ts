import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IProduct } from '../model/product';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  apiUrl: string = 'http://localhost:7283/api/';
  constructor(private http: HttpClient) {}

  getAllProducts(): Observable<IProduct[]> {
    return this.http.get<IProduct[]>(`${this.apiUrl}Products`);
  }

  getSingleProduct(id: number): Observable<IProduct> {
    return this.http.get<IProduct>(`${this.apiUrl}products/${id}`);
  }

  SaveProducts(obj: IProduct): Observable<IProduct> {
    return this.http.post<IProduct>(`${this.apiUrl}products`, obj);
  }

  UpdateProduct(id: number, obj: IProduct): Observable<IProduct> {
    console.log(id);
    return this.http.put<IProduct>(`${this.apiUrl}products/${id}`, obj);
  }

  DeleteProduct(id: number): Observable<IProduct> {
    console.log(id);
    return this.http.delete<IProduct>(`${this.apiUrl}products/${id}`);
  }
}
