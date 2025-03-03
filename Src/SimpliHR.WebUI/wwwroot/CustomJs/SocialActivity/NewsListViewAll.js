function NewsDetails(newsId, Source) {
    fetchPartialViewNewsDetails(newsId, Source);
}
function ListView() {
    fetchPartialViewNewsList();
}
function CardView() {
    fetchPartialViewNewsCard();
}
function fetchPartialViewNewsList(PageNumber = 1) {
    let EmployeeNewsPageDetails = {};
    EmployeeNewsPageDetails.PageSize = 6;
    EmployeeNewsPageDetails.PageNumber = PageNumber;
    let inputDTO = {};
    inputDTO.NewsCategoryId = $("[name='NewsCategoryId']").val();
    inputDTO.NewsSearchKeyword = $("[name='NewsSearchKeyword']").val();
    inputDTO.PageDetails = EmployeeNewsPageDetails;
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/EmployeeNews/NewsListPartialView',
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
function fetchPartialViewNewsCard(PageNumber = 1) {
    let EmployeeNewsPageDetails = {};
    EmployeeNewsPageDetails.PageSize = 6;
    EmployeeNewsPageDetails.PageNumber = PageNumber;
    let inputDTO = {};
    inputDTO.NewsCategoryId = $("[name='NewsCategoryId']").val();
    inputDTO.NewsSearchKeyword = $("[name='NewsSearchKeyword']").val();
    inputDTO.PageDetails = EmployeeNewsPageDetails;
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/EmployeeNews/NewsCardPartialView',
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
function fetchPartialViewNewsDetails(newsId, Source) {
    //let inputDTO = {};
    //inputDTO.AnnouncementTypeId = AnnouncementTypeId;
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/EmployeeNews/NewsDetails/' + newsId + '/' + Source,
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