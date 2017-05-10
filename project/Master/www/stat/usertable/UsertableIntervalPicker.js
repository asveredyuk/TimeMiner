/**
 * Created by ALEX on 10.05.2017.
 */
function UsertableIntervalPicker(tbFrom, tbTo) {
    var that = this;
    tbFrom.datepicker();
    tbFrom.change(function () {
        //var val = tbFrom.val();
        var val = tbFrom.datepicker('getDate');
        var mom = moment(val);
        var newInterval = new StatInterval(mom, UsertableController.interval().to);
        if(newInterval.to < newInterval.from){
            //make one-day interval
            newInterval.to = moment(newInterval.from).endOf('day');
        }
        UsertableController.interval(newInterval);
        tbFrom.blur();
    });
    tbTo.datepicker();
    tbTo.change(function () {
        var val = tbTo.datepicker('getDate');
        var mom = moment(val).endOf('day');
        var newInterval = new StatInterval(UsertableController.interval().from, mom);
        if(newInterval.to < newInterval.from){
            //make one-day interval
            console.log("larger!");
            newInterval.from = moment(newInterval.to).startOf('day');
            console.log(JSON.stringify(newInterval));
        }
        UsertableController.interval(newInterval);
        tbTo.blur();
    });
    this.refreshView = function () {
        var interval = UsertableController.interval();
        tbFrom.datepicker('setDate', interval.from.toDate());
        tbTo.datepicker('setDate', interval.to.toDate());
    };
    tbFrom.datepicker('option','dateFormat', 'dd-mm-yy');
    tbTo.datepicker('option','dateFormat', 'dd-mm-yy');
    UsertableController.onIntervalChanged.add(this.refreshView);
}