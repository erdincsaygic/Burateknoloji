var urlLIST = null;

$(document).ready(function () {
    $("#frmDef").validate({
        errorElement: 'span',
        errorClass: 'text-danger',
        rules: {
            "entity.Name": {
                required: true,
                minlength: 2
            },
            "entity.Phone": {
                required: true,
                digits: true,
                minlength: 11
            },
            "entity.Email": {
                required: true,
                email: true
            }
        },
        messages: {
            "entity.Name": {
                required: "Lütfen ad soyad giriniz",
                minlength: "Uzunluk en az 2 haneli olmalıdır"
            },
            "entity.Phone": {
                required: "Lütfen telefon no giriniz",
                digits: "Telefon no rakamlardan oluşmalıdır",
                minlength: "Uzunluk en az 11 haneli olmalıdır"
            },
            "entity.Email": {
                required: "Lütfen email giriniz",
                email: "Geçerli bir email giriniz"
            }
        }
    })
});