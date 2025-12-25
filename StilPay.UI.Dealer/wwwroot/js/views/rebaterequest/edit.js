var urlLIST = '/RebateRequest';

$(document).ready(function () {
    $("#frmDef2").validate({
        errorElement: 'div',
        errorClass: 'text-danger',
        rules: {
            "Iban": {
                required: true,
                minlength: 30,
            }
        },
        messages: {
            "Iban": {
                required: "Lütfen IBAN Giriniz",
                minlength: "Uzunluk 24 Haneli Olmalıdır",
            }
        }
    });

    const handleMaxAmount = (inputId, maxAmount) => {
        if (maxAmount) {
            maxAmount = parseFloat(maxAmount.replace(',', '.')); // Convert comma to dot
        }

        const inputElement = document.getElementById(inputId);
        if (inputElement && !isNaN(maxAmount)) {
            inputElement.addEventListener('input', function () {
                let currentValue = parseFloat(this.value.replace(',', '.')); // Convert comma to dot for current value
                if (currentValue > maxAmount) {
                    this.value = maxAmount.toString().replace('.', ','); // Convert dot to comma for display
                }
            });
        }
    };

    // Set max amounts for different inputs
    handleMaxAmount('creditCardAmountInput', $("#creditCardAmountInput").val());
    handleMaxAmount('foreignCardAmountInput', $("#foreignCardAmountInput").val());
    handleMaxAmount('paymentAmountInput', $("#paymentAmountInput").val());

    $("#setIban").mask('00 0000 0000 0000 0000 0000 00')

    var transactionID = $("#txtSearch").val();
    $("#btnSearch").click(function () {
        var transactionID = $("#txtSearch").val();
        if (transactionID == undefined || transactionID == null || transactionID.length == 0) {
            alertify.notify('İşlem Numarası Giriniz.', 'warning', 5, function () { }).dismissOthers();
            return;
        }
        else {
            window.location.href = "/RebateRequest/Edit/" + transactionID;
        }
    });

    $("#txtSearch").numeric_format({
        defaultvalue: "",
        thousandslimit: 18,
        centsLimit: 0,
        centsSeparator: ',',
        thousandsSeparator: '.',
        clearOnEmpty: true
    });

    $("#btnSubmit").click(function () {
        $("#mdlConfirm").modal('show');
    });

    $("#btnSubmitIban").click(function () {
        $("#mdlIban").modal('show');
    })

    $("#PaymentNotification_Iban").mask('TR00 0000 0000 0000 0000 0000 00');

    aktifTabPage();

    if (transactionID == undefined || transactionID == null || transactionID.length == 0) {
        return;
    }
    else {
        initDatas();
    }
});

function initDatas() {
    getData();
    getData2();
    getData3();
    getData4();
}

function onSuccessSetIBan(response) {
    if (response.status === "ERROR")
        alertify.notify(response.message, 'error', 5, function () { }).dismissOthers();
    else {
        alertify.notify('İşlem Başarılı.', 'success', 5, function () { }).dismissOthers();
        window.location.reload();
    }
}

function getData() {
    $('#Table1').DataTable({
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
            "url": "/RebateRequest/GetData",
            "type": "POST",
            "data": function (d) {
                d.IDMember = $("#IDMember").val(),
                d.Status = "4",
                d.StartDate =null,
                d.EndDate = null
            }
        },
        "columns":[
            { "data": "mDate", "orderable": false },
            { "data": "amount", "orderable": false },
            { "data": "modifier", "orderable": false },
            { "data": "status", "orderable": false },
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
                    $(nTd).attr('class', 'text-end');
                    $(nTd).html(sData.toFixed(2));
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
        ],

        "order": [],

        "lengthMenu": [
            [15, 25, 50, 100, -1],
            [15, 25, 50, 100, "Tümü"]
        ],
    });
}

function getData2() {
    $('#Table2').DataTable({
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
            "url": "/PaymentNotification/GetData",
            "type": "POST",
            "data": function (d) {
                d.IDMember = $("#IDMember").val(),
                d.StartDate = null,
                d.EndDate = null
            }
        },
        "initComplete": function (settings, json) {
            $('#Table2').removeAttr('style');
        },
        "columns": [
            { "data": "cDate", "orderable": false },
            { "data": "transactionNr", "orderable": false },
            { "data": "bank", "orderable": false },
            {
                "data": "actionDate", "orderable": false,
                render: function (data, type, row) {
                    return formatDate(row.actionDate) + ' ' + row.actionTime
                }
            },
            { "data": "senderName", "orderable": false },
            { "data": "amount", "orderable": false },
            { "data": "status", "orderable": false },
            { "data": "mDate", "orderable": false },
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
                "aTargets": [7],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                    $(nTd).html(sData = formatDateTime(sData));
                }
            },
            {
                "aTargets": [8],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                    idActionType = oData.isAutoNotification ? 10 : 20;
                    $(nTd).html(sData = '<a href="/Process/Detail/' + idActionType + '/' + sData + '"  target="_blank">Detay</a>');
                }
            },
        ],

        "order": [],

        "lengthMenu": [
            [15, 25, 50, 100, -1],
            [15, 25, 50, 100, "Tümü"]
        ],
    });
}

function getData3() {
    $('#Table3').DataTable({
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
            "url": "/CreditCardPaymentNotification/GetData",
            "type": "POST",
            "data": function (d) {
                d.IDMember = $("#IDMember").val(),
                d.StartDate = null,
                d.EndDate = null
            }
        },
        "initComplete": function (settings, json) {
            $('#Table3').removeAttr('style');
        },
        "columns": [
            { "data": "cDate", "orderable": false },
            { "data": "transactionNr", "orderable": false },
            { "data": "senderName", "orderable": false },
            { "data": "cardNumber", "orderable": false },
            { "data": "senderIdentityNr", "orderable": false },
            { "data": "amount", "orderable": false },
            { "data": "status", "orderable": false },
            { "data": "mDate", "orderable": false },
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
                "aTargets": [7],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                    $(nTd).html(sData = formatDateTime(sData));
                }
            },
            {
                "aTargets": [8],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                    $(nTd).html(sData = '<a href="/Process/Detail/100/' + sData + '"  target="_blank">Detay</a>');
                }
            },
        ],

        "order": [],

        "lengthMenu": [
            [15, 25, 50, 100, -1],
            [15, 25, 50, 100, "Tümü"]
        ],
    });
}

function getData4() {
    $('#Table4').DataTable({
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
            "url": "/ForeignCreditCardPaymentNotification/GetData",
            "type": "POST",
            "data": function (d) {
                d.IDMember = $("#IDMember").val(),
                    d.StartDate = null,
                    d.EndDate = null
            }
        },
        "initComplete": function (settings, json) {
            $('#Table4').removeAttr('style');
        },
        "columns": [
            { "data": "cDate", "orderable": false },
            { "data": "transactionNr", "orderable": false },
            { "data": "senderName", "orderable": false },
            { "data": "cardNumber", "orderable": false },
            { "data": "senderIdentityNr", "orderable": false },
            { "data": "amount", "orderable": false },
            { "data": "status", "orderable": false },
            { "data": "mDate", "orderable": false },
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
                "aTargets": [7],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                    $(nTd).html(sData = formatDateTime(sData));
                }
            },
            {
                "aTargets": [8],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                    $(nTd).html(sData = '<a href="/Process/Detail/140/' + sData + '"  target="_blank">Detay</a>');
                }
            },
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
    else if (status === 8)
        return 'İŞLEME ALINDI';
    else
        return 'İPTAL EDİLDİ';
}

//var Table = function () {

//    var initTable1 = function () {

//        var table = $('#Table1');

//        var otable = table.dataTable({

//            "language": {
//                "emptyTable": "Gösterilecek Veri Yok",
//                "info": "Toplam _TOTAL_ veriden _START_ ile _END_ arasındaki veriler gösteriliyor",
//                "infoEmpty": "",
//                "infoFiltered": "",
//                "lengthMenu": "_MENU_ Veri Göster",
//                "search": "Ara:",
//                "zeroRecords": "Eşleşen Veri Yok",
//                "paginate": {
//                    "previous": "Geri",
//                    "next": "İleri"
//                }
//            },

//            "fnCreatedRow": function (nRow, aData, iDataIndex) {
//            },

//            "columnDefs": [
//                {
//                    "aTargets": [0],
//                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
//                        $(nTd).attr('class', 'text-center');
//                        $(nTd).html(sData = formatDateTime(sData));
//                    }
//                },
//                {
//                    "aTargets": [1],
//                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
//                        $(nTd).attr('class', 'text-center');
//                    }
//                },
//                {
//                    "aTargets": [2],
//                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
//                    }
//                },
//                {
//                    "aTargets": [3],
//                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
//                        if (sData === 1)
//                            $(nTd).attr('class', 'text-center fw-bold text-info');
//                        else if (sData === 2)
//                            $(nTd).attr('class', 'text-center fw-bold text-success');
//                        else
//                            $(nTd).attr('class', 'text-center fw-bold text-danger');

//                        $(nTd).html(sData = StatusToText(sData));
//                    }
//                }
//            ],

//            "order": [],

//            "lengthMenu": [
//                [15, 25, 50, 100, -1],
//                [15, 25, 50, 100, "Tümü"]
//            ],

//            "columns": [
//                {
//                    "orderable": true
//                },
//                {
//                    "orderable": true
//                },
//                {
//                    "orderable": true
//                },
//                {
//                    "orderable": true
//                }
//            ]
//        });

//        var tableWrapper = $('#Table1_wrapper');
//        tableWrapper.find('.dataTables_length select').select2({
//            minimumResultsForSearch: Infinity,
//        });
//    }

//    var initTable2 = function () {

//        var table = $('#Table2');

//        var otable = table.dataTable({

//            "language": {
//                "emptyTable": "Gösterilecek Veri Yok",
//                "info": "Toplam _TOTAL_ veriden _START_ ile _END_ arasındaki veriler gösteriliyor",
//                "infoEmpty": "",
//                "infoFiltered": "",
//                "lengthMenu": "_MENU_ Veri Göster",
//                "search": "Ara:",
//                "zeroRecords": "Eşleşen Veri Yok",
//                "paginate": {
//                    "previous": "Geri",
//                    "next": "İleri"
//                }
//            },

//            "fnCreatedRow": function (nRow, aData, iDataIndex) {
//            },

//            "columnDefs": [
//                {
//                    "searchable": false,
//                    "targets": [8]
//                },
//                {
//                    "aTargets": [0],
//                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
//                        $(nTd).attr('class', 'text-center');
//                        $(nTd).html(sData = formatDateTime(sData));
//                    }
//                },
//                {
//                    "aTargets": [1],
//                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
//                        $(nTd).attr('class', 'text-center');
//                    }
//                },
//                {
//                    "aTargets": [2],
//                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
//                    }
//                },
//                {
//                    "aTargets": [3],
//                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
//                        $(nTd).attr('class', 'text-center');
//                    }
//                },
//                {
//                    "aTargets": [4],
//                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
//                    }
//                },
//                {
//                    "aTargets": [5],
//                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
//                        $(nTd).attr('class', 'text-end');
//                    }
//                },
//                {
//                    "aTargets": [6],
//                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
//                        if (sData === 1)
//                            $(nTd).attr('class', 'text-center fw-bold text-info');
//                        else if (sData === 2)
//                            $(nTd).attr('class', 'text-center fw-bold text-success');
//                        else
//                            $(nTd).attr('class', 'text-center fw-bold text-danger');

//                        $(nTd).html(sData = StatusToText(sData));
//                    }
//                },
//                {
//                    "aTargets": [7],
//                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
//                        $(nTd).attr('class', 'text-center');
//                        $(nTd).html(sData = formatDateTime(sData));
//                    }
//                },
//                {
//                    "aTargets": [8],
//                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
//                        $(nTd).attr('class', 'text-center');
//                        $(nTd).html(sData = '<a href="/Process/Detail/20/' + sData + '" target="_blank" >Detay</a>');
//                    }
//                }
//            ],

//            "order": [],

//            "lengthMenu": [
//                [15, 25, 50, 100, -1],
//                [15, 25, 50, 100, "Tümü"]
//            ],

//            "columns": [
//                {
//                    "orderable": true
//                },
//                {
//                    "orderable": true
//                },
//                {
//                    "orderable": true
//                },
//                {
//                    "orderable": true
//                },
//                {
//                    "orderable": true
//                },
//                {
//                    "orderable": true
//                },
//                {
//                    "orderable": true
//                },
//                {
//                    "orderable": true
//                },
//                {
//                    "orderable": true
//                }
//            ]
//        });

//        var tableWrapper = $('#Table2_wrapper');
//        tableWrapper.find('.dataTables_length select').select2({
//            minimumResultsForSearch: Infinity,
//        });
//    }

//    var initTable3 = function () {

//        var table = $('#Table3');

//        var otable = table.dataTable({

//            "language": {
//                "emptyTable": "Gösterilecek Veri Yok",
//                "info": "Toplam _TOTAL_ veriden _START_ ile _END_ arasındaki veriler gösteriliyor",
//                "infoEmpty": "",
//                "infoFiltered": "",
//                "lengthMenu": "_MENU_ Veri Göster",
//                "search": "Ara:",
//                "zeroRecords": "Eşleşen Veri Yok",
//                "paginate": {
//                    "previous": "Geri",
//                    "next": "İleri"
//                }
//            },

//            "fnCreatedRow": function (nRow, aData, iDataIndex) {
//            },

//            "columnDefs": [
//                {
//                    "searchable": false,
//                    "targets": [8]
//                },
//                {
//                    "aTargets": [0],
//                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
//                        $(nTd).attr('class', 'text-center');
//                        $(nTd).html(sData = formatDateTime(sData));
//                    }
//                },
//                {
//                    "aTargets": [1],
//                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
//                        $(nTd).attr('class', 'text-center');
//                    }
//                },
//                {
//                    "aTargets": [2],
//                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
//                        $(nTd).attr('class', 'text-center');
//                    }
//                },
//                {
//                    "aTargets": [3],
//                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
//                        $(nTd).attr('class', 'text-center');
//                    }
//                },
//                {
//                    "aTargets": [4],
//                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
//                    }
//                },
//                {
//                    "aTargets": [5],
//                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
//                        $(nTd).attr('class', 'text-end');
//                    }
//                },
//                {
//                    "aTargets": [6],
//                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
//                        if (sData === 1)
//                            $(nTd).attr('class', 'text-center fw-bold text-info');
//                        else if (sData === 2)
//                            $(nTd).attr('class', 'text-center fw-bold text-success');
//                        else
//                            $(nTd).attr('class', 'text-center fw-bold text-danger');

//                        $(nTd).html(sData = StatusToText(sData));
//                    }
//                },
//                {
//                    "aTargets": [7],
//                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
//                        $(nTd).attr('class', 'text-center');
//                        $(nTd).html(sData = formatDateTime(sData));
//                    }
//                },
//                {
//                    "aTargets": [8],
//                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
//                        $(nTd).attr('class', 'text-center');
//                        $(nTd).html(sData = '<a href="/Process/Detail/100/' + sData + '" target="_blank" >Detay</a>');
//                    }
//                }
//            ],

//            "order": [],

//            "lengthMenu": [
//                [15, 25, 50, 100, -1],
//                [15, 25, 50, 100, "Tümü"]
//            ],

//            "columns": [
//                {
//                    "orderable": true
//                },
//                {
//                    "orderable": true
//                },
//                {
//                    "orderable": true
//                },
//                {
//                    "orderable": true
//                },
//                {
//                    "orderable": true
//                },
//                {
//                    "orderable": true
//                },
//                {
//                    "orderable": true
//                },
//                {
//                    "orderable": true
//                },
//                {
//                    "orderable": true
//                },
//            ]
//        });

//        var tableWrapper = $('#Table2_wrapper');
//        tableWrapper.find('.dataTables_length select').select2({
//            minimumResultsForSearch: Infinity,
//        });
//    }

//    var initData = function () {
//        $.ajax({
//            type: "POST",
//            url: '/RebateRequest/Gets/',
//            contentType: 'application/json; charset=utf-8',
//            data: JSON.stringify({
//                IDMember: $("#PaymentNotification_IDMember").val(),
//                Status: null,
//                StartDate: null,
//                EndDate: null
//            }),
//            beforeSend: function () {
//                showLoading();

//                var otbl = $('#Table1').DataTable();
//                otbl.clear().draw();
//            },
//            success: function (list) {
//                var grid = [];
//                if (list)
//                    $.each(list, function (i, item) {
//                        grid.push([item.mDate, item.amount.toFixed(2), item.modifier, item.status]);
//                    })

//                var otbl = $('#Table1').DataTable();
//                otbl.rows.add(grid);
//                otbl.draw();
//            },
//            error: function () {
//                onFailure();
//            },
//            complete: function () {
//                hideLoading();
//            }
//        });

//        $.ajax({
//            type: "POST",
//            url: '/PaymentNotification/Gets/',
//            contentType: 'application/json; charset=utf-8',
//            data: JSON.stringify({
//                IDMember: $("#PaymentNotification_IDMember").val(),
//                StartDate: null,
//                EndDate: null
//            }),
//            beforeSend: function () {
//                showLoading();

//                var otbl = $('#Table2').DataTable();
//                otbl.clear().draw();
//            },
//            success: function (list) {
//                var grid = [];
//                if (list)
//                    $.each(list, function (i, item) {
//                        grid.push([item.cDate, item.transactionNr, item.bank, formatDate(item.actionDate) + ' ' + item.actionTime, item.senderName, item.amount.toFixed(2), item.status, item.mDate, item.id]);
//                    })

//                var otbl = $('#Table2').DataTable();
//                otbl.rows.add(grid);
//                otbl.draw();
//            },
//            error: function () {
//                onFailure();
//            },
//            complete: function () {
//                hideLoading();
//            }
//        });

//        $.ajax({
//            type: "POST",
//            url: '/CreditCardPaymentNotification/Gets/',
//            contentType: 'application/json; charset=utf-8',
//            data: JSON.stringify({
//                IDMember: $("#CreditCardPaymentNotification_IDMember").val(),
//                StartDate: null,
//                EndDate: null
//            }),
//            beforeSend: function () {
//                showLoading();

//                var otbl = $('#Table3').DataTable();
//                otbl.clear().draw();
//            },
//            success: function (list) {
//                var grid = [];
//                if (list)
//                    $.each(list, function (i, item) {
//                        grid.push([item.cDate, item.transactionNr, item.senderName, item.cardNumber, item.senderIdentityNr,  item.amount.toFixed(2), item.status, item.mDate, item.id]);
//                    })

//                var otbl = $('#Table3').DataTable();
//                otbl.rows.add(grid);
//                otbl.draw();
//            },
//            error: function () {
//                onFailure();
//            },
//            complete: function () {
//                hideLoading();
//            }
//        });
//    }

//    function StatusToText(status) {
//        if (status === 1)
//            return 'BEKLİYOR';
//        else if (status === 2)
//            return 'ONAYLANDI';
//        else
//            return 'İPTAL EDİLDİ';
//    }

//    return {
//        init: function () {
//            if (!jQuery().dataTable) {
//                return;
//            }
//            initTable1();
//            initTable2();
//            initTable3();
//            $("#others").removeClass("active").addClass("fade");
//            $("#creditCard").removeClass("active").addClass("fade");
//            if ($("#PaymentNotification_IDMember").val() !== "0")
//                initData();
//        }
//    };

//}();


function aktifTabPage() {

    async function bekletmeSuresi() {
        await new Promise(resolve => setTimeout(resolve, 10000));

        var activeTabId = "home"; // Aktif sekme ID'sini burada ayarlayın

        var tabLink = document.querySelector('a[href="#' + activeTabId + '"]');
        if (tabLink) {
            tabLink.click();
        }
    }
}