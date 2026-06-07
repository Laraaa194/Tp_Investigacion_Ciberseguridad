const f = new Date();
document.getElementById('fecha').textContent =
    f.toLocaleDateString('es-AR', { weekday: 'long', day: 'numeric', month: 'long', year: 'numeric' });
