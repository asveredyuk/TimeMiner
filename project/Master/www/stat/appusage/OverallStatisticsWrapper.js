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
    var loadingApplier = new DelayedApplier(
        function () {
            that.totalStatVal.html(loaderHtml);
            that.distractionsStatVal.html(loaderHtml);
            that.productiveStatVal.html(loaderHtml);
        },
        function () {
            that.totalStatVal.html('');
            that.distractionsStatVal.html('');
            that.productiveStatVal.html('');
        },
        200
    );
    that.reloadStats = function(arg)
    {
        loadingApplier.apply();
        ApiBoundary.loadOverallStats(arg, function (res) {
            loadingApplier.disapply();
            that.totalStatVal.html(CommonUtils.stringifyDurationExtended(moment.duration(res.TotalTime,'seconds')));
            that.distractionsStatVal.html(CommonUtils.stringifyDurationExtended(moment.duration(res.DistractionsTime,'seconds')));
            that.productiveStatVal.html(CommonUtils.stringifyDurationExtended(moment.duration(res.ProductiveTime,'seconds')));
        });
    };
    StatController.onIntervalChanged.add(function (arg) {
        that.reloadStats(arg);
    });

}