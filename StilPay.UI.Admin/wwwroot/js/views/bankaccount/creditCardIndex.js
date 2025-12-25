var urlLIST = '/AccountingReport/CreditCardTransactions';

$(document).ready(function () {


    Table.init();

    $("#frmDef").validate({
        errorElement: 'div',
        errorClass: 'text-danger',
        rules: {
            "entity.Amount": {
                required: true
            },
            "entity.TransactionDetailType": {
                required: true
            }
        },
        messages: {
            "entity.Amount": {
                required: "Lütfen tutar giriniz"
            },
            "entity.TransactionDetailType": {
                required: "Lütfen tip seçiniz"
            }
        }
    });

});

var Table = function () {

    var initTable = function () {

        var table = $('#Table');

        var tableWrapper = $('#Table_wrapper');
        tableWrapper.find('.dataTables_length select').select2({
            minimumResultsForSearch: Infinity,
        });
    }


    return {
        init: function () {
            if (!jQuery().dataTable) {
                return;
            }
            initTable();

        }
    };


}();



function onSuccess(response) {
    if (response.status === "ERROR") {
        $("#mdlConfirm").modal('hide');
        alertify.notify(response.message, 'error', 5, function () { }).dismissOthers();

    }
    else {
        if (urlLIST)
            window.location.href = urlLIST;
        else
            alertify.notify('İşlem Başarılı.', 'success', 5, function () { }).dismissOthers();
    }
};

function idBankChange(idBankId) {
    $("#CompanyIntegrationType").val(idBankId);
    $("#CompanyIntegrationType").val(idBankId);
};


function paymentInstitutionChange(pyId, hour) {

    $("#paymentInstitutionId").val(pyId);
    $("#pyEndofdaytime").val(hour);
};



function change2(idBankId) {

        //$.ajax({
        //    type: "POST",
        //    url: 'CreditCardTransactionsDetail/',
        //    contentType: 'application/json; charset=utf-8',
        //    data: JSON.stringify({
        //        StartDate: $("#dtStartDate").val(),
        //        EndDate: $("#dtEndDate").val()
        //    }),
        //    beforeSend: function () {
        //        showLoading();

        //        var otbl = $('#Table').DataTable();
        //        otbl.clear().draw();
        //        mainData = [];
        //    },
        //    success: function (list) {
        //        var grid = [];
        //        if (list)
        //            $.each(list, function (i, item) {
        //                grid.push([item.transactionDate, item.amount]);
        //                mainData.push({
        //                    İşlemTarihi: item.transactionDate,
        //                    Tutar: item.amount.toFixed(2)
        //                    //İşlemTarihi: formatDateTime(item.cDate),
        //                    //İşlemNumarası: item.transactionNr,
        //                    //Üyeİşyeri: item.company,
        //                    //KartNumarası: item.cardNumber,
        //                    //Telefon: item.phone,
        //                    //İşlemTutarı: item.amount.toFixed(2),
        //                    //KomistonOranı: item.commissionRate,
        //                    //İşlemÜcreti: item.commission,
        //                    //NetToplam: item.netTotal,
        //                    //Durum: StatusToText(item.status),
        //                    //Yetkili: item.modifier ?? ''
        //                });
        //            });

        //        var otbl = $('#Table').DataTable();
        //        otbl.rows.add(grid);
        //        otbl.draw();
        //    },
        //    error: function () {
        //        onFailure();
        //    },
        //    complete: function () {
        //        hideLoading();
        //    }
        //});


};