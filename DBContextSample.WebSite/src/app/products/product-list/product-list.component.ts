import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css']
})
export class ProductListComponent {
  title: string = "Produits";
  displayImages: boolean = false;
  products: any[] = [
    {
      name: 'Froment',
      code: 'C01',
      releaseDate: new Date("2022-09-01"),
      price: 2.12,
      rating: 5,
      imgUrl: "favicon.ico"
    },
    {
      name: 'Escourgeon',
      code: 'C02'
    }
  ];

  toggleImages(): void {
    this.displayImages = !this.displayImages;
  }
}
