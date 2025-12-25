var urlLIST = '';

$(document).ready(function () {
    initMultipleCompanySelectionWithoutAll();

});

function onSuccess(response) {
    if (response.status === "ERROR") {
        alertify.notify(response.message, 'error', 5, function () { }).dismissOthers();

    }
    else {
        alertify.notify(response.message, 'success', 5, function () { }).dismissOthers();
    }
};