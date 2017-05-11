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

function RowWrapper(tBody, data, hbstemplate) {
    var that = this;
    //this indicates that no changes can be applyed
    this.sending = false;
    this.data = data;
    this.row = $(hbstemplate(data));
    tBody.append(this.row);
    //relevance change buttons
    this.buttonWrapper = new ButtonsWrapper(this.row.find(".mybuttons"), this);
    this.buttonWrapper.onValueChanged = function (val, callback) {
        that.sending = true;
        data.Rel = val;
        var json = JSON.stringify(that.data);
        ApiBoundary.updateApp(json,function () {
            that.sending = false;
            callback(true);
        });
    };
    var rmbutton = this.row.find('.rmbutton');
    rmbutton.click(function () {
        rmbutton.addClass('loading');
        var json = JSON.stringify({Id:that.data.App.Id});
        ApiBoundary.deleteApp(json, function () {
            that.row.remove();
        });
    });
}
function TableWrapper(ctxt)
{
    var that = this;
    this.tbody = ctxt.find('tbody');
    this.loader = this.tbody.find('tr');
    this.lastSearch = "";
    this.reloadTable = function () {
        ScrollControl.rememberPos();
        this.tbody.empty();
        this.tbody.append(this.loader);
        ApiBoundary.getAppsList(type, function (arr) {
            ApiBoundary.loadFrontendFile('/apps/table/tablerow.hbs', function (tpl) {
                that.loader.detach();
                var template = Handlebars.compile(tpl);
                $.each(arr, function (key, value) {
                    //var res = Mustache.render(template,value);
                    //var row = tbody.append(res);
                    var r = new RowWrapper(that.tbody,value,template);

                });
                that.doSearch(that.lastSearch);
                ScrollControl.restoreScroll();
            });
        });
    };
    this.doSearch = function(searchValue){
        this.lastSearch = searchValue;
        searchValue = searchValue.toLowerCase();
        var rows = this.tbody.find('tr');
        rows.each(function () {
            var trs = $(this).find("td");
            var found = false;
            trs.each(function (index) {
                if(index > 1)
                    return false; //break
                var html = $(this).text();
                if(html.toLowerCase().indexOf(searchValue) > -1){
                    found = true;
                    return false; //break
                }
            });
            if(found)
                $(this).show();
            else
                $(this).hide();
        });
    };


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
    var addAppBtn = $(".addAppBtn");
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
    $form.find('.ui.dropdown').dropdown();
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
            },
            DomainName: {
                rules: [
                    {
                        type: 'empty',
                        prompt: "Site domain cannot be empty"
                    },
                    {
                        type   : 'regExp',
                        value : '^.*[^\\.\\\\/ ]+\.[^\\./ ]{2,6}$',
                        prompt : 'Domain is not valid'
                    }
                ]
            },
            Type : {
                rules:[
                    {
                        type :'empty',
                        prompt:'Select application type. Good apps are related to work, while Bad are distractions'
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
            addAppModal.find('.actions .approve').addClass('loading');
            ApiBoundary.addApp(json, function () {
                addAppModal.modal('hide');
                //todo:reshow hiding icon
                tableWrapper.reloadTable();
            });
            return false;
        }
    });

    addAppBtn.click(function () {
        addAppModal.reset();
        addAppModal.modal("show");
    });
}
function MakeTableSearch(callback)
{
    var $search = $("#tableSearchInput");
    var waitTimer;
    $search.keyup(function () {
        clearTimeout(waitTimer);
        var text = $(this).val();
        waitTimer= setTimeout(function () {
            callback(text);
        },100);
    });
}
$(document).ready(function () {
    MakeAddApp();
    tableWrapper = new TableWrapper($('table'));
    tableWrapper.reloadTable();
    MakeTableSearch(function (text) {
        tableWrapper.doSearch(text);
    });

    //ReloadTable();
});