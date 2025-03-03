function setActive(element, action) {
    // Get all the nav links
    var links = document.querySelectorAll('#ExitNav .nav-link');

    // Remove the active class from all nav links
    links.forEach(function (link) {
        link.classList.remove('active');
    });

    // Add the active class to the clicked nav link
    element.classList.add('active');

    //$("#projectNavValue").val(action);

    ViewEmployeeResignListAdminPartialView(action);
}

function ViewEmployeeResignListAdminPartialView(opt) {
    var resignationListDTO = {};
    resignationListDTO.OPT = opt;
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/ExitManagement/ViewEmployeeResignListAdminPartialView',
        data: JSON.stringify(resignationListDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $('#div_ViewResignListPartial').html(data);
            $(".List").dataTable();
            //let autoresize = $(".auto-resize");
            //autoresize.each((i, v) => {
            //    $(v)[0].style.height = "auto";
            //    $(v)[0].style.height = $(v)[0].scrollHeight + "px";
            //});

            //initialLastWorkingDate = $("#LastWorkingDateAdmin").val();
            //initialResignationDate = $("#ResignationDateAdmin").val();

            //initDates();

            // $("#AdminDetails").find("[name='NoticePeriodWaiveOffAdmin']").change(function () {
            //     if ($(this).is(":checked")) {
            //         var resignationDate = $("#ResignationDateAdmin").val();
            //         $("#LastWorkingDateAdmin").val(resignationDate).prop("disabled", true);
            //     } else {
            //         $("#LastWorkingDateAdmin").val(initialLastWorkingDate).prop("disabled", false);
            //     }
            // });
        },
        error: function (result) {
            UnblockUI();
            $erroralert("Transaction Failed!", result.responseText);
        }
    });
}