// Funciones comunes para la aplicación CiberAgentes

// Esperar a que el DOM esté cargado
document.addEventListener('DOMContentLoaded', function () {
    // Inicializar tooltips de Bootstrap
    initTooltips();

    // Configurar comportamiento para mostrar/ocultar contraseñas
    setupPasswordToggles();

    // Configurar botones de copia al portapapeles
    setupCopyButtons();

    // Animar el contador de puntos en el header (si existe)
    animateScoreCounter();

    // Inicializar alertas con auto-cierre
    setupAutoCloseAlerts();
});

// Inicializa los tooltips de Bootstrap
function initTooltips() {
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
}

// Configura los botones para mostrar/ocultar contraseñas
function setupPasswordToggles() {
    const toggleButtons = document.querySelectorAll('.toggle-password');

    toggleButtons.forEach(button => {
        button.addEventListener('click', function () {
            const input = this.closest('.input-group')?.querySelector('input') ||
                this.parentElement.querySelector('input');

            if (!input) return;

            const icon = this.querySelector('i');

            if (input.type === 'password') {
                input.type = 'text';
                icon.classList.remove('fa-eye');
                icon.classList.add('fa-eye-slash');
            } else {
                input.type = 'password';
                icon.classList.remove('fa-eye-slash');
                icon.classList.add('fa-eye');
            }
        });
    });
}

// Configura los botones para copiar al portapapeles
function setupCopyButtons() {
    const copyButtons = document.querySelectorAll('.copy-text, .copy-password');

    copyButtons.forEach(button => {
        button.addEventListener('click', function () {
            const text = this.dataset.text ||
                this.closest('.input-group')?.querySelector('input')?.value ||
                this.parentElement.querySelector('input, code')?.textContent;

            if (!text) return;

            // Crear un elemento de texto temporal
            const tempInput = document.createElement('input');
            tempInput.style.position = 'absolute';
            tempInput.style.left = '-9999px';
            tempInput.value = text;
            document.body.appendChild(tempInput);

            // Seleccionar y copiar el texto
            tempInput.select();
            document.execCommand('copy');
            document.body.removeChild(tempInput);

            // Mostrar mensaje de éxito
            const originalTitle = this.title || 'Copiar';
            this.title = '¡Copiado!';

            // Si existe el Bootstrap tooltip, mostrarlo
            if (bootstrap && bootstrap.Tooltip) {
                const tooltip = bootstrap.Tooltip.getInstance(this) ||
                    new bootstrap.Tooltip(this, { trigger: 'manual' });
                tooltip.show();

                // Restaurar el título y ocultar el tooltip después de un tiempo
                setTimeout(() => {
                    this.title = originalTitle;
                    tooltip.hide();
                }, 1500);
            }
        });
    });
}

// Anima el contador de puntos en el header
function animateScoreCounter() {
    const scoreElement = document.querySelector('.badge-score');

    if (!scoreElement) return;

    const finalScore = parseInt(scoreElement.textContent);
    if (isNaN(finalScore)) return;

    let currentScore = 0;
    const duration = 1000; // Duración de la animación en ms
    const stepTime = 50; // Tiempo entre cada paso en ms
    const steps = duration / stepTime;
    const increment = Math.ceil(finalScore / steps);

    scoreElement.textContent = '0 pts';

    const interval = setInterval(() => {
        currentScore += increment;

        if (currentScore >= finalScore) {
            clearInterval(interval);
            currentScore = finalScore;
        }

        scoreElement.textContent = `${currentScore} pts`;
    }, stepTime);
}

// Configurar alertas para que se cierren automáticamente después de un tiempo
function setupAutoCloseAlerts() {
    const alerts = document.querySelectorAll('.alert:not(.alert-permanent)');

    alerts.forEach(alert => {
        setTimeout(() => {
            const closeButton = alert.querySelector('.btn-close');
            if (closeButton) {
                closeButton.click();
            } else {
                // Animación de fade out si no hay botón de cierre
                alert.style.transition = 'opacity 0.5s';
                alert.style.opacity = '0';

                setTimeout(() => {
                    alert.remove();
                }, 500);
            }
        }, 5000); // 5 segundos
    });
}

// Función para evaluar la fortaleza de contraseñas en el cliente
function evaluatePasswordStrength(password) {
    if (!password) return 0;

    let score = 0;

    // Longitud: máximo 25 puntos
    score += Math.min(password.length * 2, 25);

    // Variedad de caracteres: máximo 35 puntos
    if (/[A-Z]/.test(password)) score += 5; // Mayúsculas
    if (/[a-z]/.test(password)) score += 5; // Minúsculas
    if (/[0-9]/.test(password)) score += 5; // Números
    if (/[^a-zA-Z0-9]/.test(password)) score += 10; // Caracteres especiales

    // Combinaciones adicionales: máximo 25 puntos
    if (/[A-Z].*[0-9]|[0-9].*[A-Z]/.test(password)) score += 5; // Mayúscula y número
    if (/[a-z].*[0-9]|[0-9].*[a-z]/.test(password)) score += 5; // Minúscula y número
    if (/[a-z].*[A-Z]|[A-Z].*[a-z]/.test(password)) score += 5; // Mayúscula y minúscula
    if (/[a-zA-Z].*[^a-zA-Z0-9]|[^a-zA-Z0-9].*[a-zA-Z]/.test(password)) score += 10; // Letra y especial

    // Penalizaciones: hasta -15 puntos
    if (password.length < 8) score -= (8 - password.length) * 2; // Penalización por longitud corta
    if (/([a-zA-Z0-9])\1{2,}/.test(password)) score -= 5; // Penalización por repeticiones

    // Asegurar que el score está entre 0 y 100
    return Math.max(0, Math.min(100, score));
}

// Función para generar contraseñas aleatorias en el cliente
function generateRandomPassword(length = 12, includeLowercase = true, includeUppercase = true, includeNumbers = true, includeSpecial = true) {
    // Definir conjuntos de caracteres
    const lowerChars = "abcdefghijklmnopqrstuvwxyz";
    const upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    const numberChars = "0123456789";
    const specialChars = "!@#$%^&*()-_=+[]{}|;:,.<>?";

    // Crear el conjunto total de caracteres
    let charset = "";
    if (includeLowercase) charset += lowerChars;
    if (includeUppercase) charset += upperChars;
    if (includeNumbers) charset += numberChars;
    if (includeSpecial) charset += specialChars;

    // Asegurar que al menos un tipo está habilitado
    if (!charset) {
        charset = lowerChars;
    }

    // Generar la contraseña aleatoria
    let password = "";

    // Asegurar que al menos un carácter de cada tipo requerido esté presente
    if (includeLowercase) {
        password += lowerChars.charAt(Math.floor(Math.random() * lowerChars.length));
    }
    if (includeUppercase) {
        password += upperChars.charAt(Math.floor(Math.random() * upperChars.length));
    }
    if (includeNumbers) {
        password += numberChars.charAt(Math.floor(Math.random() * numberChars.length));
    }
    if (includeSpecial) {
        password += specialChars.charAt(Math.floor(Math.random() * specialChars.length));
    }

    // Completar el resto de la contraseña
    while (password.length < length) {
        const randomIndex = Math.floor(Math.random() * charset.length);
        password += charset.charAt(randomIndex);
    }

    // Mezclar los caracteres para evitar patrones predecibles
    return password.split('').sort(() => 0.5 - Math.random()).join('');
}