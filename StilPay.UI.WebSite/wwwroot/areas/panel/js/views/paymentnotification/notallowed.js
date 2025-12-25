$(document).ready(function () {
    setTimeout(function () {
        window.parent.location.href = '/';
    }, 15000);
    history.pushState(null, document.title, location.href);
})