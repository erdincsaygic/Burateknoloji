var urlLIST = '/DealerRebateRequest/Index';

$(document).ready(function () {
    $("#entity_Iban").mask('TR00 0000 0000 0000 0000 0000 00')
});

function onSuccess(response) {
    if (response.status === "ERROR") {
        $("#mdlConfirm").modal('hide');
        alertify.notify(response.message, 'error', 5, function () { }).dismissOthers();

    }
    else {
        if (urlLIST)
            window.location.href = urlLIST;
        else
            alertify.notify('İşlem Başarılı.', 'success', 5, function () { }).dismissOthers();
    }
}