var urlEDIT = '';
var urlDELETE = '';
var urlLIST = '/AccountSummary';
$(document).ready(function () {

    initDatePicker();
    initCompanySelectionWithoutAll();

    $("#btnList").click(function () {
        getData();
    });

    $("#btnInvoice").click(function () {
        createCompanyInvoice();
    });
});

function getData() {
    $.ajax({
        type: "POST",
        url: '/AccountSummary/DealerAccountSummaryPartialView',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({
            StartDate: $("#dtStartDate").val(),
            EndDate: $("#dtEndDate").val(),
            StartDateTime: $("#dtStartDateTime").val(),
            EndDateTime: $("#dtEndDateTime").val(),
            IDCompany: $("#slcCompanies").val(),
        }),
        beforeSend: function () {
            showLoading();
        },
        success: function (response) {
            if (response.status == "ERROR") {
                alertify.notify(response.message, 'error', 5, function () { }).dismissOthers();
            } else {
                $('#partialViewContainer').html(response);
            }
        },
        error: function (response) {
            alertify.notify(response.message, 'error', 5, function () { }).dismissOthers();
        },
        complete: function () {
            hideLoading();
            //pageOpened = false;
        }
    });
}


function createCompanyInvoice() {
    $.ajax({
        type: "POST",
        url: '/AccountSummary/CreateCompanyInvoice',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(dealerAccountSummaryModel),
        beforeSend: function () {
            showLoading();
        },
        success: function (response) {
            if (response.status == "ERROR") {
                alertify.notify(response.message, 'error', 5, function () { }).dismissOthers();
            } else {
                alertify.notify("İşlem Başarılı", 'success', 5, function () { }).dismissOthers();
            }
        },
        error: function (response) {
            alertify.notify(response.message, 'error', 5, function () { }).dismissOthers();
        },
        complete: function () {
            hideLoading();
            //pageOpened = false;
        }
    });
}