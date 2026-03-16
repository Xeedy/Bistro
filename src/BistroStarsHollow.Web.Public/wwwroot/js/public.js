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

    // Infinite logo carousel
    var track = document.getElementById('breweryTrack');
    if (track) {
        var started = false;

        function initCarousel() {
            if (started) return;
            started = true;

            var originalItems = Array.from(track.children);
            if (originalItems.length === 0) return;

            // Measure one full set width
            var setWidth = 0;
            originalItems.forEach(function (item) {
                setWidth += item.offsetWidth + parseFloat(getComputedStyle(item).marginRight || 0);
            });

            if (setWidth === 0) return;

            // Create enough copies to fill viewport + one extra set for seamless wrap
            var viewportWidth = track.parentElement.offsetWidth;
            var copiesNeeded = Math.ceil(viewportWidth / setWidth) + 2;
            var originalHTML = track.innerHTML;
            var html = '';
            for (var i = 0; i < copiesNeeded; i++) {
                html += originalHTML;
            }
            track.innerHTML = html;

            // Re-measure after duplication (in case of layout shift)
            var firstSetItems = Array.from(track.children).slice(0, originalItems.length);
            setWidth = 0;
            firstSetItems.forEach(function (item) {
                setWidth += item.offsetWidth + parseFloat(getComputedStyle(item).marginRight || 0);
            });

            var speed = 60; // pixels per second
            var offset = 0;
            var paused = false;
            var lastTime = null;

            track.addEventListener('mouseenter', function () { paused = true; });
            track.addEventListener('mouseleave', function () { paused = false; });

            function animate(timestamp) {
                if (lastTime === null) lastTime = timestamp;
                var dt = (timestamp - lastTime) / 1000;
                lastTime = timestamp;

                // Cap delta to avoid big jumps when tab is backgrounded
                if (dt > 0.1) dt = 0.016;

                if (!paused && setWidth > 0) {
                    offset += speed * dt;
                    if (offset >= setWidth) offset -= setWidth;
                    track.style.transform = 'translate3d(' + (-offset) + 'px, 0, 0)';
                }

                requestAnimationFrame(animate);
            }

            requestAnimationFrame(animate);
        }

        // Start carousel: try immediately, and also after images load as fallback
        // This ensures it works whether images are cached, slow, or missing
        var images = track.querySelectorAll('img');
        var remaining = images.length;

        function tryStart() {
            remaining--;
            if (remaining <= 0) initCarousel();
        }

        if (remaining === 0) {
            initCarousel();
        } else {
            images.forEach(function (img) {
                if (img.complete) {
                    tryStart();
                } else {
                    img.addEventListener('load', tryStart, { once: true });
                    img.addEventListener('error', tryStart, { once: true });
                }
            });
            // Safety net: start anyway after 3s in case events never fire
            setTimeout(initCarousel, 3000);
        }
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
