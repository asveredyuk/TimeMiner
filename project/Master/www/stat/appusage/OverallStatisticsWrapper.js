/**
 * Created by ALEX on 12.04.2017.
 * @return {string}
 */
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

        ApiBoundary.loadOverallStats(arg, function (res) {
            that.totalStatVal.html(CommonUtils.stringifyDurationExtended(moment.duration(res.TotalTime,'seconds')));
            that.distractionsStatVal.html(CommonUtils.stringifyDurationExtended(moment.duration(res.DistractionsTime,'seconds')));
            that.productiveStatVal.html(CommonUtils.stringifyDurationExtended(moment.duration(res.ProductiveTime,'seconds')));
        });
    };
    StatController.onIntervalChanged.add(function (arg) {
        that.reloadStats(arg);
    });

}