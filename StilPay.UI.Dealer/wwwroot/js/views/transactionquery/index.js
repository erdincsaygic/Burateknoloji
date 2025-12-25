

function onSuccessRedirectUrl(response) {
    $("#queryParameterTxt").val("");
    if (response.status == "ERROR") {
        alertify.notify(response.message, 'error', 3, function () { }).dismissOthers();
    } else {
        if (response.data.secondUrl != "" && response.data.secondUrl != null) {
            window.open(response.data.url, '_blank');
            window.open(response.data.secondUrl, '_blank');
        } else
            window.open(response.data.url, '_blank');

    }
};