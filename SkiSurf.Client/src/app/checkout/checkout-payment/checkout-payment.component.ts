import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { NavigationExtras, Router } from '@angular/router';
import { loadStripe, Stripe, StripeCardCvcElement, StripeCardExpiryElement, StripeCardNumberElement } from '@stripe/stripe-js';
import { ToastrService } from 'ngx-toastr';
import { firstValueFrom } from 'rxjs';
import { BasketService } from 'src/app/basket/basket.service';
import { Basket } from 'src/app/shared/models/basket';
import { OrderToCreate } from 'src/app/shared/models/order';
import { Address } from 'src/app/shared/models/user';
import { CheckoutService } from '../checkout.service';

@Component({
  selector: 'app-checkout-payment',
  templateUrl: './checkout-payment.component.html',
  styleUrls: ['./checkout-payment.component.scss']
})
export class CheckoutPaymentComponent implements OnInit {
  @Input() checkoutForm?: FormGroup;
  @ViewChild('cardNumber', { static: true }) cardNumberElement?: ElementRef;
  @ViewChild('cardExpiry', { static: true }) cardExpiryElement?: ElementRef;
  @ViewChild('cardCvc', { static: true }) cardCvcElement?: ElementRef;
  stripe: Stripe | null = null;
  cardNumber?: StripeCardNumberElement;
  cardExpiry?: StripeCardExpiryElement;
  cardCvc?: StripeCardCvcElement;
  cardHandler = this.onChange.bind(this);
  cardNumberComplete = false;
  cardExpiryComplete = false;
  cardCvcComplete = false;
  cardErrors: any;
  loading = false;

  constructor(private basketService: BasketService, private checkoutService: CheckoutService, 
      private toastr: ToastrService, private router: Router) {}
  ngOnInit(): void {
  
  }

 ngAfterViewInit(): void {
    loadStripe('pk_test_51PeSCiRxGAGT7qNlGsvEblTtMTvAqhwZbw2Vya8KM3AKtmOISx4yWykpCK29qZ8AeqhInpaKJLrp870LYTLOfZkj007zdXgyEA').
      then(stripe => { stripe = stripe; })
    const elements = this.stripe?.elements();
    if (elements) {
      this.cardNumber = elements.create('cardNumber');
      this.cardNumber.mount(this.cardNumberElement?.nativeElement);
      this.cardNumber.on('change', this.cardHandler);
  
      this.cardExpiry = elements.create('cardExpiry');
      this.cardExpiry.mount(this.cardExpiryElement?.nativeElement);
      this.cardExpiry.on('change', this.cardHandler);
  
      this.cardCvc = elements.create('cardCvc');
      this.cardCvc.mount(this.cardCvcElement?.nativeElement);
      this.cardCvc.on('change', this.cardHandler);
    }
  }

  ngOnDestroy(): void {
    this.cardNumber?.destroy();
    this.cardExpiry?.destroy();
    this.cardCvc?.destroy();
  }
  get paymentFormComplete() {
    return this.checkoutForm?.get('checkoutForm')?.valid 
      && this.cardNumberComplete 
      && this.cardExpiryComplete 
      && this.cardCvcComplete
  }


  onChange(event:any) {
    if (event.error) {
      this.cardErrors = event.error.message;
    } else {
      this.cardErrors = null;
    }
    switch(event.elementType) {
      case 'cardNumber':
        this.cardNumberComplete = event.complete;
        break;
      case 'cardExpiry':
        this.cardExpiryComplete = event.complete;
        break;
      case 'cardCvc':
        this.cardCvcComplete = event.complete;
        break;
    }
  }

  async submitOrder() {
    this.loading = true;
    const basket = this.basketService.getCurrentBasketValue();
    if (!basket) throw new Error('cannot get basket');
    try {
      const createdOrder = await this.createOrder(basket);
      const paymentResult = await this.confirmPaymentWithStripe(basket);
      if (paymentResult.paymentIntent) {
        this.basketService.deleteBasket(basket);
        const navigationExtras: NavigationExtras = {state: createdOrder};
        this.router.navigate(['checkout/success'], navigationExtras);
      } else {
        this.toastr.error(paymentResult.error.message);
      }
    } catch (error: any) {
      console.log(error);
      this.toastr.error(error.message)
    } finally {
      this.loading = false;
    }
  }

  private async confirmPaymentWithStripe(basket: Basket | null) {
    if (!basket) throw new Error('Basket is null');
    const result = this.stripe?.confirmCardPayment(basket.clientSecret!, {
      payment_method: {
        card: this.cardNumber!,
        billing_details: {
          name: this.checkoutForm?.get('paymentForm')?.get('nameOnCard')?.value
        }
      }
    });
    if (!result) throw new Error('Problem attempting payment with stripe');
    return result;
  }

  private async createOrder(basket: Basket | null) {
    if (!basket) throw new Error('Basket is null');
    const orderToCreate = this.getOrderToCreate(basket);
    return firstValueFrom(this.checkoutService.createOrder(orderToCreate));
  }

  private getOrderToCreate(basket: Basket): OrderToCreate {
    const deliveryMethodId = this.checkoutForm?.get('deliveryForm')?.get('deliveryMethod')?.value;
    const shipToAddress = this.checkoutForm?.get('addressForm')?.value as Address;
    if (!deliveryMethodId || !shipToAddress) throw new Error('Problem with basket');
    return {
      basketId: basket.id,
      deliveryMethodId: deliveryMethodId,
      shipToAddress: shipToAddress
    }
  }
}