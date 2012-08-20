/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.actions.sessions = Csw.actions.sessions ||
        Csw.actions.register('sessions', function (cswParent, options) {
            var o = {
                urlMethod: 'getSessions',
                endSessionUrlMethod: 'endSession',
                ID: 'action_sessions'
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
                table.cell(row, 1).b({ text: 'End' });
                table.cell(row, 2).b({ text: 'Access ID' });
                table.cell(row, 3).b({ text: 'Username' });
                table.cell(row, 4).b({ text: 'Login Date' });
                table.cell(row, 5).b({ text: 'Timeout Date' });
                table.cell(row, 6).b({ text: 'Is Mobile' });
                table.cell(row, 7).b({ text: 'Session ID' });
                row += 1;

                // Sessions table
                Csw.ajax.post({
                    urlMethod: o.urlMethod,
                    data: {},
                    success: function (result) {

                        Csw.crawlObject(result, function (childObj) {
                            var cell2name = childObj.username;
                            var cell1 = table.cell(row, 1);
                            cell1.imageButton({
                                ButtonType: Csw.enums.imageButton_ButtonType.Fire,
                                AlternateText: 'Burn Session',
                                ID: o.ID + '_burn_' + childObj.sessionid,
                                onClick: Csw.makeDelegate(function (sessionid) { handleBurn(sessionid); }, childObj.sessionid)
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
                        }, false); // Csw.crawlObject()

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
