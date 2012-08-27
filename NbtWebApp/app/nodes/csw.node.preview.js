
/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';

    var preview;
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
                previewopts.delay = delay;
            }
            preview = Csw.nbt.nodePreview(Csw.literals.factory($('body')), previewopts);
            preview.open();
    }); // Csw.nodeHoverIn 


    Csw.nodeHoverOut = Csw.nodeHoverOut ||
        Csw.register('nodeHoverOut', function () {
            'use strict';
            if (false === Csw.isNullOrEmpty(preview)) {
                preview.close();
                preview = undefined;
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
                delay: 1500
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

            cswPublic.open = function (options) {
                cswPrivate.timeoutHandle = setTimeout(cswPrivate.loadPreview, cswPrivate.delay);
            }; // open()


            cswPublic.close = function () {
                clearTimeout(cswPrivate.timeoutHandle);
                
                // Clear all node previews, in case other ones are hanging around
                $('.CswNodePreview').remove();
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
