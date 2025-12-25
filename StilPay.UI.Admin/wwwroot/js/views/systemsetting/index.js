var urlLIST = null;

$(document).ready(function () {  

    $("#paramToggle").change(function () {
        if ($(this).is(":checked")) {
            $("#payNKolayToggle").bootstrapToggle('off');
        }
    });
    $("#payNKolayToggle").change(function () {
        if ($(this).is(":checked")) {
            $("#paramToggle").bootstrapToggle('off');
        }
    });

    $("#ziraatBankToggle").change(function () {
        if ($(this).is(":checked")) {
            $("#isBankToggle").bootstrapToggle('off');
        }
    });

    $("#isBankToggle").change(function () {
        if ($(this).is(":checked")) {
            $("#ziraatBankToggle").bootstrapToggle('off');
        }
    });

    $(document).on("change", "#switch", function () {
        $('#frmSubmit').submit();
    });
});


function onSuccess2(response) {
    if (response.status === "ERROR") {
        alertify.notify(response.message, 'error', 5, function () { }).dismissOthers();
        hideLoading();
        setTimeout(function () {
            location.reload();
        }, 2000)
    } else {
        alertify.notify("İşlem Başarılı", 'success', 3, function () { }).dismissOthers();
        setTimeout(function () {
            location.reload();
        }, 1000)
    }
}
