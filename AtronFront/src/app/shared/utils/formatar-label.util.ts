export function formatLabel(codigo?: string, descricao?: string): string {
  return codigo && descricao ? `${codigo} - ${descricao}` : '';
}