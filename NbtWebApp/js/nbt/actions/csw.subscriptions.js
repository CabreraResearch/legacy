/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.actions.subscriptions = Csw.actions.subscriptions ||
        Csw.actions.register('subscriptions', function (cswParent, options) {
            var o = {
                urlMethod: 'getSubscriptions',
                ID: 'action_subscriptions'
            };
            if (options) $.extend(o, options);

            cswParent.div().css({ 'text-align': 'center' }).span({ text: "Edit Your Subscriptions to Notifications and Mail Reports:" }).br();

            //ajax call - loading of two tables - one for notifications, and one for mail reports, with a CheckAll func below
            //save button, onClick ajax call - takes all checked items and subscribes them

        });
} ());
