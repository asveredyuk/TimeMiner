/**
 * Created by ALEX on 16.04.2017.
 */
var CommonUtils = {
    stringifyDurationExtended:function(dur)
    {
        //TODO: think about days
        var durationStr = '';
        //var days = dur.asDays();
        var hours = dur.hours();
        var minutes = dur.minutes();
        var seconds = dur.seconds();
        var res = '';
        //if(days > 0)
        //{
        //    res = days + 'd' + hours.pad() + 'h';
        //}
        /*else*/ if(hours > 0)
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
        return res;
    }
};