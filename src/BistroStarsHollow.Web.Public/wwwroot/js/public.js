// Bistro Stars Hollow - Public site scripts
document.addEventListener('DOMContentLoaded', function () {

    // Close mobile nav when clicking a link
    var navLinks = document.querySelectorAll('#mainNav .nav-link:not(.dropdown-toggle)');
    var navCollapse = document.getElementById('mainNav');

    navLinks.forEach(function (link) {
        link.addEventListener('click', function () {
            if (navCollapse.classList.contains('show')) {
                var bsCollapse = bootstrap.Collapse.getInstance(navCollapse);
                if (bsCollapse) {
                    bsCollapse.hide();
                }
            }
        });
    });

    // Close mobile nav when clicking dropdown items
    var dropdownItems = document.querySelectorAll('#mainNav .dropdown-item');
    dropdownItems.forEach(function (item) {
        item.addEventListener('click', function () {
            if (navCollapse.classList.contains('show')) {
                var bsCollapse = bootstrap.Collapse.getInstance(navCollapse);
                if (bsCollapse) {
                    bsCollapse.hide();
                }
            }
        });
    });

    // Header background change on scroll
    var header = document.querySelector('.site-header');
    if (header) {
        window.addEventListener('scroll', function () {
            if (window.scrollY > 50) {
                header.classList.add('scrolled');
            } else {
                header.classList.remove('scrolled');
            }
        });
    }

    // Smooth scroll for anchor links
    document.querySelectorAll('a[href*="#"]').forEach(function (anchor) {
        anchor.addEventListener('click', function (e) {
            var href = this.getAttribute('href');
            var hashIndex = href.indexOf('#');
            if (hashIndex === -1) return;

            var hash = href.substring(hashIndex);
            if (hash.length <= 1) return;

            var target = document.querySelector(hash);
            if (target) {
                // Only prevent default if we're on the same page
                var path = href.substring(0, hashIndex);
                if (!path || path === window.location.pathname) {
                    e.preventDefault();
                    target.scrollIntoView({ behavior: 'smooth' });
                }
            }
        });
    });
});
