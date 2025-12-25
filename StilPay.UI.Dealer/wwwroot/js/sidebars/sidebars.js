$(document).ready(function () {

    setTimeout(function () {
        $("#contentbody").addClass("w-100");
    }, 50)

    $('#sidebarCollapse').on('click', function () {
        $('#sidebar').toggleClass('active');
    })

    document.querySelectorAll('#collapsibleNavbar>ul.ul-nav>li.nav-item').forEach(function (everyitem) {

        everyitem.addEventListener('mouseover', function (e) {
            let el_link = this.querySelector('a[data-bs-toggle]');

            if (el_link !== null) {
                let nextEl = el_link.nextElementSibling;
                el_link.classList.add('show');
                nextEl.classList.add('show');
            }
        })

        everyitem.addEventListener('mouseleave', function (e) {
            let el_link = this.querySelector('a[data-bs-toggle]');

            if (el_link !== null) {
                let nextEl = el_link.nextElementSibling;
                el_link.classList.remove('show');
                nextEl.classList.remove('show');
            }
        })

    })

})