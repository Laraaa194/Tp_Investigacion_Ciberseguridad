(function () {
    'use strict';

    const inputBuscar = document.querySelector('.search');
    const btnBuscar = document.querySelector('.toolbar-right .btn.outline');
    const tbody = document.querySelector('.card table tbody');

    if (!inputBuscar || !tbody) return;

    function debounce(func, delay) {
        let timeoutId;
        return function (...args) {
            clearTimeout(timeoutId);
            timeoutId = setTimeout(() => func.apply(this, args), delay);
        };
    }

    async function buscarUsuarios(q) {
        const response = await fetch(`/Admin/BuscarUsuarios?q=${encodeURIComponent(q)}`);
        if (!response.ok) return;

        const usuarios = await response.json();
        actualizarTabla(usuarios);
    }

    const buscarUsuariosDebounced = debounce(buscarUsuarios, 400);

    function actualizarTabla(usuarios) {
        tbody.replaceChildren();

        if (!usuarios || usuarios.length === 0) {
            const trVacio = document.createElement('tr');
            const tdVacio = document.createElement('td');
            tdVacio.colSpan = 5;
            tdVacio.textContent = 'No se encontraron usuarios.';
            tdVacio.style.textAlign = 'center';
            tdVacio.style.padding = '20px';
            tdVacio.style.color = '#888';
            trVacio.appendChild(tdVacio);
            tbody.appendChild(trVacio);
            return;
        }

        for (const u of usuarios) {
            tbody.appendChild(crearFila(u));
        }
    }

    function obtenerAntiForgeryToken() {
        const tokenInput = document.querySelector('#antiforgery-holder input[name="__RequestVerificationToken"]');
        return tokenInput ? tokenInput.value : '';
    }

    function crearFila(u) {
        const tr = document.createElement('tr');

        const tdUsuario = document.createElement('td');
        const avatar = document.createElement('span');
        avatar.className = 'avatar';
        avatar.textContent = u.nombre.charAt(0).toUpperCase();
        tdUsuario.append(avatar, ` ${u.nombre} ${u.apellido}`);

        const tdEmail = document.createElement('td');
        tdEmail.textContent = u.email;

        const tdRol = document.createElement('td');
        const badge = document.createElement('span');
        badge.className = 'badge badge-admin';
        badge.textContent = u.rol;
        tdRol.appendChild(badge);

        const tdFecha = document.createElement('td');
        tdFecha.textContent = new Date(u.fechaRegistro).toLocaleDateString('es-AR');

        const tdAcciones = document.createElement('td');
        tdAcciones.className = 'acciones';

        // Link editar
        const linkEditar = document.createElement('a');
        linkEditar.href = `/Admin/EditarUsuario/${u.id}`;
        linkEditar.className = 'btn-icon edit';
        linkEditar.title = 'Editar';
        linkEditar.textContent = '✎';
        tdAcciones.appendChild(linkEditar);

        // Form eliminar (equivalente al que tenías en Razor)
        const form = document.createElement('form');
        form.action = '/Admin/EliminarUsuario';
        form.method = 'post';
        form.className = 'd-inline';

        const inputToken = document.createElement('input');
        inputToken.type = 'hidden';
        inputToken.name = '__RequestVerificationToken';
        inputToken.value = obtenerAntiForgeryToken();

        const inputId = document.createElement('input');
        inputId.type = 'hidden';
        inputId.name = 'id';
        inputId.value = u.id;

        const btnEliminar = document.createElement('button');
        btnEliminar.type = 'submit';
        btnEliminar.className = 'btn-icon delete';
        btnEliminar.title = 'Eliminar';
        btnEliminar.textContent = '✕';
        btnEliminar.addEventListener('click', function (e) {
            if (!confirm(`¿Seguro que querés eliminar a ${u.nombre} ${u.apellido}?`)) {
                e.preventDefault();
            }
        });

        form.append(inputToken, inputId, btnEliminar);
        tdAcciones.appendChild(form);

        tr.append(tdUsuario, tdEmail, tdRol, tdFecha, tdAcciones);
        return tr;
    }

    inputBuscar.addEventListener('input', function (e) {
        buscarUsuariosDebounced(e.target.value);
    });

    if (btnBuscar) {
        btnBuscar.addEventListener('click', function (e) {
            e.preventDefault();
            buscarUsuarios(inputBuscar.value);
        });
    }
})();