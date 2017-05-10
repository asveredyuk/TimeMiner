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
        UsertableController.interval(newInterval);
        //alert(tbFrom.val());
    });
    tbTo.datepicker();
    tbTo.change(function () {
        var val = tbTo.datepicker('getDate');
        var mom = moment(val).endOf('day');
        var newInterval = new StatInterval(UsertableController.interval().from, mom);
        UsertableController.interval(newInterval);
    });
    this.refreshView = function () {
        var interval = UsertableController.interval();
        tbFrom.datepicker('setDate', interval.from.toDate());
        tbTo.datepicker('setDate', interval.to.toDate());
    };
    tbFrom.datepicker('option','dateFormat', 'dd-mm-yy');
    tbTo.datepicker('option','dateFormat', 'dd-mm-yy');
    UsertableController.onIntervalChanged.add(this.refreshView);
    //tbTo.datepicker('setDate', moment().toDate());


}