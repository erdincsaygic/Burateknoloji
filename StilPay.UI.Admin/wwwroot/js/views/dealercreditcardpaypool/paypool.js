$(document).ready(function () {

    initDatePicker();

    $("#btnList").click(function () {
        getData();
    });

    getData()
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
            "url": "/DealerCreditCardPayPool/GetPayPool",
            "type": "POST",
            "data": function (d) {
                d.Status = $("#slcStatus").val();
                d.PaymentMethodID = $("#slcPaymentMethod").val();
                d.StartDate = $("#dtStartDate").val();
                d.EndDate = $("#dtEndDate").val();
                d.StartDateTime = $("#dtStartDateTime").val(),
                d.EndDateTime = $("#dtEndDateTime").val()
            },
        },
        //"initComplete": function (settings, json) {
        //    if (json.data != null && json.data.length > 0) {
        //        $("#divPaymentInstitutionNetAmount").text(json.data[0].paymentInstitutionNetAmount.toString().replace(".", ",") + ' TL');
        //        $("#divSPNetAmount").text(json.data[0].spNetAmount.toString().replace(".", ",") + ' TL');
        //        $("#divPaymentInstitutionTotalAmount").text(json.data[0].paymentInstitutionTotalAmount.toString().replace(".", ",") + ' TL');
        //    } else {
        //        $("#divPaymentInstitutionNetAmount").text('0.00 TL');
        //        $("#divSPNetAmount").text('0.00 TL');
        //        $("#divPaymentInstitutionTotalAmount").text('0.00 TL');
                
        //    }

        //},
        "columns": [
            { "data": "cDate", "orderable": false },
            { "data": "transactionDate", "orderable": false },
            { "data": "bankName", "orderable": false },
            { "data": "senderName", "orderable": false },
            { "data": "cardNumber", "orderable": false },
            { "data": "cardTypeId", "orderable": false },
            { "data": "transactionType", "orderable": false },
            { "data": "amount", "orderable": false },
            { "data": "status", "orderable": false },
            { "data": "description", "orderable": false },
            { "data": "spStatus", "orderable": false },
            { "data": "spDiscardedCallbackStatus", "orderable": false },
            { "data": "cbResponseStatus", "orderable": false },
            { "data": "transactionKey", "orderable": false },
            { "data": "paymentMethodName", "orderable": false },

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
                "aTargets": [6],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                }
            },
            {
                "aTargets": [7],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                    $(nTd).html(sData.toFixed(2));
                }
            },
            {
                "aTargets": [8],
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
                "aTargets": [9],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                    $(nTd).addClass('text-ellipsis');
                    $(nTd).attr('data-bs-toggle', 'tooltip');
                    $(nTd).attr('data-bs-placement', 'bottom');
                    $(nTd).attr('title', sData);

                    var maxLength = 20;
                    if (sData.length > maxLength) {
                        sData = sData.substring(0, maxLength) + '...';
                    }
                    $(nTd).html(sData);
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
                    else if (sData === 3) {
                        $(nTd).attr('class', 'text-center fw-bold text-danger');
                        $(nTd).html(sData = StatusToText(sData));
                    }
                    else if (sData === 0) {
                        $(nTd).attr('class', 'text-center fw-bold text-danger');
                        $(nTd).html(sData = "");
                    }
                    else {
                        $(nTd).attr('class', 'text-center fw-bold text-danger');
                    }

                }
            },
            {
                "aTargets": [11],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    if (sData === 1) {
                        $(nTd).attr('class', 'text-center fw-bold text-info');
                        $(nTd).html(sData = StatusToText(sData));
                    }
                    else if (sData === 2) {
                        $(nTd).attr('class', 'text-center fw-bold text-success');
                        $(nTd).html(sData = StatusToText(sData));
                    }
                    else if (sData === 3) {
                        $(nTd).attr('class', 'text-center fw-bold text-danger');
                        $(nTd).html(sData = StatusToText(sData));
                    }
                    else if (sData === 0) {
                        $(nTd).attr('class', 'text-center fw-bold text-danger');
                        $(nTd).html(sData = "");
                    }
                    else {
                        $(nTd).attr('class', 'text-center fw-bold text-danger');
                    }

                }
            },
            {
                "aTargets": [12],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    if (sData === 0) {
                        $(nTd).attr('class', 'text-center fw-bold text-danger');
                        $(nTd).html(sData = "");
                    }
                    else if (sData === 1) {
                        $(nTd).attr('class', 'text-center fw-bold text-success');
                        $(nTd).html(sData = "ULAŞTI");
                    }
                }
            },
            {
                "aTargets": [13],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                }
            },
            {
                "aTargets": [14],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
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

function StatusToText(status) {
    if (status === 1)
        return 'BEKLİYOR';
    else if (status === 2)
        return 'ONAYLANDI';
    else
        return 'İPTAL EDİLDİ';
}