var urlLIST = '/foreigncreditcardpaymentnotification/index';

$(document).ready(function () {
    $("#frmMemberType").validate({
        errorElement: 'div',
        errorClass: 'text-danger',
        rules: {
            "IDMemberType": {
                required: true
            }
        },
        messages: {
            "IDMemberType": {
                required: "Lütfen üye tipi seçiniz"
            }
        }
    })
});

function onSuccessMemberType(response) {
    if (response.status === "ERROR")
        alertify.notify(response.message, 'error', 5, function () { }).dismissOthers();
    else
        location.reload();
}