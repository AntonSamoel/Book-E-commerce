﻿var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Company/GetAll"
        },
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            { "data": "address", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                <a  href="/Admin/Company/Upsert?id=${data}" class="btn btn-secondary" > <i class="bi bi-pencil-square"></i> &nbsp; Edit</a>
                `
                },
                "width": "15%"
            }, {
                "data": "id",
                "render": function (data) {
                    return `
                <a onclick="Delete('/Admin/Company/Delete/${data}')" class="btn btn-danger"><i class="bi bi-trash3"></i> &nbsp; Delete</a>
                `
                },
                "width": "15%"
            },
        ]
    });
}
function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
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
                }
            })
        }
    })
}     