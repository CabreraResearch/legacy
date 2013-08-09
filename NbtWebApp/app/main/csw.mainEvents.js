/// <reference path="CswApp-vsdoc.js" />

(function _mainTearDown() {

    Csw.main.onReady.then(function() {
        
        Csw.subscribe(Csw.enums.events.main.refresh, Csw.main.refreshMain);

        Csw.main.register('refreshMain', function (eventObj, data) {
            Csw.clientChanges.unsetChanged();
            Csw.main.is.multi = false;
            Csw.main.clear({ all: true });
            return Csw.main.refreshSelected(data);
        });
        
        Csw.main.register('initGlobalEventTeardown', function () {
            Csw.unsubscribe('onAnyNodeButtonClick'); //omitting a function handle removes all
            Csw.unsubscribe('CswMultiEdit'); //omitting a function handle removes all
            Csw.unsubscribe('CswNodeDelete'); //omitting a function handle removes all
            Csw.publish('initPropertyTearDown');
            Csw.main.is.multi = false;
            Csw.main.is.oneTimeReset = true;
            Csw.clientChanges.unsetChanged();
        });

    });
}());