var urlEDIT = '/PaymentTransferPoolDescriptionControl/edit/__id__';
var urlDELETE = '/PaymentTransferPoolDescriptionControl/drop';
var urlLIST = '/PaymentTransferPoolDescriptionControl';

$(document).ready(function () {

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
            "url": "/PaymentTransferPoolDescriptionControl/GetData",
            "type": "POST"
        },
        "columns": [
            { "data": "cDate", "orderable": false },
            { "data": "creator", "orderable": false },
            { "data": "name", "orderable": false },
            { "data": "cardNumber", "orderable": false },
            { "data": "phone", "orderable": false },
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
            //{
            //    "aTargets": [2],
            //    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
            //        $(nTd).attr('class', 'text-center');
            //        $(nTd).html(sData = formatDateTime(sData));
            //    }
            //},
            //{
            //    "aTargets": [3],
            //    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
            //        $(nTd).attr('class', 'text-center');
            //    }
            //},
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
                    $(nTd).html(
                        '<div class="btn-group" style="width:75px">' +
                        //'    <button type="button" class="btn btn-custom text-white btn-sm" style="margin-right: 5px; border-radius: 8px;" onclick="edit(\'' + sData + '\')">' +
                        //'        <i class="fa fa-pencil"></i> ' +
                        //'    </button>' +
                        '    <button type="button" class="btn btn-danger btn-sm" style="border-radius: 8px;"onclick="drop(\'' + sData + '\')">' +
                        '        <i class="fa fa-trash"></i> ' +
                        '    </button>' +
                        '</div>'
                    );
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
