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
        var day = moment().startOf('day').add(hours,'hours');
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
        //suppose, it is properly ordered
        //TODO: use first and last log for this, not dates in periods
        //var start = data[0].from;
        //var end= data[data.length-1].to;
        var start = mktime(12.02);
        var end = mktime(23.45);

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


    $(window).resize(function () {
        var wNew = $context.width();
        var hNew = $context.height();
        if(wNew != that.totalW|| hNew != that.totalW)
        {
            that.recalculateSizes();
            that.redraw();
        }
    });
    that.recalculateSizes();
    that.redraw();
}
