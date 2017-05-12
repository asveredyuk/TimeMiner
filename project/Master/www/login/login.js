/**
 * Created by ALEX on 13.05.2017.
 */
$(document).ready(function () {
    var loginTb = $("#loginInput");
    var passwordTb = $("#passwordInput");
    $("#submitBt").click(function () {
        var obj = {
            Login : loginTb.val(),
            Password:passwordTb.val()
        };
        var json = JSON.stringify(obj);
        $.ajax({
            url: "/api/login/gettoken",
            type: 'POST',
            data: json
        }).done(function (msg) {
            var message = JSON.parse(msg);
            if(typeof message.Token != 'undefined')
            {
                //we got token
                var token = message.Token;
                Cookies.set('auth_token', token);
                window.location = '/';
                return;
            }
            if(typeof message.Error != 'undefined')
            {
                alert('error!' + message.Error);
            }


        });
    });
});