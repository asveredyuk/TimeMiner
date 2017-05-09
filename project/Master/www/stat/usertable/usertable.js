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
        var interval = new StatInterval(moment("1-5-2017"),moment("10-5-2017"));
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
}
$(document).ready(function () {
    var table = new TableWrapper($('#statTable'));
    table.reloadTable();
});