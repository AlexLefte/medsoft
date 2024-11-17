$(document).ready(function () {
    $("#specializareDropdown").change(function () {
        var selectedSpecializareID = $(this).val();

        // Make an AJAX request to the server to get the medics based on the selected SpecializareID
        $.ajax({
            url: '/admin/consultatie/GetMediciBySpecializare',
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

        timeInput.disabled = false;

        // Make an AJAX request to the server to get the medics based on the selected SpecializareID
        $.ajax({
            url: '/admin/consultatie/GetOreDisponibile',
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

    $("#pacientDropdown").change(function () {
        var selectedPacient = $(this).val();

        if (selectedPacient === 0) {
            statusDropdown.disabled = true;
        } else {
            statusDropdown.disabled = false;
        }
    });

    $("#statusDropdown").change(function () {
        var selectedStatus = $(this).val();

        console.log(selectedStatus);

        if (selectedStatus != 'Finalizata') {
            diagnostic.disabled = true;
            medicamentDropdown.disabled = true;
            doza.disabled = true;
        } else {
            diagnostic.disabled = false;
            medicamentDropdown.disabled = false;
            doza.disabled = false;
        }
    });
});