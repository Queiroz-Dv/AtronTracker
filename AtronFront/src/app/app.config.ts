import { ApplicationConfig, LOCALE_ID, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { provideNativeDateAdapter } from '@angular/material/core';
import { AuthInterceptor } from './core/interceptors/auth.interceptor';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async'; 
import { MAT_SNACK_BAR_DEFAULT_OPTIONS, MatSnackBarModule } from '@angular/material/snack-bar'; 
import { importProvidersFrom } from '@angular/core'; 
import { MatPaginatorIntl } from '@angular/material/paginator';

function criarPaginadorPtBr(): MatPaginatorIntl {
  const paginador = new MatPaginatorIntl();

  paginador.itemsPerPageLabel = 'Itens por página';
  paginador.nextPageLabel = 'Próxima página';
  paginador.previousPageLabel = 'Página anterior';
  paginador.firstPageLabel = 'Primeira página';
  paginador.lastPageLabel = 'Última página';
  paginador.getRangeLabel = (page: number, pageSize: number, length: number): string => {
    if (length === 0 || pageSize === 0) {
      return `0 de ${length}`;
    }

    const inicio = page * pageSize;
    const fim = Math.min(inicio + pageSize, length);
    return `${inicio + 1} - ${fim} de ${length}`;
  };

  return paginador;
}

export const appConfig: ApplicationConfig = {
  providers: [provideZoneChangeDetection({ eventCoalescing: true }),
  provideRouter(routes),
  provideHttpClient(withInterceptorsFromDi()), { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
  provideNativeDateAdapter(),
  provideAnimationsAsync(),
  importProvidersFrom(MatSnackBarModule),
  { provide: LOCALE_ID, useValue: 'pt-BR' },
  { provide: MatPaginatorIntl, useFactory: criarPaginadorPtBr },
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
