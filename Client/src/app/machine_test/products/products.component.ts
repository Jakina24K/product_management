import {
    Component,
    ElementRef,
    inject,
    OnInit,
    signal,
    ViewChild,
    viewChild,
} from '@angular/core';
import { ProductService } from '../../service/productSrv/product.service';
import { IProduct } from '../../model/product';
import {
    FormControl,
    FormGroup,
    ReactiveFormsModule,
    Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { LoginService } from '../../service/loginSrv/login.service';

@Component({
    selector: 'app-products',
    standalone: true,
    imports: [ReactiveFormsModule],
    templateUrl: './products.component.html',
    styleUrl: './products.component.css',
})
export class ProductsComponent implements OnInit {
    @ViewChild('newModal') modal: ElementRef | undefined;

    productSrv = inject(ProductService);

    loginSrv = inject(LoginService);

    productList = signal<IProduct[]>([]);

    productObj: IProduct = new IProduct();

    productForm: FormGroup = new FormGroup({});

    constructor(private router: Router) {
        this.initializeForm();
    }

    ngOnInit(): void {
        this.loadProducts();
    }

    initializeForm() {
        this.productForm = new FormGroup({
            productId: new FormControl(this.productObj.id),
            productName: new FormControl(this.productObj.productName, [
                Validators.required,
                Validators.minLength(4),
            ]),
            shortName: new FormControl(this.productObj.shortName),
            category: new FormControl(this.productObj.category),
            sku: new FormControl(this.productObj.sku),
            price: new FormControl(this.productObj.price),
            thumbnailImageUrl: new FormControl(
                this.productObj.thumbnailImageUrl,
            ),
            deliveryTimeSpan: new FormControl(this.productObj.deliveryTimeSpan),
        });
    }

    loadProducts() {
        this.productSrv.getAllProducts().subscribe(
            (res: IProduct[]) => {
                console.log(res);
                this.productList.set(res);
            },
            (error) => {
                this.router.navigate(['/']);
            },
        );
    }

    openModal() {
        if (this.modal) {
            this.modal.nativeElement.style.display = 'block';
        }
    }

    closeModal() {
        if (this.modal) {
            this.modal.nativeElement.style.display = 'none';
        }
    }

    SaveProduct() {
        this.productSrv.SaveProducts(this.productForm.value).subscribe(
            (res: IProduct) => {
                alert('Product Added');
                this.loadProducts();
                this.closeModal();
            },
            (error) => {
                alert('API error');
            },
        );
    }

    UpdateProduct(id: number) {
        this.productSrv.UpdateProduct(id, this.productForm.value).subscribe(
            (res: IProduct) => {
                alert('Product Updated');
                this.loadProducts();
                this.closeModal();
            },
            (error) => {
                alert('API error');
            },
        );
    }

    onDelete(id: number) {
        const isConfirm = confirm('Are you sure?');
        if (isConfirm) {
            this.productSrv.DeleteProduct(id).subscribe(
                (res: IProduct) => {
                    alert('Product Deleted');
                    this.loadProducts();
                },
                (error) => {
                    alert('API error');
                },
            );
        }
    }

    onEdit(id: number) {
        this.productSrv.getSingleProduct(id).subscribe(
            (res: IProduct) => {
                this.productObj = res;
                this.initializeForm();
                this.openModal();
            },
            (error) => {
                alert('API error');
            },
        );
    }

    UserLogOut() {
        this.loginSrv.UserLogOut().subscribe(
            () => {
                this.router.navigate(['/']);
            },
            () => {
                alert('API error');
            },
        );
    }
}
