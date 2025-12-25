var intervalBadge = null;
var intervalExitDate = null;
var intervalCookieTimeout = null;
var snd = new Audio("data:audio/mpeg;base64,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wZ0G7jqlt/OcDIKX+/1NblSX+VKfQfP8xuJJGk7////rf////+PgXTv///1JThJJQainmySAB6imUyuVbVttUo7T4Csa821OuF88f62+CZHFnGf///mQgYIEO0SMF2NVy9NxYTdlqJ8AuS4zr//SJoTUJ+CaKKTcZvosrUPo8W/MUv0f033E9E/QpN6P///v/////WRR2mwUAYUABjabRu1vrOLKAF0kIdHjnEx/iNWo7jGn1////mApxNTJQQOU1Het/NoUFTMQs6Vja///THaGIl/0fojl8mjd/Jo8W+ZfpNpCajsz7////6kn/////WRRgDz//LD1KSTDjKOciSAKxdLx5S31uYqKIWj/+5JECgAC8V5M6g9rdFyr6Vo9rW6KtHcr5DEJQRkSpLRklSigvVc4QpmyPe9H3zHR1/in9P/8VNCMJOzYUDyVjfwHP0ZgiZt/3/+9EBnDKbegdUrckhgntHaQ9vX/X/9A/////+r/////mJ3/9ItRcoVRogAcmV9N8z0pvES8QQsKoMGXEymPQyWm6E4HQLqgpv/CZJAtYXQSwoF8e6SB56zABEoW+qgZjJAZovGr0Gl5/OjFKL3JwnaX9v7/X8y1f/////////49WAzMzEYYMZLq6CUANIqbDX7lisBIdraAEPwShTRc9WZ2vAqBc4NQ9GrUNaw0Czcrte0g1NEoiU8NFjx4NFh54FSwlOlgaCp0S3hqo8SLOh3/63f7P/KgKJxxhgGSnAFMCnIogwU5JoqBIDAuBIiNLETyFmiImtYiDTSlb8ziIFYSFv/QPC38zyxEOuPeVGHQ77r/1u/+kq49//6g4gjoVQSUMYQUSAP8PwRcZIyh2kCI2OwkZICZmaZxgnsNY8DmSCWX0idhtz3VTJSqErTSB//1X7TTTVVV//uSZB2P8xwRJ4HvYcItQlWBACM4AAABpAAAACAAADSAAAAEVf/+qCE000VVVVU0002//+qqqqummmmr///qqqppppoqqqqppppoqqATkEjIyIxBlBA5KwUEDBBwkFhYWFhUVFfiqhYWFhcVFRUVFv/Ff/xUVFRYWFpMQU1FMy45OS41qqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqg==");

$(document).ready(function () {
    $("input[type=text]").attr("autocomplete", "off");

    getNotifyCounts();

    //intervalExitDate = setInterval(function () {
    //    refreshExitDate();
    //}, 30000)

    //intervalCookieTimeout = setInterval(intervalCookieTimeoutFunction, 1000);

    intervalBadge = setInterval(function () {
        getNotifyCounts();
    }, 10000)

});

function showLoading() {
    Swal.fire({
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
}

function hideLoading() {
    setTimeout(function () {
        Swal.close();
    }, 500);
}

function initBtnOperations(id, showEdit, showDrop) {
    var btnOperations = '';
    btnOperations += showEdit ? '<a href="javascript:drop(\'' + id + '\')" class="btn btn-danger btn-sm"><i class="fa fa-trash-can"></i></a>&nbsp;' : '';
    btnOperations += showDrop ? '<a href="javascript:edit(\'' + id + '\')" class="btn btn-dark btn-sm"><i class="fa fa-pencil"></i></a>' : '';

    return btnOperations;
}

function generateRandomPassword(id) {
    $.ajax({
        async: false,
        type: "GET",
        url: '/Dealer/GenerateRandomPassword',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({}),
        success: function (res) {   
            $(`#${id}`).val(res);
        }
    })
}

function edit(id) {
    window.location.href = urlEDIT.replace('__id__', id);
}

function showError(message) {
    alertify.notify(message, 'error', 5, function () { }).dismissOthers();

}
function drop(id) {
    alertify.dialog('confirm').set({ transition: 'slide' });
    alertify.confirm('Closable: false').set('closable', false).set('labels', { ok: 'EVET', cancel: 'HAYIR' });
    alertify.confirm(
        'KAYIT SİL',
        '<p class="text-danger fs-6 fw-bold">Emin misiniz ?</p>',
        function () {
            showLoading();
            $.post(urlDELETE, { "id": id }, function (response) {
                if (response.status === "ERROR")
                    showError(response.message);
                else {
                    var oTable = $('#Table').DataTable();
                    oTable.rows($('#tr_' + id)).remove().draw();
                    alertify.success('İşlem Başarılı..');
                }

            }).fail(function () {
                onFailure();
            }).always(function () {
                hideLoading();
            });
        },
        function () {
        }
    );
}

function onBegin() {
    showLoading();
}

function onFailure() {
    alertify.notify('Opps! Birşeyler ters gitti.', 'error', 10, function () { }).dismissOthers();
}

function onSuccess(response) {
    if (response.status === "ERROR")
        alertify.notify(response.message, 'error', 5, function () { }).dismissOthers();
    else {
        if (urlLIST)
            window.location.href = urlLIST;
        else
            alertify.notify('İşlem Başarılı.', 'success', 5, function () { }).dismissOthers();
    }
}

function onComplete() {
    hideLoading();
}

function onBeginSms() {
    showLoading();
}

function onFailureSms() {
    alertify.notify('Sms Gönderilemiyor!', 'error', 10, function () { }).dismissOthers();
}

function onSuccessSms(arg, response) {
    if (response.status === "OK")
        afterSms(arg, response);
    else
        alertify.notify(response.message, 'error', 10, function () { }).dismissOthers();
}

function onCompleteSms() {
    hideLoading();
}

function formatDate(dt) {
    if (dt) {
        var dts = dt.split('T');
        if (dts.length === 2)
            return dts[0].split('-').reverse().join('.');
        else
            return dts[0].split('-').reverse().join('.');
    }

    return "";
}

function formatDateTime(dt) {
    if (dt) {
        var dts = dt.split('T');

        if (dts.length === 2)
            return dts[1].substr(0, 5) + ' ' + dts[0].split('-').reverse().join('.');
        else
            return dts[0].split('-').reverse().join('.');
    }

    return "";
}

function getNotifyCounts() {
    $.ajax({
        type: "GET",
        url: '/Main/GetNotifyCounts/',
        contentType: 'application/json; charset=utf-8',
        success: function (main) {
            for (const [k, v] of Object.entries(main)) {
                if (v === 0) {
                    $("#badge" + k).text("");
                    $("#badge" + k).addClass("d-none");
                }
                else {
                    $("#badge" + k).text(v);
                    $("#badge" + k).removeClass("d-none");
                }

                if ((k === "totalPending" || k === "supports") && v > 0) {
                    var pathname = window.location.pathname;
                    if (pathname === "/Main" || pathname === "/main")
                        snd.play();
                }
            }
        },
        error: function () { },
        complete: function () { }
    })
}

function refreshExitDate() {
    $.ajax({
        type: "POST",
        url: '/Login/RefreshExitDate/',
        contentType: 'application/json; charset=utf-8',
        success: function (data) { },
        error: function () { },
        complete: function () { }
    })
}

window.onload = function () {
    cookieExprTimer = sessionStorage.getItem("cookieTimeoutTimer") || 3450;
    alertifyConfirmCounterdown = sessionStorage.getItem("alertifyConfirmCounterdown") || 60;
};

var intervalCookieTimeoutFunction = function () {
    var enabled = sessionStorage.getItem("enablecookietimeout")
    if (!enabled) {
        clearInterval(intervalCookieTimeout);
    }
    if (cookieExprTimer == 0) {
        alertCookieTimeout(); 
        clearInterval(intervalCookieTimeout);
    } else {
        cookieExprTimer--;
        sessionStorage.setItem("cookieTimeoutTimer", cookieExprTimer);
    }
};

function alertCookieTimeout() {
    var counter = 0;
    alertify.dialog('confirm').set({ transition: 'slide' }).set({
        onshow: function () {
            setInterval(function () {
                if (alertifyConfirmCounterdown <= 0) {
                    showLoading();
                    $.post("/Login/RefreshCookie", { "value": false }, function (response) {
                        if (response.status === "ERROR") {
                            sessionStorage.clear();
                            window.location.reload();
                        }
                    }).fail(function () {
                        onFailure();
                    }).always(function () {
                        hideLoading();
                    });
                }
                else {
                    alertifyConfirmCounterdown--;
                    sessionStorage.setItem("alertifyConfirmCounterdown", alertifyConfirmCounterdown);
                    counter = alertifyConfirmCounterdown;
                    $('#showCountdown').text(counter  + ' Saniye Sonra Otomatik Oturumunuz Otomatik Sonlanacaktır..')
                }
            }, 1000)
        }
    })
    alertify.confirm('Closable: false').set('closable', false).set('labels', { ok: 'EVET', cancel: 'HAYIR' })
    alertify.confirm(
        'Oturum Süresini Uzatmak İstiyor musunuz ?',
        `<p class="text-danger fs-6 fw-bold" id="showCountdown"> 60 Saniye Sonra Otomatik Oturumunuz Otomatik Sonlanacaktır..</p >`,
        function () {
            showLoading();
            $.post("/Login/RefreshCookie", { "value": true }, function (response) {
                if (response.status === "OK") {
                    sessionStorage.removeItem("alertifyConfirmCounterdown");
                    sessionStorage.removeItem("cookieTimeoutTimer");
                    window.location.reload();
                }
            }).fail(function () {
                onFailure();
            }).always(function () {
                hideLoading();
            });
        },
        function () {
            showLoading();
            $.post("/Login/RefreshCookie", { "value": false }, function (response) {
                if (response.status === "ERROR") {
                    sessionStorage.clear();
                    window.location.reload();
                }
            }).fail(function () {
                onFailure();
            }).always(function () {
                hideLoading();
            });
        }
    );
}