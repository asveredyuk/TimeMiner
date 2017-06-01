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
            var errorString = JSON.stringify(that.erroredCalls[0].xhr);
            if(typeof errorString == 'undefined' || errorString.length < 2)
                errorString = "Server unavailable";
            errorString+= "\nRetry?";
            var res = confirm(errorString);
            if(res == false) {
                that.erroredCalls = [];
                return;//do not try to do anything
            }
            //alert("Failed to connect to server. Press ok to try again");
            var calls = that.erroredCalls;
            that.erroredCalls = [];
            for(var i = 0; i < calls.length; i++)
            {
                var call = calls[i];
                call.func.apply(that,call.args);
            }
            that.runned = true;
        },1);

    },
    whenError : function (callFunc, args, xhr) {
        var call = {
            func: callFunc,
            args: args,
            xhr : xhr
        };
        this.erroredCalls.push(call);
        if(this.runned)
        {
            //we need to run alert and refresh
            this.retryCalls();
        }
    },
    loadOverallStats : function (statInterval,userId, callback) {
        var jobj = {
            Begin : statInterval.from,
            End : statInterval.to,
            UserId : userId
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
    loadProgramUsageStats : function (statInterval, userId, callback) {
        var jobj = {
            Begin : statInterval.from,
            End : statInterval.to,
            UserId : userId
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
    loadOverallStatsSeparate : function(statInterval, userId, callback) {
        var jobj = {
            Begin: statInterval.from,
            End: statInterval.to,
            UserId:userId
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
    },
    loadUnknownAppsList : function (callback) {
        $.ajax({
            url: "/api/apps/unknown",
            type: 'GET'
        }).done(function (msg) {
            var obj = JSON.parse(msg);
            callback(obj);
        }).fail(function () {
            ApiBoundary.whenError(ApiBoundary.loadUnknownAppsList, apiCallArgs);
        });
    },
    //users
    loadUsersList : function(callback){
        var apiCallArgs = arguments;
        $.ajax({
            url: "/api/config/users/gettable",
            type: 'GET'
        }).done(function (msg) {
            var obj = JSON.parse(msg);
            callback(obj);
        }).fail(function () {
            ApiBoundary.whenError(ApiBoundary.loadUsersList, apiCallArgs);
        });
    },
    addUser : function(data, callback){
        var apiCallArgs = arguments;
        $.ajax({
            url: "/api/config/users/add",
            type: 'POST',
            data: data
        }).done(function () {
            callback();
        }).fail(function (xhr) {
            ApiBoundary.whenError(ApiBoundary.addUser, apiCallArgs, xhr);
        });
    },
    deleteUser : function (data, callback) {
        var apiCallArgs = arguments;
        $.ajax({
            url: "/api/config/users/delete",
            type: 'POST',
            data: data
        }).done(function () {
            callback();
        }).fail(function (xhr) {
            ApiBoundary.whenError(ApiBoundary.deleteUser, apiCallArgs, xhr);
        });
    },
    getOverallUsersStats : function(interval, callback)
    {
        var jobj = {
            Begin : interval.from,
            End : interval.to
        };
        var json = JSON.stringify(jobj);
        var apiCallArgs = arguments;
        $.ajax({
            url: "/api/stat/getusertable",
            type: 'POST',
            data: json
        }).done(function (msg) {
            var arr = JSON.parse(msg);
            callback(arr);
        }).fail(function () {
            ApiBoundary.whenError(ApiBoundary.getOverallUsersStats(), apiCallArgs);
        });
    },
    addApp : function (data, callback) {
        var apiCallArgs = arguments;
        $.ajax({
            url: "/api/apps/addapp",
            type: 'POST',
            data: data
        }).done(function () {
            callback();
        }).fail(function (xhr) {
            ApiBoundary.whenError(ApiBoundary.addApp, apiCallArgs, xhr);
        });
    },
    deleteApp : function (data, callback) {
        var apiCallArgs = arguments;
        $.ajax({
            url: "/api/apps/rmapp",
            type: 'POST',
            data: data
        }).done(function () {
            callback();
        }).fail(function (xhr) {
            ApiBoundary.whenError(ApiBoundary.deleteApp, apiCallArgs, xhr);
        });
    },
    updateApp : function (data, callback) {
        var apiCallArgs = arguments;
        $.ajax({
            url: "/api/apps/updateapp",
            type: 'POST',
            data: data
        }).done(function () {
            callback();
        }).fail(function (xhr) {
            ApiBoundary.whenError(ApiBoundary.deleteApp, apiCallArgs, xhr);
        });
    },
    getAppsList : function (type, callback) {
        var apiCallArgs = arguments;
        $.ajax({
            url: "/api/apps/gettable/" + type,
            type: 'GET'
        }).done(function (msg) {
            var obj = JSON.parse(msg);
            callback(obj);
        }).fail(function () {
            ApiBoundary.whenError(ApiBoundary.getAppsList, apiCallArgs);
        });
    },
    loadFrontendFile: function (path, callback) {
        var apiCallArgs = arguments;
        $.ajax({
            url: path,
            type: 'GET'
        }).done(function (msg) {
            callback(msg);
        }).fail(function () {
            ApiBoundary.whenError(ApiBoundary.loadFrontendFile, apiCallArgs);
        });
    },
    loadActiveTimeBounds: function (statInterval, userId, callback) {
        var jobj = {
            Begin : statInterval.from,
            End : statInterval.to,
            UserId : userId
        };
        var json = JSON.stringify(jobj);
        var apiCallArgs = arguments;
        $.ajax({
            url: "/api/stat/activebounds",
            type: 'POST',
            data: json
        }).done(function (msg) {
            var arr = JSON.parse(msg);
            callback(arr);
        }).fail(function () {
            ApiBoundary.whenError(ApiBoundary.loadActiveTimeBounds, apiCallArgs);
        });
    },
    loadTaskStats : function (statInterval, userId, callback) {
        var jobj = {
            Begin : statInterval.from,
            End : statInterval.to,
            UserId : userId
        };
        var json = JSON.stringify(jobj);
        var apiCallArgs = arguments;
        $.ajax({
            url: "/api/stat/taskstat",
            type: 'POST',
            data: json
        }).done(function (msg) {
            var arr = JSON.parse(msg);
            callback(arr);
        }).fail(function () {
            ApiBoundary.whenError(ApiBoundary.loadTaskStats, apiCallArgs);
        });
    },
    loadOfflineActivity: function (statInterval, userId, callback) {
        var jobj = {
            Begin : statInterval.from,
            End : statInterval.to,
            UserId : userId
        };
        var json = JSON.stringify(jobj);
        var apiCallArgs = arguments;
        $.ajax({
            url: "/api/stat/offlineactivity",
            type: 'POST',
            data: json
        }).done(function (msg) {
            var arr = JSON.parse(msg);
            callback(arr);
        }).fail(function () {
            ApiBoundary.whenError(ApiBoundary.loadOfflineActivity, apiCallArgs);
        });
    },
    getPluginsList : function (callback) {
        var apiCallArgs = arguments;
        $.ajax({
            url: "/api/config/plugins/get",
            type: 'GET'
        }).done(function (msg) {
            var obj = JSON.parse(msg);
            callback(obj);
        }).fail(function () {
            ApiBoundary.whenError(ApiBoundary.getPluginsList, apiCallArgs);
        });
    },
    uploadInstallPlugin: function (base64, callback) {
        var apiCallArgs = arguments;
        $.ajax({
            url: "/api/config/plugins/install",
            type: 'POST',
            data: base64
        }).done(function (msg) {
            var obj = JSON.parse(msg);
            callback(obj);
        }).fail(function () {
            ApiBoundary.whenError(ApiBoundary.uploadInstallPlugin, apiCallArgs);
        });
    },
    uninstallPlugin: function (guid, callback) {
        var obj = {
            Guid : guid
        };
        var json = JSON.stringify(obj);
        var apiCallArgs = arguments;
        $.ajax({
            url: "/api/config/plugins/uninstall",
            type: 'POST',
            data: json
        }).done(function () {
            callback();
        }).fail(function () {
            ApiBoundary.whenError(ApiBoundary.uninstallPlugin, apiCallArgs);
        });
    }

};