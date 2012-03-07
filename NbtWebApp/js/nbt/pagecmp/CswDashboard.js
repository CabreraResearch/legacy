/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/js/ChemSW-vsdoc.js" />

(function ($) {
    "use strict";
    $.fn.CswDashboard = function (options) {

        var o = {
            Url: '/NbtWebApp/wsNBT.asmx/getDashboard',
            onSuccess: function () { }
        };

        if (options) {
            $.extend(o, options);
        }

        var $DashDiv = $(this);
        
        Csw.ajax.post({
            url: o.Url,
            data: {},
            stringify: false,
            success: function (data) {

                var table = Csw.controls.table({
                    $parent: $DashDiv,
                    ID: Csw.controls.dom.makeId(o.ID, 'DashboardTable')
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




