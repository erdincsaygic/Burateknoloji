var urlRedirect = '/panel/paymentnotification/profile';

var hasSendSms = $("#jsonHasSendSms").val();
$(document).ready(function () {
    $("#frmValidate").validate({
        errorElement: 'span',
        errorClass: 'text-danger',
        rules: {
            "confirmcode": {
                required: true,
                digits: true,
                minlength: 6
            }
        },
        messages: {
            "confirmcode": {
                required: "Kod giriniz",
                digits: "Kod sadece rakamlardan oluşması gerekir",
                minlength: "Kod 6 rakamdan oluşmalıdır"
            },
        }
    });

    // Get saved timer and expired state
    var savedTime = parseInt(sessionStorage.getItem("validationTimer"));
    var isExpired = sessionStorage.getItem("isTimerExpired") === "true";

    if (hasSendSms == 'False') {
        time = 120;
        sessionStorage.setItem("isTimerExpired", "false");
    } else if (isExpired) {
        time = 0; 
    } else {
        time = savedTime || 120; 
    }

    if (time <= 0) {
        // Timer expired state
        $("#btnSendAgain").removeAttr("style");
        $("#idSecond").text("00");
        $("#idMinute").text("00");
    } else {
        // Start countdown
        setInterval(function () {
            if (time <= 0) {
                sessionStorage.setItem("isTimerExpired", "true");
                $("#btnSendAgain").removeAttr("style");
                $("#idSecond").text("00");
                $("#idMinute").text("00");
            } else {
                time--;
                sessionStorage.setItem("validationTimer", time);
                sessionStorage.setItem("isTimerExpired", "false");

                var s = time % 60;
                $("#idSecond").text(s.toString().padStart(2, '0'));

                var m = parseInt(time / 60);
                $("#idMinute").text(m.toString().padStart(2, '0'));
            }
        }, 1000);
    }
});

function onBegin() {
    $("#btnSubmit").attr("disabled", "disabled");
}

function onFailure() {
    Swal.fire({
        title: '<p class="fs-4 text-danger">Opps!</p>',
        confirmButtonText: "Tamam",
        showConfirmButton: true,
        confirmButtonColor: "#dc3545",
        allowOutsideClick: false,
        allowEscapeKey: false,
    });

    $("#btnSubmit").removeAttr("disabled");
}

function onSuccess(response) {
    if (response.status === "ERROR") {
        alertify.notify(response.message, 'error', 5, function () { }).dismissOthers();

        if (response.message == "Deneme hakkınız bitti. Yeni doğrulama kodu alınız!") {
            time = 0;
            sessionStorage.setItem("validationTimer", time);
            sessionStorage.setItem("isTimerExpired", "true");
            $("#idSecond").text("00");
            $("#idMinute").text("00");

            $("#btnSendAgain").removeAttr("style").off("click").on("click", function () {
                time = 120; // Restart timer when button is clicked
                sessionStorage.setItem("validationTimer", time);
                sessionStorage.setItem("isTimerExpired", "false");

                $(this).css("display", "none");
            });
        }
    } else {
        window.location.href = urlRedirect;
    }

    $("#btnSubmit").removeAttr("disabled");
}
