$(document).ready(function () {
    initDatePicker();

    $("#btnExcel").click(function () {
        downloadExcel("İade-Bildirimleri", mainData);
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
            "url": "/RebateRequest/GetData",
            "type": "POST",
            "data": function (d) {
                d.StartDate = $("#dtStartDate").val();
                d.EndDate = $("#dtEndDate").val();
                d.Status = $("#slcStatus").val();
            }
        }, 
        "columns": [
            {
                "data": "actionDate", "orderable": false,
                render: function (data, type, row) {
                    return formatDate(row.actionDate) + ' ' + row.actionTime
                }
            },
            { "data": "transactionNr", "orderable": false },
            { "data": "transactionID", "orderable": false },
            { "data": "member", "orderable": false },
            { "data": "phone", "orderable": false },
            { "data": "amount", "orderable": false },
            { "data": "status", "orderable": false },
            { "data": "mDate", "orderable": false }, 
        ],

        "columnDefs": [
            {
                "aTargets": [0],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
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
                    $(nTd).html(sData.toFixed(2));
                }
            },
            {
                "aTargets": [6],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    if (sData === 1 || sData === 8) {
                        $(nTd).attr('class', 'text-center fw-bold text-info');
                        $(nTd).html(sData = StatusToText(sData));
                    }
                    else if (sData === 2) {
                        $(nTd).attr('class', 'text-center fw-bold text-success');
                        $(nTd).html(sData = StatusToText(sData));
                    }
                    else {
                        $(nTd).attr('class', 'text-center fw-bold text-danger');
                        $(nTd).html(sData = StatusToText(sData) + (oData.description != null && oData.description != '' ? '<br/>' + oData.description : ''));
                    }

                }
            },
            {
                "aTargets": [7],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                    $(nTd).html(sData = formatDateTime(sData));
                }
            }
        ],

        "order": [],

        "lengthMenu": [
            [15, 25, 50, 100, -1],
            [15, 25, 50, 100, "Tümü"]
        ],
    });
}


function StatusToText(status) {
    if (status === 1)
        return 'BEKLİYOR';
    else if (status === 2)
        return 'ONAYLANDI';
    else if (status === 8)
        return 'İŞLEME ALINDI';
    else
        return 'İPTAL EDİLDİ';
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
                //nRow.setAttribute("id", "tr_" + aData[8]);
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
                        $(nTd).attr('class', 'text-center');
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
                        $(nTd).attr('class', 'text-center');
                        if (sData === 1) {
                            $(nTd).attr('class', 'text-center fw-bold text-info');
                            $(nTd).html(sData = StatusToText(sData));
                        }
                        else if (sData === 2) {
                            $(nTd).attr('class', 'text-center fw-bold text-success');
                            $(nTd).html(sData = StatusToText(sData));
                        }
                        else {
                            $(nTd).attr('class', 'text-center fw-bold text-danger');
                            $(nTd).html(sData = StatusToText(sData) + (oData.description != null && oData.description != '' ? '<br/>' + oData.description : ''));
                        }                                           
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
            url: 'RebateRequest/Gets/',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({
                IDMember: null,
                Status: $("#slcStatus").val(),
                StartDate: null,
                EndDate: null
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
                        grid.push([item.transactionDate, item.transactionNr, item.transactionID, item.member, item.phone, item.amount.toFixed(2), item.status, item.mDate, item.id, item.description]);
                        mainData.push({
                            İşlemTarihi: formatDateTime(item.cDate),
                            İşlemNumarası: item.transactionNr,
                            İşlemID: item.transactionID,
                            ÜyeAdı: item.member,
                            Telefon: item.phone,
                            İadeTutarı: item.amount.toFixed(2),
                            Durum: StatusToText(item.status),
                            İadeTarihi: formatDateTime(item.mDate)
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