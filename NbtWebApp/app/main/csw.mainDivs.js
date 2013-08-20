/// <reference path="CswApp-vsdoc.js" />

(function _initMain() {

    Csw.main.onReady.then(function() {
        Csw.main.body = Csw.main.body || Csw.main.register('body', Csw.domNode({ ID: 'body' })); // case 27563 review K3663 comment 1
        Csw.main.ajaxImage = Csw.main.ajaxImage || Csw.main.register('ajaxImage', Csw.domNode({ ID: 'ajaxImage' }));
        Csw.main.ajaxSpacer = Csw.main.ajaxSpacer || Csw.main.register('ajaxSpacer', Csw.domNode({ ID: 'ajaxSpacer' }));
        Csw.main.centerBottomDiv = Csw.main.centerBottomDiv || Csw.main.register('centerBottomDiv', Csw.domNode({ ID: 'CenterBottomDiv' }));
        Csw.main.centerTopDiv = Csw.main.centerTopDiv || Csw.main.register('centerTopDiv', Csw.domNode({ ID: 'CenterTopDiv' }));
        Csw.main.headerDashboard = Csw.main.headerDashboard || Csw.main.register('headerDashboard', Csw.domNode({ ID: 'header_dashboard' }));
        Csw.main.headerMenu = Csw.main.headerMenu || Csw.main.register('headerMenu', Csw.domNode({ ID: 'header_menu' }));
        Csw.main.headerQuota = Csw.main.headerQuota || Csw.main.register('headerQuota', Csw.domNode({ ID: 'header_quota' }));
        Csw.main.headerUsername = Csw.main.headerUsername || Csw.main.register('headerUsername', Csw.domNode({ ID: 'header_username' }));
        Csw.main.leftDiv = Csw.main.leftDiv || Csw.main.register('leftDiv', Csw.domNode({ ID: 'LeftDiv' }));
        Csw.main.mainMenuDiv = Csw.main.mainMenuDiv || Csw.main.register('mainMenuDiv', Csw.domNode({ ID: 'MainMenuDiv' }));
        Csw.main.rightDiv = Csw.main.rightDiv || Csw.main.register('rightDiv', Csw.domNode({ ID: 'RightDiv' }));
        Csw.main.searchDiv = Csw.main.searchDiv || Csw.main.register('searchDiv', Csw.domNode({ ID: 'SearchDiv' }));
        Csw.main.viewSelectDiv = Csw.main.viewSelectDiv || Csw.main.register('viewSelectDiv', Csw.domNode({ ID: 'ViewSelectDiv' }));
        Csw.main.watermark = Csw.main.watermark || Csw.main.register('watermark', Csw.domNode({ ID: 'watermark' }));
        

        return Csw.ajax.deprecatedWsNbt({
            urlMethod: 'getWatermark',
            success: function (result) {
                Csw.main.watermark.text(result.watermark || '');
            }
        });

    });
}());