/**
 * Created by ALEX on 07.05.2017.
 */
var roka;
function UserSelector(ctxt)
{
    roka = ctxt;
    var that = this;
    this.context = ctxt;
    ctxt.dropdown(
        {
            fullTextSearch : true,
            onChange : function(value, text, $choice){
                if(value != StatController.userId())
                    StatController.userId(value);
            }
        }
    );
    //load items
    var template = '<div class="item" data-value="{{Id}}"><i class="user icon"></i>{{Name}} {{Surname}}</div>';
    template = Handlebars.compile(template);
    var placement = ctxt.find(".contentPlacement");
    ApiBoundary.loadUsersList(function (arr) {
        $.each(arr, function(key, value){
            var item = $(template(value));
            placement.append(item);
        });
        ctxt.dropdown('refresh');
        ctxt.dropdown('set selected',StatController.userId());

    });

    //TODO: think if it is really needed
    // StatController.onUserIdChanged.add(function (value) {
    //     if(ctxt.dropdown('get value') != value) {
    //         ctxt.dropdown("set selected", value);
    //     }
    // });
}
