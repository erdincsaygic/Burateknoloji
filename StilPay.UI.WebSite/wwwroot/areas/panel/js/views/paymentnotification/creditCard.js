var urlRedirect = '/panel/paymentnotification/validation';

$(document).ready(function () {
    $("#creditCardForm").validate({
        errorElement: 'span',
        errorClass: 'text-danger',
        rules: {
            "CreditCardModel.SenderName": {
                required: true,
            },
            "CreditCardModel.CardNumber": {
                required: true,
                minlength: 16
            },
            "CreditCardModel.ExpirationDate": {
                required: true,
            },
            "CreditCardModel.SecurityCode": {
                required: true,
                minlength: 3
            },
            "CreditCardModel.PhoneNumber": {
                required: true,
                digits: true,
                minlength: 10
            }, 
            "CreditCardModel.CountryCode": {
                required: true,
            }, 
        },
        messages: {
            "CreditCardModel.SenderName": {
                required: "Ad-Soyad giriniz"
            },
            "CreditCardModel.CardNumber": {
                required: "Kredi kartı numarası giriniz.",
                minlength: "Kredi Kartı 16 haneli olmalıdır."
            },
            "CreditCardModel.ExpirationDate": {
                required: "Son kullanma tarihi giriniz",
            },
            "CreditCardModel.SecurityCode": {
                required: "CVV giriniz",
                minlength: "CVV 3 rakamdan oluşmaktadır"
            },
            "CreditCardModel.PhoneNumber": {
                required: "Telefon numarası başında '0' olmadan giriniz",
                digits: "Rakamlardan oluşmalıdır",
                minlength: "10 haneli olmalıdır",
                maxlength: "En fazla 10 hane girebilirsiniz"
            },
            "CreditCardModel.CountryCode": {
                required: "Ülke kodu boş bırakılamaz",
            },
        }
    })

    var onlyLettersRule = document.getElementById('name');
    onlyLettersRule.addEventListener("keyup", function () {
        if (onlyLettersRule.value.match(/[^a-zA-Z' 'wığüşöçĞÜŞÖÇİ]/g)) {
            onlyLettersRule.value = this.value.replace(/[^a-zA-Z' 'wığüşöçĞÜŞÖÇİ]/g, '');
        }
    })

    $("#phone").attr('maxlength', 10);
    $('#phone').val("");
    $('#cardnumber').val("");
    $('#phone').keyup( function (e) {
        if (this.value[0] == 0)
            this.value = "";
        if (this.value.length > this.maxLength)
            this.value = this.value.slice(0, this.maxLength);       
    });


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

    $('table').on("change", "input[type='radio']", function (e) {
        if ($(this).is(':checked')) {
            e.target.value = e.target.id;
            
            $('#installmentAmount').val($(this).parent().next().next().text());
        }
    });

    $("#cardnumber").bind('cut', function () {
        document.getElementById("personel").innerHTML = "";
    });

    $("#cardnumber").bind("paste", function (e) {
        document.getElementById("cardnumber").value = "";
    });

    setTimeout(function () {
        history.back();
    }, 600000);

    history.pushState(null, document.title, location.href);
});



function getCreditCardNumber() {
    var creditCardNumber = document.getElementById("cardnumber").value.replace(/\D/g, "");
    document.getElementById("personel").innerHTML = "";
    if (creditCardNumber.length > 16) {
        creditCardNumber = creditCardNumber.slice(0, 16);    
    }
    if (creditCardNumber.length == 16) {
        $.ajax({ 
            type: 'GET',
            url: '/Panel/PaymentNotification/InstallmentOptions',
            data: { creditCardNumber },
            success: function (data) {
                if (data.status == "ERROR") {
                    alertify.notify(data.message, 'error', 5, function () { }).dismissOthers();
                }
                else {
                    getInstallmentOptionsValues(data)
                }
            },
            error: function (data) {
                alertify.notify(data.message, 'error', 5, function () { }).dismissOthers();
                setTimeout(function () {
                    history.back();
                }, 5000)
            }
        })
    }
}

function getInstallmentOptionsValues(data) {
    showLoading();
    setTimeout(function () {
        $.each(data, function (key, value) {
            if (parseInt(key) == 1) {
                $('#personel')
                    .append(`<tr>`)
                    .append(`<th></th>`)
                    .append(`<th>Taksit Sayısı</th>`)
                    .append(`<th>Ödeme</th>`)
                    .append(`</tr>`)
                    .append(`<tr>`)
                    .append(`<td><input type="radio" id=${parseInt(key)} name="CreditCardModel.InstallmentMonth" value="1" checked></td>`)
                    .append(`<td><label for=${parseInt(key)}>Tek Çekim</label></td>`)
                    .append(`<td>${value}</td>`)
                    .append(`</tr>`)

                $('#installmentAmount').val(value);
            }
            else {
                $('#personel')
                    .append(`<tr>`)
                    .append(`<td><input type="radio" id=${parseInt(key)} name="CreditCardModel.InstallmentMonth"></td>`)
                    .append(`<td><label for=${parseInt(key)}>${key}</label></td>`)
                    .append(`<td>${value}</td>`)
                    .append(`</tr>`)
            }
        });
    }, 1000);
    hideLoading();
}

function showLoading() {
    Swal.fire({
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
}

function hideLoading() {
    setTimeout(function () {
        Swal.close();
    }, 500);
}


function onBegin() {
    $("#btnSubmit").removeClass("hovers");
    $("#btnSubmit").attr("disabled", "disabled");

    showLoading();
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
    hideLoading();
}

function onSuccess(response) {
    setTimeout(function () {
        if (response.status === "ERROR") {
            if (response.message && response.message == "hasEntity") {
                window.location.href = response.data;
            } else {
                alertify.notify(response.message, 'error', 5, function () { }).dismissOthers();
                setTimeout(function () {
                    history.back();
                }, 5000)

                $("#btnSubmit").addClass("hovers");
                $("#btnSubmit").removeAttr("disabled");
                hideLoading();
            }
        }
        else
            window.location.href = urlRedirect;
    }, 1000)
}

