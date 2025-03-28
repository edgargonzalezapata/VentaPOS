// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Animación para las tarjetas de planes cuando la página está cargada
document.addEventListener('DOMContentLoaded', function() {
    // Verificar si estamos en la página de planes
    if (document.querySelector('.planes-container')) {
        const planCards = document.querySelectorAll('.plan-card');
        
        // Añadir clase para animación de entrada con un retraso escalonado
        planCards.forEach((card, index) => {
            setTimeout(() => {
                card.classList.add('animated');
            }, 200 * index);
        });
    }
});
