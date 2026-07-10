document.addEventListener('DOMContentLoaded', function () {
    var notificacoes = document.querySelectorAll('.close-alert');
   
    for (var i = 0; i < notificacoes.length; i++) {
        notificacoes[i].addEventListener('click', function () {
            var alert = this.closest('.alert');
            alert.style.display = 'none';
        });
    }
});

document.addEventListener('DOMContentLoaded', function () {
    var validacaoPadronizada = document.querySelectorAll('.text-danger');
    for (var i = 0; i < validacaoPadronizada.length; i++) {
        validacaoPadronizada[i].addEventListener('click', function () {
            var span = this;
            span.style.display = 'none';
        });
    }
});