/**
 * Created by ALEX on 10.05.2017.
 */
function _UsertableController(){
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
UsertableController = new _UsertableController();