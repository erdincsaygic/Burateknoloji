var urlLIST = '/panel/master/index';

const banks = $.parseJSON($("#jsonBanks").val());

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
            }
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
            }
        }
    })


    if (banks && banks.length > 0) {
        $("#imgBank").attr("src", '../../areas/panel/img/banks/' + banks[0].Img);
        $("#txtBank").val(banks[0].Name);
        $("#entity_IDBank").val(banks[0].ID);
        $("#spanIban").text(banks[0].IBAN ?? "-");
        $("#spanAlici").text(banks[0].Title ?? "-");
        $("#spanSube").text((banks[0].Branch ?? "-") + ' & ' + (banks[0].AccountNr ?? "-"));
    }

    $(".bank").click(function () {
        $(".bank").removeClass("active-bank");
        $(this).addClass("active-bank");
        var idBank = $(this).attr("data-bank-id");

        for (i = 0; i <= banks.length; i++) {
            if (banks[i].ID === idBank) {
                $("#imgBank").attr("src", '../../areas/panel/img/banks/' + banks[i].Img);
                $("#txtBank").val(banks[i].Name);
                $("#entity_IDBank").val(banks[i].ID);
                $("#spanIban").text(banks[i].IBAN ?? "-");
                $("#spanAlici").text(banks[i].Title ?? "-");
                $("#spanSube").text((banks[i].Branch ?? "-") + ' & ' + (banks[i].AccountNr ?? "-"));
                break;
            }
        }
    });

    $("#btnNotifyPayment").click(function () {
        $("#mdlPaymentRequest").modal("show");
    });

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
})

function copytoclipboard(id) {
    var copyText = $("#" + id).text();

    navigator.clipboard.writeText(copyText).then(() => {
        $("#toastPlacement").toast({
            animation: true,
            autohide: true,
            delay: 1000
        });
        $("#toastPlacement").toast("show");
    });
}
