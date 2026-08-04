import { Component, OnInit } from '@angular/core';
import { UsuarioFormComponent } from '../usuario-form/usuario-form.component';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { UsuarioService } from '../../services/usuario.service';
import { ActivatedRoute, Router } from '@angular/router';
import { SharedModule } from '../../../../../shared/modules/shared.module';
import { UsuarioRequest } from '../../models/request/usuario-request';
import { converterDataParaFormulario, formatarDataParaEnvio } from '../../../../../shared/utils/data-form.utils';

@Component({
  selector: 'c-usuario-edit',
  imports: [ReactiveFormsModule, SharedModule, UsuarioFormComponent],
  templateUrl: './usuario-edit.component.html',
})
export class UsuarioEditComponent implements OnInit {
  form!: FormGroup;
  codigo?: string | number;

  constructor(
    private fb: FormBuilder,
    private service: UsuarioService,
    private route: ActivatedRoute,
    public router: Router,
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      codigo: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(10)]],
      nome: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(25)]],
      sobrenome: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
      dataNascimento: [null],
      email: ['', Validators.required],
      cargoCodigo: [null, Validators.required],
      departamentoCodigo: ['', Validators.required],
      gestorImediatoCodigo: [null]
    });

    this.codigo = this.route.snapshot.paramMap.get('codigo');
    if (this.codigo) {
      this.form.get('codigo')?.disable();
      this.service.obterPorCodigo(this.codigo).subscribe(usr => this.form.patchValue({
        ...usr,
        dataNascimento: converterDataParaFormulario(usr.dataNascimento)
      }));
    }
  }

  salvar() {
    const dadosForm = this.form.getRawValue();
    const usuarioPayload = new UsuarioRequest(
      dadosForm.codigo,
      dadosForm.nome,
      dadosForm.sobrenome,
      dadosForm.email,
      dadosForm.cargoCodigo,
      dadosForm.departamentoCodigo,
      formatarDataParaEnvio(dadosForm.dataNascimento),
      dadosForm.gestorImediatoCodigo);

    const operacao = this.codigo
      ? this.service.atualizar(this.codigo, usuarioPayload)
      : this.service.gravar(usuarioPayload);

    operacao.subscribe(() => this.router.navigate(['atron/usuarios']));
  }
}
