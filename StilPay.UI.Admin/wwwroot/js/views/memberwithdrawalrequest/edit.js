var urlLIST = '/MemberWithdrawalRequest/Index';

$(document).ready(function () {

});

function onSuccess(response) {
    if (response.status === "ERROR") {
        alertify.notify(response.message, 'error', 5, function () { }).dismissOthers();
        setTimeout(function () {
            window.parent.location.href = '/';
        }, 5000)
    }
    else
        window.location.href = urlRedirect;
}