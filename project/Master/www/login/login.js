/**
 * Created by ALEX on 13.05.2017.
 */
$(document).ready(function () {
    /*var loginTb = $("#loginInput");
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
    });*/
    var form = $('form');
    form.form();
    var loginTb = form.find('input[name="login"]');
    var passwordTb = form.find('input[name="password"]');
    var button = form.find('.button');
    button.click(function () {
        button.addClass('loading');
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
                window.location = '/';//redirect
                return;
            }
            if(typeof message.Error != 'undefined')
            {
                //got error
                form.form('add errors', [message.Error]);
                //loginTb.parent().parent().addClass('error');
                //passwordTb.parent().parent().addClass('error');
                button.removeClass('loading');
            }
        });
    });
    form.submit(function () {
        button.click();
        return false;
    });

});