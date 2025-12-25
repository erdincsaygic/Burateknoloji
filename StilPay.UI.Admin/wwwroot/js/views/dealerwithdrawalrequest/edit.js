var urlRedirect = '/DealerWithdrawalRequest/Index';

$(document).ready(function () {

});


function onSuccess(response) {
    if (response.status === "ERROR") {
        alertify.notify(response.message, 'error', 5, function () { }).dismissOthers();
    }
    else
        window.location.href = urlRedirect;
}