$(document).ready(function () {
    ProjectTrackerPartialView();
});

function ProjectTrackerPartialView() {
    let inputDTO = {};
    inputDTO.Source = "md";
    inputDTO.ProjectStatusType = "Incomplete";
    inputDTO.PageSize = 4;
    inputDTO.PageNumber = 1;
    inputDTO.SearchKeyword = "";
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/Employee/ProjectsForDashboard',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            $('#div_ProjectTrackerPartialView').html(data);
            UnblockUI();
        },
        error: function (result) {
            alert(result.responseText);
            UnblockUI();
        }
    });
}