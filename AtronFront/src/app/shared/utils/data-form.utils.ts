export function converterDataParaFormulario(data?: Date | string | null): Date | null {
  if (!data) return null;

  if (data instanceof Date) {
    return Number.isNaN(data.getTime()) ? null : new Date(data.getTime());
  }

  const valor = data.toString().trim();
  const formatoBrasileiro = /^(\d{2})\/(\d{2})\/(\d{4})$/.exec(valor);
  const formatoIso = /^(\d{4})-(\d{2})-(\d{2})/.exec(valor);
  const partes = formatoBrasileiro
    ? {
        ano: Number(formatoBrasileiro[3]),
        mes: Number(formatoBrasileiro[2]),
        dia: Number(formatoBrasileiro[1])
      }
    : formatoIso
      ? {
          ano: Number(formatoIso[1]),
          mes: Number(formatoIso[2]),
          dia: Number(formatoIso[3])
        }
      : null;

  if (!partes) return null;

  const resultado = new Date(0);
  resultado.setHours(0, 0, 0, 0);
  resultado.setFullYear(partes.ano, partes.mes - 1, partes.dia);

  const dataValida =
    resultado.getFullYear() === partes.ano &&
    resultado.getMonth() === partes.mes - 1 &&
    resultado.getDate() === partes.dia;

  return dataValida ? resultado : null;
}

export function formatarDataParaEnvio(data?: Date | string | null): string | null {
  if (!data) return null;

  if (data instanceof Date) {
    if (Number.isNaN(data.getTime())) return null;

    const ano = data.getFullYear();
    const mes = (data.getMonth() + 1).toString().padStart(2, '0');
    const dia = data.getDate().toString().padStart(2, '0');
    return `${ano}-${mes}-${dia}`;
  }

  const valor = data.toString().trim();
  if (/^\d{4}-\d{2}-\d{2}$/.test(valor)) return valor;

  const partes = /^(\d{2})\/(\d{2})\/(\d{4})$/.exec(valor);
  if (!partes) return valor;

  const [, dia, mes, ano] = partes;
  return `${ano}-${mes}-${dia}`;
}
