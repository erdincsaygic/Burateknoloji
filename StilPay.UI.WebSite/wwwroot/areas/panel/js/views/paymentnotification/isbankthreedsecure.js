const redirectUrl = $("#jsonRedirectUrl").val();
const transferStatus = $("#jsonTransferStatus").val();

$(document).ready(function () {
    setTimeout(function () {
        if (redirectUrl === undefined || redirectUrl === null || redirectUrl === "") {
            window.parent.location.href = '/';
        }
        else {
            window.parent.location.href = redirectUrl;
        }
    }, 300000)

    if (transferStatus === "true") {
        setTimeout(function () {
            if (redirectUrl === undefined || redirectUrl === null || redirectUrl === "") {
                window.parent.location.href = '/';
            }
            else {
                window.parent.location.href = redirectUrl;
            }
        }, 15000)
    }

    if (transferStatus === "false") {
        requestForStatus(1);
    }

    $("#btnGoSite").click(function () {
        if (redirectUrl === undefined || redirectUrl === null || redirectUrl === "") {
            window.parent.location.href = '/';
        }
        else {
            window.parent.location.href = redirectUrl;
        }
    })
})

function requestForStatus(i) {
    setTimeout(function () {
        $.ajax({
            type: "GET",
            url: '/Panel/PaymentNotification/StatusControlCreditCard/',
            contentType: 'application/json; charset=utf-8',
            beforeSend: function () {
            },
            success: function (response) {
                if (response.status == "OK" && response.data) {
                    $("#status").removeClass("onaystay")
                    $("#status").addClass("onay")
                    $("#status").text("ONAYLANDI");
                    $("#spinner").css("display", "none");
                    setTimeout(function () {
                        if (redirectUrl === undefined || redirectUrl === null || redirectUrl === "") {
                            window.parent.location.href = '/';
                        }
                        else {
                            window.parent.location.href = redirectUrl;
                        }
                    }, 15000)
                }
                if (response.status == "OK" && !response.data) {
                    requestForStatus(i + 1);
                }
                if (response.status == "ERROR" && (response.message != null && response.message != "")) {
                    $("#transactionStatusHeaderDiv").css("background-color", "red");
                    $("#transactionStatusHeader").text("İşlem Başarısız");
                    $("#status").removeClass("onaystay");
                    $("#status").css("color", "red");
                    $("#status").text("İPTAL EDİLDİ");
                    $("#spinner").css("display", "none");
                    $("#responseText").removeClass("onay");
                    $("#responseText").css("color", "red");
                    $("#responseText").text(`${response.message}`); 
                    setTimeout(function () {
                        if (redirectUrl === undefined || redirectUrl === null || redirectUrl === "") {
                            window.parent.location.href = '/';
                        }
                        else {
                            window.parent.location.href = redirectUrl;
                        }
                    }, 15000)
                }
            },
            error: function () {
            },
            complete: function () {
            }
        });
    }, 10000)
}