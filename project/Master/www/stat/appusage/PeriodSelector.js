/**
 * Created by ALEX on 16.04.2017.
 */
function PeriodSelector($context) {
    var that = this;


    this.goBack = function () {
        var prev = moment(that.val.from).subtract(1, 'day');
        that.changeValueByUser(prev);
    };
    this.goForward = function () {
        var next = moment(that.val.from).add(1, 'day');
        that.changeValueByUser(next);
    };
    this.refreshView = function (val) {
        var text = val.from.format('DD.MM.YY');
        that.controls.dateSelectorBt.html(text);
        //that.controls.datePicker.datepicker('setDate', val.from.toDate());
    };
    //view setup
    this.controlSetup = function () {
        that.controls = {
            dateSelectorBt: $context.find('.dateSelectorBt'),
            dateSelectorBtPrev: $context.find('.dateSelectorBtPrev'),
            dateSelectorBtNext: $context.find('.dateSelectorBtNext'),
            datePicker: $context.find('.dateSelectDatePicker'),
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
            that.changeValueByUser(val);
        });
        that.controls.datePicker.datepicker();
    };
    this.controlSetup();

    var waitTimer;
    this.changeValueByUser = function (date) {
        this.val = new StatInterval(date);
        this.refreshView(this.val);
        clearTimeout(waitTimer);
        waitTimer = setTimeout(function () {
            //set the interval
            StatController.interval(that.val);
            //that.onChangedValueAccepted.fire(arg);
        }, 500);
    };
    StatController.onIntervalChanged.add(function (newVal) {
        that.val = newVal;
        that.refreshView(newVal);
    });
}