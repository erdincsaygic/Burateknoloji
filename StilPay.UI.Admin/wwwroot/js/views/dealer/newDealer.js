var urlLIST = '/dealer/index';

$(document).ready(function () {
    $("#frmDef").validate({
        errorElement: 'span',
        errorClass: 'text-danger',
        rules: {
            "Name": {
                required: true,
                minlength: 2
            },
            "Phone": {
                required: true,
                digits: true,
                minlength: 11
            },
            "Title": {
                required: true,
                minlength: 2
            },
            "Password": {
                required: true,
                minlength: 4
            },
            "TaxNr": {
                required: true,
                digits: true,
                minlength: 10
            },
            "TaxOffice": {
                required: true,
                minlength: 2
            },
            "Address": {
                required: true,
                minlength: 2
            },
            "MonthlyGiro": {
                required: true
            },
            "Email": {
                required: true,
                email: true
            },
            "SiteUrl": {
                required: true
            },
            "RedirectUrl": {
                required: true
            },
            "CallbackUrl": {
                required: true
            },
            "WithdrawalRequestCallBack": {
                required: true
            },
            "IPAddress": {
                required: true
            },
            "AutoTransferLimit": {
                required: true
            },
            "AutoWithdrawalLimit": {
                required: true
            },
            "AutoCreditCardLimit": {
                required: true
            },
            "AutoForeignCreditCardLimit": {
                required: true
            },
            "CreditCardRate": {
                required: true
            },
            "ForeignCreditCardRate": {
                required: true
            },
            "TransferRate": {
                required: true
            },
            "MobilePayRate": {
                required: true
            },
            "WithdrawalTransferAmount": {
                required: true
            },
            "WithdrawalEftAmount": {
                required: true
            }
        },
        messages: {
            "Name": {
                required: "Lütfen ad soyad giriniz",
                minlength: "Uzunluk en az 2 haneli olmalıdır"
            },
            "Phone": {
                required: "Lütfen gsm numarası giriniz",
                digits: "Gsm no rakamlardan oluşmalıdır",
                minlength: "Uzunluk 11 haneli olmalıdır"
            },
            "Title": {
                required: "Lütfen ünvan giriniz",
                minlength: "Uzunluk en az 2 haneli olmalıdır"
            },
            "Password": {
                required: "Lütfen şifre giriniz",
                minlength: "Uzunluk en az 4 haneli olmalıdır"
            },
            "TaxNr": {
                required: "Lütfen vergi no giriniz",
                digits: "Vergi no rakamlardan oluşmalıdır",
                minlength: "Uzunluk en az 10 haneli olmalıdır"
            },
            "TaxOffice": {
                required: "Lütfen vergi dairesi giriniz",
                minlength: "Uzunluk en az 2 haneli olmalıdır"
            },
            "Address": {
                required: "Lütfen adres giriniz",
                minlength: "Uzunluk en az 2 haneli olmalıdır"
            },
            "MonthlyGiro": {
                required: "Lütfen ortalama aylık ciro seçiniz"
            },
            "Email": {
                required: "Lütfen email giriniz",
                email: "Geçerli bir email giriniz"
            },
            "SiteUrl": {
                required: "Lütfen site adresi giriniz"
            },
            "RedirectUrl": {
                required: "Lütfen yönlendirilecek adresi giriniz"
            },
            "CallbackUrl": {
                required: "Lütfen callback url giriniz",
            },
            "WithdrawalRequestCallBack": {
                required: "Lütfen çekim talebi callback url giriniz"
            },
            "IPAddress": {
                required: "Lütfen ip adresi giriniz"
            },
            "AutoTransferLimit": {
                required: "Lütfen havale/eft transfer limiti giriniz"
            },
            "AutoWithdrawalLimit": {
                required: "Lütfen çekim talebi limiti giriniz"
            },
            "AutoCreditCardLimit": {
                required: "Lütfen kredi kartı ödeme limiti giriniz"
            },
            "AutoForeignCreditCardLimit": {
                required: "Lütfen yurt dışı kredi kartı ödeme limiti giriniz"
            },
            "CreditCardRate": {
                required: "Lütfen kredi kartı komisyon oranı giriniz"
            },
            "ForeignCreditCardRate": {
                required: "Lütfen yurt dışı kredi kartı komisyon oranı giriniz"
            },
            "TransferRate": {
                required: "Lütfen havale komisyon oranı giriniz"
            },
            "MobilePayRate": {
                required: "Lütfen mobil ödeme komisyon oranı giriniz"
            },
            "WithdrawalTransferAmount": {
                required: "Lütfen çekim talebi havale/eft komisyon tutarı giriniz"
            },
            "WithdrawalEftAmount": {
                required: "Lütfen çekim talebi fast komisyon tutarı giriniz"
            }
        }
    });

    //#region AutoWithdrawalLimit

    $("#AutoWithdrawalLimit").on({
        keyup: function () {
            formatCurrency($(this));
        },
        blur: function () {
            formatCurrency($(this), "blur");
        }
    });
    //#endregion AutoWithdrawalLimit

    //#region AutoTransferlLimit


    $("#AutoTransferLimit").on({
        keyup: function () {
            formatCurrency($(this));
        },
        blur: function () {
            formatCurrency($(this), "blur");
        }
    });

    //#endregion AutoTransferLimit

    //#region AutoCreditCardLimit
   
    $("#AutoCreditCardLimit").on({
        keyup: function () {
            formatCurrency($(this));
        },
        blur: function () {
            formatCurrency($(this), "blur");
        }
    });
    //#endregion AutoCreditCardLimit


    //#region AutoForeignCreditCardLimit
    $("#AutoForeignCreditCardLimit").on({
        keyup: function () {
            formatCurrency($(this));
        },
        blur: function () {
            formatCurrency($(this), "blur");
        }
    });
    //#endregion AutoForeignCreditCardLimit

    $("#CreditCardRate").numeric_format({
        thousandslimit: 2,
        centsLimit: 2,
        centsSeparator: ',',
        thousandsSeparator: ',',
        clearOnEmpty: false
    });

    $("#ForeignCreditCardRate").numeric_format({
        thousandslimit: 2,
        centsLimit: 2,
        centsSeparator: ',',
        thousandsSeparator: ',',
        clearOnEmpty: false
    });

    $("#TransferRate").numeric_format({
        thousandslimit: 2,
        centsLimit: 2,
        centsSeparator: ',',
        thousandsSeparator: ',',
        clearOnEmpty: false
    });

    $("#MobilePayRate").numeric_format({
        thousandslimit: 2,
        centsLimit: 2,
        centsSeparator: ',',
        thousandsSeparator: ',',
        clearOnEmpty: false
    });

    $("#WithdrawalTransferAmount").numeric_format({
        thousandslimit: 2,
        centsLimit: 2,
        centsSeparator: ',',
        thousandsSeparator: ',',
        clearOnEmpty: false
    });

    $("#WithdrawalEftAmount").numeric_format({
        thousandslimit: 2,
        centsLimit: 2,
        centsSeparator: ',',
        thousandsSeparator: ',',
        clearOnEmpty: false
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

function isExchangeCreditCardPanelChange(selectElement) {
    const selectedValue = selectElement.value;
    if (selectedValue === 'true') {
        $("#visibleCurrencyCode").removeAttr("hidden");
    } else {
        $("#visibleCurrencyCode").attr("hidden", true);
    }
}

