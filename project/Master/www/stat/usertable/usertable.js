/**
 * Created by ALEX on 10.05.2017.
 */
function RowWrapper(tBody, data, template) {
    var that = this;
    this.data = data;
    this.row = $(template(data));
    tBody.append(this.row);
}
function TableWrapper(ctxt){
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
        var interval = UsertableController.interval();
        loadingApplier.apply();
        ApiBoundary.getOverallUsersStats(interval, function (arr) {
                $.ajax("/stat/usertable/tablerow.hbs")
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
    UsertableController.onIntervalChanged.add(this.reloadTable);
}
$(document).ready(function () {
    var table = new TableWrapper($('#statTable'));
    var intervalPicker = new UsertableIntervalPicker($('#intervalFrom'),$('#intervalTo'));
    var nowInterval = new StatInterval(moment().startOf('day'), moment().endOf('day'));
    UsertableController.interval(nowInterval);

});