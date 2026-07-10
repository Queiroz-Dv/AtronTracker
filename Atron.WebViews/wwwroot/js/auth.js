// Adicionei apenas um exemplo para extensibilidade futura, caso precise de eventos ou manipulações dinâmicas
document.addEventListener("DOMContentLoaded", () => {
    console.log("Login page loaded successfully.");

    // Exemplo: Adicionar comportamento ao focar em inputs
    const inputs = document.querySelectorAll(".form-control");
    inputs.forEach(input => {
        input.addEventListener("focus", () => {
            input.style.boxShadow = "0 0 8px rgba(114, 137, 218, 0.8)";
        });

        input.addEventListener("blur", () => {
            input.style.boxShadow = "";
        });
    });    
});