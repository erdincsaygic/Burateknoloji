var urlRedirect = '/panel/paymentnotification/validation';

$(document).ready(function () {
    setTimeout(function () {
        window.parent.location.href = '/';
    }, 120000)
})

function onBegin() {
    $("#btnSubmit").removeClass("hovers");
    $("#btnSubmit").attr("disabled", "disabled");
}

function onFailure() {
    setTimeout(function () {
        Swal.fire({
            title: '<p class="fs-4 text-danger">Opps!</p>',
            showConfirmButton: true,
            confirmButtonText: "Tamam",
            confirmButtonColor: "#dc3545",
            allowOutsideClick: false,
            allowEscapeKey: false,
        })
    }, 1000)

    $("#btnSubmit").addClass("hovers");
    $("#btnSubmit").removeAttr("disabled");
}

function onSuccess(response) {
    setTimeout(function () {
        if (response.status === "ERROR")
            window.location.reload();
        else
            window.location.href = urlRedirect;
    }, 1000)
}