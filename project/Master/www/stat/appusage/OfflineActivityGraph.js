/**
 * Created by ALEX on 20.05.2017.
 */
function OfflineActivityGraph(domElement){
    var that = this;
    var BOTTOM_LABELS_H = 28;
    var LEFT_RIGHT_EMPTY = 30;
    var INNER_MARGIN = 1;
    var $context = $(domElement);
    var draw = SVG(domElement);
    this.recalculateSizes = function () {
        this.totalW = $context.width();
        this.totalH = $context.height();
        this.barsH = this.totalH-BOTTOM_LABELS_H ;
        this.barsW = this.totalW-LEFT_RIGHT_EMPTY*2;
    };

    function mktime(hours) {
        //make time at given day at some hours
        var day = moment().startOf('day').add(hours,'hours').add(-5,'days');
        return day;
    }
    function GenerateLog(){
        //make fake log for now
        var per1 = {
            from:mktime(15.14),
            to:mktime(15.66),
            name:"Break",
            type:0
        };
        var per2 = {
            from:mktime(16.10),
            to:mktime(17),
            name:"Work call",
            type:1
        };
        return [per1,per2];
    }
    this.redraw = function(){
        draw.clear();

        var rect = draw.rect(that.totalW, that.barsH).attr({fill:'#EEE'});
        var data = GenerateLog();
        if(data.length == 0)
            return;//empty!
        //suppose, it is properly ordered
        var start = that.displayFrom;
        var end = that.displayTo;
        if(start == null || end == null)
        {
            //recalculate according to the data
            start = data[0].from;
            end= data[data.length-1].to;
        }
        //total period length
        var periodLengthMs = end.diff(start);
        //pixels per one ms
        var pixPerMs = that.barsW/periodLengthMs;
        //draw each task on timeline
        var group = draw.group().move(LEFT_RIGHT_EMPTY,0);
        var times = [];
        var vals = [];
        var colors = ['#db2828','#21ba45'];
        $.each(data, function (key, value) {
            var fromMs = value.from.diff(start);
            var lenms = value.to.diff(value.from);
            var rect = group.rect(lenms*pixPerMs-INNER_MARGIN*2,that.barsH).move(fromMs*pixPerMs+INNER_MARGIN,0).attr({fill:colors[value.type]});
            var text = group.plain(value.name);
            text.font({
                family:   'Helvetica',
                size:     15,
                anchor:   'middle'
            });
            text.attr({ fill: '#FFF'});
            text.move(fromMs*pixPerMs + lenms*pixPerMs/2, that.barsH/2-8);
            if($.inArray(value.from.valueOf(), vals)<0) {
                times.push(value.from);
                vals.push(value.from.valueOf());
            }
            if($.inArray(value.to.valueOf(),vals)<0) {
                times.push(value.to);
                vals.push(value.to.valueOf());
            }
        });
        //draw times
        var timesGroup = draw.group().move(LEFT_RIGHT_EMPTY,that.barsH+10);
        for(var i = 0; i < times.length; i++) {
            var curTime = times[i];
            var msFromStart = curTime.diff(start);
            var curPos = msFromStart*pixPerMs;
            var text = timesGroup.plain(curTime.format('HH:mm'));
            text.font({
                family:   'Helvetica',
                size:     15,
                anchor:   'middle'
            });
            text.attr({ fill: '#555'});
            text.move(curPos,0);
            timesGroup.line(curPos,-10,curPos,0).stroke({color:'#CCC', width:2});
        }
    };
    this.reloadStats = function () {
        var interval = StatController.interval();
        var userid = StatController.userId();
        //ApiBoundary.loadTaskStats(interval, userid, function (arr) {
            ApiBoundary.loadActiveTimeBounds(interval,userid, function (boundsArr) {

                //PrepareData(arr);
                //that.data = arr;
                if(boundsArr.length == 0)
                {
                    that.displayFrom = null;
                    that.displayTo = null;
                }
                else
                {
                    that.displayFrom = moment(boundsArr[0].Begin);
                    that.displayTo = moment(boundsArr[boundsArr.length-1].End);
                }
                that.recalculateSizes();
                that.redraw();
            });

        //});
    };

    $(window).resize(function () {
        var wNew = $context.width();
        var hNew = $context.height();
        if(wNew != that.totalW|| hNew != that.totalW)
        {
            that.recalculateSizes();
            that.redraw();
        }
    });
    StatController.onAnyChanged.add(this.reloadStats);
}
