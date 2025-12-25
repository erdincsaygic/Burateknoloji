$(document).ready(function () {
    initCompanySelection();

    initDatePicker();

    $("#btnList").click(function () {
        if ($("#slcCompanies").val() === null || $("#slcCompanies").val() === undefined || !$("#slcCompanies").val()) {
            alertify.notify('Lütfen Üye İşyeri Seçiniz.', 'warning', 3, function () { }).dismissOthers();
        }
        else {
            //Table.init();
            getData();
        }
    });

    $("#btnExcel").click(function () {
        downloadExcel("Üye-İşyeri-Hareketleri", mainData);
    });

    getData();
    //Table.init();

});

var total = 0;
function getData() {
    $('#Table').DataTable({
        destroy: true,
        serverSide: true,
        processing: true,
        searching: false,
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
            "url": "/DealerTransaction/GetData",
            "type": "POST",
            "data": function (d) {
                d.IDCompany = $("#slcCompanies").val();
                d.StartDate = $("#dtStartdate").val();
                d.EndDate = $("#dtEnddate").val();
            },
            "dataSrc": function (json) {
                if (json.data.length == 0 || json.data === []) {
                    total = 0;
                }
                else {

                    total = json.data[0].balance;

                    for (var j = 0; j <= json.data.length - 1; j++) {
                        if (j == 0) {
                            json.data[j].balance = total;
                        }
                        else {
                            //if (j == json.data.length - 1) {
                            //    if (parseInt(json.data[j - 1].idActionType) === 10 || parseInt(json.data[j - 1].idActionType) === 20 || parseInt(json.data[j - 1].idActionType) === 70 || parseInt(json.data[j - 1].idActionType) === 100)
                            //        json.data[j].balance = json.data[j].netTotal;
                            //    else
                            //        json.data[j].balance = json.data[j].netTotal * -1;
                            //}
                            //else {
                            if (parseInt(json.data[j - 1].idActionType) === 10 || parseInt(json.data[j - 1].idActionType) === 20 || parseInt(json.data[j - 1].idActionType) === 70 || parseInt(json.data[j - 1].idActionType) === 100 || parseInt(json.data[j - 1].idActionType) === 120 || parseInt(json.data[j - 1].idActionType) === 140 || parseInt(json.data[j - 1].idActionType) === 150 || parseInt(json.data[j - 1].idActionType) === 170)
                                json.data[j].balance = parseFloat(json.data[j - 1].balance) - parseFloat(json.data[j - 1].netTotal);
                            else
                                json.data[j].balance = parseFloat(json.data[j - 1].balance) + parseFloat(json.data[j - 1].netTotal);
                            //}
                        }
                    }
                }
                return json.data;
            }
        },
        "columns": [
            { "data": "transactionDate", "orderable": false },
            { "data": "transactionNr", "orderable": false },
            { "data": "actionType", "orderable": false },
            { "data": "bank", "orderable": false },
            { "data": "total", "orderable": false },
            { "data": "commissionRate", "orderable": false },
            { "data": "commission", "orderable": false },
            { "data": "netTotal", "orderable": false },
            {
                "data": "balance", "orderable": false,
                render: function (data, type, row) {
                    //if (page == 1) {
                    //    row.balance = total;
                    //}
                    //else {
                    //    row.balance = total - row.netTotal
                    //}
                    //row.balance = 
                    return row.balance.toFixed(2);
                }
            },
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
                    if (parseInt(oData.idActionType) === 10 || parseInt(oData.idActionType) === 20 || parseInt(oData.idActionType) === 70 || parseInt(oData.idActionType) === 100 || parseInt(oData.idActionType) === 120 || parseInt(oData.idActionType) === 140 || parseInt(oData.idActionType) === 150 || parseInt(oData.idActionType) === 170)
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
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-end');
                    $(nTd).html(sData.toFixed(2));
                }
            },
            {
                "aTargets": [8],
                //render: $.fn.dataTable.render.number(',', '.', 3, ''),
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    if (parseInt(oData.idActionType) === 10 || parseInt(oData.idActionType) === 20 || parseInt(oData.idActionType) === 70 || parseInt(oData.idActionType) === 100 || parseInt(oData.idActionType) === 120 || parseInt(oData.idActionType) === 140 || parseInt(oData.idActionType) === 150 || parseInt(oData.idActionType) === 170)
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

        "lengthMenu": [
            [15, 25, 50, 100, 500, 1000 , -1],
            [15, 25, 50, 100, 500, 1000 , "Tümü"]
        ],
    });
}

var isInit = false;
var mainData = [];
var Table = function () {

    var initTable = function () {

        var table = $('#Table');

        var otable = table.dataTable({
            "language": {
                "emptyTable": "Gösterilecek Veri Yok",
                "info": "Toplam TOTAL veriden START ile END arasındaki veriler gösteriliyor",
                "infoEmpty": "",
                "infoFiltered": "",
                "lengthMenu": "MENU Veri Göster",
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
                        if (parseInt(oData[9]) === 10 || parseInt(oData[9]) === 20 || parseInt(oData[9]) === 70 || parseInt(oData[9]) === 100 || parseInt(oData[9]) === 120)
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
                        if (parseInt(oData[9]) === 10 || parseInt(oData[9]) === 20 || parseInt(oData[9]) === 70 || parseInt(oData[9]) === 100 || parseInt(oData[9]) === 120)
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
        if (!$("#slcCompanies").val() || $("#slcCompanies").val() == "all") {
            alertify.notify('Lütfen üye işyeri seçiniz.', 'warning', 3, function () { }).dismissOthers();
            return;
        }

        $.ajax({
            type: "POST",
            url: 'DealerTransaction/GetListOld/',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({
                IDCompany: $("#slcCompanies").val(),
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
                            if (parseInt(list[j].idActionType) === 10 || parseInt(list[j].idActionType) === 20 || parseInt(list[j].idActionType) === 70 || parseInt(list[j].idActionType) === 100)
                                list[j].balance = list[j].netTotal;
                            else
                                list[j].balance = list[j].netTotal * -1;
                        }
                        else {
                            if (parseInt(list[j].idActionType) === 10 || parseInt(list[j].idActionType) === 20 || parseInt(list[j].idActionType) === 70 || parseInt(list[j].idActionType) === 100)
                                list[j].balance = parseFloat(list[j + 1].balance) + parseFloat(list[j].netTotal);
                            else
                                list[j].balance = parseFloat(list[j + 1].balance) - parseFloat(list[j].netTotal);
                        }
                    }

                    $.each(list, function (i, item) {
                        grid.push([item.transactionDate, item.transactionNr, item.actionType, item.bank, item.total.toFixed(2), item.commissionRate, item.commission, item.netTotal.toFixed(2), item.balance.toFixed(2), item.idActionType]);
                        mainData.push({
                            İşlemTarihi: formatDateTime(item.transactionDate),
                            İşlemNumarası: item.transactionNr,
                            İşlemTipi: item.actionType,
                            Banka: item.bank,
                            İşlemTutarı: item.total,
                            İşlemÜcreti: item.commission,
                            KomisyonOranı: item.commissionRate,
                            NetToplam: item.netTotal,
                            Bakiye: item.balance,
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