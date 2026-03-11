export class FormErrorMessages {
  // Mapeia o tipo do erro (ex: 'required', 'minlength') para uma função que gera a mensagem
  private static messages = new Map<string, (error: any) => string>([
    ['required', () => 'Campo obrigatório.'],
    ['minlength', (error) => `Mínimo de ${error.requiredLength} caracteres.`],
    ['maxlength', (error) => `Máximo de ${error.requiredLength} caracteres.`],
    ['pattern', () => 'Formato inválido.'],
  ]);

  /**
 * Retorna a mensagem com base no tipo do erro.
 * @param errorKey Nome do erro (ex: 'required', 'minlength', etc)
 * @param errorValue Objeto com os dados do erro (ex: { requiredLength: 3, actualLength: 1 })
 */
  static getMessage(errorKey: string, errorValue: any): string {
    return this.messages.get(errorKey)?.(errorValue) || 'Campo inválido.';
  }
}