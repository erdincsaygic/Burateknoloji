var urlRedirect = 'https://bayi.stilpay.com/'; 
//var urlRedirect = 'http://localhost:26653/'; 

var interval = null;

$(document).ready(function () {
    $("#frmLogin").validate({
        errorElement: 'div',
        errorClass: 'text-danger',
        rules: {
            "phone": {
                required: true,
                digits: true,
                minlength: 11
            },
            "password": {
                required: true,
                minlength: 4
            }
            //,
            //"captchacode": {
            //    required: true,
            //    minlength: 4
            //}
        },
        messages: {
            "phone": {
                required: "Telefon giriniz",
                digits: "Sadece rakam giriniz",
                minlength: "11 haneli giriniz"
            },
            "password": {
                required: "Şifre giriniz",
                minlength: "Şifre 4 haneden kısa olamaz"
            }
            //,
            //"captchacode": {
            //    required: "Kod giriniz",
            //    minlength: "4 haneli giriniz"
            //}
        }
    })


    sessionStorage.clear();

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
                digits: "Sadece rakam giriniz",
                minlength: "6 haneli giriniz"
            },
        }
    })

    //$("#img-captcha").click(function () {
    //    resetCaptcha();
    //})

    //interval = setInterval(resetCaptchaImage, 30000)
});

//function resetCaptchaInterval() {
//    clearInterval(interval);
//    interval = setInterval(resetCaptchaImage, 30000);
//}

//function resetCaptchaImage() {
//    d = new Date();
//    $("#img-captcha").attr("src", "/get-captcha-image?" + d.getTime());
//}

//function resetCaptcha() {
//    resetCaptchaImage();
//    resetCaptchaInterval();
//}

function OnBeginLogin() {
    $("#btnSubmitLogin").attr("disabled", "disabled");

    $("#toastBody").html('<div class="spinner-border text-primary" role="status"></div>');
    $("#toast").toast({ animation: true, autohide: true, delay: 1000000 });
    $("#toast").toast("show");
}


function OnFailureLogin() {
    setTimeout(() => {
        $("#btnSubmitLogin").removeAttr("disabled");

        $("#toastBody").html('<p class="text-danger" role="status">Opps !</p>');
    }, 500);
}

function OnSuccessLogin(response) {
    setTimeout(() => {
        if (response.status === "ERROR") {
            $("#btnSubmitLogin").removeAttr("disabled");
            $("#toastBody").html('<p class="text-danger" role="status">' + response.message + '</p>');
            resetCaptcha();
        }
        else {
            $("#toast").toast("hide");
            $("#frmLogin").addClass("d-none");
            $("#frmValidate").removeClass("d-none");

            startTimerLogin();
        }
    }, 500);
}

function startTimerLogin() {
    setInterval(() => {
        var _time = $("#idTime").text().trim();
        _time = parseInt(_time);

        if (_time <= 0)
            window.parent.location.href = '/';
        else {
            _time = _time - 1;
            var s = _time % 60;
            $("#idSecond").text(s.toString().padStart(2, '0'));
            var m = parseInt(_time / 60);
            $("#idMinute").text(m.toString().padStart(2, '0'));
        }

        $("#idTime").text(_time);
    }, 1000)
}

function OnBeginValidate() {
    $("#btnSubmitValidate").attr("disabled", "disabled");

    $("#toastBody").html('<div class="spinner-border text-primary" role="status"></div>');
    $("#toast").toast({ animation: true, autohide: true, delay: 1000000 });
    $("#toast").toast("show");
}

function OnFailureValidate() {
    setTimeout(() => {
        $("#btnSubmitValidate").removeAttr("disabled");

        $("#toastBody").html('<p class="text-danger" role="status">Opps !</p>');
    }, 500);
}

function OnSuccessValidate(response) {
    setTimeout(() => {
        if (response.status === "ERROR") {
            $("#btnSubmitValidate").removeAttr("disabled");

            $("#toastBody").html('<p class="text-danger"  role="status">' + response.message + '</p>');
        }
        else {
            $("#toast").toast("hide");
            window.parent.location.href = urlRedirect;
        }
    }, 500);
}