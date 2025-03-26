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
    this.fullText = this.texts[this.activeSlide]; // Get text for the active slide
    this.displayedText = ''; // Clear text
    this.charIndex = 0; // Reset typing index
    this.typeText(); // Restart typing effect
  }

  typeText(): void {
    if (this.charIndex < this.fullText.length) {
      this.displayedText += this.fullText.charAt(this.charIndex);
      this.charIndex++;
      setTimeout(() => this.typeText(), 120); // Adjust typing speed here
    }
  }
}