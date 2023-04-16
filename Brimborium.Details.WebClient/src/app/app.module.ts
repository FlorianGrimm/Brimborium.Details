import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import {APP_BASE_HREF, PlatformLocation} from '@angular/common';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

export function getBaseHref(platformLocation: PlatformLocation): string {
  return platformLocation.getBaseHrefFromDOM();
}

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule
  ],
  //providers: [{provide: APP_BASE_HREF, useValue: '/app'}],
  providers: [
    {
      provide: APP_BASE_HREF,
      useFactory: getBaseHref,
      deps: [PlatformLocation]
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
