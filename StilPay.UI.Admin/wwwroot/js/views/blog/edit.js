var urlLIST = '/Blog/Index'

$(document).ready(function () {
    $("#frmDef").validate({
        errorElement: 'span',
        errorClass: 'text-danger',
        rules: {
            "entity.Title": {
                required: true,
                minlength: 3
            },
            "entity.Body": {
                required: true,
                minlength: 3
            },
            "entity.OrderNr": {
                required: true,
                minlength: 1
            }
        },
        messages: {
            "entity.Title": {
                required: "Lütfen başlık giriniz",
                minlength: "Uzunluk en az 3 haneli olmalıdır",
            },
            "entity.Body": {
                required: "Lütfen içerik giriniz",
                minlength: "Uzunluk en az 3 haneli olmalıdır",
            },
            "entity.OrderNr": {
                required: "Lütfen sira belirtin",
                minlength: "En az bir sayı girin",
            }
        }
    })
});
