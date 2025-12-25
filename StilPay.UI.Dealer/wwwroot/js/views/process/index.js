
$(document).ready(function () {
    initDatePicker();

    $("#btnExcel").click(function () {
        downloadExcel("İşlemler", mainData);
    });

    $("#btnList").click(function () {
        getData();
    });

    getData();
});

function getData() {

    $('#Table').DataTable({
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
            "url": "/Process/GetData",
            "type": "POST",
            "data": function (d) {
                d.StartDate = $("#dtStartDate").val();
                d.EndDate = $("#dtEndDate").val();
            },
        },
        "initComplete": function (settings, json) {
            if (json.data != null && json.data.length > 0) {
                $("#divNet").text(json.data[0].balance + ' TL');
                $("#divDate").text($("#dtStartDate").val() + ' - ' + $("#dtEndDate").val());
                $("#divTotal").text(json.data[0].balanceTotal + ' / ' + json.recordsFiltered + ' Ad');
                $("#divCommission").text(json.data[0].balanceCommission + ' / ' + json.data[0].commissionCount + ' Ad');
                $("#divRebate").text(json.data[0].rebateTotal + ' / ' + json.data[0].rebateTotalCount + ' Ad');
            } else {         
                $("#divNet").text(0,00 + ' TL');
                $("#divDate").text($("#dtStartDate").val() + ' - ' + $("#dtEndDate").val());
                $("#divTotal").text(0,00 + ' / ' + 0 + ' Ad');
                $("#divCommission").text(0,00 + ' / ' + 0 + ' Ad');
                $("#divRebate").text(0,00 + ' / ' + 0 + ' Ad');
            }

        },
        "columns": [
            { "data": "transactionDate", "orderable": false },
            { "data": "transactionNr", "orderable": false },
            { "data": "actionType", "orderable": false },
            { "data": "member", "orderable": false },
            { "data": "bank", "orderable": false },
            { "data": "total", "orderable": false },
            { "data": "commissionRate", "orderable": false },
            { "data": "commission", "orderable": false },
            { "data": "netTotal", "orderable": false },
            { "data": "id", "orderable": false },
        ],

        "columnDefs": [
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
                    if (parseInt(oData.idActionType) === 10 || parseInt(oData.idActionType) === 20 || parseInt(oData.idActionType) === 70 || parseInt(oData.idActionType) === 100 || parseInt(oData.idActionType) === 140)
                        $(nTd).attr('class', 'text-center fw-bold text-success');
                    else
                        $(nTd).attr('class', 'text-center fw-bold text-danger');
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
                    $(nTd).attr('class', 'text-end');
                    $(nTd).html(sData.toFixed(2));
                }
            },
            {
                "aTargets": [6],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-end');
                    $(nTd).html(sData.toFixed(2));
                }
            },
            {
                "aTargets": [7],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-end');
                    $(nTd).html(sData.toFixed(2));
                }
            },
            {
                "aTargets": [8],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-end');
                    $(nTd).html(sData.toFixed(2));
                }
            },
            {
                "aTargets": [9],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                    $(nTd).html(sData = '<a href="/Process/Detail/' + oData.idActionType + '/' + sData + '"  target="_blank">Detay</a>');
                }
            },
        ],

        "order": [],

        "lengthMenu": [
            [15, 25, 50, 100, -1],
            [15, 25, 50, 100, "Tümü"]
        ],
    });

}

var isInit = false;
var mainData = [];
var _list = [];

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
                //nRow.setAttribute("id", "tr_" + aData[9]);
            },

            "columnDefs": [
                {
                    "searchable": false,
                    "targets": [9, 10]
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
                        $(nTd).attr('class', 'text-end');
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
                        $(nTd).attr('class', 'text-end');
                    }
                },
                {
                    "aTargets": [9],
                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                        $(nTd).attr('class', 'text-center');
                        $(nTd).html(sData = '<a href="/Process/Detail/' + sData + '/' + oData[10] + '" target="_blank" >Detay</a>');
                    }
                },
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
            url: '/Process/Gets/',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({
                StartDate: $("#dtStartDate").val(),
                EndDate: $("#dtEndDate").val()
            }),
            beforeSend: function () {
                showLoading();

                var otbl = $('#Table').DataTable();
                otbl.clear().draw();
                _list = [];
                mainData = [];
            },
            success: function (list) {
                var _total = 0, _commission = 0, _rebate = 0, _inCount = 0, _commissionCount = 0, _outCount = 0;
                var grid = [];
                if (list)
                    $.each(list, function (i, item) {
                        if (item.idActionType === '10' || item.idActionType === '20' || item.idActionType === '100') {
                            _total += item.total;
                            _inCount++;
                            _commission += item.commission;
                            _commissionCount++;
                        }
                        else if (item.idActionType === '70') {
                            _total += item.total;
                            _inCount++;
                        }
                        else {
                            _rebate -= item.total;
                            _outCount--;
                        }
                        grid.push([item.transactionDate, item.transactionNr, item.actionType, item.member, item.bank, item.total.toFixed(2), item.commissionRate, item.commission, item.netTotal, item.idActionType, item.id]);
                        _list = list;
                        mainData.push({
                            İşlemTarihi: formatDateTime(item.transactionDate),
                            İşlemNumarası: item.transactionNr,
                            İşlemTipi: item.actionType,
                            Üye: item.member,
                            Banka: item.bank,
                            İşlemTutarı: item.total.toFixed(2),
                            İşlemÜcreti: item.commission,
                            KomisyonOranı: item.commissionRate,
                            NetToplam: item.netTotal.toFixed(2),
                        })
                    });

                var otbl = $('#Table').DataTable();
                otbl.rows.add(grid);
                otbl.draw();

                $("#divDate").text($("#dtStartDate").val() + ' - ' + $("#dtEndDate").val());
                $("#divTotal").text(_total.toFixed(2) + ' / ' + _inCount + ' Ad');
                $("#divCommission").text(_commission.toFixed(2) + ' / ' + _commissionCount + ' Ad');
                $("#divRebate").text(_rebate.toFixed(2) + ' / ' + _outCount + ' Ad');
                $("#divNet").text((_total + _rebate - _commission).toFixed(2) + ' TL');
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