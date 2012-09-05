
/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';

    var previews = {};
    Csw.nodeHoverIn = Csw.nodeHoverIn || 
        Csw.register('nodeHoverIn', function (event, nodeid, cswnbtnodekey, delay) {
            'use strict';
            var previewopts = {
                ID: nodeid + '_preview',
                nodeid: nodeid,
                cswnbtnodekey: cswnbtnodekey,
                eventArg: event
            };
            if (Csw.number(delay, -1) >= 0) {
                previewopts.openDelay = delay;
            }
            previews[nodeid] = Csw.nbt.nodePreview(Csw.body, previewopts);
            previews[nodeid].open();
    }); // Csw.nodeHoverIn 


    Csw.nodeHoverOut = Csw.nodeHoverOut ||
        Csw.register('nodeHoverOut', function (event, nodeid) {
            'use strict';
            if (false === Csw.isNullOrEmpty(previews[nodeid])) {
                previews[nodeid].close(); 
                previews[nodeid] = undefined;
            }
    }); // Csw.nodeHoverOut


    Csw.nbt.nodePreview = Csw.nbt.nodePreview ||
        Csw.nbt.register('nodePreview', function (cswParent, options) {
            'use strict';
            var cswPrivate = {
                ID: '',
                nodeid: '',
                cswnbtnodekey: '',
                eventArg: {},
                openDelay: 1500,
                closeDelay: 500
            };
            Csw.extend(cswPrivate, options);
                
            var cswPublic = {};
            
            cswPrivate.fixDimensions = function() {
                // Make sure preview div is within the window
                var windowX = $(window).width() - 10;
                var windowY = $(window).height() - 10;
                var divwidth = cswPrivate.div.$.width();
                var divheight = cswPrivate.div.$.height();
                var X = cswPrivate.eventArg.pageX + 20; // move it to the right of the cursor, to keep from preventing click events
                var Y = cswPrivate.eventArg.pageY;

                if (X + divwidth > windowX) X = windowX - divwidth;
                // this doesn't work with page scrolling
                // if(Y + divheight > windowY) Y = windowY - divheight;

                cswPrivate.div.css({
                    maxWidth: windowX,
                    maxHeight: windowY,
                    top: Y + 'px',
                    left: X + 'px'
                });
                cswPrivate.div.css('z-index', '100');
            } // fixDimensions()

            cswPrivate.loadPreview = function() {
                cswPrivate.div.show();
                
                Csw.layouts.tabsAndProps(cswPrivate.div, {
                    ID: cswPrivate.ID + 'tabs',
                    nodeids: [cswPrivate.nodeid],
                    cswnbtnodekeys: [cswPrivate.cswnbtnodekey],
                    EditMode: Csw.enums.editMode.Preview,
                    AjaxWatchGlobal: false,
                    ShowAsReport: false,
                    onInitFinish: function (AtLeastOneProp) {
                        cswPrivate.loadingDiv.remove();
                        if (AtLeastOneProp) {
                            cswPrivate.fixDimensions();
                        } else {
                            cswPrivate.div.hide();
                        }
                    }
                });
            } // loadPreview()

            cswPrivate.hoverIn = function() {
                clearTimeout(cswPrivate.closeTimeoutHandle);
            }; // hoverIn()
            
            cswPrivate.hoverOut = function() {
                cswPublic.close();
            }; // hoverOut()

            
            cswPublic.open = function (options) {
                cswPrivate.openTimeoutHandle = Csw.defer(cswPrivate.loadPreview, cswPrivate.openDelay);
            }; // open()


            cswPublic.close = function () {
                clearTimeout(cswPrivate.openTimeoutHandle);
//                // Clear all node previews, in case other ones are hanging around
//                $('.CswNodePreview').remove();
                cswPrivate.closeTimeoutHandle = Csw.defer(function() { cswPrivate.div.remove(); }, cswPrivate.closeDelay);
            }; // close()

            // constructor
            (function() {
                cswPrivate.div = cswParent.div({ 
                        ID: cswPrivate.ID, 
                        cssclass: 'CswNodePreview' 
                    })
                    .css({
                        position: 'absolute',
                        overflow: 'auto',
                        border: '1px solid #003366',
                        padding: '2px',
                        backgroundColor: '#ffffff'
                    })
                    .hide();
                cswPrivate.div.$.hover(cswPrivate.hoverIn, cswPrivate.hoverOut);

                cswPrivate.fixDimensions();

                cswPrivate.loadingDiv = cswPrivate.div.div({ 
                        text: '&nbsp;&nbsp;&nbsp;Loading...',
                        styles: { 
                            fontStyle: 'italic'
                        }
                    });
            } ()); // constructor

            return cswPublic;
    }); // Csw.controls.nodeLink
} ());
