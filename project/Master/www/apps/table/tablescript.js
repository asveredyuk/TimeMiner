/**
 * Created by ALEX on 21.02.2017.
 */

var tableWrapper;


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
    //relevance change buttons
    this.buttonWrapper = new ButtonsWrapper(this.row.find(".mybuttons"), this);
    this.buttonWrapper.onValueChanged = function (val, callback) {
        that.sending = true;
        data.Rel = val;
        /*setTimeout(function () {


        },(Math.random() + 0.5)*1000);*/
        $.post("/api/apps/updateapp",JSON.stringify(that.data),function () {
            console.log("Value changed to " + val);
            that.sending = false;
            callback(/*Math.random() < 0.8*/true);
        });
    };
    var rmbutton = this.row.find('.rmbutton');
    rmbutton.click(function () {
        rmbutton.addClass('loading');
        $.post('/api/apps/rmapp',JSON.stringify({Id:that.data.App.Id}),function (res) {
            tableWrapper.reloadTable();
        })
    });

}
function TableWrapper(ctxt)
{
    var that = this;
    this.tbody = ctxt.find('tbody');
    this.loader = this.tbody.find('tr');
    this.reloadTable = function () {
        this.tbody.empty();
        this.tbody.append(this.loader);
        $.ajax("/api/apps/gettable")
            .done(function (msg) {
                $.ajax("/apps/table/tablerow.html")
                    .done(function (template) {
                        var arr = JSON.parse(msg);
                        that.loader.detach();
                        Mustache.parse(template);
                        $.each(arr, function (key, value) {
                            //var res = Mustache.render(template,value);
                            //var row = tbody.append(res);
                            var r = new RowWrapper(that.tbody,value,template);

                        });
                    });

            })
            .fail(function () {
                //set message that failed to load
                console.log("failed to load data");
            });
    }
}
// function ReloadTable()
// {
//     var tbody = $("tbody");
//     if(!tbody.loader)
//     {
//         tbody.loader = tbody.find('tr');
//         tbody.loader.detach();
//     }
//     tbody.empty();
//     tbody.append(tbody.loader);
//
//
// }
function MakeAddApp()
{
    var addAppBtn = $("#addAppBtn");
    var addAppModal = $("#addAppModal");
    // addAppModal.appNameInput = addAppModal.find('input[name="app-name"]');
    // addAppModal.procNameInput = addAppModal.find('input[name="proc-name"]');
    // addAppModal.reset = function () {
    //     addAppModal.appNameInput.val('');
    //     addAppModal.appNameInput.parent().removeClass('error');
    //     addAppModal.procNameInput.val('');
    //     addAppModal.procNameInput.parent().removeClass('error');
    // };
    // addAppModal.modal('setting',{
    //     onApprove : function () {
    //         //TODO: indicate that it is loading!
    //         var appName = addAppModal.appNameInput.val();
    //         if(appName.length == 0)
    //         {
    //             addAppModal.appNameInput.parent().addClass('error');
    //             return false;
    //         }
    //         var procName = addAppModal.procNameInput.val();
    //         if(procName.length == 0)
    //         {
    //             addAppModal.procNameInput.parent().addClass('error');
    //             return false;
    //         }
    //         //todo: extract this!
    //         var json = JSON.stringify({
    //             AppName:appName,
    //             ProcName:procName
    //         });
    //         $.post("/api/apps/addapp", json, function () {
    //             addAppModal.modal('hide');
    //             //todo:reshow hiding icon
    //             tableWrapper.reloadTable();
    //         });
    //         return false;
    //     }
    // });
    var $form =addAppModal.find('.ui.form');
    $form.form({
        fields: {
            AppName: {
                rules:[
                    {
                        type: 'empty',
                        prompt: "Application name cannot be empty"
                    }
                ]
            },
            ProcName: {
                rules: [
                    {
                        type: 'empty',
                        prompt: "Process name cannot be empty"
                    },
                    {
                        type   : 'regExp',
                        value : '^[^\\\\\/:\"><|]+$',
                        prompt : 'Process name cannot contain [\\/:"><|] symbols'
                    }
                ]
            }
        }
    });
    addAppModal.reset = function () {
        $form.form('clear');
        $form.find('.error.message').html('');
    };

    addAppModal.modal('setting',{
        onApprove: function () {
            //TODO: indicate that it is loading!
            if(!$form.form('validate form')){
                return false;
            }
            var data = $form.form('get values');
            var json = JSON.stringify(data);
            /*$.post("/api/apps/addapp", json, function () {
                addAppModal.modal('hide');
                //todo:reshow hiding icon
                tableWrapper.reloadTable();
            });*/
            return false;
        }
    });

    addAppBtn.click(function () {
        addAppModal.reset();
        addAppModal.modal("show");
    });
}
$(document).ready(function () {
    MakeAddApp();
    tableWrapper = new TableWrapper($('table'));
    tableWrapper.reloadTable();
    //ReloadTable();
});