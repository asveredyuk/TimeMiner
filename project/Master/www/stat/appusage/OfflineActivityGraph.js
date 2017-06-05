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


    var popup = $('<div class="ui popup"> <div class="header">Hello here!</div> <div class="ui star rating" data-rating="3"></div> </div>');
    popup.appendTo(domElement);
    var ghost = $('<div style="width: 50px; height: 5px;position: relative; margin-bottom: -5px"></div>');
    var inner = $('<div style="width: 100%; height: 100%"></div>');
    inner.appendTo(ghost);
    ghost.prependTo(domElement);
    inner.popup({
        popup: popup,
        on:'manual'
    });

    this.recalculateSizes = function () {
        this.totalW = $context.width();
        this.totalH = $context.height();
        this.barsH = this.totalH-BOTTOM_LABELS_H ;
        this.barsW = this.totalW-LEFT_RIGHT_EMPTY*2;
    };
    function PrepareData(arr)
    {
        $.each(arr, function (index, value) {
            value.from = moment(value.Begin);
            value.to = moment(value.End);
            value.name = value.ShortName;
            value.type = value.Relevance*1;
        })
    }
    this.redraw = function(){
        draw.clear();
        if(inner.popup("is visible"))
        {
            inner.popup('hide');
        }
        var rect = draw.rect(that.totalW, that.barsH).attr({fill:'#EEE'});
        var data = that.data;
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
        var colors = ['#21ba45', '#db2828'];
        $.each(data, function (key, value) {
            var fromMs = value.from.diff(start);
            var lenms = value.to.diff(value.from);
            var rectWidth = lenms*pixPerMs-INNER_MARGIN*2;
            var moveX = fromMs*pixPerMs+INNER_MARGIN;
            var rect = group.rect(rectWidth,that.barsH).move(moveX,0).attr({fill:colors[value.type]});
            rect.click(function () {
                if(inner.boundItem == rect)
                {
                    //popup was already shown for given element
                    inner.popup('hide');
                    inner.boundItem = null;
                    return;
                }
                //move ghost element to the center of block
                if(inner.popup("is visible"))
                {
                    ghost.animate({marginLeft:moveX + LEFT_RIGHT_EMPTY + rectWidth/2-25}, 150);
                }
                else
                {
                    ghost.css({marginLeft:moveX + LEFT_RIGHT_EMPTY + rectWidth/2-25});
                }

                //TODO: set template view here
                popup.html(value.name);
                inner.popup('reposition');
                //show the popup
                inner.popup('show');
                //assign bound item to this
                inner.boundItem = rect;
            });
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
        var poses = []; //list of positions of added items
        for(var i = 0; i < times.length; i++) {
            var curTime = times[i];
            var msFromStart = curTime.diff(start);
            var curPos = msFromStart*pixPerMs;
            var found = false;
            for(var j =0; j < poses.length; j++){
                var dist = Math.abs(poses[j]-curPos);
                if(dist < 50) {
                    found = true;
                    break;
                }
            }
            if(found) {
                continue; // it is too near
            }
            poses.push(curPos);
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
        ApiBoundary.loadOfflineActivity(interval, userid, function (arr) {
            ApiBoundary.loadActiveTimeBounds(interval,userid, function (boundsArr) {

                PrepareData(arr);
                that.data = arr;
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

        });
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
