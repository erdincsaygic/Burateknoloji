var urlLIST = '/bankaccount/index';

$(document).ready(function () {
    $("#frmDef").validate({
        errorElement: 'div',
        errorClass: 'text-danger',
        rules: {
            "entity.Name": {
                required: true
            },
            "entity.IDBank": {
                required: true
            },
            "entity.Title": {
                required: true
            },
            "entity.IBAN": {
                required: true,
                minlength: 32
            }
        },
        messages: {
            "entity.Name": {
                required: "Lütfen takma ad giriniz"
            },
            "entity.IDBank": {
                required: "Lütfen banka seçiniz"
            },
            "entity.Title": {
                required: "Lütfen hesap sahibi giriniz"
            },
            "entity.IBAN": {
                required: "Lütfen IBAN giriniz",
                minlength: "Uzunluk en az 26 haneli olmalıdır"
            }
        }
    })

    $("#entity_IBAN").mask('TR00 0000 0000 0000 0000 0000 00')
});
