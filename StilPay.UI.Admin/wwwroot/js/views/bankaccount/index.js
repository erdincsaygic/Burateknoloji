var urlEDIT = '/AccountingReport/EditBankAccount/__id__';
var urlDELETE = '/AccountingReport/DropBankAccount';
var urlLIST = '/AccountingReport/BankTransactions';

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
    //$("#btnExpense").click(function () {
    //    Table2.expense();
    //});

    $("#getBankBalancesLogBtn").click(function () {
        getBankBalancesLogData();
    });

});

var Table = function () {

    var initTable = function () {

        var table = $('#Table');

        var otable = table.dataTable({

            "language": {
                "emptyTable": "Gösterilecek Veri Yok",
                "info": "Toplam _TOTAL_ veriden _START_ ile _END_ arasındaki veriler gösteriliyor",
                "infoEmpty": "",
                "infoFiltered": "",
                "lengthMenu": "_MENU_ Veri Göster",
                "search": "Ara:",
                "zeroRecords": "Eşleşen Veri Yok",
                "paginate": {
                    "previous": "Geri",
                    "next": "İleri"
                }
            },

            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                nRow.setAttribute("id", "tr_" + aData[7]);
            },

            "columnDefs": [
                {
                    "searchable": false,
                    "targets": [6, 7]
                },
                {
                    "aTargets": [0],
                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                        $(nTd).attr('class', 'text-center');
                        if (sData === true)
                            $(nTd).html(sData = '<span class="badge bg-info px-4 py-2">AKTİF</span>');
                        else
                            $(nTd).html(sData = '<span class="badge bg-secondary px-4 py-2">PASİF</span>');
                    }
                },
                {
                    "aTargets": [1],
                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    }
                },
                {
                    "aTargets": [2],
                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                        $(nTd).attr('class', 'text-center');
                        if (sData === true)
                            $(nTd).html(sData = '<span class="badge bg-danger px-4 py-2">GİDER HESABI</span>');
                        else
                            $(nTd).html(sData = '<span class="badge bg-success px-4 py-2">GELİR HESABI</span>');
                    }
                },
                {
                    "aTargets": [3],
                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                        $(nTd).attr('class', 'text-center');
                        if (sData === true)
                            $(nTd).html(sData = '<span class="badge bg-success px-4 py-2">EVET</span>');
                        else
                            $(nTd).html(sData = '<span class="badge bg-danger px-4 py-2">HAYIR</span>');
                    }
                },
                {
                    "aTargets": [4],
                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                        $(nTd).attr('class', 'text-center');
                    }
                },
                {
                    "aTargets": [5],
                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    }
                },
                {
                    "aTargets": [6],
                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    }
                },
                {
                    "aTargets": [7],
                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    }
                },
                {
                    "aTargets": [8],
                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                        $(nTd).attr('class', 'text-center');
                        $(nTd).html(sData = initBtnOperations(sData, false, true));
                    }
                },
                {
                    "aTargets": [9],
                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                        $(nTd).attr('class', 'd-none');
                    }
                }
            ],

            "order": [],

            "lengthMenu": [
                [15, 25, 50, 100, -1],
                [15, 25, 50, 100, "Tümü"]
            ],

            "columns": [
                {
                    "orderable": true
                },
                {
                    "orderable": true
                },
                {
                    "orderable": true
                },
                {
                    "orderable": true
                },
                {
                    "orderable": true
                },
                {
                    "orderable": true
                },
                {
                    "orderable": true
                },
                {
                    "orderable": false
                },
                {
                    "orderable": false
                }
            ]
        });

        var tableWrapper = $('#Table_wrapper');
        tableWrapper.find('.dataTables_length select').select2({
            minimumResultsForSearch: Infinity,
        });
    }

    var initData = function () {
        showLoading();

        $.get('/AccountingReport/GetBankAccount/', function (list) {
            var otbl = $('#Table').DataTable();
            if (list)
                $.each(list, function (i, item) {
                    otbl.rows.add([[item.statusFlag, item.name, item.isExitAccount, item.showInSystemSettings, 'TRY', item.bank, item.title, item.iban, item.id, item.id]]);
                })
            otbl.draw();
        }).fail(function () {
            onFailure();
        }).always(function () {
            hideLoading();
        });
    }

    return {
        init: function () {
            if (!jQuery().dataTable) {
                return;
            }
            initTable();
            initData();
        }
    };


}();

var Table2 = function () {


    var expenseData = function () {
        $.ajax({
            type: "POST",
            url: '/AccountingReport/CompanyFinanceTransferInsert/',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({
                IDCompany: "",
                Amount: 0,
                Description: "",
                PaymentMethod: 1,
                TransactionType: 1,
                TransactionDetailType: 1,
                TransactionDate: null
            }),
            beforeSend: function () {
                showLoading();
            },
            success: function (list) {


            },
            error: function () {
                onFailure();
            },
            complete: function () {
                hideLoading();
                $("#mdlExpense").modal("hide");
            }
        });
    }

    return {
        expense: function () {
            expenseData();
        }
    };

}();

function getBankBalancesLogData() {
    $('#bankBalanceLogsTable').DataTable({
        destroy: true,
        serverSide: true,
        processing: true,
        "language": {
            "emptyTable": "Gösterilecek Veri Yok",
            "info": "Toplam _TOTAL_ veriden _START_ ile _END_ arasındaki veriler gösteriliyor",
            "infoEmpty": "",
            "infoFiltered": "",
            "lengthMenu": "_MENU_ Veri Göster",
            "search": "Ara:",
            "zeroRecords": "Eşleşen Veri Yok",
            "paginate": {
                "previous": "Geri",
                "next": "İleri"
            }
        },
        "ajax": {
            "url": "/AccountingReport/GetBankBalancesLogData",
            "type": "POST",
            "data": function (d) {
                d.StartDate = $("#dtStartDate").val();
                d.EndDate = $("#dtEndDate").val();
            },
        },
        "initComplete": function (settings, json) {
            $('#bankBalanceLogsTable').removeAttr('style'); 
        },
        "columnDefs": [{
            "targets": [5],
            "searchable": false
        }],
        "columns": [
            { "data": "cDate", "orderable": false },
            { "data": "creator", "orderable": false },
            { "data": "bank", "orderable": false },
            { "data": "isExitAccount", "orderable": false },
            { "data": "iban", "orderable": false },
            { "data": "balance", "orderable": false }
        ],

        "columnDefs": [
            {
                "aTargets": [0],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center col-1');
                    $(nTd).html(sData = formatDateTime(sData));
                }
            },
            {
                "aTargets": [1],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center col-1');
                }
            },
            {
                "aTargets": [2],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center col-1');
                }
            },
            {
                "aTargets": [3],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center col-1');
                    if (sData === true)
                        $(nTd).html(sData = '<span class="badge bg-danger px-4 py-2">GİDER HESABI</span>');
                    else
                        $(nTd).html(sData = '<span class="badge bg-success px-4 py-2">GELİR HESABI</span>');
                }
            },
            {
                "aTargets": [4],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center col-2');
                }
            },
            {
                "aTargets": [5],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center col-1');
                    $(nTd).html(sData.toFixed(2));
                }
            }
        ],

        "order": [],

        "lengthMenu": [
            [10, 20, 30, -1],
            [10, 20, 30, "Tümü"]
        ],
    });
}

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

function onSuccessGetBankBalance(response) {
    getBankBalancesLogData();
    if (response.status === "ERROR") {
        alertify.notify(response.message, 'error', 5, function () { }).dismissOthers();

    }
    else {
        alertify.notify('İşlem Başarılı.', 'success', 5, function () { }).dismissOthers();
    }
};

function idBankChange(idBankId) {

    $("#idBank1").val(idBankId);
    $("#idBank2").val(idBankId);
    $("#idBank3").val(idBankId);
    $("#idBank4").val(idBankId);

};