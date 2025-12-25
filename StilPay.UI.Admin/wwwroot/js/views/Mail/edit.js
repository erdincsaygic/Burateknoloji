var urlLIST = '/Mail/Index';

$(document).ready(function () {
    $("#frmDef").validate({
        errorElement: 'span',
        errorClass: 'text-danger',
        rules: {
            "entity.Name": {
                required: true,
                minlength: 3
            },
        },
        messages: {
            "entity.Name": {
                required: "Lütfen başlık giriniz",
                minlength: "Uzunluk en az 2 haneli olmalıdır",
            },
        }
    })
});
