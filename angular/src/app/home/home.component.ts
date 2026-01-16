import { Component, inject } from '@angular/core';
import { AuthService} from '@abp/ng.core';
import { SHARED_IMPORTS } from '../shared/shared-imports.constants';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
  imports: [...SHARED_IMPORTS]
})
export class HomeComponent {

  images = [
      "assets/images/slider/slider1.jpg",
      "assets/images/slider/slider2.jpg",
      "assets/images/slider/slider3.jpg",
      "assets/images/slider/slider4.jpg",
      "assets/images/slider/slider5.avif"
    ];
  effect = 'scrollx';

  private authService = inject(AuthService);

  get hasLoggedIn(): boolean {
    return this.authService.isAuthenticated
  }

  login() {
    this.authService.navigateToLogin();
  }
}
