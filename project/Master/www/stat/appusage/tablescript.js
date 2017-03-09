/**
 * Created by ALEX on 02.03.2017.
 */
Number.prototype.pad = function(size) {
    var s = String(this);
    while (s.length < (size || 2)) {s = "0" + s;}
    return s;
};
function RowWrapper(tBody, data, template) {
    var that = this;
    var EnumToColor = {
        0:'green',
        1:'yellow',
        2:'red',
        3:'grey'
    };
    //this indicates that no changes can be applyed
    this.data = data;
    this.row = $(Mustache.render(template,data));

    tBody.append(this.row);
    this.postRowUpdate = function () {
        var mom = moment.duration(this.data.SecondsSpent,'seconds');
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
        this.row.find('.timeLabel').html(res);
        var progressBar = this.row.find('.ui .progress');
        progressBar.progress({
            showActivity:false
        });
        progressBar.progress('set percent', this.data.Percent);
        if(this.data.Percent < 5)
            progressBar.find('.progress').remove();
        progressBar.addClass(EnumToColor[this.data.Desc.Rel]);
    };
    this.postRowUpdate();

    //replace time with good one

    //row.css("background-color","red");
    //relevance change buttons

}

function TableWrapper(ctxt)
{
    var that = this;
    this.tbody = ctxt.find('tbody');
    this.loader = this.tbody.find('tr');
    this.reloadTable = function () {
        this.tbody.empty();
        this.tbody.append(this.loader);
        $.ajax("/api/stat")
            .done(function (msg) {
                $.ajax("/stat/appusage/tablerow.html")
                    .done(function (template) {
                        var arr = JSON.parse(msg);
                        that.loader.detach();
                        Mustache.parse(template);
                        $.each(arr, function (key, value) {
                            //var res = Mustache.render(template,value);
                            //var row = tbody.append(res);
                            var r = new RowWrapper(that.tbody,value,template);
                        });
                    });

            })
            .fail(function () {
                //set message that failed to load
                console.log("failed to load data");
            });
    }
}

$(document).ready(function () {
    var wrapper = new TableWrapper($("table"));
    wrapper.reloadTable();
    //ReloadTable();
});