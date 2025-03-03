function AddPartialView(encAnnouncementTypeId) {
    var inputDTO = {};
    inputDTO.encAnnouncementTypeId = encAnnouncementTypeId;
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/AnnouncementTypeMaster/AddPartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            $('#div_AddPartial').html(data);
            UnblockUI();
            initErrorValidate();
        },
        error: function (result) {
            $erroralert("Transaction Failed!", result.responseText + '!');
            UnblockUI();
        }
    });
}

function ListPartialView() {
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/AnnouncementTypeMaster/ListPartialView',
        //data: JSON.stringify(resignationListDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            $('#div_ListPartial').html(data);
            $(".List").DataTable({
                "order": []
            });
            UnblockUI();
        },
        error: function (result) {
            $erroralert("Transaction Failed!", result.responseText + '!');
            UnblockUI();
        }
    });
}

function Reset() {
    AddPartialView("");
}
function isValidateForm() {
    let res = true;
    let AnnouncementType = $("#AnnoucementTypeForm").find("[name='AnnouncementType']").val();

    if (!(AnnouncementType != null && AnnouncementType != undefined && $.trim(AnnouncementType) != "")) {
        $("#AnnoucementTypeForm").find("[name='AnnouncementType']").parent().find(".text-danger").text("This field is mandatory");
        $("#AnnoucementTypeForm").find("[name='AnnouncementType']").parent().find(".text-danger").show();
        res = false;
    }
    return res;
}

function Save() {
    BlockUI();
    if (!isValidateForm()) {
        UnblockUI();
        return;
    }
    let inputDTO = {};
    inputDTO.AnnouncementType = $("#AnnoucementTypeForm").find("[name='AnnouncementType']").val();
    inputDTO.encAnnouncementTypeId = $("#AnnoucementTypeForm").find("[name='encAnnouncementTypeId']").val();
    BlockUI();
    $.ajax({
        type: "POST",
        url: "/AnnouncementTypeMaster/Save",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            UnblockUI();
            $successalert("", "Transaction Successful!");
            setTimeout(function () {
                AddPartialView("");
                ListPartialView();
            }, 1000);
        },
        error: function (error) {
            $erroralert("Transaction Failed!", error.responseText + '!'); UnblockUI();
            UnblockUI();
        }
    });

}



function deleteRecord(encAnnouncementTypeId) {
    Swal.fire({ title: 'Are you sure?', text: "This will get deleted permanently!", icon: 'warning', showCancelButton: true, confirmButtonText: 'Yes, delete it!', customClass: { confirmButton: 'btn btn-primary me-3', cancelButton: 'btn btn-label-secondary' }, buttonsStyling: false }).then(function (result) {
        if (result.value) {
            BlockUI();
            var inputDTO = {
                "encAnnouncementTypeId": encAnnouncementTypeId
            };
            $.ajax({
                type: "POST",
                url: "/AnnouncementTypeMaster/Delete",
                contentType: 'application/json',
                data: JSON.stringify(inputDTO),
                success: function (data) {
                    UnblockUI();
                    $successalert("", "Transaction Successful!");
                    setTimeout(function () {
                        AddPartialView("");
                        ListPartialView();
                    }, 1000);
                },
                error: function (error) {
                    $erroralert("Transaction Failed!", error.responseText + '!');
                    UnblockUI();
                }
            });
        }
    });
}