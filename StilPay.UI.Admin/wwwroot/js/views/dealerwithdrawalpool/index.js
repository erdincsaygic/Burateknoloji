var urlEDIT = '';
var urlDELETE = '';
var urlLIST = '/DealerTransferPayPool';

$(document).ready(function () {

    initDatePicker();

    $("#btnList").click(function () {
        getData();
    });

    getData();

    $('#mdlRebate').on('show.bs.modal', function (e) {
        var tpId = $(e.relatedTarget).data('tp-id');
        $(e.currentTarget).find('input[name="tpId"]').val(tpId);
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
            "url": "/DealerWithdrawalPool/GetData",
            "type": "POST",
            "data": function (d) {
                d.Status = $("#slcStatus").val();
                d.StartDate = $("#dtStartDate").val();
                d.EndDate = $("#dtEndDate").val();
                d.IDBank = $("#slcBank").val();
            },
        },
        "columns": [
            { "data": "cDate", "orderable": false },
            { "data": "transactionDate", "orderable": false },
            { "data": "isRebate", "orderable": false },
            { "data": "bank", "orderable": false },
            { "data": "receiverName", "orderable": false },
            { "data": "receiverIban", "orderable": false },
            { "data": "amount", "orderable": false },
            { "data": "description", "orderable": false },
            { "data": "responseDescription", "orderable": false },
            { "data": "responseTransactionNr", "orderable": false },
            { "data": "status", "orderable": false },
            //{ "data": "status", "orderable": false }
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
                    $(nTd).html(sData ? 'İade' : 'Çekim Talebi' );
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
                    $(nTd).attr('class', 'text-end');
                    $(nTd).html(sData.toFixed(2));
                }
            },
            {
                "aTargets": [7],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
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
            //{
            //    "aTargets": [10],
            //    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
            //        $(nTd).attr('class', 'text-center fw-bold');
            //        if (sData === 1) {
            //            $(nTd).html(
            //                '<div style="width:150px"> <button type="button" class="btn-danger" style="border-radius:0.25rem; padding: 0 10px;" data-bs-toggle="modal" data-bs-target="#mdlRebate" data-tp-id="' + oData.id + '" >İade Et</button>' + ' / ' + '<button type="button" class="btn-success" style="border-radius:0.25rem;  padding: 0 10px;" data-bs-toggle="modal" data-bs-target="#mdlPairingWithDealer" data-tp-id="' + oData.id + '" >Eşleştir</button> </div>')

            //        } else {
            //            $(nTd).html("")
            //        }
            //    }
            //},
        ],

        "order": [],

        "lengthMenu": [
            [15, 25, 50, 100, 500, 1000],
            [15, 25, 50, 100, 500, 1000]
        ],
    });
}

function StatusToText(status) {
    if (status === 1)
        return 'EŞLEŞMEDİ';
    else if (status === 2)
        return 'EŞLEŞTİ';
    else if (status === 3)
        return 'GEÇERSİZ';
    else
        return 'İADE EDİLDİ';
}
