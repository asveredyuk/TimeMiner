/**
 * Created by ALEX on 13.04.2017.
 */
Handlebars.registerHelper("switch", function(value, options) {
    this._switch_value_ = value;
    var html = options.fn(this); // Process the body of the switch block
    delete this._switch_value_;
    return html;
});
Handlebars.registerHelper("case", function() {
    // Convert "arguments" to a real array - stackoverflow.com/a/4775938
    var args = Array.prototype.slice.call(arguments);

    var options    = args.pop();
    var caseValues = args;

    if (caseValues.indexOf(this._switch_value_) === -1) {
        return '';
    } else {
        return options.fn(this);
    }
});
/*
example
{{#switch state}}
    {{#case "page1" "page2"}}toolbar{{/case}}
    {{#case "page1" break=true}}page1{{/case}}
    {{#case "page2" break=true}}page2{{/case}}
    {{#case "page3" break=true}}page3{{/case}}
{{#default}}page0{{/default}}
{{/switch}}
 */