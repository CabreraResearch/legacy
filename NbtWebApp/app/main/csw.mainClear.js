/// <reference path="CswApp-vsdoc.js" />

(function _initMain() {
    
    // see http://stackoverflow.com/questions/17544965/unhandled-rejection-reasons-should-be-empty
    Q.stopUnhandledRejectionTracking();
    
    Csw.main.onReady.then(function() {

        Csw.main.register('clear', function (options) {
            ///<summary>Clears the contents of the page.</summary>
            ///<param name="options">An object representing the elements to clear: all, left, right, centertop, centerbottom.</param>
            //if (debugOn()) Csw.debug.log('Main.clear()');
            
            var o = {
                left: false,
                right: false,
                centertop: false,
                centerbottom: false,
                all: false
            };
            if (options) {
                Csw.extend(o, options);
            }

            if (o.all || o.left) {
                Csw.main.leftDiv.empty();
                Csw.main.sidebarDiv.empty();
                Csw.main.leftDiv.show();
            }
            if (o.all || o.right) {
                Csw.main.rightDiv.empty();
            }
            if (o.all || o.centertop) {
                Csw.main.centerTopDiv.empty();
            }
            if (o.all || o.centerbottom) {
                Csw.main.centerBottomDiv.empty();
            }
            if (o.all) {
                Csw.main.mainMenuDiv.empty();
            }
            return true;
        });

        Csw.subscribe(Csw.enums.events.main.clear, function (eventObj, opts) {
            Csw.main.clear(opts);
        });

    });
}());