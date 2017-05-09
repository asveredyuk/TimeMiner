/**
 * Created by ALEX on 15.04.2017.
 */

function _StatController()
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
    this.userId = function (val, silent) {
        if(typeof val != 'undefined')
        {
            //setter
            userId = val;
            console.log("user id changed to " + val);
            if(!silent)
            {
                onUserIdChanged.fire(val);
            }
        }
        return userId;
    };
    var onAnyChanged = $.Callbacks();
    this.onAnyChanged = onAnyChanged;
    onIntervalChanged.add(onAnyChanged.fire);
    onUserIdChanged.add(onAnyChanged.fire);
}
//singleton XD
StatController = new _StatController();
//Make user id persistent
function RestoreUserId(){
    var cookie = Cookies.get('stat_userid');
    if(typeof cookie != 'undefined')
    {
        StatController.userId(cookie, 1);
    }
    StatController.onUserIdChanged.add(function (value) {
        Cookies.set('stat_userid',value);
    })
}
RestoreUserId();