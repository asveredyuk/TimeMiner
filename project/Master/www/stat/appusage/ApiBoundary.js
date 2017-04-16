/**
 * Created by ALEX on 16.04.2017.
 */
//all calls to the server should be done from here
var ApiBoundary = {
    //TODO: think about this and how it may be done later (like big overlay with error);
    //TODO: all failed calls must be collected to be retried later
    erroredCalls:[],
    runned:true,
    retryCalls : function () {
        this.runned = false;
        var that = this;
        setTimeout(function () {
            alert("Failed to connect to server. Press ok to try again");
            var calls = that.erroredCalls;
            that.erroredCalls = [];
            for(var i = 0; i < calls.length; i++)
            {
                var call = calls[i];
                call.func.apply(that,call.args);
                //callFunc.apply(this, args);
            }
            that.runned = true;
        },1);

    },
    whenError : function (callFunc, args) {
        var call = {
            func: callFunc,
            args: args
        };
        this.erroredCalls.push(call);
        if(this.runned)
        {
            //we need to run alert and refresh
            this.retryCalls();
        }
    },
    loadOverallStats : function (statInterval, callback) {
        var jobj = {
            Begin : statInterval.from,
            End : statInterval.to
        };
        var json = JSON.stringify(jobj);
        var apiCallArgs = arguments;
        $.ajax({
            url: "/api/stat/overall_productivity",
            type: 'POST',
            data: json
        }).done(function (msg) {
            var obj = JSON.parse(msg);
            callback(obj);
        }).fail(function () {
            ApiBoundary.whenError(ApiBoundary.loadOverallStats, apiCallArgs);
        });
    },
    loadProgramUsageStats : function (statInterval, callback) {
        var jobj = {
            Begin : statInterval.from,
            End : statInterval.to
        };
        var json = JSON.stringify(jobj);
        var apiCallArgs = arguments;
        $.ajax({
            url: "/api/stat/appusage",
            type: 'POST',
            data: json
        }).done(function (msg) {
            var arr = JSON.parse(msg);
            callback(arr);
        }).fail(function () {
            ApiBoundary.whenError(ApiBoundary.loadProgramUsageStats, apiCallArgs);
        });
    },
    loadOverallStatsSeparate : function(statInterval, callback){
        var jobj = {
            Begin : statInterval.from,
            End : statInterval.to
        };
        var json = JSON.stringify(jobj);
        var apiCallArgs = arguments;
        $.ajax({
            url: "/api/stat/overall_prod_per_day",
            type: 'POST',
            data: json
        }).done(function (msg) {
            var obj = JSON.parse(msg);
            callback(obj);
        }).fail(function () {
            ApiBoundary.whenError(ApiBoundary.loadOverallStatsSeparate, apiCallArgs);
        });
    }
};