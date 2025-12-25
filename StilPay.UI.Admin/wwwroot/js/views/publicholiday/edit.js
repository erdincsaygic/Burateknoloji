var urlLIST = '/PublicHoliday/Index'

$(document).ready(function () {
    $("#frmPublicHoliday").validate({
        errorElement: 'div',
        errorClass: 'text-danger',
        rules: {
            "entity.Name": {
                required: true
            },
            "entity.HolidayDate": {
                required: true
            }
        },
        messages: {
            "entity.Name": {
                required: "Lütfen tatil ismi giriniz"
            },
            "entity.HolidayDate": {
                required: "Lütfen tatil tarihi giriniz"
            }
        }
    })
});
