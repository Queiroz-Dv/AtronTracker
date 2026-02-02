import { Directive, ElementRef, HostListener } from '@angular/core';

/**
 * Diretiva que aplica máscara de data no formato brasileiro (dd/MM/yyyy).
 * Insere automaticamente as barras conforme o usuário digita.
 * 
 * Uso: <input appDateMask />
 */
@Directive({
  selector: '[appDateMask]',
  standalone: true
})
export class DateMaskDirective {
  
  constructor(private el: ElementRef<HTMLInputElement>) {}

  @HostListener('input', ['$event'])
  onInput(event: Event): void {
    const input = this.el.nativeElement;
    let value = input.value;
    
    // Remove tudo que não é dígito
    value = value.replace(/\D/g, '');
    
    // Limita a 8 dígitos (ddMMyyyy)
    value = value.substring(0, 8);
    
    // Aplica a máscara dd/MM/yyyy
    if (value.length > 4) {
      value = value.substring(0, 2) + '/' + value.substring(2, 4) + '/' + value.substring(4);
    } else if (value.length > 2) {
      value = value.substring(0, 2) + '/' + value.substring(2);
    }
    
    input.value = value;
    
    // Dispara evento de input para atualizar o ngModel/formControl
    input.dispatchEvent(new Event('input', { bubbles: true }));
  }

  @HostListener('keydown', ['$event'])
  onKeyDown(event: KeyboardEvent): void {
    // Permite teclas especiais: Backspace, Delete, Tab, Escape, Enter, setas
    const allowedKeys = ['Backspace', 'Delete', 'Tab', 'Escape', 'Enter', 
                         'ArrowLeft', 'ArrowRight', 'Home', 'End'];
    
    if (allowedKeys.includes(event.key)) {
      return;
    }

    // Permite Ctrl+A, Ctrl+C, Ctrl+V, Ctrl+X
    if (event.ctrlKey && ['a', 'c', 'v', 'x'].includes(event.key.toLowerCase())) {
      return;
    }

    // Bloqueia qualquer caractere que não seja dígito
    if (!/^\d$/.test(event.key)) {
      event.preventDefault();
    }
  }

  @HostListener('paste', ['$event'])
  onPaste(event: ClipboardEvent): void {
    event.preventDefault();
    const pastedText = event.clipboardData?.getData('text') || '';
    
    // Remove não-dígitos do texto colado
    const digitsOnly = pastedText.replace(/\D/g, '').substring(0, 8);
    
    // Aplica a máscara
    let formatted = digitsOnly;
    if (formatted.length > 4) {
      formatted = formatted.substring(0, 2) + '/' + formatted.substring(2, 4) + '/' + formatted.substring(4);
    } else if (formatted.length > 2) {
      formatted = formatted.substring(0, 2) + '/' + formatted.substring(2);
    }
    
    this.el.nativeElement.value = formatted;
    this.el.nativeElement.dispatchEvent(new Event('input', { bubbles: true }));
  }
}
