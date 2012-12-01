/// <reference path="~/app/CswApp-vsdoc.js" />


(function ($) {
    "use strict";
    $.fn.CswDashboard = function (options) {

        var o = {
            Url: 'getDashboard',
            onSuccess: function () { }
        };

        if (options) {
            Csw.extend(o, options);
        }

        var $DashDiv = $(this);
        
        Csw.ajax.post({
            urlMethod: o.Url,
            data: {},
            stringify: false,
            success: function (data) {

                var table = Csw.literals.table({
                    $parent: $DashDiv,
                    name: 'DashboardTable'
                });
                table.addClass('DashboardTable');

                var $tr = table.append('<tr />');

                for (var dashId in data) {
                    if (data.hasOwnProperty(dashId)) {
                        var thisIcon = data[dashId];
                        var cellcontent;
                        if (false === Csw.isNullOrEmpty( thisIcon.href )) {
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
                    }
                }
                o.onSuccess();

            } // success{}
        });

        // For proper chaining support
        return this;

    }; // function (options) {
})(jQuery);




