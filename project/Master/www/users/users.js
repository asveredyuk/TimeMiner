/**
 * Created by ALEX on 06.05.2017.
 */
var tableWrapper;
function download(filename, text) {
    var pom = document.createElement('a');
    pom.setAttribute('href', 'data:text/plain;charset=utf-8,' + encodeURIComponent(text));
    pom.setAttribute('download', filename);

    if (document.createEvent) {
        var event = document.createEvent('MouseEvents');
        event.initEvent('click', true, true);
        pom.dispatchEvent(event);
    }
    else {
        pom.click();
    }
}
function RowWrapper(tbody, data, hbstemplate)
{
    var that = this;
    this.data = data;
    this.row = $(hbstemplate(data));
    tbody.append(this.row);
    var rmbutton = this.row.find('.rmbutton');
    rmbutton.click(function () {
        rmbutton.addClass('loading');
        var json = JSON.stringify({Id:that.data.Id});
        ApiBoundary.deleteUser(json, function () {
            that.row.remove();
        });
    });
    var dlcfgbutton = this.row.find('.dlcfgbutton');
    dlcfgbutton.click(function () {
        dlcfgbutton.addClass('loading');
        ApiBoundary.loadConfigForUser(that.data.Id, function (text) {
            dlcfgbutton.removeClass('loading');
            download('config.json',text);
        });
        
    });
}
function TableWrapper(ctxt)
{
    var that = this;
    this.tbody = ctxt.find('tbody');
    this.loader = this.tbody.find('tr');
    this.reloadTable = function(){
        this.tbody.empty();
        this.tbody.append(this.loader);
        ApiBoundary.loadUsersList(function (data) {
            ApiBoundary.loadFrontendFile("/users/tablerow.hbs", function (tpl) {
                that.loader.detach();
                var template = Handlebars.compile(tpl);
                $.each(data, function (key, value) {
                    var r = new RowWrapper(that.tbody, value, template);
                })
            });
        });
    }
}
function MakeModal()
{
    var addUserBtn = $('.addUserBtn');
    var addUserModal = $('#addUserModal');


    var $form =addUserModal.find('.ui.form');
    $form.find('.ui.dropdown').dropdown();
    $.fn.form.settings.rules.guidOrEmpty = function(value) {
        if(value.length == 0)
            return true;
        var pattern = new RegExp('^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$', 'i');
        if(pattern.test(value))
            return true;
        return false;
    };
    $form.form({
        fields: {
            Name: {
                rules:[
                    {
                        type: 'empty',
                        prompt: "Name cannot be empty"
                    }
                ]
            },
            Surname: {
                rules: [
                    {
                        type: 'empty',
                        prompt: "Surname cannot be empty"
                    }
                ]
            },
            Id: {
                rules: [
                    {
                        type: 'guidOrEmpty',
                        prompt: "Id must be specified as GUID or be empty"
                    }
                ]
            }
        }
    });
    addUserModal.reset = function () {
        $form.form('clear');
        $form.find('.error.message').html('');
        addUserModal.find('.actions .approve').removeClass('loading');
    };
    addUserModal.modal('setting',{
        onApprove: function () {
            //TODO: indicate that it is loading!
            if(!$form.form('validate form')){
                return false;
            }


            var data = $form.form('get values');
            var json = JSON.stringify(data);
            addUserModal.find('.actions .approve').addClass('loading');
            ApiBoundary.addUser(json, function () {
                addUserModal.modal('hide');
                //todo:reshow hiding icon
                tableWrapper.reloadTable();
            });
            return false;
        }
    });

    addUserBtn.click(function () {
        addUserModal.reset();
        addUserModal.modal("show");
    });
}
$(document).ready(function () {
    var table = new TableWrapper($("table"));
    tableWrapper = table;
    MakeModal();
    table.reloadTable();
});