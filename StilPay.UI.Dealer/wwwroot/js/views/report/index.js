var urlEDIT = '';
var urlDELETE = '';
var urlLIST = '/AccountSummary';
$(document).ready(function () {

    initDatePicker();

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
        url: '/Report/AccountSummaryPartialView',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({
            StartDate: $("#dtStartDate").val(),
            EndDate: $("#dtEndDate").val(),
            StartDateTime: $("#dtStartDateTime").val(),
            EndDateTime: $("#dtEndDateTime").val(),
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
        }
    });
}
