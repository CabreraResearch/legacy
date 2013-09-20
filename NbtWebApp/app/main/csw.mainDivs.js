/// <reference path="CswApp-vsdoc.js" />

(function _initMain() {

    Csw.main.onReady.then(function() {
        Csw.main.register('body', Csw.domNode({ ID: 'body' })); // case 27563 review K3663 comment 1
        Csw.main.register('ajaxImage', Csw.domNode({ ID: 'ajaxImage' }));
        Csw.main.register('ajaxSpacer', Csw.domNode({ ID: 'ajaxSpacer' }));
        Csw.main.register('centerBottomDiv', Csw.domNode({ ID: 'CenterBottomDiv' }));
        Csw.main.register('centerTopDiv', Csw.domNode({ ID: 'CenterTopDiv' }));
        Csw.main.register('headerDashboard', Csw.domNode({ ID: 'header_dashboard' }));
        Csw.main.register('headerMenu', Csw.domNode({ ID: 'header_menu' }));
        Csw.main.register('headerQuota', Csw.domNode({ ID: 'header_quota' }));
        Csw.main.register('headerUsername', Csw.domNode({ ID: 'header_username' }));
        Csw.main.register('leftDiv', Csw.domNode({ ID: 'LeftDiv' }));
        Csw.main.register('mainMenuDiv', Csw.domNode({ ID: 'MainMenuDiv' }));
        Csw.main.register('rightDiv', Csw.domNode({ ID: 'RightDiv' }));
        Csw.main.register('searchDiv', Csw.domNode({ ID: 'SearchDiv' }));
        Csw.main.register('viewSelectDiv', Csw.domNode({ ID: 'ViewSelectDiv' }));
        Csw.main.register('watermark', Csw.domNode({ ID: 'watermark' }));
        
        return Csw.ajax.deprecatedWsNbt({
            urlMethod: 'getWatermark',
            useCache: true,
            success: function (result) {
                var text = (result) ? result.watermark || '' : '';
                var watermarkImage = Csw.main.watermark.svg({
                    ID: 'watermark_svg',
                    width: document.width * .9,
                });
                watermarkImage.text({
                    text: text,
                    y: 200,
                    fontsize: 200,
                });
            }
        });

    });
}());