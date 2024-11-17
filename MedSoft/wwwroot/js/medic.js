/*var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/medic/getallvm' },
        "columns": [
            { data: 'medic.numeMedic', 'width': "10%" },
            { data: 'medic.prenumeMedic', 'width': "10%" },
            { data: 'medic.cnp', 'width': "10%" },
            { data: 'medic.adresa', 'width': "10%" },
            { data: 'numarTelefon', 'width': "10%" },
            { data: 'email', 'width': "15%" },
            { data: 'medic.specializare.nume', 'width': "10%" },
            { data: 'medic.pretConsultatie', 'width': "5%" },
            {
                data: 'medic.medicID',
                "render": function (data) {
                    console.log(data);
                    return `<div class="w-75 btn-group" role="group">
                            <a href="/admin/medic/upsert?id=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Modifica</a>
                            <a onClick=Delete("/medic/product/delete/${data}") class="btn btn-danger mx-2"> <i class="bi bi-trash-fill"></i> Sterge</a>
                        </div>`                
                },
                'width': "20%"
            }
        ]
    });
}
*/
/*function Delete(url) {
    Swal.fire({
        title: 'Esti sigur?',
        text: "Modificarea nu este reversibila!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Da!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        toastr.success(data.message);
                    }
                    else {
                        toastr.error(data.message);
                    }
                },
            })
        }
    })
}*/



