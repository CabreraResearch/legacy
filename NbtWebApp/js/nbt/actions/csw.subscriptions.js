/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.actions.subscriptions = Csw.actions.subscriptions ||
        Csw.actions.register('subscriptions', function (cswParent, options) {
            var o = {
                urlMethod: 'getSubscriptions',
                ID: 'action_subscriptions'
            };
            if (options) Csw.extend(o, options)

            var description,
                controlTables,
                notificationControl = { table: null, row: 1 },
                mailReportControl = { table: null, row: 1 };

            var initializeNotificationTable = function () {
                notificationControl.table = controlTables.cell(2, 1).table({
                    suffix: 'tbl',
                    border: 1,
                    cellpadding: 5
                });

                notificationControl.table.cell(notificationControl.row, 1).b({ text: 'Notification' });
                notificationControl.table.cell(notificationControl.row, 2).b({ text: 'Subscribe' });
                notificationControl.row += 1;
            }

            var initializeMailReportTable = function () {
                mailReportControl.table = controlTables.cell(2, 2).table({
                    suffix: 'tbl',
                    border: 1,
                    cellpadding: 5
                });

                mailReportControl.table.cell(mailReportControl.row, 1).b({ text: 'Mail Report' });
                mailReportControl.table.cell(mailReportControl.row, 2).b({ text: 'Subscribe' });
                mailReportControl.row += 1;
            }

            var initializeUI = function () {
                cswParent.br();
                
                controlTables = cswParent.table({
                    suffix: 'tbl',
                    cellpadding: 10
                });
                description = controlTables.cell(1,1).span({ text: "Edit Your Subscriptions to Notifications and Mail Reports:" });
                initializeNotificationTable();
                initializeMailReportTable();
                //ajax call - loading of two tables - one for notifications, and one for mail reports, with a CheckAll func below
                //save button, onClick ajax call - takes all checked items and subscribes them
            }

            initializeUI();

        });
} ());
