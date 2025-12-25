var urlLIST = '/panel/login/index';

$(document).ready(function () {
    $("#frmDef").validate({
        errorElement: 'span',
        errorClass: 'text-danger',
        rules: {
            "entity.IdentityNr": {
                required: true,
                digits: true,
                minlength: 11
            },
            "entity.Name": {
                required: true,
                minlength: 2
            },
            "entity.Email": {
                required: true,
                email: true
            },
            "entity.BirthYear": {
                required: true,
                digits: true,
                minlength: 4
            }
        },
        messages: {
            "entity.IdentityNr": {
                required: "Lütfen kimlik no giriniz",
                digits: "Kimlik no rakamlardan oluşmalıdır",
                minlength: "Uzunluk 11 haneli olmalıdır"
            },
            "entity.Name": {
                required: "Lütfen ad soyad giriniz",
                minlength: "Uzunluk en az 2 haneli olmalıdır"
            },
            "entity.Email": {
                required: "Lütfen email giriniz",
                email: "Geçerli bir email giriniz"
            },
            "entity.BirthYear": {
                required: "Lütfen doğum yılı giriniz",
                digits: "Doğum Yılı rakamlardan oluşmalıdır",
                minlength: "Uzunluk en az 4 haneli olmalıdır"
            }
        }
    })
});
