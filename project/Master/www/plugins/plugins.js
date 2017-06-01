function RowWrapper(tbody, data, template){
    var that = this;
    this.data = data;
    this.row = $(template(data));
    tbody.append(this.row);
    var uninstallBt = this.row.find('.uninstallBt');
    var uninstallProgress = this.row.find('.uninstallBtProgress');
    uninstallBt.click(function () {
        uninstallBt.hide();
        uninstallProgress.show();
        ApiBoundary.uninstallPlugin(that.data.Guid, function () {
            //uninstallBt.show();
            //uninstallProgress.hide();
            window.location.reload();
        });
    });
}
function TableWrapper(ctxt)
{
    var that = this;
    this.tbody = ctxt.find('tbody');

    this.loader = that.tbody.find('tr');
    var loadingApplier = new DelayedApplier(
        function () {
            that.tbody.empty();
            that.tbody.append(that.loader);
        },
        function () {
            that.loader.detach();
            that.tbody.empty();
        },
        200
    );
    this.reloadTable = function () {
        loadingApplier.apply();
        ApiBoundary.getPluginsList(function (arr) {
            ApiBoundary.loadFrontendFile("/plugins/tablerow.hbs", function (tpl) {
                loadingApplier.disapply();
                var template = Handlebars.compile(tpl);
                $.each(arr, function (key, value) {
                    var r = new RowWrapper(that.tbody,value,template);
                })
            });
        });
    }
}
function InstallBtWrapper(ctxt, fakeInput)
{
    ctxt.click(function () {
        fakeInput.click();
    });
    fakeInput.change(function () {
        ctxt.addClass('loading');
        var file = fakeInput.get(0).files[0];
        if(file.name.substring(file.name.length - 4) != '.dll')
        {
            alert('Error: this is not dll file!');
            ctxt.removeClass('loading');
            return;
        }
        var reader = new FileReader();
        reader.readAsBinaryString(file);
        reader.onload = function () {
            var res = btoa(reader.result);
            ApiBoundary.uploadInstallPlugin(res, function (msg) {
                if(typeof msg.Error != 'undefined')
                {
                    ctxt.removeClass('loading');
                    alert("Failed to install, " + msg.Error);
                }
                else
                {
                    window.location.reload();
                }
            });
        };
        reader.onerror = function (error) {
            ctxt.removeClass('loading');
            alert('Error: ', error);
        };
    });
}
$(document).ready(function () {
    var table = new TableWrapper($('#pluginsTable'));
    var installBt = new InstallBtWrapper($('#pluginInstall'), $('#pluginInstallFakeFile'));
    table.reloadTable();
});