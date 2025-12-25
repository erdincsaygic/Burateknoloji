var urlLIST = '/WithdrawalRequest/Index';

const bankAccounts = $.parseJSON($("#jsonBankAccounts").val());
const transferCommissionAmount = $.parseJSON($("#jsonTransferAmount").val());
const eftCommissionAmount = $.parseJSON($("#jsonEftAmount").val());
const foreignCurrencySwiftAmount = $.parseJSON($("#jsonForeignCurrencySwiftAmount").val());


$(document).ready(function () {
    $("#frmDef").validate({
        errorElement: 'div',
        errorClass: 'text-danger',
        rules: {
            "entity.IDBank": {
                required: true
            },
            "entity.IBAN": {
                required: true,
                minlength: 30
            },
            "entity.Title": {
                required: true
            }
        },
        messages: {
            "entity.IDBank": {
                required: "Lütfen banka seçiniz"
            },
            "entity.IBAN": {
                required: "Lütfen IBAN giriniz",
                minlength: "Uzunluk 24 haneli olmalıdır"
            },
            "entity.Title": {
                required: "Lütfen hesap sahibi giriniz"
            }
        }
    })

    $("#entity_IBAN").mask('00 0000 0000 0000 0000 0000 00')

    $("#entity_Amount").numeric_format({
        defaultvalue: "0,00",
        thousandslimit: 7,
        centsLimit: 2,
        centsSeparator: ',',
        thousandsSeparator: '.',
        clearOnEmpty: false
    })

    $("#slcBankAccount").change(function (e) {
        if (e.target.value) {
            for (var i = 0; i < bankAccounts.length; i++) {
                var bankAccount = bankAccounts[i];
                if (e.target.value === bankAccount.ID) {
                    $("#entity_IDBank").val(bankAccount.IDBank);
                    $("#entity_IBAN").val(bankAccount.IBAN.replace("TR", ""));
                    $("#entity_Title").val(bankAccount.Title);

                    $("#entity_IBAN").trigger('blur');
                    $("#entity_Title").trigger('blur');
                    break;
                }
            }
        }
        else {
            $("#entity_IDBank").val("");
            $("#entity_IBAN").val("");
            $("#entity_Title").val("");

            $("#entity_IBAN").trigger('blur');
            $("#entity_Title").trigger('blur');
        }
    });

    $("#entity_IDBank").val("");
    $("#entity_IBAN").val("");
    $("#entity_Title").val("");

    $("#rdEFT").prop("checked", true)

    $("#btnSubmit").click(function () {
        if ($("#frmDef").valid()) {
            $("#txtDetailBank").val($("#entity_IDBank option:selected").text());
            $("#txtDetailIBAN").val('TR' + $("#entity_IBAN").val());
            $("#txtDetailTitle").val($("#entity_Title").val());
            $("#txtDetailAmount").val($("#entity_Amount").val());
            $("#txtDetailType").val($("#transferType").hasClass("d-none") ? 'Swift' : $("input[name='entity.IsEFT']:checked").val().toString() === 'true' ? 'Havale/EFT' : 'FAST');
            $("#txtCommissionAmount").val($("#entity_CostTotal").val());
            $("#txtDealerDescription").val($("#entity_DealerDescription").val());
            $("#txtCurrencyCode").val($("#costCurrency").text());
            $("#mdlDetail").modal('show');
        }
    });

    if ($("input[name='entity.IsEFT']:checked").val().toString() === 'true') {
        $("#entity_CostTotal").val(transferCommissionAmount.toFixed(2).replace(".",","))
    } 
    else {
        $("#entity_CostTotal").val(eftCommissionAmount.toFixed(2).replace(".", ","))
    }

    $("input[name='entity.IsEFT']").change(function () {

        if (document.getElementById('costCurrency').textContent == "TRY") {
            if (this.value == 'true') {
                $("#entity_CostTotal").val(transferCommissionAmount.toFixed(2).replace(".", ","))
            }
            else {
                $("#entity_CostTotal").val(eftCommissionAmount.toFixed(2).replace(".", ","))
            }
        } else {
            if (this.value == 'true') {
                $("#entity_CostTotal").val(foreignCurrencyTransferAmount.toFixed(2).replace(".", ","))
            }
            else {
                $("#entity_CostTotal").val(foreignCurrencySwiftAmount.toFixed(2).replace(".", ","))
            }
        }

    });

    $("#btnOK").click(function () {
        $("#mdlDetail").modal('hide');
        $("#frmDef").submit();
    })

    $('#entity_IBAN, #entity_Title').on('blur', function () {
        var iban = $('#entity_IBAN').val();
        var title = $('#entity_Title').val();

        if (iban && title) {
            $.ajax({
                url: '/WithdrawalRequest/CheckIbanAndTitle',
                type: 'POST',
                data: { iban: iban, title: title },
                success: function (response) {
                    if (response.match) {
                        $('#entity_CostTotal').val(0);
                    } else {
                        if ($("input[name='entity.IsEFT']:checked").val().toString() === 'true') {
                            $("#entity_CostTotal").val(transferCommissionAmount.toFixed(2).replace(".", ","))
                        }
                        else {
                            $("#entity_CostTotal").val(eftCommissionAmount.toFixed(2).replace(".", ","))
                        }

                    }
                }
            });
        } else {
            if ($("input[name='entity.IsEFT']:checked").val().toString() === 'true') {
                $("#entity_CostTotal").val(transferCommissionAmount.toFixed(2).replace(".", ","))
            }
            else {
                $("#entity_CostTotal").val(eftCommissionAmount.toFixed(2).replace(".", ","))
            }
        }
    });
});


document.addEventListener('DOMContentLoaded', function () {
    var currencySelect = document.getElementById('currencySelect');
    var amountCurrency = document.getElementById('amountCurrency');
    var costCurrency = document.getElementById('costCurrency');

    // Update currency code based on selection
    currencySelect.addEventListener('change', function () {
        var selectedCurrency = currencySelect.options[currencySelect.selectedIndex].value;
        amountCurrency.textContent = selectedCurrency;
        costCurrency.textContent = selectedCurrency;

        if (selectedCurrency != "TRY") {

            $("#transferType").addClass("d-none");
            $("#entity_CostTotal").val(foreignCurrencySwiftAmount.toFixed(2).replace(".", ","))

        } else {
            $("#transferType").removeClass("d-none");
            if ($("input[name='entity.IsEFT']:checked").val().toString() === 'true') {
                $("#entity_CostTotal").val(transferCommissionAmount.toFixed(2).replace(".", ","))
            }
            else {
                $("#entity_CostTotal").val(eftCommissionAmount.toFixed(2).replace(".", ","))
            }
        }

    });


    // Set initial currency
    amountCurrency.textContent = currencySelect.value;
    costCurrency.textContent = currencySelect.value;
});

