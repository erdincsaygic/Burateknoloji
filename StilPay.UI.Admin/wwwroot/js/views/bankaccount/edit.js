var urlLIST = '/AccountingReport/BankAccount';
var urlEDIT = '/BlogCategory/Edit/__id__';
var urlDELETE = '/BlogCategory/Drop';

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
            },
            "entity.AccountNr": {
                required: true
            },
            "entity.OrderNr": {
                min: 1,
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
            },
            "entity.AccountNr": {
                required: "Lütfen hesap numarası giriniz",
            },
            "entity.OrderNr": {
                min: "Lütfen 1 den büyük değer giriniz",
            }
        }
    })

    $("#entity_IBAN").mask('TR00 0000 0000 0000 0000 0000 00')
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
