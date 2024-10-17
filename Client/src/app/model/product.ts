export class IProduct {
    id: number
    productName: string
    shortName: string
    category: string
    sku: any
    price: string
    thumbnailImageUrl: string
    deliveryTimeSpan: string

    constructor() {
      this.id = 0
      this.productName = ''
      this.shortName = ''
      this.category = ''
      this.price = ''
      this.thumbnailImageUrl = ''
      this.deliveryTimeSpan = ''
    }
  }