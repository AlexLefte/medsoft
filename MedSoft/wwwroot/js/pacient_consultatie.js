$(document).ready(function () {
    $("#specializareDropdown").change(function () {
        var selectedSpecializareID = $(this).val();

        // Make an AJAX request to the server to get the medics based on the selected SpecializareID
        $.ajax({
            url: '/pacient/consultatie/GetMediciBySpecializare',
            type: 'POST',
            data: { specializareID: selectedSpecializareID },
            success: function (medici) {
                // Update the medic dropdown options based on the response
                var medicDropdown = $("#medicDropdown");

                medicDropdown.prop('disabled', false);

                medicDropdown.empty(); // Clear existing options

                medicDropdown.append('<option value="">--Selecteaza Medic--</option>');

                $.each(medici, function (index, medic) {
                    medicDropdown.append('<option value="' + medic.value + '">' + medic.text + '</option>');
                });
            },
            error: function (error) {
                console.error(error);
            }
        });
    });

    $("#dateInput").change(function () {
        var selectedDate = $(this).val();
        var selectedMedicID = $("#medicDropdown").val();
        var selectedConsultatieID = $("#consultatieID").val();

        // Medicul este selectat din home page
        if (window.medicID != '') {
            selectedMedicID = window.medicID;
        }

        timeInput.disabled = false;

        // Make an AJAX request to the server to get the medics based on the selected SpecializareID
        $.ajax({
            url: '/pacient/consultatie/GetOreDisponibile',
            type: 'POST',
            data: {
                data: selectedDate,
                medicID: selectedMedicID,
                consultatieID: selectedConsultatieID
            },
            success: function (oreDisponibile) {
                // Update the hour picker options based on the response
                var timePicker = $("#timeInput");
                timePicker.empty(); // Clear existing options

                $.each(oreDisponibile, function (index, hour) {
                    timePicker.append('<option value="' + hour + '">' + hour + '</option>');
                });
            },
            error: function (error) {
                console.error(error);
            }
        });
    });

    $("#medicDropdown").change(function () {
        var selectedMedicID = $(this).val();

        if (selectedMedicID === 0) {
            dateInput.disabled = true;
        } else {
            dateInput.disabled = false;

            // Trigger the change event on dateInput
            $(dateInput).trigger('change');
        }
    });

    $("#timeInput").change(function () {
        var selectedTime = $(this).val();

        if (selectedTime === 0) {
            pacientDropdown.disabled = true;
        } else {
            pacientDropdown.disabled = false;
        }
    });

    if (window.medicID != '') {
        // Preia elementul ce contine numele medicului
        var medicElement = '#medicDropdown';

        // Declanseaza
        $(medicElement).trigger('change');
    }
});