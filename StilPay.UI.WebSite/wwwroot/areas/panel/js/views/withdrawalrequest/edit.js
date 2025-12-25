var urlLIST = '/panel/master/index';

$(document).ready(function () {
    $("#frmDef").validate({
        errorElement: 'div',
        errorClass: 'text-danger',
        rules: {
            "entity.IDBank": {
                required: true
            },
            "entity.IBAN": {
                required: true,
                minlength: 30
            },
            "entity.Title": {
                required: true
            }
        },
        messages: {
            "entity.IDBank": {
                required: "Lütfen banka seçiniz"
            },
            "entity.IBAN": {
                required: "Lütfen IBAN giriniz",
                minlength: "Uzunluk 24 haneli olmalıdır"
            },
            "entity.Title": {
                required: "Lütfen hesap sahibi giriniz"
            }
        }
    })

    $("#entity_IBAN").mask('00 0000 0000 0000 0000 0000 00')

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
            $("#txtDetailBank").val($("#entity_IDBank option:selected").text());
            $("#txtDetailIBAN").val('TR' + $("#entity_IBAN").val());
            $("#txtDetailTitle").val($("#entity_Title").val());
            $("#txtDetailAmount").val($("#entity_Amount").val());
            $("#txtDetailType").val($("input[name='entity.IsEFT']:checked").val().toString() === 'true' ? 'Havale/EFT' : 'FAST');
            $("#mdlDetail").modal('show');
        }
    })

    $("#btnOK").click(function () {
        $("#mdlDetail").modal('hide');
        $("#frmDef").submit();
    })

});
