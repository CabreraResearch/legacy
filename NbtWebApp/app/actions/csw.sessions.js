/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.actions.sessions = Csw.actions.sessions ||
        Csw.actions.register('sessions', function (cswParent, options) {
            var o = {
                urlMethod: 'getSessions',
                endSessionUrlMethod: 'endSession' ,
                name: 'action_sessions'
            };
            if (options) Csw.extend(o, options);

            var table;
            var row;

            function initTable() {
                cswParent.empty();
                table = cswParent.table({
                    suffix: 'tbl',
                    border: 1,
                    cellpadding: 5
                });
                row = 1;

                // Header row
                table.cell(row, 1).span({ cssclass: 'CswThinGridHeaderShow', text: 'End' });
                table.cell(row, 2).span({ cssclass: 'CswThinGridHeaderShow', text: 'Access ID' });
                table.cell(row, 3).span({ cssclass: 'CswThinGridHeaderShow', text: 'Username' });
                table.cell(row, 4).span({ cssclass: 'CswThinGridHeaderShow', text: 'Login Date' });
                table.cell(row, 5).span({ cssclass: 'CswThinGridHeaderShow', text: 'Timeout Date' });
                table.cell(row, 6).span({ cssclass: 'CswThinGridHeaderShow', text: 'Is Mobile' });
                table.cell(row, 7).span({ cssclass: 'CswThinGridHeaderShow', text: 'Session ID' });
                row += 1;

                // Sessions table
                Csw.ajax.post({
                    urlMethod: o.urlMethod,
                    data: {},
                    success: function (result) {

                        Csw.eachRecursive(result, function (childObj) {
                            var cell2name = childObj.username;
                            var cell1 = table.cell(row, 1);
                            cell1.icon({
                                name: o.name + '_end_' + childObj.sessionid,
                                hovertext: 'End Session',
                                iconType: Csw.enums.iconType.x,
                                state: Csw.enums.iconState.normal,
                                isButton: true,
                                onClick: Csw.makeDelegate(function (sessionid) { handleBurn(sessionid); }, childObj.sessionid),
                                size: 16
                            });


                            if (childObj.sessionid === Csw.cookie.get(Csw.cookie.cookieNames.SessionId)) {
                                cell2name += "&nbsp;(you)";
                            }
                            table.cell(row, 2).text(childObj.accessid);
                            table.cell(row, 3).append(cell2name);
                            table.cell(row, 4).text(childObj.logindate);
                            table.cell(row, 5).text(childObj.timeoutdate);
                            table.cell(row, 6).text(childObj.ismobile);
                            table.cell(row, 7).text(childObj.sessionid);
                            row += 1;
                        }, false); // Csw.eachRecursive()

                    } // success
                }); // ajax()
            }

            // initTable()

            function handleBurn(sessionId) {
                Csw.ajax.post({
                    urlMethod: o.endSessionUrlMethod,
                    data: { SessionId: sessionId },
                    success: function () {
                        initTable();
                    }
                });
            }

            // handleBurn()

            initTable();

        });


} ());
