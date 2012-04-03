///// <reference path="~/js/CswNbt-vsdoc.js" />
///// <reference path="~/js/CswCommon-vsdoc.js" />


//(function ($) {
//    "use strict";
//    var pluginName = 'CswSessions';

//    var methods = {
//        init: function (options) {
//            var o = {
//                Url: '/NbtWebApp/wsNBT.asmx/getSessions',
//                EndSessionUrl: '/NbtWebApp/wsNBT.asmx/endSession',
//                ID: 'action_sessions'
//            };
//            if (options) $.extend(o, options);

//            var $Div = $(this);
//            var table;
//            var row;
//            
//            function initTable() {
//                $Div.contents().remove();
//                table = Csw.literals.table({
//                    $parent: $Div,
//                    ID: Csw.makeId(o.ID, 'tbl'),
//                    border: 1,
//                    cellpadding: 5
//                });
//                row = 1;

//                // Header row
//                table.cell(row, 1).append('<b>End</b>');
//                table.cell(row, 2).append('<b>Username</b>');
//                table.cell(row, 3).append('<b>Login Date</b>');
//                table.cell(row, 4).append('<b>Timeout Date</b>');
//                table.cell(row, 5).append('<b>Access ID</b>');
//                table.cell(row, 6).append('<b>Session ID</b>');
//                row += 1;

//                // Sessions table
//                Csw.ajax.post({
//                    url: o.Url,
//                    data: {},
//                    success: function (result) {

//                        Csw.crawlObject(result, function (childObj) {
//                            var cell2name = childObj.username;
//                            var cell1 = table.cell(row, 1);
//                            cell1.imageButton({ ButtonType: Csw.enums.imageButton_ButtonType.Fire,
//                                AlternateText: 'Burn Session',
//                                ID: o.ID + '_burn_' + childObj.sessionid,
//                                onClick: Csw.makeDelegate(function (sessionid) { handleBurn(sessionid); }, childObj.sessionid)
//                            });
//                            
//                            if (childObj.sessionid === Csw.cookie.get(Csw.cookie.cookieNames.SessionId)) {
//                                cell2name += "&nbsp;(you)";
//                            } 
//                            table.cell(row, 2).append(cell2name);
//                            table.cell(row, 3).text(childObj.logindate);
//                            table.cell(row, 4).text(childObj.timeoutdate);
//                            table.cell(row, 5).text(childObj.accessid);
//                            table.cell(row, 6).text(childObj.sessionid);

//                            row += 1;
//                        }, false); // Csw.crawlObject()

//                    } // success
//                }); // ajax()
//            } // initTable()

//            function handleBurn(sessionId) {
//                Csw.ajax.post({
//                    url: o.EndSessionUrl,
//                    data: { SessionId: sessionId },
//                    success: function () {
//                        initTable();
//                    }
//                });
//            } // handleBurn()

//            initTable();

//        } // init
//    }; // methods


//    // Method calling logic
//    $.fn.CswSessions = function (method) {

//        if (methods[method]) {
//            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
//        } else if (typeof method === 'object' || !method) {
//            return methods.init.apply(this, arguments);
//        } else {
//            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
//        }

//    };
//})(jQuery);
