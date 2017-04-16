/**
 * Created by ALEX on 15.04.2017.
 */
function StatInterval(from, to)
{
    if(typeof to == 'undefined')
    {
        //one type argument
        from = moment(from).startOf('day');
        to = moment(from).endOf('day');
    }
    this.from = from;
    this.to = to;
    this.valueOf = function () {
        return this.from + "-" + this.to;
    }
}
function StatController()
{
    //current interval
    var interval = {};

    var onIntervalChanged = $.Callbacks();
    //event, raised when value changed
    this.onIntervalChanged = onIntervalChanged;
    this.interval = function(val)
    {
        if(typeof val != "undefined")
        {
            //setter
            interval = val;
            onIntervalChanged.fire(val);
        }
        return interval;
    };
}
//singleton XD
StatController = new StatController();