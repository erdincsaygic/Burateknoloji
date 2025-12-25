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
            "entity.IdentityNr": {
                required: true,
                digits: true,
                minlength: 11
            },
            "entity.Phone": {
                required: true,
                digits: true,
                minlength: 11
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
            "entity.Name": {
                required: "Lütfen ad soyad giriniz",
                minlength: "Uzunluk en az 2 haneli olmalıdır"
            },
            "entity.IdentityNr": {
                required: "Lütfen TC kimlik no giriniz",
                digits: "TC kimlik no rakamlardan oluşmalıdır",
                minlength: "Uzunluk en az 11 haneli olmalıdır"
            },
            "entity.Phone": {
                required: "Lütfen telefon no giriniz",
                digits: "Telefon no rakamlardan oluşmalıdır",
                minlength: "Uzunluk en az 11 haneli olmalıdır"
            },
            "entity.Email": {
                required: "Lütfen email giriniz",
                email: "Geçerli bir email giriniz"
            },
            "entity.BirthYear": {
                required: "Lütfen doğum yılı giriniz",
                digits: "Doğum yılı rakamlardan oluşmalıdır",
                minlength: "Uzunluk en az 4 haneli olmalıdır"
            }
        }
    })
});

function afterSms(arg, response) {
    if (arg === 'Member_Edit_Using_Balance') {
        $("#mdlUsingBalance").modal('show');
    }
    else if (arg === 'Member_Edit_Balance_Update') {
        $("#mdlUsingBalance").modal('show');
    }
    else if (arg === 'Member_Edit_Member_Type') {
        $("#mdlMemberType").modal('show');
    }
    else if (arg === 'Member_Edit_Member_Status') {
        $("#mdlMemberStatus").modal('show');
    }
}