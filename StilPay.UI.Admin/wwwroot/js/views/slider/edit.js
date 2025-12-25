var urlLIST = '/Slider/Index'

$(document).ready(function () {

    const showButtonSelect = document.getElementById("showButtonSelect");
    const buttonDetails = document.getElementById("buttonDetails");

    function toggleButtonDetails() {
        if (showButtonSelect.value === "true") {
            buttonDetails.style.display = "block";
        } else {
            buttonDetails.style.display = "none";
        }
    }

    // Initialize the display on page load
    toggleButtonDetails();

    // Listen for changes on the dropdown
    showButtonSelect.addEventListener("change", toggleButtonDetails);


    const fileInput = document.getElementById('imageInput');
    const preview = document.getElementById('imagePreview');
    const existingImageUrl = document.getElementById('existingImageUrl').value;

    // Check if there's an existing image URL and display the preview if so
    if (existingImageUrl) {
        preview.src = existingImageUrl;
        preview.style.display = 'block';
    }

    // Listen for changes on the file input
    fileInput.addEventListener('change', function (event) {
        const file = event.target.files[0];

        if (file) {
            const reader = new FileReader();

            reader.onload = function (e) {
                preview.src = e.target.result;
                preview.style.display = 'block';
            };

            reader.readAsDataURL(file);
        } else {
            preview.src = '#';
            preview.style.display = 'none';
        }
    });

    $("#frmDef").validate({
        errorElement: 'span',
        errorClass: 'text-danger',
        rules: {
            "entity.OrderNr": {
                required: true,
                min: 1
            },
            "entity.Name": {
                required: true,
            }
        },
        messages: {
            "entity.OrderNr": {
                required: "Lütfen sira belirtin",
                min: "En az bir sayı girin",
            },
            "entity.Name": {
                required: "Lütfen slider adı belirtin",
            }
        }
    })
});
