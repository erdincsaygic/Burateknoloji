var urlEDIT = '';
var urlDELETE = '';
var urlLIST = '/DealerTransferPayPool';

var _pinCtx = {
    dt: null,
    tr: null,
    rowData: null
};

$(document).ready(function () {

    initDatePicker();

    $("#btnList").click(function () {
        getData();
    });

    $("#btnSearchInBank").click(function () {
        //searchInBank();
        Table.init();
    });

    $('#mdlPin').on('shown.bs.modal', function () {
        $('#pin_value').trigger('focus');
    });

    $(document).off('keydown.pin').on('keydown.pin', '#pin_value', function (e) {
        if (e.key === 'Enter') {
            $('#btnSavePin').trigger('click');
        }
    });

    $('#btnSavePin').off('click').on('click', function () {
        savePinAndUpdateSenderNameOnly();
    });

    getData();

    $('#mdlRebate').on('show.bs.modal', function (e) {
        var tpId = $(e.relatedTarget).data('tp-id');
        $(e.currentTarget).find('input[name="tpId"]').val(tpId);
    });

    $('#mdlPairingWithDealer').on('show.bs.modal', function (e) {
        $('#dealerId').val('');
        var tpId = $(e.relatedTarget).data('tp-id');
        $(e.currentTarget).find('input[name="tpId"]').val(tpId);
    });

    $('#mdlSetStatusNotPairing').on('show.bs.modal', function (e) {
        var tpId = $(e.relatedTarget).data('tp-id');
        $(e.currentTarget).find('input[name="tpId"]').val(tpId);
    });

    $('#mdlSearchInBank').on('show.bs.modal', function (e) {
        var currentDate = new Date();

        currentDate.setMinutes(currentDate.getMinutes() - 2);
        var formattedDateEnd = currentDate.toLocaleString().slice(11, 19).replace("T", " ");

        var currentDate = new Date();
        currentDate.setHours(currentDate.getHours() - 3);
        var formattedDateStart = currentDate.toLocaleString().slice(11, 19).replace("T", " ");

        $(e.currentTarget).find('input[id="dtStartDateSearchInBank"]').val(currentDate.toLocaleString().slice(0, 10));
        $(e.currentTarget).find('input[id="dtStartDateTimeSearchInBank"]').val(formattedDateStart);
        $(e.currentTarget).find('input[id="dtEndDateTimeSearchInBank"]').val(formattedDateEnd);

    });

    $('#mdlSearchInBank').on('hidden.bs.modal', function (e) {
        var otbl = $('#TableNotInThePool').DataTable();
        otbl.clear().draw();

    });



    $("#frmPairingWithDealer").validate({
        errorElement: 'div',
        errorClass: 'text-danger',
        rules: {
            "dealerId": {
                required: true
            },
        },
        messages: {
            "dealerId": {
                required: "Lütfen üye işyeri seçiniz"
            }
        }
    });
});

function HideShow() {
    $("#idFilter").slideToggle(500);
}

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
            "url": "/DealerTransferPayPool/GetData",
            "type": "POST",
            "data": function (d) {
                d.Status = $("#slcStatus").val();
                d.StartDate = $("#dtStartDate").val();
                d.EndDate = $("#dtEndDate").val();
                d.StartDateTime = $("#dtStartDateTime").val();
                d.EndDateTime = $("#dtEndDateTime").val();
                d.IDBank = $("#slcBank").val();
                d.IsHaveReferenceNr = $("#slcIsHaveReferenceNr").val();
                d.ResponseStatus = $("#slcResponseStatus").val();
                d.IsCaughtInFraudControl = $("#slcFraudControl").val();
            },
        },
        "initComplete": function (settings, json) {
            if (json.data != null && json.data.length > 0) {
                $("#divMatches").text(json.data[0].matchesBalance.toString().replace(".", ",") + ' TL' + ' / ' + json.data[0].matchesCount + ' Ad');
                $("#divDate").text($("#dtStartDate").val() + ' - ' + $("#dtEndDate").val());
                $("#divNotMatches").text(json.data[0].notMatchesBalance.toString().replace(".", ",") + ' TL' + ' / ' + json.data[0].notMatchesCount + ' Ad');
            } else {
                $("#divMatches").text(0 + ' TL' + ' / ' + 0 + ' Ad');
                $("#divDate").text($("#dtStartDate").val() + ' - ' + $("#dtEndDate").val());
                $("#divNotMatches").text(0 + ' TL' + ' / ' + 0 + ' Ad');
            }

            var dt = $('#Table').DataTable();

            $('#Table tbody').off('dblclick', 'td');

            $('#Table tbody').on('dblclick', 'td', function () {
                var idx = dt.cell(this).index();
                if (!idx) return;

                if (idx.column !== 3) return;

                var tr = $(this).closest('tr');
                var rowData = dt.row(tr).data();
                if (!rowData) return;

                openPinModal(rowData, dt, tr[0]);
            });

        },
        "columns": [
            { "data": "cDate", "orderable": false },
            { "data": "transactionDate", "orderable": false },
            { "data": "bank", "orderable": false },
            { "data": "senderName", "orderable": false },
            { "data": "senderIban", "orderable": false },
            { "data": "amount", "orderable": false },
            { "data": "description", "orderable": false },
            { "data": "fraudControlDescription", "orderable": false },
            { "data": "company", "orderable": false },
            { "data": "responseTransactionNr", "orderable": false },
            { "data": "isHaveReferenceNr", "orderable": false },
            { "data": "responseStatus", "orderable": false },
            { "data": "status", "orderable": false },
            { "data": "status", "orderable": false }
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
                    $(nTd).html(sData = formatDateTime(sData));
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
                    $(nTd).attr('class', 'text-end');
                    $(nTd).html(sData.toFixed(2));
                }
            },
            {
                "aTargets": [6],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                }
            },
            {
                "aTargets": [7],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');

                    console.log(oData.isCaughtInFraudControl)
                    if (oData.isCaughtInFraudControl) {
                        $(nTd).addClass('fraud-highlight');
                    }
                }
            },
            {
                "aTargets": [8],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                }
            },
            {
                "aTargets": [9],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                }
            },
            {

                "aTargets": [10],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                    if (sData === true)
                        $(nTd).html(sData = '<span class="badge bg-success px-4 py-2">EVET</span>');
                    else
                        $(nTd).html(sData = '<span class="badge bg-danger px-4 py-2">HAYIR</span>');
                }
            },
            {

                "aTargets": [11],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                    if (sData === 1) {
                        $(nTd).attr('class', 'text-center fw-bold text-success');
                        $(nTd).html(sData = ResponseStatusToText(sData));
                    }
                    else if (sData === 2) {
                        $(nTd).attr('class', 'text-center fw-bold text-danger');
                        $(nTd).html(sData = ResponseStatusToText(sData));
                    }
                    else if (sData === 3) {
                        $(nTd).attr('class', 'text-center fw-bold text-danger');
                        $(nTd).html(sData = ResponseStatusToText(sData));
                    }
                    else {
                        $(nTd).attr('class', 'text-center fw-bold text-danger');
                        $(nTd).html(sData = ResponseStatusToText(sData));
                    }
                }
            },
            {
                "aTargets": [12],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
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
                        $(nTd).html(sData = StatusToText(sData));
                    }
                }
            },
            {
                "aTargets": [13],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center fw-bold');
                    if (sData === 1) {
                        $(nTd).html(
                            '<div style="width:150px"> <button type="button" class="btn-danger" style="border-radius:0.25rem; padding: 0 10px;" data-bs-toggle="modal" data-bs-target="#mdlRebate" data-tp-id="' + oData.id + '" >İade Et</button>' + ' / ' + '<button type="button" class="btn-success" style="border-radius:0.25rem;  padding: 0 10px;" data-bs-toggle="modal" data-bs-target="#mdlPairingWithDealer" data-tp-id="' + oData.id + '" >Eşleştir</button> </div>')
                    }
                    else if (sData === 10) {
                        $(nTd).html(
                            '<div style="width:150px"> <button type="button" class="btn-danger" style="border-radius:0.25rem; padding: 0 10px;margin-bottom: 5px;" data-bs-toggle="modal" data-bs-target="#mdlRebate" data-tp-id="' + oData.id + '" >İade Et</button>' + ' / ' + '<button type="button" class="btn-success" style="border-radius:0.25rem;  padding: 0 10px;" data-bs-toggle="modal" data-bs-target="#mdlSetStatusNotPairing" data-tp-id="' + oData.id + '" >Eşleşmedi Durumuna Al</button> </div>')
                    } else {
                        $(nTd).html("")
                    }
                }
            },
        ],

        "order": [],

        "lengthMenu": [
            [15, 25, 50, 100, 500, 1000],
            [15, 25, 50, 100, 500, 1000]
        ],
    });
}

var isInit = false;
var mainData = [];
var _list = [];

var Table = function () {

    var initTable = function () {

        var table = $('#TableNotInThePool');

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
                        $(nTd).html(sData.toFixed(2));
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
                        $(nTd).html('<button class="btn btn-success btn-sm" id="sendToTransferBtn"><i class="fa fa-save"></i>&nbsp;&nbsp;&nbsp;Havuza Aktar</button>');

                        $(nTd).find('#sendToTransferBtn').on('click', function () {
                            sendDataToApi(oData, $(nTd).closest('tr'));
                        });

                    }
                }
            ],

            "order": [],

            "pageLength": 15,

            "lengthMenu": [
                [15, 30, 50, 100, -1],
                [15, 30, 50, 100, "Tümü"]
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

        var tableWrapper = $('#Table_wrapper');
        tableWrapper.find('.dataTables_length select').select2({
            minimumResultsForSearch: Infinity,
        });
    }

    var initData = function () {
        $.ajax({
            type: "POST",
            url: '/DealerTransferPayPool/SearchInBank/',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({
                StartDate : $("#dtStartDateSearchInBank").val(),
                EndDate : $("#dtEndDateSearchInBank").val(),
                StartDateTime : $("#dtStartDateTimeSearchInBank").val(),
                EndDateTime : $("#dtEndDateTimeSearchInBank").val()
            }),
            beforeSend: function () {
                showLoading();

                var otbl = $('#TableNotInThePool').DataTable();
                otbl.clear().draw();
                _list = [];
                mainData = [];
            },
            success: function (list) {
                var grid = [];
                if (list)
                    $.each(list, function (i, item) {
                        grid.push([item.transactionDate, item.bank, item.senderName, item.amount, item.description, item.transactionKey]);
                        _list = list;
                        mainData.push({
                            İşlemTarihi: formatDateTime(item.transactionDate),
                            Banka: item.bank,
                            GöndericiAdı: item.senderName,
                            GönderimTutar: item.amount,
                            Açıklama: item.description,
                            İşlemler: item.transactionKey,
                        })
                    });

                var otbl = $('#TableNotInThePool').DataTable();
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

function sendDataToApi(data, rowElement) {
    $.ajax({
        type: 'POST',
        url: '/DealerTransferPayPool/SaveSelectedDataToTransferPool/', 
        contentType: 'application/json',
        data: JSON.stringify({
            TransactionDate:data[0],
            Bank: data[1],
            SenderName: data[2],
            Amount: data[3],
            Description: data[4],
            TransactionKey: data[5],
        }),
        success: function (response) {
            if (response.status == "ERROR") {
                alertify.notify(response.message, 'error', 5, function () { }).dismissOthers();
            }
            else {
                var otbl = $('#TableNotInThePool').DataTable();
                otbl.row(rowElement).remove().draw();
                alertify.notify(response.message, 'success', 5, function () { }).dismissOthers();
            }


        },
        error: function (error) {
            alertify.notify(error, 'error', 5, function () { }).dismissOthers();

        }
    });
}

function openPinModal(rowData, dt, trEl) {
    _pinCtx.dt = dt;
    _pinCtx.tr = trEl;
    _pinCtx.rowData = rowData;

    $('#pin_tpId').val(rowData.id);
    $('#pin_value').val('');

    $('#pin_senderName').val(rowData.senderName || '');

    $('#mdlPin').modal('show');
}

function savePinAndUpdateSenderNameOnly() {
    var tpId = ($('#pin_tpId').val() || '').trim();
    var pin = ($('#pin_value').val() || '').trim();
    var senderName = ($('#pin_senderName').val() || '').trim();

    if (!tpId) {
        alertify.notify('Kayıt bulunamadı (tpId boş)', 'error', 5).dismissOthers();
        return;
    }
    if (!pin) {
        alertify.notify('PIN zorunlu', 'error', 5).dismissOthers();
        return;
    }
    if (!senderName) {
        alertify.notify('Gönderici adı zorunlu', 'error', 5).dismissOthers();
        return;
    }

    $.ajax({
        type: 'POST',
        url: '/DealerTransferPayPool/SetSenderNameByPin',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({ tpId: tpId, pin: pin, senderName: senderName }),
        beforeSend: function () {
            showLoading();
        },
        success: function (res) {
            if (res && res.status === "ERROR") {
                alertify.notify(res.message || 'Hata oluştu', 'error', 5).dismissOthers();
                return;
            }

            if (!res || !res.senderName) {
                alertify.notify('SenderName dönmedi', 'error', 5).dismissOthers();
                return;
            }

            var currentRowData = _pinCtx.dt.row(_pinCtx.tr).data();
            if (currentRowData) {
                currentRowData.senderName = res.senderName;
            }

            _pinCtx.dt.cell(_pinCtx.tr, 3).data(res.senderName);

            _pinCtx.dt.row(_pinCtx.tr).invalidate();
            _pinCtx.dt.draw(false);

            $('#mdlPin').modal('hide');
            alertify.notify(res.message || 'Gönderici adı güncellendi', 'success', 5).dismissOthers();
        },
        error: function () {
            alertify.notify('İşlem sırasında hata oluştu', 'error', 5).dismissOthers();
        },
        complete: function () {
            hideLoading();
        }
    });
}

function HideShow() {
    $("#idFilter").slideToggle(500);
}

function StatusToText(status) {
    if (status === 1)
        return 'EŞLEŞMEDİ';
    else if (status === 2)
        return 'EŞLEŞTİ';
    else if (status === 3)
        return 'İADE EDİLDİ';
    else
        return 'RİSKLİ İŞLEM';
}

function ResponseStatusToText(status) {
    if (status === 1)
        return 'Bakiye Eklendi';
    else if (status === 2)
        return 'Bakiye Eklenmedi / Red';
    else if (status === 3)
        return 'Bakiye Eklenmedi / Hata';
    else
        return 'Cevap Alınamadı';
}