import { Component, inject, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule, FormArray, FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { PerfilDeAcessoService } from '../../services/perfil-de-acesso.service';
import { PerfilDeAcessoFormComponent } from '../perfil-de-acesso-form/perfil-de-acesso-form.component';
import { ModuloService } from '../../../modulos/services/modulo.service';
import { SharedModule } from '../../../../shared/modules/shared.module';
import { ModuloModel } from '../../../modulos/interfaces/modulo.interface';

@Component({
  selector: 'c-perfil-de-acesso-edit',
  imports: [PerfilDeAcessoFormComponent, SharedModule, ReactiveFormsModule],
  templateUrl: './perfil-de-acesso-edit.component.html',
  styleUrls: ['../perfil-de-acesso.component.css']
})
export class PerfilDeAcessoEditComponent implements OnInit {
  codigo?: string;
  form!: FormGroup;

  constructor(private fb: FormBuilder, private service: PerfilDeAcessoService, private route: ActivatedRoute, public router: Router) { }

  moduloService = inject(ModuloService);

  ngOnInit(): void {
    this.criarFormulario();

    this.moduloService.obterTodos().subscribe(mods => {
      this.todosModulos = mods;
    });

    const codigo = this.route.snapshot.paramMap.get('codigo');
    if (codigo) {
      this.carregarPerfil(codigo);
      this.form.get('codigo')?.disable();
    }
  }

  carregarPerfil(codigo: string) {
    this.service.obterPorCodigo(codigo).subscribe(perfil => {
      this.form.patchValue({
        codigo: perfil.codigo,
        descricao: perfil.descricao,
        modulos: perfil.modulos?.map(m => m.codigo) || [],
      });
    });
  }

  private criarFormulario() {
    this.form = this.fb.group({
      codigo: ['', [Validators.required]],
      descricao: ['', [Validators.required]],
      modulos: [[]]
    });
  }

  todosModulos: ModuloModel[] = [];

  salvar() {
    const perfil = this.form.value;

    const codigosSelecionados: string[] = perfil.modulos;

    const modulosSelecionados = this.todosModulos.filter(mod =>
      codigosSelecionados.includes(mod.codigo)
    );

    const perfilPayload = {
      codigo: perfil.codigo,
      descricao: perfil.descricao,
      modulos: modulosSelecionados.map(mod => ({
        codigo: mod.codigo,
        descricao: mod.descricao
      }))
    };

    const codigoDaUrl = this.route.snapshot.paramMap.get('codigo');

    if (this.codigo !== codigoDaUrl) {
      this.codigo = codigoDaUrl;
    }

    if (codigoDaUrl !== perfilPayload.codigo && !perfilPayload.codigo) {
      perfilPayload.codigo = codigoDaUrl;
    }

    const operacao = this.codigo
      ? this.service.atualizar(this.codigo, perfilPayload)
      : this.service.gravar(perfilPayload);

    operacao.subscribe(() => this.router.navigate(['atron/perfil-de-acesso']));
  }
}