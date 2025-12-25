var urlLIST = "";

$(document).ready(function () {

    $("#frmCommission").validate({
        errorElement: 'span',
        errorClass: 'text-danger',
        rules: {
            "Commission.CreditCardRate": {
                required: true,
            },
            "Commission.TransferRate": {
                required: true,
            },
            "Commission.MobilePayRate": {
                required: true,
            }
            ,
            "ConfirmCode": {
                required: true,
                digits: true,
                minlength: 6
            }
        },
        messages: {
            "Commission.CreditCardRate": {
                required: "Lütfen oran giriniz",
            },
            "Commission.TransferRate": {
                required: "Lütfen oran giriniz"
            },
            "Commission.MobilePayRate": {
                required: "Lütfen oran giriniz",
            }
            ,
            "ConfirmCode": {
                required: "Lütfen doğrulama kodu giriniz",
                digits: "Rakamlardan oluşmalıdır",
                minlength: "6 haneli olmalıdır"
            }
        }
    })

    $("#Commission_CreditCardRate").numeric_format({
        defaultvalue: "0,00",
        thousandslimit: 2,
        centsLimit: 2,
        centsSeparator: ',',
        thousandsSeparator: ',',
        clearOnEmpty: false
    });

    $("#Commission_CreditCardBlocked").numeric_format({
        defaultvalue: "0",
        thousandslimit: 2,
        centsLimit: 0,
        centsSeparator: ',',
        thousandsSeparator: ',',
        clearOnEmpty: false
    });

    $("#Commission_ForeignCreditCardRate").numeric_format({
        defaultvalue: "0,00",
        thousandslimit: 2,
        centsLimit: 2,
        centsSeparator: ',',
        thousandsSeparator: ',',
        clearOnEmpty: false
    });

    $("#Commission_ForeignCreditCardBlocked").numeric_format({
        defaultvalue: "0",
        thousandslimit: 2,
        centsLimit: 0,
        centsSeparator: ',',
        thousandsSeparator: ',',
        clearOnEmpty: false
    });

    $("#Commission_TransferRate").numeric_format({
        defaultvalue: "0,00",
        thousandslimit: 6,
        centsLimit: 2,
        centsSeparator: ',',
        thousandsSeparator: ',',
        clearOnEmpty: false
    });


    $("#Commission_WithdrawalTransferAmount").numeric_format({
        defaultvalue: "0,00",
        thousandslimit: 4,
        centsLimit: 2,
        centsSeparator: ',',
        thousandsSeparator: ',',
        clearOnEmpty: false
    });

    $("#Commission_WithdrawalEftAmount").numeric_format({
        defaultvalue: "0,00",
        thousandslimit: 4,
        centsLimit: 2,
        centsSeparator: ',',
        thousandsSeparator: ',',
        clearOnEmpty: false
    });
    $("#Commission_WithdrawalForeignCurrencySwiftAmount").numeric_format({
        defaultvalue: "0,00",
        thousandslimit: 4,
        centsLimit: 2,
        centsSeparator: ',',
        thousandsSeparator: ',',
        clearOnEmpty: false
    });
    $("#Commission_SPWithdrawalTransferCostAmount").numeric_format({
        defaultvalue: "0,00",
        thousandslimit: 4,
        centsLimit: 2,
        centsSeparator: ',',
        thousandsSeparator: ',',
        clearOnEmpty: false
    });

    $("#Commission_SPWithdrawalEftCostAmount").numeric_format({
        defaultvalue: "0,00",
        thousandslimit: 4,
        centsLimit: 2,
        centsSeparator: ',',
        thousandsSeparator: ',',
        clearOnEmpty: false
    });
    $("#Commission_SPWithdrawalForeignCurrencySwiftCostAmount").numeric_format({
        defaultvalue: "0,00",
        thousandslimit: 4,
        centsLimit: 2,
        centsSeparator: ',',
        thousandsSeparator: ',',
        clearOnEmpty: false
    });
    $("#Commission_TransferBlocked").numeric_format({
        defaultvalue: "0",
        thousandslimit: 2,
        centsLimit: 0,
        centsSeparator: ',',
        thousandsSeparator: ',',
        clearOnEmpty: false
    });

    $("#Commission_MobilePayRate").numeric_format({
        defaultvalue: "0,00",
        thousandslimit: 2,
        centsLimit: 0,
        centsSeparator: ',',
        thousandsSeparator: ',',
        clearOnEmpty: false
    });

    $("#Commission_MobilePayBlocked").numeric_format({
        defaultvalue: "0",
        thousandslimit: 2,
        centsLimit: 2,
        centsSeparator: ',',
        thousandsSeparator: ',',
        clearOnEmpty: false
    });

    $("#Commission_ToslaRate").numeric_format({
        defaultvalue: "0,00",
        thousandslimit: 2,
        centsLimit: 2,
        centsSeparator: ',',
        thousandsSeparator: ',',
        clearOnEmpty: false
    });

    $("#Commission_ToslaBlocked").numeric_format({
        defaultvalue: "0",
        thousandslimit: 2,
        centsLimit: 0,
        centsSeparator: ',',
        thousandsSeparator: ',',
        clearOnEmpty: false
    });

    $("#ConfirmCode").numeric_format({
        defaultvalue: "",
        thousandslimit: 6,
        centsLimit: 0,
        centsSeparator: ',',
        thousandsSeparator: ',',
        clearOnEmpty: true
    });

    $("#btnSendCode").click(function () {
        $.post("/Dealer/SendConfirmSms", { "operationType": "Commission_Rate_ConfirmCode" }, function (response) {
            if (response.status === "OK") {
                alertify.success('İşlem başarılı..');
                $("button[type=submit").removeClass("d-none");
            }
            else
                alertify.error('Opps! Hata oluştu..');
        })
    });

    $('#mdlCommission').on('hidden.bs.modal', function () {
        $(this).find('form').trigger('reset');
    });
});


function showMdlCommission(id) {
    for (var i = 1; i <= 5; i++)
        $("#c" + i).addClass("d-none");

    $("#c" + id).removeClass("d-none");

    if (id == 2) {
        $(".modal-dialog").addClass("modal-lg");
    } else {
        $(".modal-dialog").removeClass("modal-lg");
    }

    $('#mdlCommission').modal('toggle');
}

function onSuccessCommission(response) {
    setTimeout(function () {
        if (response.status === "ERROR") {
            alertify.notify(response.message, 'error', 3, function () { }).dismissOthers();
            $('#mdlCommission').modal('toggle');
            hideLoading();
        }
        else
            window.location.reload();
    }, 500);
}