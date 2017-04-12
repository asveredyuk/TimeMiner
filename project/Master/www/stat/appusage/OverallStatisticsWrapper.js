/**
 * Created by ALEX on 12.04.2017.
 * @return {string}
 */
function StringifyMoment(mom)
{
    var durationStr = '';
    var days = mom.days();
    var hours = mom.hours();
    var minutes = mom.minutes();
    var seconds = mom.seconds();
    var res = '';
    if(days > 0)
    {
        res = days + 'd' + hours.pad() + 'h';
    }
    else if(hours > 0)
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
function OverallStatisticsWrapper($context)
{
    var that = this;
    that.totalStatVal = $context.find(".ostat_totaltime");
    that.distractionsStatVal = $context.find(".ostat_distractions");
    that.productiveStatVal = $context.find(".ostat_productive");
    //TODO: this hardcoded html is not very good
    var loaderHtml = '<div class="ui active inline loader"></div>';

    that.reloadStats = function(arg)
    {
        that.totalStatVal.html(loaderHtml);
        that.distractionsStatVal.html(loaderHtml);
        that.productiveStatVal.html(loaderHtml);

        var jobj = {
            Begin : arg.from,
            End : arg.to
        };
        var json = JSON.stringify(jobj);
        $.ajax({
            url: "/api/stat/overall_productivity",
            type: 'POST',
            data: json
        }).done(function (msg) {
            var obj = JSON.parse(msg);
            that.totalStatVal.html(StringifyMoment(moment.duration(obj.TotalTime,'seconds')));
            that.distractionsStatVal.html(StringifyMoment(moment.duration(obj.DistractionsTime,'seconds')));
            that.productiveStatVal.html(StringifyMoment(moment.duration(obj.ProductiveTime,'seconds')));
        });
    };

}