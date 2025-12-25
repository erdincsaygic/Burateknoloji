var urlEDIT = '';
var urlDELETE = '';
var urlLIST = '/AccountSummary';
var pageOpened = true;
$(document).ready(function () {
    $("#btnList").click(function () {
        getData();
    });

    if (pageOpened) {
        initMonthAndYearSelection(true, 0,0);
    }




});

function getData() {
    $.ajax({
        type: "GET",
        url: '/AccountSummary/GetDataMonthlyAccountSummary',
        data: {
            year: $("#slcYear").val(),
            month: $("#slcMonth").val()
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (response) {
            if (response.status == "ERROR") {
                alertify.notify(response.message, 'error', 5, function () { }).dismissOthers();
            }
            else {
                var selectedMonth = $(response).find('#selectedMonth').val();
                var selectedYear = $(response).find('#selectedYear').val();

                $("body").html(response);

                initMonthAndYearSelection(false, selectedMonth, selectedYear);

                $("#slcMonth").val(selectedMonth).trigger('change');
                $("#slcYear").val(selectedYear).trigger('change');
            }
        },
        error: function (response) {
            alertify.notify(response.message, 'error', 5, function () { }).dismissOthers();
        },
        complete: function () {
            hideLoading();
            pageOpened = false;
        }
    });
}
//function getData() {
//    $('#Table').DataTable({
//        destroy: true,
//        serverSide: true,
//        processing: true,
//        "language": {
//            "emptyTable": "Gösterilecek Veri Yok",
//            "info": "Toplam _TOTAL_ veriden _START_ ile _END_ arasındaki veriler gösteriliyor",
//            "infoEmpty": "",
//            "infoFiltered": "",
//            "lengthMenu": "_MENU_ Veri Göster",
//            "search": "Ara:",
//            "zeroRecords": "Eşleşen Veri Yok",
//            "paginate": {
//                "previous": "Geri",
//                "next": "İleri"
//            }
//        },
//        "ajax": {
//            "url": "/AccountSummary/GetDataMonthlyAccountSummary",
//            "type": "POST",
//        },
//        "columns": [
//            { "data": "cDate", "orderable": false },
//            { "data": "reportNo", "orderable": false },
//            { "data": "creator", "orderable": false },
//            { "data": "netAmount", "orderable": false },
//            { "data": "profit", "orderable": false },
//            { "data": "difference", "orderable": false },
//            { "data": "id", "orderable": false },
//            { "data": "id", "orderable": false },
//        ],

//        "columnDefs": [
//            {
//                "aTargets": [0],
//                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
//                    $(nTd).attr('class', 'text-center');
//                    $(nTd).html(sData = formatDateTime(sData));
//                }
//            },
//            {
//                "aTargets": [1],
//                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
//                    $(nTd).attr('class', 'text-center');
//                }
//            },
//            {
//                "aTargets": [2],
//                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
//                    $(nTd).attr('class', 'text-center');
//                }
//            },
//            {
//                "aTargets": [3],
//                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
//                    $(nTd).attr('class', 'text-center');
//                }
//            },
//            {
//                "aTargets": [4],
//                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
//                    $(nTd).attr('class', 'text-center');
//                }
//            },
//            {
//                "aTargets": [5],
//                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
//                    $(nTd).attr('class', 'text-center');
//                }
//            },
//            {
//                "aTargets": [6],
//                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
//                    $(nTd).attr('class', 'text-center');
//                    $(nTd).html(sData = '<a href="/AccountSummary/Detail/' + sData + '" target="_blank" >Detay</a>');
//                }
//            },
//            {
//                "aTargets": [7],
//                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
//                    $(nTd).attr('class', 'text-center');
//                    $(nTd).html(sData = '<a class="dropdown-item text-danger" href="javascript:void(0)" onclick="deleteReport(\'' + sData + '\')"><i class="fa fa-trash"></i> Sil</a>');
//                }
//            }
//        ],

//        "order": [],

//        "lengthMenu": [
//            [15, 25, 50, 100, -1],
//            [15, 25, 50, 100, "Tümü"]
//        ],
//    });
//}


//function deleteReport(id) {
//    alertify.dialog('confirm').set({ transition: 'slide' });
//    alertify.confirm('Closable: false').set('closable', false).set('labels', { ok: 'EVET', cancel: 'HAYIR' });
//    alertify.confirm(
//        'KAYIT SİL',
//        '<p class="text-danger fs-6 fw-bold">Emin misiniz ?</p>',
//        function () {
//            showLoading();
//            $.ajax({
//                type: "POST",
//                url: "/AccountSummary/Delete",
//                data: { id },
//                success: function (response) {
//                    if (response.status == "ERROR") {
//                        alertify.notify(response.message, 'error', 5, function () { }).dismissOthers();
//                    }
//                    else {
//                        alertify.notify('İşlem Başarılı', 'success', 5, function () { }).dismissOthers();
//                        setTimeout(function () {
//                            window.location.reload();
//                        }, 2000)
//                    }
//                },
//                error: function (response) {
//                    alertify.notify(response.message, 'error', 5, function () { }).dismissOthers();
//                    setTimeout(function () {
//                        history.back();
//                    }, 5000)
//                },
//                complete: function () {
//                    hideLoading();
//                }
//            });
//        },
//        function () {
//        }
//    );


//}