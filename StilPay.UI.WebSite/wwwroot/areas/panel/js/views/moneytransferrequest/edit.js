var urlLIST = '/panel/master/index';

$(document).ready(function () {
    $("#frmDef").validate({
        errorElement: 'div',
        errorClass: 'text-danger',
        rules: {
            "entity.ReceiverPhone": {
                required: true,
                digits: true,
                minlength: 10
            }
        },
        messages: {
            "entity.ReceiverPhone": {
                required: "Lütfen telefon giriniz",
                digits: "Rakamlardan oluşmalıdır",
                minlength: "Uzunluk 10 haneli olmalıdır"
            }
        }
    })

    $("#entity_ReceiverPhone").mask('0000000000')

    $("#entity_Amount").numeric_format({
        defaultvalue: "0,00",
        thousandslimit: 7,
        centsLimit: 2,
        centsSeparator: ',',
        thousandsSeparator: '.',
        clearOnEmpty: false
    })

    $("#btnSubmit").click(function () {
        if ($("#frmDef").valid()) {
            $("#txtDetailAmount").val($("#entity_Amount").val());
            $("#txtDetailReceiverPhone").val($("#entity_ReceiverPhone").val());
            $("#mdlDetail").modal('show');
        }
    })

    $("#btnOK").click(function () {
        $("#mdlDetail").modal('hide');
        $("#frmDef").submit();
    })

});