$(document).ready(function () {
    initDatePicker();

    $("#btnExcel").click(function () {
        downloadExcel("Ödeme-Bildirimleri", mainData);
    });

    $("#btnList").click(function () {
        Table.init();
    });

    Table.init();
});

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
                nRow.setAttribute("id", "tr_" + aData[8]);
            },

            "columnDefs": [
                {
                    "searchable": false,
                    "targets": [8]
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
                    }
                },
                {
                    "aTargets": [3],
                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                        $(nTd).attr('class', 'text-center');
                    }
                },
                {
                    "aTargets": [4],
                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
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
                        if (sData === 1)
                            $(nTd).attr('class', 'text-center fw-bold text-info');
                        else if (sData === 2)
                            $(nTd).attr('class', 'text-center fw-bold text-success');
                        else
                            $(nTd).attr('class', 'text-center fw-bold text-danger');

                        $(nTd).html(sData = StatusToText(sData));
                    }
                },
                {
                    "aTargets": [7],
                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                        $(nTd).attr('class', 'text-center');
                        $(nTd).html(sData = formatDateTime(sData));
                    }
                },
                {
                    "aTargets": [8],
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
                    "orderable": true
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
        $.ajax({
            type: "POST",
            url: 'PaymentRequest/Gets/',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({
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
                if (list)
                    $.each(list, function (i, item) {
                        grid.push([item.cDate, item.transactionNr, item.bank, formatDate(item.actionDate) + ' ' + item.actionTime, item.senderName, item.amount.toFixed(2), item.status, item.mDate, item.id]);
                        mainData.push({
                            İşlemTarihi: formatDateTime(item.cDate),
                            İşlemNumarası: item.transactionNr,
                            ÜyeAdı: item.member,
                            Telefon: item.phone,
                            Banka: item.bank,
                            GönderimTarihi: formatDate(item.actionDate) + ' ' + item.actionTime,
                            GöndeririciAdı: item.senderName,
                            GönderimTutarı: item.amount.toFixed(2),
                            Durum: StatusToText(item.status),
                            OnayİptalTarihi: formatDateTime(item.mDate)
                        });
                    });

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

    function StatusToText(status) {
        if (status === 1)
            return 'BEKLİYOR';
        else if (status === 2)
            return 'ONAYLANDI';
        else
            return 'İPTAL EDİLDİ';
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
            initData();
        }
    };

}();