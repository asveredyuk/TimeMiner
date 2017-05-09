/**
 * Created by ALEX on 10.05.2017.
 */
//requires commonUtils.js
//requires moment.js
Handlebars.registerHelper("humanizeSeconds", function(value) {
    return CommonUtils.stringifyDurationExtended(moment.duration(value,'seconds'));
});