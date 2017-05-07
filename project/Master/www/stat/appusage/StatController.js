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

    var userId = "00000000-0000-0000-0000-000000000000";
    var onUserIdChanged = $.Callbacks();
    this.onUserIdChanged = onUserIdChanged;
    this.userId = function (val) {
        if(typeof val != 'undefined')
        {
            //setter
            userId = val;
            console.log("user id changed to " + val);
            onUserIdChanged.fire(val);
        }
        return userId;
    };
    var onAnyChanged = $.Callbacks();
    this.onAnyChanged = onAnyChanged;
    onIntervalChanged.add(onAnyChanged.fire);
    onUserIdChanged.add(onAnyChanged.fire);

}
//singleton XD
StatController = new StatController();