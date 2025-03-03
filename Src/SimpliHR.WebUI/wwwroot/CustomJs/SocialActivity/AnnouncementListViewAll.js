function AnnouncementDetails(encEmployeeAnnouncementId, Source) {
    fetchPartialViewAnnouncementDetails(encEmployeeAnnouncementId, Source);
}
function ListView() {
    fetchPartialViewAnnouncementList();
}
function CardView() {
    fetchPartialViewAnnouncementCard();

}
function fetchPartialViewAnnouncementList(PageNumber = 1) {

    let EmployeeAnnouncementPageDetails = {};
    EmployeeAnnouncementPageDetails.PageSize = 6;
    EmployeeAnnouncementPageDetails.PageNumber = PageNumber;

    let inputDTO = {};
    inputDTO.AnnouncementTypeId = $("[name='AnnouncementTypeId']").val();
    inputDTO.AnnouncementSearchKeyword = $("[name='AnnouncementSearchKeyword']").val();
    inputDTO.PageDetails = EmployeeAnnouncementPageDetails;
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/EmployeeAccouncement/AnnouncementListPartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            $('#div_PartialView').html(data);
            //$("[name='NewsSearchKeyword']").focus();
            UnblockUI();
        },
        error: function (result) {
            alert(result.responseText);
            UnblockUI();
        }
    });
}
function fetchPartialViewAnnouncementCard(PageNumber = 1) {
    let EmployeeAnnouncementPageDetails = {};
    EmployeeAnnouncementPageDetails.PageSize = 6;
    EmployeeAnnouncementPageDetails.PageNumber = PageNumber;

    let inputDTO = {};
    inputDTO.AnnouncementTypeId = $("[name='AnnouncementTypeId']").val();
    inputDTO.AnnouncementSearchKeyword = $("[name='AnnouncementSearchKeyword']").val();
    inputDTO.PageDetails = EmployeeAnnouncementPageDetails;
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/EmployeeAccouncement/AnnouncementCardPartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            $('#div_PartialView').html(data);
            UnblockUI();
        },
        error: function (result) {
            alert(result.responseText);
            UnblockUI();
        }
    });
}
function fetchPartialViewAnnouncementDetails(encEmployeeAnnouncementId, Source) {
    //let inputDTO = {};
    //inputDTO.AnnouncementTypeId = AnnouncementTypeId;
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/EmployeeAccouncement/AnnouncementDetails/' + encEmployeeAnnouncementId + '/' + Source,
        //data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            $('#div_PartialView').html(data);
            UnblockUI();
        },
        error: function (result) {
            alert(result.responseText);
            UnblockUI();
        }
    });
}



function SubmitVote(questionId) {
    let inputDTO = {};
    inputDTO.QuestionId = questionId;
    inputDTO.OptionId = $('input[name="vote_' + questionId + '"]:checked').val();

    if (inputDTO.OptionId == undefined || inputDTO.OptionId == null || inputDTO.OptionId == 0 || inputDTO.OptionId == "") {
        $erroralert("Ooops!", "Please select atleast one option" + '!');
    }

    $.ajax({
        type: "POST",
        url: "/EmployeeAccouncement/SubmitVote",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            $("[name='pollQuestionID_" + questionId+"']").find("[name='btnVote']").remove();
            $("[name='pollQuestionID_" + questionId + "']").find("[name='vote_" + questionId + "']").each((i, v) => {
                $(v).parent().css({ "pointer-events": "none" });
            })
            $successalert("Success!", "Saved succesfully.");
            UnblockUI();
        },
        error: function (error) {
            $erroralert("Error!", error.responseText + '!');
            UnblockUI();
        }
    });
}