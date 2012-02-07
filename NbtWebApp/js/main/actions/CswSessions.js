/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />


(function ($) {
    "use strict";
    var pluginName = 'CswSessions';

    var methods = {
        init: function (options) {
            var o = {
                Url: '/NbtWebApp/wsNBT.asmx/getSessions',
                EndSessionUrl: '/NbtWebApp/wsNBT.asmx/endSession',
                ID: 'action_sessions'
            };
            if (options) $.extend(o, options);

            var $Div = $(this);
            var table;
            var row;
            
            function initTable() {
                $Div.contents().remove();
                table = Csw.controls.table({
                    $parent: $Div,
                    ID: Csw.controls.dom.makeId(o.ID, 'tbl'),
                    border: 1,
                    cellpadding: 5
                });
                row = 1;

                // Header row
                table.add(row, 1, '<b>End</b>');
                table.add(row, 2, '<b>Username</b>');
                table.add(row, 3, '<b>Login Date</b>');
                table.add(row, 4, '<b>Timeout Date</b>');
                table.add(row, 5, '<b>Access ID</b>');
                table.add(row, 6, '<b>Session ID</b>');
                row += 1;

                // Sessions table
                Csw.ajax.post({
                    url: o.Url,
                    data: {},
                    success: function (result) {

                        Csw.crawlObject(result, function (childObj) {
                            var cell2name = childObj.username;
                            var $cell1 = table.cell(row, 1);
                            $cell1.CswImageButton({ ButtonType: Csw.enums.imageButton_ButtonType.Fire,
                                AlternateText: 'Burn Session',
                                ID: o.ID + '_burn_' + childObj.sessionid,
                                onClick: Csw.makeDelegate(function (sessionid) { handleBurn(sessionid); }, childObj.sessionid)
                            });
                            
                            if (childObj.sessionid === Csw.cookie.get(Csw.cookie.cookieNames.SessionId)) {
                                cell2name += "&nbsp;(you)";
                            } 
                            table.add(row, 2, cell2name);
                            table.add(row, 3, childObj.logindate);
                            table.add(row, 4, childObj.timeoutdate);
                            table.add(row, 5, childObj.accessid);
                            table.add(row, 6, childObj.sessionid);

                            row += 1;
                        }, false); // Csw.crawlObject()

                    } // success
                }); // ajax()
            } // initTable()

            function handleBurn(sessionId) {
                Csw.ajax.post({
                    url: o.EndSessionUrl,
                    data: { SessionId: sessionId },
                    success: function () {
                        initTable();
                    }
                });
            } // handleBurn()

            initTable();

        } // init
    }; // methods


    // Method calling logic
    $.fn.CswSessions = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }

    };
})(jQuery);
