/**
 * Created by ALEX on 21.02.2017.
 */

//jctxt is jQuery object!
function ButtonsWrapper(ctxt)
{
    var that = this;
    this.context = ctxt;
    //current active value
    this.value = this.context.attr("data-value");
    //lock variable that data is already sending
    var sending = false;

    //catch buttons
    this.buttons = {
        good : ctxt.find("[name='good']"),
        neutral : ctxt.find("[name='neutral']"),
        bad : ctxt.find("[name='bad']")
    };
    $.each(this.buttons, function (key,value) {
        value.activeClass = value.attr('data-active-class');
        //add click handler
        value.click(function () {
            this.blur();
            that.onButtonClicked(value);
        })
    });
    this.refreshView = function () {
        $.each(this.buttons,function (key, value) {
            value.removeClass(value.activeClass);
        });
        var cur = this.buttons[this.value];
        cur.addClass(cur.activeClass);

    };
    this.onButtonClicked = function (btn) {
        if(sending)
            return; //data is already sending
        var name = btn.attr('name');
        //console.log(name);
        btn.addClass('loading');
        //simulate sending
        this.sendData(name, function (success) {
            if(success)
            {
                that.value = name;
            }
            btn.removeClass('loading');
            that.refreshView();
        });
        /*var prevValue = this.value;
        this.value = name;
        this.refreshView();*/
        //set button waiting
    };
    this.sendData = function (data, onCompleteCallback) {
        sending = true;
        setTimeout(function () {
            sending = false;
            onCompleteCallback(/*Math.random() < 0.8*/true);
        },(Math.random() + 0.5)*1000);
    };
    this.refreshView();


}
$(document).ready(function () {
    setTimeout(function () {
        //$.ajax("http://localhost:1132/www/txt.html")
        $.ajax("http://localhost:8080/apps/ajax")
            .done(function (msg) {
                //set the content
                $("tbody").html(msg);
                $(".mybuttons").each(function (i, el) {
                    var wrapper = new ButtonsWrapper($(el));
                    //var val = $(el).attr("data-value")*1;
                    //console.log(val);
                });
            })
            .fail(function () {
                //set message that failed to load
                console.log("failed to load data");
            });
    },1000);


});