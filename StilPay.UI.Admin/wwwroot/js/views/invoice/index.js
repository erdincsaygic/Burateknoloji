var table;
$(document).ready(function () {
    initCompanySelection();
    initDatePicker();

    $("#btnList").click(function () {
        getData();
    });

});

function getData() {
    table = $('#Table').DataTable({
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
            "url": "/Invoice/GetData",
            "type": "POST",
            "data": function (d) {
                d.IDCompany = $("#slcCompanies").val();
                d.StartDate = $("#dtStartDate").val();
                d.EndDate = $("#dtEndDate").val();
            },
        },
        "columnDefs": [{
            "targets": [9],
            "searchable": false
        }],
        "columns": [
            { "data": "invoiceNumber", "orderable": false },
            { "data": "cDate", "orderable": false },
            { "data": "invoiceStartDateTime", "orderable": false },
            { "data": "invoiceEndDateTime", "orderable": false },
            { "data": "company", "orderable": false },
            { "data": "netAmount", "orderable": false },
            { "data": "taxAmount", "orderable": false },
            { "data": "totalAmount", "orderable": false },
            { "data": "currencyCode", "orderable": false },
            { "data": "exchangeRate", "orderable": false },
            { "data": "creator", "orderable": false },
            { "data": "id", "orderable": false },
            { "data": "status", "orderable": false },
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
                    $(nTd).html(sData = formatDateTime(sData));
                }
            },
            {
                "aTargets": [2],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                    $(nTd).html(sData = formatDateTime(sData));
                }
            },
            {
                "aTargets": [3],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                    $(nTd).html(sData = formatDateTime(sData));
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

                }
            },
            {
                "aTargets": [11],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                    if (oData.sendStatus) {
                        $(nTd).html('<button class="btn btn-sm btn-success" id="sendIntegratorBtn" disabled>Gönderildi</button>');
                    } else {
                        $(nTd).html('<button class="btn btn-sm btn-success" id="sendIntegratorBtn" onclick="sendIntegrator(\'' + sData + '\')">Gönder</button>');
                    }
                }
            },
            {
                "aTargets": [12],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    console.log(oData)
                    console.log(sData)
                    $(nTd).attr('class', 'text-center');
                    if (oData.status == 1) {
                        $(nTd).html(sData = '<span class="badge bg-success px-4 py-2" style="font-size:0.75rem">Onaylandı</span>');
                    }
                    else if (oData.status == 2) {
                        $(nTd).html(sData = '<span class="badge bg-danger px-4 py-2" style="font-size:0.75rem">İptal Edildi</span>');
                    }
                    else {
                        $(nTd).html(
                            '<div class="btn-group w-100">' +
                            '    <button type="button" class="btn btn-primary btn-sm" style="margin-right: 5px;border-radius: 8px;" onclick="updateNetTotal(\'' + oData.id + '\', \'1\')">' +
                            '        <i class="fa fa-pencil"></i>' +
                            '    </button>' +
                            '    <button type="button" class="btn btn-success btn-sm" style="margin-right: 5px;border-radius: 8px;" onclick="updateIncoiveStatus(\'' + oData.id + '\', \'1\')">' +
                            '        <i class="fa fa-check"></i>' +
                            '    </button>' +
                            '    <button type="button" class="btn btn-danger btn-sm" style="border-radius: 8px;" onclick="updateIncoiveStatus(\'' + oData.id + '\', \'2\')">' +
                            '        <i class="fa fa-trash"></i>' +
                            '    </button>' +
                            //'    <button type="button" class="btn btn-danger btn-sm" style="border-radius: 8px;" onclick="window.open(\'' + oData.parasutPrintUrl + '\', \'_blank\')">' +
                            //'        <i class="fa fa-invoice"></i>' +
                            //'    </button>' +
                            '</div>'
                        );
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


function sendIntegrator(id) {
    showLoading();
    $.ajax({
        type: "POST",
        url: '/Invoice/SendInvoiceToIntegrator',
        data: { idInvoice: id },
        success: function (response) {
            if (response.status == "OK") {
                updateSendStatusInTable(id, null)
                alertify.success("Faturalandırma entegratöre başarıyla iletildi.");
            }
            else
                alertify.error(response.message);

        },
        error: function () {
            onFailure();
        },
        complete: function () {
            hideLoading();
        }
    });
}

function updateIncoiveStatus(id , status) {
    showLoading();
    $.ajax({
        type: "POST",
        url: '/Invoice/UpdateIncoiveStatus',
        data: { idInvoice: id, status: status },
        success: function (response) {
            if (response.status == "OK") {
                updateSendStatusInTable(id)
                alertify.success("İşlem Başarılı");
            }
            else
                alertify.error(response.message);

        },
        error: function () {
            onFailure();
        },
        complete: function () {
            hideLoading();
        }
    });
}

function updateSendStatusInTable(invoiceId, status = null) {
    var table = $('#Table').DataTable();

    var row = table.row(function (idx, data, node) {
        return data.id === invoiceId; 
    });

    if (row.node()) {
        var buttonHtml = '<button class="btn btn-sm btn-success" id="sendIntegratorBtn" disabled>Gönderildi</button>';
        row.cell(row.index(), 10).data(buttonHtml).draw(false);

        var statusHtml = '';

        if (status) {
            if (status == 1) {
                statusHtml = '<span class="badge bg-success px-4 py-2" style="font-size:0.75rem">Onaylandı</span>';
            } else {
                statusHtml = '<span class="badge bg-danger px-4 py-2" style="font-size:0.75rem">İptal Edildi</span>';
            }

            row.cell(row.index(), 11).data(statusHtml).draw(false);
        }
    }
}

function updateNetTotal(invoiceId) {
    var table = $('#Table').DataTable();

    var row = table.row(function (idx, data, node) {
        return data.id === invoiceId;
    });

    if (row.node()) {
        var invoiceData = row.data();

        $('#idInvoice').val(invoiceData.id);

        $('#modalTitle').text(invoiceData.invoiceNumber + ' Numaralı Faturayı Düzenle');

        $('#invoceId').val(invoiceId);
        $('#netAmountOld').text(invoiceData.netAmount.toFixed(2)); 
        $('#taxAmountOld').text(invoiceData.taxAmount.toFixed(2)); 
        $('#totalAmountOld').text(invoiceData.totalAmount.toFixed(2));
        $('#exchangeRateOld').text(invoiceData.exchangeRate.toFixed(2));


        if (invoiceData.currencyCode == "TRY")
            $('#divExchangeRate').hide();
        else
            $('#divExchangeRate').show();


        $('#taxAmount').val('');
        $('#netAmount').val('');
        $('#exchangeRate').val(0);
        $('#totalAmount').val(1);

        $('#mdlEditInvoice').modal('show');

        $('#totalAmount').on({
            keyup: function () {
                formatCurrency($(this));
            },
            blur: function () {
                formatCurrency($(this), "blur");
            },
            input: function () {
                
                var totalAmount = parseFloat($(this).val().replace(/\./g, '').replace(',', '.')) || 0;

                if (totalAmount < 1) {
                    $('#totalAmount').val(1);
                    totalAmount = 1;
                }

                var netAmount = totalAmount / (100 + invoiceData.taxRate) * 100;
                var taxAmount = totalAmount - netAmount;

                $('#taxAmount').val(taxAmount.toFixed(2).replace('.', ','));
                $('#netAmount').val(netAmount.toFixed(2).replace('.', ','));
            }
        });
    }
}


function onSuccessUpdate(response) {
    if (response.status == "ERROR")
        alertify.notify(response.message, 'error', 3, function () { }).dismissOthers();

    else {
        var updatedInvoiceId = response.data.id;
        var updatedNetAmount = response.data.netAmount; 
        var updatedTaxAmount = response.data.taxAmount; 
        var updatedTotalAmount = response.data.totalAmount;
        var updatedExchangeRate = response.data.exchangeRate;

        var table = $('#Table').DataTable();

        var row = table.row(function (idx, data, node) {
            return data.id === updatedInvoiceId;
        });

        if (row.node()) {
            row.data({
                ...row.data(), 
                netAmount: updatedNetAmount, 
                taxAmount: updatedTaxAmount, 
                totalAmount: updatedTotalAmount,
                exchangeRate: updatedExchangeRate
            }).draw(false);
        }

        alertify.notify(response.message, 'success', 3, function () { }).dismissOthers();
        $('#mdlEditInvoice').modal('hide');
        hideLoading();
    }
}
