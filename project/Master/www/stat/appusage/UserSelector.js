/**
 * Created by ALEX on 07.05.2017.
 */
function UserSelector(ctxt)
{
    var that = this;
    this.context = ctxt;

    ctxt.dropdown(
        {
            fullTextSearch : true,
            onChange : function(value, text, $choice){
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
    });

}
