/**
 * Created by ALEX on 21.02.2017.
 */

//jctxt is jQuery object!
function ButtonsWrapper(ctxt, rowWrapper)
{
    var that = this;
    this.context = ctxt;

    this.onValueChanged = function (value, callback) {
        console.log("no value changed handler found")
    };
    //current active value

    var converterFromEnum = {
        0:"good",
        1:"neutral",
        2:"bad"
    };
    var converterToEnum = {
        good:0,
        neutral:1,
        bad:2
    };
    this.value = converterFromEnum[rowWrapper.data.Rel];
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
        if(rowWrapper.sending)
            return; //data is already sending
        var name = btn.attr('name');
        var enumVal = converterToEnum[name];
        //console.log(name);
        btn.addClass('loading');
        //begin sending
        this.onValueChanged(enumVal, function (success) {
            //allow user to change value
            if(success)
            {
                that.value = name;
            }
            btn.removeClass('loading');
            that.refreshView();
        });
    };
    this.refreshView();


}

function RowWrapper(tBody, data, template) {
    var that = this;
    //this indicates that no changes can be applyed
    this.sending = false;
    this.data = data;
    this.row = $(Mustache.render(template,data));
    tBody.append(this.row);
    //row.css("background-color","red");
    this.buttonWrapper = new ButtonsWrapper(this.row.find(".mybuttons"), this);
    this.buttonWrapper.onValueChanged = function (val, callback) {
        that.sending = true;
        data.Rel = val;
        /*setTimeout(function () {


        },(Math.random() + 0.5)*1000);*/
        $.post("/api/apps/updateitem",JSON.stringify(that.data),function () {
            console.log("Value changed to " + val);
            that.sending = false;
            callback(/*Math.random() < 0.8*/true);
        });
    };

}
$(document).ready(function () {

    $.ajax("/api/apps/gettable")
        .done(function (msg) {
            $.ajax("/table/tablerow.html")
                .done(function (template) {
                    var arr = JSON.parse(msg);
                    var tbody = $("tbody");
                    tbody.html("");
                    Mustache.parse(template);
                    $.each(arr, function (key, value) {
                        //var res = Mustache.render(template,value);
                        //var row = tbody.append(res);
                        var r = new RowWrapper(tbody,value,template);

                    });

                    /*                        $(".mybuttons").each(function (i, el) {
                     var wrapper = new ButtonsWrapper($(el));
                     //var val = $(el).attr("data-value")*1;
                     //console.log(val);
                     });*/
                });
            //set the content

        })
        .fail(function () {
            //set message that failed to load
            console.log("failed to load data");
        });


});