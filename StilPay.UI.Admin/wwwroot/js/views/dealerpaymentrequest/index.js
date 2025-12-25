var urlEDIT = '';
var urlDELETE = '';

$(document).ready(function () {

    $("#btnExcel").click(function () {
        downloadExcel("Üye-İşyeri-Ödeme-Bildirimleri", mainData);
    });

    Table.init();
});

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
                nRow.setAttribute("id", "tr_" + aData[9]);
            },

            "columnDefs": [
                {
                    "searchable": false,
                    "targets": [9]
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
                        $(nTd).attr('class', 'text-center');
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
                        $(nTd).attr('class', 'text-end');
                    }
                },
                {
                    "aTargets": [8],
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
                    "aTargets": [9],
                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                        $(nTd).attr('class', 'text-center');
                        $(nTd).html(sData = '<a href="/DealerPaymentRequest/Edit/' + sData + '" >Detay</a>');
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
        showLoading();

        $.get('/dealerpaymentrequest/gets', function (list) {
            var otbl = $('#Table').DataTable();
            if (list)
                $.each(list, function (i, item) {
                    otbl.rows.add([[item.cDate, item.transactionNr, item.company, item.phone, item.bank, formatDate(item.actionDate) + ' ' + item.actionTime, item.senderName, item.amount.toFixed(2), item.status, item.id]]);
                    mainData.push({
                        İşlemTarihi: formatDateTime(item.cDate),
                        İşlemNumarası: item.transactionNr,
                        Üyeİşyeri: item.company,
                        Telefon: item.phone,
                        Banka: item.bank,
                        GönderimTarihi: formatDate(item.actionDate) + ' ' + item.actionTime,
                        GöndeririciAdı: item.senderName,
                        GönderimTutarı: item.amount.toFixed(2),
                        Durum: StatusToText(item.status)
                    });
                });
            otbl.draw();
        }).fail(function () {
            onFailure();
        }).always(function () {
            hideLoading();
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
            initTable();
            initData();
        }
    };

}();