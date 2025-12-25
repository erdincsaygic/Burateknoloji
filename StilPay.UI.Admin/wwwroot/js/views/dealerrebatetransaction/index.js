var urlEDIT = '';
var urlDELETE = '';

$(document).ready(function () {
    initCompanySelection();

    initDatePicker();

    $("#btnExcel").click(function () {
        downloadExcel("Üye-İşyeri-İade-Talepleri", mainData);
    });

    $("#btnList").click(function () {
        getData();
    });

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
            "url": "/DealerRebateTransaction/GetData",
            "type": "POST",
            "data": function (d) {
                d.IDCompany = $("#slcCompanies").val();
                d.StartDate = $("#dtStartDate").val();
                d.PaymentMethod = $("#slcPaymentMethod").val();
                d.EndDate = $("#dtEndDate").val();
                d.Status = $("#slcStatus").val();
            }
        },
        "columns": [
            { "data": "cDate", "orderable": false },
            { "data": "mDate", "orderable": false },
            { "data": "transactionNr", "orderable": false },
            { "data": "transactionID", "orderable": false },
            { "data": "company", "orderable": false },
            { "data": "phone", "orderable": false },
            { "data": "member", "orderable": false },
            { "data": "senderName", "orderable": false },
            { "data": "memberPhone", "orderable": false },
            { "data": "amount", "orderable": false },
            { "data": "status", "orderable": false },
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
                }
            },
            {
                "aTargets": [5],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
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
                    $(nTd).html(sData.toFixed(2));
                }
            },
            {
                "aTargets": [10],
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
                "aTargets": [11],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                    $(nTd).html(sData = '<a href="/DealerRebateTransaction/Edit/' + sData + '" target="_blank" >Detay</a>');
                }
            }
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
        return 'BEKLİYOR';
    else if (status === 2)
        return 'ONAYLANDI';
    else if (status === 8)
        return 'İŞLEME ALINDI';
    else
        return 'İPTAL EDİLDİ';
}