var urlEDIT = '';
var urlDELETE = '';

$(document).ready(function () {
    $("#btnExcel").click(function () {
        downloadExcel("Ödeme-Bildirimleri", mainData);
    });

    getData();
});

function getData() {
    $('#TableManuel').DataTable({
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
            "url": "/PaymentNotification/GetDataManuel",
            "type": "POST",
            //"dataSrc": function (data) {
            //    var datas = [];
            //    for (const [key, value] of Object.entries(data.data) ) {
            //        if (!value.isAutoNotification) {
            //            datas.push(value)
            //        }
            //    }
            //    data.recordsFiltered = datas.length;
            //    return datas;
            //}
        },
        "columnDefs": [{
            "targets": [9],
            "searchable": false
        }],
        "columns": [
            { "data": "cDate", "orderable": false },
            { "data": "transactionNr", "orderable": false },
            { "data": "company", "orderable": false },
            { "data": "companyPhone", "orderable": false },
            { "data": "senderName", "orderable": false },
            { "data": "phone", "orderable": false },
            {
                "data": "actionDate", "orderable": false,
                render: function (data, type, row) {
                    return formatDate(row.actionDate) + ' ' + row.actionTime
                }
            },
            { "data": "amount", "orderable": false },
            { "data": "status", "orderable": false },
            { "data": "id", "orderable": false },
        ],
        "rowCallback": function (row, data) {
            $(row).on('click', function () {
                var detailsRow = $(this).next('.details');

                if (detailsRow.length === 0) {
                    var detailRowHtml = '<tr class="details" style="display: none;"><td colspan="12"><div class="detail-container" style="max-height: 300px; overflow-y: auto;"><div class="loading">Yükleniyor...</div></div></td></tr>';
                    $(this).after(detailRowHtml);
                    detailsRow = $(this).next('.details');
                    loadAllDetails(data.senderName, detailsRow);
                } else {
                    detailsRow.toggle();
                }
            });
        },
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
                    $(nTd).attr('class', 'text-end');
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
                "aTargets": [9],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                    $(nTd).html(sData = '<a href="/PaymentNotification/Edit/' + sData + '" target="_blank" >Detay</a>');
                }
            }
        ],

        "order": [],

        "lengthMenu": [
            [15, 25, 50, 100, 500, 1000],
            [15, 25, 50, 100, 500, 1000]
        ],
    });

    $('#TableAuto').DataTable({
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
            "url": "/PaymentNotification/GetDataAuto",
            "type": "POST",
            //"dataSrc": function (data) {
            //    var datas = [];
            //    for (const [key, value] of Object.entries(data.data)) {
            //        if (value.isAutoNotification) {
            //            datas.push(value)
            //        }
            //    }
            //    data.recordsFiltered = datas.length;
            //    return datas;
            //},
        },
        "initComplete": function (settings, json) {
            $("#auto").removeClass("active");
        },
        "columnDefs": [{
            "targets": [9],
            "searchable": false
        }],
        "columns": [
            { "data": "cDate", "orderable": false },
            { "data": "transactionNr", "orderable": false },
            { "data": "company", "orderable": false },
            { "data": "companyPhone", "orderable": false },
            { "data": "senderName", "orderable": false },
            { "data": "phone", "orderable": false },
            {
                "data": "actionDate", "orderable": false,
                render: function (data, type, row) {
                    return formatDate(row.actionDate) + ' ' + row.actionTime
                }
            },
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
                    $(nTd).attr('class', 'text-end');
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
                "aTargets": [9],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                    $(nTd).html(sData = '<a href="/PaymentNotification/Edit/' + sData + '" target="_blank" >Detay</a>');
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


function loadAllDetails(senderName, detailsRow) {
    showLoading();
    $.ajax({
        url: '/DealerTransactionQuery/PaymentNotificationSearchList',
        type: 'POST',
        data: { SenderName: senderName, length: 100, start: 0, search: null, status: 2 },
        success: function (response) {
            var detailContainer = detailsRow.find('.detail-container');
            var detailHtml = '';

            if (response.data && response.data.length > 0) {
                detailHtml += '<table class="table table-bordered detail-table"><thead><tr><th style="font-weight: bold;">Tarih</th><th style="font-weight: bold;">Üye İşyeri</th><th style="font-weight: bold;">İşlem No</th><th style="font-weight: bold;">Üye İşyeri İşlem No</th><th style="font-weight: bold;">Gönderici Adı</th><th style="font-weight: bold;">Gönderici Telefon</th><th style="font-weight: bold;">Tutar</th></tr></thead><tbody>';

                response.data.forEach(function (detail) {
                    detailHtml += '<tr><td>' + formatDateTime(detail.cDate) + '</td><td>' + detail.company + '</td><td>' + detail.transactionNr + '</td><td>' + detail.transactionID + '</td><td>' + detail.senderName + '</td><td>' + detail.phone + '</td><td>' + detail.amount.toFixed(2) + '</td></tr>';
                });

                detailHtml += '</tbody></table>';

                detailContainer.html(detailHtml);
                detailsRow.show();
            } else {
                detailContainer.html('<div class="no-data">Veri bulunamadı.</div>');
                detailsRow.show();
            }
        },
        error: function () {
            var detailContainer = detailsRow.find('.detail-container');
            detailContainer.html('<div class="error">Veri yüklenirken hata oluştu.</div>');
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