/**
 * Created by ALEX on 17.04.2017.
 */
function MonthSelector($context)
{
    var that = this;
    this.goBack = function () {
        var interval = StatController.interval();
        var newInterval = new StatInterval(moment(interval.from).subtract(1,'months'),moment(interval.to).subtract(1,'months'));
        StatController.interval(newInterval);
    };
    this.goForward = function () {
        var interval = StatController.interval();
        var newInterval = new StatInterval(moment(interval.from).add(1,'months'),moment(interval.to).add(1,'months'));
        StatController.interval(newInterval);
    };
    this.refreshView = function (val) {
        var text = val.from.format('MMMM YYYY');
        that.controls.dateSelectorBt.html(text);
    };
    this.controlSetup = function () {
        this.controls = {
            dateSelectorBt: $context.find('.dateSelectorBt'),
            dateSelectorBtPrev: $context.find('.dateSelectorBtPrev'),
            dateSelectorBtNext: $context.find('.dateSelectorBtNext')
        };
        this.controls.dateSelectorBtPrev.click(function () {
            that.goBack();
        });
        this.controls.dateSelectorBtNext.click(function () {
            that.goForward();
        });
    };
    this.controlSetup();
    StatController.onIntervalChanged.add(this.refreshView);
}