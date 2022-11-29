function Theater(id) {
    let result = confirm("Are you sure you want to delete?");
    if (result) {
        $.ajax({
            type: "get",
            url: "/Home/RemoveTheater?id=" + id,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (response) {
                alert(response.message);
                if (response.success == true) {
                    /*setTimeout(function () { window.location = '/Home/ReservationsByUserId'; }, 1000);*/
                    location.reload();
                }
            },
            error: function () {
                alert("error");
            }
        });
    }
}
function GoIndex() {
    setTimeout(function () { window.location = '/Home/Index'; }, 100);
}