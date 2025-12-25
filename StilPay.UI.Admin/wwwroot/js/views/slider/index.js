var urlEDIT = '/slider/edit/__id__';
var urlDELETE = '/slider/drop';

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
            "url": "/Slider/GetData",
            "type": "POST",
            //"data": function (d) {
            //    d.IDCompany = $("#slcCompanies").val();
            //    d.StartDate = $("#dtStartDate").val();
            //    d.EndDate = $("#dtEndDate").val();
            //},
        },
        "columns": [
            { "data": "cDate", "orderable": false },
            { "data": "creator", "orderable": false },
            { "data": "mDate", "orderable": false },
            { "data": "modifier", "orderable": false },
            { "data": "name", "orderable": false },
            { "data": "header", "orderable": false },
            { "data": "imageUrl", "orderable": false },
            { "data": "orderNr", "orderable": false },
            { "data": "id", "orderable": false },
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
                    $(nTd).html(sData = formatDateTime(sData));
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
                    $(nTd).attr('class', 'text-center');
                }
            },
            {
                "aTargets": [6],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).html('<img src="' + sData + '" alt="image" style="max-width: 100px; max-height: 100px;">');
                    $(nTd).attr('class', 'text-end');

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
                    $(nTd).html(sData = '<a href="/Slider/Edit/' + sData + '" target="_blank" >Detay</a>');
                }
            },
            {
                "aTargets": [9],
                "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).attr('class', 'text-center');
       
                    $(nTd).html(
                        '<div class="btn-group">' +
                        '    <button type="button" class="btn btn-danger btn-sm" style="border-radius: 8px;"onclick="drop(\'' + sData + '\')">' +
                        '        <i class="fa fa-trash"></i> ' +
                        '    </button>' +
                        '</div>'
                    );
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