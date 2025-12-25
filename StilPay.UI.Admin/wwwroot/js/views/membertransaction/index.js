
$(document).ready(function () {
    initMemberSelection();

    initDatePicker();

    $("#btnExcel").click(function () {
        downloadExcel("Bireysel-Üye-Hareketleri", mainData);
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
            "url": "/MemberTransaction/GetData",
            "type": "POST",
            "data": function (d) {
                d.IDCompany = $("#slcMembers").val();
                d.StartDate = $("#dtStartDate").val();
                d.EndDate = $("#dtEndDate").val();
            },
        },
        "columns": [
            { "data": "transactionDate", "orderable": false },
            { "data": "transactionNr", "orderable": false },
            { "data": "actionType", "orderable": false },
            { "data": "bank", "orderable": false },
            { "data": "total", "orderable": false },
            { "data": "commission", "orderable": false },
            { "data": "netTotal", "orderable": false },
            { "data": "balance", "orderable": false },
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
                    if (parseInt(oData.idActionType) === 60 || parseInt(oData.idActionType) === 80)
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
                    $(nTd).html(sData.toFixed(2));
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
                //render: $.fn.dataTable.render.number(',', '.', 3, ''),
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    if (parseInt(oData.idActionType) === 60 || parseInt(oData.idActionType) === 80)
                        $(nTd).attr('class', 'text-end fw-bold text-success');
                    else
                        $(nTd).attr('class', 'text-end fw-bold text-danger');
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
                //nRow.setAttribute("id", "tr_" + aData[9]);
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
                        if (parseInt(oData.idActionType) === 80 || parseInt(oData.idActionType) === 60)
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
                        $(nTd).attr('class', 'text-end');
                    }
                },
                {
                    "aTargets": [8],
                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                        if (parseInt(oData.idActionType) === 80 || parseInt(oData.idActionType) === 60)
                            $(nTd).attr('class', 'text-end fw-bold text-success');
                        else
                            $(nTd).attr('class', 'text-end fw-bold text-danger');
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

            "pageLength": 15,

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
        if (!$("#slcMembers").val()) {
            alertify.notify('Lütfen bireysel üye seçiniz.', 'warning', 3, function () { }).dismissOthers();
            return;
        }

        $.ajax({
            type: "POST",
            url: 'MemberTransaction/Gets/',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({
                IDMember: $("#slcMembers").val(),
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
                            if (parseInt(list[j].idActionType) === 80 || parseInt(list[j].idActionType) === 60)
                                list[j].balance = list[j].netTotal;
                            else
                                list[j].balance = list[j].netTotal * -1;
                        }
                        else {
                            if (parseInt(list[j].idActionType) === 80 || parseInt(list[j].idActionType) === 60)
                                list[j].balance = parseFloat(list[j + 1].balance) + parseFloat(list[j].netTotal);
                            else
                                list[j].balance = parseFloat(list[j + 1].balance) - parseFloat(list[j].netTotal);
                        }
                    }

                    $.each(list, function (i, item) {
                        grid.push([item.transactionDate, item.transactionNr, item.actionType, item.bank, item.total.toFixed(2), item.commission.toFixed(2), item.costTotal.toFixed(2), item.netTotal.toFixed(2), item.balance.toFixed(2), item.idActionType]);
                        mainData.push({
                            İşlemTarihi: formatDateTime(item.transactionDate),
                            İşlemNumarası: item.transactionNr,
                            İşlemTipi: item.actionType,
                            Banka: item.bank,
                            İşlemTutarı: item.total.toFixed(2),
                            İşlemÜcreti: item.costTotal.toFixed(2),
                            Komisyon: item.commission.toFixed(2),
                            NetToplam: item.netTotal.toFixed(2),
                            Bakiye: item.balance.toFixed(2),
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