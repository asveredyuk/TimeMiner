/**
 * Created by ALEX on 02.03.2017.
 */

Number.prototype.pad = function (size) {
    var s = String(this);
    while (s.length < (size || 2)) {
        s = "0" + s;
    }
    return s;
};
function RowWrapper(tBody, data, template) {
    var that = this;
    var EnumToColor = {
        0: 'green',
        1: 'yellow',
        2: 'red',
        3: 'grey'
    };
    //this indicates that no changes can be applyed
    this.data = data;
    this.row = $(template(data));

    tBody.append(this.row);
    this.postRowUpdate = function () {
        var mom = moment.duration(this.data.SecondsSpent, 'seconds');
        var res = CommonUtils.stringifyDurationExtended(mom);
        this.row.find('.timeLabel').html(res);
        var progressBar = this.row.find('.ui .progress');
        progressBar.progress({
            showActivity: false
        });
        progressBar.progress('set percent', this.data.Percent);
        if (this.data.Percent < 5)
            progressBar.find('.progress').remove();
        progressBar.addClass(EnumToColor[this.data.Desc.Rel]);
    };
    this.postRowUpdate();
}

function TableWrapper(ctxt) {
    var that = this;
    this.tbody = ctxt.find('tbody');
    this.loader = that.tbody.find('tr');
    var loadingApplier = new DelayedApplier(
        function () {
            that.tbody.empty();
            that.tbody.append(that.loader);
        },
        function () {
            that.loader.detach();
            that.tbody.empty();
        },
        200
    );
    this.reloadTable = function () {
        var interval = StatController.interval();
        var userId = StatController.userId();
        loadingApplier.apply();
        ApiBoundary.loadProgramUsageStats(interval, userId, function (arr) {
            $.ajax("/stat/appusage/tablerow.hbs")
                .done(function (tpl) {
                    loadingApplier.disapply();
                    var template = Handlebars.compile(tpl);
                    //Mustache.parse(template);
                    $.each(arr, function (key, value) {
                        //var res = Mustache.render(template,value);
                        //var row = tbody.append(res);
                        var r = new RowWrapper(that.tbody, value, template);
                    });
                });
            }
        );
    };
    StatController.onAnyChanged.add(this.reloadTable);
}
$(document).ready(function () {
    var selector = new PeriodSelector($('#selector'));
    var mselector = new MonthSelector($('#monthSelector'));
    var wrapper = new TableWrapper($(".statTable"));
    var ostat = new OverallStatisticsWrapper($(".ostat"));
    var prodGraph = new ProductivityGraph($("#prodGraph").get(0));
    var userSelector = new UserSelector($("#userselect"));
    StatController.interval(new StatInterval(moment()));
});