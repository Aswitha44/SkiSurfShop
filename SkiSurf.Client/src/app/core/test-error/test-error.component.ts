import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-test-error',
  templateUrl: './test-error.component.html',
  styleUrls: ['./test-error.component.scss']
})
export class TestErrorComponent {

   baseUrl=environment.apiUrl;
   validationErrors:string[]=[];
   constructor(private http:HttpClient){}

   get404Error(){
    this.http.get(this.baseUrl+'products/42').subscribe({
      next:response =>console.log(response),
      error: error=>console.log(error)
    })
   }

   get500Error(){
    this.http.get(this.baseUrl+'buggy/servererror').subscribe({
      next:response =>console.log(response),
      error: error=>console.log(error)
    })
   }

   get400Error(){
    this.http.get(this.baseUrl+'buggy/badrequest').subscribe({
      next:response =>console.log(response),
      error: error=>console.log(error)
    })
   }

   get400ValidError(){
    this.http.get(this.baseUrl+'products/xyz').subscribe({
      next:response =>console.log(response),
      error: error=>{console.log(error)
      this.validationErrors=error.errors;
      }
    })
   }
}
