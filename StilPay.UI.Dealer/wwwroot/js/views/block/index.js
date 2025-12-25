var urlLIST = null;

$(document).ready(function () {

    getData();
    getData2();
    getData3();
    getData4();
    getData5();
    getData6();

    aktifTabPage();

    //$("#creditCard").removeClass("active").addClass("fade");
});

function getData() {

    $('#TableBlocked').DataTable({
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
            "url": "/Block/GetBlockeds",
            "type": "POST",
        },
        "columnDefs": [{
            "targets": [3],
            "searchable": false
        }],
        "columns": [
            { "data": "mDate", "orderable": false },
            { "data": "transactionNr", "orderable": false },
            { "data": "netTotal", "orderable": false },
            { "data": "blockedDays", "orderable": false }
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
                    $(nTd).attr('class', 'text-end');
                    $(nTd).html(sData.toFixed(2));
                }
            },
            {
                "aTargets": [3],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                    $(nTd).html(sData = sData + ' Gün');
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

function getData2() {

    $('#TableNotBlocked').DataTable({
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
            "url": "/Block/GetNotBlockeds",
            "type": "POST",
        },
        "columnDefs": [{
            "targets": [3],
            "searchable": false
        }],
        "columns": [
            { "data": "mDate", "orderable": false },
            { "data": "transactionNr", "orderable": false },
            { "data": "netTotal", "orderable": false },
            { "data": "blockedDays", "orderable": false }
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
                    $(nTd).attr('class', 'text-end');
                    $(nTd).html(sData.toFixed(2));
                }
            },
            {
                "aTargets": [3],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                    $(nTd).html(sData = sData + ' Gün');
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

function getData3() {

    $('#CreditCardTableBlocked').DataTable({
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
            "url": "/Block/GetCreditCardBlockeds",
            "type": "POST",
        },
        "initComplete": function (settings, json) {
            $('#CreditCardTableBlocked').removeAttr('style');
        },
        "columnDefs": [{
            "targets": [3],
            "searchable": false
        }],
        "columns": [
            { "data": "mDate", "orderable": false },
            { "data": "transactionNr", "orderable": false },
            { "data": "netTotal", "orderable": false },
            { "data": "blockedDays", "orderable": false }
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
                    $(nTd).attr('class', 'text-end');
                    $(nTd).html(sData.toFixed(2));
                }
            },
            {
                "aTargets": [3],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                    $(nTd).html(sData = sData + ' Gün');
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

function getData4() {

    $('#CreditCardTableNotBlocked').DataTable({
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
            "url": "/Block/GetCreditCardNotBlockeds",
            "type": "POST",
        },
        "initComplete": function (settings, json) {
            $('#CreditCardTableNotBlocked').removeAttr('style');
        },
        "columnDefs": [{
            "targets": [3],
            "searchable": false
        }],
        "columns": [
            { "data": "mDate", "orderable": false },
            { "data": "transactionNr", "orderable": false },
            { "data": "netTotal", "orderable": false },
            { "data": "blockedDays", "orderable": false }
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
                    $(nTd).attr('class', 'text-end');
                    $(nTd).html(sData.toFixed(2));
                }
            },
            {
                "aTargets": [3],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                    $(nTd).html(sData = sData + ' Gün');
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

function getData5() {

    $('#ForeignCreditCardTableBlocked').DataTable({
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
            "url": "/Block/GetForeignCreditCardBlockeds",
            "type": "POST",
        },
        "initComplete": function (settings, json) {
            $('#ForeignCreditCardTableBlocked').removeAttr('style');
        },
        "columnDefs": [{
            "targets": [3],
            "searchable": false
        }],
        "columns": [
            { "data": "mDate", "orderable": false },
            { "data": "transactionNr", "orderable": false },
            { "data": "netTotal", "orderable": false },
            { "data": "blockedDays", "orderable": false }
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
                    $(nTd).attr('class', 'text-end');
                    $(nTd).html(sData.toFixed(2));
                }
            },
            {
                "aTargets": [3],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                    $(nTd).html(sData = sData + ' Gün');
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

function getData6() {

    $('#ForeignCreditCardTableNotBlocked').DataTable({
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
            "url": "/Block/GetForeignCreditCardNotBlockeds",
            "type": "POST",
        },
        "initComplete": function (settings, json) {
            $('#ForeignCreditCardTableNotBlocked').removeAttr('style');
        },
        "columnDefs": [{
            "targets": [3],
            "searchable": false
        }],
        "columns": [
            { "data": "mDate", "orderable": false },
            { "data": "transactionNr", "orderable": false },
            { "data": "netTotal", "orderable": false },
            { "data": "blockedDays", "orderable": false }
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
                    $(nTd).attr('class', 'text-end');
                    $(nTd).html(sData.toFixed(2));
                }
            },
            {
                "aTargets": [3],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
                    $(nTd).html(sData = sData + ' Gün');
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

function aktifTabPage() {

    async function bekletmeSuresi() {
        await new Promise(resolve => setTimeout(resolve, 10000)); 

        var activeTabId = "bank"; // Aktif sekme ID'sini burada ayarlayın

        var tabLink = document.querySelector('a[href="#' + activeTabId + '"]');
        if (tabLink) {
            tabLink.click();
        }
    }


}