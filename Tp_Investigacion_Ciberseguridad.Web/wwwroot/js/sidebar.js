const toggle = document.getElementById('sidebarToggle');
const sidebar = document.getElementById('sidebar');
const shell = document.querySelector('.gs-app-shell');

const COLLAPSED_KEY = 'gs_sidebar_collapsed';

if (localStorage.getItem(COLLAPSED_KEY) === 'true') {
    sidebar.classList.add('collapsed');
    shell.classList.add('sidebar-collapsed');
}

toggle.addEventListener('click', () => {
    const isCollapsed = sidebar.classList.toggle('collapsed');
    shell.classList.toggle('sidebar-collapsed', isCollapsed);
    localStorage.setItem(COLLAPSED_KEY, isCollapsed);
});