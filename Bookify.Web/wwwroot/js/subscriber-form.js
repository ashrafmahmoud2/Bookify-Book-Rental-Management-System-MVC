$(document).ready(function () {
    $('#GovernorateId').on('change', function () {

        var governorateId = $(this).val();
        var areaDropdown = $('#AreaId');

        areaDropdown.empty().append('<option value="">Select an area...</option>');

        if (governorateId) {
            $.ajax({
                url: `/Subscribers/GetAreasByGovernorate?governorateId=${governorateId}`,
                dataType: 'json',
                success: function (areas) {

                    // Ensure the response contains valid data
                    if (areas.length > 0) {
                        $.each(areas, function (i, area) {
                            $('<option></option>')
                                .attr("value", area.id)
                                .text(area.name)
                                .appendTo(areaDropdown);
                        });

                        // Trigger a UI refresh if needed
                        areaDropdown.trigger('change');
                    }
                },
                error: function () {
                    showErrorMessage();
                }
            });
        }
    });
});
