/// <reference path="CswApp-vsdoc.js" />

(function _initMain() {

    function refresh(onSuccess, data) {
        if (data) {
            Csw.main.headerDashboard.empty();

            var table = Csw.main.headerDashboard.table({
                name: 'DashboardTable'
            });
            table.addClass('DashboardTable');

            var $tr = table.append('<tr />');

            Csw.iterate(data, function(thisIcon, dashId) {
                var cellcontent;
                if (false === Csw.isNullOrEmpty(thisIcon.href)) {
                    cellcontent = '<td class="DashboardCell">' +
                        '  <a target="_blank" href="' + thisIcon.href + '">' +
                        '    <div title="' + thisIcon.text + '" id="' + dashId + '" class="' + dashId + '" />' +
                        '  </a>' +
                        '</td>';
                } else {
                    cellcontent = '<td class="DashboardCell">' +
                        '  <div title="' + thisIcon.text + '" id="' + dashId + '" class="' + dashId + '" />' +
                        '</td>';
                }
                $tr.append(cellcontent);
            });

            if (onSuccess) {
                onSuccess();
            }
        }
        return true;
    }

    Csw.main.onReady.then(function () {

        Csw.main.register('refreshDashboard', function(onSuccess) {
            
            function doRefresh(ret) {
                return refresh(onSuccess, ret);
            }

            return Csw.ajax.post({
                urlMethod: 'getDashboard',
                watchGlobal: false,
                useCache: true,
                success: doRefresh
            });
        });
    });
}());