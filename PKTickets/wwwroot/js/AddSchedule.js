$(document).ready(function () {
    $("#AddSchedule").submit(function (e) {
        e.preventDefault();
        $.ajax({
            url: "/Home/SaveSchedule",
            type: "Post",
            data: $("#AddSchedule").serialize(),
            dataType: 'json',
            success: function (response) {
                alert(response.message);
                if (response.success == true) {
                    setTimeout(function () { window.location = '/Home/ScreenSchedules?id=' + $('#ScreenId').val(); }, 500);
                    $('#ScreenId').val();
                }
            },
            error: function () {
                alert("error");
            }
        });
    });
});