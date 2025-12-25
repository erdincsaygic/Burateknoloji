
$(document).ready(function () {
    initCompanySelection();
    initActionTypesSelection();
    initDatePicker();

    $("#btnExcel").click(function () {
        downloadExcel("Faturalandırılmamış-Üye-İşyeri-Hareketleri", mainData);
    });

    $("#btnList").click(function () {
        Table.init();
    });

    Table.init();

    $("#btnFaturalastir").click(function () {
        var rows = $("#Table tbody tr");
        if (rows.length > 0 && rows[0].cells[7] != null) {
            ConvertToInvoice();
        }
        else {
            alertify.notify('İşlem listesi boş!', 'warning', 3, function () { }).dismissOthers();
            return;
        }
            
        
    });
});

function ConvertToInvoice() {
    alertify.dialog('confirm').set({ transition: 'slide' });
    alertify.confirm('Closable: false').set('closable', false).set('labels', { ok: 'EVET', cancel: 'HAYIR' });
    alertify.confirm(
        'UYARI',
        '<p class="text-danger fs-6 fw-bold">Emin misiniz ?</p>',
        function () {
            showLoading();
            
            var idList = [];
            var rows = $("#Table tbody tr");
            
            for (var i = 0; i < rows.length; i++) {
                var row = rows[i];
                var cells = row.cells;
                var id = cells[7].innerHTML;
                idList.push(id);
            }
            
            $.ajax({
                url: "/dealertransaction/converttoinvoice",
                type: "POST",
                data: JSON.stringify(idList),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (result) {
                    if (result) {
                        alertify.success("Faturalandırma işlemi başarıyla tamamlandı.");
                        Table.init();
                    }
                    else {
                        alertify.error("Faturalandırma işlemi başarısız.");
                    }
                },
                error: function (result) {
                    alertify.error("Faturalandırma işlemi başarısız.");
                },
                complete: function () {
                    hideLoading();
                }
            });
            
        },
        function () {
        }
    );
}

var isInit = false;
var mainData = [];
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
                    "targets": [7]
                },
                {
                    "aTargets": [0],
                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                        $(nTd).attr('class', 'text-center');
                        $(nTd).html(sData = formatDateTime(sData));
                    }
                },
                {
                    "aTargets": [1],
                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                        $(nTd).attr('class', 'text-center');
                    }
                },
                {
                    "aTargets": [2],
                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                        if (parseInt(oData[8]) === 10 || parseInt(oData[8]) === 20 || parseInt(oData[8]) === 70)
                            $(nTd).attr('class', 'text-center fw-bold text-success');
                        else
                            $(nTd).attr('class', 'text-center fw-bold text-danger');
                    }
                },
                {
                    "aTargets": [3],
                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    }
                },
                {
                    "aTargets": [4],
                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                        $(nTd).attr('class', 'text-end');
                    }
                },
                {
                    "aTargets": [5],
                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                        $(nTd).attr('class', 'text-end');
                    }
                },
                {
                    "aTargets": [6],
                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                        $(nTd).attr('class', 'text-end');
                    }
                },
                {
                    "aTargets": [7],
                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                        $(nTd).attr('class', 'd-none');
                    }
                }
            ],

            "order": [],

            "pageLength": -1,

            "lengthMenu": [
                [15, 25, 50, 100, -1],
                [15, 25, 50, 100, "Tümü"]
            ],

            "columns": [
                {
                    "orderable": false
                },
                {
                    "orderable": false
                },
                {
                    "orderable": false
                },
                {
                    "orderable": false
                },
                {
                    "orderable": false
                },
                {
                    "orderable": false
                },
                {
                    "orderable": false
                },
                {
                    "orderable": false
                },
            ]
        });

        var tableWrapper = $('#Table_wrapper');
        tableWrapper.find('.dataTables_length select').select2({
            minimumResultsForSearch: Infinity,
        });
    }

    var initData = function () {
        if (!$("#slcCompanies").val() || $("#slcCompanies").val() == "all") {
            alertify.notify('Lütfen üye işyeri seçiniz.', 'warning', 3, function () { }).dismissOthers();
            return;
        }

        if (!$("#slcActionTypes").val()) {
            alertify.notify('Lütfen işlem tipi seçiniz.', 'warning', 3, function () { }).dismissOthers();
            return;
        }

        $.ajax({
            type: "POST",
            url: '/DealerTransaction/GetUnbilledTransactions/',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({
                IDCompany: $("#slcCompanies").val(),
                IDActionType: $("#slcActionTypes").val(),
                StartDate: $("#dtStartDate").val(),
                EndDate: $("#dtEndDate").val()
            }),
            beforeSend: function () {
                showLoading();

                var otbl = $('#Table').DataTable();
                otbl.clear().draw();
                mainData = [];
            },
            success: function (list) {
                var grid = [];
                if (list) {
                    for (var j = list.length - 1; j >= 0; j--) {
                        if (j == list.length - 1) {
                            if (parseInt(list[j].idActionType) === 10 || parseInt(list[j].idActionType) === 20 || parseInt(list[j].idActionType) === 70)
                                list[j].balance = list[j].netTotal;
                            else
                                list[j].balance = list[j].netTotal * -1;
                        }
                        else {
                            if (parseInt(list[j].idActionType) === 10 || parseInt(list[j].idActionType) === 20 || parseInt(list[j].idActionType) === 70)
                                list[j].balance = parseFloat(list[j + 1].balance) + parseFloat(list[j].netTotal);
                            else
                                list[j].balance = parseFloat(list[j + 1].balance) - parseFloat(list[j].netTotal);
                        }
                    }

                    $.each(list, function (i, item) {
                        grid.push([item.transactionDate, item.transactionNr, item.actionType, item.bank, item.commissionTaxAmount.toFixed(2), item.commissionNetAmount.toFixed(2), item.commission.toFixed(2), item.id, item.idActionType]);
                        mainData.push({
                            İşlemTarihi: formatDateTime(item.transactionDate),
                            İşlemNumarası: item.transactionNr,
                            İşlemTipi: item.actionType,
                            Banka: item.bank,
                            Vergi: item.commissionTaxAmount.toFixed(2),
                            NetTutar: item.commissionNetAmount.toFixed(2),
                            ToplamTutar: item.commission.toFixed(2),
                            id: item.id,
                            dActionType: item.dActionType
                        });
                    });
                }
                var otbl = $('#Table').DataTable();
                otbl.rows.add(grid);
                otbl.draw();
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
            if (!jQuery().dataTable) {
                return;
            }
            if (!isInit) {
                initTable();
                isInit = true;
            }
            else {
                initData();
            }
        }
    };

}();