var urlLIST = null;

const towns = $.parseJSON($("#jsonTowns").val());

$(document).ready(function () {
    $("#frmDef").validate({
        errorElement: 'div',
        errorClass: 'text-danger',
        rules: {
            "entity.IDCity": {
                required: true
            },
            "entity.IDTown": {
                required: true
            },
            "entity.Address": {
                required: true
            }
        },
        messages: {
            "entity.IDCity": {
                required: "Lütfen il seçiniz"
            },
            "entity.IDTown": {
                required: "Lütfen ilçe seçiniz"
            },
            "entity.Address": {
                required: "Lütfen adres giriniz"
            }
        }
    })

    var firstVal = $("#entity_IDTown").val();
    $("#entity_IDCity").change(function (e) {

        $('#entity_IDTown').empty()

        $("#entity_IDTown").append(new Option('Seçiniz', '', true, false))

        $.each(towns, function (index, item) {
            if (e.target.value === item.IDCity)
                $("#entity_IDTown").append(new Option(item.Name, item.ID, false, (item.ID === firstVal)))
        })
    })

})