function RowWrapper(tbody, data, template){
    var that = this;
    this.data = data;
    this.row = $(template(data));
    tbody.append(this.row);
}
function TableWrapper(ctxt)
{
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
        loadingApplier.apply();
        ApiBoundary.getPluginsList(function (arr) {
            ApiBoundary.loadFrontendFile("/plugins/tablerow.hbs", function (tpl) {
                loadingApplier.disapply();
                var template = Handlebars.compile(tpl);
                $.each(arr, function (key, value) {
                    var r = new RowWrapper(that.tbody,value,template);
                })
            });
        });
    }
}
$(document).ready(function () {
    var table = new TableWrapper($('#pluginsTable'));
    table.reloadTable();
});