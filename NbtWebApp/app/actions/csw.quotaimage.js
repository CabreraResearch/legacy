
/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {

    Csw.actions.quotaImage = Csw.actions.quotaImage ||
        Csw.actions.register('quoteImage', function (cswParent, options) {
            'use strict';
            var o = {
                urlMethod: 'getQuotaPercent',
                name: 'action_quota_image',
                onSuccess: null
            };
            if (options) {
                Csw.extend(o, options);
            }

            return Csw.ajax.post({
                urlMethod: o.urlMethod,
                data: {},
                success: function (data) {
                    var percentUsed = Csw.number(data.result, 0);
                    var image = '';
                    cswParent.empty();
                    if (data.showquota) {
                        if (percentUsed <= 50) {
                            image = "good.gif";
                        } else if (percentUsed > 50 && percentUsed <= 75) {
                            image = "half.gif";
                        } else if (percentUsed > 75) {
                            image = "bad.gif";
                        }
                        cswParent.img({ src: 'Images/quota/' + image, title: 'Quota Usage: ' + percentUsed + '%' });
                    }
                    Csw.tryExec(o.onSuccess);
                } // success
            }); // ajax()

        });
}());
