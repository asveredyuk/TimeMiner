$(document).ready(function () {
    var userControl = $('#topMenuUserControl');
    userControl.dropdown();
    userControl.find('.topMenuLogout').click(function () {
        Cookies.remove('auth_token');
        window.location = "";
    });
});