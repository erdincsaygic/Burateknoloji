var urlRedirect = '/panel/paymentnotification/finish';

$(document).ready(function () {
    $("#frmProfile").validate({
        errorElement: 'span',
        errorClass: 'text-danger',
        rules: {
            "IdentityNr": {
                required: true,
                digits: true,
                minlength: 11
            },
            "Name": {
                required: true
            },
            "Email": {
                required: true,
                email: true
            },
            "BirthYear": {
                required: true,
                digits: true,
                minlength: 4
            },
            "Confirm": {
                required: true
            }
        },
        messages: {
            "IdentityNr": {
                required: "Lütfen T.C. kimlik giriniz",
                digits: "Rakamlardan oluşmalıdır",
                minlength: "11 haneli olmalıdır"
            },
            "Name": {
                required: "Lütfen ad-soyad giriniz"
            },
            "Email": {
                required: "Lütfen email giriniz",
                email: "Geçerli bir email giriniz"
            },
            "BirthYear": {
                required: "Lütfen doğum yılı giriniz",
                digits: "Rakamlardan oluşmalıdır",
                minlength: "4 haneli olmalıdır"
            },
            "Confirm": {
                required: "Lütfen onaylayınız",
            }
        }
    })

    $("#chkConfirm").click(function () {
        if ($(this).prop("checked") == true) {
            $("#btnSubmit").addClass("hovers");
            $("#btnSubmit").removeAttr("disabled");

        }
        else if ($(this).prop("checked") == false) {
            $("#btnSubmit").removeClass("hovers");
            $("#btnSubmit").attr("disabled", "disabled");
        }
    })

    setTimeout(function () {
        window.parent.location.href = '/';
    }, 120000)
});

function onBegin() {
    Swal.fire({
        html: '<p class="fs-3 text-dark">Profiliniz oluşturuluyor..</p>',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    })
}

function onFailure() {
    setTimeout(function () {
        Swal.fire({
            title: '<p class="fs-4 text-danger">Opps!</p>',
            confirmButtonText: "Tamam",
            showConfirmButton: true,
            confirmButtonColor: "#dc3545",
            allowOutsideClick: false,
            allowEscapeKey: false,
        })
    }, 1000)
}

function onSuccess(response) {
    setTimeout(function () {
        if (response.status === "ERROR")
            alertify.notify(response.message, 'error', 5, function () { }).dismissOthers();
        else {
            window.location.href = urlRedirect;
        }
    }, 1000)
}
