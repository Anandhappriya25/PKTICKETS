$(document).ready(function () {
    $("#AddScreen").submit(function (e) {
        e.preventDefault();
        $.ajax({
            url: "/Home/SaveScreen",
            type: "Post",
            data: $("#AddScreen").serialize(),
            dataType: 'json',
            success: function (response) {
                alert(response.message);
                if (response.success == true) {
                    setTimeout(function () { window.location = '/Home/TheaterScreens?id=' + $('#TheaterId').val(); }, 500);
                    $('#TheaterId').val();
                }
            },
            error: function () {
                alert("error");
            }
        });
    });
});