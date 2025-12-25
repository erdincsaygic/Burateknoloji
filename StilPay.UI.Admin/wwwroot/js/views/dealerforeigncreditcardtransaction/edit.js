var urlLIST = '/dealerforeigncreditcardtransaction/index';

$(document).ready(function () {
    $("#callbackBtn").click(function () {
        Table.init();
    });
});

var isInit = false;
var Table = function () {
    var initTable = function () {

        var table = $('#Table');

        var otable = table.dataTable({
            searching: false,
            "language": {
                "emptyTable": "Gösterilecek Veri Yok",
                "infoEmpty": "",
                "infoFiltered": "",
                "info": "Toplam _TOTAL_ veriden _START_ ile _END_ arasındaki veriler gösteriliyor",
                "lengthMenu": "_MENU_ Veri Göster",
                //"search": "Ara:",
                "zeroRecords": "Eşleşen Veri Yok",
                "paginate": {
                    "previous": "Geri",
                    "next": "İleri"
                }
            },

            
            "fnCreatedRow": function (nRow, aData, iDataIndex) {
                //nRow.setAttribute("id", "tr_" + aData[9]);
            },

            "columnDefs": [
                {
                    "searchable": false,
                    //"targets": [9, 10]
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
                        $(nTd).attr('class', 'text-center');
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
                        $(nTd).attr('class', 'text-center');
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
                        $(nTd).attr('class', 'text-center');
                    }
                }
            ],

            "order": [],
            "pageLength": 3,
            "lengthMenu": [
                [3, 10, -1],
                [3, 10, "Tümü"]
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
                }
            ]
        });
    }

    var initData = function () {
        $.ajax({
            type: "POST",
            url: '/CallbackResponseLog/GetCallbacks/',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({
                TransactionID: $('#transactionID').val()
            }),
            beforeSend: function () {
                showLoading();

                var otbl = $('#Table').DataTable();
                otbl.clear().draw();
                _list = [];
                mainData = [];
            },
            success: function (list) {
                var grid = [];
                if (list)
                $.each(list, function (i, item) {
                    grid.push([item.cDate, item.company, item.serviceType, item.transactionNr, item.transactionType, item.responseStatus, item.callback]);
                    _list = list;
                    mainData.push({
                        CallbackTarihi: formatDateTime(item.cDate),
                        Üyeİşyeri: item.company,
                        ServisTipi: item.serviceType,
                        İşlemNumarası: item.transactionNr,
                        İşlemTipi: item.transactionType,
                        ResponseStatus: item.responseStatus,
                        Callback: item.callback,
                    })
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