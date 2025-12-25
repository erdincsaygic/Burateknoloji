var intervalCookieTimeout = null;
$(document).ready(function () {
    $("input[type=text]").attr("autocomplete", "off");

    //intervalCookieTimeout = setInterval(intervalCookieTimeoutFunction, 1000);
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
    btnOperations += showEdit && showDrop ? '<a href="javascript:drop(\'' + id + '\')" class="btn btn-danger btn-sm"><i class="fa fa-trash-can"></i></a>&nbsp;' : '<a style="visibility: hidden;" class="btn btn-danger btn-sm"><i class="fa fa-trash-can"></i></a>&nbsp;';
    btnOperations += showDrop ? '<a href="javascript:edit(\'' + id + '\')" class="btn btn-dark btn-sm"><i class="fa fa-pencil"></i></a>' : '';

    return btnOperations;
}

function edit(id) {
    window.location.href = urlEDIT.replace('__id__', id);
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


window.onload = function () {
    cookieExprTimer = sessionStorage.getItem("cookieTimeoutTimer") || 810;
    alertifyConfirmCounterdown = sessionStorage.getItem("alertifyConfirmCounterdown") || 60;
};

var intervalCookieTimeoutFunction = function () {
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
                    $('#showCountdown').text(counter + ' Saniye Sonra Otomatik Oturumunuz Otomatik Sonlanacaktır..')
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