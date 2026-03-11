// Sidebar toggle for mobile
document.addEventListener('DOMContentLoaded', function () {
    const sidebar = document.getElementById('sidebar');
    const sidebarToggle = document.getElementById('sidebarToggle');
    const sidebarClose = document.getElementById('sidebarClose');
    const sidebarOverlay = document.getElementById('sidebarOverlay');

    function openSidebar() {
        sidebar?.classList.add('show');
        sidebarOverlay?.classList.add('show');
    }

    function closeSidebar() {
        sidebar?.classList.remove('show');
        sidebarOverlay?.classList.remove('show');
    }

    sidebarToggle?.addEventListener('click', openSidebar);
    sidebarClose?.addEventListener('click', closeSidebar);
    sidebarOverlay?.addEventListener('click', closeSidebar);
});
