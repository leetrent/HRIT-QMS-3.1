$(document).ready(function () {
    //console.log("[qms_empl_search.js] => BEGIN:");

    $('#EmployeeSearchResult').autocomplete({
        source: function (request, response) {
            $.ajax({
                url: '/api/employee/search',
                dataType: "json",
                data: {
                    term: request.term,
                    showInactive: $('#ShowInactiveEmployees').is(':checked')
                },
                success: function (data) {
                    response(data);
                }
            });
        },
        minLength: 2
    });
    //console.log(":END <= [qms_empl_search.js]");
});