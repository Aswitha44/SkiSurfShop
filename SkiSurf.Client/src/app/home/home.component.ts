import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent {
  texts = [
    'Welcome!',
    'Ski Gear Live!',
    'Surfing? Coming Soon!!' // Different text for the third slide
  ];
 constructor(private router: Router) { }
  displayedText = '';
  fullText = this.texts[0]; // Default text for the first slide
  charIndex = 0;
  activeSlide = 0; // Track the active slide

  ngOnInit(): void {
    this.typeText();
    
  }

  onGoToShop(): void {
    this.router.navigateByUrl('/shop');
  }

  onSlideChange(): void {
    this.fullText = this.texts[this.activeSlide]; 
    this.displayedText = '';  
    this.charIndex = 0;  
    setTimeout(() => this.typeText(), 300); 
  }

  typeText(): void {
    if (this.charIndex < this.fullText.length) {
      this.displayedText += this.fullText.charAt(this.charIndex);
      this.charIndex++;
      setTimeout(() => this.typeText(), 120); // Adjust typing speed here
    }
  }
}