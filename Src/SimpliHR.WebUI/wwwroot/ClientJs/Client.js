
function Validation() {

    if ($("#CompanyName").val() == "") {
        alert("Company Name cannot be empity");
        return false;
    }
    if ($("#GSTN").val() == "") {
        alert("GSTN cannot be empity");
        return false;
    }