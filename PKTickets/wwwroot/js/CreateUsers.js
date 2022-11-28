$(document).ready(function () {
    $("#CreateUsers").submit(function (e) {
        e.preventDefault();
        $.ajax({
            url: "/Home/Save",
            type: "Post",
            data: $("#CreateUsers").serialize(),
            success: function (response) {
                alert(response.message);
                if (response.success == true) {
                    if (response.role == true) {
                        setTimeout(function () { window.location = '/Home/UserList'; }, 500);
                    }
                    else {
                        setTimeout(function () { window.location = '/Home/Index'; }, 500);
                    }
                }
            },
            error: function () {
                alert("error");
            }
        }); 
    });
});
function GoIndex() {
    setTimeout(function () { window.location = '/Home/Index'; }, 100);
}
function GoUser() {
    setTimeout(function () { window.location = '/Home/UsersList'; }, 100);
}