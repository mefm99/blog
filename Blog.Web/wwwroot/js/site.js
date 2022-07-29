// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function Logout() {
    Swal.fire({
        title: 'هل تريد حقاً تسجيل الخروج؟',
        icon: 'question',
        iconHtml: '؟',
        confirmButtonText: 'نعم',
        cancelButtonText: 'لا',
        showCancelButton: true,
        showCloseButton: true
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: 'POST',
                url: '/Home/Logout',
                dataType: 'json',
                success: function (data) {
                    window.location.href = data.url;
                }
            });
        }
    });
}
function HideArticle(articleid) {
    Swal.fire({
        title: 'هل تريد حقاً إخفاء المقالة؟',
        icon: 'question',
        iconHtml: '؟',
        confirmButtonText: 'نعم',
        cancelButtonText: 'لا',
        showCancelButton: true,
        showCloseButton: true
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: 'POST',
                url: '/DashBoard/HideArticle?articleId=' + articleid,
                dataType: 'json',
                success: function (data) {
                    window.location.href = data.url;
                }
            });
        }
    });
}
function ShowArticle(articleid) {
    Swal.fire({
        title: 'هل تريد حقاً إظهار المقالة؟',
        icon: 'question',
        iconHtml: '؟',
        confirmButtonText: 'نعم',
        cancelButtonText: 'لا',
        showCancelButton: true,
        showCloseButton: true
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: 'POST',
                url: '/DashBoard/ShowArticle?articleId=' + articleid,
                dataType: 'json',
                success: function (data) {
                    window.location.href = data.url;
                }
            });
        }
    });
}
function DeleteArticle(articleid) {
    Swal.fire({
        title: 'هل تريد حقاً حذف المقالة؟',
        icon: 'question',
        iconHtml: '؟',
        confirmButtonText: 'نعم',
        cancelButtonText: 'لا',
        showCancelButton: true,
        showCloseButton: true
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: 'POST',
                url: '/DashBoard/DeleteArticle?articleId=' + articleid,
                dataType: 'json',
                success: function (data) {
                    window.location.href = data.url;
                }
            });
        }
    });
}
function EditArticle(articleid) {
    var url = '/DashBoard/EditArticle?articleId=' + articleid;
    window.location.href = url;
}
function ReadURL(input,id) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $(id).attr('src', e.target.result);
        };
        reader.readAsDataURL(input.files[0]);
    }
}
function AddTreasury() {
    $.ajax({
        type: "GET",
        url: "/System/Popup",
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (o) {
            $("#modal-placeholder").html();
            $("#modal-placeholder").html(o);
            $("#add-treasury").modal("show");
        },
        failure: function (o) {
            alert("failure loading treasury");
        },
        error: function (o) {
            alert("error loading treasury");
        },
    });
}
function EditTreasury(treasuryId) {
    var url = '/System/EditTreasury?treasuryId=' + treasuryId;
    window.location.href = url;
}