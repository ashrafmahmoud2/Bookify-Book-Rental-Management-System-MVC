var updatedRow;
var dataTable;


//js-select2
function applySellect2() {
    $('.js-select2').select2();
    $('.js-select2').on('select2:select', function (e) {
        $('form').not('#SignOut').validate().element('#' + $(this).attr('id'));
    });
}


function dissableSubmitButtonInModal() {
    let submitButton = $('#Modal :submit');
    submitButton.prop('disabled', true);
    submitButton.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Please wait...');
}

function disableSubmitButtonInForm() {
    let submitButton = $('.submit-from-btn');
    submitButton.prop('disabled', true);
    submitButton.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Please wait...');
}

function onModalBegin() {
    dissableSubmitButtonInModal();
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
        text: message.responseText = !undefined ? message.responseText : message ,
        customClass: {
            confirmButton: "btn btn-outline btn-outline-dashed btn-outline-primary btn-active-light-primary"
        }
    });
}

function onModalSuccess(rowHtml) {
    showSuccessMessage();
    $('#Modal').modal('hide');

    if (updatedRow !== undefined) {
        dataTable.row(updatedRow).remove().draw(false);  // Use draw(false) to keep pagination state
        updatedRow = undefined;
    }

    let newRow = $(rowHtml);
    dataTable.row.add(newRow).draw(false);  // Use draw(false) to keep pagination state

    KTMenu.init();
    KTMenu.initHandlers();
    KTMenu.initGlobalHandlerss();
}

function onModalComplete() {
    $('#Modal :submit').prop('disabled', false).text('Save');
}


//

$(document).ready(function () {

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

                            showSuccessMessage("");

                        },
                        error: function () {

                            showErrorMessage();

                        }

                    });

                }
            }

        });
    });


    //Handle Add and update
    $('body').delegate('.js-render-modal', 'click', function () {
        var btn = $(this);
        var modal = $('#Modal');


        modal.find('#ModalLabel').text(btn.data('title'));

        if (btn.data('update') !== undefined) {
            updatedRow = btn.closest('tr');
        }

        $.get({
            url: btn.data('url'),
            success: function (form) {
                modal.find('.modal-body').html(form);
                $.validator.unobtrusive.parse(modal);
                applySellect2()
            },
            error: function () {
                showErrorMessage();
            }
        });

        modal.modal('show');
    });


    // Handle Delete
    $('body').on('click', '.js-delete', function () {
        prompt("arrive")
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
                    type: 'Delete',
                    success: () => {
                        dataTable.row(row).remove().draw(false);
                        showSuccessMessage("Deleted successfully!");
                    },
                    error: showErrorMessage
                });
            }
        });
    });




    $('form').not('#SignOut').on('submit', function () {
        if ($('.js-tinymce').length > 0) {
            $('.js-tinymce').each(function () {
                var input = $(this);
                var editorId = input.attr('id'); // Get the ID of the textarea
                if (editorId) { // Ensure ID exists to avoid errors
                    var content = tinymce.get(editorId).getContent(); // Get TinyMCE content
                    input.val(content); // Set it back to the textarea
                }
            });
        }
        //var valid = $(this).valid(); 
        //if (valid) disableSubmitButtonInForm();

    });





    //js-select2
    applySellect2();




    //TinyMCE only if the page has an element with class .js-tinymce
    if ($('.js-tinymce').length > 0) {
        tinymce.init({
            selector: ".js-tinymce",
            toolbar: 'undo redo | blocks fontfamily fontsize | bold italic underline strikethrough | link image media table mergetags | addcomment showcomments | spellcheckdialog a11ycheck typography | align lineheight | checklist numlist bullist indent outdent | emoticons charmap | removeformat',
            plugins: [
                'anchor', 'autolink', 'charmap', 'codesample', 'emoticons', 'image', 'link', 'lists', 'media', 'searchreplace', 'table', 'visualblocks', 'wordcount',
                'checklist', 'mediaembed', 'casechange', 'export', 'formatpainter', 'pageembed', 'a11ychecker', 'tinymcespellchecker', 'permanentpen', 'powerpaste', 'advtable', 'advcode', 'editimage', 'advtemplate', 'ai', 'mentions', 'tinycomments', 'tableofcontents', 'footnotes', 'mergetags', 'autocorrect', 'typography', 'inlinecss', 'markdown', 'importword', 'exportword', 'exportpdf'
            ]

        });
    }


    ////DataTable
    dataTable = $('table').DataTable({
        dom: 'Bfrtip',
        buttons: [
            //{ extend: 'copy', text: '📋 Copy Table', className: 'btn btn-primary', exportOptions: { columns: [0, 1, 2, 3] } },
            //{ extend: 'csv', text: '📁 Export CSV', className: 'btn btn-success', exportOptions: { columns: [0, 1, 2, 3] } }, 
            { extend: 'excel', text: '📊 Download Excel', className: 'btn btn-info', exportOptions: { columns: [0, 1, 2, 3] } },
            { extend: 'pdf', text: '📄 Save as PDF', className: 'btn btn-danger', exportOptions: { columns: [0, 1, 2, 3] } },
            //    { extend: 'print', text: '🖨️ Print View', className: 'btn btn-warning', exportOptions: { columns: [0, 1, 2, 3] } }
        ]
    });


    //flatpickr
    $('.js-flatpickr').flatpickr({
        defaultDate: new Date().toISOString().split('T')[0],
        minDate: "1/1/2022",
        maxDate: "1/1/2026",
        monthSelectorType: "static"

    });



    var message = $('#Message').text();
    if (message !== '') {
        showSuccessMessage(message);
    }

    //$('.js-render-modal').on('click', function () {
    //    prompt("arrive")
    //});

    //Handle Add and update
    $('body').delegate('.js-render-modal', 'click', function () {
        var btn = $(this);
        var modal = $('#Modal');


        modal.find('#ModalLabel').text(btn.data('title'));

        if (btn.data('update') !== undefined) {
            updatedRow = btn.closest('tr');
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



    ////Handal Sign Out
    //$('#SignOut').submit();
})












