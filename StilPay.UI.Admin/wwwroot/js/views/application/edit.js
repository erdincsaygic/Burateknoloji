var urlLIST = '/application/index';

$(document).ready(function () {
    $(".btn-document").click(function () {
        $.ajax({
            type: "POST",
            url: '/Application/SetFileStatus/',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({
                ID: $("#Application_ID").val(),
                File: $(this).attr("data-file-title"),
                Status: $(this).attr("data-file-status")
            }),
            beforeSend: function () {
                showLoading();
            },
            success: function (response) {
                if (response.status === "ERROR") {
                    hideLoading();
                    alertify.notify(response.message, 'error', 5, function () { }).dismissOthers();
                }
                else
                    location.reload();
            },
            error: function () {
                onFailure();
            },
            complete: function () {
            }
        })
    });

    $("#sendSmsFrm").submit(function (e) {
        var msg = $('#messageBodyTextarea').val();
        if (msg == null || msg == "") {
            alertify.notify('Mesaj Alanı Boş Bırakılamaz', 'error', 5, function () { }).dismissOthers();
            e.preventDefault(e);
            return false;
        }

        else {
            $('#messageBody').val($('#messageBodyTextarea').val());
        }
    });
});
