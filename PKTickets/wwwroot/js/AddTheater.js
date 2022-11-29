
$(document).ready(function () {
    $("#AddTheater").submit(function (e) {
        e.preventDefault();
        $.ajax({
            url: "/Home/SaveTheater",
            type: "Post",
            data: $("#AddTheater").serialize(),
            dataType: 'json',
            success: function (response) {
                alert(response.message);
                if (response.success == true) {
                    setTimeout(function () { window.location = '/Home/TheatersList'; }, 500);
                }
            },
            error: function () {
                alert("error");
            }
        });
    });
});

