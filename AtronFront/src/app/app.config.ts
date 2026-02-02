import { ApplicationConfig, LOCALE_ID, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { provideNativeDateAdapter } from '@angular/material/core';
import { AuthInterceptor } from './core/interceptors/auth.interceptor';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async'; 
import { MAT_SNACK_BAR_DEFAULT_OPTIONS, MatSnackBarModule } from '@angular/material/snack-bar'; 
import { importProvidersFrom } from '@angular/core'; 

export const appConfig: ApplicationConfig = {
  providers: [provideZoneChangeDetection({ eventCoalescing: true }),
  provideRouter(routes),
  provideHttpClient(withInterceptorsFromDi()), { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
  provideNativeDateAdapter(),
  provideAnimationsAsync(),
  importProvidersFrom(MatSnackBarModule),
  { provide: LOCALE_ID, useValue: 'pt-BR' },
  {
    provide: MAT_SNACK_BAR_DEFAULT_OPTIONS,
    useValue: {
      duration: 3000, 
      verticalPosition: 'top', 
      horizontalPosition: 'center' 
    }
  }
  ],
};