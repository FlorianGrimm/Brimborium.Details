import { APP_BASE_HREF } from '@angular/common';
import { Component, Inject } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.sass']
})
export class AppComponent {
  title = 'Brimborium.Details.WebClient';

  constructor(@Inject(APP_BASE_HREF) public baseHref:string) {
    console.log("baseHref",baseHref);
  }
}
