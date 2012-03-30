/// <reference path="~/js/CswCommon-vsdoc.js" />
/// <reference path="~/js/CswNbt-vsdoc.js" />

(function () {

    Csw.actions.quotaImage = Csw.actions.quotaImage ||
        Csw.actions.register('quoteImage', function (cswParent, options) {
            'use strict';
            var o = {
                urlMethod: 'getQuotaPercent',
                ID: 'action_quota_image'
            };
            if (options) {
                $.extend(o, options);
            }

            Csw.ajax.post({
                urlMethod: o.urlMethod,
                data: {},
                success: function (data) {
                    var percentUsed = Csw.number(data.result, 0);
                    var image = '';
                    cswParent.empty();
                    if (percentUsed > 0) {
                        if (percentUsed <= 33) {
                            image = "good.gif";
                        } else if (percentUsed > 33 && percentUsed <= 66) {
                            image = "half.gif";
                        } else if (percentUsed > 66) {
                            image = "bad.gif";
                        }
                        cswParent.img({ src: 'Images/quota/' + image, title: 'Quota Usage: ' + percentUsed + '%' });
                    }
                } // success
            }); // ajax()

        });
} ());
