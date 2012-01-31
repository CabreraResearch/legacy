/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />


(function ($) { /// <param name="$" type="jQuery" />
    "use strict";        
    var pluginName = 'CswSessions';

    var methods = {
        init: function (options) {
            var o = {
                Url: '/NbtWebApp/wsNBT.asmx/getSessions',
                EndSessionUrl: '/NbtWebApp/wsNBT.asmx/endSession',
                ID: 'action_sessions'
            };
            if(options) $.extend(o, options);

            var $Div = $(this);
            var $table;
            var row;
            var $cell1, $cell2, $cell3, $cell4, $cell5, $cell6;

            function initTable()
            {
                $Div.contents().remove();
                $table = $Div.CswTable('init', { ID: o.ID + '_tbl', border: 1, cellpadding: 5 });
                row = 1;

                // Header row
                $cell1 = $table.CswTable('cell', row, 1);
                $cell1.append('<b>End</b>');
                $cell2 = $table.CswTable('cell', row, 2);
                $cell2.append('<b>Username</b>');
                $cell3 = $table.CswTable('cell', row, 3);
                $cell3.append('<b>Login Date</b>');
                $cell4 = $table.CswTable('cell', row, 4);
                $cell4.append('<b>Timeout Date</b>');
                $cell5 = $table.CswTable('cell', row, 5);
                $cell5.append('<b>Access ID</b>');
                $cell6 = $table.CswTable('cell', row, 6);
                $cell6.append('<b>Session ID</b>');
                row += 1;

                // Sessions table
                Csw.ajax({
                    url: o.Url,
                    data: {},
                    success: function (result) {

                        Csw.crawlObject(result, function (childObj) {
                            $cell1 = $table.CswTable('cell', row, 1);
                            $cell1.CswImageButton({ ButtonType: CswImageButton_ButtonType.Fire,
                                                    AlternateText: 'Burn Session',
                                                    ID: o.ID + '_burn_' + childObj.sessionid,
                                                    onClick: Csw.makeDelegate( function (sessionid) { handleBurn(sessionid); }, childObj.sessionid)
                                                });

                            $cell2 = $table.CswTable('cell', row, 2);
                            var cswCookie = Csw.cookie();
                            if(childObj.sessionid === cswCookie.get(cswCookie.cookieNames.SessionId)) {
                                $cell2.append(childObj.username + "&nbsp;(you)");
                            } else {
                                $cell2.append(childObj.username);
                            }

                            $cell3 = $table.CswTable('cell', row, 3);
                            $cell3.append(childObj.logindate);

                            $cell4 = $table.CswTable('cell', row, 4);
                            $cell4.append(childObj.timeoutdate);

                            $cell5 = $table.CswTable('cell', row, 5);
                            $cell5.append(childObj.accessid);

                            $cell6 = $table.CswTable('cell', row, 6);
                            $cell6.append(childObj.sessionid);

                            row += 1;
                        }, false); // Csw.crawlObject()

                    } // success
                }); // ajax()
            } // initTable()

            function handleBurn(sessionId)
            {
                Csw.ajax({
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
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName ); return false;
        }    
  
    };
})(jQuery);
