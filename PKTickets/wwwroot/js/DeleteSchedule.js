function ScheduleDelete(id) {
    let result = confirm("Are you sure you want to delete?");
    if (result) {
        $.ajax({
            type: "get",
            url: "/Home/RemoveSchedule?id=" + id,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (response) {
                alert(response.message);
                if (response.success == true) {
                    setTimeout(function () { window.location.reload(); }, 500);
                }
            },
            error: function () {
                alert("error");
            }
        });
    }
}
