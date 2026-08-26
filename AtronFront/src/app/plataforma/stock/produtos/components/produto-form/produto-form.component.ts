import { Component, Input } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { PageEvent } from '@angular/material/paginator';
import { ControlErrorComponent } from '../../../../../shared/components/control-error/control-error.component';
import { DateMaskDirective } from '../../../../../shared/directives/date-mask.directive';
import { SharedModule } from '../../../../../shared/modules/shared.module';
import { Categoria } from '../../../categorias/models/categoria.model';

@Component({
  selector: 'c-produto-form',
  standalone: true,
  imports: [SharedModule, ReactiveFormsModule, ControlErrorComponent, DateMaskDirective],
  templateUrl: './produto-form.component.html',
  styleUrl: './produto-form.component.css'
})
export class ProdutoFormComponent {
  @Input({ required: true }) form!: FormGroup;
  @Input() categoriasAtivas: Categoria[] = [];
  @Input() editando = false;
  @Input() statusDescricao = 'Ativo';
  @Input() dataEfetivaBaixa: string | null = null;

  filtroCategorias = '';
  paginaCategorias = 0;
  readonly tamanhoPaginaCategorias = 5;

  get gerarPorLote(): boolean {
    return !this.editando && !!this.form?.get('gerarPorLote')?.value;
  }

  get categoriasFiltradas(): Categoria[] {
    const filtro = this.normalizarTexto(this.filtroCategorias);
    if (!filtro) return this.categoriasAtivas;

    return this.categoriasAtivas.filter(categoria =>
      this.normalizarTexto(`${categoria.codigo} ${categoria.descricao}`).includes(filtro)
    );
  }

  get categoriasPaginadas(): Categoria[] {
    const inicio = this.paginaCategorias * this.tamanhoPaginaCategorias;
    return this.categoriasFiltradas.slice(inicio, inicio + this.tamanhoPaginaCategorias);
  }

  get totalCategoriasFiltradas(): number {
    return this.categoriasFiltradas.length;
  }

  filtrarCategorias(evento: Event): void {
    this.filtroCategorias = (evento.target as HTMLInputElement).value;
    this.paginaCategorias = 0;
  }

  alterarPaginaCategorias(evento: PageEvent): void {
    this.paginaCategorias = evento.pageIndex;
  }

  categoriaSelecionada(codigo: string): boolean {
    return this.codigosCategoriasSelecionadas.includes(codigo);
  }

  alternarCategoria(codigo: string, selecionada: boolean): void {
    const codigos = new Set(this.codigosCategoriasSelecionadas);

    if (selecionada) codigos.add(codigo);
    if (!selecionada) codigos.delete(codigo);

    this.form.get('categoriaCodigos')?.setValue([...codigos]);
    this.form.get('categoriaCodigos')?.markAsDirty();
  }

  private get codigosCategoriasSelecionadas(): string[] {
    return this.form?.get('categoriaCodigos')?.value ?? [];
  }

  private normalizarTexto(valor: string): string {
    return valor
      .normalize('NFD')
      .replace(/[\u0300-\u036f]/g, '')
      .trim()
      .toLocaleLowerCase('pt-BR');
  }
}
