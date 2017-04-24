function RowWrapper($tBody, data, template){
    var that = this;
    this.data = data;
    this.row = $(template(data));
    $tBody.append(this.row);
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
        //progressBar.addClass();
    };
    this.postRowUpdate();
}
function TableWrapper($ctxt){
    var that = this;
    this.tbody = $ctxt.find('tbody');
    this.loader = that.tbody.find('tr');

    this.loadTable = function(){
        ApiBoundary.loadUnknownAppsList(function (arr) {
            $.ajax("/apps/unknown/tablerow.hbs")
                .done(function (tpl) {
                    var template = Handlebars.compile(tpl);
                    that.loader.detach();
                    that.tbody.empty();
                    $.each(arr, function (key, value) {
                        //var res = Mustache.render(template,value);
                        //var row = tbody.append(res);
                        console.log("a");
                        var r = new RowWrapper(that.tbody, value, template);
                    });
                });
        })
    }
}
$(document).ready(function () {
    var wrapper = new TableWrapper($(".maintable"));
    wrapper.loadTable();
});