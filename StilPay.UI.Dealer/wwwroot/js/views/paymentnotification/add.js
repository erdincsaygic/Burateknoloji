var urlLIST = '/PaymentNotification';

$(document).ready(function () {
    $("#frmDef").validate({
        errorElement: 'div',
        errorClass: 'text-danger',
        rules: {
            "entity.IDBank": {
                required: true
            },
            "entity.SenderName": {
                required: true
            },
            "entity.ActionDate": {
                required: true
            },
            "entity.ActionTime": {
                required: true
            },
            "entity.Amount": { required: true }
        },
        messages: {
            "entity.IDBank": {
                required: "Lütfen banka seçiniz"
            },
            "entity.SenderName": {
                required: "Lütfen gönderici giriniz",
            },
            "entity.ActionDate": {
                required: "Lütfen ödeme tarihi giriniz"
            },
            "entity.ActionTime": {
                required: "Lütfen ödeme saati giriniz"
            },
            "entity.Amount": { required: "Lütfen ödeme tutarı giriniz" }
        }
    })


    $('.timepicker').timepicker({
        timeFormat: 'HH:mm',
        interval: 1,
        minTime: '0',
        maxTime: '23',
        defaultTime: '',
        startTime: '00:00',
        dynamic: false,
        dropdown: false,
        scrollbar: false
    }).mask('00:00')
        .focusout(function () {
            if (!$(this).val())
                $('.timepicker').val("00:00")
        })

    $(".datepicker").datepicker({
        showAnim: "slide",
        autoSize: true,
        dateFormat: "dd.mm.yy",
        maxDate: "0d"
    })

    $("#entity_Amount").numeric_format({
        defaultvalue: "0,00",
        thousandslimit: 7,
        centsLimit: 2,
        centsSeparator: ',',
        thousandsSeparator: '.',
        clearOnEmpty: false
    })

    if ($("#entity_Amount").val() === "") {
        $("#entity_Amount").val("0,00");
    }
})
