/**
 * Created by ALEX on 02.03.2017.
 */
function PeriodSelector($context, current){
    var that = this;
    //this is called every time when value changes
    this.onValueChanged = $.Callbacks();
    //this is called only after value has changed and 100ms passed with no changes
    this.onChangedValueAccepted = $.Callbacks();
    //this function waits 500ms
    this.assignValue = function(date){
        var valFrom = moment(date).startOf('day');
        var valTo = moment(date).endOf('day');
        that.val = {
            from:valFrom,
            to:valTo
        };
        that.onValueChanged.fire(that.val);
    };
    this.goBack = function(){
        var prev = that.val.from.subtract(1,'day');
        that.assignValue(prev);
    };
    this.goForward = function(){
        var next = that.val.from.add(1,'day');
        that.assignValue(next);
    };
    this.refreshView = function(val){
        var text = val.from.format('DD.MM.YY');
        that.controls.dateSelectorBt.html(text);
        //that.controls.datePicker.datepicker('setDate', val.from.toDate());
    };
    //view setup
    this.controlSetup = function() {
        that.controls = {
            dateSelectorBt: $context.find('.dateSelectorBt'),
            dateSelectorBtPrev: $context.find('.dateSelectorBtPrev'),
            dateSelectorBtNext: $context.find('.dateSelectorBtNext'),
            datePicker : $context.find('.dateSelectDatePicker'),
            applyPickerBt: $context.find('.dateSelectorApplyPickerBt')
        };
        that.controls.dateSelectorBtPrev.click(function () {
            that.goBack();
        });
        that.controls.dateSelectorBtNext.click(function () {
            that.goForward();
        });
        that.controls.dateSelectorBt.popup({
            popup: '.dateSelectPopup',
            on: 'click'
        });
        that.controls.applyPickerBt.click(function () {
            that.controls.dateSelectorBt.popup('hide');
            var val = that.controls.datePicker.datepicker('getDate');
            that.assignValue(val);
        });
        that.controls.datePicker.datepicker();
    };
    //init
    this.controlSetup();
    this.onValueChanged.add(this.refreshView);
    this.assignValue(current);
    //implement onChangedValueAccepted firing
    //added here to avoid first fire (or move to top?)
    var waitTimer;
    this.onValueChanged.add(function (arg) {
        clearTimeout(waitTimer);
        waitTimer = setTimeout(function () {
            that.onChangedValueAccepted.fire(arg);
        },500);
    });
}
Number.prototype.pad = function(size) {
    var s = String(this);
    while (s.length < (size || 2)) {s = "0" + s;}
    return s;
};
function RowWrapper(tBody, data, template) {
    var that = this;
    var EnumToColor = {
        0:'green',
        1:'yellow',
        2:'red',
        3:'grey'
    };
    //this indicates that no changes can be applyed
    this.data = data;
    this.row = $(Mustache.render(template,data));

    tBody.append(this.row);
    this.postRowUpdate = function () {
        var mom = moment.duration(this.data.SecondsSpent,'seconds');
        var durationStr = '';
        var days = mom.days();
        var hours = mom.hours();
        var minutes = mom.minutes();
        var seconds = mom.seconds();
        var res = '';
        if(days > 0)
        {
            res = days + 'd' + hours.pad() + 'h';
        }
        else if(hours > 0)
        {
            res = hours + 'h' + minutes.pad() + 'm';
        }
        else if(minutes > 0)
        {
            res = minutes + 'm' + seconds.pad() + 's';
        }
        else {
            res = seconds + 's';
        }
        this.row.find('.timeLabel').html(res);
        var progressBar = this.row.find('.ui .progress');
        progressBar.progress({
            showActivity:false
        });
        progressBar.progress('set percent', this.data.Percent);
        if(this.data.Percent < 5)
            progressBar.find('.progress').remove();
        progressBar.addClass(EnumToColor[this.data.Desc.Rel]);
    };
    this.postRowUpdate();

    //replace time with good one

    //row.css("background-color","red");
    //relevance change buttons

}

function TableWrapper(ctxt)
{
    var that = this;
    this.tbody = ctxt.find('tbody');
    this.loader = that.tbody.find('tr');
    this.reloadTable = function (arg) {
        var jobj = {
            Begin : arg.from,
            End : arg.to
        };
        var json = JSON.stringify(jobj);
        that.tbody.empty();
        that.tbody.append(that.loader);
        $.ajax({
            url: "/api/stat",
            type: 'POST',
            data: json
        })
            .done(function (msg) {
                $.ajax("/stat/appusage/tablerow.html")
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
$(document).ready(function () {
    var selector = new PeriodSelector($('#selector'));
    var wrapper = new TableWrapper($(".statTable"));
    wrapper.reloadTable(selector.val);
    selector.onChangedValueAccepted.add(function (arg) {
        wrapper.reloadTable(arg);
    });
    //ReloadTable();
});