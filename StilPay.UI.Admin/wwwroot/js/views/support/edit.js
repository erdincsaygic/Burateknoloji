var urlLIST = '/support/pending';

$(document).ready(function () {
    $("#frmDef").validate({
        errorElement: 'div',
        errorClass: 'text-danger',
        rules: {
            "entity.Answer": {
                required: true
            }
        },
        messages: {
            "entity.Answer": {
                required: "Lütfen cevap giriniz"
            }
        }
    })

    Table.init();
});

var Table = function () {

    var initTable = function () {

        var table = $('#Table');

        var otable = table.dataTable({

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

            "fnCreatedRow": function (nRow, aData, iDataIndex) {
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
                        if (sData === 'CEVAP')
                            $(nTd).attr('class', 'text-center fw-bold text-success');
                        else
                            $(nTd).attr('class', 'text-center fw-bold text-danger');
                    }
                },
                {
                    "aTargets": [2],
                    "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
                    }
                }
            ],

            "order": [],

            "pageLength": 10,

            "lengthMenu": [
                [10, 25, 50, 100, -1],
                [10, 25, 50, 100, "Tümü"]
            ],

            "columns": [
                {
                    "orderable": false
                },
                {
                    "orderable": false
                },
                {
                    "orderable": false
                }
            ]
        });

        var tableWrapper = $('#Table_wrapper');
        tableWrapper.find('.dataTables_length select').select2({
            minimumResultsForSearch: Infinity,
        });
    }

    var initData = function () {
        $.ajax({
            type: "POST",
            url: '/Support/Gets/',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({
                IDMember: $("#entity_IDMember").val(),
                IDCompany: $("#entity_IDCompany").val(),
                StartDate: null,
                EndDate: null,
                Status: null
            }),
            beforeSend: function () {
                showLoading();

                var otbl = $('#Table').DataTable();
                otbl.clear().draw();
            },
            success: function (list) {
                var grid = [];
                if (list)
                    $.each(list, function (i, item) {
                        if (item.answer)
                            grid.push([item.mDate, 'CEVAP', item.answer]);
                        grid.push([item.cDate, 'SORU', item.question]);
                    })

                var otbl = $('#Table').DataTable();
                otbl.rows.add(grid);
                otbl.draw();
            },
            error: function () {
                onFailure();
            },
            complete: function () {
                hideLoading();
            }
        });
    }

    return {
        init: function () {
            if (!jQuery().dataTable) {
                return;
            }
            initTable();
            initData();
        }
    };

}();