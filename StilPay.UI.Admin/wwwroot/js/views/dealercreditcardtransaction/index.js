$(document).ready(function () {
    initCompanySelection();

    initDatePicker();

    $("#btnExcel").click(function () {
        downloadExcel("Üye-İşyeri-Kredi-Kartı-Ödeme-Hareketleri", mainData);
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
            "url": "/DealerCreditCardTransaction/GetData",
            "type": "POST",
            "data": function (d) {
                d.IDCompany = $("#slcCompanies").val();
                d.StartDate = $("#dtStartDate").val();
                d.EndDate = $("#dtEndDate").val();
                d.Status = $("#slcStatus").val();
                d.PaymentMethodID = $("#slcPaymentMethod").val();
            },
        },

        "columns": [
            { "data": "cDate", "orderable": false },
            { "data": "transactionNr", "orderable": false },
            { "data": "isAutoNotification", "orderable": false },
            { "data": "company", "orderable": false },
            { "data": "member", "orderable": false },
            { "data": "cardNumber", "orderable": false },
            { "data": "cardTypeId", "orderable": false },
            { "data": "paymentInstitutionName", "orderable": false },
            { "data": "phone", "orderable": false },
            { "data": "amount", "orderable": false },
            { "data": "commissionRate", "orderable": false },
            { "data": "commission", "orderable": false },
            { "data": "netTotal", "orderable": false },
            { "data": "paymentInstitutionNetAmount", "orderable": false },
            { "data": "paymentInstitutionCommissionRate", "orderable": false },
            { "data": "status", "orderable": false },
            { "data": "modifier", "orderable": false },
            { "data": "id", "orderable": false }
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
                    if (sData === true)
                        $(nTd).html(sData = '<span class="badge bg-success px-4 py-2">EVET</span>');
                    else
                        $(nTd).html(sData = '<span class="badge bg-danger px-4 py-2">HAYIR</span>');
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
                    if (sData === 1) {
                        $(nTd).html('Banka Kartı');
                    }
                    else if (sData === 2) {
                        $(nTd).html('Kredi Kartı');
                    }
                    else if (sData === 3) {
                        $(nTd).html('TOSLA');
                    }
                    else {
                        $(nTd).html('Bilinmiyor');
                    }
                }
            },
            {
                "aTargets": [7],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                }
            },
            {
                "aTargets": [8],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                }
            },
            {
                "aTargets": [9],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-end');
                    $(nTd).html(sData.toFixed(2));
                }
            },
            {
                "aTargets": [10],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-end');
                    $(nTd).html(sData.toFixed(2));
                }
            },
            {
                "aTargets": [11],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-end');
                    $(nTd).html(sData.toFixed(2));
                }
            },
            {
                "aTargets": [12],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-end');
                    $(nTd).html(sData.toFixed(2));
                }
            },
            {
                "aTargets": [13],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-end');
                    $(nTd).html(sData == null ? 0.0.toFixed(2) : sData.toFixed(2));
                }
            },
            {
                "aTargets": [14],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-end');
                    $(nTd).html(sData == null ? 0.0.toFixed(2) : sData.toFixed(2));
                }
            },
            {
                "aTargets": [15],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
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
            },
            {
                "aTargets": [16],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                }
            },
            {
                "aTargets": [17],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                    $(nTd).html(sData = '<a href="/DealerCreditCardTransaction/Edit/' + sData + '" target="_blank" >Detay</a>');
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