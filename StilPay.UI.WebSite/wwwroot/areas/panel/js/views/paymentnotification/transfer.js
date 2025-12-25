var urlRedirect = '/panel/paymentnotification/information';

const banks = $.parseJSON($("#jsonCompanyBanks").val());

$(document).ready(function () {
    $("#frmTransfer").validate({
        errorElement: 'span',
        errorClass: 'text-danger',
        rules: {
            "entity.IDBank": {
                required: true
            },
            "entity.Phone": {
                required: true,
                digits: true,
                minlength: 10
            },
            "entity.Amount": {
                required: true,
            },
            "entity.ActionDate": {
                required: true
            },
            "entity.SenderName": {
                required: true
            },
            //"entity.SenderIdentityNr": {
            //    required: true,
            //    digits: true,
            //    minlength: 11
            //}
        },
        messages: {
            "entity.IDBank": {
                required: "Banka seçiniz"
            },
            "entity.Phone": {
                required: "Telefon giriniz",
                digits: "Rakamlardan oluşmalıdır",
                minlength: "10 haneli olmalıdır"
            },
            "entity.Amount": {
                required: "Tutar giriniz",
            },
            "entity.ActionDate": {
                required: "Tarih seçiniz"
            },
            "entity.SenderName": {
                required: "Ad-Soyad giriniz"
            },
            //"entity.SenderIdentityNr": {
            //    required: "TC Kimlik No giriniz",
            //    digits: "Rakamlardan oluşmalıdır",
            //    minlength: "11 haneli olmalıdır"
            //}
        }
    })


    $('#phone').keyup(function (e) {
        if (this.value[0] == 0)
            this.value = "";
        if (this.value.length > this.maxLength)
            this.value = this.value.slice(0, this.maxLength);
    });

    $(".bank").click(function () {
        $(".bank").removeClass("hoveractive");
        $(this).addClass("hoveractive");
        var idBank = $(this).attr("data-bank-id");
        $("#entity_IDBank").val(idBank);
        InitializeBankSection(idBank);
    });

    $('#phone').val("");

    $("#box3").prop('checked', true);
    $("#btnSubmit").addClass("hovers");
    $("#btnSubmit").removeAttr("disabled");

    $("#box3").click(function () {
        if ($(this).prop("checked") == true) {
            $("#btnSubmit").addClass("hovers");
            $("#btnSubmit").removeAttr("disabled");

        }
        else if ($(this).prop("checked") == false) {
            alertify.notify('Lütfen Mesafeli Satış Sözleşmesini Onaylayınız', 'error', 5, function () { }).dismissOthers();
            $("#btnSubmit").removeClass("hovers");
            $("#btnSubmit").attr("disabled", "disabled");
        }
    })

    setTimeout(function () {
        history.back();
    }, 600000)

    if (banks && banks.length > 0) {
        InitializeBankSection(banks[0].CompanyBankAccountID);
        $("#imgBank").attr("src", '../img/banks/' + banks[0].Img);
        $("#spanBank").text((banks[0].Bank ?? '-'));
        $("#spanIban").text(banks[0].IBAN ?? "-");
        $("#spanAlici").text(banks[0].Title ?? "-");
        $("#spanAccountNo").text(banks[0].AccountNr ?? "-");
    }
});

function InitializeBankSection(idBank) {

    if (banks && banks.length > 0 && idBank) {
        for (i = 0; i <= banks.length; i++) {
            if (banks[i].CompanyBankAccountID === idBank) {
                $("#entity_IDBank").val(banks[i].IDBank);
                $("#entity_CompanyBankAccountID").val(banks[i].CompanyBankAccountID);
                $("#bankIframeWarnText").text((banks[i].IFrameWarnText ?? ""))
                $("#imgBank").attr("src", '/areas/panel/img/banks/' + banks[i].Img);
                $("#spanBank").text((banks[i].Bank ?? '-'));
                $("#spanIban").text((banks[i].IBAN ?? '-'));
                $("#spanAlici").text((banks[i].Title ?? '-'));
                $("#spanAccountNo").text(banks[i].AccountNr ?? "-");
                break;
            }

        }
    }
}

function onBegin() {
    $("#btnSubmit").removeClass("hovers");
    $("#btnSubmit").attr("disabled", "disabled");
}

function onFailure() {
    setTimeout(function () {
        Swal.fire({
            title: '<p class="fs-4 text-danger">Opps!</p>',
            showConfirmButton: true,
            confirmButtonText: "Tamam",
            confirmButtonColor: "#dc3545",
            allowOutsideClick: false,
            allowEscapeKey: false,
        })
    }, 1000)

    $("#btnSubmit").addClass("hovers");
    $("#btnSubmit").removeAttr("disabled");
}

function onSuccess(response) {
    setTimeout(function () {
        if (response.status === "ERROR")
            window.location.reload();
        else
            window.location.href = urlRedirect;
    }, 1000)
}

document.getElementById("spanIban").onclick = (e) => {
    navigator.clipboard.writeText(e.currentTarget.innerText);
    alertify.notify('IBAN Bilgisi Kopyalandı', 'success', 5, function () { }).dismissOthers();
};

document.getElementById("spanAlici").onclick = (e) => {
    navigator.clipboard.writeText(e.currentTarget.innerText);
    alertify.notify('Hesap Adı Bilgisi Kopyalandı', 'success', 5, function () { }).dismissOthers();
};

document.getElementById("spanAccountNo").onclick = (e) => {
    navigator.clipboard.writeText(e.currentTarget.innerText);
    alertify.notify('Hesap No Bilgileri Kopyalandı', 'success', 5, function () { }).dismissOthers();
};


function copyIban(e) {
    var val = document.getElementById("spanIban").innerText;
    navigator.clipboard.writeText(val);
    alertify.notify('IBAN Bilgisi Kopyalandı', 'success', 5, function () { }).dismissOthers();
}

function copyAlici(e) {
    var val = document.getElementById("spanAlici").innerText;
    navigator.clipboard.writeText(val);
    alertify.notify('Hesap Adı Bilgisi Kopyalandı', 'success', 5, function () { }).dismissOthers();
}

function copyBranchCode(e) {
    var val = document.getElementById("branchCodeVal").innerText;
    navigator.clipboard.writeText(val);
    alertify.notify('Şube Kodu Bilgisi Kopyalandı', 'success', 5, function () { }).dismissOthers();
}

function copyAccountNo(e) {
    var val = document.getElementById("spanAccountNo").innerText;
    navigator.clipboard.writeText(val);
    alertify.notify('Hesap No Bilgileri Kopyalandı', 'success', 5, function () { }).dismissOthers();
}
