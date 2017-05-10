var AddPopup;
function RowWrapper($tBody, data, template){
    var that = this;
    this.data = data;
    this.row = $(template(data));
    $tBody.append(this.row);
    this.postRowUpdate = function () {
        var mom = moment.duration(this.data.SecondsSpent, 'seconds');
        var res = CommonUtils.stringifyDurationExtended(mom);
        this.row.find('.timeLabel').html(res);
        var progressBar = this.row.find('.ui .progress');
        progressBar.progress({
            showActivity: false
        });
        progressBar.progress('set percent', this.data.Percent);
        if (this.data.Percent < 5)
            progressBar.find('.progress').remove();
        //progressBar.addClass();
    };
    var addButton =this.row.find('.addbutton');
    var popupclass = '.addAppPopup';
    addButton.click(function () {
        AddPopup.switch(addButton, function (type) {
            AddPopup.unbindAndHide();
            var data = {
                AppName : that.data.Identifier,
                Type:type
            };
            switch(that.data.Type){
                case 'process':
                    data.ProcName = that.data.Identifier;
                    break;
                case 'site':
                    data.DomainName = that.data.Identifier;
                    break;
                default:
                    alert('error with this app');
                    return;
            }
            var json = JSON.stringify(data);
            addButton.addClass('loading');
            ApiBoundary.addApp(json, function () {
                addButton.removeClass('loading');
                addButton.addClass('disabled');
                addButton.html('<i class="checkmark icon"></i>');
            });
        });
        // var identifier = that.data.Identifier;
        // var action = {
        //     action:'add',
        //     identifier:identifier
        // };
        // Cookies.set('action', JSON.stringify(action));
        // switch(that.data.Type){
        //     case 'process':
        //         window.location.href = '/config/apps';
        //         break;
        //     case 'site':
        //         window.location.href = '/config/sites';
        //         break;
        //     default:
        //         alert('error with this app');
        //         break;
        // }
    });
    this.postRowUpdate();
}
function TableWrapper($ctxt){
    var that = this;
    this.tbody = $ctxt.find('tbody');
    this.loader = that.tbody.find('tr');

    this.loadTable = function(){
        ApiBoundary.loadUnknownAppsList(function (arr) {
            $.ajax("/apps/unknown/tablerow.hbs")
                .done(function (tpl) {
                    var template = Handlebars.compile(tpl);
                    that.loader.detach();
                    that.tbody.empty();
                    $.each(arr, function (key, value) {
                        //var res = Mustache.render(template,value);
                        //var row = tbody.append(res);
                        var r = new RowWrapper(that.tbody, value, template);
                    });
                });
        })
    }
}
$(document).ready(function () {
    AddPopup = new DynamicPopup($('.addAppPopup'), function () {
        var that = this;
        var goodbt = that.context.find('.addGoodBt');
        var neutralBt = that.context.find('.addNeutralBt');
        var badBt = that.context.find('.addBadBt');
        goodbt.click(function () {
            that.callback(0);
        });
        neutralBt.click(function () {
            that.callback(1);
        });
        badBt.click(function () {
            that.callback(2);
        });
    });
    var wrapper = new TableWrapper($(".maintable"));
    wrapper.loadTable();
});