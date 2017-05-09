/**
 * Created by ALEX on 16.04.2017.
 */
Number.prototype.pad = function (size) {
    var s = String(this);
    while (s.length < (size || 2)) {
        s = "0" + s;
    }
    return s;
};
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
function DelayedApplier(applyFunc, disapplyFunc, delay) {
    var timeout;
    this.apply = function () {
        clearTimeout(timeout);
        timeout = setTimeout(applyFunc,delay);
    };
    this.disapply= function () {
        clearTimeout(timeout);
        disapplyFunc();
    }
}
var ScrollControl = {
    nowScroll:0,
    rememberPos: function () {
        this.nowScroll = $(window).scrollTop();
    },
    restoreScroll:function () {
        $(window).scrollTop(this.nowScroll);
    }
};
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