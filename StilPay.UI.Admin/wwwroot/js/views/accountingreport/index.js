
$(document).ready(function () {
    initDatePicker();
    $("#btnList").click(function () {
        Table.init();
    });

    Table.init();
});

function HideShow() {
    $("#idFilter").slideToggle(500);
}

var isInit = false;
var mainData = [];
var _list = [];

var Table = function () {

    var initData = function () {
        $.ajax({
            type: "POST",
            url: '/AccountingReport/Gets/',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({
                StartDate: $("#dtStartDate").val(),
                EndDate: $("#dtEndDate").val(),
                StartDateTime: $("#dtStartDateTime").val(),
                EndDateTime: $("#dtEndDateTime").val()
            }),
            beforeSend: function () {
                showLoading();
            },
            success: function (list) {

                _totalCiro = 0, _totalProfit = 0, _transferTotalAmount = 0, _transferCommissionProfitAmount = 0, _transferDealerPaidAmount = 0, _TransferNotMatchedAmount = 0,
                _creditCardTotalAmount = 0, _creditCardCommissionProfitAmount = 0,  _creditCardPaymentInstitutionPaidAmount = 0, _creditCardDealerPaidAmount = 0,
                _withdrawalTotalAmount = 0, _withdrawalEftAmount = 0, _withdrawalTotalPaidAmount = 0, _withdrawalTotalTransactionFeeAmount = 0, _withdrawalFastAmount = 0,
                    _totalIncomeAmount = 0, _totalExpenseAmount = 0, _rebateExpenseProfitAmount = 0, _fraudExpenseProfitAmount = 0;

                _totalCiro = list.transferTotalAmount + list.creditCardTotalAmount + list.withdrawalTotalAmount + list.foreignCreditCardTotalAmount,
                _totalProfit = list.transferCommissionProfitAmount + list.creditCardCommissionProfitAmount + list.withdrawalTotalTransactionFeeAmount + list.foreignCreditCardCommissionProfitAmount,

                _transferTotalAmount = list.transferTotalAmount,
                _transferCommissionProfitAmount = list.transferCommissionProfitAmount,
                _transferDealerPaidAmount = list.transferDealerPaidAmount,
                _TransferNotMatchedAmount = list.transferNotMatchedAmount,

                _creditCardTotalAmount = list.creditCardTotalAmount,
                _creditCardCommissionProfitAmount = list.creditCardCommissionProfitAmount,
                _creditCardPaymentInstitutionPaidAmount = list.creditCardPaymentInstitutionPaidAmount,
                _creditCardDealerPaidAmount = list.creditCardDealerPaidAmount,

                _foreignCreditCardTotalAmount = list.foreignCreditCardTotalAmount,
                _foreignCreditCardCommissionProfitAmount = list.foreignCreditCardCommissionProfitAmount,
                _foreignCreditCardPaymentInstitutionPaidAmount = list.foreignCreditCardPaymentInstitutionPaidAmount,
                _foreignCreditCardDealerPaidAmount = list.foreignCreditCardDealerPaidAmount,

                _withdrawalTotalAmount = list.withdrawalTotalAmount,
                _withdrawalEftAmount = list.withdrawalEftAmount,
                _withdrawalTotalPaidAmount = list.withdrawalTotalPaidAmount,
                _withdrawalTotalTransactionFeeAmount = list.withdrawalTotalTransactionFeeAmount,
                _withdrawalFastAmount = list.withdrawalFastAmount,

                _rebateExpenseProfitAmount = list.rebateExpenseProfitAmount
                _fraudExpenseProfitAmount = list.fraudExpenseProfitAmount
                _totalIncomeAmount = list.totalIncomeAmount,
                _totalExpenseAmount = list.totalExpenseAmount;

                
                $("#divTotalCiro").text(_totalCiro.toFixed(2).replace(".", ",") + ' TL');
                $("#divTotalProfit").text(_totalProfit.toFixed(2).replace(".", ",") + ' TL');

                $("#divTransferTotalAmount").text(_transferTotalAmount.toFixed(2).replace(".", ",") + ' TL');
                $("#divTransferCommissionProfitAmount").text(_transferCommissionProfitAmount.toFixed(2).replace(".", ",") + ' TL');
                $("#divTransferDealerPaidAmount").text(_transferDealerPaidAmount.toFixed(2).replace(".", ",") + ' TL');
                $("#divTransferNotMatchedAmount").text(_TransferNotMatchedAmount.toFixed(2).replace(".", ",") + ' TL');

                $("#divCreditCardTotalAmount").text(_creditCardTotalAmount.toFixed(2).replace(".", ",") + ' TL');
                $("#divCreditCardCommissionProfitAmount").text(_creditCardCommissionProfitAmount.toFixed(2).replace(".", ",") + ' TL');
                $("#divCreditCardDealerPaidAmount").text(_creditCardDealerPaidAmount.toFixed(2).replace(".", ",") + ' TL');
                $("#divCreditCardPaymentInstitutionPaidAmount").text(_creditCardPaymentInstitutionPaidAmount.toFixed(2).replace(".", ",") + ' TL');

                $("#divForeignCreditCardTotalAmount").text(_foreignCreditCardTotalAmount.toFixed(2).replace(".", ",") + ' TL');
                $("#divForeignCreditCardCommissionProfitAmount").text(_foreignCreditCardCommissionProfitAmount.toFixed(2).replace(".", ",") + ' TL');
                $("#divForeignCreditCardDealerPaidAmount").text(_foreignCreditCardDealerPaidAmount.toFixed(2).replace(".", ",") + ' TL');
                $("#divForeignCreditCardPaymentInstitutionPaidAmount").text(_foreignCreditCardPaymentInstitutionPaidAmount.toFixed(2).replace(".", ",") + ' TL');
                
                $("#divWithdrawalTotalAmount").text(_withdrawalTotalAmount.toFixed(2).replace(".", ",") + ' TL');
                $("#divWithdrawalTotalPaidAmount").text(_withdrawalTotalPaidAmount.toFixed(2).replace(".", ",") + ' TL');
                $("#divWithdrawalEftAmount").text(_withdrawalEftAmount.toFixed(2).replace(".", ",") + ' TL');
                $("#divWithdrawalFastAmount").text(_withdrawalFastAmount.toFixed(2).replace(".", ",") + ' TL');
                $("#divWithdrawalTotalTransactionFeeAmount").text(_withdrawalTotalTransactionFeeAmount.toFixed(2).replace(".", ",") + ' TL');

                $("#divTotalRebateExpenseProfitAmount").text(_rebateExpenseProfitAmount.toFixed(2).replace(".", ",") + ' TL');

                $("#divTotalFraudExpenseProfitAmount").text(_fraudExpenseProfitAmount.toFixed(2).replace(".", ",") + ' TL');

                $("#divTotalExpenseAmount").text(_totalExpenseAmount.toFixed(2).replace(".", ",") + ' TL');
                $("#divTotalIncomeAmount").text(_totalIncomeAmount.toFixed(2).replace(".", ",") + ' TL');


                
                //$("#divCommission").text(_commission.toFixed(2) + ' / ' + _commissionCount + ' Ad');
            },
            error: function () {
                onFailure();
            },
            complete: function () {
                hideLoading();
            }
        });
    }

    return {
        init: function () {

            initData();
        }
    };

}();
