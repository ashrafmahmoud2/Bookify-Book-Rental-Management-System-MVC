﻿
$(document).ready(function () {
    $('.js-renew').on('click', function () {

        var subscriberKey = $(this).data('key');

        bootbox.confirm({
            message: "Are you sure that you need to renew this subscription?",
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-success'
                },
                cancel: {
                    label: 'No',
                    className: 'btn-secondary'
                }
            },
            callback: function (result) {
                if (result) {
                    $.post({
                        url: `/Subscribers/RenewSubscription?sKey=${subscriberKey}`,
                        data: {
                            '__RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                        },
                        success: function (row) {
                            $('#SubscriptionsTable').find('tbody').append(row);


                            var activeIcon = $('#ActiveStatusIcon');
                            activeIcon.removeClass('d-none');
                            activeIcon.siblings('svg').remove();
                            activeIcon.parents('.card').removeClass('bg-warning').addClass('bg-success');

                            $('#CardStatus').text('Active subscriber');
                            $('#StatusBadge').removeClass('badge-light-warning').addClass('badge-light-success').text('Active subscriber');
                            $('#RentalButton').removeClass('d-none');
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

    $('.js-cancel-rentel').on('click', function () {

        var rentalId = $(this).data('id');
        var $button = $(this);

        bootbox.confirm({
            message: "Are you sure that you want to cancel this rental?",
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
                    $.post({
                        url: `/Rentals/MarkAsDeleted?rentalId=${rentalId}`,
                        data: {
                            '__RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                        },
                        success: function (result) {

                            $button.closest('tr').remove();

                            if ($('#RentalsTable tbody tr').length === 0) {
                                $('.table-responsive').addClass('d-none');
                                $('.alert').removeClass('d-none');
                            }



                            const $totalCount = $('#TotelCopies');
                            const currentCount = parseInt($totalCount.text(), 10);
                            $totalCount.text(currentCount - result);

                           


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
});