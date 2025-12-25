var urlLIST = '/announcement/index';

$(document).ready(function () {
    $("#frmDef").validate({
        errorElement: 'div',
        errorClass: 'text-danger',
        rules: {
            "entity.StartDate": {
                required: true
            },
            "entity.EndDate": {
                required: true
            },
            "entity.Title": {
                required: true
            },
            "entity.Body": {
                required: true
            }
        },
        messages: {
            "entity.StartDate": {
                required: "Lütfen başlangıç tarihi seçiniz"
            },
            "entity.EndDate": {
                required: "Lütfen bitiş tarihi seçiniz"
            },
            "entity.Title": {
                required: "Lütfen başlık giriniz"
            },
            "entity.Body": {
                required: "Lütfen içerik giriniz"
            }
        }
    })

    $(".datepicker").datepicker({
        //changeMonth: true,
        //changeYear: true,
        dateFormat: "dd.mm.yy",
        altFormat: "dd.mm.yy",
        monthNames: ["Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran", "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık"],
        dayNamesMin: ["Pa", "Pt", "Sl", "Ça", "Pe", "Cu", "Ct"],
        firstDay: 1
    })
});
