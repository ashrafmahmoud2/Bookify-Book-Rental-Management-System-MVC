var updatedRow;
var dataTable;

function onModalBegin() {
    let submitButton = $('#Modal :submit');
    submitButton.prop('disabled', true);
    submitButton.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Please wait...');

}



function showSuccessMessage(message = 'Saved successfully!') {
    Swal.fire({
        icon: 'success',
        title: 'Success',
        text: message,
        customClass: {
            confirmButton: "btn btn-outline btn-outline-dashed btn-outline-primary btn-active-light-primary"
        }
    });
}

function showErrorMessage(message = 'Something went wrong!') {
    Swal.fire({
        icon: 'error',
        title: 'Oops...',
        text: message,
        customClass: {
            confirmButton: "btn btn-outline btn-outline-dashed btn-outline-primary btn-active-light-primary"
        }
    });
}

function onModalSuccess(row) {
    showSuccessMessage();
    $('#Modal').modal('hide');

    if (updatedRow !== undefined) {
        dataTable.row(updatedRow).remove().draw();
        updatedRow = undefined;
    }


    let newRow = $(row);
    dataTable.row.add(newRow).draw();    

    KTMenu.init();
    KTMenu.initHandlers();
}

function onModalComplete() {
    $('#Modal :submit').prop('disabled', false).text('Save');
}




$(document).ready(function () {


    var message = $('#Message').text();
    if (message !== '') {
        showSuccessMessage(message);
    }

    $('table').DataTable({
        dom: 'Bfrtip',
        buttons: [
            { extend: 'copy', text: '📋 Copy Table', className: 'btn btn-primary', exportOptions: { columns: [0, 1, 2, 3] } },
            { extend: 'csv', text: '📁 Export CSV', className: 'btn btn-success', exportOptions: { columns: [0, 1, 2, 3] } }, // Added missing comma
            { extend: 'excel', text: '📊 Download Excel', className: 'btn btn-info', exportOptions: { columns: [0, 1, 2, 3] } },
            { extend: 'pdf', text: '📄 Save as PDF', className: 'btn btn-danger', exportOptions: { columns: [0, 1, 2, 3] } },
            { extend: 'print', text: '🖨️ Print View', className: 'btn btn-warning', exportOptions: { columns: [0, 1, 2, 3] } }
        ]
    });

 
    //Handle bootstrap modal
    $('body').delegate('.js-render-modal', 'click', function () {
        var btn = $(this);
        var modal = $('#Modal');

        modal.find('#ModalLabel').text(btn.data('title'));

        if (btn.data('update') !== undefined) {
            updatedRow = btn.parents('tr');
            console.log(updatedRow);
        }

        $.get({
            url: btn.data('url'),
            success: function (form) {
                modal.find('.modal-body').html(form);
                $.validator.unobtrusive.parse(modal);
            },
            error: function () {
                showErrorMessage();
            }
        });

        modal.modal('show');
    });

    //Handle  toggle-status
    $('body').delegate('.js-toggle-status', 'click', function () {
        let btn = $(this);
        let row = btn.closest('tr');
        let status = row.find('.js-status');

        bootbox.confirm({
            message: `Are you sure you want to toggle the status of item ${btn.data('id')}?`,
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-danger'
                },
                cancel: {
                    label: 'No',
                    className: 'btn-secondary'
                }
            },
            callback: function (result) {
                if (result) {
                    $.ajax({
                        url: btn.data('url'),
                        type: 'PUT',
                        data: {
                            '__RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                        },
                        success: (lastUpdateOn) => {
                            let newStatus = status.text().trim() === 'Deleted' ? 'Available' : 'Deleted';
                            status.text(newStatus).toggleClass('badge-light-danger badge-light-success');
                            row.find('.js-update-On').html(lastUpdateOn);
                            row.addClass('animate__animated animate__flash');

                            showSuccessMessage();

                        },
                        error: function () {
                            showErrorMessage();
                        }

                    });

                }
            }
        });
    });



    // Handle Delete
    $('body').on('click', '.js-delete', function () {
        let btn = $(this);
        let row = btn.closest('tr');

        bootbox.confirm({
            message: `Are you sure you want to delete item ${btn.data('id')}?`,
            buttons: {
                confirm: { label: 'Yes', className: 'btn-danger' },
                cancel: { label: 'No', className: 'btn-secondary' }
            },
            callback: function (confirmed) {
                if (!confirmed) return;

                $.ajax({
                    url: btn.data('url'),
                    type: 'PUT',
                    success: () => {
                        row.fadeOut(300, function () {
                            $(this).remove();
                        });
                        showSuccessMessage();
                    },
                    error: showErrorMessage
                });
            }
        });
    });

});








