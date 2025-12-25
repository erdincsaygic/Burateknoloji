$(document).ready(function () {
    initCompanySelection();

    $("#btnList").click(function () {
        getData();
    });
});

function getData() {
    $('#TableCard').DataTable({
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
            "url": "/DealerTransactionQuery/TransferDetailedSearchList",
            "type": "POST",
            "data": function (d) {
                d.IDCompany = $("#slcCompanies").val();
                d.SenderName = $("#txtSenderName").val();
                d.SenderPhone = $("#txtSenderPhone").val();
                d.SenderReferenceNr = $("#txtSenderReferenceNr").val();
                d.Status = $("#slcStatus").val();
                d.Amount = $("#txtAmount").val();
            },
        },

        "columns": [
            { "data": "cDate", "orderable": false },
            { "data": "transactionNr", "orderable": false },
            { "data": "transactionID", "orderable": false },
            { "data": "company", "orderable": false },
            { "data": "senderName", "orderable": false },
            { "data": "senderPhone", "orderable": false },
            { "data": "senderReferenceNr", "orderable": false },
            { "data": "amount", "orderable": false },
            { "data": "status", "orderable": false },
            { "data": "rebateID", "orderable": false },
            { "data": "entityUrl", "orderable": false },
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
                    if (oData.transactionID == null) {
                        if (sData === 1) {
                            $(nTd).attr('class', 'text-center fw-bold text-info');
                            $(nTd).html(sData = StatusToTextPool(sData));
                        }
                        else if (sData === 2) {
                            $(nTd).attr('class', 'text-center fw-bold text-success');
                            $(nTd).html(sData = StatusToTextPool(sData));
                        }
                        else if (sData === 3) {
                            $(nTd).attr('class', 'text-center fw-bold text-info');
                            $(nTd).html(sData = StatusToTextPool(sData));
                        }
                        else if (sData === 4) {
                            $(nTd).attr('class', 'text-center fw-bold text-info');
                            $(nTd).html(sData = StatusToTextPool(sData));
                        }
                    } else {
                        if (sData === 1) {
                            $(nTd).attr('class', 'text-center fw-bold text-info');
                            $(nTd).html(sData = StatusToText(sData));
                        }
                        else if (sData === 2) {
                            $(nTd).attr('class', 'text-center fw-bold text-success');
                            $(nTd).html(sData = StatusToText(sData));
                        }
                        else if (sData === 5) {
                            $(nTd).attr('class', 'text-center fw-bold text-info');
                            $(nTd).html(sData = StatusToText(sData));
                        }
                        else if (sData === 6) {
                            $(nTd).attr('class', 'text-center fw-bold text-info');
                            $(nTd).html(sData = StatusToText(sData));
                        }
                        else if (sData === 7) {
                            $(nTd).attr('class', 'text-center fw-bold text-danger');
                            $(nTd).html(sData = StatusToText(sData));
                        }
                        else if (sData === 9) {
                            $(nTd).attr('class', 'text-center fw-bold text-danger');
                            $(nTd).html(sData = StatusToText(sData));
                        }
                        else {
                            $(nTd).attr('class', 'text-center fw-bold text-danger text-ellipsis');
                            $(nTd).attr('data-bs-toggle', 'tooltip');
                            $(nTd).attr('data-bs-placement', 'bottom');


                            var description = oData.description != null && oData.description != '' ? oData.description : '';
                            var maxLength = 20;
                            if (description.length > maxLength) {
                                $(nTd).attr('title', description);
                                description = description.substring(0, maxLength) + '...';
                            }
                            $(nTd).html(sData = StatusToText(sData) + '<br />' + description);
                        }
                    }
                }
            },
            {
                "aTargets": [9],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                    if (oData.rebateID == "" || oData.rebateID == null && oData.transactionID != null) {
                        $(nTd).html(sData = 'İade Talebi Yok');
                    }
                    else if (oData.transactionID == null) {
                        $(nTd).html(sData = '');
                    }
                    else {
                        $(nTd).html(sData = '<button class="btn btn-custom btn-sm" onclick="window.open(\'/DealerRebateTransaction/Edit/' + oData.rebateID + '\', \'_blank\')" >İade Detayı</button>');
                    }

                }
            },
            {
                "aTargets": [10],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                    if (oData.transactionID == null) {
                        $(nTd).html(sData = 'Ödeme Havuzundaki Veri');
                    } else {
                        $(nTd).html(sData = '<button class="btn btn-custom btn-sm" onclick="window.open(\'' + oData.entityUrl + '\', \'_blank\')">İşlem Detayı</button>');
                    }
                }
            }
        ],

        "order": [],

        "lengthMenu": [
            [15, 25, 50, 100, 500, 1000, 2000],
            [15, 25, 50, 100, 500, 1000, 2000]
        ],
    });
}


function StatusToText(status) {
    if (status === 1)
        return 'BEKLİYOR';
    else if (status === 2)
        return 'ONAYLANDI';
    else if (status === 5)
        return 'Maneul Ödeme Havuzuna Gönderildi';
    else if (status === 6)
        return 'Fraud Havuzunda';
    else if (status === 7)
        return 'Fraud';
    else if (status === 9)
        return 'İade Edildi';
    else
        return 'İPTAL EDİLDİ';
}

function StatusToTextPool(status) {
    if (status === 1)
        return 'EŞLEŞMEDİ';
    else if (status === 2)
        return 'EŞLEŞTİ';
    else if (status === 3)
        return 'İADE EDİLDİ';
    else
        return 'RİSKLİ İŞLEM';
}