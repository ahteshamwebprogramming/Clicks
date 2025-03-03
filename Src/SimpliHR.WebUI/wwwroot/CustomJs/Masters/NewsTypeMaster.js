function AddPartialView(encNewsCategoryTagId) {
    var inputDTO = {};
    inputDTO.encNewsCategoryTagId = encNewsCategoryTagId;
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/NewsTypeMaster/AddPartialView',
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
        url: '/NewsTypeMaster/ListPartialView',
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
    let NewsCategoryTag = $("#NewsCategoryTagForm").find("[name='NewsCategoryTag']").val();

    if (!(NewsCategoryTag != null && NewsCategoryTag != undefined && $.trim(NewsCategoryTag) != "")) {
        $("#NewsCategoryTagForm").find("[name='NewsCategoryTag']").parent().find(".text-danger").text("This field is mandatory");
        $("#NewsCategoryTagForm").find("[name='NewsCategoryTag']").parent().find(".text-danger").show();
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
    inputDTO.NewsCategoryTag = $("#NewsCategoryTagForm").find("[name='NewsCategoryTag']").val();
    inputDTO.encNewsCategoryTagId = $("#NewsCategoryTagForm").find("[name='encNewsCategoryTagId']").val();
    BlockUI();
    $.ajax({
        type: "POST",
        url: "/NewsTypeMaster/Save",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            UnblockUI();
            AddPartialView("");
            ListPartialView();
            $successalert("", "Transaction Successful!");
        },
        error: function (error) {
            $erroralert("Error!", error.responseText + '!'); 
            UnblockUI();
        }
    });

}



function deleteRecord(encNewsCategoryTagId) {
    Swal.fire({ title: 'Are you sure?', text: "This will get deleted permanently!", icon: 'warning', showCancelButton: true, confirmButtonText: 'Yes, delete it!', customClass: { confirmButton: 'btn btn-primary me-3', cancelButton: 'btn btn-label-secondary' }, buttonsStyling: false }).then(function (result) {
        if (result.value) {
            BlockUI();
            var inputDTO = {
                "encNewsCategoryTagId": encNewsCategoryTagId
            };
            $.ajax({
                type: "POST",
                url: "/NewsTypeMaster/Delete",
                contentType: 'application/json',
                data: JSON.stringify(inputDTO),
                success: function (data) {
                    UnblockUI();
                    AddPartialView("");
                    ListPartialView();
                    $successalert("", "Transaction Successful!");                    
                },
                error: function (error) {
                    $erroralert("Error!", error.responseText + '!');
                    UnblockUI();
                }
            });
        }
    });
}