var urlLIST = null;

$(document).ready(function () {
    $("#frmDef").validate({
        errorElement: 'span',
        errorClass: 'text-danger',
        rules: {
            "Password": {
                required: true,
                minlength: 4
            },
            "NewPassword": {
                required: true,
                minlength: 4
            },
            "ConfirmPassword": {
                required: true,
                equalTo: "#NewPassword"
            },
            "ConfirmCode": {
                required: true,
                digits: true,
                minlength: 6
            }
        },
        messages: {
            "Password": {
                required: "Lütfen şifre giriniz",
                minlength: "Uzunluk en az 4 haneli olmalıdır"
            },
            "NewPassword": {
                required: "Lütfen yeni şifre giriniz",
                minlength: "Uzunluk en az 4 haneli olmalıdır"
            },
            "ConfirmPassword": {
                required: "Lütfen yeni şifre tekrarı giriniz",
                equalTo: "Şifreler uyumlu değil"
            },
            "ConfirmCode": {
                required: "Kod giriniz",
                digits: "Kod sadece rakamlardan oluşması gerekir",
                minlength: "Kod 6 rakamdan oluşmalıdır"
            }
        }
    })

    $("#btnSendCode").click(function () {
        if ($("#frmDef").valid()) {
            $.post("/user/sendconfirmsms", { "operationType": "User_Security_ConfirmCode" }, function (response) {
                if (response.status === "OK") {
                    $("#divConfirm").addClass("d-none");
                    $("#divSubmit").removeClass("d-none");
                }
                else
                    alertify.success('Opps! Hata oluştu..');
            })
        }

    });
});

function onSuccessReset(response) {
    $("#divConfirm").removeClass("d-none");
    $('#frmDef').trigger("reset");
    $("#divSubmit").addClass("d-none");

    if (response.status === "ERROR")
        alertify.error(response.message);
    else
        alertify.success('İşlem Başarılı..');
}