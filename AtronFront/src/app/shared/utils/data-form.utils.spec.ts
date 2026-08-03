import { converterDataParaFormulario, formatarDataParaEnvio } from './data-form.utils';

describe('normalização de datas de formulário', () => {
  it('carrega data brasileira cujo dia é maior que doze', () => {
    const data = converterDataParaFormulario('31/01/2026');

    expect(data).not.toBeNull();
    expect(data?.getFullYear()).toBe(2026);
    expect(data?.getMonth()).toBe(0);
    expect(data?.getDate()).toBe(31);
  });

  it('não inverte dia e mês de uma data brasileira ambígua', () => {
    const data = converterDataParaFormulario('12/01/2026');

    expect(data).not.toBeNull();
    expect(data?.getMonth()).toBe(0);
    expect(data?.getDate()).toBe(12);
  });

  it('carrega a data ISO retornada pela API sem deslocar o dia', () => {
    const data = converterDataParaFormulario('2026-01-31T00:00:00');

    expect(data).not.toBeNull();
    expect(formatarDataParaEnvio(data)).toBe('2026-01-31');
  });
});
