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
    var BOTTOM_LABELS_H = 20;
    var $context = $(domElement);
    var draw = SVG(domElement);
    this.recalculateSizes = function () {
        this.totalW = $context.width();
        this.totalH = $context.height();

        this.barsH = this.totalH-BOTTOM_LABELS_H ;
        this.barsW = this.totalW;
    };

    this.redraw = function () {
        console.log(this.totalW + 'x' + this.totalH);
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
        //var rect = draw.rect(w/2,h).move(w/4,0).attr({ fill: '#f06' });

        var itemW = (this.barsW-(count-1)*MARGIN)/count;
        var bars = draw.group().move(0,this.barsH).transform({scaleY:this.barsH/max}).flip('y');
        var dates = draw.group().move(0,this.barsH);
        // for(var i = 0; i < this.data.length; i++)
        // {
        //     var d = this.data[i];
        //     var x = (itemW+MARGIN)*i;
        //     var group = bars.group();
        //     var bgRect = group.rect(itemW,d.TotalTime).move(x,0).attr({ fill: 'transparent' });
        //     bgRect.click(function () {
        //
        //     });
        //     var goodRect= group.rect(itemW,d.ProductiveTime).move(x,0).attr({ fill: GREEN , 'pointer-events':'none'});
        //     var badRect= group.rect(itemW,d.DistractionsTime).move(x,d.ProductiveTime).attr({ fill: RED ,'pointer-events':'none'});
        //
        //
        //
        //     var text = dates.plain(moment(d.Date).date() + "");
        //     text.font({
        //         family:   'Helvetica',
        //         size:     15,
        //         anchor:   'middle'
        //     });
        //     text.attr({ fill: DATES_COLOR });
        //     text.move(x + itemW/2,0);
        // }
        $.each(this.data, function (index, value) {
            var d = value;
            var x = (itemW+MARGIN)*index;
            var group = bars.group();
            var bgRect = group.rect(itemW,d.TotalTime).move(x,0).attr({ fill: 'transparent' });
            bgRect.click(function () {
                var date = moment(d.Date);
                var newInterval = new StatInterval(date);
                StatController.interval(newInterval);
            });
            var goodRect= group.rect(itemW,d.ProductiveTime).move(x,0).attr({ fill: GREEN , 'pointer-events':'none'});
            var badRect= group.rect(itemW,d.DistractionsTime).move(x,d.ProductiveTime).attr({ fill: RED ,'pointer-events':'none'});

            var text = dates.plain(moment(d.Date).date() + "");
            text.font({
                family:   'Helvetica',
                size:     15,
                anchor:   'middle'
            });
            text.attr({ fill: DATES_COLOR });
            text.move(x + itemW/2,0);
        });
    };
    StatController.onIntervalChanged.add(function (interval) {
        var intervalCenter = moment((interval.from + interval.to)/2);
        var monthBegin = moment(intervalCenter).startOf('month');
        var monthEnd = moment(intervalCenter).endOf('month');
        var newInterval = new StatInterval(monthBegin, monthEnd);
        if(typeof that.nowInterval == 'undefined' || that.nowInterval.valueOf() != newInterval.valueOf())
        {
            //interval really changed
            that.nowInterval = newInterval;
            ApiBoundary.loadOverallStatsSeparate(newInterval, function (data) {
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