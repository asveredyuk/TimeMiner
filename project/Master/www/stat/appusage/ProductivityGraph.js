/**
 * Created by ALEX on 16.04.2017.
 */
function ProductivityGraph(domElement)
{
    var that = this;
    var RED= '#be5457';//'#D35D60';
    var GREEN = '#77a953';//'#84BB5C';

    var DATES_COLOR = "#555";

    var YELLOW = '#DBBD08';
    var MARGIN = 15;
    var BOTTOM_LABELS_H = 28;
    var $context = $(domElement);
    var draw = SVG(domElement);
    var point;
    var bars = []; //bars objects
    var loadingApplier = new DelayedApplier(
        function () {
            $(domElement).addClass('loading');
        },
        function () {
            $(domElement).removeClass('loading');
        },
        200
    );
    this.recalculateSizes = function () {
        this.totalW = $context.width();
        this.totalH = $context.height();

        this.barsH = this.totalH-BOTTOM_LABELS_H ;
        this.barsW = this.totalW;
    };

    this.redraw = function () {
        console.log(this.totalW + 'x' + this.totalH);
        bars = [];
        var currentDate = StatController.interval().from; //TODO: may be a problem later
        var count = this.data.length;
        var max = 0;
        $(this.data).each(function () {
            var d = this;
            if(max < d.TotalTime)
            {
                max = d.TotalTime;
            }
        });
        draw.clear();

        var itemW = (this.barsW-(count-1)*MARGIN)/count;
        var barsGroup = draw.group().move(0,this.barsH).transform({scaleY:this.barsH/max}).flip('y');
        var dates = draw.group().move(0,this.barsH+10);
        var lines = draw.group().move(0,this.barsH);
        lines.line(0,0,this.totalW,0).stroke({width:0.25});
        //draw each element
        $.each(this.data, function (index, value) {
            var d = value;
            var x = (itemW+MARGIN)*index;
            var group = barsGroup.group();
            var bgRect = group.rect(itemW,max).move(x,0).attr({ fill: 'transparent' });
            bgRect.click(function () {
                var date = moment(d.Date);
                var newInterval = new StatInterval(date);
                StatController.interval(newInterval);
            });
            var goodRect= group.rect(itemW,d.ProductiveTime).move(x,0).attr({ fill: GREEN , 'pointer-events':'none'});
            var badRect= group.rect(itemW,d.DistractionsTime).move(x,d.ProductiveTime).attr({ fill: RED ,'pointer-events':'none'});
            var pointPlace = {
                x: x + itemW/2 - 3.5,
                y:3.5
            };
            if(moment(d.Date).startOf('day').valueOf() == moment(currentDate).startOf('day').valueOf()) {
                point = lines.circle(7,7).attr({fill:"#444d56"}).move(pointPlace.x, pointPlace.y);
            }
            var text = dates.plain(moment(d.Date).date() + "");
            text.font({
                family:   'Helvetica',
                size:     15,
                anchor:   'middle'
            });
            text.attr({ fill: DATES_COLOR });
            text.move(x + itemW/2,0);
            var bar = {
                data:d,
                ptPlace:pointPlace
            };
            bars.push(bar);
        });

    };
    this.movePoint = function (dateTo) {
        for(var i = 0; i < bars.length; i++)
        {
            var bar = bars[i];
            if(moment(bar.data.Date).startOf('day').valueOf() == dateTo.valueOf())
            {
                var pos = bar.ptPlace;
                point.animate(1500, function (pos) {
                    if (pos == !!pos) return pos;
                    return Math.pow(2, -8 * Math.sqrt(pos)) * Math.sin((pos - 0.075) * (2 * Math.PI) / .3) + 1;
                    //return .04 * t / (--t) * Math.sin(25 * t)
                }).move(pos.x,pos.y);
                return;
            }
        }
        console.log("point pos not found!")
    };
    StatController.onIntervalChanged.add(function (interval) {
        var intervalCenter = moment((interval.from + interval.to)/2);
        var monthBegin = moment(intervalCenter).startOf('month');
        var monthEnd = moment(intervalCenter).endOf('month');
        var newInterval = new StatInterval(monthBegin, monthEnd);
        if(typeof that.nowInterval == 'undefined' || that.nowInterval.valueOf() != newInterval.valueOf())
        {
            //month interval changed
            that.nowInterval = newInterval;

            loadingApplier.apply();
            ApiBoundary.loadOverallStatsSeparate(newInterval, function (data) {
                loadingApplier.disapply();
                //in data we ba have not all monthes
                var arrData = [];
                var day = moment(that.nowInterval.from).startOf('day');
                while (day < that.nowInterval.to)
                {
                    var found = null;
                    for(var i = 0; i < data.length; i++)
                    {
                        if(moment(data[i].Date).startOf('day').valueOf() == day.valueOf())
                        {
                            found = data[i];
                            break;
                        }
                    }
                    if(found)
                    {
                        arrData.push(found);
                    }
                    else
                    {
                        var dummy = {
                            Date: moment(day),
                            ProductiveTime:0,
                            DistractionsTime:0,
                            TotalTime:0
                        };
                        arrData.push(dummy);
                    }
                    day.add(1,'days');
                }
                that.data = arrData;
                that.recalculateSizes();
                that.redraw();
            })
        }
        that.movePoint(moment(interval.from).startOf('day'));

    });
    $(window).resize(function () {
        var wNew = $context.width();
        var hNew = $context.height();
        if(wNew != that.totalW|| hNew != that.totalW)
        {
            if(typeof that.nowInterval == 'undefined')
            {
                return;
            }
            that.recalculateSizes();
            that.redraw();
        }
    });
    //init
    //this.recalculateSizes();
    //this.redraw();


}