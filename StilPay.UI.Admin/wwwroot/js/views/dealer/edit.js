var urlLIST = "";
const companyUsers = $.parseJSON($("#jsonCompanyUsers").val());

$(document).ready(function () {
    initCompanySelectionWithoutAll(false);

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
            "entity.Title": {
                required: true,
                minlength: 2
            },
            "entity.TaxNr": {
                required: true,
                digits: true,
                minlength: 10
            },
            "entity.TaxOffice": {
                required: true,
                minlength: 2
            },
            "entity.Address": {
                required: true,
                minlength: 2
            },
            "entity.MonthlyGiro": {
                required: true
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
                required: "Lütfen gsm numarası giriniz",
                digits: "Gsm no rakamlardan oluşmalıdır",
                minlength: "Uzunluk 11 haneli olmalıdır"
            },
            "entity.Title": {
                required: "Lütfen ünvan giriniz",
                minlength: "Uzunluk en az 2 haneli olmalıdır"
            },
            "entity.TaxNr": {
                required: "Lütfen vergi no giriniz",
                digits: "Vergi no rakamlardan oluşmalıdır",
                minlength: "Uzunluk en az 10 haneli olmalıdır"
            },
            "entity.TaxOffice": {
                required: "Lütfen vergi dairesi giriniz",
                minlength: "Uzunluk en az 2 haneli olmalıdır"
            },
            "entity.Address": {
                required: "Lütfen adres giriniz",
                minlength: "Uzunluk en az 2 haneli olmalıdır"
            },
            "entity.MonthlyGiro": {
                required: "Lütfen ortalama aylık ciro seçiniz"
            },
            "entity.Email": {
                required: "Lütfen email giriniz",
                email: "Geçerli bir email giriniz"
            }
        }
    });

    //#region UsingBalance
    $("#frmUsingBalance").validate({
        errorElement: 'span',
        errorClass: 'text-danger',
        rules: {
            "UsingBalanceConfirmCode": {
                required: true,
                digits: true,
                minlength: 6
            }
        },
        messages: {
            "UsingBalanceConfirmCode": {
                required: "Lütfen doğrulama kodu giriniz",
                digits: "Rakamlardan oluşmalıdır",
                minlength: "6 haneli olmalıdır"
            }
        }
    });

    //#region BalanceTransfer
    $("#frmBalanceTransfer").validate({
        errorElement: 'span',
        errorClass: 'text-danger',
        rules: {
            "BalanceTransfer_ConfirmCode": {
                required: true,
                digits: true,
                minlength: 6
            }
        },
        messages: {
            "BalanceTransfer_ConfirmCode": {
                required: "Lütfen doğrulama kodu giriniz",
                digits: "Rakamlardan oluşmalıdır",
                minlength: "6 haneli olmalıdır"
            }
        }
    });

    $('#mdlUsingBalance').on('hidden.bs.modal', function () {
        $(this).find('form').each(function () {
            this.reset();
        });
    });

    $("#UsingBalanceConfirmCode").numeric_format({
        defaultvalue: "",
        thousandslimit: 6,
        centsLimit: 0,
        centsSeparator: ',',
        thousandsSeparator: ',',
        clearOnEmpty: true
    });

    $("#BalanceTransfer_ConfirmCode").numeric_format({
        defaultvalue: "",
        thousandslimit: 6,
        centsLimit: 0,
        centsSeparator: ',',
        thousandsSeparator: ',',
        clearOnEmpty: true
    });



    $("#Balance_Amount").on({
        keyup: function () {
            formatCurrency($(this));
        },
        blur: function () {
            formatCurrency($(this), "blur");
        }
    });

    $("#BalanceTransfer_Amount").on({
        keyup: function () {
            formatCurrency($(this));
        },
        blur: function () {
            formatCurrency($(this), "blur");
        }
    });

    $("#mdlUsingBalanceSendCode").click(function () {
        $.post("/Dealer/SendConfirmSms", { "operationType": "UsingBalance_ConfirmCode" }, function (response) {
            if (response.status === "OK") {
                alertify.success('İşlem başarılı..');
                $("#Balance_Amount").removeAttr("disabled");
                $("#balanceUpdateBtn").removeClass("disabled");
            }
            else
                alertify.error('Opps! Hata oluştu..');
        })
    });

    document.getElementById('slcCompanies').addEventListener('change', function () {
        const selectedOption = this.options[this.selectedIndex];
        let selectedText = selectedOption.value ? selectedOption.text : "";

        if (selectedText.includes('-')) {
            selectedText = selectedText.split('-')[0].trim();
        }

        document.getElementById('selectedCompanyName').value = selectedText;
    });

    $("#mdBalanceTransferSendCode").click(function () {
        var companyName = $("#companyName").val();
        var selectedCompanyName = $("#selectedCompanyName").val();
        var amount = $("#BalanceTransfer_Amount").val();

        if (selectedCompanyName == undefined || !selectedCompanyName || selectedCompanyName === "") {
            alertify.error('Üye işyeri seçiniz');
            return
        }
        if (amount == undefined || !amount) {
            alertify.error('Tutar giriniz');
            return
        }

        var message = `${companyName} üye işyerinden - ${selectedCompanyName} Tutar: ${amount} kullanılabilir bakiye aktarımı için `

        $.post("/Dealer/SendConfirmSms", { "operationType": "BalanceTransfer_ConfirmCode", "message": message, "phone": "05452162199" }, function (response) {
            if (response.status === "OK") {
                alertify.success('İşlem başarılı..');
                $("#transferBtn").removeClass("disabled");
            }
            else
                alertify.error('Opps! Hata oluştu..');
        })
    });

    $("#mdlUsingBalanceBtn").on("click", function () {
        $("#balanceUpdateBtn").addClass("disabled");

        $("#transferBtn").addClass("disabled");

        $("#Balance_Amount").removeAttr("value");

        $("#BalanceTransfer_Amount").removeAttr("value");
        $("#mdlUsingBalance").modal('toggle');
    });

    document.getElementById('operationType').addEventListener('change', function () {
        const selectedOption = this.value;

        // Formları göster/gizle
        document.querySelectorAll('.operationForm').forEach(form => {
            form.classList.add('d-none');
            form.reset();  // Formu sıfırla
        });

        document.querySelectorAll('.modal-footer button[type="submit"]').forEach(btn => btn.classList.add('d-none'));

        if (selectedOption === 'balanceUpdate') {
            document.querySelector('.balanceUpdateForm').classList.remove('d-none');
            document.getElementById('balanceUpdateBtn').classList.remove('d-none');
        } else if (selectedOption === 'balanceTransfer') {
            document.querySelector('.balanceTransfer').classList.remove('d-none');
            document.getElementById('transferBtn').classList.remove('d-none');
        }
    });
    // #endregion UsingBalance

    // #region NegativeBalanceLimit

    $("#frmNegativeBalanceLimit").validate({
        errorElement: 'span',
        errorClass: 'text-danger',
        rules: {
            "NegativeBalanceLimit": {
                required: true,
            },
            "NegativeBalanceConfirmCode": {
                required: true,
                digits: true,
                minlength: 6
            }
        },
        messages: {
            "NegativeBalanceLimit": {
                required: "Lütfen tutar giriniz",
            },
            "NegativeBalanceConfirmCode": {
                required: "Lütfen doğrulama kodu giriniz",
                digits: "Rakamlardan oluşmalıdır",
                minlength: "6 haneli olmalıdır"
            }
        }
    });

    $("#NegativeBalanceLimit").removeAttr("value");

    $("#NegativeBalanceLimit").on({
        keyup: function () {
            formatCurrency($(this));
        },
        blur: function () {
            formatCurrency($(this), "blur");
        }
    });

    $('#mdlNegativeBalanceLimit').on('hidden.bs.modal', function () {
        $(this).find('form').trigger('reset');
    });

    $("#mdlNegativeBalanceLimitSendCode").click(function () {
        $.post("/Dealer/SendConfirmSms", { "operationType": "NegativeBalanceLimit_ConfirmCode" }, function (response) {
            if (response.status === "OK") {
                alertify.success('İşlem başarılı..');
                $("#NegativeBalanceLimit").removeAttr("disabled");        
                $("button[type=submit").removeClass("d-none");
            }
            else
                alertify.error('Opps! Hata oluştu..');
        })
    });

    $("#mdlNegativeBalanceLimitBtn").on("click", function () {
        $("#mdlNegativeBalanceLimit").modal('toggle');
    });

    $("#NegativeBalanceConfirmCode").numeric_format({
        defaultvalue: "",
        thousandslimit: 6,
        centsLimit: 0,
        centsSeparator: ',',
        thousandsSeparator: ',',
        clearOnEmpty: true
    });

    //#endregion NegativeBalanceLimit

    //#region CompanyStatus
    $("#frmCompanyStatus").validate({
        errorElement: 'span',
        errorClass: 'text-danger',
        rules: {
            "UsingBalanceConfirmCode": {
                required: true,
                digits: true,
                minlength: 6
            }
        },
        messages: {
            "UsingBalanceConfirmCode": {
                required: "Lütfen doğrulama kodu giriniz",
                digits: "Rakamlardan oluşmalıdır",
                minlength: "6 haneli olmalıdır"
            }
        }
    });

    $('#mdlCompanyStatus').on('hidden.bs.modal', function () {
        $(this).find('form').trigger('reset');
    });

    $("#CompanyStatusConfirmCode").numeric_format({
        defaultvalue: "",
        thousandslimit: 6,
        centsLimit: 0,
        centsSeparator: ',',
        thousandsSeparator: ',',
        clearOnEmpty: true
    });


    $("#mdlCompanyStatusSendCode").click(function () {
        $.post("/Dealer/SendConfirmSms", { "operationType": "CompanyStatus_ConfirmCode" }, function (response) {
            if (response.status === "OK") {
                alertify.success('İşlem başarılı..');
                $("#selectStatus").removeAttr("disabled");  
                $("button[type=submit").removeClass("d-none");
            }
            else
                alertify.error('Opps! Hata oluştu..');
        })
    });

    $("#mdlCompanyStatusBtn").on("click", function () {

        if ($("#statusFlag").val() == "Aktif") {
            $('#selectStatus option[value=True]').attr('selected', 'selected');
        }
        else {
            $('#selectStatus option[value=False]').attr('selected', 'selected');
        }
        
        $("#selectStatus").attr("selected");  
        $("#mdlCompanyStatus").modal('toggle');
    });
    // #endregion CompanyStatus

    //#region AutoWithdrawalLimit
    $("#frmAutoWithdrawalLimit").validate({
        errorElement: 'span',
        errorClass: 'text-danger',
        rules: {
            "entity.AutoWithdrawalLimit": {
                required: true,
            },
            "AutoWithdrawalLimitConfirmCode": {
                required: true,
                digits: true,
                minlength: 6
            }
        },
        messages: {
            "entity.AutoWithdrawalLimit": {
                required: "Lütfen tutar giriniz",
            },
            "AutoWithdrawalLimitConfirmCode": {
                required: "Lütfen doğrulama kodu giriniz",
                digits: "Rakamlardan oluşmalıdır",
                minlength: "6 haneli olmalıdır"
            }
        }
    });

    $('#mdlAutoWithdrawalLimit').on('hidden.bs.modal', function () {
        $(this).find('form').trigger('reset');
    });

    $("#AutoWithdrawalLimitConfirmCode").numeric_format({
        defaultvalue: "",
        thousandslimit: 6,
        centsLimit: 0,
        centsSeparator: ',',
        thousandsSeparator: ',',
        clearOnEmpty: true
    });

    $("#entity_AutoWithdrawalLimit").removeAttr("value");

    $("#entity_AutoWithdrawalLimit").on({
        keyup: function () {
            formatCurrency($(this));
        },
        blur: function () {
            formatCurrency($(this), "blur");
        }
    });

    $("#mdlAutoWithdrawalLimitSendCode").click(function () {
        $.post("/Dealer/SendConfirmSms", { "operationType": "AutoWithdrawalLimit_ConfirmCode" }, function (response) {
            if (response.status === "OK") {
                alertify.success('İşlem başarılı..');
                $("#entity_AutoWithdrawalLimit").removeAttr("disabled");
                //$("button[type=submit").removeClass("d-none");
                $("#autoWithdrawalSubmitBtn").removeClass("d-none");
            }
            else
                alertify.error('Opps! Hata oluştu..');
        })
    });

    $("#mdlAutoWithdrawalLimitBtn").on("click", function () {
        $("#mdlAutoWithdrawalLimit").modal('toggle');
    });
    //#endregion AutoWithdrawalLimit

    //#region AutoTransferlLimit
    $("#frmAutoTransferLimit").validate({
        errorElement: 'span',
        errorClass: 'text-danger',
        rules: {
            "entity.AutoTransferLimit": {
                required: true,
            },
            "AutoTransferLimitConfirmCode": {
                required: true,
                digits: true,
                minlength: 6
            }
        },
        messages: {
            "entity.AutoTransferLimit": {
                required: "Lütfen tutar giriniz",
            },
            "AutoTransferLimitConfirmCode": {
                required: "Lütfen doğrulama kodu giriniz",
                digits: "Rakamlardan oluşmalıdır",
                minlength: "6 haneli olmalıdır"
            }
        }
    });

    $('#mdlAutoWithdrawalLimit').on('hidden.bs.modal', function () {
        $(this).find('form').trigger('reset');
    });

    $("#mdlAutoTransferLimit").numeric_format({
        defaultvalue: "",
        thousandslimit: 6,
        centsLimit: 0,
        centsSeparator: ',',
        thousandsSeparator: ',',
        clearOnEmpty: true
    });

    $("#entity_AutoTransferLimit").removeAttr("value");

    $("#entity_AutoTransferLimit").on({
        keyup: function () {
            formatCurrency($(this));
        },
        blur: function () {
            formatCurrency($(this), "blur");
        }
    });

    $("#mdlAutoTransferLimitSendCode").click(function () {
        $.post("/Dealer/SendConfirmSms", { "operationType": "AutoTransferLimit_ConfirmCode" }, function (response) {
            if (response.status === "OK") {
                alertify.success('İşlem başarılı..');
                $("#entity_AutoTransferLimit").removeAttr("disabled");
                //$("button[type=submit").removeClass("d-none");
                $("#autoTransferSubmitBtn").removeClass("d-none");
            }
            else
                alertify.error('Opps! Hata oluştu..');
        })
    });

    $("#mdlAutoTransferLimitBtn").on("click", function () {
        $("#mdlAutoTransferLimit").modal('toggle');
    });
    //#endregion AutoTransferLimit

    //#region AutoCreditCardLimit
    $("#frmAutoCreditCardLimit").validate({
        errorElement: 'span',
        errorClass: 'text-danger',
        rules: {
            "entity.AutoCreditCardLimit": {
                required: true,
            },
            "AutoCreditCardLimitConfirmCode": {
                required: true,
                digits: true,
                minlength: 6
            }
        },
        messages: {
            "entity.AutoCreditCardLimit": {
                required: "Lütfen tutar giriniz",
            },
            "AutoCreditCardLimitConfirmCode": {
                required: "Lütfen doğrulama kodu giriniz",
                digits: "Rakamlardan oluşmalıdır",
                minlength: "6 haneli olmalıdır"
            }
        }
    });

    $('#mdlAutoCreditCardLimit').on('hidden.bs.modal', function () {
        $(this).find('form').trigger('reset');
    });

    $("#AutoCreditCardLimitConfirmCode").numeric_format({
        defaultvalue: "",
        thousandslimit: 6,
        centsLimit: 0,
        centsSeparator: ',',
        thousandsSeparator: ',',
        clearOnEmpty: true
    });

    $("#entity_AutoCreditCardLimit").removeAttr("value");

    $("#entity_AutoCreditCardLimit").on({
        keyup: function () {
            formatCurrency($(this));
        },
        blur: function () {
            formatCurrency($(this), "blur");
        }
    });

    $("#mdlAutoCreditCardLimitSendCode").click(function () {
        $.post("/Dealer/SendConfirmSms", { "operationType": "AutoCreditCardLimit_ConfirmCode" }, function (response) {
            if (response.status === "OK") {
                alertify.success('İşlem başarılı..');
                $("#entity_AutoCreditCardLimit").removeAttr("disabled");
                //$("button[type=submit").removeClass("d-none");
                $("#autoCreditCardSubmitBtn").removeClass("d-none");
            }
            else
                alertify.error('Opps! Hata oluştu..');
        })
    });

    $("#mdlAutoCreditCardLimitBtn").on("click", function () {
        $("#mdlAutoCreditCardLimit").modal('toggle');
    });
    //#endregion AutoCreditCardLimit


    //#region AutoForeignCreditCardLimit
    $("#frmAutoForeignCreditCardLimit").validate({
        errorElement: 'span',
        errorClass: 'text-danger',
        rules: {
            "entity.AutoForeignCreditCardLimit": {
                required: true,
            },
            "AutoForeignCreditCardLimitConfirmCode": {
                required: true,
                digits: true,
                minlength: 6
            }
        },
        messages: {
            "entity.AutoForeignCreditCardLimit": {
                required: "Lütfen tutar giriniz",
            },
            "AutoForeignCreditCardLimitConfirmCode": {
                required: "Lütfen doğrulama kodu giriniz",
                digits: "Rakamlardan oluşmalıdır",
                minlength: "6 haneli olmalıdır"
            }
        }
    });

    $('#mdlAutoForeignCreditCardLimit').on('hidden.bs.modal', function () {
        $(this).find('form').trigger('reset');
    });

    $("#AutoForeignCreditCardLimitConfirmCode").numeric_format({
        defaultvalue: "",
        thousandslimit: 6,
        centsLimit: 0,
        centsSeparator: ',',
        thousandsSeparator: ',',
        clearOnEmpty: true
    });

    $("#entity_AutoForeignCreditCardLimit").removeAttr("value");

    $("#entity_AutoForeignCreditCardLimit").on({
        keyup: function () {
            formatCurrency($(this));
        },
        blur: function () {
            formatCurrency($(this), "blur");
        }
    });

    $("#mdlAutoForeignCreditCardLimitSendCode").click(function () {
        $.post("/Dealer/SendConfirmSms", { "operationType": "AutoForeignCreditCardLimit_ConfirmCode" }, function (response) {
            if (response.status === "OK") {
                alertify.success('İşlem başarılı..');
                $("#entity_AutoForeignCreditCardLimit").removeAttr("disabled");
                //$("button[type=submit").removeClass("d-none");
                $("#autoForeignCreditCardSubmitBtn").removeClass("d-none");
            }
            else
                alertify.error('Opps! Hata oluştu..');
        })
    });

    $("#mdlAutoForeignCreditCardLimitBtn").on("click", function () {
        $("#mdlAutoForeignCreditCardLimit").modal('toggle');
    });
    //#endregion AutoForeignCreditCardLimit

    if ($('#paramToggle').is(':checked')){
        $('#payNKolayToggle').attr("disabled", "disabled");
    };
    if ($('#payNKolayToggle').is(':checked')) {
        $('#paramToggle').attr("disabled", "disabled");
    };

    //#region CompanyCreateWithdrawalRequest
    $("#frmCompanyCreateWithdrawalRequest").validate({
        errorElement: 'span',
        errorClass: 'text-danger',
        rules: {
            "CompanyCreateWithdrawalRequest.Amount": {
                required: true,
            },
            "CompanyCreateWithdrawalRequest.CurrencyCostTotal": {
                required: true,
            },
            "CompanyCreateWithdrawalRequest.ConfirmCode": {
                required: true,
                digits: true,
                minlength: 6
            }
        },
        messages: {
            "CompanyCreateWithdrawalRequest.Amount": {
                required: "Lütfen tutar giriniz",
            },
            "CompanyCreateWithdrawalRequest.CurrencyCostTotal": {
                required: " Lütfen işlem ücreti giriniz",
            },
            "CompanyCreateWithdrawalRequest.ConfirmCode": {
                required: "Lütfen doğrulama kodu giriniz",
                digits: "Rakamlardan oluşmalıdır",
                minlength: "6 haneli olmalıdır"
            }
        }
    });

    $("#mdlCreateWithdrawalRequestBtn").on("click", function () {
        $("#CompanyCreateWithdrawalRequest_ConfirmCode").attr("disabled", true);
        $("#mdlCompanyCreateWithdrawalRequestSendCode").attr("disabled", true);
        $("#mdlCreateWithdrawalRequest").modal('toggle');
    });

    $('#mdlCreateWithdrawalRequest').on('hidden.bs.modal', function () {
        $(this).find('form').trigger('reset');
    });

    $("#CompanyCreateWithdrawalRequest_ConfirmCode").numeric_format({
        defaultvalue: "",
        thousandslimit: 6,
        centsLimit: 0,
        centsSeparator: ',',
        thousandsSeparator: ',',
        clearOnEmpty: true
    });

    $("#CompanyCreateWithdrawalRequest_Amount").removeAttr("value");

    function checkInputs() {
        var amountValue = parseFloat($("#CompanyCreateWithdrawalRequest_Amount").val().replace(',', '.'));
        var currencyCostTotalValue = $("#CompanyCreateWithdrawalRequest_CurrencyCostTotal").val();

        if (amountValue >= 0.01 && currencyCostTotalValue !== "") {
            $("#mdlCompanyCreateWithdrawalRequestSendCode").attr("disabled", false);
        } else {
            $("#mdlCompanyCreateWithdrawalRequestSendCode").attr("disabled", true);
        }
    }

    $("#CompanyCreateWithdrawalRequest_Amount").on({
        keyup: function () {
            formatCurrency($(this));
            checkInputs(); 
        },
        blur: function () {
            formatCurrency($(this), "blur");
            checkInputs(); 
        }
    });

    $("#CompanyCreateWithdrawalRequest_CurrencyCostTotal").on({
        keyup: function () {
            formatCurrency($(this));
            checkInputs(); 
        },
        blur: function () {
            formatCurrency($(this), "blur");
            checkInputs();
        }
    });

    $("#mdlCompanyCreateWithdrawalRequestSendCode").click(function () {
        var amount = $("#CompanyCreateWithdrawalRequest_Amount").val();
        var currency = $("#CompanyCreateWithdrawalRequest_CurrencyCode").val();
        var costTotal = $("#CompanyCreateWithdrawalRequest_CurrencyCostTotal").val();
        var companyName = $("#companyName").val();
        var total = parseFloat($("#CompanyCreateWithdrawalRequest_Amount").val().replace(',', '.')) + parseFloat($("#CompanyCreateWithdrawalRequest_CurrencyCostTotal").val().replace(',', '.'))

        var message = `${companyName} üye işyeri adına Tutar: ${amount} - İşlem Ücreti: ${costTotal} - Para Birimi: ${currency} - Toplam: ${total} ${currency} tutarındaki çekim bildirimi için `
        $.post("/Dealer/SendConfirmSms", { "operationType": "CompanyCreateWithdrawalRequestSendCode", "message" : message, "phone": "05452162199" }, function (response) {
            if (response.status === "OK") {
                alertify.success('İşlem başarılı..');
                $("#CompanyCreateWithdrawalRequest_ConfirmCode").removeAttr("disabled");
                //$("button[type=submit").removeClass("d-none");
                $("#createWithdrawalRequestSubmitBtn").removeClass("d-none");
            }
            else
                alertify.error('Opps! Hata oluştu..');
        })
    });
});

function formatNumber(n) {
    return n.replace(/\D/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ".")
};
function formatCurrency(input, blur) {
    // appends $ to value, validates decimal side
    // and puts cursor back in right position.

    // get input value
    var input_val = input.val();

    // don't validate empty input
    if (input_val === "") { return; }

    // original length
    var original_len = input_val.length;

    // initial caret position 
    var caret_pos = input.prop("selectionStart");

    // check for decimal
    if (input_val.indexOf(",") >= 0) {

        // get position of first decimal
        // this prevents multiple decimals from
        // being entered
        var decimal_pos = input_val.indexOf(",");

        // split number by decimal point
        var left_side = input_val.substring(0, decimal_pos);
        var right_side = input_val.substring(decimal_pos);

        // add commas to left side of number
        left_side = formatNumber(left_side);

        // validate right side
        right_side = formatNumber(right_side);

        // On blur make sure 2 numbers after decimal
        if (blur === "blur") {
            right_side += "00";
        }

        // Limit decimal to only 2 digits
        right_side = right_side.substring(0, 2);

        // join number by .
        input_val = left_side + "," + right_side;

    } else {
        // no decimal entered
        // add commas to number
        // remove all non-digits
        input_val = formatNumber(input_val);

        // final formatting
        if (blur === "blur") {
            input_val += ",00";
        }
    }

    // send updated string to input
    input.val(input_val);

    // put caret back in the right position
    var updated_len = input_val.length;
    caret_pos = updated_len - original_len + caret_pos;
    input[0].setSelectionRange(caret_pos, caret_pos);
};

function onUsingBalance(response) {
    setTimeout(function () {
        if (response.status === "ERROR") {
            alertify.notify(response.message, 'error', 5, function () { }).dismissOthers();
            hideLoading();
        }
        else
            window.location.reload();
    }, 1000);
}

function onSuccessCreditCardPaymentMethod(response) {
    if (response.status === "ERROR") {
        alertify.notify(response.message, 'error', 5, function () { }).dismissOthers();
        hideLoading();
        if (!$('#creditCardToggle').is(':checked')) {
            $('#paramToggle').bootstrapToggle('off')
            $('#payNKolayToggle').bootstrapToggle('off')
        };

        if ($("#ForeignCreditCardPayNKolayToggle").is(':checked')) {
            $('#ForeignCreditCardPayNKolayToggle').bootstrapToggle('off');
        };

    } else {
        if ($('#paramToggle').is(':checked') && !$('#payNKolayToggle').is(':checked')) {
            $('#payNKolayToggle').attr("disabled", "disabled");
        }
        if (!$('#paramToggle').is(':checked') && $('#payNKolayToggle').is(':checked')) {
            $('#paramToggle').attr("disabled", "disabled");
        }
        if (!$('#paramToggle').is(':checked') && !$('#payNKolayToggle').is(':checked')) {
            $('#paramToggle, #payNKolayToggle').removeAttr("disabled", "disabled");
        }
    }
}

function onSuccessIframeUseSettings(response) {
    if (response.status === "ERROR") {
        alertify.notify(response.message, 'error', 5, function () { }).dismissOthers();
        hideLoading();
    } else {
        if (!$('#creditCardToggle').is(':checked')) {
            $('#paramToggle').bootstrapToggle('off')  
            $('#payNKolayToggle').bootstrapToggle('off')  
        };
    }
}

function onSuccessCreateWithdrawal(response) {
    if (response.status === "ERROR") {
        alertify.notify(response.message, 'error', 5, function () { }).dismissOthers();
        hideLoading();
    } else {
        window.location.reload();
    }
}

//function isExchangeCreditCardPanelChange(selectElement) {
//    const selectedValue = selectElement.value;
//    if (selectedValue === 'true') {
//        $("#visibleCurrencyCode").removeAttr("hidden");
//    } else {
//        $("#visibleCurrencyCode").attr("hidden", true);
//    }
//}


function onFailureTransfer() {
    alertify.notify('Opps! Birşeyler ters gitti.', 'error', 10, function () { }).dismissOthers();
}

function onSuccessTransfer(response) {
    if (response.status === "ERROR") {
        hideLoading();
        alertify.notify(response.message, 'error', 5, function () { }).dismissOthers();
    } else {
        window.location.reload();
    }
}