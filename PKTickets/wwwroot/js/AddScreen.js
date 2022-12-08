$(document).ready(function () {
    $("#AddScreen").submit(function (e) {
        e.preventDefault();
        $.ajax({
            url: "/Home/SaveScreen",
            type: "Post",
            data: $("#AddScreen").serialize(),
            dataType: 'json',
            success: function (response) {
                if (response.success == true) {
                    $.toast({
                        heading: 'Success',
                        text: response.message,
                        showHideTransition: 'slide',
                        icon: 'success',
                        hideAfter: 5000,
                        position: 'top-center',
                        stack: false,
                        loader: false,
                        textAlign: 'center',
                        bgColor: 'green',
                        textColor: 'white'
                    })
                    setTimeout(function () { window.location = '/Home/TheaterScreens?id=' + $('#TheaterId').val(); }, 5000);
                    $('#TheaterId').val();
                }
                else {
                    $.toast({
                        heading: 'Error',
                        loader: false,
                        text: response.message,
                        showHideTransition: 'slide',
                        icon: 'error',
                        hideAfter: 5000,
                        position: 'top-center',
                        stack: false,
                        textAlign: 'center',
                        bgColor: 'red',
                        textColor: 'white'
                    })
                }
            },
            error: function () {
                alert("error");
            }
        });
    });
});